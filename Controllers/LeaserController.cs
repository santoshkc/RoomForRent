using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
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

        public IActionResult Create() {
            return View();
        }

        [HttpPost]
        public IActionResult Create(Leaser roomLeaser)
        {
            if (this.ModelState.IsValid)
            {
                var id = this.leaserRepository.Leaser.Count() + 1;
                roomLeaser.ID = id;
                this.leaserRepository.AddLeaser(roomLeaser);
                return RedirectToAction("Index");
            }
            else
            {
                return View();
            }
        }
    }
}
