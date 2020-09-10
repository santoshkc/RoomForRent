using RoomForRent.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RoomForRent.Repositories
{
    public interface ITransactionRepository
    {
        IRenterRepository RenterRepository { get; }

        ILeaserRepository LeaserRepository { get; }

        IEnumerable<RenterLeaserTransaction> Transactions { get; }

        Task<RenterLeaserTransaction> GetTransactionByIdAsync(int transactionId);

        void AddTransaction(RenterLeaserTransaction renterLeaserTransaction);

        void ModifyTransaction(RenterLeaserTransaction renterLeaserTransaction);

        Task<IEnumerable<Leaser>> GetUnlinkedLeasers(int renterId);

        Task<IEnumerable<Renter>> GetUnlinkedRenters(int leaserId);

        Task<bool> SaveChangesAsync();
    }
}
