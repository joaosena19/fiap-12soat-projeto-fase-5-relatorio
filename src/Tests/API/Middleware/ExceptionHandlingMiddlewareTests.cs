using API.Middleware;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace Tests.API.Middleware;

public class ExceptionHandlingMiddlewareTests
{
    private readonly Mock<ILogger<ExceptionHandlingMiddleware>> _loggerMock = new();

    [Fact(DisplayName = "Deve retornar status code correto para DomainException")]
    [Trait("API", "ExceptionHandlingMiddleware")]
    public async Task InvokeAsync_DeveRetornarStatusCorreto_ParaDomainException()
    {
        // Arrange
        var contexto = new DefaultHttpContext();
        contexto.Response.Body = new MemoryStream();
        var middleware = new ExceptionHandlingMiddleware(_ => throw new DomainException("recurso não encontrado", ErrorType.ResourceNotFound), _loggerMock.Object);

        // Act
        await middleware.InvokeAsync(contexto);

        // Assert
        contexto.Response.StatusCode.ShouldBe(404);
        contexto.Response.Body.Seek(0, SeekOrigin.Begin);
        var corpo = await new StreamReader(contexto.Response.Body).ReadToEndAsync();
        var json = JsonSerializer.Deserialize<JsonElement>(corpo);
        json.GetProperty("message").GetString().ShouldBe("recurso não encontrado");
    }

    [Fact(DisplayName = "Deve retornar 500 para exceção genérica")]
    [Trait("API", "ExceptionHandlingMiddleware")]
    public async Task InvokeAsync_DeveRetornar500_ParaExcecaoGenerica()
    {
        // Arrange
        var contexto = new DefaultHttpContext();
        contexto.Response.Body = new MemoryStream();
        var middleware = new ExceptionHandlingMiddleware(_ => throw new InvalidOperationException("falha inesperada"), _loggerMock.Object);

        // Act
        await middleware.InvokeAsync(contexto);

        // Assert
        contexto.Response.StatusCode.ShouldBe(500);
        contexto.Response.Body.Seek(0, SeekOrigin.Begin);
        var corpo = await new StreamReader(contexto.Response.Body).ReadToEndAsync();
        var json = JsonSerializer.Deserialize<JsonElement>(corpo);
        json.GetProperty("statusCode").GetInt32().ShouldBe(500);
    }

    [Fact(DisplayName = "Deve passar requisição adiante quando não há exceção")]
    [Trait("API", "ExceptionHandlingMiddleware")]
    public async Task InvokeAsync_DevePassarRequisicao_QuandoNaoHaExcecao()
    {
        // Arrange
        var foiChamado = false;
        var contexto = new DefaultHttpContext();
        var middleware = new ExceptionHandlingMiddleware(_ => { foiChamado = true; return Task.CompletedTask; }, _loggerMock.Object);

        // Act
        await middleware.InvokeAsync(contexto);

        // Assert
        foiChamado.ShouldBeTrue();
    }
}
