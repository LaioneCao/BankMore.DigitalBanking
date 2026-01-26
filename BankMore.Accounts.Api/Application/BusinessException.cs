namespace BankMore.Accounts.Api.Application
{
    public sealed class BusinessException : Exception
    {
        public string Type { get; }

        public BusinessException(string message, string type) : base(message)
            => Type = type;
    }
}
