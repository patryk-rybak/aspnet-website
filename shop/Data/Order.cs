namespace shop.Data
{
    public class Order
    {
        public int id { get; set; }
        public int user_id { get; set; }
        public DateTime created_at { get; set; } = DateTime.UtcNow;
        public string status { get; set; } = "Pending";
        public User User { get; set; }
        public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
    }
}