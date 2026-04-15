using DataUltimaTentativaResultadoDiagrama = Domain.ResultadoDiagrama.ValueObjects.ResultadoDiagrama.DataUltimaTentativa;

namespace Tests.Domain.ResultadoDiagrama.ValueObjects;

public class DataUltimaTentativaTests
{
    [Fact(DisplayName = "Deve criar data de última tentativa com valor válido")]
    [Trait("ValueObject", "DataUltimaTentativa")]
    public void Constructor_DeveCriar_QuandoValorValido()
    {
        // Arrange
        var valor = DateTimeOffset.UtcNow;

        // Act
        var dataUltimaTentativa = new DataUltimaTentativaResultadoDiagrama(valor);

        // Assert
        dataUltimaTentativa.Valor.ShouldBe(valor);
    }

    [Fact(DisplayName = "Deve lançar exceção quando data de última tentativa é default")]
    [Trait("ValueObject", "DataUltimaTentativa")]
    public void Constructor_DeveLancarExcecao_QuandoValorDefault()
    {
        // Arrange
        Action acao = () => _ = new DataUltimaTentativaResultadoDiagrama(default);

        // Act & Assert
        acao.DeveLancarExcecaoDeValidacao("Data de última tentativa inválida");
    }

    [Fact(DisplayName = "Deve permitir reconstrução por ORM via construtor privado")]
    [Trait("ValueObject", "DataUltimaTentativa")]
    public void ConstrutorPrivado_DeveInstanciar_ParaReconstrucaoORM()
    {
        // Act
        var instancia = (DataUltimaTentativaResultadoDiagrama)Activator.CreateInstance(typeof(DataUltimaTentativaResultadoDiagrama), nonPublic: true)!;

        // Assert
        instancia.ShouldNotBeNull();
        instancia.Valor.ShouldBe(default(DateTimeOffset));
    }
}
