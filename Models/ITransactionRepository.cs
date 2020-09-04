using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RoomForRent.Models
{
    public interface ITransactionRepository
    {
        IRenterRepository RenterRepository { get; }

        ILeaserRepository LeaserRepository { get; }

        IEnumerable<RenterLeaserTransaction> Transactions { get; }

        void AddTransaction(RenterLeaserTransaction renterLeaserTransaction);

        void ModifyTransaction(RenterLeaserTransaction renterLeaserTransaction);

        bool SaveChangesAsync();
    }
}
