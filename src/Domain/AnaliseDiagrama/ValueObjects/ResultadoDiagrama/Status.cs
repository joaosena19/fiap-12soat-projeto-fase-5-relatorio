using Domain.AnaliseDiagrama.Enums;
using Shared.Attributes;
using Shared.Enums;
using Shared.Exceptions;

namespace Domain.AnaliseDiagrama.ValueObjects.ResultadoDiagrama;

[ValueObject]
public record Status
{
    public StatusAnaliseEnum Valor { get; init; }

    private Status()
    {
        Valor = default;
    }

    public Status(StatusAnaliseEnum valor)
    {
        if (!Enum.IsDefined(valor))
            throw new DomainException($"Status de análise '{valor}' é inválido", ErrorType.InvalidInput);

        Valor = valor;
    }
}
