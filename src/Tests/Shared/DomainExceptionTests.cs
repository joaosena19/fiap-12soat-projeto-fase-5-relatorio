namespace Tests.Shared;

public class DomainExceptionTests
{
    [Fact(DisplayName = "Construtor simples deve definir mensagem e ErrorType")]
    [Trait("Shared", "DomainException")]
    public void Construtor_DeveDefinirMensagemEErrorType()
    {
        // Act
        var excecao = new DomainException("entrada inválida", ErrorType.InvalidInput);

        // Assert
        excecao.Message.ShouldBe("entrada inválida");
        excecao.ErrorType.ShouldBe(ErrorType.InvalidInput);
        excecao.LogTemplate.ShouldBe("entrada inválida");
        excecao.LogArgs.ShouldBeEmpty();
    }

    [Fact(DisplayName = "Construtor com valores padrão deve usar InvalidInput")]
    [Trait("Shared", "DomainException")]
    public void Construtor_PadraDeveUsarInvalidInput()
    {
        // Act
        var excecao = new DomainException();

        // Assert
        excecao.Message.ShouldBe("Invalid input");
        excecao.ErrorType.ShouldBe(ErrorType.InvalidInput);
    }

    [Fact(DisplayName = "Construtor com log template deve definir template e args separados")]
    [Trait("Shared", "DomainException")]
    public void Construtor_ComLogTemplate_DeveDefinirTemplateEArgs()
    {
        // Arrange
        var logArgs = new object[] { Guid.NewGuid(), "teste" };

        // Act
        var excecao = new DomainException("mensagem para o usuário", ErrorType.ResourceNotFound, "Recurso {Id} não encontrado: {Motivo}", logArgs);

        // Assert
        excecao.Message.ShouldBe("mensagem para o usuário");
        excecao.ErrorType.ShouldBe(ErrorType.ResourceNotFound);
        excecao.LogTemplate.ShouldBe("Recurso {Id} não encontrado: {Motivo}");
        excecao.LogArgs.Length.ShouldBe(2);
    }

    [Fact(DisplayName = "DomainException deve herdar de Exception")]
    [Trait("Shared", "DomainException")]
    public void DomainException_DeveHerdarDeException()
    {
        // Act
        var excecao = new DomainException("erro", ErrorType.UnexpectedError);

        // Assert
        excecao.ShouldBeAssignableTo<Exception>();
    }
}
