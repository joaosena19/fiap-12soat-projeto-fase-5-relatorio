using Domain.AnaliseDiagrama.Enums;
using Shared.Attributes;
using Shared.Enums;
using Shared.Exceptions;

namespace Domain.AnaliseDiagrama.ValueObjects.ErroResultadoDiagrama;

[ValueObject]
public record TipoRelatorio
{
    public TipoRelatorioEnum? Valor { get; init; }

    private TipoRelatorio()
    {
        Valor = null;
    }

    public TipoRelatorio(TipoRelatorioEnum? valor)
    {
        if (valor.HasValue && !Enum.IsDefined(valor.Value))
            throw new DomainException($"Tipo de relatório '{valor}' é inválido", ErrorType.InvalidInput);

        Valor = valor;
    }
}
