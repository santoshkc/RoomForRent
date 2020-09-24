using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using RoomForRent.Infrastructure;
using RoomForRent.Models;
using RoomForRent.Models.ViewModel;
using RoomForRent.Repositories;
using RoomForRent.Services.RenterServiceProvider;

namespace RoomForRent.Controllers
{
    [Authorize(Roles = "Admin")]
    public class RenterController : Controller
    {
        private readonly IOptions<RoomForRentOptions> optionsAccessor;

        private readonly IRenterRepository renterRepository;

        private readonly RenterService renterService = null;

        public RenterController(IOptions<RoomForRentOptions> optionsAccessor, IRenterRepository renterRepository)
        {
            this.optionsAccessor = optionsAccessor;
            this.ItemsPerPage = optionsAccessor.Value.ItemsPerPage;

            this.renterRepository = renterRepository;
            renterService = new RenterService(renterRepository);
        }

        private readonly int ItemsPerPage;

        public async Task<IActionResult> Index(int pageCount,string renterName=null)
        {
            if (pageCount <= 1)
                pageCount = 1;

            if(string.IsNullOrWhiteSpace(renterName) == false)
            {
                var renters = await this.renterRepository
                                .GetRentersByName(renterName, pageCount, this.ItemsPerPage);

                var renterViewInfo = new RenterListViewModel
                {
                    CurrentRenter = renterName,
                    Renters = renters,
                    PagingInfo = new PagingInfo
                    {
                        CurrentPage = pageCount,
                        ItemsPerPage = this.ItemsPerPage,
                        TotalItems = await this.renterRepository.GetRentersByNameCount(renterName,false)
                    }
                };
                return View(renterViewInfo);
            } 
            else
            {
                var renterViewInfo = await renterService
                    .GetRenterList(pageCount, ItemsPerPage);
                return View(renterViewInfo);
            }
        }

        public async Task<IActionResult> History()
        {
            var pastRenters = await renterService.GetHistory();

            return View(pastRenters);
        }

        [HttpPost]
        public async Task<IActionResult> MarkAsFound(int renterId)
        {
            var success = await renterService.MarkAsFound(renterId);
            if (success)
                return RedirectToAction("Index");
            else
                return BadRequest();
        }

        public async Task<IActionResult> Details([FromRoute(Name ="id")] int renterId)
        {
            var renter = await renterService.GetRenterDetails(renterId);
            return View(renter);
        }

        // custom attributed created for implementing
        // POST-REDIRECT-GET pattern
        [ImportModelState]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        // custom attribute created for implementing
        // POST-REDIRECT-GET pattern
        [ExportModelState]
        public async Task<IActionResult> Create(RenterCreateModel renterCreateModel)
        {
            if(this.ModelState.IsValid)
            {
                var result = await renterService.CreateRenter(renterCreateModel);

                return RedirectToAction("Index");
            }
            else
            {
                return RedirectToAction("Create");
            }
        }

        [ImportModelState]
        public async Task<IActionResult> EditDetails([FromRoute(Name = "id")] int renterId)
        {
            var renterEditModel = await renterService.GetRenterEditDetails(renterId);

            if(renterEditModel != null)
                return View(renterEditModel);
            else
                return NotFound();
        }

        [HttpPost]
        [ExportModelState]
        public async Task<IActionResult> EditDetails(RenterEditModel renterEditModel)
        {
            if (ModelState.IsValid)
            {
                var result = await renterService.EditRenterDetails(renterEditModel);

                if (result == true)
                    return RedirectToAction("Index");
                else
                    return BadRequest();
            }

            return RedirectToAction("EditDetails", new { id = renterEditModel.ID });
        }
    }
}
