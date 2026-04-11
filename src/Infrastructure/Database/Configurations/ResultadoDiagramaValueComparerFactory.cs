using Domain.ResultadoDiagrama.Aggregates;
using Domain.ResultadoDiagrama.Entities;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Infrastructure.Database.Configurations;

internal static class ResultadoDiagramaValueComparerFactory
{
    public static ValueComparer<List<RelatorioGerado>> CriarValueComparerRelatorios()
    {
        return new ValueComparer<List<RelatorioGerado>>(
            (left, right) => ResultadoDiagramaPersistenciaMapper.SerializarRelatorios(left) == ResultadoDiagramaPersistenciaMapper.SerializarRelatorios(right),
            value => ResultadoDiagramaPersistenciaMapper.SerializarRelatorios(value).GetHashCode(),
            value => ResultadoDiagramaPersistenciaMapper.DeserializarRelatorios(ResultadoDiagramaPersistenciaMapper.SerializarRelatorios(value)));
    }

    public static ValueComparer<List<ErroResultadoDiagrama>> CriarValueComparerErros()
    {
        return new ValueComparer<List<ErroResultadoDiagrama>>(
            (left, right) => ResultadoDiagramaPersistenciaMapper.SerializarErros(left) == ResultadoDiagramaPersistenciaMapper.SerializarErros(right),
            value => ResultadoDiagramaPersistenciaMapper.SerializarErros(value).GetHashCode(),
            value => ResultadoDiagramaPersistenciaMapper.DeserializarErros(ResultadoDiagramaPersistenciaMapper.SerializarErros(value)));
    }

    public static ValueComparer<AnaliseResultado?> CriarValueComparerAnaliseResultado()
    {
        return new ValueComparer<AnaliseResultado?>(
            (left, right) => ResultadoDiagramaPersistenciaMapper.SerializarAnaliseResultado(left) == ResultadoDiagramaPersistenciaMapper.SerializarAnaliseResultado(right),
            value => ResultadoDiagramaPersistenciaMapper.SerializarAnaliseResultado(value).GetHashCode(),
            value => ResultadoDiagramaPersistenciaMapper.DeserializarAnaliseResultado(ResultadoDiagramaPersistenciaMapper.SerializarAnaliseResultado(value)));
    }
}