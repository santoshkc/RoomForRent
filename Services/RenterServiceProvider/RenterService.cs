using Microsoft.EntityFrameworkCore;
using RoomForRent.Models;
using RoomForRent.Models.ViewModel;
using RoomForRent.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RoomForRent.Services.RenterServiceProvider
{
    public class RenterService
    {
        private readonly IRenterRepository renterRepository;

        public RenterService(IRenterRepository renterRepository)
        {
            this.renterRepository = renterRepository;
        }

        internal async Task<RenterListViewModel> GetRenterList(int pageCount, int itemsPerPage)
        {
            var renters = await this.renterRepository
                            .Renters
                            .Where(x => x.Found == null || x.Found == false)
                            .Skip((pageCount - 1) * itemsPerPage)
                            .Take(itemsPerPage)
                            .ToListAsync();

            var renterViewInfo = new RenterListViewModel
            {
                Renters = renters,
                PagingInfo = new PagingInfo
                {
                    CurrentPage = pageCount,
                    ItemsPerPage = itemsPerPage,
                    TotalItems = await this.renterRepository.Renters
                                .CountAsync(x => x.Found == null
                                || x.Found.Value == false)
                }
            };
            return renterViewInfo;
        }

        internal async Task<IEnumerable<Renter>> GetHistory()
        {
            var pastRenters = await this.renterRepository
                .Renters
                .Where(x => x.Found == true)
                .ToListAsync();

            return pastRenters;
        }

        internal async Task<bool> MarkAsFound(int renterId)
        {
            var renter = await this.renterRepository
                .Renters
                .Where(x => x.ID == renterId)
                .FirstOrDefaultAsync();

            if (renter != null)
            {
                renter.Found = true;
                renter.FoundDate = DateTime.Now;
                await this.renterRepository.SaveChangesAsync();
                return true;
            }
            return false;
        }

        internal async Task<Renter> GetRenterDetails(int renterId)
        {
            var renter = await this.renterRepository
                .Renters
                .Where(x => x.ID == renterId)
                .FirstOrDefaultAsync();

            return renter;
        }

        internal async Task<bool> CreateRenter(RenterCreateModel renterCreateModel)
        {
            Renter renter = CreateRenterObjectFromRenterCreateModel(renterCreateModel);
            this.renterRepository.AddRenter(renter);
            return await this.renterRepository.SaveChangesAsync();
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

        internal async Task<RenterEditModel> GetRenterEditDetails(int renterId)
        {
            var renter = await this.renterRepository
               .Renters
               .Where(x => x.ID == renterId)
               .FirstOrDefaultAsync();

            // will use auto mapper later
            // for now manual mapping

            if (renter != null)
            {
                RenterEditModel renterEditModel = MapRenterObjectToRenterEditModel(renter);
                return renterEditModel;
            }
            return null;
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

        internal async Task<bool> EditRenterDetails(RenterEditModel renterEditModel)
        {
            var renterModel = await this.
                    renterRepository
                    .Renters
                    .Where(x => x.ID == renterEditModel.ID)
                    .FirstOrDefaultAsync();
            if (renterModel == null)
            {
                return false;
            }

            MapRenterEditModelToRenterObject(renterEditModel, renterModel);

            return await this.renterRepository.SaveChangesAsync();
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
