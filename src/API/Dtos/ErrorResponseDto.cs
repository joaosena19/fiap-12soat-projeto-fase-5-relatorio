namespace API.Dtos;

/// <summary>
/// DTO para respostas de erro padronizadas da API
/// </summary>
public class ErrorResponseDto
{
    public string Message { get; set; }
    public int StatusCode { get; set; }

    public ErrorResponseDto(string message, int statusCode)
    {
        Message = message;
        StatusCode = statusCode;
    }
}
