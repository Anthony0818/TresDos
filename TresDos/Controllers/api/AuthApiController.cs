using Azure.Core;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using TresDos.Application.DTOs.UserDto;
using TresDos.Application.Feature.Users.Commands;
using TresDos.Feature.Users.Commands;


[ApiController]
[Route("api/[controller]")]
public class AuthApiController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<AuthApiController> _logger;

    public AuthApiController(IMediator mediator, ILogger<AuthApiController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterUserCommand command)
    {
        try
        {
            return Ok(await _mediator.Send(command));
        }
        catch (ArgumentException ex)
        {
            _logger.LogError(ex, "Register failed for {RegisterUserCommand}", command);
            return StatusCode(500, new { Error = "Internal server error" });
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogError(ex, "Register failed for {RegisterUserCommand}", command);
            return StatusCode(500, new { Error = "Internal server error" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Register failed for {RegisterUserCommand}", command);
            return StatusCode(500, new { Error = "Internal server error" });
        }
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginDto request)
    {
        _logger.LogInformation("Login request received for {Username}", request.Username);
        try
        {
            var token = await _mediator.Send(new LoginCommand(request.Username, request.Password));
            _logger.LogInformation("Login succeeded for {Username}", request.Username);
            return Ok(new { Token = token });
        } 
        catch (UnauthorizedAccessException)
        {
            _logger.LogWarning("Unauthorized login attempt for {Username}", request.Username);
            return Unauthorized(new { Error = "Invalid credentials" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Login failed for {Username}", request.Username);
            return StatusCode(500, new { Error = "Internal server error" });
        }
    }
}