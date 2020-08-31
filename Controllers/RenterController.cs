using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using RoomForRent.Models;

namespace RoomForRent.Controllers
{
    public class RenterController : Controller
    {
        private readonly IRenterRepository renterRepository;

        public RenterController(IRenterRepository renterRepository)
        {
            this.renterRepository = renterRepository;
        }

        public IActionResult Index()
        {
            var renters = this.renterRepository
                .Renters
                .Where(x => x.Found == null || x.Found == false);
            return View(renters);
        }

        [HttpPost]
        public IActionResult MarkAsFound(int renterId)
        {
            var renter = this.renterRepository
                .Renters
                .Where(x => x.ID == renterId)
                .FirstOrDefault();

            if(renter != null)
            {
                renter.Found = true;
            }
            return RedirectToAction("Index");
        }

        public IActionResult Details([FromRoute(Name ="id")] int renterId)
        {
            var renter = this.renterRepository
                .Renters
                .Where(x => x.ID == renterId)
                .FirstOrDefault();
            return View(renter);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(Renter renter)
        {
            if(this.ModelState.IsValid)
            {
                var id = this.renterRepository.Renters.Count() + 1;
                renter.ID = id;
                this.renterRepository.AddRenter(renter);
                return RedirectToAction("Index");
            }
            else
            {
                return View();
            }
        }
    }
}
