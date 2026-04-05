using Application.Extensions;

namespace Tests.Application.Extensions;

public class LoggerExtensionsTests
{
    private readonly Mock<IAppLogger> _loggerMock;

    public LoggerExtensionsTests()
    {
        _loggerMock = MockLogger.Criar();
    }

    [Fact(DisplayName = "ComUseCase deve adicionar propriedade UseCase com nome da classe")]
    [Trait("Application", "LoggerExtensions")]
    public void ComUseCase_DeveAdicionarPropriedade_ComNomeDaClasse()
    {
        // Arrange
        var useCase = new FakeUseCase();

        // Act
        var resultado = _loggerMock.Object.ComUseCase(useCase);

        // Assert
        resultado.ShouldNotBeNull();
        _loggerMock.Verify(x => x.ComPropriedade("UseCase", "FakeUseCase"), Times.Once);
    }

    [Fact(DisplayName = "ComEnvioMensagem deve adicionar propriedades de mensageria com tipo Envio")]
    [Trait("Application", "LoggerExtensions")]
    public void ComEnvioMensagem_DeveAdicionarPropriedades_ComTipoEnvio()
    {
        // Arrange
        var publisher = new FakePublisher();

        // Act
        var resultado = _loggerMock.Object.ComEnvioMensagem(publisher);

        // Assert
        resultado.ShouldNotBeNull();
        _loggerMock.Verify(x => x.ComPropriedade("Mensageria", "FakePublisher"), Times.Once);
        _loggerMock.Verify(x => x.ComPropriedade("TipoMensageria", "Envio"), Times.Once);
    }

    [Fact(DisplayName = "ComConsumoMensagem deve adicionar propriedades de mensageria com tipo Consumo")]
    [Trait("Application", "LoggerExtensions")]
    public void ComConsumoMensagem_DeveAdicionarPropriedades_ComTipoConsumo()
    {
        // Arrange
        var consumer = new FakeConsumer();

        // Act
        var resultado = _loggerMock.Object.ComConsumoMensagem(consumer);

        // Assert
        resultado.ShouldNotBeNull();
        _loggerMock.Verify(x => x.ComPropriedade("Mensageria", "FakeConsumer"), Times.Once);
        _loggerMock.Verify(x => x.ComPropriedade("TipoMensageria", "Consumo"), Times.Once);
    }

    [Fact(DisplayName = "ComDomainErrorType deve adicionar propriedade com ErrorType da exception")]
    [Trait("Application", "LoggerExtensions")]
    public void ComDomainErrorType_DeveAdicionarPropriedade_ComErrorType()
    {
        // Arrange
        var excecao = new DomainException("erro teste", ErrorType.ResourceNotFound);

        // Act
        var resultado = _loggerMock.Object.ComDomainErrorType(excecao);

        // Assert
        resultado.ShouldNotBeNull();
        _loggerMock.Verify(x => x.ComPropriedade("DomainErrorType", ErrorType.ResourceNotFound), Times.Once);
    }

    private class FakeUseCase { }
    private class FakePublisher { }
    private class FakeConsumer { }
}
