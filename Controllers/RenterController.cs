using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using RoomForRent.Infrastructure;
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

        public IActionResult History()
        {
            var pastRenters = this.renterRepository
                .Renters
                .Where(x => x.Found == true);
            return View(pastRenters);
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
                renter.FoundDate = DateTime.Now;
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

        // custom attributed created for implementing
        // POST-REDIRECT-GET pattern
        [ImportModelState]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        // custom attributed created for implementing
        // POST-REDIRECT-GET pattern
        [ExportModelState]
        public IActionResult Create(RenterCreateModel renterCreateModel)
        {
            if(this.ModelState.IsValid)
            {
                var id = this.renterRepository.Renters.Count() + 1;
                Renter renter = CreateRenterObjectFromRenterCreateModel(renterCreateModel, id);
                this.renterRepository.AddRenter(renter);
                return RedirectToAction("Index");
            }
            else
            {
                return RedirectToAction("Create");
            }
        }

        private static Renter CreateRenterObjectFromRenterCreateModel(RenterCreateModel renterCreateModel, int id)
        {
            return new Renter
            {
                ID = id,
                Name = renterCreateModel.Name,
                Address = renterCreateModel.Address,
                ContactNumber = renterCreateModel.Address,
                Description = renterCreateModel.Description,
                SeekedAsset = renterCreateModel.SeekedAsset,
            };
        }

        [ImportModelState]
        public IActionResult EditDetails([FromRoute(Name = "id")] int renterId)
        {
            var renter = this.renterRepository
                .Renters
                .Where(x => x.ID == renterId)
                .FirstOrDefault();

            // will use auto mapper later
            // for now manual mapping
            if(renter != null)
            {
                RenterEditModel renterEditModel = MapRenterObjectToRenterEditModel(renterId, renter);
                return View(renterEditModel);
            }
            else
            {
                return NotFound();
            }
        }

        private static RenterEditModel MapRenterObjectToRenterEditModel(int renterId, Renter renter)
        {
            return new RenterEditModel
            {
                ID = renterId,
                Name = renter.Name,
                ContactNumber = renter.ContactNumber,
                Address = renter.Address,
                Description = renter.Description,
                SeekedAsset = renter.SeekedAsset,
            };
        }

        [HttpPost]
        [ExportModelState]
        public IActionResult EditDetails(RenterEditModel renterEditModel)
        {
            if (ModelState.IsValid)
            {
                var renterModel = this.
                    renterRepository
                    .Renters
                    .Where(x => x.ID == renterEditModel.ID)
                    .FirstOrDefault();
                if (renterModel == null)
                {
                    return NotFound();
                }

                MapRenterEditModelToRenterObject(renterEditModel, renterModel);

                return RedirectToAction("Index");
            }
            return RedirectToAction("EditDetails", new { id = renterEditModel.ID });
        }

        private static void MapRenterEditModelToRenterObject(RenterEditModel renterEditModel, Renter renterModel)
        {
            renterModel.Name = renterEditModel.Name;
            renterModel.Address = renterEditModel.Address;
            renterModel.ContactNumber = renterEditModel.ContactNumber;
            renterModel.Description = renterEditModel.Description;
            renterEditModel.SeekedAsset = renterEditModel.SeekedAsset;
        }
    }
}
