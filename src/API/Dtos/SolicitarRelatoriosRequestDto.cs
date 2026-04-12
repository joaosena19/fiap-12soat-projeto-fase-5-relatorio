using Domain.ResultadoDiagrama.Enums;

namespace API.Dtos;

/// <summary>
/// Requisição para solicitar geração de relatórios pendentes ou com erro.
/// </summary>
public class SolicitarRelatoriosRequestDto
{
    /// <summary>
    /// Lista de tipos de relatório a gerar. Valores possíveis: Json, Markdown, Pdf.
    /// </summary>
    /// <example>["Markdown", "Pdf"]</example>
    public List<TipoRelatorioEnum> TiposRelatorio { get; set; } = new();
}