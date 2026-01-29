namespace BankMore.Accounts.Application.ListMovements
{
    public class MovementListItem
    {
        public string IdMovimento { get; init; } = string.Empty;
        public string DataMovimento { get; init; } = string.Empty;
        public string TipoMovimento { get; init; } = string.Empty; // "C" ou "D"
        public decimal Valor { get; init; }
    }
}
