using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RoomForRent.DataAccessLayer;
using RoomForRent.Models;
using RoomForRent.Models.ViewModel;

namespace RoomForRent.Controllers
{
    public class HomeController : Controller
    {
        private readonly ITransactionRepository transactionRepository;

        private readonly RenterLeaserTransactionDAL renterLeaserTransactionDAL = null;

        public HomeController(ITransactionRepository transactionRepository)
        {
            this.transactionRepository = transactionRepository;
            renterLeaserTransactionDAL = new RenterLeaserTransactionDAL(transactionRepository);
        }

        public IActionResult Index()
        {
            var transactions = renterLeaserTransactionDAL.GetTransactions();
            return View(transactions);
        }

        public IActionResult LinkToLeaser([FromRoute(Name ="id")] int renterId)
        {
            RenterLeasersLinkDto linkDto = renterLeaserTransactionDAL.GetRenterLeasersLinkDto(renterId);
            return View(linkDto);
        }

        [HttpPost]
        public IActionResult LinkToLeaser([FromForm(Name = "leaserId")] int leaserId, 
                [FromForm(Name = "renterId")] int renterId)
        {
            var result = this.renterLeaserTransactionDAL
                .LinkRenterAndLeaser(leaserId, renterId);

            if (result == true)
                return RedirectToAction(nameof(Index));

            return NotFound();
        }

        public IActionResult LinkToRenter([FromRoute(Name ="id")] int leaserId)
        {
            LeaserRentersLinkDto linkDto = renterLeaserTransactionDAL.GetLeaserRentersLinkDto(leaserId);
            return View(linkDto);
        }

        [HttpPost]
        public IActionResult LinkToRenter([FromForm(Name = "leaserId")] int leaserId, [FromForm(Name ="renterId")] int renterId)
        {
            var result = this.renterLeaserTransactionDAL
                .LinkRenterAndLeaser(leaserId, renterId);

            if(result == true)
                return RedirectToAction(nameof(Index));

            return NotFound();
        }

        [HttpPost]
        public IActionResult UpdateTransactionStatus([FromRoute(Name ="id")] int transactionId, 
                [FromForm(Name = "transactionStatus")] RenterLeaserTransactionStatus transactionStatus)
        {
            var result = this.renterLeaserTransactionDAL
                .UpdateTransaction(transactionId, transactionStatus);
            if(result == false)
            {
                return NotFound();
            }

            return RedirectToAction(nameof(Index));
        }

        public IActionResult CreateTransaction(int? leaserId, int? renterId )
        {
            var renterLeaserTransactionCreateDto = 
                renterLeaserTransactionDAL.GetDataForTransactionLink(leaserId, renterId);

            return View(renterLeaserTransactionCreateDto);
        }

        [HttpPost]
        public IActionResult CreateTransaction(int leaserId, int renterId)
        {
            var result = this.renterLeaserTransactionDAL
                .LinkRenterAndLeaser(leaserId, renterId);

            if (result == true)
                return RedirectToAction(nameof(Index));

            return NotFound();
        }
    }
}
