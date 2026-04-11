namespace Tests.Helpers.MockExtensions;

public static class MetricsServiceMockExtensions
{
    public static void DeveTerRegistradoRelatorioGerado(this Mock<IMetricsService> mock)
    {
        mock.Verify(x => x.RegistrarRelatorioGerado(It.IsAny<Guid>(), It.IsAny<TipoRelatorioEnum>()), Times.AtLeastOnce);
    }

    public static void NaoDeveTerRegistradoRelatorioGerado(this Mock<IMetricsService> mock)
    {
        mock.Verify(x => x.RegistrarRelatorioGerado(It.IsAny<Guid>(), It.IsAny<TipoRelatorioEnum>()), Times.Never);
    }

    public static void DeveTerRegistradoRelatorioComFalha(this Mock<IMetricsService> mock)
    {
        mock.Verify(x => x.RegistrarRelatorioComFalha(It.IsAny<Guid>(), It.IsAny<TipoRelatorioEnum>(), It.IsAny<string>()), Times.AtLeastOnce);
    }

    public static void NaoDeveTerRegistradoRelatorioComFalha(this Mock<IMetricsService> mock)
    {
        mock.Verify(x => x.RegistrarRelatorioComFalha(It.IsAny<Guid>(), It.IsAny<TipoRelatorioEnum>(), It.IsAny<string>()), Times.Never);
    }
}