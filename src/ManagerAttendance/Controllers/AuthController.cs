using ManagerAttendance.Common;
using ManagerAttendance.DTOs;
using ManagerAttendance.Services;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace ManagerAttendance.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    /// <summary>
    /// User Login Endpoint
    /// </summary>
    /// <param name="dto">Login credentials</param>
    /// <returns>AuthResponseDto containing JWT token</returns>
    [HttpPost("login")]
    [ValidationFilter]
    [SwaggerOperation(Summary = "User Login", Description = "Authenticates user credentials and returns a JWT Bearer token.")]
    [ProducesResponseType(typeof(AuthResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Login([FromBody] LoginDto dto)
    {
        var result = await _authService.LoginAsync(dto);
        if (result == null)
        {
            return Unauthorized(new { message = "Invalid email or password." });
        }

        return Ok(result);
    }

    /// <summary>
    /// User Registration Endpoint
    /// </summary>
    /// <param name="dto">Registration details</param>
    /// <returns>AuthResponseDto containing JWT token</returns>
    [HttpPost("register")]
    [ValidationFilter]
    [SwaggerOperation(Summary = "User Registration", Description = "Registers a new user account and returns a JWT Bearer token.")]
    [ProducesResponseType(typeof(AuthResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Register([FromBody] RegisterDto dto)
    {
        var result = await _authService.RegisterAsync(dto);
        if (result == null)
        {
            return BadRequest(new { message = "User registration failed. Email may already be registered." });
        }

        return Ok(result);
    }
}
