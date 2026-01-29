using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankMore.Accounts.Domain.Services
{
    public interface IJwtTokenService
    {
        string GenerateToken(Guid contaId, int numeroConta);
    }
}
