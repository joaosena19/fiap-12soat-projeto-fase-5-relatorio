using Domain.ResultadoDiagrama.ValueObjects.RelatorioGerado;

namespace Tests.Domain.ResultadoDiagrama.ValueObjects;

public class ConteudosTests
{
    [Fact(DisplayName = "Deve criar conteúdo vazio")]
    [Trait("ValueObject", "Conteudos")]
    public void Vazio_DeveCriarColecaoVazia_QuandoChamado()
    {
        // Act
        var conteudos = Conteudos.Vazio();

        // Assert
        conteudos.Valores.Count.ShouldBe(0);
    }

    [Fact(DisplayName = "Deve criar conteúdo com dicionário válido")]
    [Trait("ValueObject", "Conteudos")]
    public void Criar_DeveCriar_QuandoDicionarioValido()
    {
        // Arrange
        var valores = new Dictionary<string, string> { ["chave"] = "valor" };

        // Act
        var conteudos = Conteudos.Criar(valores);

        // Assert
        conteudos.ObterValor("chave").ShouldBe("valor");
    }

    [Fact(DisplayName = "Deve lançar exceção quando dicionário é vazio")]
    [Trait("ValueObject", "Conteudos")]
    public void Criar_DeveLancarExcecao_QuandoDicionarioVazio()
    {
        // Arrange
        Action acao = () => _ = Conteudos.Criar(new Dictionary<string, string>());

        // Act & Assert
        acao.DeveLancarExcecaoDeValidacao("não podem ser nulos ou vazios");
    }

    [Fact(DisplayName = "Deve adicionar conteúdo mantendo imutabilidade")]
    [Trait("ValueObject", "Conteudos")]
    public void Adicionar_DeveRetornarNovaInstancia_QuandoChamado()
    {
        // Arrange
        var conteudos = Conteudos.Criar(new Dictionary<string, string> { ["a"] = "1" });

        // Act
        var novoConteudos = conteudos.Adicionar("b", "2");

        // Assert
        conteudos.ContemChave("b").ShouldBeFalse();
        novoConteudos.ContemChave("b").ShouldBeTrue();
        novoConteudos.ObterValor("b").ShouldBe("2");
    }
}