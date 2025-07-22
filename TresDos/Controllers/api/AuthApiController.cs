using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;
using TresDos.Data;
using TresDos.Models;
using TresDosApi.DTOs;

[ApiController]
[Route("api/[controller]")]
public class AuthApiController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly TokenService _tokenService;

    public AuthApiController(AppDbContext context, TokenService tokenService)
    {
        _context = context;
        _tokenService = tokenService;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(UserDto dto)
    {
        if (await _context.Users.AnyAsync(u => u.Username == dto.Username))
            return BadRequest("User already exists.");

        var user = new User
        {
            Username = dto.Username,
            PasswordHash = HashPassword(dto.Password),
            Role = "User"
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        return Ok("User registered");
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(UserDto dto)
    {
        var user = await _context.Users.SingleOrDefaultAsync(u => u.Username == dto.Username);
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