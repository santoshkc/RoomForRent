using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RoomForRent.Infrastructure;
using RoomForRent.Models;
using RoomForRent.Models.ViewModel;
using RoomForRent.Repositories;
using RoomForRent.Services.RenterServiceProvider;

namespace RoomForRent.Controllers
{
    public class RenterController : Controller
    {
        private readonly IRenterRepository renterRepository;

        private readonly RenterService renterService = null;

        public RenterController(IRenterRepository renterRepository)
        {
            this.renterRepository = renterRepository;
            renterService = new RenterService(renterRepository);

        }

        private static int ItemsPerPage = 5;

        public async Task<IActionResult> Index(int pageCount)
        {
            if (pageCount <= 1)
                pageCount = 1;

            var renterViewInfo = await renterService.GetRenterList(pageCount,ItemsPerPage);

            return View(renterViewInfo);
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
            var renter = await this.renterRepository
                .Renters
                .Where(x => x.ID == renterId)
                .FirstOrDefaultAsync();

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
