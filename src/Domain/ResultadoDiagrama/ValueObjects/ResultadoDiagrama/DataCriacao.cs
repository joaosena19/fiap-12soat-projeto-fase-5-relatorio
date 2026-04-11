using Shared.Attributes;
using Shared.Enums;
using Shared.Exceptions;

namespace Domain.ResultadoDiagrama.ValueObjects.ResultadoDiagrama;

[ValueObject]
public record DataCriacao
{
    private readonly DateTimeOffset _valor;

    private DataCriacao() { }

    public DataCriacao(DateTimeOffset valor)
    {
        if (valor == default)
            throw new DomainException("Data de criação inválida", ErrorType.InvalidInput);

        _valor = valor;
    }

    public DateTimeOffset Valor => _valor;
}
