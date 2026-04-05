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

    public static void DeveConterErroComMensagem(this ResultadoDiagrama resultado, string mensagemEsperada)
    {
        resultado.DeveEstarComErro();
        resultado.Erros[^1].Mensagem.Valor.ShouldBe(mensagemEsperada);
    }
}
