using shop.Services;
using shop.Data;
using Microsoft.AspNetCore.Mvc;

namespace shop.Controllers
{
    public class ShoppingCartController : Controller
    {
        private readonly ShoppingCartService _shoppingCartService;
        private readonly ProdCatService _prodCatService;

        public ShoppingCartController(ShoppingCartService shoppingCartService, ProdCatService prodCatService)
        {
            _shoppingCartService = shoppingCartService;
            _prodCatService = prodCatService;
        }

        public async Task<IActionResult> Cart()
        {
            var cartProductsIds = _shoppingCartService.GetCart();
            var validCartProducts = new List<Product>();

            foreach (var id in cartProductsIds)
            {
                var product = await _prodCatService.GetProduct(id);
                if (product != null)
                    validCartProducts.Add(product);
            }

            ViewBag.CartProducts = validCartProducts;
            return View();
        }

        [HttpGet]
        public IActionResult Add(int productId)
        {
            _shoppingCartService.AddToCart(productId);
            return Redirect(Request.Headers["Referer"].ToString());
        }

        [HttpGet]
        public IActionResult Remove(int productId)
        {
            _shoppingCartService.RemoveFromCart(productId);
            return Redirect(Request.Headers["Referer"].ToString());
        }

        [HttpGet]
        public IActionResult Clear()
        {
            _shoppingCartService.ClearCart();
            return Redirect(Request.Headers["Referer"].ToString());
        }
    }
}