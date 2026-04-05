using Domain.ResultadoDiagrama.Aggregates;
using Domain.ResultadoDiagrama.ValueObjects.RelatorioGerado;

namespace Tests.Domain.ResultadoDiagrama;

public class RelatorioGeradoTests
{
    [Fact(DisplayName = "Deve criar relatório com estado inicial esperado")]
    [Trait("Entity", "RelatorioGerado")]
    public void Criar_DeveCriarComEstadoInicial_QuandoTipoValido()
    {
        // Act
        var relatorioGerado = RelatorioGerado.Criar(TipoRelatorioEnum.Markdown);

        // Assert
        relatorioGerado.Tipo.Valor.ShouldBe(TipoRelatorioEnum.Markdown);
        relatorioGerado.Status.Valor.ShouldBe(StatusRelatorioEnum.NaoSolicitado);
        relatorioGerado.Conteudos.Valores.Count.ShouldBe(0);
        relatorioGerado.DataGeracao.ShouldBeNull();
    }

    [Fact(DisplayName = "Deve concluir relatório com conteúdo e data")]
    [Trait("Entity", "RelatorioGerado")]
    public void Concluir_DeveDefinirConteudoEData_QuandoChamado()
    {
        // Arrange
        var relatorioGerado = RelatorioGerado.Criar(TipoRelatorioEnum.Json);
        var conteudos = Conteudos.Criar(new Dictionary<string, string> { ["conteudo"] = "ok" });

        // Act
        relatorioGerado.Concluir(conteudos);

        // Assert
        relatorioGerado.Status.Valor.ShouldBe(StatusRelatorioEnum.Concluido);
        relatorioGerado.Conteudos.ObterValor("conteudo").ShouldBe("ok");
        relatorioGerado.DataGeracao.ShouldNotBeNull();
    }

    [Fact(DisplayName = "Deve permitir gerar quando status é não solicitado")]
    [Trait("Entity", "RelatorioGerado")]
    public void PodeGerar_DeveRetornarTrue_QuandoStatusPermiteGeracao()
    {
        // Arrange
        var relatorioGerado = RelatorioGerado.Criar(TipoRelatorioEnum.Pdf);

        // Act
        var podeGerar = relatorioGerado.PodeGerar();

        // Assert
        podeGerar.ShouldBeTrue();
    }

    [Fact(DisplayName = "Não deve permitir gerar quando status é concluído")]
    [Trait("Entity", "RelatorioGerado")]
    public void PodeGerar_DeveRetornarFalse_QuandoStatusNaoPermiteGeracao()
    {
        // Arrange
        var relatorioGerado = RelatorioGerado.Criar(TipoRelatorioEnum.Pdf);
        relatorioGerado.Concluir(Conteudos.Criar(new Dictionary<string, string> { ["conteudo"] = "ok" }));

        // Act
        var podeGerar = relatorioGerado.PodeGerar();

        // Assert
        podeGerar.ShouldBeFalse();
    }
}