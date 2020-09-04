using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RoomForRent.Models.ViewModel
{
    public class RenterListViewModel
    {
        public IEnumerable<Renter> Renters { get; set; }

        public PagingInfo PagingInfo { get; set; }
    }
}
