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
        private readonly IRenterRepository roomRepository;

        public HomeController(IRenterRepository roomRepository)
        {
            this.roomRepository = roomRepository;
        }

        public IActionResult Index()
        {
            var roomsAvailable = this.roomRepository.Renters;
            return View(roomsAvailable);
        }
    }
}
