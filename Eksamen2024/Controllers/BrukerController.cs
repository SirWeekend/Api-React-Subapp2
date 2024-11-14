using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Eksamen2024.Models;
using Eksamen2024.Helpers;
using Eksamen2024.DAL;
using System.Reflection.Metadata.Ecma335;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

public class BrukerController : Controller
{
    private readonly ApplicationDbContext _dbContext;

    public BrukerController(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
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

            var isPasswordValid = PasswordHelper.VerifyPassword(password, user.HashedPassword);
            if (!isPasswordValid)
            {
                ModelState.AddModelError("", "Invalid login attempt.");
                return View();
            }

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Email, user.Email)
            };

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);
            HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal).Wait();

            // Lagre brukernavn i TempData for personlig hilsen
            TempData["Username"] = user.Username;

            return RedirectToAction("Index", "Home"); // Omdiriger til hjemmesiden etter innlogging
        }
        return View();
    }

    [HttpGet]
    public IActionResult Registrer()
    {
        return View();
    }

    public IActionResult Lagret()
    {
        return View();
    }

    public IActionResult Admin()
    {
        return View();
    }

    public IActionResult Login()
    {
        return View();   
    }

    [HttpPost]
    public IActionResult Registrer(string username, string email, string password)
    {
        if(ModelState.IsValid)
        {
            var existingUser = _dbContext.Users.SingleOrDefault(u => u.Username == username);
            if(existingUser != null)
            {
                ModelState.AddModelError("", "Username already exists.");
                return View();
            }

            var hashedPassword = PasswordHelper.HashPassword(password);
            var newUser = new User
            {
                Username = username,
                Email = email,
                HashedPassword = hashedPassword
            };

            _dbContext.Users.Add(newUser);
            _dbContext.SaveChanges();

            return RedirectToAction("Login", "Bruker");
        }

        return View();
    }

    public IActionResult Logout()
    {
        HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return RedirectToAction("Login", "Bruker");
    }
}
