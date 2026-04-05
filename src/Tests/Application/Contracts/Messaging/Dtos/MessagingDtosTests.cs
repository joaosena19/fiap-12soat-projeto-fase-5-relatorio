using Application.Contracts.Messaging.Dtos;
using Domain.ResultadoDiagrama.Enums;

namespace Tests.Application.Contracts.Messaging.Dtos;

public class MessagingDtosTests
{
    [Fact(DisplayName = "Deve mapear propriedades do DTO de processamento analisado")]
    [Trait("Application", "MessagingDtos")]
    public void ProcessamentoDiagramaAnalisadoDto_DeveMapearPropriedades_QuandoInicializado()
    {
        // Arrange
        var dataConclusao = DateTimeOffset.UtcNow;

        // Act
        var dto = new ProcessamentoDiagramaAnalisadoDto
        {
            CorrelationId = "corr-1",
            AnaliseDiagramaId = Guid.NewGuid(),
            DescricaoAnalise = "Descricao",
            ComponentesIdentificados = ["API"],
            RiscosArquiteturais = ["SPOF"],
            RecomendacoesBasicas = ["Retry"],
            DataConclusao = dataConclusao
        };

        // Assert
        dto.CorrelationId.ShouldBe("corr-1");
        dto.DescricaoAnalise.ShouldBe("Descricao");
        dto.ComponentesIdentificados.ShouldContain("API");
        dto.RiscosArquiteturais.ShouldContain("SPOF");
        dto.RecomendacoesBasicas.ShouldContain("Retry");
        dto.DataConclusao.ShouldBe(dataConclusao);
    }

    [Fact(DisplayName = "Deve mapear propriedades do DTO de processamento com erro")]
    [Trait("Application", "MessagingDtos")]
    public void ProcessamentoDiagramaErroDto_DeveMapearPropriedades_QuandoInicializado()
    {
        // Arrange
        var dataErro = DateTimeOffset.UtcNow;

        // Act
        var dto = new ProcessamentoDiagramaErroDto
        {
            CorrelationId = "corr-2",
            AnaliseDiagramaId = Guid.NewGuid(),
            Motivo = "Falha",
            TentativasRealizadas = 2,
            DataErro = dataErro
        };

        // Assert
        dto.CorrelationId.ShouldBe("corr-2");
        dto.Motivo.ShouldBe("Falha");
        dto.TentativasRealizadas.ShouldBe(2);
        dto.DataErro.ShouldBe(dataErro);
    }

    [Fact(DisplayName = "Deve mapear propriedades do DTO de processamento iniciado")]
    [Trait("Application", "MessagingDtos")]
    public void ProcessamentoDiagramaIniciadoDto_DeveMapearPropriedades_QuandoInicializado()
    {
        // Arrange
        var dataInicio = DateTimeOffset.UtcNow;

        // Act
        var dto = new ProcessamentoDiagramaIniciadoDto
        {
            CorrelationId = "corr-3",
            AnaliseDiagramaId = Guid.NewGuid(),
            NomeOriginal = "diagrama.png",
            Extensao = ".png",
            DataInicio = dataInicio
        };

        // Assert
        dto.CorrelationId.ShouldBe("corr-3");
        dto.NomeOriginal.ShouldBe("diagrama.png");
        dto.Extensao.ShouldBe(".png");
        dto.DataInicio.ShouldBe(dataInicio);
    }

    [Fact(DisplayName = "Deve mapear propriedades do DTO de upload concluído")]
    [Trait("Application", "MessagingDtos")]
    public void UploadDiagramaConcluidoDto_DeveMapearPropriedades_QuandoInicializado()
    {
        // Arrange
        var dataCriacao = DateTimeOffset.UtcNow;

        // Act
        var dto = new UploadDiagramaConcluidoDto
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

        // Assert
        dto.CorrelationId.ShouldBe("corr-4");
        dto.NomeOriginal.ShouldBe("diagrama.pdf");
        dto.Extensao.ShouldBe(".pdf");
        dto.Tamanho.ShouldBe(2048);
        dto.Hash.ShouldBe("hash");
        dto.NomeFisico.ShouldBe("arquivo.pdf");
        dto.LocalizacaoUrl.ShouldBe("s3://bucket/arquivo.pdf");
        dto.DataCriacao.ShouldBe(dataCriacao);
    }

    [Fact(DisplayName = "Deve mapear propriedades do DTO de upload rejeitado")]
    [Trait("Application", "MessagingDtos")]
    public void UploadDiagramaRejeitadoDto_DeveMapearPropriedades_QuandoInicializado()
    {
        // Arrange
        var dataRejeicao = DateTimeOffset.UtcNow;

        // Act
        var dto = new UploadDiagramaRejeitadoDto
        {
            CorrelationId = "corr-5",
            AnaliseDiagramaId = Guid.NewGuid(),
            MotivoRejeicao = "Assinatura invalida",
            DataRejeicao = dataRejeicao
        };

        // Assert
        dto.CorrelationId.ShouldBe("corr-5");
        dto.MotivoRejeicao.ShouldBe("Assinatura invalida");
        dto.DataRejeicao.ShouldBe(dataRejeicao);
    }

    [Fact(DisplayName = "Deve mapear propriedades do DTO de solicitação de geração")]
    [Trait("Application", "MessagingDtos")]
    public void SolicitarGeracaoRelatoriosDto_DeveMapearPropriedades_QuandoInicializado()
    {
        // Act
        var dto = new SolicitarGeracaoRelatoriosDto
        {
            AnaliseDiagramaId = Guid.NewGuid(),
            TiposRelatorio = [TipoRelatorioEnum.Json, TipoRelatorioEnum.Markdown]
        };

        // Assert
        dto.TiposRelatorio.Count.ShouldBe(2);
        dto.TiposRelatorio.ShouldContain(TipoRelatorioEnum.Json);
        dto.TiposRelatorio.ShouldContain(TipoRelatorioEnum.Markdown);
    }
}
