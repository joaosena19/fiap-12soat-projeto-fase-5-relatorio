using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Database;

/// <summary>
/// Contexto de banco de dados do serviço de Relatório.
/// </summary>
public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options) { }

    public DbSet<Domain.AnaliseDiagrama.Aggregates.ResultadoDiagrama> ResultadosDiagrama { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
    }
}
