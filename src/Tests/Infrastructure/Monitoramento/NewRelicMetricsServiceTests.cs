using Infrastructure.Monitoramento;
using Infrastructure.Monitoramento.Correlation;

namespace Tests.Infrastructure.Monitoramento;

public class NewRelicMetricsServiceTests
{
    [Fact(DisplayName = "Não deve lançar erro ao registrar análise recebida")]
    [Trait("Infrastructure", "NewRelicMetricsService")]
    public void RegistrarAnaliseRecebida_NaoDeveLancarErro_QuandoChamado()
    {
        // Arrange
        var service = new NewRelicMetricsService();

        // Act
        var acao = () => service.RegistrarAnaliseRecebida(Guid.NewGuid(), ".png");

        // Assert
        acao.ShouldNotThrow();
    }

    [Fact(DisplayName = "Não deve lançar erro ao registrar análise concluída")]
    [Trait("Infrastructure", "NewRelicMetricsService")]
    public void RegistrarAnaliseConcluida_NaoDeveLancarErro_QuandoChamado()
    {
        // Arrange
        var service = new NewRelicMetricsService();

        // Act
        var acao = () => service.RegistrarAnaliseConcluida(Guid.NewGuid());

        // Assert
        acao.ShouldNotThrow();
    }

    [Fact(DisplayName = "Não deve lançar erro ao registrar análise com falha")]
    [Trait("Infrastructure", "NewRelicMetricsService")]
    public void RegistrarAnaliseComFalha_NaoDeveLancarErro_QuandoChamado()
    {
        // Arrange
        var service = new NewRelicMetricsService();

        // Act
        var acao = () => service.RegistrarAnaliseComFalha(Guid.NewGuid(), "erro de teste");

        // Assert
        acao.ShouldNotThrow();
    }

    [Fact(DisplayName = "Não deve lançar erro ao registrar relatório gerado com correlation id")]
    [Trait("Infrastructure", "NewRelicMetricsService")]
    public void RegistrarRelatorioGerado_NaoDeveLancarErro_QuandoCorrelationIdPresente()
    {
        // Arrange
        var service = new NewRelicMetricsService();

        // Act
        using (CorrelationContext.Push(Guid.NewGuid().ToString()))
        {
            var acao = () => service.RegistrarRelatorioGerado(Guid.NewGuid(), TipoRelatorioEnum.Pdf);

            // Assert
            acao.ShouldNotThrow();
        }
    }

    [Fact(DisplayName = "Não deve lançar erro ao registrar relatório com falha")]
    [Trait("Infrastructure", "NewRelicMetricsService")]
    public void RegistrarRelatorioComFalha_NaoDeveLancarErro_QuandoChamado()
    {
        // Arrange
        var service = new NewRelicMetricsService();

        // Act
        var acao = () => service.RegistrarRelatorioComFalha(Guid.NewGuid(), TipoRelatorioEnum.Markdown, "falha de teste");

        // Assert
        acao.ShouldNotThrow();
    }
}
