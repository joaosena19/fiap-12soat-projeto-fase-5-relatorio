using Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace Tests.Infrastructure.Database;

public class DesignTimeDbContextFactoryTests
{
    [Fact(DisplayName = "Deve criar AppDbContext com args vazios")]
    [Trait("Infrastructure", "DesignTimeDbContextFactory")]
    public void CreateDbContext_DeveCriarContexto_QuandoArgsVazios()
    {
        // Arrange
        var factory = new DesignTimeDbContextFactory();

        // Act
        var contexto = factory.CreateDbContext(Array.Empty<string>());

        // Assert
        contexto.ShouldNotBeNull();
        contexto.ShouldBeOfType<AppDbContext>();
        contexto.Database.GetConnectionString().ShouldNotBeNullOrWhiteSpace();

        contexto.Dispose();
    }
}
