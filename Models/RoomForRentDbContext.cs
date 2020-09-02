using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RoomForRent.Models
{
    public class RoomForRentDbContext : DbContext
    {
        public RoomForRentDbContext(DbContextOptions<RoomForRentDbContext> options)
            : base(options)
        { }

        public DbSet<Renter> Renters { get; set; }
        
        public DbSet<Leaser> Leasers { get; set; }
    }
}
