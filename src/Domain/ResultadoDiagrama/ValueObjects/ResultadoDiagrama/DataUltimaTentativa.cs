using Shared.Attributes;
using Shared.Enums;
using Shared.Exceptions;

namespace Domain.ResultadoDiagrama.ValueObjects.ResultadoDiagrama;

[ValueObject]
public record DataUltimaTentativa
{
    private readonly DateTimeOffset _valor;

    private DataUltimaTentativa() { }

    public DataUltimaTentativa(DateTimeOffset valor)
    {
        if (valor == default)
            throw new DomainException("Data de última tentativa inválida", ErrorType.InvalidInput);

        _valor = valor;
    }

    public DateTimeOffset Valor => _valor;
}
