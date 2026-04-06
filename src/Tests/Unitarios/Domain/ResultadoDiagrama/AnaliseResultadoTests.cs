using Domain.ResultadoDiagrama.Entities;

namespace Tests.Domain.ResultadoDiagrama;

public class AnaliseResultadoTests
{
    [Fact(DisplayName = "Deve criar análise quando dados são válidos")]
    [Trait("Entity", "AnaliseResultado")]
    public void Criar_DeveCriarComSucesso_QuandoDadosValidos()
    {
        // Arrange
        var descricaoAnalise = " Análise gerada ";
        var componentes = new List<string> { " API " };
        var riscos = new List<string> { " Acoplamento " };
        var recomendacoes = new List<string> { " Refatorar " };

        // Act
        var analiseResultado = AnaliseResultado.Criar(descricaoAnalise, componentes, riscos, recomendacoes);

        // Assert
        analiseResultado.DeveConterContagens("Análise gerada", 1, 1, 1);
    }

    [Fact(DisplayName = "Deve lançar exceção quando descrição é inválida")]
    [Trait("Entity", "AnaliseResultado")]
    public void Criar_DeveLancarExcecao_QuandoDescricaoInvalida()
    {
        // Arrange
        Action acao = () => AnaliseResultado.Criar("", ["API"], ["Risco"], ["Recomendação"]);

        // Act & Assert
        acao.DeveLancarExcecaoDeValidacao("Descrição da análise");
    }
}