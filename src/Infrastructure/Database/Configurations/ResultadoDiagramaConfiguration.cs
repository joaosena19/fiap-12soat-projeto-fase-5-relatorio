using Domain.ResultadoDiagrama.Aggregates;
using Domain.ResultadoDiagrama.Entities;
using Domain.ResultadoDiagrama.Enums;
using Domain.ResultadoDiagrama.ValueObjects.RelatorioGerado;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ResultadoDiagramaDataCriacao = Domain.ResultadoDiagrama.ValueObjects.ResultadoDiagrama.DataCriacao;
using ResultadoDiagramaStatus = Domain.ResultadoDiagrama.ValueObjects.ResultadoDiagrama.Status;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Infrastructure.Database.Configurations;

public class ResultadoDiagramaConfiguration : IEntityTypeConfiguration<ResultadoDiagrama>
{
    private static readonly JsonSerializerOptions JsonOptions = new() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase, Converters = { new JsonStringEnumConverter() } };

    public void Configure(EntityTypeBuilder<ResultadoDiagrama> builder)
    {
        var relatoriosProperty = builder.Property(a => a.Relatorios)
            .HasColumnName("relatorios")
            .HasColumnType("jsonb")
            .HasConversion(
                value => SerializarRelatorios(value),
                value => DeserializarRelatorios(value));

        var errosProperty = builder.Property(a => a.Erros)
            .HasColumnName("erros")
            .HasColumnType("jsonb")
            .HasConversion(
                value => SerializarErros(value),
                value => DeserializarErros(value));

        var analiseResultadoProperty = builder.Property(a => a.AnaliseResultado)
            .HasColumnName("analise_resultado")
            .HasColumnType("jsonb")
            .HasConversion(
                value => SerializarAnaliseResultado(value),
                value => DeserializarAnaliseResultado(value));

        builder.ToTable("resultado_diagramas");
        builder.HasKey(a => a.Id);
        builder.Property(a => a.Id).HasColumnName("id");
        builder.Property(a => a.AnaliseDiagramaId).HasColumnName("analise_diagrama_id").IsRequired();
        builder.Property(a => a.DataCriacao)
               .HasColumnName("data_criacao")
               .IsRequired()
               .HasConversion(
                   value => value.Valor,
                   value => new ResultadoDiagramaDataCriacao(value));

        builder.Property(a => a.Status)
               .HasColumnName("status_analise")
               .IsRequired()
               .HasConversion(
                   value => value.Valor.ToString().ToLower(),
                   value => new ResultadoDiagramaStatus(Enum.Parse<StatusAnaliseEnum>(value, true)));

        relatoriosProperty.Metadata.SetValueComparer(CriarValueComparerRelatorios());
        errosProperty.Metadata.SetValueComparer(CriarValueComparerErros());
        analiseResultadoProperty.Metadata.SetValueComparer(CriarValueComparerAnaliseResultado());

        builder.HasIndex(a => a.AnaliseDiagramaId);
    }

    private static ValueComparer<List<RelatorioGerado>> CriarValueComparerRelatorios()
    {
        return new ValueComparer<List<RelatorioGerado>>(
            (left, right) => SerializarRelatorios(left) == SerializarRelatorios(right),
            value => SerializarRelatorios(value).GetHashCode(),
            value => DeserializarRelatorios(SerializarRelatorios(value)));
    }

    private static ValueComparer<List<ErroResultadoDiagrama>> CriarValueComparerErros()
    {
        return new ValueComparer<List<ErroResultadoDiagrama>>(
            (left, right) => SerializarErros(left) == SerializarErros(right),
            value => SerializarErros(value).GetHashCode(),
            value => DeserializarErros(SerializarErros(value)));
    }

    private static ValueComparer<AnaliseResultado?> CriarValueComparerAnaliseResultado()
    {
        return new ValueComparer<AnaliseResultado?>(
            (left, right) => SerializarAnaliseResultado(left) == SerializarAnaliseResultado(right),
            value => SerializarAnaliseResultado(value).GetHashCode(),
            value => DeserializarAnaliseResultado(SerializarAnaliseResultado(value)));
    }

    private static string SerializarRelatorios(List<RelatorioGerado>? relatorios)
    {
        return JsonSerializer.Serialize((relatorios ?? new List<RelatorioGerado>()).Select(RelatorioGeradoPersistencia.Criar).ToList(), JsonOptions);
    }

    private static List<RelatorioGerado> DeserializarRelatorios(string? valorSerializado)
    {
        if (string.IsNullOrEmpty(valorSerializado))
            return new List<RelatorioGerado>();

        var itensPersistidos = JsonSerializer.Deserialize<List<RelatorioGeradoPersistencia>>(valorSerializado, JsonOptions);
        if (itensPersistidos == null)
            return new List<RelatorioGerado>();

        return itensPersistidos.Select(item => item.ParaDominio()).ToList();
    }

    private static string SerializarErros(List<ErroResultadoDiagrama>? erros)
    {
        return JsonSerializer.Serialize((erros ?? new List<ErroResultadoDiagrama>()).Select(ErroResultadoDiagramaPersistencia.Criar).ToList(), JsonOptions);
    }

    private static List<ErroResultadoDiagrama> DeserializarErros(string? valorSerializado)
    {
        if (string.IsNullOrEmpty(valorSerializado))
            return new List<ErroResultadoDiagrama>();

        var itensPersistidos = JsonSerializer.Deserialize<List<ErroResultadoDiagramaPersistencia>>(valorSerializado, JsonOptions);
        if (itensPersistidos == null)
            return new List<ErroResultadoDiagrama>();

        return itensPersistidos.Select(item => item.ParaDominio()).ToList();
    }

    private static string SerializarAnaliseResultado(AnaliseResultado? analiseResultado)
    {
        if (analiseResultado == null)
            return "null";

        return JsonSerializer.Serialize(AnaliseResultadoPersistencia.Criar(analiseResultado), JsonOptions);
    }

    private static AnaliseResultado? DeserializarAnaliseResultado(string? valorSerializado)
    {
        if (string.IsNullOrEmpty(valorSerializado) || valorSerializado == "null")
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
            return ErroResultadoDiagrama.Criar(Mensagem, TipoRelatorio);
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
