using Domain.ResultadoDiagrama.Aggregates;
using Domain.ResultadoDiagrama.Entities;
using Domain.ResultadoDiagrama.Enums;
using Domain.ResultadoDiagrama.ValueObjects.RelatorioGerado;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Infrastructure.Database.Configurations;

internal static class ResultadoDiagramaPersistenciaMapper
{
    private const string ValorNuloSerializado = "null";

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        Converters = { new JsonStringEnumConverter() }
    };

    public static string SerializarRelatorios(List<RelatorioGerado>? relatorios)
    {
        return JsonSerializer.Serialize((relatorios ?? []).Select(RelatorioGeradoPersistencia.Criar).ToList(), JsonOptions);
    }

    public static List<RelatorioGerado> DeserializarRelatorios(string? valorSerializado)
    {
        if (string.IsNullOrEmpty(valorSerializado))
            return [];

        var itensPersistidos = JsonSerializer.Deserialize<List<RelatorioGeradoPersistencia>>(valorSerializado, JsonOptions);
        if (itensPersistidos == null)
            return [];

        return itensPersistidos.Select(item => item.ParaDominio()).ToList();
    }

    public static string SerializarErros(List<ErroResultadoDiagrama>? erros)
    {
        return JsonSerializer.Serialize((erros ?? []).Select(ErroResultadoDiagramaPersistencia.Criar).ToList(), JsonOptions);
    }

    public static List<ErroResultadoDiagrama> DeserializarErros(string? valorSerializado)
    {
        if (string.IsNullOrEmpty(valorSerializado))
            return [];

        var itensPersistidos = JsonSerializer.Deserialize<List<ErroResultadoDiagramaPersistencia>>(valorSerializado, JsonOptions);
        if (itensPersistidos == null)
            return [];

        return itensPersistidos.Select(item => item.ParaDominio()).ToList();
    }

    public static string SerializarAnaliseResultado(AnaliseResultado? analiseResultado)
    {
        if (analiseResultado == null)
            return ValorNuloSerializado;

        return JsonSerializer.Serialize(AnaliseResultadoPersistencia.Criar(analiseResultado), JsonOptions);
    }

    public static AnaliseResultado? DeserializarAnaliseResultado(string? valorSerializado)
    {
        if (string.IsNullOrEmpty(valorSerializado) || valorSerializado == ValorNuloSerializado)
            return null;

        var persistido = JsonSerializer.Deserialize<AnaliseResultadoPersistencia>(valorSerializado, JsonOptions);
        return persistido?.ParaDominio();
    }

    private sealed class RelatorioGeradoPersistencia
    {
        public TipoRelatorioEnum Tipo { get; set; }
        public StatusRelatorioEnum Status { get; set; }
        public Dictionary<string, string> Conteudos { get; set; } = new();
        public DateTimeOffset? DataGeracao { get; set; }

        public static RelatorioGeradoPersistencia Criar(RelatorioGerado relatorio)
        {
            return new RelatorioGeradoPersistencia
            {
                Tipo = relatorio.Tipo.Valor,
                Status = relatorio.Status.Valor,
                Conteudos = relatorio.Conteudos.Valores.ToDictionary(item => item.Key, item => item.Value),
                DataGeracao = relatorio.DataGeracao?.Valor
            };
        }

        public RelatorioGerado ParaDominio()
        {
            var conteudos = Conteudos.Count > 0 ? Domain.ResultadoDiagrama.ValueObjects.RelatorioGerado.Conteudos.Criar(Conteudos) : Domain.ResultadoDiagrama.ValueObjects.RelatorioGerado.Conteudos.Vazio();

            return RelatorioGerado.Reidratar(
                new Tipo(Tipo),
                new Domain.ResultadoDiagrama.ValueObjects.RelatorioGerado.Status(Status),
                conteudos,
                DataGeracao.HasValue ? new DataGeracao(DataGeracao.Value) : null);
        }
    }

    private sealed class ErroResultadoDiagramaPersistencia
    {
        public string Mensagem { get; set; } = string.Empty;
        public TipoRelatorioEnum? TipoRelatorio { get; set; }
        public DateTimeOffset DataOcorrencia { get; set; }

        public static ErroResultadoDiagramaPersistencia Criar(ErroResultadoDiagrama erro)
        {
            return new ErroResultadoDiagramaPersistencia
            {
                Mensagem = erro.Mensagem.Valor,
                TipoRelatorio = erro.TipoRelatorio.Valor,
                DataOcorrencia = erro.DataOcorrencia.Valor
            };
        }

        public ErroResultadoDiagrama ParaDominio()
        {
            return ErroResultadoDiagrama.Reidratar(Mensagem, TipoRelatorio, DataOcorrencia);
        }
    }

    private sealed class AnaliseResultadoPersistencia
    {
        public string DescricaoAnalise { get; set; } = string.Empty;
        public List<string> ComponentesIdentificados { get; set; } = new();
        public List<string> RiscosArquiteturais { get; set; } = new();
        public List<string> RecomendacoesBasicas { get; set; } = new();

        public static AnaliseResultadoPersistencia Criar(AnaliseResultado analiseResultado)
        {
            return new AnaliseResultadoPersistencia
            {
                DescricaoAnalise = analiseResultado.DescricaoAnalise.Valor,
                ComponentesIdentificados = analiseResultado.ComponentesIdentificados.Select(item => item.Valor).ToList(),
                RiscosArquiteturais = analiseResultado.RiscosArquiteturais.Select(item => item.Valor).ToList(),
                RecomendacoesBasicas = analiseResultado.RecomendacoesBasicas.Select(item => item.Valor).ToList()
            };
        }

        public AnaliseResultado ParaDominio()
        {
            return AnaliseResultado.Criar(DescricaoAnalise, ComponentesIdentificados, RiscosArquiteturais, RecomendacoesBasicas);
        }
    }
}