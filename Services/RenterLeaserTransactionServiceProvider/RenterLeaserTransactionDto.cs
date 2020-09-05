using RoomForRent.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace RoomForRent.Services.RenterLeaserTransactionServiceProvider
{
    public class RenterLeaserTransactionDto
    {
        [Required]
        public int Id { get; set; }

        public string LeaserName { get; set; }

        public string RenterName { get; set; }

        [Required]
        public RenterLeaserTransactionStatus TransactionStatus { get; set; }

        public DateTime CreatedDate { get; set; }

        public DateTime LastModifiedDate { get; set; }

    }
}
