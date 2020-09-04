using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace RoomForRent.Models.RenterLeaserTransactions
{
    public class RenterLeaserTransaction
    {
        [Key]
        public int ID { get; set; }

        public int? LeaserId { get; set; }

        public int? RenterId { get; set; }

        [Required]
        public RenterLeaserTransactionStatus TransactionStatus { get; set; }

        [Required]
        public DateTime CreatedDate { get; set; }

        [Required]
        public DateTime LastModifiedDate { get; set; }
    }
}
