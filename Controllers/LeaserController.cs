using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RoomForRent.Infrastructure;
using RoomForRent.Models;
using RoomForRent.Models.LeaserModels;
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

        public async Task<IActionResult> Index(int pageCount) {
            if (pageCount <= 1)
                pageCount = 1;

            var leasers = await this.leaserRepository
                .Leaser
                .Include( x => x.AssetInfo)
                .Where(x => x.AssetInfo.IsLeased == null 
                || x.AssetInfo.IsLeased== false)
                .Skip( (pageCount-1) * ItemsPerPage)
                .Take(ItemsPerPage)
                .ToListAsync();

            var leaserViewInfo = new LeaserListViewModel
            {
                Leasers = leasers,
                PagingInfo = new PagingInfo
                {
                    CurrentPage = pageCount,
                    ItemsPerPage = LeaserController.ItemsPerPage,
                    TotalItems = await this.leaserRepository.Leaser
                                 .Include(x => x.AssetInfo)
                                .CountAsync(x => x.AssetInfo.IsLeased == null
                                || x.AssetInfo.IsLeased == false)
                }
            };
            return View(leaserViewInfo);
        }

        // custom attribute created for implementing
        // POST-REDIRECT-GET pattern
        [ImportModelState]
        public async Task<IActionResult> EditDetails([FromRoute(Name = "id")] int leaserId)
        {
            var leaser = await this.leaserRepository
                .Leaser
                .Where(x => x.ID == leaserId)
                .Include(x => x.AssetInfo)
                .FirstOrDefaultAsync();

            if (leaser == null)
                return NotFound();

            LeaserEditModel leaserEditModel = CreateLeaserEditModelFromLeaser(leaser);
            return View(leaserEditModel);
        }

        private static LeaserEditModel CreateLeaserEditModelFromLeaser(Leaser leaser)
        {
            return new LeaserEditModel
            {
                ID = leaser.ID,
                Name = leaser.Name,
                Address = leaser.Address,
                ContactNumber = leaser.ContactNumber,
                AssetLocation = leaser.AssetInfo.Location,
                AssetType = leaser.AssetInfo.Type,
                Description = leaser.AssetInfo.Description
            };
        }

        [HttpPost]
        // custom attribute created for implementing
        // POST-REDIRECT-GET pattern
        [ExportModelState]
        public async Task<IActionResult> EditDetails(LeaserEditModel leaserEditModel)
        {
            if(ModelState.IsValid)
            {
                var leaser = await this.leaserRepository
                    .Leaser
                    .Where(x => x.ID == leaserEditModel.ID)
                    .Include(x => x.AssetInfo)
                    .FirstOrDefaultAsync();
                if (leaser == null)
                    return NotFound();
                MapLeaserEditModelToLeaserObject(leaserEditModel, leaser);

                await this.leaserRepository.SaveChangesAsync();

                return RedirectToAction("Index");
            }
            return RedirectToAction("EditDetails", new { id = leaserEditModel.ID });
        }

        private static void MapLeaserEditModelToLeaserObject(LeaserEditModel leaserEditModel, Leaser leaser)
        {
            leaser.Address = leaserEditModel.Address;
            leaser.ContactNumber = leaserEditModel.ContactNumber;
            leaser.Name = leaserEditModel.Name;
            leaser.AssetInfo.Description = leaserEditModel.Description;
            leaser.AssetInfo.Location = leaserEditModel.AssetLocation;
            leaser.AssetInfo.Type = leaserEditModel.AssetType;
        }

        public async Task<IActionResult> Details([FromRoute(Name ="id")] int leaserId)
        {
            var leaser = await this.leaserRepository
                .Leaser
                .Where(x => x.ID == leaserId)
                .Include(x => x.AssetInfo)
                .FirstOrDefaultAsync();

            return View(leaser);
        }

        public async Task<IActionResult> History(int pageCount)
        {
            if (pageCount < 1)
                pageCount = 1;

            var pastLeasers = await this.leaserRepository.
                Leaser
                .Include(x => x.AssetInfo)
                .Where(x => x.AssetInfo.IsLeased == true)
                .Skip( (pageCount - 1) * ItemsPerPage)
                .Take(ItemsPerPage)
                .ToListAsync();

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
        public async Task<IActionResult> MarkAsLeased(int leaserId) {
            var leaser = await this.leaserRepository.Leaser
                .Where(x => x.ID == leaserId)
                .Include(x => x.AssetInfo)
                .FirstOrDefaultAsync();
            if(leaser != null)
            {
                leaser.AssetInfo.IsLeased = true;
                leaser.AssetInfo.LeasedDate = DateTime.Now;

                await this.leaserRepository.SaveChangesAsync();
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
        public async Task<IActionResult> Create(LeaserCreateModel leaserCreateModel)
        {
            if (this.ModelState.IsValid)
            {
                Leaser leaser = CreateLeaserFromLeaserCreateModel(leaserCreateModel);
                this.leaserRepository.AddLeaser(leaser);
                await this.leaserRepository.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            else
            {
                return RedirectToAction("Create");
            }
        }

        private static Leaser CreateLeaserFromLeaserCreateModel(LeaserCreateModel leaserCreateModel)
        {
            return new Leaser
            {
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
