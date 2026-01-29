using MediatR;

namespace BankMore.Accounts.Application.Commands.CloseAccount
{

    public sealed class CloseAccountCommand : IRequest<Unit>
    {
        public Guid ContaId { get; init; }
        public string Senha { get; init; } = string.Empty;
    }
}
