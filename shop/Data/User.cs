namespace shop.Data;
public class User
{
    public int id { get; set; }
    public string name { get; set; }
    public string email { get; set; }
    public ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
    public ICollection<Order> Orders { get; set; } = new List<Order>();
}
