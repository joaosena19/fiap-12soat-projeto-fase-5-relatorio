using Domain.ResultadoDiagrama.Enums;
using Shared.Attributes;

namespace Domain.ResultadoDiagrama.ValueObjects.ErroResultadoDiagrama;

[ValueObject]
public record OrigemErro
{
    private readonly OrigemErroEnum? _valor;

    private OrigemErro() { }

    public OrigemErro(OrigemErroEnum? valor)
    {
        _valor = valor;
    }

    public OrigemErroEnum? Valor => _valor;
}
