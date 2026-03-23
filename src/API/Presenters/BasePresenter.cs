using API.Dtos;
using API.Extensions;
using Microsoft.AspNetCore.Mvc;
using Shared.Enums;

namespace API.Presenters;

public abstract class BasePresenter
{
    protected IActionResult? _resultado;
    protected bool _foiSucesso;

    protected BasePresenter()
    {
        _foiSucesso = false;
    }

    public void ApresentarErro(string mensagem, ErrorType errorType)
    {
        var statusCode = (int)errorType.ToHttpStatusCode();
        var errorResponse = new ErrorResponseDto(mensagem, statusCode);

        _resultado = new ObjectResult(errorResponse) { StatusCode = statusCode };
        _foiSucesso = false;
    }

    public IActionResult ObterResultado()
    {
        return _resultado ?? new StatusCodeResult(500);
    }

    public bool FoiSucesso => _foiSucesso;

    protected void DefinirSucesso(IActionResult resultado)
    {
        _resultado = resultado;
        _foiSucesso = true;
    }

    protected void DefinirSucesso(object dados)
    {
        _resultado = new OkObjectResult(dados);
        _foiSucesso = true;
    }
}
