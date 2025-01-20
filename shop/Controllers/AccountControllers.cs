using System.Diagnostics; // nwm po co to jest
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using shop.Models;
using System.Security.Claims;
using shop.Authentication;
using shop.Data;

namespace shop.Controllers; //niepewny czy jest dobrze

public class AccountController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly AuthService _authService;
    private readonly AppDbContext _context;


    public AccountController
    (
        ILogger<HomeController> logger,
        AuthService authService,
        AppDbContext context
    )
    {
        _logger = logger;
        _authService = authService;
        _context = context;
    }

    [HttpGet]
    public IActionResult Login(int? categoryId)
    {
        // czy nei musz etu owrzyc modeu i go przekazywac aby podtrzyac stan?
        if (User.Identity.IsAuthenticated)
            return RedirectToAction("Index", "Home", new { categoryId });
        ViewBag.CategoryId = categoryId;
        return View();
    }
    
    [HttpPost]
    public async Task<IActionResult> Login(LoginModel model, int? categoryId)
    {
        if (User.Identity.IsAuthenticated)
            return RedirectToAction("Index", "Home", new { categoryId });

        if (ModelState.IsValid)
        {
            var user = await _authService.Login(model);
            if (user != null)
            {
                var userRoles = await _authService.GetUserRoles(user.id); // ogarniamy role
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Email, model.Email),
                    new Claim(ClaimTypes.Name, user.name)
                };
                claims.AddRange(userRoles.Select(role => new Claim(ClaimTypes.Role, role)));

                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var authProperties = new AuthenticationProperties
                {
                    IsPersistent = true// bez tego przegladarka moze po wylaczeniu usunac cookie
                }; 
                HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity), authProperties);
                return RedirectToAction("Index", "Home", new { categoryId });
            }
        }
        ViewBag.CategoryId = categoryId;
        return View(model);
    }

    [HttpGet]
    public IActionResult Register(int? categoryId)
    {
        if (User.Identity.IsAuthenticated)
            return RedirectToAction("Index", "Home", new { categoryId });
        ViewBag.CategoryId = categoryId;
        return View();
    }
    
    [HttpPost]
    public async Task <IActionResult> Register(RegisterModel model, int? categoryId)
    {
        if (User.Identity.IsAuthenticated)
            return RedirectToAction("Index", "Home", new { categoryId });

        if (ModelState.IsValid)
        {
            await _authService.Register(model);
            return RedirectToAction("Login", new { categoryId });
        }
        ViewBag.CategoryId = categoryId;
        return View(model);
    }

    [HttpGet]
    [Authorize]
    public async Task<IActionResult> Logout(int? categoryId)
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return RedirectToAction("Index", "Home", new { categoryId });
    }

    public IActionResult AddToCart()
    {
        return View();
    }

    // ... cart
}
