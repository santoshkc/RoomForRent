using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RoomForRent.DataAccessLayer
{
    public class RenterLeaserTransactionCreateDto
    {
        public IEnumerable<RenterTransactionInfo> Renters { get; set; }

        public IEnumerable<LeaserTransactionInfo> Leasers { get; set; }

    }

    public class RenterTransactionInfo
    {
        public int RenterId { get; set; }

        public string Name { get; set; }

        public string Address { get; set; }
    }

    public class LeaserTransactionInfo
    {
        public int LeaserId { get; set; }

        public string Name { get; set; }

        public string Address { get; set; }
    }
}
