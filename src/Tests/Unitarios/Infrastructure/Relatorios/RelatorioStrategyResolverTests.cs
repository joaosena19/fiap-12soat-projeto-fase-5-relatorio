using Infrastructure.Relatorios;

namespace Tests.Infrastructure.Relatorios;

public class RelatorioStrategyResolverTests
{
    [Fact(DisplayName = "Deve resolver a strategy correta para o tipo informado")]
    [Trait("Infrastructure", "RelatorioStrategyResolver")]
    public void Resolver_DeveRetornarStrategyCorreta_QuandoTipoExiste()
    {
        // Arrange
        var strategyMock = new Mock<IRelatorioStrategy>();
        strategyMock.ComTipoRelatorio(TipoRelatorioEnum.Json);
        var resolver = new RelatorioStrategyResolver(new[] { strategyMock.Object });

        // Act
        var resultado = resolver.Resolver(TipoRelatorioEnum.Json);

        // Assert
        resultado.ShouldBe(strategyMock.Object);
    }

    [Fact(DisplayName = "Deve lançar InvalidOperationException quando tipo não encontrado")]
    [Trait("Infrastructure", "RelatorioStrategyResolver")]
    public void Resolver_DeveLancarExcecao_QuandoTipoNaoEncontrado()
    {
        // Arrange
        var strategyMock = new Mock<IRelatorioStrategy>();
        strategyMock.ComTipoRelatorio(TipoRelatorioEnum.Json);
        var resolver = new RelatorioStrategyResolver(new[] { strategyMock.Object });

        // Act & Assert
        var excecao = Should.Throw<InvalidOperationException>(() => resolver.Resolver(TipoRelatorioEnum.Pdf));
        excecao.Message.ShouldContain("Pdf");
    }

    [Fact(DisplayName = "Deve lançar InvalidOperationException quando nenhuma strategy registrada")]
    [Trait("Infrastructure", "RelatorioStrategyResolver")]
    public void Resolver_DeveLancarExcecao_QuandoNenhumaStrategyRegistrada()
    {
        // Arrange
        var resolver = new RelatorioStrategyResolver(Enumerable.Empty<IRelatorioStrategy>());

        // Act & Assert
        Should.Throw<InvalidOperationException>(() => resolver.Resolver(TipoRelatorioEnum.Json));
    }
}
