using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RoomForRent.Models
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

        public async Task<bool> SaveChangesAsync()
        {
            var result = await roomForRentDbContext.SaveChangesAsync() >= 0;
            return result;
        }
    }
}
