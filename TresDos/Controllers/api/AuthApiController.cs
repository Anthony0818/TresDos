using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;
using TresDos.Application.DTOs.ProductDto;
using TresDos.Application.DTOs.UserDto;
using TresDos.Application.Interfaces;
using TresDos.Infrastructure;

[ApiController]
[Route("api/[controller]")]
public class AuthApiController : ControllerBase
{
    private readonly TokenService _tokenService;
    private readonly IUserService _service;

    public AuthApiController(IUserService service, TokenService tokenService)
    {
        _service = service;
        _tokenService = tokenService;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterUserDto dto)
    {
        //var user = await _service.Login(dto.Username);
        //if (user.Username == dto.Username)
        //    return BadRequest("User already exists.");

        _service.Register(dto);
        return Ok("User registered");
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(UserDto dto)
    {
        var user = await _service.GetByUsernameAsync(dto.Username);
        if (user == null || !VerifyPassword(dto.Password, user.PasswordHash))
            return Unauthorized("Invalid credentials");

        var token = _tokenService.CreateToken(user);
        return Ok(new { token });
    }

    private static string HashPassword(string password)
    {
        using var sha = SHA256.Create();
        return Convert.ToBase64String(sha.ComputeHash(Encoding.UTF8.GetBytes(password)));
    }

    private static bool VerifyPassword(string password, string hash)
        => HashPassword(password) == hash;
}