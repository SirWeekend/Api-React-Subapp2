using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Eksamen2024.Models;
using Eksamen2024.Helpers;
using System.Reflection.Metadata.Ecma335;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

public class BrukerController : Controller
{
    private readonly ApplicationDbContext _dbContext;

    public BrukerController(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    [HttpGet]
    public IActionResult Login()
    {
        return View();
    }

    [HttpPost]
    public IActionResult Login(string username, string password)
    {
        if (ModelState.IsValid)
        {
            var user = _dbContext.Users.SingleOrDefault(u => u.Username == username);

            if (user == null)
            {
                ModelState.AddModelError("", "Invalid login attempt.");
                return View();
            }

            var passwordHash = PasswordHelper.HashPassword(password);
            if (passwordHash != user.PasswordHash)
            {
                ModelState.AddModelError("", "Invalid login attempt.");
                return View();
            }

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Username)
            };

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);

            HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

            return RedirectToAction("Index", "Home");
        }

        return View();
    }

    [HttpGet]
    public IActionResult Register()
    {
        return View();
    }

    [HttpPost]
    public IActionResult Register(string username, string password)
    {
        if(ModelState.IsValid)
        {
            var existingUser = _dbContext.Users.SingleOrDefault(u => u.Username == username);
            if(existingUser != null)
            {
                return View();
            }

            var passwordHash = PasswordHelper.HashPassword(password);
            var newUser = new User
            {
                Username = username,
                PasswordHash = passwordHash
            };

            _dbContext.Users.Add(newUser);
            _dbContext.SaveChanges();

            return RedirectToAction("Login", "Account");
        }

        return View();
    }

    public IActionResult Logout()
    {
        HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return RedirectToAction("Login", "Account");
    }
}