using API.Extensions;
using System.Net;

namespace Tests.API.Extensions;

public class ErrorTypeExtensionsTests
{
    [Theory(DisplayName = "Deve mapear ErrorType para HttpStatusCode corretamente")]
    [Trait("API", "ErrorTypeExtensions")]
    [InlineData(ErrorType.InvalidInput, HttpStatusCode.BadRequest)]
    [InlineData(ErrorType.ResourceNotFound, HttpStatusCode.NotFound)]
    [InlineData(ErrorType.ReferenceNotFound, HttpStatusCode.UnprocessableEntity)]
    [InlineData(ErrorType.DomainRuleBroken, HttpStatusCode.UnprocessableEntity)]
    [InlineData(ErrorType.Conflict, HttpStatusCode.Conflict)]
    [InlineData(ErrorType.Unauthorized, HttpStatusCode.Unauthorized)]
    [InlineData(ErrorType.NotAllowed, HttpStatusCode.Forbidden)]
    [InlineData(ErrorType.UnexpectedError, HttpStatusCode.InternalServerError)]
    public void ToHttpStatusCode_DeveMapearCorretamente(ErrorType errorType, HttpStatusCode expectedStatusCode)
    {
        // Act
        var resultado = errorType.ToHttpStatusCode();

        // Assert
        resultado.ShouldBe(expectedStatusCode);
    }

    [Fact(DisplayName = "Deve retornar InternalServerError para valor desconhecido")]
    [Trait("API", "ErrorTypeExtensions")]
    public void ToHttpStatusCode_DeveRetornarInternalServerError_ParaValorDesconhecido()
    {
        // Arrange
        var errorType = (ErrorType)999;

        // Act
        var resultado = errorType.ToHttpStatusCode();

        // Assert
        resultado.ShouldBe(HttpStatusCode.InternalServerError);
    }
}
