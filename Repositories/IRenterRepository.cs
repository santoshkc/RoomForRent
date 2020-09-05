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

        Task<bool> SaveChangesAsync();
    }
}
