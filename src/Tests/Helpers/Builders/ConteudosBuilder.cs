using Domain.ResultadoDiagrama.ValueObjects.RelatorioGerado;

namespace Tests.Helpers.Builders;

public class ConteudosBuilder
{
    private readonly Dictionary<string, string> _valores = new();

    public ConteudosBuilder ComConteudo(string chave, string valor) { _valores[chave] = valor; return this; }

    public ConteudosBuilder Padrao() { _valores["conteudo"] = "ok"; return this; }

    public Conteudos Build() => Conteudos.Criar(_valores);

    public static Conteudos CriarPadrao() => new ConteudosBuilder().Padrao().Build();
}
