namespace BankMore.Accounts.Api.Domain.Entities;

public class ContaCorrente
{
    public Guid Id { get; private set; }
    public int Numero { get; private set; }
    public string CPFTitular { get; private set; } = string.Empty;
    public string NomeTitular { get; private set; } = string.Empty;
    public bool Ativa { get; private set; }
    public string SenhaHash { get; private set; } = string.Empty;
    public string Salt { get; private set; } = string.Empty;

    protected ContaCorrente() { }

    public ContaCorrente(
        int numero,
        string cpfTitular,
        string nomeTitular,
        string senhaHash,
        string salt)
    {
        if (numero <= 0)
            throw new InvalidOperationException("Número da conta inválido.");

        if (!CpfEhValido(cpfTitular))
            throw new InvalidOperationException("CPF do titular inválido.");

        if (string.IsNullOrWhiteSpace(nomeTitular))
            throw new InvalidOperationException("Nome do titular é obrigatório.");

        if (string.IsNullOrWhiteSpace(senhaHash) || string.IsNullOrWhiteSpace(salt))
            throw new InvalidOperationException("Senha inválida (hash/salt ausentes).");

        Id = Guid.NewGuid();
        Numero = numero;
        CPFTitular = new string(cpfTitular.Where(char.IsDigit).ToArray());
        NomeTitular = nomeTitular.Trim();
        SenhaHash = senhaHash;
        Salt = salt;
        Ativa = true;
    }


    public void Inativar() => Ativa = false;






    private static bool CpfEhValido(string cpf)
    {
        if (string.IsNullOrWhiteSpace(cpf))
            return false;

        cpf = new string(cpf.Where(char.IsDigit).ToArray());

        if (cpf.Length != 11)
            return false;

        // Rejeita CPFs com todos os dígitos iguais
        if (cpf.Distinct().Count() == 1)
            return false;

        var numeros = cpf.Select(c => int.Parse(c.ToString())).ToArray();

        // Primeiro dígito verificador
        var soma = 0;
        for (var i = 0; i < 9; i++)
            soma += numeros[i] * (10 - i);

        var resto = soma % 11;
        var digito1 = resto < 2 ? 0 : 11 - resto;

        if (numeros[9] != digito1)
            return false;

        // Segundo dígito verificador
        soma = 0;
        for (var i = 0; i < 10; i++)
            soma += numeros[i] * (11 - i);

        resto = soma % 11;
        var digito2 = resto < 2 ? 0 : 11 - resto;

        return numeros[10] == digito2;
    }


    public static ContaCorrente Rehidratar(
    Guid id,
    int numero,
    string cpfTitular,
    string nomeTitular,
    bool ativa,
    string senhaHash,
    string salt)
    {
        var conta = new ContaCorrente(numero, cpfTitular, nomeTitular, senhaHash, salt);

        // sobrescreve o Id gerado no construtor
        conta.Id = id;

        conta.Ativa = ativa;

        return conta;
    }

}
