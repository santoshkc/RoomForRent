using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace RoomForRent.Models.LeaserModels
{
    public class LeaserEditModel
    {
        [Required(ErrorMessage = "")]
        public int ID { get; set; }

        [Required(ErrorMessage = "Name should be entered.")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Address should be entered.")]
        public string Address { get; set; }

        [Required(ErrorMessage = "Contact Number should be entered.")]
        public string ContactNumber { get; set; }

        [Required(ErrorMessage = "Asset type should be entered.")]
        public AssetType AssetType { get; set; }

        [Required(ErrorMessage = "Asset Location should be required.")]
        public string AssetLocation { get; set; }

        [Required(ErrorMessage = "Description should be required.")]
        public string Description { get; set; }
    }
}
