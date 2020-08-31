using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RoomForRent.Models
{
    public interface IRenterRepository
    {
        IEnumerable<Renter> Renters { get; }

        void AddRenter(Renter renter);
    }
}
