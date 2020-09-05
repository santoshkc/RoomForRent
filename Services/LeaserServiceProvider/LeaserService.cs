using Microsoft.EntityFrameworkCore;
using RoomForRent.Models;
using RoomForRent.Models.LeaserModels;
using RoomForRent.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RoomForRent.Services.LeaserServiceProvider
{
    public class LeaserService
    {
        private readonly ILeaserRepository leaserRepository;

        public LeaserService(ILeaserRepository leaserRepository)
        {
            this.leaserRepository = leaserRepository;
        }

        internal async Task<(List<Leaser> leasers,int activeLeasers)> GetLeasers(int pageCount, int itemsPerPage)
        {
            var leasers = await this.leaserRepository
                .Leaser
                .Include(x => x.AssetInfo)
                .Where(x => x.AssetInfo.IsLeased == null
                    || x.AssetInfo.IsLeased == false)
                .Skip((pageCount - 1) * itemsPerPage)
                .Take(itemsPerPage)
                .ToListAsync();

            var totalCount = await this.leaserRepository.Leaser
                                 .Include(x => x.AssetInfo)
                                .CountAsync(x => x.AssetInfo.IsLeased == null
                                || x.AssetInfo.IsLeased == false);

            return (leasers,totalCount);
        }

        internal async Task<LeaserEditModel> GetLeaserEditDetails(int leaserId)
        {
            var leaser = await this.leaserRepository
                .Leaser
                .Where(x => x.ID == leaserId)
                .Include(x => x.AssetInfo)
                .FirstOrDefaultAsync();

            if (leaser != null)
            {
                LeaserEditModel leaserEditModel 
                    = CreateLeaserEditModelFromLeaser(leaser);

                return leaserEditModel;
            }
            return null;
        }

        internal async Task<bool> UpdateLeaserEditDetails(LeaserEditModel leaserEditModel)
        {
            var leaser = await this.leaserRepository
                    .Leaser
                    .Where(x => x.ID == leaserEditModel.ID)
                    .Include(x => x.AssetInfo)
                    .FirstOrDefaultAsync();
            if (leaser == null)
                return false;
            MapLeaserEditModelToLeaserObject(leaserEditModel, leaser);

            return await this.leaserRepository.SaveChangesAsync();
        }

        internal async Task<Leaser> GetLeaserDetails(int leaserId)
        {
            var leaser = await this.leaserRepository
               .Leaser
               .Where(x => x.ID == leaserId)
               .Include(x => x.AssetInfo)
               .FirstOrDefaultAsync();
            return leaser;
        }

        internal async Task<(List<Leaser> pastLeasers,int totalItems)> GetLeaserHistory(int pageCount, int itemsPerPage)
        {
            var pastLeasers = await this.leaserRepository.
                Leaser
                .Include(x => x.AssetInfo)
                .Where(x => x.AssetInfo.IsLeased == true)
                .Skip((pageCount - 1) * itemsPerPage)
                .Take(itemsPerPage)
                .ToListAsync();

            var totalItems = this.leaserRepository.
                    Leaser.Count(x => x.AssetInfo.IsLeased == true);

            return (pastLeasers, totalItems);
        }

        internal async Task<bool> MarkAsLeased(int leaserId)
        {
            var leaser = await this.leaserRepository.Leaser
                .Where(x => x.ID == leaserId)
                .Include(x => x.AssetInfo)
                .FirstOrDefaultAsync();
            if (leaser != null)
            {
                leaser.AssetInfo.IsLeased = true;
                leaser.AssetInfo.LeasedDate = DateTime.Now;

                return await this.leaserRepository.SaveChangesAsync();

            }
            return false;
        }

        internal async Task CreateLeaser(LeaserCreateModel leaserCreateModel)
        {
            Leaser leaser = CreateLeaserFromLeaserCreateModel(leaserCreateModel);
            this.leaserRepository.AddLeaser(leaser);
            await this.leaserRepository.SaveChangesAsync();
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

        private static void MapLeaserEditModelToLeaserObject(LeaserEditModel leaserEditModel, Leaser leaser)
        {
            leaser.Address = leaserEditModel.Address;
            leaser.ContactNumber = leaserEditModel.ContactNumber;
            leaser.Name = leaserEditModel.Name;
            leaser.AssetInfo.Description = leaserEditModel.Description;
            leaser.AssetInfo.Location = leaserEditModel.AssetLocation;
            leaser.AssetInfo.Type = leaserEditModel.AssetType;
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
    }
}
