using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RoomForRent.Models.ViewModel
{
    public class LeaserRentersLinkDto
    {
        public IEnumerable<Renter> Renters { get; set; }

        public Leaser Leaser { get; set; }
    }

    public class RenterLeasersLinkDto
    {
        public IEnumerable<Leaser> Leasers { get; set; }

        public Renter Renter { get; set; }
    }
}
