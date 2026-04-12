using Domain.ResultadoDiagrama.Aggregates;

namespace Tests.Helpers.Extensions;

public static class RelatorioGeradoAssertionExtensions
{
    public static void DeveEstarInicializado(this RelatorioGerado relatorio, TipoRelatorioEnum tipo)
    {
        relatorio.Tipo.Valor.ShouldBe(tipo);
        relatorio.Status.Valor.ShouldBe(StatusRelatorioEnum.NaoSolicitado);
        relatorio.Conteudos.Valores.Count.ShouldBe(0);
        relatorio.DataGeracao.ShouldBeNull();
    }

    public static void DeveEstarInicializadoComStatus(this RelatorioGerado relatorio, TipoRelatorioEnum tipo, StatusRelatorioEnum status)
    {
        relatorio.Tipo.Valor.ShouldBe(tipo);
        relatorio.Status.Valor.ShouldBe(status);
        relatorio.Conteudos.Valores.Count.ShouldBe(0);
        relatorio.DataGeracao.ShouldBeNull();
    }

    public static void DeveEstarConcluido(this RelatorioGerado relatorio, string chaveConteudo, string valorConteudo)
    {
        relatorio.Status.Valor.ShouldBe(StatusRelatorioEnum.Concluido);
        relatorio.Conteudos.ObterValor(chaveConteudo).ShouldBe(valorConteudo);
        relatorio.DataGeracao.ShouldNotBeNull();
    }
}
