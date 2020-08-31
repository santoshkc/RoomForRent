using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RoomForRent.Models
{
    public class InMemoryRenterRepository : IRenterRepository
    {
        private readonly List<Renter> renters = null;

        public InMemoryRenterRepository()
        {
            renters = new List<Renter> {
                    new Renter
                    {
                        ID = 1,
                        Name = "Santosh",
                        Address = "Tinkune",
                        ContactNumber = "9849",
                        Description = "Requires 2 room",
                        SeekedAsset = AssetType.Room,
                    },
                    new Renter
                    {
                        ID = 2,
                        Name = "Sabin",
                        Address = "Baneshwor",
                        ContactNumber = "9849123",
                        Description = "Requires 3 room flat",
                        SeekedAsset = AssetType.Flat,
                    }
            };
        }

        public IEnumerable<Renter> Renters
        {
            get
            {
                return renters;
            }
        }

        public void AddRenter(Renter renter)
        {
            this.renters.Add(renter);
        }

        //public IEnumerable<Room> Rooms
        //{
        //    get {
        //            var rooms = new List<Room>
        //            {
        //                new Room
        //                {
        //                    Count = 2,
        //                    Location = "Tinkune",
        //                    Price = 10000
        //                },
        //                new Room
        //                {
        //                    Count = 4,
        //                    Location = "Koteshwor",
        //                    Price = 20000
        //                }
        //            };
        //            return rooms;  
        //    }
        //}
    }
}
