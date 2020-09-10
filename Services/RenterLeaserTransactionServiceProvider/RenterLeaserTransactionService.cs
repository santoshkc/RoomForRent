using Microsoft.EntityFrameworkCore;
using RoomForRent.Models;
using RoomForRent.Models.ViewModel;
using RoomForRent.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RoomForRent.Services.RenterLeaserTransactionServiceProvider
{
    public class RenterLeaserTransactionService
    {
        private readonly ITransactionRepository transactionRepository;

        public RenterLeaserTransactionService(ITransactionRepository transactionRepository)
        {
            this.transactionRepository = transactionRepository;
        }

        internal async Task<RenterLeasersLinkDto> GetPotentialLeasersDto(int renterId)
        {
            var renter = await this.transactionRepository
                            .RenterRepository
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
            var leaser = await this.transactionRepository.
                                LeaserRepository
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
            var leaser = await this.transactionRepository
                .LeaserRepository.GetLeaserByIdAsync(leaserId);

            if(leaser == null)
            {
                return false;
            }

            var renter = await this.transactionRepository
                .RenterRepository.GetRenterByIdAsync(renterId);

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
                var renterInfo = await this.transactionRepository
                    .RenterRepository
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
                var renters = await this.transactionRepository
                        .RenterRepository
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
                var leaserInfo = await this.transactionRepository
                    .LeaserRepository.GetLeaserByIdAsync(leaserId.Value);

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
                var leasers = await this.transactionRepository
                    .LeaserRepository
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

                await this.transactionRepository.SaveChangesAsync();

                return true;
            }
            return false;
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
                    LeaserName =  this.transactionRepository
                    .LeaserRepository.Leaser.
                    Where(y => y.ID == x.LeaserId).FirstOrDefault()?.Name ?? "N/A",
                    RenterName = this.transactionRepository
                    .RenterRepository.Renters.
                    Where(y => y.ID == x.RenterId).FirstOrDefault()?.Name ?? "N/A",
                });
        }

        public IEnumerable<RenterLeaserTransactionDto> GetAllRenterTransactions(int renterId, bool showAll = false)
        {
            return this.transactionRepository
                .Transactions
                .Where(x => x.RenterId == renterId && 
                    (showAll || x.TransactionStatus == RenterLeaserTransactionStatus.Pending))
                .Select(x => new RenterLeaserTransactionDto
                {
                    Id = x.ID,
                    CreatedDate = x.CreatedDate,
                    LastModifiedDate = x.LastModifiedDate,
                    TransactionStatus = x.TransactionStatus,
                    LeaserName = this.transactionRepository
                    .LeaserRepository.Leaser.
                    Where(y => y.ID == x.LeaserId).FirstOrDefault()?.Name ?? "N/A",
                    RenterName = this.transactionRepository
                    .RenterRepository.Renters.
                    Where(y => y.ID == x.RenterId).FirstOrDefault()?.Name ?? "N/A",
                });
        }

        public IEnumerable<RenterLeaserTransactionDto> GetAllLeaserTransactions(int leaserId, bool showAll = false)
        {
            return this.transactionRepository
                .Transactions
                .Where(x => x.LeaserId == leaserId &&
                    (showAll || x.TransactionStatus == RenterLeaserTransactionStatus.Pending))
                .Select(x => new RenterLeaserTransactionDto
                {
                    Id = x.ID,
                    CreatedDate = x.CreatedDate,
                    LastModifiedDate = x.LastModifiedDate,
                    TransactionStatus = x.TransactionStatus,
                    LeaserName = this.transactionRepository
                    .LeaserRepository.Leaser.
                    Where(y => y.ID == x.LeaserId).FirstOrDefault()?.Name ?? "N/A",
                    RenterName = this.transactionRepository
                    .RenterRepository.Renters.
                    Where(y => y.ID == x.RenterId).FirstOrDefault()?.Name ?? "N/A",
                });
        }
    }
}
