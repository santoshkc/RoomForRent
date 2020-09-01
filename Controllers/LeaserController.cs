using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using RoomForRent.Infrastructure;
using RoomForRent.Models;
using RoomForRent.Models.ViewModel;

namespace RoomForRent.Controllers
{
    public class LeaserController : Controller
    {
        private readonly ILeaserRepository leaserRepository;

        public LeaserController(ILeaserRepository roomLeaserRepository) {
            this.leaserRepository = roomLeaserRepository;
        }
        
        private static int ItemsPerPage = 5;

        public IActionResult Index(int pageCount) {
            if (pageCount <= 1)
                pageCount = 1;

            var leasers = this.leaserRepository
                .Leaser
                .Where(x => x.AssetInfo.IsLeased == null 
                || x.AssetInfo.IsLeased== false)
                .Skip( (pageCount-1) * ItemsPerPage)
                .Take(ItemsPerPage)
                .ToList();

            var leaserViewInfo = new LeaserListViewModel
            {
                Leasers = leasers,
                PagingInfo = new PagingInfo
                {
                    CurrentPage = pageCount,
                    ItemsPerPage = LeaserController.ItemsPerPage,
                    TotalItems = this.leaserRepository.Leaser
                                .Count(x => x.AssetInfo.IsLeased == null
                                || x.AssetInfo.IsLeased == false)
                }
            };
            return View(leaserViewInfo);
        }

        // custom attribute created for implementing
        // POST-REDIRECT-GET pattern
        [ImportModelState]
        public IActionResult EditDetails([FromRoute(Name = "id")] int leaserId)
        {
            var leaser = this.leaserRepository
                .Leaser
                .Where(x => x.ID == leaserId)
                .FirstOrDefault();
            return View(leaser);
        }

        [HttpPost]
        // custom attribute created for implementing
        // POST-REDIRECT-GET pattern
        [ExportModelState]
        public IActionResult EditDetails(Leaser leaser)
        {
            if(ModelState.IsValid)
            {
                return RedirectToAction("Index");
            }
            return RedirectToAction("EditDetails", new { id = leaser.ID });
        }

        public IActionResult Details([FromRoute(Name ="id")] int leaserId)
        {
            var leaser = this.leaserRepository
                .Leaser
                .Where(x => x.ID == leaserId).FirstOrDefault();

            return View(leaser);
        }

        public IActionResult History(int pageCount)
        {
            if (pageCount < 1)
                pageCount = 1;

            var pastLeasers = this.leaserRepository.
                Leaser
                .Where(x => x.AssetInfo.IsLeased == true)
                .Skip( (pageCount - 1) * ItemsPerPage)
                .Take(ItemsPerPage)
                .ToList();

            var leaserHistoryViewModel = new LeaserHistoryViewModel
            {
                Leasers = pastLeasers,
                PagingInfo = new PagingInfo
                {
                    CurrentPage = pageCount,
                    ItemsPerPage = ItemsPerPage,
                    TotalItems = this.leaserRepository.
                    Leaser.Count(x => x.AssetInfo.IsLeased == true)
                }
            };
            return View(leaserHistoryViewModel);
        }

        [HttpPost]
        public IActionResult MarkAsLeased(int leaserId) {
            var leaser = this.leaserRepository.Leaser
                .Where(x => x.ID == leaserId).FirstOrDefault();
            if(leaser != null)
            {
                leaser.AssetInfo.IsLeased = true;
                leaser.AssetInfo.LeasedDate = DateTime.Now;
            }
            return RedirectToAction("Index");
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
        public IActionResult Create(LeaserCreateModel leaserCreateModel)
        {
            if (this.ModelState.IsValid)
            {
                var id = this.leaserRepository.Leaser.Count() + 1;
                Leaser leaser = CreateLeaserFromLeaserCreateModel(leaserCreateModel, id);
                this.leaserRepository.AddLeaser(leaser);
                return RedirectToAction("Index");
            }
            else
            {
                return RedirectToAction("Create");
            }
        }

        private static Leaser CreateLeaserFromLeaserCreateModel(LeaserCreateModel leaserCreateModel, int id)
        {
            return new Leaser
            {
                ID = id,
                Name = leaserCreateModel.Name,
                Address = leaserCreateModel.Address,
                ContactNumber = leaserCreateModel.ContactNumber,
                AssetInfo = new Asset
                {
                    Description = leaserCreateModel.Description,
                    Location = leaserCreateModel.AssetLocation,
                    Type = leaserCreateModel.AssetType,
                }
            };
        }
    }
}
