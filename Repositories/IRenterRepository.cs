using RoomForRent.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RoomForRent.Repositories
{
    public interface IRenterRepository
    {
        IQueryable<Renter> Renters { get; }

        void AddRenter(Renter renter);

        Task<Renter> GetRenterByIdAsync(int renterId);

        Task<IEnumerable<Renter>> GetRenters(int pageCount, int itemsPerPage, bool retrivePastLeasers = false);

        Task<IEnumerable<Renter>> GetRentersByName(string renterName, int pageCount, int itemsPerPage, bool retrivePastLeasers = false);

        Task<int> GetRentersByNameCount(string renterName, bool retrievePastLeasers = false);

        Task<int> GetRentersCount(bool retrievePastLeasers = false);

        Task<bool> SaveChangesAsync();
    }
}
