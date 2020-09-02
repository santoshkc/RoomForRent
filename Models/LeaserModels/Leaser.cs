using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace RoomForRent.Models
{
    public class Leaser
    {
        [Key]
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
        [ForeignKey(nameof(Leaser))]
        [Key]
        public int ID { get; set; }

        public Leaser Leaser { get; set; }

        [Required(ErrorMessage ="Asset type should be entered.")]
        public AssetType Type { get; set; }

        [Required(ErrorMessage = "Location should be required.")]
        public string Location { get; set; }

        public bool? IsLeased { get; set; }
        public DateTime? LeasedDate { get; set; }

        [Required(ErrorMessage = "Description should be required.")]
        public string Description { get; set; }
    }
}
