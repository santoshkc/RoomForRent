using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RoomForRent.Models
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

        public async Task<bool> SaveChangesAsync()
        {
            var result = await this.roomForRentDbContext.SaveChangesAsync() >= 0;
            return result;
        }
    }
}
