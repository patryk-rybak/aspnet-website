using System.Diagnostics;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using shop.Data;
using shop.Models;
using shop.ProductCategory;

namespace shop.Controllers; //niepewny czy jest dobrze

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly ProdCatService _prodCatService;

    public HomeController(ILogger<HomeController> logger, ProdCatService prodCatService)
    {
        _logger = logger;
        _prodCatService = prodCatService;
    }

    [HttpGet]
    public async Task<IActionResult> Index(int? categoryId) // to mozna zamienic na dodatkowy action ktory zwroci Index view po prostu
    {
        if (categoryId != null)
        {
            List<Product> products = await _prodCatService.GetProductsByCategory((int)categoryId);
            ViewBag.FrontProducts = products;
        }
        else
        {
            List<Product> products = await _prodCatService.GetFrontProducts();
            ViewBag.FrontProducts = products;
        }
        List<Category> categories = await _prodCatService.GetCategories();
        ViewBag.Categories = categories;
        ViewBag.CategoryId = categoryId;
        return View();
    }

    [HttpGet]
    public async Task<IActionResult> Search(string query)
    {
        if (string.IsNullOrEmpty(query))
        {
            return Redirect(Request.Headers["Referer"].ToString());
        }

        List<Product> products = await _prodCatService.SearchProductsByQuery(query);
        ViewBag.FrontProducts = products;
        ViewBag.CategoryId = null;
        List<Category> categories = await _prodCatService.GetCategories();
        ViewBag.Categories = categories;
        return View("Index");
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}