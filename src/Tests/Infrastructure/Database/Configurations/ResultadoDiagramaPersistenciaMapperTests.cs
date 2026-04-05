using Domain.ResultadoDiagrama.Entities;
using Domain.ResultadoDiagrama.ValueObjects.RelatorioGerado;
using Infrastructure.Database.Configurations;

namespace Tests.Infrastructure.Database.Configurations;

public class ResultadoDiagramaPersistenciaMapperTests
{
    [Fact(DisplayName = "Deve serializar e deserializar relatórios preservando conteúdo")]
    [Trait("Infrastructure", "ResultadoDiagramaPersistenciaMapper")]
    public void Relatorios_DevePreservarConteudo_QuandoSerializarEDeserializar()
    {
        // Arrange
        var dataGeracao = DateTimeOffset.UtcNow;
        var relatorios = new List<global::Domain.ResultadoDiagrama.Aggregates.RelatorioGerado>
        {
            global::Domain.ResultadoDiagrama.Aggregates.RelatorioGerado.Reidratar(
                new Tipo(TipoRelatorioEnum.Json),
                new Status(StatusRelatorioEnum.Concluido),
                Conteudos.Criar(new Dictionary<string, string> { ["arquivo"] = "relatorio.json" }),
                new DataGeracao(dataGeracao))
        };

        // Act
        var serializado = ResultadoDiagramaPersistenciaMapper.SerializarRelatorios(relatorios);
        var desserializado = ResultadoDiagramaPersistenciaMapper.DeserializarRelatorios(serializado);

        // Assert
        desserializado.ShouldHaveSingleItem();
        desserializado[0].Tipo.Valor.ShouldBe(TipoRelatorioEnum.Json);
        desserializado[0].Status.Valor.ShouldBe(StatusRelatorioEnum.Concluido);
        desserializado[0].Conteudos.Valores["arquivo"].ShouldBe("relatorio.json");
        desserializado[0].DataGeracao!.Valor.ShouldBe(dataGeracao);
    }

    [Fact(DisplayName = "Deve retornar coleção vazia quando relatórios serializados são nulos ou vazios")]
    [Trait("Infrastructure", "ResultadoDiagramaPersistenciaMapper")]
    public void DeserializarRelatorios_DeveRetornarVazio_QuandoValorInvalido()
    {
        // Act
        var retornoNulo = ResultadoDiagramaPersistenciaMapper.DeserializarRelatorios(null);
        var retornoVazio = ResultadoDiagramaPersistenciaMapper.DeserializarRelatorios(string.Empty);

        // Assert
        retornoNulo.ShouldBeEmpty();
        retornoVazio.ShouldBeEmpty();
    }

    [Fact(DisplayName = "Deve serializar e deserializar erros preservando propriedades")]
    [Trait("Infrastructure", "ResultadoDiagramaPersistenciaMapper")]
    public void Erros_DevePreservarConteudo_QuandoSerializarEDeserializar()
    {
        // Arrange
        var dataOcorrencia = DateTimeOffset.UtcNow;
        var erros = new List<global::Domain.ResultadoDiagrama.Aggregates.ErroResultadoDiagrama>
        {
            global::Domain.ResultadoDiagrama.Aggregates.ErroResultadoDiagrama.Reidratar("Falha ao gerar relatório", TipoRelatorioEnum.Pdf, dataOcorrencia)
        };

        // Act
        var serializado = ResultadoDiagramaPersistenciaMapper.SerializarErros(erros);
        var desserializado = ResultadoDiagramaPersistenciaMapper.DeserializarErros(serializado);

        // Assert
        desserializado.ShouldHaveSingleItem();
        desserializado[0].Mensagem.Valor.ShouldBe("Falha ao gerar relatório");
        desserializado[0].TipoRelatorio.Valor.ShouldBe(TipoRelatorioEnum.Pdf);
        desserializado[0].DataOcorrencia.Valor.ShouldBe(dataOcorrencia);
    }

    [Fact(DisplayName = "Deve serializar e deserializar análise preservando listas e descrição")]
    [Trait("Infrastructure", "ResultadoDiagramaPersistenciaMapper")]
    public void AnaliseResultado_DevePreservarConteudo_QuandoSerializarEDeserializar()
    {
        // Arrange
        var analiseResultado = AnaliseResultado.Criar(
            "Analise completa",
            ["API", "Banco"],
            ["Acoplamento"],
            ["Separar contextos"]);

        // Act
        var serializado = ResultadoDiagramaPersistenciaMapper.SerializarAnaliseResultado(analiseResultado);
        var desserializado = ResultadoDiagramaPersistenciaMapper.DeserializarAnaliseResultado(serializado);

        // Assert
        desserializado.ShouldNotBeNull();
        desserializado.DescricaoAnalise.Valor.ShouldBe("Analise completa");
        desserializado.ComponentesIdentificados.Select(item => item.Valor).ShouldBe(["API", "Banco"]);
        desserializado.RiscosArquiteturais.Select(item => item.Valor).ShouldBe(["Acoplamento"]);
        desserializado.RecomendacoesBasicas.Select(item => item.Valor).ShouldBe(["Separar contextos"]);
    }

    [Fact(DisplayName = "Deve retornar nulo quando análise serializada é nula, vazia ou literal null")]
    [Trait("Infrastructure", "ResultadoDiagramaPersistenciaMapper")]
    public void DeserializarAnaliseResultado_DeveRetornarNulo_QuandoValorInvalido()
    {
        // Act
        var retornoNulo = ResultadoDiagramaPersistenciaMapper.DeserializarAnaliseResultado(null);
        var retornoVazio = ResultadoDiagramaPersistenciaMapper.DeserializarAnaliseResultado(string.Empty);
        var retornoLiteralNull = ResultadoDiagramaPersistenciaMapper.DeserializarAnaliseResultado("null");

        // Assert
        retornoNulo.ShouldBeNull();
        retornoVazio.ShouldBeNull();
        retornoLiteralNull.ShouldBeNull();
    }
}