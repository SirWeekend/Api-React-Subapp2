using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Eksamen2024.Models;
using Eksamen2024.Helpers;
using Eksamen2024.DAL;
using Eksamen2024.DTOs;

[ApiController]
[Route("api/[controller]")]
public class BrukerController : ControllerBase
{
    private readonly ApplicationDbContext _dbContext;

    public BrukerController(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    /// <summary>
    /// Login bruker
    /// </summary>
    [HttpPost("Login")]
    public IActionResult Login([FromBody] LoginRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest("Invalid data provided.");
        }

        var user = _dbContext.Users.SingleOrDefault(u => u.Username == request.Username);

        if (user == null || !PasswordHelper.VerifyPassword(request.Password, user.HashedPassword))
        {
            return Unauthorized("Invalid username or password.");
        }

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
            new Claim(ClaimTypes.Name, user.Username),
            new Claim(ClaimTypes.Email, user.Email)
        };

        var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        var principal = new ClaimsPrincipal(identity);
        HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal).Wait();

        return Ok(new { Username = user.Username });
    }

    /// <summary>
    /// Registrer ny bruker
    /// </summary>
    [HttpPost("Registrer")]
    public IActionResult Registrer([FromBody] RegisterRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest("Invalid data provided.");
        }

        var existingUser = _dbContext.Users.SingleOrDefault(u => u.Username == request.Username);
        if (existingUser != null)
        {
            return Conflict("Username already exists.");
        }

        var hashedPassword = PasswordHelper.HashPassword(request.Password);
        var newUser = new User
        {
            Username = request.Username,
            Email = request.Email,
            HashedPassword = hashedPassword
        };

        _dbContext.Users.Add(newUser);
        _dbContext.SaveChanges();

        return Ok("User registered successfully.");
    }

    /// <summary>
    /// Logout bruker
    /// </summary>
    [HttpPost("Logout")]
    public IActionResult Logout()
    {
        HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return Ok("User logged out successfully.");
    }
}
