using System.Security.Claims;
using Newtonsoft.Json;
using shop.Data;

namespace shop.Services
{
    public class ShoppingCartService
    {
        private readonly string cartCoockieName;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ShoppingCartService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
            cartCoockieName = "ShoppingCart" + _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        }

        public List<int> GetCart()
        {
            var cookie = _httpContextAccessor.HttpContext.Request.Cookies[cartCoockieName];
            if (string.IsNullOrEmpty(cookie))
                return new List<int>();

            return string.IsNullOrEmpty(cookie)
            ? new List<int>()
            : JsonConvert.DeserializeObject<List<int>>(cookie);
        }

        public void AddToCart(int productId)
        {
            var cart = GetCart();
            if (cart.Any(id => id == productId))
                return;

            cart.Add(productId);
            var cookieValue = JsonConvert.SerializeObject(cart);
            _httpContextAccessor.HttpContext.Response.Cookies.Append(cartCoockieName, cookieValue);
        }

        public void RemoveFromCart(int productId)
        {
            var cart = GetCart();
            if (!cart.Any(id => id == productId))
                return;
            
            cart.Remove(productId);
            var cookieValue = JsonConvert.SerializeObject(cart);
            _httpContextAccessor.HttpContext.Response.Cookies.Append(cartCoockieName, cookieValue);
        }

        public void ClearCart()
        {
            _httpContextAccessor.HttpContext.Response.Cookies.Delete(cartCoockieName);
        }
    }
}