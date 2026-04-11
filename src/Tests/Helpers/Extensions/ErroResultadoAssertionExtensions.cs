using Domain.ResultadoDiagrama.Aggregates;

namespace Tests.Helpers.Extensions;

public static class ErroResultadoAssertionExtensions
{
    public static void DeveConterDados(this ErroResultadoDiagrama erro, string mensagem, TipoRelatorioEnum? tipo)
    {
        erro.Mensagem.Valor.ShouldBe(mensagem);
        erro.TipoRelatorio.Valor.ShouldBe(tipo);
        erro.DataOcorrencia.Valor.ShouldNotBe(default);
    }

    public static void DeveConterDados(this ErroResultadoDiagrama erro, string mensagem, TipoRelatorioEnum? tipo, DateTimeOffset dataOcorrencia)
    {
        erro.Mensagem.Valor.ShouldBe(mensagem);
        erro.TipoRelatorio.Valor.ShouldBe(tipo);
        erro.DataOcorrencia.Valor.ShouldBe(dataOcorrencia);
    }
}
