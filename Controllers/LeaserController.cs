﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using RoomForRent.Models;

namespace RoomForRent.Controllers
{
    public class LeaserController : Controller
    {
        private readonly ILeaserRepository leaserRepository;

        public LeaserController(ILeaserRepository roomLeaserRepository) {
            this.leaserRepository = roomLeaserRepository;
        }

        public IActionResult Index() {
            var leasers = this.leaserRepository
                .Leaser
                .Where(x => x.AssetInfo.IsLeased == null 
                || x.AssetInfo.IsLeased== false);
            return View(leasers);
        }

        public IActionResult Details([FromRoute(Name ="id")] int leaserId)
        {
            var leaser = this.leaserRepository
                .Leaser
                .Where(x => x.ID == leaserId).FirstOrDefault();

            return View(leaser);
        }

        public IActionResult History()
        {
            var pastLeasers = this.leaserRepository.
                Leaser
                .Where(x => x.AssetInfo.IsLeased == true);
            return View(pastLeasers);
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
