namespace BankMore.Accounts.Application.OpenAccount
{
    public class OpenAccountCommand
    {
        public string NomeTitular { get; init; } = string.Empty;
        public string CPFTitular { get; init; } = string.Empty;
        public string Senha { get; init; } = string.Empty;
    }

}
