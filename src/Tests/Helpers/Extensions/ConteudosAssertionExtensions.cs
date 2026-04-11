using Domain.ResultadoDiagrama.ValueObjects.RelatorioGerado;

namespace Tests.Helpers.Extensions;

public static class ConteudosAssertionExtensions
{
    public static void DeveEstarVazio(this Conteudos conteudos)
    {
        conteudos.Valores.Count.ShouldBe(0);
    }

    public static void DeveConterConteudo(this Conteudos conteudos, string chave, string valor)
    {
        conteudos.ObterValor(chave).ShouldBe(valor);
    }

    public static void DeveConterChave(this Conteudos conteudos, string chave)
    {
        conteudos.ContemChave(chave).ShouldBeTrue();
    }

    public static void NaoDeveConterChave(this Conteudos conteudos, string chave)
    {
        conteudos.ContemChave(chave).ShouldBeFalse();
    }
}
