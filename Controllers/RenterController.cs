using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RoomForRent.Infrastructure;
using RoomForRent.Models;
using RoomForRent.Models.ViewModel;

namespace RoomForRent.Controllers
{
    public class RenterController : Controller
    {
        private readonly IRenterRepository renterRepository;

        public RenterController(IRenterRepository renterRepository)
        {
            this.renterRepository = renterRepository;
        }

        private static int ItemsPerPage = 5;

        public async Task<IActionResult> Index(int pageCount)
        {
            if (pageCount <= 1)
                pageCount = 1;

            var renters = await this.renterRepository
                .Renters
                .Where(x => x.Found == null || x.Found == false)
                .Skip((pageCount - 1) * ItemsPerPage)
                .Take(ItemsPerPage)
                .ToListAsync();

            var renterViewInfo = new RenterListViewModel
            {
                Renters = renters,
                PagingInfo = new PagingInfo
                {
                    CurrentPage = pageCount,
                    ItemsPerPage = ItemsPerPage,
                    TotalItems = await this.renterRepository.Renters
                                .CountAsync(x => x.Found == null
                                || x.Found.Value == false)
                }
            };
            return View(renterViewInfo);
        }

        public async Task<IActionResult> History()
        {
            var pastRenters = await this.renterRepository
                .Renters
                .Where(x => x.Found == true)
                .ToListAsync();
            return View(pastRenters);
        }

        [HttpPost]
        public async Task<IActionResult> MarkAsFound(int renterId)
        {
            var renter = await this.renterRepository
                .Renters
                .Where(x => x.ID == renterId)
                .FirstOrDefaultAsync();

            if(renter != null)
            {
                renter.Found = true;
                renter.FoundDate = DateTime.Now;
                await this.renterRepository.SaveChangesAsync();
            }
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Details([FromRoute(Name ="id")] int renterId)
        {
            var renter = await this.renterRepository
                .Renters
                .Where(x => x.ID == renterId)
                .FirstOrDefaultAsync();
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
        public async Task<IActionResult> Create(RenterCreateModel renterCreateModel)
        {
            if(this.ModelState.IsValid)
            {
                Renter renter = CreateRenterObjectFromRenterCreateModel(renterCreateModel);
                this.renterRepository.AddRenter(renter);
                await this.renterRepository.SaveChangesAsync();

                return RedirectToAction("Index");
            }
            else
            {
                return RedirectToAction("Create");
            }
        }

        private static Renter CreateRenterObjectFromRenterCreateModel(RenterCreateModel renterCreateModel)
        {
            return new Renter
            {
                Name = renterCreateModel.Name,
                Address = renterCreateModel.Address,
                ContactNumber = renterCreateModel.Address,
                Description = renterCreateModel.Description,
                SeekedAsset = renterCreateModel.SeekedAsset,
            };
        }

        [ImportModelState]
        public async Task<IActionResult> EditDetails([FromRoute(Name = "id")] int renterId)
        {
            var renter = await this.renterRepository
                .Renters
                .Where(x => x.ID == renterId)
                .FirstOrDefaultAsync();

            // will use auto mapper later
            // for now manual mapping
            if(renter != null)
            {
                RenterEditModel renterEditModel = MapRenterObjectToRenterEditModel(renter);
                return View(renterEditModel);
            }
            else
            {
                return NotFound();
            }
        }

        private static RenterEditModel MapRenterObjectToRenterEditModel(Renter renter)
        {
            return new RenterEditModel
            {
                Name = renter.Name,
                ContactNumber = renter.ContactNumber,
                Address = renter.Address,
                Description = renter.Description,
                SeekedAsset = renter.SeekedAsset,
            };
        }

        [HttpPost]
        [ExportModelState]
        public async Task<IActionResult> EditDetails(RenterEditModel renterEditModel)
        {
            if (ModelState.IsValid)
            {
                var renterModel = await this.
                    renterRepository
                    .Renters
                    .Where(x => x.ID == renterEditModel.ID)
                    .FirstOrDefaultAsync();
                if (renterModel == null)
                {
                    return NotFound();
                }

                MapRenterEditModelToRenterObject(renterEditModel, renterModel);

                await this.renterRepository.SaveChangesAsync();

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
            renterModel.SeekedAsset = renterEditModel.SeekedAsset;
        }
    }
}
