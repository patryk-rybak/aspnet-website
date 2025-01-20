using Microsoft.AspNetCore.StaticAssets;

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

        public static async Task CreateProducts(AppDbContext context)
        {
            if (!context.Products.Any())
            {
                //..
            }
        }
    }
}