namespace Tests.Helpers.MockExtensions;

public static class RelatorioMessagePublisherMockExtensions
{
    public static void AoPublicarSolicitacaoGeracaoNaoFazNada(this Mock<IRelatorioMessagePublisher> mock)
    {
        mock.Setup(x => x.PublicarSolicitacaoGeracaoAsync(It.IsAny<Guid>(), It.IsAny<IReadOnlyCollection<TipoRelatorioEnum>>())).Returns(Task.CompletedTask);
    }

    public static void DeveTerPublicadoSolicitacaoGeracao(this Mock<IRelatorioMessagePublisher> mock)
    {
        mock.Verify(x => x.PublicarSolicitacaoGeracaoAsync(It.IsAny<Guid>(), It.IsAny<IReadOnlyCollection<TipoRelatorioEnum>>()), Times.AtLeastOnce);
    }

    public static void DeveTerPublicadoSolicitacaoGeracaoComTipos(this Mock<IRelatorioMessagePublisher> mock, params TipoRelatorioEnum[] tiposRelatorio)
    {
        mock.Verify(x => x.PublicarSolicitacaoGeracaoAsync(
            It.IsAny<Guid>(),
            It.Is<IReadOnlyCollection<TipoRelatorioEnum>>(itens => itens.Count == tiposRelatorio.Length && itens.All(tiposRelatorio.Contains))), Times.AtLeastOnce);
    }

    public static void NaoDeveTerPublicadoSolicitacaoGeracao(this Mock<IRelatorioMessagePublisher> mock)
    {
        mock.Verify(x => x.PublicarSolicitacaoGeracaoAsync(It.IsAny<Guid>(), It.IsAny<IReadOnlyCollection<TipoRelatorioEnum>>()), Times.Never);
    }
}