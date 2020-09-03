using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RoomForRent.Models
{
    public interface ITransactionRepository
    {
        IEnumerable<RenterLeaserTransaction> Transactions { get; }

        void AddTransaction(RenterLeaserTransaction renterLeaserTransaction);

        void ModifyTransaction(RenterLeaserTransaction renterLeaserTransaction);

        bool SaveChangesAsync();
    }
}
