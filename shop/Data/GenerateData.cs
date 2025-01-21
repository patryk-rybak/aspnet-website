using Microsoft.AspNetCore.StaticAssets;
using Microsoft.EntityFrameworkCore;

namespace shop.Data
{
    public class GenerateData
    {
        public static async Task CreateRoles(AppDbContext context)
        {
            if(!context.Roles.Any())
            {
                var adminRole = new Role { name = "admin" };
                var userRole = new Role { name = "user" };
                
                await context.Roles.AddRangeAsync(adminRole, userRole);
                await context.SaveChangesAsync();
            }
        }

        public static async Task CreateAdmin(AppDbContext context)
        {
            // ciekawe co jak nie bedize roli
            var admin_role_id = context.Roles.FirstOrDefaultAsync(r => r.name == "admin").Result.id;
            if (!context.UserRoles.Select(ur => ur.role_id == ).Any())
            {

            }
        }
    }
}