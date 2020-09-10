using Microsoft.EntityFrameworkCore;
using RoomForRent.Models;
using RoomForRent.Persistence.Contexts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RoomForRent.Repositories
{
    public class EfLeaserRepository : ILeaserRepository
    {
        private readonly RoomForRentDbContext roomForRentDbContext;

        public EfLeaserRepository(RoomForRentDbContext roomForRentDbContext)
        {
            this.roomForRentDbContext = roomForRentDbContext;
        }

        public IQueryable<Leaser> Leaser => roomForRentDbContext.Leasers;

        public void AddLeaser(Leaser roomLeaser)
        {
            roomForRentDbContext.Leasers.Add(roomLeaser);
        }

        public async Task<Leaser> GetLeaserByIdAsync(int leaserId)
        {
            return await roomForRentDbContext.Leasers
                            .Where(x => x.ID == leaserId)
                            .Include(x => x.AssetInfo)
                            .FirstOrDefaultAsync();
        }

        public async Task<int> GetLeasersCount(bool retrievePastLeasers = false)
        {
            return await this.roomForRentDbContext
                            .Leasers
                            .CountAsync(x => retrievePastLeasers ?
                               x.AssetInfo.IsLeased == true
                               : (x.AssetInfo.IsLeased == null || x.AssetInfo.IsLeased == false)
                               );
        }

        public async Task<IEnumerable<Leaser>> GetLeasers(int pageCount,int itemsPerPage,bool retrivePastLeasers = false)
        {
            return await this.roomForRentDbContext
                            .Leasers
                            .Include(x => x.AssetInfo)
                            .Where(x => retrivePastLeasers ?
                                    x.AssetInfo.IsLeased == true
                                    : (x.AssetInfo.IsLeased == null || x.AssetInfo.IsLeased == false))
                            .Skip((pageCount - 1) * itemsPerPage)
                            .Take(itemsPerPage)
                            .ToListAsync();
        }

        public async Task<bool> SaveChangesAsync()
        {
            var result = await roomForRentDbContext.SaveChangesAsync() >= 0;
            return result;
        }
    }
}
