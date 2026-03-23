using Shared.Attributes;
using Shared.Enums;
using Shared.Exceptions;

namespace Domain.ResultadoDiagrama.ValueObjects.ErroResultadoDiagrama;

[ValueObject]
public record DataOcorrencia
{
    private readonly DateTimeOffset _valor;

    private DataOcorrencia() { }

    public DataOcorrencia(DateTimeOffset valor)
    {
        if (valor == default)
            throw new DomainException("Data de ocorrência inválida", ErrorType.InvalidInput);

        _valor = valor;
    }

    public DateTimeOffset Valor => _valor;
}
