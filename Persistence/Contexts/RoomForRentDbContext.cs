using Microsoft.EntityFrameworkCore;
using RoomForRent.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RoomForRent.Persistence.Contexts
{
    public class RoomForRentDbContext : DbContext
    {
        public RoomForRentDbContext(DbContextOptions<RoomForRentDbContext> options)
            : base(options)
        { }

        public DbSet<Renter> Renters { get; set; }
        
        public DbSet<Leaser> Leasers { get; set; }

        public DbSet<RenterLeaserTransaction> Transactions { get; set; }
    }
}
