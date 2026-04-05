using Infrastructure.Monitoramento;

namespace Tests.Infrastructure.Monitoramento;

public class ContextualLoggerTests
{
    private readonly Mock<IAppLogger> _innerLoggerMock;
    private readonly ContextualLogger _contextualLogger;

    public ContextualLoggerTests()
    {
        _innerLoggerMock = new Mock<IAppLogger>();
        _innerLoggerMock.Setup(x => x.ComPropriedade(It.IsAny<string>(), It.IsAny<object?>())).Returns(_innerLoggerMock.Object);
        _contextualLogger = new ContextualLogger(_innerLoggerMock.Object, new Dictionary<string, object?> { ["chave"] = "valor" });
    }

    [Fact(DisplayName = "LogDebug deve delegar para o inner logger")]
    [Trait("Infrastructure", "ContextualLogger")]
    public void LogDebug_DeveDelegar_ParaInnerLogger()
    {
        // Act
        _contextualLogger.LogDebug("mensagem {Param}", "teste");

        // Assert
        _innerLoggerMock.Verify(x => x.LogDebug("mensagem {Param}", It.IsAny<object[]>()), Times.Once);
    }

    [Fact(DisplayName = "LogInformation deve delegar para o inner logger")]
    [Trait("Infrastructure", "ContextualLogger")]
    public void LogInformation_DeveDelegar_ParaInnerLogger()
    {
        // Act
        _contextualLogger.LogInformation("mensagem {Param}", "teste");

        // Assert
        _innerLoggerMock.Verify(x => x.LogInformation("mensagem {Param}", It.IsAny<object[]>()), Times.Once);
    }

    [Fact(DisplayName = "LogWarning deve delegar para o inner logger")]
    [Trait("Infrastructure", "ContextualLogger")]
    public void LogWarning_DeveDelegar_ParaInnerLogger()
    {
        // Act
        _contextualLogger.LogWarning("mensagem {Param}", "teste");

        // Assert
        _innerLoggerMock.Verify(x => x.LogWarning("mensagem {Param}", It.IsAny<object[]>()), Times.Once);
    }

    [Fact(DisplayName = "LogWarning com exception deve delegar para o inner logger")]
    [Trait("Infrastructure", "ContextualLogger")]
    public void LogWarning_ComException_DeveDelegar_ParaInnerLogger()
    {
        // Arrange
        var excecao = new InvalidOperationException("falha");

        // Act
        _contextualLogger.LogWarning(excecao, "mensagem {Param}", "teste");

        // Assert
        _innerLoggerMock.Verify(x => x.LogWarning(excecao, "mensagem {Param}", It.IsAny<object[]>()), Times.Once);
    }

    [Fact(DisplayName = "LogError deve delegar para o inner logger")]
    [Trait("Infrastructure", "ContextualLogger")]
    public void LogError_DeveDelegar_ParaInnerLogger()
    {
        // Act
        _contextualLogger.LogError("mensagem {Param}", "teste");

        // Assert
        _innerLoggerMock.Verify(x => x.LogError("mensagem {Param}", It.IsAny<object[]>()), Times.Once);
    }

    [Fact(DisplayName = "LogError com exception deve delegar para o inner logger")]
    [Trait("Infrastructure", "ContextualLogger")]
    public void LogError_ComException_DeveDelegar_ParaInnerLogger()
    {
        // Arrange
        var excecao = new InvalidOperationException("falha");

        // Act
        _contextualLogger.LogError(excecao, "mensagem {Param}", "teste");

        // Assert
        _innerLoggerMock.Verify(x => x.LogError(excecao, "mensagem {Param}", It.IsAny<object[]>()), Times.Once);
    }

    [Fact(DisplayName = "ComPropriedade deve retornar nova instância com contexto adicional")]
    [Trait("Infrastructure", "ContextualLogger")]
    public void ComPropriedade_DeveRetornarNovaInstancia_ComContextoAdicional()
    {
        // Act
        var novoLogger = _contextualLogger.ComPropriedade("novaChave", "novoValor");

        // Assert
        novoLogger.ShouldNotBeNull();
        novoLogger.ShouldNotBeSameAs(_contextualLogger);
        novoLogger.ShouldBeOfType<ContextualLogger>();
    }
}
