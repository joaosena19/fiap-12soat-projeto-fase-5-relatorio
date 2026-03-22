using Shared.Attributes;
using Shared.Enums;
using Shared.Exceptions;

namespace Domain.AnaliseDiagrama.ValueObjects.ResultadoDiagrama;

[ValueObject]
public record DataCriacao
{
    public DateTimeOffset Valor { get; init; }

    private DataCriacao()
    {
        Valor = default;
    }

    public DataCriacao(DateTimeOffset valor)
    {
        if (valor == default)
            throw new DomainException("Data de criação inválida", ErrorType.InvalidInput);

        Valor = valor;
    }
}
