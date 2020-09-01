using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace RoomForRent.Models
{
    public class RenterCreateModel
    {
        [Required(ErrorMessage = "Name should be entered.")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Address should be entered.")]
        public string Address { get; set; }

        [Required(ErrorMessage = "Contact Number should be entered.")]
        public string ContactNumber { get; set; }

        [Required(ErrorMessage = "Seeked Asset should be entered.")]
        public AssetType SeekedAsset { get; set; }

        [Required(ErrorMessage = "Description should be entered.")]
        public string Description { get; set; }
    }
}
