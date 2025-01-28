using System.Diagnostics; // nwm po co to jest
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using shop.Models;
using System.Security.Claims;
using shop.Data;
using shop.Services;
using Newtonsoft.Json;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.EntityFrameworkCore;

// chyba za duzo includow

namespace shop.Controllers; 

[Authorize(Roles = "admin")]
public class AdminController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly AuthService _authService;
    // private readonly AppDbContext _context; // tez neipotrzbene
    private readonly ProdCatService _prodCatService;
    private readonly OrderService _orderService;
    private readonly AppDbContext _context;


    public AdminController
    (
        ILogger<HomeController> logger,
        AuthService authService,
        // AppDbContext context, // nie potrzebne raczej b przez servisy to robimy
        ProdCatService prodCatService,
        OrderService orderService,
        AppDbContext context
    )
    {
        _logger = logger;
        _authService = authService;
        // _context = context; // chyba juz nie uzywam bezposrednio
        _prodCatService = prodCatService;
        _orderService = orderService;
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> AddProduct()
    {
        ViewBag.Categories = await _prodCatService.GetCategories();
        return View(new ProductModel());
    }

    [HttpPost]
    public async Task<IActionResult> AddProduct(ProductModel model, string action, string? newCategoryName)
    {
        if (action == "addCategory" && newCategoryName != null)
        {
            ModelState.Clear();
            var categoryModel = new CategoryModel { Name = newCategoryName };
            if (TryValidateModel(categoryModel))
            {
                await _prodCatService.AddCategory(categoryModel);
            }
        }
        else if (action == "addProduct")
        {
            if (ModelState.IsValid)
            {
                await _prodCatService.AddProduct(model);
                return RedirectToAction("Index", "Home");
            }
        }
        else
            throw new ArgumentException("Invalid action", nameof(action));

        ViewBag.Categories = await _prodCatService.GetCategories();
        return View(model);
    }

    [HttpGet]
    public async Task<IActionResult> DeleteProduct(int productId)
    {
        await _prodCatService.DeleteProduct(productId);
        var returnUrl = Request.Headers["Referer"].ToString();
        return Redirect(returnUrl);
    }

    [HttpGet]
    public IActionResult AddCategory()
    {
        return View(new CategoryModel());
    }

    [HttpPost]
    public async Task<IActionResult> AddCategory(CategoryModel model)
    {
        if (ModelState.IsValid)
        {
            await _prodCatService.AddCategory(model);
            return RedirectToAction("Index", "Home");
        }
        return View(model);
    }

    public async Task<IActionResult> ShowOrders()
    {
        var orders = await _orderService.GetOrders();
        ViewBag.Orders = orders;
        return View();
    }   

    public async Task<IActionResult> ShowUsers()
    {
        var users = await _context.Users.Include(u => u.UserRoles).ThenInclude(ur => ur.Role).ToListAsync();
        ViewBag.Users = users;
        return View();
    }
}