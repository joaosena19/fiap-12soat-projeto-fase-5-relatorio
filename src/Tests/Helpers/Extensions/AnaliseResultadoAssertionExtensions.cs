using Domain.ResultadoDiagrama.Entities;

namespace Tests.Helpers.Extensions;

public static class AnaliseResultadoAssertionExtensions
{
    public static void DeveConterContagens(this AnaliseResultado analise, string descricao, int componentes, int riscos, int recomendacoes)
    {
        analise.DescricaoAnalise.Valor.ShouldBe(descricao);
        analise.ComponentesIdentificados.Count.ShouldBe(componentes);
        analise.RiscosArquiteturais.Count.ShouldBe(riscos);
        analise.RecomendacoesBasicas.Count.ShouldBe(recomendacoes);
    }

    public static void DeveConterListasSerializadas(this AnaliseResultado analise, string descricao, IEnumerable<string> componentes, IEnumerable<string> riscos, IEnumerable<string> recomendacoes)
    {
        analise.DescricaoAnalise.Valor.ShouldBe(descricao);
        analise.ComponentesIdentificados.Select(item => item.Valor).ShouldBe(componentes);
        analise.RiscosArquiteturais.Select(item => item.Valor).ShouldBe(riscos);
        analise.RecomendacoesBasicas.Select(item => item.Valor).ShouldBe(recomendacoes);
    }
}
