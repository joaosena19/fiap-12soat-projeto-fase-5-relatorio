using Domain.AnaliseDiagrama.Enums;
using Shared.Attributes;
using Shared.Enums;
using Shared.Exceptions;

namespace Domain.AnaliseDiagrama.ValueObjects.RelatorioGerado;

[ValueObject]
public record Status
{
    public StatusRelatorioEnum Valor { get; init; }

    private Status()
    {
        Valor = default;
    }

    public Status(StatusRelatorioEnum valor)
    {
        if (!Enum.IsDefined(valor))
            throw new DomainException($"Status do relatório '{valor}' é inválido", ErrorType.InvalidInput);

        Valor = valor;
    }
}