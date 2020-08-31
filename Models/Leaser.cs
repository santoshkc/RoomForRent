using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace RoomForRent.Models
{
    public class Leaser
    {
        public int ID { get; set; }

        [Required(ErrorMessage ="Name should be entered.")]
        public string Name { get; set; }

        [Required(ErrorMessage ="Address should be entered.")]
        public string Address { get; set; }
        
        [Required(ErrorMessage ="Contact Number should be entered.")]
        public string ContactNumber { get; set; }

        public Asset AssetInfo { get; set; }
    }

    public enum AssetType { Flat = 1, House, Room }

    public class Asset
    {
        [Required(ErrorMessage ="Asset type should be entered.")]
        public AssetType Type { get; set; }

        [Required(ErrorMessage = "Location should be required.")]
        public string Location { get; set; }

        public bool? IsLeased { get; set; } 

        [Required(ErrorMessage = "Description should be required.")]
        public string Description { get; set; }
    }
}
