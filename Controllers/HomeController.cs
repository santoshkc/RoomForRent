using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using RoomForRent.Models;

namespace RoomForRent.Controllers
{
    public class HomeController : Controller
    {
        private readonly ITransactionRepository transactionRepository;

        private readonly DataAccessLayer.RenterLeaserTransactionDAL renterLeaserTransactionDAL = null;

        public HomeController(ITransactionRepository transactionRepository)
        {
            this.transactionRepository = transactionRepository;

            renterLeaserTransactionDAL = new DataAccessLayer.RenterLeaserTransactionDAL(transactionRepository);
        }

        public IActionResult Index()
        {
            var transactions = renterLeaserTransactionDAL.GetTransactions();
            return View(transactions);
        }

        [HttpPost]
        public IActionResult UpdateTransactionStatus([FromRoute(Name ="id")] int transactionId, [FromForm(Name = "transactionStatus")] RenterLeaserTransactionStatus transactionStatus)
        {
           var result = this.renterLeaserTransactionDAL.UpdateTransaction(transactionId, transactionStatus);
            if(result == false)
            {
                return NotFound();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
