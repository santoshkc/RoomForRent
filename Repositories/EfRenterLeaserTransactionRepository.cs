using RoomForRent.Models;
using RoomForRent.Persistence.Contexts;
using RoomForRent.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RoomForRent.Repositories 
{ 
    public class EfRenterLeaserTransactionRepository : ITransactionRepository
    {
        private readonly RoomForRentDbContext roomForRentDbContext;

        public EfRenterLeaserTransactionRepository(
            RoomForRentDbContext roomForRentDbContext,
            IRenterRepository renterRepository,
            ILeaserRepository leaserRepository
            )
        {
            this.roomForRentDbContext = roomForRentDbContext;

            this.RenterRepository = renterRepository;
            this.LeaserRepository = leaserRepository;
        }

        public IRenterRepository RenterRepository { get; }

        public ILeaserRepository LeaserRepository { get; }

        public IEnumerable<RenterLeaserTransaction> Transactions => roomForRentDbContext.Transactions;

        public void AddTransaction(RenterLeaserTransaction renterLeaserTransaction)
        {
            this.roomForRentDbContext.Transactions.Add(renterLeaserTransaction);
        }


        public void ModifyTransaction(RenterLeaserTransaction renterLeaserTransaction)
        {
            this.roomForRentDbContext.
                Transactions.Update(renterLeaserTransaction);
        }


        // TODO: make real async later.
        public bool SaveChangesAsync()
        {
            return this.roomForRentDbContext.SaveChanges() >= 0;
        }
    }
}
