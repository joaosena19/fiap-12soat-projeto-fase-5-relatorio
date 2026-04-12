using Domain.ResultadoDiagrama.Aggregates;

namespace Tests.Helpers.MockExtensions;

public static class ResultadoDiagramaGatewayMockExtensions
{
    public static ObterPorAnaliseDiagramaIdSetup AoObterPorAnaliseDiagramaId(this Mock<IResultadoDiagramaGateway> mock, Guid analiseDiagramaId)
    {
        return new ObterPorAnaliseDiagramaIdSetup(mock, analiseDiagramaId);
    }

    public static SalvarSetup AoSalvar(this Mock<IResultadoDiagramaGateway> mock)
    {
        return new SalvarSetup(mock);
    }

    public static ListarSetup AoListar(this Mock<IResultadoDiagramaGateway> mock)
    {
        return new ListarSetup(mock);
    }

    public static void DeveTerSalvo(this Mock<IResultadoDiagramaGateway> mock)
    {
        mock.Verify(x => x.SalvarAsync(It.IsAny<ResultadoDiagrama>()), Times.AtLeastOnce);
    }

    public static void DeveTerSalvo(this Mock<IResultadoDiagramaGateway> mock, int vezes)
    {
        mock.Verify(x => x.SalvarAsync(It.IsAny<ResultadoDiagrama>()), Times.Exactly(vezes));
    }

    public static void NaoDeveTerSalvo(this Mock<IResultadoDiagramaGateway> mock)
    {
        mock.Verify(x => x.SalvarAsync(It.IsAny<ResultadoDiagrama>()), Times.Never);
    }

    public sealed class ObterPorAnaliseDiagramaIdSetup
    {
        private readonly Mock<IResultadoDiagramaGateway> _mock;
        private readonly Guid _analiseDiagramaId;

        public ObterPorAnaliseDiagramaIdSetup(Mock<IResultadoDiagramaGateway> mock, Guid analiseDiagramaId)
        {
            _mock = mock;
            _analiseDiagramaId = analiseDiagramaId;
        }

        public void Retorna(ResultadoDiagrama resultadoDiagrama)
        {
            _mock.Setup(x => x.ObterPorAnaliseDiagramaIdAsync(_analiseDiagramaId)).ReturnsAsync(resultadoDiagrama);
        }

        public void NaoRetornaNada()
        {
            _mock.Setup(x => x.ObterPorAnaliseDiagramaIdAsync(_analiseDiagramaId)).ReturnsAsync((ResultadoDiagrama?)null);
        }

        public void LancaExcecao(Exception excecao)
        {
            _mock.Setup(x => x.ObterPorAnaliseDiagramaIdAsync(_analiseDiagramaId)).ThrowsAsync(excecao);
        }
    }

    public sealed class SalvarSetup
    {
        private readonly Mock<IResultadoDiagramaGateway> _mock;

        public SalvarSetup(Mock<IResultadoDiagramaGateway> mock)
        {
            _mock = mock;
        }

        public void RetornaMesmoObjeto()
        {
            _mock.Setup(x => x.SalvarAsync(It.IsAny<ResultadoDiagrama>())).ReturnsAsync((ResultadoDiagrama resultadoDiagrama) => resultadoDiagrama);
        }

        public void LancaExcecao(Exception excecao)
        {
            _mock.Setup(x => x.SalvarAsync(It.IsAny<ResultadoDiagrama>())).ThrowsAsync(excecao);
        }
    }

    public sealed class ListarSetup
    {
        private readonly Mock<IResultadoDiagramaGateway> _mock;

        public ListarSetup(Mock<IResultadoDiagramaGateway> mock) => _mock = mock;

        public void Retorna(IReadOnlyCollection<ResultadoDiagrama> resultados)
        {
            _mock.Setup(x => x.ListarAsync()).ReturnsAsync(resultados);
        }

        public void LancaExcecao(Exception excecao)
        {
            _mock.Setup(x => x.ListarAsync()).ThrowsAsync(excecao);
        }
    }
}