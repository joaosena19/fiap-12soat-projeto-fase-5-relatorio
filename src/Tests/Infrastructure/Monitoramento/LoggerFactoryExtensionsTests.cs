using Infrastructure.Monitoramento;
using Microsoft.Extensions.Logging;

namespace Tests.Infrastructure.Monitoramento;

public class LoggerFactoryExtensionsTests
{
    [Fact(DisplayName = "CriarAppLogger deve retornar LoggerAdapter")]
    [Trait("Infrastructure", "LoggerFactoryExtensions")]
    public void CriarAppLogger_DeveRetornarLoggerAdapter()
    {
        // Arrange
        var loggerFactoryMock = new Mock<ILoggerFactory>();
        loggerFactoryMock.Setup(x => x.CreateLogger(It.IsAny<string>())).Returns(new Mock<ILogger>().Object);

        // Act
        var resultado = loggerFactoryMock.Object.CriarAppLogger<object>();

        // Assert
        resultado.ShouldNotBeNull();
        resultado.ShouldBeOfType<LoggerAdapter<object>>();
    }
}
