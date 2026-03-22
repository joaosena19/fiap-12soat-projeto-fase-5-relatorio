using System.Net;
using Shared.Enums;

namespace API.Extensions;

/// <summary>
/// Extensões para converter tipos de erro customizados para códigos de status HTTP
/// </summary>
public static class ErrorTypeExtensions
{
    public static HttpStatusCode ToHttpStatusCode(this ErrorType errorType)
    {
        return errorType switch
        {
            ErrorType.InvalidInput => HttpStatusCode.BadRequest,
            ErrorType.ResourceNotFound => HttpStatusCode.NotFound,
            ErrorType.ReferenceNotFound => HttpStatusCode.UnprocessableEntity,
            ErrorType.DomainRuleBroken => HttpStatusCode.UnprocessableEntity,
            ErrorType.Conflict => HttpStatusCode.Conflict,
            ErrorType.Unauthorized => HttpStatusCode.Unauthorized,
            ErrorType.NotAllowed => HttpStatusCode.Forbidden,
            ErrorType.UnexpectedError => HttpStatusCode.InternalServerError,
            _ => HttpStatusCode.InternalServerError
        };
    }
}
