using Microsoft.AspNetCore.Mvc;
using Todo.Application.Common.Models;
using System.Security.Claims;

namespace Todo.Api.Controllers;

[ApiController]
[Route("api/v{version:apiVersion}/[controller]")]
public abstract class ApiController : ControllerBase
{
    protected string UserId => User.FindFirstValue(ClaimTypes.NameIdentifier) ?? throw new UnauthorizedAccessException();

    protected IActionResult HandleResult(Result result)
    {
        if (result.IsSuccess)
        {
            return Ok();
        }

        return MapError(result.Error!);
    }

    protected IActionResult HandleResult<T>(Result<T> result)
    {
        if (result.IsSuccess)
        {
            return Ok(result.Value);
        }

        return MapError(result.Error!);
    }

    private IActionResult MapError(ResultError error)
    {
        return error.Type switch
        {
            ResultErrorType.Validation => BadRequest(new { error.Message, error.Details }),
            ResultErrorType.NotFound => NotFound(new { error.Message }),
            ResultErrorType.Conflict => Conflict(new { error.Message }),
            _ => StatusCode(500, new { error.Message })
        };
    }
}
