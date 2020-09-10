using RoomForRent.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RoomForRent.Repositories
{
    public interface ILeaserRepository
    {
        IQueryable<Leaser> Leaser { get; }

        void AddLeaser(Leaser roomLeaser);

        Task<Leaser> GetLeaserByIdAsync(int leaserId);

        Task<bool> SaveChangesAsync();

        Task<IEnumerable<Leaser>> GetLeasers(int pageCount, int itemsPerPage, bool retrivePastLeasers = false);

        Task<int> GetLeasersCount(bool retrievePastLeasers = false);
    }
}
