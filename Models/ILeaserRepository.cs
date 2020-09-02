using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RoomForRent.Models
{
    public interface ILeaserRepository
    {
        IQueryable<Leaser> Leaser { get; }

        void AddLeaser(Leaser roomLeaser);

        Task<bool> SaveChangesAsync();
    }
}
