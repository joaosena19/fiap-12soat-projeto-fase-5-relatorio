using API.Dtos;
using API.Presenters;
using Application.Contracts.Messaging;
using Application.ResultadoDiagrama.Dtos;
using Domain.AnaliseDiagrama.Enums;
using Infrastructure.Database;
using Infrastructure.Handlers;
using Infrastructure.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace API.Endpoints;

/// <summary>
/// Controller responsável pela consulta de relatórios de análises de diagramas.
/// </summary>
[Route("api/relatorio")]
[ApiController]
[Produces("application/json")]
public class RelatorioController : BaseController
{
    private readonly AppDbContext _context;
    private readonly IRelatorioMessagePublisher _messagePublisher;

    public RelatorioController(AppDbContext context, ILoggerFactory loggerFactory, IRelatorioMessagePublisher messagePublisher) : base(loggerFactory)
    {
        _context = context;
        _messagePublisher = messagePublisher;
    }

    /// <summary>
    /// Busca uma análise de diagrama pelo AnaliseDiagramaId.
    /// </summary>
    /// <returns>Dados dos relatórios da análise</returns>
    [HttpGet("{analiseDiagramaId:guid}")]
    [ProducesResponseType(typeof(RetornoResultadoDiagramaDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> BuscarPorAnaliseDiagramaId(Guid analiseDiagramaId)
    {
        var gateway = new ResultadoDiagramaRepository(_context);
        var presenter = new BuscarResultadoDiagramaPresenter();
        var handler = new ResultadoDiagramaHandler(_loggerFactory);

        await handler.BuscarPorAnaliseDiagramaIdAsync(analiseDiagramaId, gateway, presenter);

        return presenter.ObterResultado();
    }

    /// <summary>
    /// Solicita a geração de relatórios pendentes ou com erro.
    /// </summary>
    /// <returns>Status da solicitação por tipo de relatório</returns>
    [HttpPost("{analiseDiagramaId:guid}")]
    [ProducesResponseType(typeof(ResultadoSolicitacaoRelatoriosDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ResultadoSolicitacaoRelatoriosDto), StatusCodes.Status202Accepted)]
    [ProducesResponseType(typeof(ResultadoSolicitacaoRelatoriosDto), 207)]
    [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> SolicitarRelatorios(Guid analiseDiagramaId, [FromBody] SolicitarRelatoriosRequestDto request)
    {
        var gateway = new ResultadoDiagramaRepository(_context);
        var presenter = new SolicitarGeracaoRelatoriosPresenter();
        var handler = new ResultadoDiagramaHandler(_loggerFactory);

        await handler.SolicitarGeracaoRelatoriosAsync(analiseDiagramaId, request.TiposRelatorio ?? new List<TipoRelatorioEnum>(), gateway, _messagePublisher, presenter);

        return presenter.ObterResultado();
    }
}
