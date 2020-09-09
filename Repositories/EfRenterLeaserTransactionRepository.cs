using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.EntityFrameworkCore;
using RoomForRent.Models;
using RoomForRent.Persistence.Contexts;
using RoomForRent.Repositories;
using RoomForRent.Services.LeaserServiceProvider;
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

        public async Task<IEnumerable<Leaser>> GetUnlinkedLeasers(int renterId)
        {

            var leasers = await this.roomForRentDbContext
                        .Leasers
                        .Include(x => x.AssetInfo)
                        .Where(x =>
                            (x.AssetInfo.IsLeased == null
                                || x.AssetInfo.IsLeased == false
                            ) &&
                            this.roomForRentDbContext
                            .Transactions
                            .Any(y =>
                               y.LeaserId == x.ID
                               && y.RenterId == renterId
                               //&& y.TransactionStatus == RenterLeaserTransactionStatus.Pending
                            ) == false
                             ).ToListAsync();
            return leasers;
        }

        public async Task<IEnumerable<Renter>> GetUnlinkedRenters(int leaserId)
        {
            var renters = await this.roomForRentDbContext
                        .Renters
                        .Where(x =>
                            (x.Found.HasValue == false
                                || x.Found.Value == false
                            ) &&
                            this.roomForRentDbContext
                            .Transactions
                            .Any(y =>
                               y.RenterId == x.ID
                               && y.LeaserId == leaserId
                            //&& y.TransactionStatus == RenterLeaserTransactionStatus.Pending
                            ) == false
                ).ToListAsync();
            return renters;
        }


        // TODO: make real async later.
        public bool SaveChangesAsync()
        {
            return this.roomForRentDbContext.SaveChanges() >= 0;
        }
    }
}
