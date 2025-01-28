using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using shop.Data;

namespace shop.Services
{
    public class OrderService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly AppDbContext _context;
        private readonly int userId;

        public OrderService(IHttpContextAccessor httpContextAccessor, AppDbContext context)
        {
            _httpContextAccessor = httpContextAccessor;
            _context = context;
            userId = int.Parse(_httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
        }

        public void Submit(List<int> cart) // powinienem miec chyba ogolny model  koszyka z odpowiednimi kotraktami i kazac serwisom z niego korzystac i najwyzej jakby reprezentacja koszyka sie jakos zmieniala to i musialaby spelniac kontrakty
        {
            var order = new Order { user_id = userId };
            _context.Orders.Add(order); // co jesli race condiditon
            _context.SaveChanges();
            foreach (var productId in cart)
                _context.OrderItems.Add(new OrderItem { product_id = productId, order_id = order.id });
            _context.SaveChanges();
        }

        public async Task<List<Order>> GetOrders()
        {
            return await _context.Orders
                .Include(o => o.User)
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
                .ToListAsync();
        }
    }
}