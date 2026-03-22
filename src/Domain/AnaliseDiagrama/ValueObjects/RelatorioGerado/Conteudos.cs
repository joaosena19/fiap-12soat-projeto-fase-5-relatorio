using Shared.Attributes;
using Shared.Enums;
using Shared.Exceptions;

namespace Domain.AnaliseDiagrama.ValueObjects.RelatorioGerado;

[ValueObject]
public record Conteudos
{
    public IReadOnlyDictionary<string, string> Valores { get; init; }

    private Conteudos()
    {
        Valores = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
    }

    private Conteudos(IDictionary<string, string> valores)
    {
        Valores = new Dictionary<string, string>(valores, StringComparer.OrdinalIgnoreCase);
    }

    public static Conteudos Vazio() => new();

    /// <summary>
    /// Cria uma coleção de conteúdos a partir de um dicionário existente.
    /// </summary>
    public static Conteudos Criar(IDictionary<string, string> valores)
    {
        if (valores == null || valores.Count == 0)
            throw new DomainException("Os conteúdos não podem ser nulos ou vazios.", ErrorType.InvalidInput);

        if (valores.Any(item => string.IsNullOrWhiteSpace(item.Key)))
            throw new DomainException("Os conteúdos possuem chaves inválidas ou vazias.", ErrorType.InvalidInput);

        if (valores.Any(item => item.Value == null))
            throw new DomainException("Os conteúdos possuem valores nulos.", ErrorType.InvalidInput);

        return new Conteudos(valores);
    }

    /// <summary>
    /// Adiciona um novo par de chave/valor mantendo a imutabilidade do record.
    /// </summary>
    public Conteudos Adicionar(string chave, string valor)
    {
        if (string.IsNullOrWhiteSpace(chave))
            throw new DomainException("A chave do conteúdo não pode ser nula ou vazia.", ErrorType.InvalidInput);

        if (valor == null)
            throw new DomainException($"O valor para a chave '{chave}' não pode ser nulo.", ErrorType.InvalidInput);

        var novoDicionario = new Dictionary<string, string>(Valores, StringComparer.OrdinalIgnoreCase)
        {
            [chave] = valor
        };

        return new Conteudos(novoDicionario);
    }

    public string? ObterValor(string chave)
    {
        return Valores.TryGetValue(chave, out var valor) ? valor : null;
    }

    public bool ContemChave(string chave)
    {
        return Valores.ContainsKey(chave);
    }
}