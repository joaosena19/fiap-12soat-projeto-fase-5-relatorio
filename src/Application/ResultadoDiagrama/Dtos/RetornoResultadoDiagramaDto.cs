using Domain.AnaliseDiagrama.Enums;

namespace Application.ResultadoDiagrama.Dtos;

public class RetornoResultadoDiagramaDto
{
    public Guid AnaliseDiagramaId { get; set; }
    public StatusAnaliseEnum Status { get; set; }
    public List<RelatorioDto> Relatorios { get; set; } = new();
    public List<ErroResultadoDiagramaDto> Erros { get; set; } = new();
    public DateTimeOffset DataCriacao { get; set; }
}

public class RelatorioDto
{
    public TipoRelatorioEnum Tipo { get; set; }
    public StatusRelatorioEnum Status { get; set; }
    public Dictionary<string, string> Conteudos { get; set; } = new();
    public DateTimeOffset? DataGeracao { get; set; }
}

public class ErroResultadoDiagramaDto
{
    public string Mensagem { get; set; } = string.Empty;
    public TipoRelatorioEnum? TipoRelatorio { get; set; }
    public DateTimeOffset DataOcorrencia { get; set; }
}