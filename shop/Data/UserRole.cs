namespace shop.Data;
public class UserRole
{
    public int user_id { get; set; }
    public int role_id { get; set; }

    public User User { get; set; }
    public Role Role { get; set; }// za[yrtac sie po co tak pisze sie w aspnecie skoro nie ma tego w bazie danych]
}
