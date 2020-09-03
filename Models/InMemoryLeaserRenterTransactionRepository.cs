﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RoomForRent.Models
{
    public class InMemoryLeaserRenterTransactionRepository : ITransactionRepository
    {
        private List<RenterLeaserTransaction> renterLeaserTransactions = null;
        public InMemoryLeaserRenterTransactionRepository()
        {
            renterLeaserTransactions = new List<RenterLeaserTransaction>
            {
                new RenterLeaserTransaction
                {
                    ID = 1,
                    LeaserId = 1,
                    RenterId = 2,
                    CreatedDate = DateTime.Now,
                    LastModifiedDate = DateTime.Now,
                    TransactionStatus = RenterLeaserTransactionStatus.Pending,
                },
                new RenterLeaserTransaction
                {
                    ID = 2,
                    LeaserId = 1,
                    RenterId = 3,
                    CreatedDate = DateTime.Now,
                    LastModifiedDate = DateTime.Now,
                    TransactionStatus = RenterLeaserTransactionStatus.Pending,
                },
                new RenterLeaserTransaction
                {
                    ID = 3,
                    LeaserId = 1,
                    RenterId = 2,
                    CreatedDate = DateTime.Now,
                    LastModifiedDate = DateTime.Now,
                    TransactionStatus = RenterLeaserTransactionStatus.Pending,
                },
                new RenterLeaserTransaction
                {
                    ID = 4,
                    LeaserId = 2,
                    RenterId = 5,
                    CreatedDate = DateTime.Now,
                    LastModifiedDate = DateTime.Now,
                    TransactionStatus = RenterLeaserTransactionStatus.Pending,
                }
            };
        }

        public IEnumerable<RenterLeaserTransaction> Transactions
        {
            get
            {
                return renterLeaserTransactions;
            }
        }

        public void AddTransaction(RenterLeaserTransaction renterLeaserTransaction)
        {
            this.renterLeaserTransactions.Add(renterLeaserTransaction);
        }

        public void ModifyTransaction(RenterLeaserTransaction renterLeaserTransaction)
        {
            this.renterLeaserTransactions
                .RemoveAll(x => x.ID == renterLeaserTransaction.ID);
            this.renterLeaserTransactions.Add(renterLeaserTransaction);
        }

        public bool SaveChangesAsync()
        {
            return true;
        }
    }
}
