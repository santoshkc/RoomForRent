using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RoomForRent.Models
{
    public class RenterLeaserTransaction
    {
        public int ID { get; set; }

        public int RenterId { get; set; }

        public int LeaserId { get; set; }

        public RenterLeaserTransactionStatus TransactionStatus { get; set; }

        public DateTime CreatedDate { get; set; }

        public DateTime LastModifiedDate { get; set; }
    }

    public enum RenterLeaserTransactionStatus { Pending, Deal, NoDeal }
}
