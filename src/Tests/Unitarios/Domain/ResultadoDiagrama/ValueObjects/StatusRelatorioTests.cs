using StatusRelatorio = Domain.ResultadoDiagrama.ValueObjects.RelatorioGerado.Status;

namespace Tests.Domain.ResultadoDiagrama.ValueObjects;

public class StatusRelatorioTests
{
    [Theory(DisplayName = "Deve criar status de relatório com valor válido")]
    [InlineData(StatusRelatorioEnum.NaoSolicitado)]
    [InlineData(StatusRelatorioEnum.Solicitado)]
    [InlineData(StatusRelatorioEnum.EmProcessamento)]
    [InlineData(StatusRelatorioEnum.Concluido)]
    [InlineData(StatusRelatorioEnum.Erro)]
    [Trait("ValueObject", "StatusRelatorio")]
    public void Constructor_DeveCriar_QuandoValorValido(StatusRelatorioEnum valor)
    {
        // Act
        var statusRelatorio = new StatusRelatorio(valor);

        // Assert
        statusRelatorio.Valor.ShouldBe(valor);
    }

    [Fact(DisplayName = "Deve lançar exceção quando status de relatório é inválido")]
    [Trait("ValueObject", "StatusRelatorio")]
    public void Constructor_DeveLancarExcecao_QuandoValorInvalido()
    {
        // Arrange
        Action acao = () => _ = new StatusRelatorio((StatusRelatorioEnum)999);

        // Act & Assert
        acao.DeveLancarExcecaoDeValidacao("Status do relatório");
    }

    [Fact(DisplayName = "Deve permitir reconstrução por ORM via construtor privado")]
    [Trait("ValueObject", "StatusRelatorio")]
    public void ConstrutorPrivado_DeveInstanciar_ParaReconstrucaoORM()
    {
        // Act
        var instancia = (StatusRelatorio)Activator.CreateInstance(typeof(StatusRelatorio), nonPublic: true)!;

        // Assert
        instancia.ShouldNotBeNull();
        instancia.Valor.ShouldBe(default(StatusRelatorioEnum));
    }
}