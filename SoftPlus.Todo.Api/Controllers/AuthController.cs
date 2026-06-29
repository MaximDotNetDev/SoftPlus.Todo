using Microsoft.AspNetCore.Mvc;
using SoftPlus.Todo.Interfaces.DTOs.Auth;
using SoftPlus.Todo.Interfaces.Services;

namespace SoftPlus.Todo.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class AuthController(IAuthService authService) : ControllerBase
{
    [HttpPost("register")]
    [ProducesResponseType(typeof(AuthResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Register([FromBody] RegisterDto dto, CancellationToken cancellationToken)
    {
        var (success, errorMessage, data) = await authService.RegisterAsync(dto, cancellationToken).ConfigureAwait(false);

        if (!success)
        {
            return BadRequest(errorMessage);
        }

        return Ok(data);
    }

    [HttpPost("login")]
    [ProducesResponseType(typeof(AuthResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Login([FromBody] LoginDto dto, CancellationToken cancellationToken)
    {
        var (success, errorMessage, data) = await authService.LoginAsync(dto, cancellationToken).ConfigureAwait(false);

        if (!success)
        {
            return BadRequest(errorMessage);
        }

        return Ok(data);
    }
}