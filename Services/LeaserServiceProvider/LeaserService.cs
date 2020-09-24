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

        internal async Task<(IEnumerable<Leaser> leasers,int activeLeasers)> GetLeasers(int pageCount, int itemsPerPage)
        {
            var leasers = await GetActiveLeasers(pageCount, itemsPerPage);
            int totalCount = await GetActiveLeasersCount();

            return (leasers, totalCount);
        }

        private async Task<int> GetActiveLeasersCount()
        {
            return await GetLeasersCountInternal(false);
        }

        private async Task<IEnumerable<Leaser>> GetActiveLeasers(int pageCount, int itemsPerPage)
        {
            return await GetLeasersInternal(pageCount, itemsPerPage,false);
        }

        internal async Task<LeaserEditModel> GetLeaserEditDetails(int leaserId)
        {
            var leaser = await this.leaserRepository
                    .GetLeaserByIdAsync(leaserId);

            if (leaser != null)
            {
                LeaserEditModel leaserEditModel 
                    = CreateLeaserEditModelFromLeaser(leaser);

                return leaserEditModel;
            }
            return null;
        }

        internal async Task<(IEnumerable<Leaser> leasers, int activeLeasers)> GetLeasersByNameAsync(string leaserName, int pageCount, int itemsPerPage)
        {
            var leasers = await this.leaserRepository
                .GetLeasersByNameAsync(leaserName, pageCount, itemsPerPage);

            var activeLeasersCount = await this.leaserRepository
                .GetLeasersByNameCountAsync(leaserName);

            return (leasers, activeLeasersCount);
        }

        internal async Task<bool> UpdateLeaserEditDetails(LeaserEditModel leaserEditModel)
        {
            var leaser = await this.leaserRepository.
                            GetLeaserByIdAsync(leaserEditModel.ID);
            if (leaser == null)
                return false;
            MapLeaserEditModelToLeaserObject(leaserEditModel, leaser);

            return await this.leaserRepository.SaveChangesAsync();
        }

        internal async Task<Leaser> GetLeaserDetails(int leaserId)
        {
            var leaser = await this.leaserRepository
                            .GetLeaserByIdAsync(leaserId);
            return leaser;
        }

        internal async Task<(IEnumerable<Leaser> pastLeasers,int pastLeasersCount)> GetLeaserHistory(int pageCount, int itemsPerPage)
        {
            var pastLeasers = await GetPastLeasers(pageCount, itemsPerPage);
            int totalItems = await GetPastLeasersCount();

            return (pastLeasers, totalItems);
        }

        private async Task<int> GetPastLeasersCount()
        {
            return await GetLeasersCountInternal(true);
        }

        private async Task<IEnumerable<Leaser>> GetPastLeasers(int pageCount, int itemsPerPage)
        {
            return await GetLeasersInternal(pageCount,itemsPerPage,true);
        }

        private async Task<int> GetLeasersCountInternal(bool retrievePastLeasers = false)
        {
            return await this.leaserRepository.GetLeasersCount(retrievePastLeasers);
        }

        private async Task<IEnumerable<Leaser>> GetLeasersInternal(int pageCount, int itemsPerPage,bool retrievePastUsers = false)
        {
            return await this.leaserRepository.GetLeasers(pageCount, itemsPerPage, retrievePastUsers);
        }

        internal async Task<bool> MarkAsLeased(int leaserId,bool callSaveChanges = true)
        {
            Leaser leaser = await this.leaserRepository
                    .GetLeaserByIdAsync(leaserId);

            if (leaser != null)
            {
                leaser.AssetInfo.IsLeased = true;
                leaser.AssetInfo.LeasedDate = DateTime.Now;

                if(callSaveChanges)
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
