using Shared.Attributes;
using Shared.Enums;
using Shared.Exceptions;

namespace Domain.AnaliseDiagrama.ValueObjects.ErroResultadoDiagrama;

[ValueObject]
public record DataOcorrencia
{
    public DateTimeOffset Valor { get; init; }

    private DataOcorrencia()
    {
        Valor = default;
    }

    public DataOcorrencia(DateTimeOffset valor)
    {
        if (valor == default)
            throw new DomainException("Data de ocorrência inválida", ErrorType.InvalidInput);

        Valor = valor;
    }
}
