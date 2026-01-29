using MediatR;

namespace BankMore.Accounts.Application.Commands.OpenAccount
{
    public class OpenAccountCommand : IRequest<OpenAccountResult>
    {
        public string NomeTitular { get; init; } = string.Empty;
        public string CPFTitular { get; init; } = string.Empty;
        public string Senha { get; init; } = string.Empty;
    }

}
