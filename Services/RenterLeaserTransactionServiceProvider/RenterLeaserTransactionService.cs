using Microsoft.EntityFrameworkCore;
using RoomForRent.Models;
using RoomForRent.Models.ViewModel;
using RoomForRent.Repositories;
using RoomForRent.Services.LeaserServiceProvider;
using RoomForRent.Services.RenterServiceProvider;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RoomForRent.Services.RenterLeaserTransactionServiceProvider
{
    public class RenterLeaserTransactionService
    {
        private readonly IRepositoryWrapper repositoryWrapper;

        private readonly ITransactionRepository transactionRepository;

        private readonly LeaserService leaserService = null;

        private readonly RenterService renterService = null;

        private readonly IRenterRepository renterRepository = null;

        private readonly ILeaserRepository leaserRepository = null;

        public RenterLeaserTransactionService(IRepositoryWrapper repositoryWrapper)
        {
            this.repositoryWrapper = repositoryWrapper;
            this.transactionRepository = this.repositoryWrapper.TransactionRepository;

            this.renterRepository = repositoryWrapper.RenterRepository;
            this.leaserRepository = repositoryWrapper.LeaserRepository;

            this.leaserService = new LeaserService(this.leaserRepository);
            this.renterService = new RenterService(this.renterRepository);
        }

        internal async Task<RenterLeasersLinkDto> GetPotentialLeasersDto(int renterId)
        {
            var renter = await this.renterRepository
                            .GetRenterByIdAsync(renterId);

            var leasers = await this.transactionRepository
                .GetUnlinkedLeasers(renterId);

            var linkDto = new RenterLeasersLinkDto
            {
                Renter = renter,
                Leasers = leasers,
            };
            return linkDto;
        }

        internal async Task<LeaserRentersLinkDto> GetPotentialRentersDto(int leaserId)
        {
            var leaser = await this.leaserRepository
                                .GetLeaserByIdAsync(leaserId);

            var renters = await this.transactionRepository
                .GetUnlinkedRenters(leaserId);

            var linkDto = new LeaserRentersLinkDto
            {
                Leaser = leaser,
                Renters = renters,
            };
            return linkDto;
        }

        internal async Task<bool> LinkRenterAndLeaser(int leaserId,int renterId)
        {
            var leaser = await this.leaserRepository
                .GetLeaserByIdAsync(leaserId);

            if(leaser == null)
            {
                return false;
            }

            var renter = await this.renterRepository
                .GetRenterByIdAsync(renterId);

            if(renter == null)
            {
                return false;
            }

            var transaction = new RenterLeaserTransaction
            {
                LeaserId = leaserId,
                RenterId = renterId,
                CreatedDate = DateTime.Now,
                LastModifiedDate = DateTime.Now,
                TransactionStatus = RenterLeaserTransactionStatus.Pending,
            };

            this.transactionRepository.AddTransaction(transaction);
            return await this.transactionRepository.SaveChangesAsync();
        }

        internal async Task<RenterLeaserTransactionCreateDto> GetDataForTransactionLink(int? leaserId, int? renterId)
        {
            var rentersInfo = new List<RenterTransactionInfo>();
            if (renterId.HasValue)
            {
                var renterInfo = await this.renterRepository
                    .GetRenterByIdAsync(renterId.Value);

                if (renterInfo != null)
                {
                    var renterTransactionInfo = new RenterTransactionInfo
                    {
                        RenterId = (int)renterInfo.ID,
                        Name = renterInfo.Name,
                        Address = renterInfo.Address,
                    };
                    rentersInfo.Add(renterTransactionInfo);
                }
            }
            else
            {
                var renters = await this.renterRepository
                        .GetRenters(1, 5, false);
                        
                var rentersToAdd = renters.Select(x => new RenterTransactionInfo
                            {
                                RenterId = (int)x.ID,
                                Name = x.Name,
                                Address = x.Address,
                            }
                        ).ToList();

                rentersInfo.AddRange(rentersToAdd);
            }

            var leasersInfo = new List<LeaserTransactionInfo>();
            if (leaserId.HasValue)
            {
                var leaserInfo = await this.leaserRepository
                    .GetLeaserByIdAsync(leaserId.Value);

                if (leaserInfo != null)
                {
                    var leaserTransactionInfo = new LeaserTransactionInfo
                        {
                            LeaserId = leaserInfo.ID,
                            Address = leaserInfo.Address,
                            Name = leaserInfo.Name,
                        };
                    leasersInfo.Add(leaserTransactionInfo);
                }
            }
            else
            {
                var leasers = await this.leaserRepository
                    .GetLeasers(1, 5, false);
                    
                var leasersToAdd = leasers.Select(x => new LeaserTransactionInfo
                        {
                            LeaserId = x.ID,
                            Name = x.Name,
                            Address = x.Address,
                        }).ToList();

                leasersInfo.AddRange(leasersToAdd);
            }

            return new RenterLeaserTransactionCreateDto
            {
                Leasers = leasersInfo,
                Renters = rentersInfo,
            };
        }

        internal bool CancelLeaserTransaction(int leaserId)
        {
            var leaserTransactions = this.GetAllLeaserTransactionsInternal(leaserId);
            foreach(var transaction in leaserTransactions)
            {
                transaction.LastModifiedDate = DateTime.Now;
                transaction.TransactionStatus = RenterLeaserTransactionStatus.NoDeal;
            }

            var leaserTask = this.leaserRepository
                    .GetLeaserByIdAsync(leaserId);

            leaserTask.Wait();
            var leaser = leaserTask.Result;
            leaser.AssetInfo.IsLeased = true;
            leaser.AssetInfo.LeasedDate = DateTime.Now;

            return this.repositoryWrapper.Save();
        }

        internal bool CancelRenterTransaction(int renterId)
        {
            var renterTransactions
                = this.GetAllRenterTransactionsInternal(renterId);
            foreach (var transaction in renterTransactions)
            {
                transaction.LastModifiedDate = DateTime.Now;
                transaction.TransactionStatus = RenterLeaserTransactionStatus.NoDeal;
            }

            var renterTask = this.renterRepository
                .GetRenterByIdAsync(renterId);
            renterTask.Wait();
            var renter = renterTask.Result;
            renter.Found = true;
            renter.FoundDate = DateTime.Now;

            return this.repositoryWrapper.Save();
        }

        internal bool SaveChanges()
        {
            var task = this.transactionRepository.SaveChangesAsync();
            task.Wait();
            return task.Result;
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

        public async Task<bool> UpdateTransaction(int transactionId,RenterLeaserTransactionStatus renterLeaserTransactionStatus)
        {
            var transaction = await this.transactionRepository
                .GetTransactionByIdAsync(transactionId);

            if(transaction != null)
            {
                transaction.TransactionStatus = renterLeaserTransactionStatus;
                transaction.LastModifiedDate = DateTime.Now;

                this.transactionRepository.ModifyTransaction(transaction);
                if(renterLeaserTransactionStatus == RenterLeaserTransactionStatus.Deal)
                {
                    await this.ModifyRemainingTransactions(transaction);
                }

                await this.transactionRepository.SaveChangesAsync();

                return true;
            }
            return false;
        }

        private async Task ModifyRemainingTransactions(RenterLeaserTransaction currentTransaction)
        {
            var renterId = currentTransaction.RenterId;
            var leaserId = currentTransaction.LeaserId;

            await this.renterService.MarkAsFound(renterId,false);
            await this.leaserService.MarkAsLeased(leaserId, false);

            var remainingTransactions = this.transactionRepository
                .Transactions
                .Where(x => x.ID != currentTransaction.ID
                    && x.TransactionStatus == RenterLeaserTransactionStatus.Pending
                && (
                    (x.LeaserId == leaserId) || (x.RenterId == renterId)
                )).ToList();

            foreach(var remainingTransaction in remainingTransactions)
            {
                remainingTransaction.TransactionStatus = RenterLeaserTransactionStatus.NoDeal;
                remainingTransaction.LastModifiedDate = DateTime.Now;
            }            
        }

        private IEnumerable<RenterLeaserTransaction> GetRemainingLeaserTransactions(int transactionId, int leaserId)
        {
            var remainingLeasers = this.transactionRepository
                .Transactions
                .Where(x => x.ID != transactionId
                && x.LeaserId == leaserId)
                .ToList();

            return remainingLeasers;
        }

        private IEnumerable<RenterLeaserTransaction> GetRemainingRenterTransactions(int transactionId, int renterId)
        {
            var remainingRenters = this.transactionRepository
                .Transactions
                .Where(x => x.ID != transactionId
                && x.RenterId == renterId)
                .ToList();

            return remainingRenters;
        }

        public IEnumerable<RenterLeaserTransactionDto> GetTransactions()
        {
            return this.transactionRepository
                .Transactions
                .Where(x => x.TransactionStatus == RenterLeaserTransactionStatus.Pending)
                .Select(x => new RenterLeaserTransactionDto
                {
                    Id = x.ID,
                    CreatedDate = x.CreatedDate,
                    LastModifiedDate = x.LastModifiedDate,
                    TransactionStatus = x.TransactionStatus,
                    LeaserName =  this.leaserRepository.Leaser.
                        Where(y => y.ID == x.LeaserId).FirstOrDefault()?.Name ?? "N/A",
                    RenterName = this.renterRepository.Renters.
                        Where(y => y.ID == x.RenterId).FirstOrDefault()?.Name ?? "N/A",
                }).ToList();
        }

        public IEnumerable<RenterLeaserTransactionDto> GetAllRenterTransactions(int renterId, bool showAll = false)
        {
            var transactions = GetAllLeaserTransactionsInternal(renterId, showAll);
            return transactions
                .Select(x => new RenterLeaserTransactionDto
                {
                    Id = x.ID,
                    CreatedDate = x.CreatedDate,
                    LastModifiedDate = x.LastModifiedDate,
                    TransactionStatus = x.TransactionStatus,
                    LeaserName = this.leaserRepository
                        .Leaser.Where(y => y.ID == x.LeaserId).FirstOrDefault()?.Name ?? "N/A",
                    RenterName = this.renterRepository
                        .Renters.Where(y => y.ID == x.RenterId).FirstOrDefault()?.Name ?? "N/A",
                });
        }

        private IEnumerable<RenterLeaserTransaction> GetAllRenterTransactionsInternal(int renterId, bool showAll = false)
        {
            return this.transactionRepository
                .Transactions
                .Where(x => x.RenterId == renterId &&
                    (showAll || x.TransactionStatus == RenterLeaserTransactionStatus.Pending));
        }

        private IEnumerable<RenterLeaserTransaction> GetAllLeaserTransactionsInternal(int leaserId, bool showAll = false)
        {
            return this.transactionRepository
                .Transactions
                .Where(x => x.LeaserId == leaserId &&
                    (showAll || x.TransactionStatus == RenterLeaserTransactionStatus.Pending));
        }

        public IEnumerable<RenterLeaserTransactionDto> GetAllLeaserTransactions(int leaserId, bool showAll = false)
        {
            var transactions = GetAllLeaserTransactionsInternal(leaserId, showAll);
            return transactions
                .Select(x => new RenterLeaserTransactionDto
                {
                    Id = x.ID,
                    CreatedDate = x.CreatedDate,
                    LastModifiedDate = x.LastModifiedDate,
                    TransactionStatus = x.TransactionStatus,
                    LeaserName = this.leaserRepository
                        .Leaser.Where(y => y.ID == x.LeaserId).FirstOrDefault()?.Name ?? "N/A",
                    RenterName = this.renterRepository
                        .Renters.Where(y => y.ID == x.RenterId).FirstOrDefault()?.Name ?? "N/A",
                });
        }
    }
}
