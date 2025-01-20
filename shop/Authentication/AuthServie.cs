using shop.Data;
using shop.Models;
using Microsoft.EntityFrameworkCore;

namespace shop.Authentication
{
    public class AuthService
    {
        private readonly AppDbContext _context;
        private readonly PasswordHasher _hasher = new();

        public AuthService(AppDbContext context)
        {
            _context = context;
        }

        public async Task Register(RegisterModel model)
        {
            var hashPassword = _hasher.HashPassword(model.Password);

            var user = new User { name = model.Name, email = model.Email };
            await _context.Users.AddAsync(user); 
            await _context.SaveChangesAsync();

            var password = new Password { user_id = user.id, hash = hashPassword };
            await _context.Passwords.AddAsync(password);

            var selectedRole = await _context.Roles.SingleOrDefaultAsync(r => r.name == model.Role);
            if(selectedRole == null)
            {
                throw new Exception("Wybrana rola nie istnieje.");
            }

            var userRole = new UserRole
            {
                user_id = user.id,
                role_id = selectedRole.id
            };
            await _context.UserRoles.AddAsync(userRole);
            await _context.SaveChangesAsync();
        }

        public async Task<User> Login(LoginModel model)
        {
            var user = await _context.Users.SingleOrDefaultAsync(u => u.email == model.Email);
            if (user == null) return null;

            var passwordEntity = await _context.Passwords.SingleOrDefaultAsync(p => p.user_id == user.id);
            if (passwordEntity == null) return null;

            if (_hasher.Verify(model.Password, passwordEntity.hash))
            {
                return user;
            }

            return null;
        }

        public async Task<List<string>> GetUserRoles(int userId)
        {
            var userRoles = await _context.UserRoles
                .Where(ur => ur.user_id == userId)
                .Select(ur => ur.Role.name)
                .ToListAsync();

            return userRoles;
        }
    }
}