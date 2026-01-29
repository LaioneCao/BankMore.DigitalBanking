using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankMore.Transfers.Domain.Services
{
    public interface IAccountsClient
    {
        Task DebitAsync(string token, string requisitionId, decimal valor);
        Task CreditAsync(string token, string requisitionId, int numeroContaDestino, decimal valor);
        Task CreditSelfAsync(string token, string requisitionId, decimal valor);
    }

}
