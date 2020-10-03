using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RoomForRent.Helpers;
using RoomForRent.Models;
using RoomForRent.Models.ViewModel;
using RoomForRent.Repositories;
using RoomForRent.Services.RenterLeaserTransactionServiceProvider;

namespace RoomForRent.Controllers
{
    [Authorize(Roles = "Admin")]
    public class HomeController : Controller
    {
        private readonly ITransactionRepository transactionRepository;

        private readonly RenterLeaserTransactionService renterLeaserTransactionService = null;

        public HomeController(IRepositoryWrapper repositoryWrapper)
        {
            this.transactionRepository = repositoryWrapper.TransactionRepository;
            renterLeaserTransactionService = new RenterLeaserTransactionService(repositoryWrapper);
        }

        public IActionResult Index()
        {
            //if(!HttpContext.User.Identity.IsAuthenticated)
            //{
            //    return RedirectToAction("Login", "Account", 
            //        new { returnUrl = this.HttpContext.Request.Path });
            //}

            var transactions = renterLeaserTransactionService.GetTransactions();
            return View(transactions);
        }

        public async Task<IActionResult> LinkToLeaser([FromRoute(Name ="id")] int renterId)
        {
            RenterLeasersLinkDto linkDto 
                = await renterLeaserTransactionService.GetPotentialLeasersDto(renterId);
            return View(linkDto);
        }

        [HttpPost]
        public async Task<IActionResult> LinkToLeaser([FromForm(Name = "leaserId")] int leaserId, 
                [FromForm(Name = "renterId")] int renterId)
        {
            var result = await this.renterLeaserTransactionService
                .LinkRenterAndLeaser(leaserId, renterId);

            if (result == true)
                return RedirectToAction(nameof(Index));

            return BadRequest();
        }

        public async Task<IActionResult> LinkToRenter([FromRoute(Name ="id")] int leaserId)
        {
            LeaserRentersLinkDto linkDto = 
                await renterLeaserTransactionService.GetPotentialRentersDto(leaserId);
            return View(linkDto);
        }

        [HttpPost]
        public async Task<IActionResult> LinkToRenter([FromForm(Name = "leaserId")] int leaserId, [FromForm(Name ="renterId")] int renterId)
        {
            var result = await this.renterLeaserTransactionService
                .LinkRenterAndLeaser(leaserId, renterId);

            if(result == true)
                return RedirectToAction(nameof(Index));

            return BadRequest();
        }

        [HttpPost]
        public async Task<IActionResult> UpdateTransactionStatus([FromRoute(Name ="id")] int transactionId, 
                [FromForm(Name = "transactionStatus")] RenterLeaserTransactionStatus transactionStatus)
        {
            var result = await this.renterLeaserTransactionService
                .UpdateTransaction(transactionId, transactionStatus);
            if(result == false)
            {
                return BadRequest();
            }

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> CreateTransaction(int? leaserId, int? renterId )
        {
            var renterLeaserTransactionCreateDto = await
                renterLeaserTransactionService.GetDataForTransactionLink(leaserId, renterId);

            return View(renterLeaserTransactionCreateDto);
        }

        [HttpPost]
        public async Task<IActionResult> CreateTransaction(int leaserId, int renterId)
        {
            var result = await this.renterLeaserTransactionService
                .LinkRenterAndLeaser(leaserId, renterId);

            if (result == true)
                return RedirectToAction(nameof(Index));

            return BadRequest();
        }
    }
}
