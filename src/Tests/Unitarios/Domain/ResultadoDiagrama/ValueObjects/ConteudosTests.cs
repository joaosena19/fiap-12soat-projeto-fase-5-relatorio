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
        conteudos.DeveEstarVazio();
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
        conteudos.DeveConterConteudo("chave", "valor");
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
        conteudos.NaoDeveConterChave("b");
        novoConteudos.DeveConterChave("b");
        novoConteudos.DeveConterConteudo("b", "2");
    }

    [Fact(DisplayName = "Não deve criar Conteudos com chave vazia no dicionário")]
    [Trait("ValueObject", "Conteudos")]
    public void Criar_DeveLancarExcecao_QuandoDicionarioContemChaveVazia()
    {
        // Arrange
        Action acao = () => _ = Conteudos.Criar(new Dictionary<string, string> { [""] = "valor" });

        // Act & Assert
        acao.DeveLancarExcecaoDeValidacao("chaves inválidas");
    }

    [Fact(DisplayName = "Não deve criar Conteudos com valor nulo no dicionário")]
    [Trait("ValueObject", "Conteudos")]
    public void Criar_DeveLancarExcecao_QuandoDicionarioContemValorNulo()
    {
        // Arrange
        Action acao = () => _ = Conteudos.Criar(new Dictionary<string, string> { ["chave"] = null! });

        // Act & Assert
        acao.DeveLancarExcecaoDeValidacao("valores nulos");
    }

    [Fact(DisplayName = "Não deve adicionar conteúdo com chave vazia")]
    [Trait("ValueObject", "Conteudos")]
    public void Adicionar_DeveLancarExcecao_QuandoChaveVazia()
    {
        // Arrange
        var conteudos = Conteudos.Vazio();
        Action acao = () => _ = conteudos.Adicionar(string.Empty, "valor");

        // Act & Assert
        acao.DeveLancarExcecaoDeValidacao("chave do conteúdo não pode ser nula");
    }

    [Fact(DisplayName = "Não deve adicionar conteúdo com valor nulo")]
    [Trait("ValueObject", "Conteudos")]
    public void Adicionar_DeveLancarExcecao_QuandoValorNulo()
    {
        // Arrange
        var conteudos = Conteudos.Vazio();
        Action acao = () => _ = conteudos.Adicionar("chave", null!);

        // Act & Assert
        acao.DeveLancarExcecaoDeValidacao("não pode ser nulo");
    }
}