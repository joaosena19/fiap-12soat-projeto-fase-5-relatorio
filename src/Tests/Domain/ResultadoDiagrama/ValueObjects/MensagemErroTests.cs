using MensagemErro = Domain.ResultadoDiagrama.ValueObjects.ErroResultadoDiagrama.Mensagem;

namespace Tests.Domain.ResultadoDiagrama.ValueObjects;

public class MensagemErroTests
{
    [Fact(DisplayName = "Deve criar mensagem de erro quando valor é válido")]
    [Trait("ValueObject", "MensagemErro")]
    public void Constructor_DeveCriar_QuandoValorValido()
    {
        // Act
        var mensagemErro = new MensagemErro(" Falha ao processar ");

        // Assert
        mensagemErro.Valor.ShouldBe("Falha ao processar");
    }

    [Fact(DisplayName = "Deve lançar exceção quando mensagem de erro é vazia")]
    [Trait("ValueObject", "MensagemErro")]
    public void Constructor_DeveLancarExcecao_QuandoValorVazio()
    {
        // Arrange
        Action acao = () => _ = new MensagemErro(" ");

        // Act & Assert
        acao.DeveLancarExcecaoDeValidacao("Mensagem de erro");
    }
}