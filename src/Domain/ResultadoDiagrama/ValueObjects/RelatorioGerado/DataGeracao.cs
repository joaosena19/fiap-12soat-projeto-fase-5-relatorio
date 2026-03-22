using Shared.Attributes;
using Shared.Enums;
using Shared.Exceptions;

namespace Domain.ResultadoDiagrama.ValueObjects.RelatorioGerado;

[ValueObject]
public record DataGeracao
{
    private readonly DateTimeOffset _valor;

    private DataGeracao() { }

    public DataGeracao(DateTimeOffset valor)
    {
        if (valor == default)
            throw new DomainException("Data de geração inválida", ErrorType.InvalidInput);

        _valor = valor;
    }

    public DateTimeOffset Valor => _valor;
}