using Domain.ResultadoDiagrama.Enums;
using Shared.Attributes;
using Shared.Enums;
using Shared.Exceptions;

namespace Domain.ResultadoDiagrama.ValueObjects.RelatorioGerado;

[ValueObject]
public record Status
{
    private readonly StatusRelatorioEnum _valor;

    private Status() { }

    public Status(StatusRelatorioEnum valor)
    {
        if (!Enum.IsDefined(valor))
            throw new DomainException($"Status do relatório '{valor}' é inválido", ErrorType.InvalidInput);

        _valor = valor;
    }

    public StatusRelatorioEnum Valor => _valor;
}