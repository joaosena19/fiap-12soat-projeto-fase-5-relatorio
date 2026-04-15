using Domain.ResultadoDiagrama.Aggregates;
using Domain.ResultadoDiagrama.Entities;
using Domain.ResultadoDiagrama.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ResultadoDiagramaDataCriacao = Domain.ResultadoDiagrama.ValueObjects.ResultadoDiagrama.DataCriacao;
using ResultadoDiagramaDataUltimaTentativa = Domain.ResultadoDiagrama.ValueObjects.ResultadoDiagrama.DataUltimaTentativa;
using ResultadoDiagramaStatus = Domain.ResultadoDiagrama.ValueObjects.ResultadoDiagrama.Status;

namespace Infrastructure.Database.Configurations;

public class ResultadoDiagramaConfiguration : IEntityTypeConfiguration<ResultadoDiagrama>
{
    public void Configure(EntityTypeBuilder<ResultadoDiagrama> builder)
    {
        var relatoriosProperty = builder.Property(a => a.Relatorios)
            .HasColumnName("relatorios")
            .HasColumnType("jsonb")
            .HasConversion(
                value => ResultadoDiagramaPersistenciaMapper.SerializarRelatorios(value),
                value => ResultadoDiagramaPersistenciaMapper.DeserializarRelatorios(value));

        var errosProperty = builder.Property(a => a.Erros)
            .HasColumnName("erros")
            .HasColumnType("jsonb")
            .HasConversion(
                value => ResultadoDiagramaPersistenciaMapper.SerializarErros(value),
                value => ResultadoDiagramaPersistenciaMapper.DeserializarErros(value));

        var analiseResultadoProperty = builder.Property(a => a.AnaliseResultado)
            .HasColumnName("analise_resultado")
            .HasColumnType("jsonb")
            .HasConversion(
                value => ResultadoDiagramaPersistenciaMapper.SerializarAnaliseResultado(value),
                value => ResultadoDiagramaPersistenciaMapper.DeserializarAnaliseResultado(value));

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

        builder.Property(a => a.DataUltimaTentativa)
               .HasColumnName("data_ultima_tentativa")
               .IsRequired()
               .HasConversion(
                   value => value.Valor,
                   value => new ResultadoDiagramaDataUltimaTentativa(value));

        builder.Property(a => a.Status)
               .HasColumnName("status_analise")
               .IsRequired()
               .HasConversion(
                   value => value.Valor.ToString().ToLower(),
                   value => new ResultadoDiagramaStatus(Enum.Parse<StatusAnaliseEnum>(value, true)));

        relatoriosProperty.Metadata.SetValueComparer(ResultadoDiagramaValueComparerFactory.CriarValueComparerRelatorios());
        errosProperty.Metadata.SetValueComparer(ResultadoDiagramaValueComparerFactory.CriarValueComparerErros());
        analiseResultadoProperty.Metadata.SetValueComparer(ResultadoDiagramaValueComparerFactory.CriarValueComparerAnaliseResultado());

        builder.HasIndex(a => a.AnaliseDiagramaId).IsUnique();
    }
}
