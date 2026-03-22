using Application.Contracts.Gateways;
using Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

/// <summary>
/// Repositório para operações de persistência de resultado de diagrama.
/// </summary>
public class ResultadoDiagramaRepository : IResultadoDiagramaGateway
{
    private readonly AppDbContext _context;

    public ResultadoDiagramaRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Domain.ResultadoDiagrama.Aggregates.ResultadoDiagrama> SalvarAsync(Domain.ResultadoDiagrama.Aggregates.ResultadoDiagrama resultadoDiagrama)
    {
        var existente = await _context.ResultadosDiagrama.FindAsync(resultadoDiagrama.Id);

        if (existente == null)
            await _context.ResultadosDiagrama.AddAsync(resultadoDiagrama);

        await _context.SaveChangesAsync();
        return resultadoDiagrama;
    }

    public async Task<Domain.ResultadoDiagrama.Aggregates.ResultadoDiagrama?> ObterPorAnaliseDiagramaIdAsync(Guid analiseDiagramaId)
    {
        return await _context.ResultadosDiagrama.FirstOrDefaultAsync(item => item.AnaliseDiagramaId == analiseDiagramaId);
    }
}