using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RoomForRent.Models
{
    public class InMemoryRoomLeaserRepository : ILeaserRepository
    {
        private List<Leaser> leasers = new List<Leaser>
        {
            new Leaser
            {
                ID=1,
                Name = "Santosh",
                Address = "Tinkune",
                ContactNumber = "984910000",
                AssetInfo = new Asset
                {
                    Type = AssetType.Flat,
                    Location = "Tinkune",
                    Description = "4 room flat",
                }
            },
            new Leaser
            {
                ID=2,
                Name = "Sabin",
                Address = "Koteshwor",
                ContactNumber = "98422323",
                AssetInfo = new Asset
                {
                    Type = AssetType.House,
                    Location = "Baneshwor",
                    Description = "4 storey house",
                }
            }
        };

        public IQueryable<Leaser> Leaser
        {
            get {
                //return leasers;
                return null;
            }
        }

        public void AddLeaser(Leaser roomLeaser)
        {
            leasers.Add(roomLeaser);
        }

        public async Task<bool> SaveChangesAsync()
        {
            return true;
        }
    }
}
