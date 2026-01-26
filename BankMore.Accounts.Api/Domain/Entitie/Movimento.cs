using BankMore.Accounts.Api.Domain.Enums;

namespace BankMore.Accounts.Api.Domain.Entities;

public class Movimento
{
    public Guid Id { get; private set; }
    public Guid ContaCorrenteId { get; private set; }
    public DateTime DataMovimento { get; private set; }
    public TipoMovimento Tipo { get; private set; }
    public decimal Valor { get; private set; }

    protected Movimento() { }

    public Movimento(
        Guid contaCorrenteId,
        TipoMovimento tipo,
        decimal valor)
    {
        if (valor <= 0)
            throw new ArgumentException("O valor do movimento deve ser maior que zero.");

        Id = Guid.NewGuid();
        ContaCorrenteId = contaCorrenteId;
        Tipo = tipo;
        Valor = valor;
        DataMovimento = DateTime.UtcNow;
    }
}
