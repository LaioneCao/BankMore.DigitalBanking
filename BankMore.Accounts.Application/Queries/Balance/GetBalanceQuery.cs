using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankMore.Accounts.Application.Queries.Balance
{
    public sealed class GetBalanceQuery : IRequest<GetBalanceResult>
    {
        public Guid ContaId { get; init; }
    }
}
