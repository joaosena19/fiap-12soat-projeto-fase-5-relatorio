using Microsoft.AspNetCore.Mvc;

namespace API.Endpoints;

public abstract class BaseController : ControllerBase
{
    protected readonly ILoggerFactory _loggerFactory;

    protected BaseController(ILoggerFactory loggerFactory)
    {
        _loggerFactory = loggerFactory;
    }
}
