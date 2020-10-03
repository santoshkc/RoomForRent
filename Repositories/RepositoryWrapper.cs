using Microsoft.EntityFrameworkCore;
using RoomForRent.Persistence.Contexts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RoomForRent.Repositories
{ 
    public interface IRepositoryWrapper
    {
        IRenterRepository RenterRepository { get; }

        ILeaserRepository LeaserRepository { get; }

        ITransactionRepository TransactionRepository { get; }

        bool Save();

        Task<bool> SaveAsync();
    }

    public class RepositoryWrapper : IRepositoryWrapper
    {
        private readonly RoomForRentDbContext dbContext;

        private readonly IRenterRepository renterRepository = null;
        private readonly ILeaserRepository leaserRepository;

        private readonly ITransactionRepository transactionRepository = null;

        public RepositoryWrapper(RoomForRentDbContext dbContext)
        {
            this.dbContext = dbContext;

            this.renterRepository = new EfRenterRepository(dbContext);
            this.leaserRepository = new EfLeaserRepository(dbContext);
            this.transactionRepository = new EfRenterLeaserTransactionRepository(dbContext, renterRepository, leaserRepository);
        }

        public IRenterRepository RenterRepository => renterRepository;

        public ILeaserRepository LeaserRepository => leaserRepository;

        public ITransactionRepository TransactionRepository => transactionRepository;

        public bool Save()
        {
            return this.dbContext.SaveChanges() >= 0;
        }

        public async Task<bool> SaveAsync()
        {
            var result = await this.dbContext.SaveChangesAsync();
            return result >= 0;
        }
    }
}
