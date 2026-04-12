using Domain.ResultadoDiagrama.Aggregates;

namespace Tests.Helpers.MockExtensions;

public static class ListarResultadosDiagramaPresenterMockExtensions
{
    public static void DeveTerApresentadoSucesso(this Mock<IListarResultadosDiagramaPresenter> mock)
    {
        mock.Verify(x => x.ApresentarSucesso(It.IsAny<IReadOnlyCollection<ResultadoDiagrama>>()), Times.Once);
    }

    public static void NaoDeveTerApresentadoSucesso(this Mock<IListarResultadosDiagramaPresenter> mock)
    {
        mock.Verify(x => x.ApresentarSucesso(It.IsAny<IReadOnlyCollection<ResultadoDiagrama>>()), Times.Never);
    }

    public static void DeveTerApresentadoErro(this Mock<IListarResultadosDiagramaPresenter> mock, ErrorType? errorType = null)
    {
        if (errorType.HasValue)
            mock.Verify(x => x.ApresentarErro(It.IsAny<string>(), errorType.Value), Times.AtLeastOnce);
        else
            mock.Verify(x => x.ApresentarErro(It.IsAny<string>(), It.IsAny<ErrorType>()), Times.AtLeastOnce);
    }

    public static void NaoDeveTerApresentadoErro(this Mock<IListarResultadosDiagramaPresenter> mock)
    {
        mock.Verify(x => x.ApresentarErro(It.IsAny<string>(), It.IsAny<ErrorType>()), Times.Never);
    }
}
