using Microsoft.AspNetCore.Mvc;
using RoomForRent.Controllers;
using RoomForRent.Repositories;
using RoomForRent.Services.RenterLeaserTransactionServiceProvider;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RoomForRent.Components
{
    public class ActiveTransactionsViewComponent : ViewComponent
    {
        private RenterLeaserTransactionService service = null;

        public ActiveTransactionsViewComponent(IRepositoryWrapper repositoryWrapper)
        {
            service = new RenterLeaserTransactionService(repositoryWrapper);
        }

        public IViewComponentResult Invoke(int id, bool isRenter)
        {
            //var x = this.RouteData?.Values["id"] ?? string.Empty;

            //if (!int.TryParse(x.ToString(), out int renterId))
            //{
            //    renterId = 0;
            //}

            //int id = renterId;
                
            //    bool isRenter = true;

            var transactionViewDto = new TransactionViewDto
            {
                ShowAssociatedLeasers = isRenter,
            };

            if(isRenter)
                transactionViewDto.TransactionDtos = service.GetAllRenterTransactions(id);
            else
                transactionViewDto.TransactionDtos = service.GetAllLeaserTransactions(id);
            
            return View(transactionViewDto);
        }
    }

    public class TransactionViewDto
    {
        public bool ShowAssociatedLeasers { get; set; }
        public IEnumerable<RenterLeaserTransactionDto> TransactionDtos { get; set; }
    }
}
