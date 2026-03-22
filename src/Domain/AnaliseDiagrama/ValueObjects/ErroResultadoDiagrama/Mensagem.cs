using Shared.Attributes;
using Shared.Enums;
using Shared.Exceptions;

namespace Domain.AnaliseDiagrama.ValueObjects.ErroResultadoDiagrama;

[ValueObject]
public record Mensagem
{
    private readonly string _valor = string.Empty;

    private Mensagem() { }

    public Mensagem(string valor)
    {
        if (string.IsNullOrWhiteSpace(valor))
            throw new DomainException("Mensagem de erro não pode ser vazia", ErrorType.InvalidInput);

        _valor = valor.Trim();
    }

    public string Valor => _valor;
}
