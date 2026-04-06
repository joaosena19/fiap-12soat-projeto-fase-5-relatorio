using Application.Contracts.Messaging.Dtos;
using Domain.ResultadoDiagrama.Enums;
using System.Text.Json;

namespace Tests.Application.Contracts.Messaging.Dtos;

public class MessagingDtosTests
{
    [Fact(DisplayName = "Deve manter dados no roundtrip JSON do DTO de processamento analisado")]
    [Trait("Application", "MessagingDtos")]
    public void ProcessamentoDiagramaAnalisadoDto_DeveManterDados_QuandoRoundtripJson()
    {
        // Arrange
        var dataConclusao = DateTimeOffset.UtcNow;
        var original = new ProcessamentoDiagramaAnalisadoDto
        {
            CorrelationId = "corr-1",
            AnaliseDiagramaId = Guid.NewGuid(),
            DescricaoAnalise = "Descricao",
            ComponentesIdentificados = ["API"],
            RiscosArquiteturais = ["SPOF"],
            RecomendacoesBasicas = ["Retry"],
            DataConclusao = dataConclusao
        };

        // Act
        var json = JsonSerializer.Serialize(original);
        var dto = JsonSerializer.Deserialize<ProcessamentoDiagramaAnalisadoDto>(json);

        // Assert
        dto.ShouldNotBeNull();
        dto.CorrelationId.ShouldBe("corr-1");
        dto.DescricaoAnalise.ShouldBe("Descricao");
        dto.ComponentesIdentificados.ShouldContain("API");
        dto.RiscosArquiteturais.ShouldContain("SPOF");
        dto.RecomendacoesBasicas.ShouldContain("Retry");
        dto.DataConclusao.ShouldBe(dataConclusao);
    }

    [Fact(DisplayName = "Deve manter dados no roundtrip JSON do DTO de processamento com erro")]
    [Trait("Application", "MessagingDtos")]
    public void ProcessamentoDiagramaErroDto_DeveManterDados_QuandoRoundtripJson()
    {
        // Arrange
        var dataErro = DateTimeOffset.UtcNow;
        var original = new ProcessamentoDiagramaErroDto
        {
            CorrelationId = "corr-2",
            AnaliseDiagramaId = Guid.NewGuid(),
            Motivo = "Falha",
            TentativasRealizadas = 2,
            DataErro = dataErro
        };

        // Act
        var json = JsonSerializer.Serialize(original);
        var dto = JsonSerializer.Deserialize<ProcessamentoDiagramaErroDto>(json);

        // Assert
        dto.ShouldNotBeNull();
        dto.CorrelationId.ShouldBe("corr-2");
        dto.Motivo.ShouldBe("Falha");
        dto.TentativasRealizadas.ShouldBe(2);
        dto.DataErro.ShouldBe(dataErro);
    }

    [Fact(DisplayName = "Deve manter dados no roundtrip JSON do DTO de processamento iniciado")]
    [Trait("Application", "MessagingDtos")]
    public void ProcessamentoDiagramaIniciadoDto_DeveManterDados_QuandoRoundtripJson()
    {
        // Arrange
        var dataInicio = DateTimeOffset.UtcNow;
        var original = new ProcessamentoDiagramaIniciadoDto
        {
            CorrelationId = "corr-3",
            AnaliseDiagramaId = Guid.NewGuid(),
            NomeOriginal = "diagrama.png",
            Extensao = ".png",
            DataInicio = dataInicio
        };

        // Act
        var json = JsonSerializer.Serialize(original);
        var dto = JsonSerializer.Deserialize<ProcessamentoDiagramaIniciadoDto>(json);

        // Assert
        dto.ShouldNotBeNull();
        dto.CorrelationId.ShouldBe("corr-3");
        dto.NomeOriginal.ShouldBe("diagrama.png");
        dto.Extensao.ShouldBe(".png");
        dto.DataInicio.ShouldBe(dataInicio);
    }

    [Fact(DisplayName = "Deve manter dados no roundtrip JSON do DTO de upload concluído")]
    [Trait("Application", "MessagingDtos")]
    public void UploadDiagramaConcluidoDto_DeveManterDados_QuandoRoundtripJson()
    {
        // Arrange
        var dataCriacao = DateTimeOffset.UtcNow;
        var original = new UploadDiagramaConcluidoDto
        {
            CorrelationId = "corr-4",
            AnaliseDiagramaId = Guid.NewGuid(),
            NomeOriginal = "diagrama.pdf",
            Extensao = ".pdf",
            Tamanho = 2048,
            Hash = "hash",
            NomeFisico = "arquivo.pdf",
            LocalizacaoUrl = "s3://bucket/arquivo.pdf",
            DataCriacao = dataCriacao
        };

        // Act
        var json = JsonSerializer.Serialize(original);
        var dto = JsonSerializer.Deserialize<UploadDiagramaConcluidoDto>(json);

        // Assert
        dto.ShouldNotBeNull();
        dto.CorrelationId.ShouldBe("corr-4");
        dto.NomeOriginal.ShouldBe("diagrama.pdf");
        dto.Extensao.ShouldBe(".pdf");
        dto.Tamanho.ShouldBe(2048);
        dto.Hash.ShouldBe("hash");
        dto.NomeFisico.ShouldBe("arquivo.pdf");
        dto.LocalizacaoUrl.ShouldBe("s3://bucket/arquivo.pdf");
        dto.DataCriacao.ShouldBe(dataCriacao);
    }

    [Fact(DisplayName = "Deve manter dados no roundtrip JSON do DTO de upload rejeitado")]
    [Trait("Application", "MessagingDtos")]
    public void UploadDiagramaRejeitadoDto_DeveManterDados_QuandoRoundtripJson()
    {
        // Arrange
        var dataRejeicao = DateTimeOffset.UtcNow;
        var original = new UploadDiagramaRejeitadoDto
        {
            CorrelationId = "corr-5",
            AnaliseDiagramaId = Guid.NewGuid(),
            MotivoRejeicao = "Assinatura invalida",
            DataRejeicao = dataRejeicao
        };

        // Act
        var json = JsonSerializer.Serialize(original);
        var dto = JsonSerializer.Deserialize<UploadDiagramaRejeitadoDto>(json);

        // Assert
        dto.ShouldNotBeNull();
        dto.CorrelationId.ShouldBe("corr-5");
        dto.MotivoRejeicao.ShouldBe("Assinatura invalida");
        dto.DataRejeicao.ShouldBe(dataRejeicao);
    }

    [Fact(DisplayName = "Deve manter dados no roundtrip JSON do DTO de solicitação de geração")]
    [Trait("Application", "MessagingDtos")]
    public void SolicitarGeracaoRelatoriosDto_DeveManterDados_QuandoRoundtripJson()
    {
        // Arrange
        var original = new SolicitarGeracaoRelatoriosDto
        {
            AnaliseDiagramaId = Guid.NewGuid(),
            TiposRelatorio = [TipoRelatorioEnum.Json, TipoRelatorioEnum.Markdown]
        };

        // Act
        var json = JsonSerializer.Serialize(original);
        var dto = JsonSerializer.Deserialize<SolicitarGeracaoRelatoriosDto>(json);

        // Assert
        dto.ShouldNotBeNull();
        dto.TiposRelatorio.Count.ShouldBe(2);
        dto.TiposRelatorio.ShouldContain(TipoRelatorioEnum.Json);
        dto.TiposRelatorio.ShouldContain(TipoRelatorioEnum.Markdown);
    }
}
