﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using RoomForRent.Infrastructure;
using RoomForRent.Models;
using RoomForRent.Models.LeaserModels;
using RoomForRent.Models.ViewModel;
using RoomForRent.Repositories;
using RoomForRent.Services.LeaserServiceProvider;
using RoomForRent.Services.RenterLeaserTransactionServiceProvider;

namespace RoomForRent.Controllers
{
    [Authorize(Roles = "Admin")]
    public class LeaserController : Controller
    {
        private readonly IRepositoryWrapper repositoryWrapper;
        private readonly LeaserService leaserService = null;
        private readonly IOptions<RoomForRentOptions> optionsAccessor;

        private readonly RenterLeaserTransactionService transactionService = null;

        public LeaserController(IOptions<RoomForRentOptions> optionsAccessor, IRepositoryWrapper repositoryWrapper)
        {
            this.repositoryWrapper = repositoryWrapper;

            leaserService = new LeaserService(this.repositoryWrapper.LeaserRepository);

            transactionService = new RenterLeaserTransactionService(this.repositoryWrapper);

            this.optionsAccessor = optionsAccessor;
            TransactionRepository = this.repositoryWrapper.TransactionRepository;
            this.ItemsPerPage = this.optionsAccessor.Value.ItemsPerPage;
        }
        
        private readonly int ItemsPerPage;

        public ITransactionRepository TransactionRepository { get; }

        public async Task<IActionResult> Index(int pageCount, [FromQuery]string leaserName = null) {
            if (pageCount <= 1)
                pageCount = 1;

            if(!string.IsNullOrWhiteSpace(leaserName))
            {
                var (activeLeasers,activeLeasersCount) = await this.leaserService
                    .GetLeasersByNameAsync(leaserName, pageCount, ItemsPerPage);

                var leaserByNameViewInfo = new LeaserListViewModel
                {
                    CurrentLeaser = leaserName,
                    Leasers = activeLeasers,
                    PagingInfo = new PagingInfo
                    {
                        CurrentPage = pageCount,
                        ItemsPerPage = ItemsPerPage,
                        TotalItems = activeLeasersCount
                    }
                };
                return View(leaserByNameViewInfo);
            }

            var (leasers, totalActiveLeasers) = await this.leaserService.GetLeasers(pageCount,ItemsPerPage);

            var leaserViewInfo = new LeaserListViewModel
            {
                Leasers = leasers,
                PagingInfo = new PagingInfo
                {
                    CurrentPage = pageCount,
                    ItemsPerPage = ItemsPerPage,
                    TotalItems = totalActiveLeasers
                }
            };
            return View(leaserViewInfo);
        }

        // custom attribute created for implementing
        // POST-REDIRECT-GET pattern
        [ImportModelState]
        public async Task<IActionResult> EditDetails([FromRoute(Name = "id")] int leaserId)
        {
            var leaserEditDetails = await this.leaserService.GetLeaserEditDetails(leaserId);

            if(leaserEditDetails == null)
                return NotFound();

            return View(leaserEditDetails);
        }

       

        [HttpPost]
        // custom attribute created for implementing
        // POST-REDIRECT-GET pattern
        [ExportModelState]
        public async Task<IActionResult> EditDetails(LeaserEditModel leaserEditModel)
        {
            if(ModelState.IsValid)
            {
                var result = await this.leaserService.UpdateLeaserEditDetails(leaserEditModel);

                if(result == false)
                {
                    // should redirect to custom error page
                    // for now show as badrequest
                    return BadRequest();
                }

                return RedirectToAction("Index");
            }
            return RedirectToAction("EditDetails", new { id = leaserEditModel.ID });
        }

        public async Task<IActionResult> Details([FromRoute(Name ="id")] int leaserId)
        {
            var leaser = await this.leaserService.GetLeaserDetails(leaserId);

            return View(leaser);
        }

        public async Task<IActionResult> History(int pageCount)
        {
            if (pageCount < 1)
                pageCount = 1;

            var (pastLeasers, totalCount) = await this.leaserService.GetLeaserHistory(pageCount, ItemsPerPage);

            var leaserHistoryViewModel = new LeaserHistoryViewModel
            {
                Leasers = pastLeasers,
                PagingInfo = new PagingInfo
                {
                    CurrentPage = pageCount,
                    ItemsPerPage = ItemsPerPage,
                    TotalItems = totalCount
                }
            };
            return View(leaserHistoryViewModel);
        }

        [HttpPost]
        public async Task<IActionResult> MarkAsLeased(int leaserId) {

            var result = await this.leaserService.MarkAsLeased(leaserId);

            if(result == true)
                return RedirectToAction("Index");

            // should display custom error page
            // for now show bad request
            return BadRequest();
        }

        [HttpPost]
        public JsonResult CancelLease([FromForm] int leaserId)
        {
            var result = this.transactionService.CancelLeaserTransaction(leaserId);
            return new JsonResult(new { LeaserId = leaserId, IsSuccess = result });
        }

        // custom attribute created for implementing
        // POST-REDIRECT-GET pattern
        [ImportModelState]
        public IActionResult Create() {
            return View(new LeaserCreateModel());
        }

        [HttpPost]
        // custom attribute created for implementing
        // POST-REDIRECT-GET pattern
        [ExportModelState]
        public async Task<IActionResult> Create(LeaserCreateModel leaserCreateModel)
        {
            if (this.ModelState.IsValid)
            {
                await this.leaserService.CreateLeaser(leaserCreateModel);
                return RedirectToAction("Index");
            }
            else
            {
                return RedirectToAction("Create");
            }
        }
    }
}
