using Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace Tests.Infrastructure.Database;

public class DesignTimeDbContextFactoryTests
{
    [Fact(DisplayName = "Deve criar AppDbContext com string de conexão padrão")]
    [Trait("Infrastructure", "DesignTimeDbContextFactory")]
    public void CreateDbContext_DeveCriarContexto_ComStringConexaoPadrao()
    {
        // Arrange
        var factory = new DesignTimeDbContextFactory();

        // Act
        var contexto = factory.CreateDbContext(Array.Empty<string>());

        // Assert
        contexto.ShouldNotBeNull();
        contexto.ShouldBeOfType<AppDbContext>();

        contexto.Dispose();
    }

    [Fact(DisplayName = "Deve criar AppDbContext com args vazios")]
    [Trait("Infrastructure", "DesignTimeDbContextFactory")]
    public void CreateDbContext_DeveCriarContexto_QuandoArgsVazios()
    {
        // Arrange
        var factory = new DesignTimeDbContextFactory();

        // Act
        var contexto = factory.CreateDbContext(new string[] { });

        // Assert
        contexto.ShouldNotBeNull();
        contexto.Database.GetConnectionString().ShouldNotBeNullOrWhiteSpace();

        contexto.Dispose();
    }
}
