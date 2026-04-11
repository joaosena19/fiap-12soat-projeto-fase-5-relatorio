using Domain.ResultadoDiagrama.Enums;
using Shared.Attributes;
using Shared.Enums;
using Shared.Exceptions;

namespace Domain.ResultadoDiagrama.ValueObjects.RelatorioGerado;

[ValueObject]
public record Tipo
{
    private readonly TipoRelatorioEnum _valor;

    private Tipo() { }

    public Tipo(TipoRelatorioEnum valor)
    {
        if (!Enum.IsDefined(valor))
            throw new DomainException($"Tipo de relatório '{valor}' é inválido", ErrorType.InvalidInput);

        _valor = valor;
    }

    public TipoRelatorioEnum Valor => _valor;
}