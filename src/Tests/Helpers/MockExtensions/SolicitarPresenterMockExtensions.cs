namespace Tests.Helpers.MockExtensions;

public static class SolicitarPresenterMockExtensions
{
    public static void DeveTerApresentadoSucesso(this Mock<ISolicitarGeracaoRelatoriosPresenter> mock)
    {
        mock.Verify(x => x.ApresentarSucesso(It.IsAny<ResultadoSolicitacaoRelatoriosDto>()), Times.AtLeastOnce);
    }

    public static void DeveTerApresentadoSucessoCom(this Mock<ISolicitarGeracaoRelatoriosPresenter> mock, Func<ResultadoSolicitacaoRelatoriosDto, bool> predicado)
    {
        mock.Verify(x => x.ApresentarSucesso(It.Is<ResultadoSolicitacaoRelatoriosDto>(resultadoSolicitacaoRelatoriosDto => predicado(resultadoSolicitacaoRelatoriosDto))), Times.AtLeastOnce);
    }

    public static void NaoDeveTerApresentadoSucesso(this Mock<ISolicitarGeracaoRelatoriosPresenter> mock)
    {
        mock.Verify(x => x.ApresentarSucesso(It.IsAny<ResultadoSolicitacaoRelatoriosDto>()), Times.Never);
    }

    public static void DeveTerApresentadoErro(this Mock<ISolicitarGeracaoRelatoriosPresenter> mock, ErrorType? errorType = null)
    {
        if (errorType == null)
        {
            mock.Verify(x => x.ApresentarErro(It.IsAny<string>(), It.IsAny<ErrorType>()), Times.AtLeastOnce);
            return;
        }

        mock.Verify(x => x.ApresentarErro(It.IsAny<string>(), errorType.Value), Times.AtLeastOnce);
    }

    public static void NaoDeveTerApresentadoErro(this Mock<ISolicitarGeracaoRelatoriosPresenter> mock)
    {
        mock.Verify(x => x.ApresentarErro(It.IsAny<string>(), It.IsAny<ErrorType>()), Times.Never);
    }
}