namespace shop.Data;
public class Role
{
    public int id { get; set; }
    public string name { get; set; }
    public ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
}