namespace Application.Contracts.Armazenamento;

/// <summary>
/// Contrato para armazenamento de arquivos gerados pelo serviço de relatórios.
/// </summary>
public interface IArmazenamentoArquivoService
{
    /// <summary>
    /// Armazena um arquivo e retorna a URL de acesso.
    /// </summary>
    /// <returns>URL pública ou interna do arquivo armazenado.</returns>
    Task<string> ArmazenarAsync(Guid analiseDiagramaId, byte[] conteudo, string nomeArquivo, string contentType);
}
