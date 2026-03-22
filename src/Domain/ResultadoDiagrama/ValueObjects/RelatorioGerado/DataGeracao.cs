using Shared.Attributes;
using Shared.Enums;
using Shared.Exceptions;

namespace Domain.AnaliseDiagrama.ValueObjects.RelatorioGerado;

[ValueObject]
public record DataGeracao
{
    public DateTimeOffset Valor { get; init; }

    private DataGeracao()
    {
        Valor = default;
    }

    public DataGeracao(DateTimeOffset valor)
    {
        if (valor == default)
            throw new DomainException("Data de geração inválida", ErrorType.InvalidInput);

        Valor = valor;
    }
}