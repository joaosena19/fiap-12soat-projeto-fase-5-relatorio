using Domain.ResultadoDiagrama.Entities;
using Infrastructure.Relatorios.Pdf;
using QuestPDF.Fluent;
using QuestPDF.Infrastructure;

namespace Tests.Infrastructure.Relatorios.Pdf;

public class RelatorioAnalisePdfDocumentoTests
{
    [Fact(DisplayName = "Deve gerar PDF com conteúdo quando análise é válida")]
    [Trait("Infrastructure", "RelatorioAnalisePdfDocumento")]
    public void GeneratePdf_DeveGerarConteudo_QuandoAnaliseValida()
    {
        // Arrange
        QuestPDF.Settings.License = LicenseType.Community;
        var analiseResultado = AnaliseResultado.Criar(
            "Descricao da analise",
            ["API Gateway", "Fila SQS"],
            ["Acoplamento elevado"],
            ["Isolar módulos"]);
        var documento = new RelatorioAnalisePdfDocumento(analiseResultado);

        // Act
        var pdf = documento.GeneratePdf();

        // Assert
        pdf.ShouldNotBeNull();
        pdf.Length.ShouldBeGreaterThan(0);
    }

    [Fact(DisplayName = "Deve retornar metadata padrão do documento")]
    [Trait("Infrastructure", "RelatorioAnalisePdfDocumento")]
    public void GetMetadata_DeveRetornarMetadataPadrao_QuandoInvocado()
    {
        // Arrange
        var documento = new RelatorioAnalisePdfDocumento(AnaliseResultado.Criar("Descricao", ["API"], ["Risco"], ["Recomendacao"]));

        // Act
        var metadata = documento.GetMetadata();

        // Assert
        metadata.ShouldNotBeNull();
        metadata.Title.ShouldBe(DocumentMetadata.Default.Title);
    }
}