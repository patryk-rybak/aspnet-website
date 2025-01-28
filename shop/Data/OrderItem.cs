namespace shop.Data
{
    public class OrderItem
    {
        public int id { get; set; }
        public int order_id { get; set; }
        public int product_id { get; set; }
        public Order Order { get; set; }
        public Product Product { get; set; }
    }
}