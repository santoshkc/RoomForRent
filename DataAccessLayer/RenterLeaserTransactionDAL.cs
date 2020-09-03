using RoomForRent.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RoomForRent.DataAccessLayer
{
    public class RenterLeaserTransactionDAL
    {
        private readonly ITransactionRepository transactionRepository;

        public RenterLeaserTransactionDAL(ITransactionRepository transactionRepository)
        {
            this.transactionRepository = transactionRepository;
        }

        public RenterLeaserTransactionDto GetTransaction(int transactionId)
        {
            var transaction = this.transactionRepository
                .Transactions
                .Where(x => x.ID == transactionId)
                .FirstOrDefault();

            if(transaction != null)
            {
                return new RenterLeaserTransactionDto
                {
                    Id = transaction.ID,
                    CreatedDate = transaction.CreatedDate,
                    LastModifiedDate = transaction.LastModifiedDate,
                    TransactionStatus = transaction.TransactionStatus,
                    LeaserName = "Leaser 1",
                    RenterName = "Renter 1",
                };
            }
            return null;
        }

        public bool UpdateTransaction(int transactionId,RenterLeaserTransactionStatus renterLeaserTransactionStatus)
        {
            var transaction = this.transactionRepository
                .Transactions
                .Where(x => x.ID == transactionId)
                .FirstOrDefault();

            if(transaction != null)
            {
                transaction.TransactionStatus = renterLeaserTransactionStatus;
                transaction.LastModifiedDate = DateTime.Now;

                return true;
            }

            return false;
        }

        public IEnumerable<RenterLeaserTransactionDto> GetTransactions()
        {
            return this.transactionRepository
                .Transactions
                .Select(x => new RenterLeaserTransactionDto
                {
                    Id = x.ID,
                    CreatedDate = x.CreatedDate,
                    LastModifiedDate = x.LastModifiedDate,
                    TransactionStatus = x.TransactionStatus,
                    LeaserName = $"Leaser{x.ID}" ,
                    RenterName = $"Renter{x.ID}",
                });
        }
    }
}
