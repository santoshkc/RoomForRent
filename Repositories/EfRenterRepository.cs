using Microsoft.EntityFrameworkCore;
using RoomForRent.Models;
using RoomForRent.Persistence.Contexts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RoomForRent.Repositories
{
    public class EfRenterRepository : IRenterRepository
    {
        private readonly RoomForRentDbContext roomForRentDbContext;

        public EfRenterRepository(RoomForRentDbContext roomForRentDbContext)
        {
            this.roomForRentDbContext = roomForRentDbContext;
        }

        public IQueryable<Renter> Renters => this.roomForRentDbContext.Renters;

        public void AddRenter(Renter renter)
        {
            this.roomForRentDbContext.Renters.Add(renter);
        }

        public async Task<Renter> GetRenterByIdAsync(int renterId)
        {
            var renter = await this.roomForRentDbContext
                .Renters
                .Where(x => x.ID == renterId)
                .FirstOrDefaultAsync();
            return renter;
        }

        public async Task<IEnumerable<Renter>> GetRenters(int pageCount, int itemsPerPage, bool retrivePastLeasers = false)
        {
            var renters = await this.roomForRentDbContext
                            .Renters
                            .Where(x => retrivePastLeasers ? 
                                    x.Found == true
                                    : (x.Found == null || x.Found == false)
                                )
                            .Skip((pageCount - 1) * itemsPerPage)
                            .Take(itemsPerPage)
                            .ToListAsync();

            return renters;
        }

        public async Task<IEnumerable<Renter>> GetRentersByName(string renterName,int pageCount, int itemsPerPage, bool retrivePastLeasers = false)
        {
            var renters = await this.roomForRentDbContext
                            .Renters
                            .Where(x => (retrivePastLeasers ?
                                    x.Found == true
                                    : (x.Found.HasValue == false || x.Found.Value == false)
                                    ) 
                                && EF.Functions.Like(x.Name, $"%{renterName}%")
                                )
                            .Skip((pageCount - 1) * itemsPerPage)
                            .Take(itemsPerPage)
                            .ToListAsync();

            return renters;
        }

        public async Task<int> GetRentersCount(bool retrievePastLeasers = false)
        {
            return await this.roomForRentDbContext.Renters
                                .CountAsync(x => retrievePastLeasers 
                                    ? x.Found == true
                                  : (x.Found == null || x.Found.Value == false)
                                  );
        }

        public async Task<int> GetRentersByNameCount(string renterName, bool retrievePastLeasers = false)
        {
            return await this.roomForRentDbContext.Renters
                                .CountAsync(x => (retrievePastLeasers
                                    ? x.Found == true
                                  : (x.Found == null || x.Found.Value == false)
                                  ) && EF.Functions.Like(x.Name,$"%{renterName}%")
                                  );
        }

        public async Task<bool> SaveChangesAsync()
        {
            var result = await this.roomForRentDbContext.SaveChangesAsync() >= 0;
            return result;
        }
    }
}
