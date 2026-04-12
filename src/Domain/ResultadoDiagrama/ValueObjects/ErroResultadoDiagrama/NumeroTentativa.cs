using Shared.Attributes;

namespace Domain.ResultadoDiagrama.ValueObjects.ErroResultadoDiagrama;

[ValueObject]
public record NumeroTentativa
{
    private readonly int? _valor;

    private NumeroTentativa() { }

    public NumeroTentativa(int? valor)
    {
        _valor = valor;
    }

    public int? Valor => _valor;
}
