using Domain.ResultadoDiagrama.Aggregates;

namespace Tests.Helpers.Extensions;

public static class ResultadoDiagramaAssertionExtensions
{
    public static void DeveEstarComStatus(this ResultadoDiagrama resultado, StatusAnaliseEnum statusEsperado)
    {
        resultado.Status.Valor.ShouldBe(statusEsperado);
    }

    public static void DeveEstarAnalisado(this ResultadoDiagrama resultado)
    {
        resultado.Status.Valor.ShouldBe(StatusAnaliseEnum.Analisado);
        resultado.AnaliseResultado.ShouldNotBeNull();
    }

    public static void DeveEstarAnalisadoComDescricao(this ResultadoDiagrama resultado, string descricaoEsperada)
    {
        resultado.DeveEstarAnalisado();
        resultado.AnaliseResultado!.DescricaoAnalise.Valor.ShouldBe(descricaoEsperada);
    }

    public static void DeveEstarComErro(this ResultadoDiagrama resultado)
    {
        resultado.Status.Valor.ShouldBe(StatusAnaliseEnum.Erro);
        resultado.Erros.ShouldNotBeEmpty();
    }

    public static void DeveEstarRejeitado(this ResultadoDiagrama resultado)
    {
        resultado.Status.Valor.ShouldBe(StatusAnaliseEnum.Rejeitado);
        resultado.Erros.ShouldNotBeEmpty();
    }

    public static void DeveConterErroComMensagem(this ResultadoDiagrama resultado, string mensagemEsperada)
    {
        resultado.DeveEstarComErro();
        resultado.Erros[^1].Mensagem.Valor.ShouldBe(mensagemEsperada);
    }

    public static void DeveEstarRecentementeCriado(this ResultadoDiagrama resultado, Guid analiseDiagramaId)
    {
        resultado.Id.ShouldNotBe(Guid.Empty);
        resultado.AnaliseDiagramaId.ShouldBe(analiseDiagramaId);
        resultado.DeveEstarComStatus(StatusAnaliseEnum.Recebido);
        resultado.AnaliseResultado.ShouldBeNull();
        resultado.Relatorios.Count.ShouldBe(Enum.GetValues<TipoRelatorioEnum>().Length);
        resultado.Relatorios.Where(r => r.Tipo.Valor == TipoRelatorioEnum.Json).All(r => r.Status.Valor == StatusRelatorioEnum.Automatico).ShouldBeTrue();
        resultado.Relatorios.Where(r => r.Tipo.Valor != TipoRelatorioEnum.Json).All(r => r.Status.Valor == StatusRelatorioEnum.NaoSolicitado).ShouldBeTrue();
        resultado.Erros.ShouldBeEmpty();
        resultado.DataCriacao.Valor.ShouldNotBe(default);
        resultado.DataUltimaTentativa.Valor.ShouldBe(resultado.DataCriacao.Valor);
    }

    public static void DeveEstarComErroSemRelatorio(this ResultadoDiagrama resultado)
    {
        resultado.DeveEstarComStatus(StatusAnaliseEnum.Erro);
        resultado.Erros.Count.ShouldBe(1);
        resultado.Erros[0].TipoRelatorio.Valor.ShouldBeNull();
    }

    public static void DeveEstarComErroEUltimoMotivo(this ResultadoDiagrama resultado, string mensagemEsperada)
    {
        resultado.Status.Valor.ShouldBe(StatusAnaliseEnum.Erro);
        resultado.Erros.ShouldNotBeEmpty();
        resultado.Erros[^1].Mensagem.Valor.ShouldBe(mensagemEsperada);
    }

    public static void DeveTerErroDoTipoRelatorio(this ResultadoDiagrama resultado, TipoRelatorioEnum tipoRelatorio)
    {
        resultado.Erros.ShouldNotBeEmpty();
        resultado.Erros.Any(item => item.TipoRelatorio.Valor == tipoRelatorio).ShouldBeTrue();
    }

    public static void DeveTerErroComOrigem(this ResultadoDiagrama resultado, OrigemErroEnum origemEsperada)
    {
        resultado.Erros.ShouldNotBeEmpty();
        resultado.Erros.Any(e => e.OrigemErro.Valor == origemEsperada).ShouldBeTrue();
    }

    public static void DeveTerErroComTentativa(this ResultadoDiagrama resultado, int tentativaEsperada)
    {
        resultado.Erros.ShouldNotBeEmpty();
        resultado.Erros.Any(e => e.NumeroTentativa.Valor == tentativaEsperada).ShouldBeTrue();
    }

    public static void DeveEstarPreparadoParaReprocessamento(this ResultadoDiagrama resultado)
    {
        resultado.Status.Valor.ShouldBe(StatusAnaliseEnum.EmProcessamento);
        resultado.AnaliseResultado.ShouldBeNull();
        resultado.Relatorios.Count.ShouldBe(Enum.GetValues<TipoRelatorioEnum>().Length);
        resultado.Relatorios.First(r => r.Tipo.Valor == TipoRelatorioEnum.Json).Status.Valor.ShouldBe(StatusRelatorioEnum.Automatico);
        resultado.Relatorios.Where(r => r.Tipo.Valor != TipoRelatorioEnum.Json).All(r => r.Status.Valor == StatusRelatorioEnum.NaoSolicitado).ShouldBeTrue();
        resultado.Erros.ShouldNotBeEmpty();
        resultado.DataUltimaTentativa.Valor.ShouldBeGreaterThan(resultado.DataCriacao.Valor);
    }
}
