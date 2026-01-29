using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankMore.Accounts.Application.Queries.Balance
{
    public sealed class GetBalanceQuery
    {
        public Guid ContaId { get; init; }
    }
}
