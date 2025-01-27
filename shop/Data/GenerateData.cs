using Microsoft.AspNetCore.StaticAssets;
using Microsoft.EntityFrameworkCore;
using shop.Models;
using System.ComponentModel.DataAnnotations;
using shop.Services;

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
            System.Console.WriteLine("CreateRoles done.");
        }

        public static async Task CreateAdmin(AppDbContext context, AuthService authService)
        {
            var adminRole = await context.Roles.FirstOrDefaultAsync(r => r.name == "admin");
            if (adminRole == null)
            {
            throw new Exception("Admin role does not exist.");
            }

            var adminRoleId = adminRole.id;
            if (!context.UserRoles.Any(ur => ur.role_id == adminRoleId))
            {
                RegisterModel model = new RegisterModel
                {
                    Name = "admin",
                    Email = "admin@admin.com",
                    Password = "admin"
                };

                // Validate the model
                var validationResults = new List<ValidationResult>();
                var validationContext = new ValidationContext(model, null, null);

                if (!Validator.TryValidateObject(model, validationContext, validationResults, true))
                {
                    throw new ValidationException("RegisterModel is not valid.");
                }

                await authService.Register(model, true); // is_admin = true
            }
            System.Console.WriteLine("CreatAdmin done.");
        }

        public static async Task CreateCategories(AppDbContext context, ProdCatService prodCatService)
        {
            if(!context.Categories.Any())
            {
                var category1 = new CategoryModel { Name = "Category 1" };
                var category2 = new CategoryModel { Name = "Category 2" };

                // Validate the model
                var validationResults = new List<ValidationResult>();
                var validationContext = new ValidationContext(category1, null, null);
                if (!Validator.TryValidateObject(category1, validationContext, validationResults, true))
                    throw new ValidationException("Category model is not valid.");
                validationContext = new ValidationContext(category2, null, null);
                if (!Validator.TryValidateObject(category2, validationContext, validationResults, true))
                    throw new ValidationException("Category model is not valid.");
                
                await prodCatService.AddCategory(category1);
                await prodCatService.AddCategory(category2);
            }
            System.Console.WriteLine("CreateCategories done.");
        }

        public static async Task CreateProducts(AppDbContext context, ProdCatService prodCatService)
        {
            if(context.Categories.Any() && !context.Products.Any())
            {
                var category = (await prodCatService.GetCategories()).FirstOrDefault();
                for (int i = 0; i < 10; i++)
                {
                    var product = new ProductModel
                    {
                        Name = $"Product {i}",
                        Description = $"Description {i}",
                        Price = (decimal)4217.99,
                        CategoryName = category.name
                    };

                    // jak mam zwalidowac model?
                    // oprzedni sposob generuje blad

                    await prodCatService.AddProduct(product);
                }
            }
            System.Console.WriteLine("CreateProducts done.");
        }
    }
}