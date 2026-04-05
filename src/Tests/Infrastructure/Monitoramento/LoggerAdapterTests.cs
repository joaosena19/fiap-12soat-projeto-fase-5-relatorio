using Infrastructure.Monitoramento;
using Microsoft.Extensions.Logging;

namespace Tests.Infrastructure.Monitoramento;

public class LoggerAdapterTests
{
    private readonly Mock<ILogger<object>> _loggerMock;
    private readonly LoggerAdapter<object> _adapter;

    public LoggerAdapterTests()
    {
        _loggerMock = new Mock<ILogger<object>>();
        _adapter = new LoggerAdapter<object>(_loggerMock.Object);
    }

    [Fact(DisplayName = "LogDebug deve chamar ILogger.LogDebug")]
    [Trait("Infrastructure", "LoggerAdapter")]
    public void LogDebug_DeveChamarILogger()
    {
        // Act
        _adapter.LogDebug("mensagem {Param}", "valor");

        // Assert
        _loggerMock.Verify(x => x.Log(LogLevel.Debug, It.IsAny<EventId>(), It.IsAny<It.IsAnyType>(), null, It.IsAny<Func<It.IsAnyType, Exception?, string>>()), Times.Once);
    }

    [Fact(DisplayName = "LogInformation deve chamar ILogger.LogInformation")]
    [Trait("Infrastructure", "LoggerAdapter")]
    public void LogInformation_DeveChamarILogger()
    {
        // Act
        _adapter.LogInformation("mensagem {Param}", "valor");

        // Assert
        _loggerMock.Verify(x => x.Log(LogLevel.Information, It.IsAny<EventId>(), It.IsAny<It.IsAnyType>(), null, It.IsAny<Func<It.IsAnyType, Exception?, string>>()), Times.Once);
    }

    [Fact(DisplayName = "LogWarning deve chamar ILogger.LogWarning")]
    [Trait("Infrastructure", "LoggerAdapter")]
    public void LogWarning_DeveChamarILogger()
    {
        // Act
        _adapter.LogWarning("mensagem {Param}", "valor");

        // Assert
        _loggerMock.Verify(x => x.Log(LogLevel.Warning, It.IsAny<EventId>(), It.IsAny<It.IsAnyType>(), null, It.IsAny<Func<It.IsAnyType, Exception?, string>>()), Times.Once);
    }

    [Fact(DisplayName = "LogWarning com exception deve chamar ILogger.LogWarning com exception")]
    [Trait("Infrastructure", "LoggerAdapter")]
    public void LogWarning_ComException_DeveChamarILogger()
    {
        // Arrange
        var excecao = new InvalidOperationException("falha");

        // Act
        _adapter.LogWarning(excecao, "mensagem {Param}", "valor");

        // Assert
        _loggerMock.Verify(x => x.Log(LogLevel.Warning, It.IsAny<EventId>(), It.IsAny<It.IsAnyType>(), excecao, It.IsAny<Func<It.IsAnyType, Exception?, string>>()), Times.Once);
    }

    [Fact(DisplayName = "LogError deve chamar ILogger.LogError")]
    [Trait("Infrastructure", "LoggerAdapter")]
    public void LogError_DeveChamarILogger()
    {
        // Act
        _adapter.LogError("mensagem {Param}", "valor");

        // Assert
        _loggerMock.Verify(x => x.Log(LogLevel.Error, It.IsAny<EventId>(), It.IsAny<It.IsAnyType>(), null, It.IsAny<Func<It.IsAnyType, Exception?, string>>()), Times.Once);
    }

    [Fact(DisplayName = "LogError com exception deve chamar ILogger.LogError com exception")]
    [Trait("Infrastructure", "LoggerAdapter")]
    public void LogError_ComException_DeveChamarILogger()
    {
        // Arrange
        var excecao = new InvalidOperationException("falha");

        // Act
        _adapter.LogError(excecao, "mensagem {Param}", "valor");

        // Assert
        _loggerMock.Verify(x => x.Log(LogLevel.Error, It.IsAny<EventId>(), It.IsAny<It.IsAnyType>(), excecao, It.IsAny<Func<It.IsAnyType, Exception?, string>>()), Times.Once);
    }

    [Fact(DisplayName = "ComPropriedade deve retornar ContextualLogger")]
    [Trait("Infrastructure", "LoggerAdapter")]
    public void ComPropriedade_DeveRetornarContextualLogger()
    {
        // Act
        var resultado = _adapter.ComPropriedade("chave", "valor");

        // Assert
        resultado.ShouldNotBeNull();
        resultado.ShouldBeOfType<ContextualLogger>();
    }
}
