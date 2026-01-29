using MediatR;

namespace BankMore.Accounts.Application.Commands.Login
{
    public sealed class LoginCommand : IRequest<LoginResult>
    {
        public string? CPFTitular { get; init; }
        public int? NumeroConta { get; init; }
        public string Senha { get; init; } = string.Empty;
    }
}
