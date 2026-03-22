using Domain.ResultadoDiagrama.Enums;
using Shared.Attributes;
using Shared.Enums;
using Shared.Exceptions;

namespace Domain.ResultadoDiagrama.ValueObjects.ResultadoDiagrama;

[ValueObject]
public record Status
{
    private readonly StatusAnaliseEnum _valor;

    private Status() { }

    public Status(StatusAnaliseEnum valor)
    {
        if (!Enum.IsDefined(valor))
            throw new DomainException($"Status de análise '{valor}' é inválido", ErrorType.InvalidInput);

        _valor = valor;
    }

    public StatusAnaliseEnum Valor => _valor;
}
