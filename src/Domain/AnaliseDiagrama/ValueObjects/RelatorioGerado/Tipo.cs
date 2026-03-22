using Domain.AnaliseDiagrama.Enums;
using Shared.Attributes;
using Shared.Enums;
using Shared.Exceptions;

namespace Domain.AnaliseDiagrama.ValueObjects.RelatorioGerado;

[ValueObject]
public record Tipo
{
    public TipoRelatorioEnum Valor { get; init; }

    private Tipo()
    {
        Valor = default;
    }

    public Tipo(TipoRelatorioEnum valor)
    {
        if (!Enum.IsDefined(valor))
            throw new DomainException($"Tipo de relatório '{valor}' é inválido", ErrorType.InvalidInput);

        Valor = valor;
    }
}