using ConteudosRelatorio = Domain.ResultadoDiagrama.ValueObjects.RelatorioGerado.Conteudos;

namespace Tests.Helpers.MockExtensions;

public static class RelatorioStrategyMockExtensions
{
    public static ResolverSetup AoResolver(this Mock<IRelatorioStrategyResolver> mock, TipoRelatorioEnum tipoRelatorio)
    {
        return new ResolverSetup(mock, tipoRelatorio);
    }

    public static GerarSetup AoGerar(this Mock<IRelatorioStrategy> mock)
    {
        return new GerarSetup(mock);
    }

    public static void ComTipoRelatorio(this Mock<IRelatorioStrategy> mock, TipoRelatorioEnum tipo)
    {
        mock.Setup(x => x.TipoRelatorio).Returns(tipo);
    }

    public static void DeveTerGerado(this Mock<IRelatorioStrategy> mock)
        => mock.Verify(item => item.GerarAsync(It.IsAny<global::Domain.ResultadoDiagrama.Aggregates.ResultadoDiagrama>()), Times.Once);

    public sealed class ResolverSetup
    {
        private readonly Mock<IRelatorioStrategyResolver> _mock;
        private readonly TipoRelatorioEnum _tipoRelatorio;

        public ResolverSetup(Mock<IRelatorioStrategyResolver> mock, TipoRelatorioEnum tipoRelatorio)
        {
            _mock = mock;
            _tipoRelatorio = tipoRelatorio;
        }

        public void Retorna(IRelatorioStrategy strategy)
        {
            _mock.Setup(x => x.Resolver(_tipoRelatorio)).Returns(strategy);
        }

        public void LancaExcecao(Exception excecao)
        {
            _mock.Setup(x => x.Resolver(_tipoRelatorio)).Throws(excecao);
        }
    }

    public sealed class GerarSetup
    {
        private readonly Mock<IRelatorioStrategy> _mock;

        public GerarSetup(Mock<IRelatorioStrategy> mock)
        {
            _mock = mock;
        }

        public void Retorna(ConteudosRelatorio conteudos)
        {
            _mock.Setup(x => x.GerarAsync(It.IsAny<global::Domain.ResultadoDiagrama.Aggregates.ResultadoDiagrama>())).ReturnsAsync(conteudos);
        }

        public void LancaExcecao(Exception excecao)
        {
            _mock.Setup(x => x.GerarAsync(It.IsAny<global::Domain.ResultadoDiagrama.Aggregates.ResultadoDiagrama>())).ThrowsAsync(excecao);
        }
    }
}