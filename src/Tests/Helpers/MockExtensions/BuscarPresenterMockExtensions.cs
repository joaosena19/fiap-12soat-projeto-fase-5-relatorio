namespace Tests.Helpers.MockExtensions;

public static class BuscarPresenterMockExtensions
{
    public static void DeveTerApresentadoSucesso(this Mock<IBuscarResultadoDiagramaPresenter> mock)
    {
        mock.Verify(x => x.ApresentarSucesso(It.IsAny<global::Domain.ResultadoDiagrama.Aggregates.ResultadoDiagrama>()), Times.AtLeastOnce);
    }

    public static void NaoDeveTerApresentadoSucesso(this Mock<IBuscarResultadoDiagramaPresenter> mock)
    {
        mock.Verify(x => x.ApresentarSucesso(It.IsAny<global::Domain.ResultadoDiagrama.Aggregates.ResultadoDiagrama>()), Times.Never);
    }

    public static void DeveTerApresentadoErro(this Mock<IBuscarResultadoDiagramaPresenter> mock, ErrorType? errorType = null)
    {
        if (errorType == null)
        {
            mock.Verify(x => x.ApresentarErro(It.IsAny<string>(), It.IsAny<ErrorType>()), Times.AtLeastOnce);
            return;
        }

        mock.Verify(x => x.ApresentarErro(It.IsAny<string>(), errorType.Value), Times.AtLeastOnce);
    }

    public static void NaoDeveTerApresentadoErro(this Mock<IBuscarResultadoDiagramaPresenter> mock)
    {
        mock.Verify(x => x.ApresentarErro(It.IsAny<string>(), It.IsAny<ErrorType>()), Times.Never);
    }
}