using Domain.ResultadoDiagrama.Enums;
using Shared.Attributes;
using Shared.Enums;
using Shared.Exceptions;

namespace Domain.ResultadoDiagrama.ValueObjects.ErroResultadoDiagrama;

[ValueObject]
public record TipoRelatorio
{
    private readonly TipoRelatorioEnum? _valor;

    private TipoRelatorio() { }

    public TipoRelatorio(TipoRelatorioEnum? valor)
    {
        if (valor.HasValue && !Enum.IsDefined(valor.Value))
            throw new DomainException($"Tipo de relatório '{valor}' é inválido", ErrorType.InvalidInput);

        _valor = valor;
    }

    public TipoRelatorioEnum? Valor => _valor;
}
