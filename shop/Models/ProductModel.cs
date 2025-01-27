using System.ComponentModel.DataAnnotations;
using shop.Data;

namespace shop.Models;
public class ProductModel
{
    // [Required]
    public int? Id { get; set; } = null;
    [Required]
    public string Name { get; set; }
    [Required]
    public string Description { get; set; }
    [Required]
    [Range(0.00, 1000000.00)]
    public decimal Price { get; set; }
    [Required] [CategoryExistsAttribute]
    public string CategoryName { get; set; }
}

public class CategoryExistsAttribute : ValidationAttribute
{
    protected override ValidationResult IsValid(object value, ValidationContext validationContext)
    {
        var context = (AppDbContext)validationContext.GetService(typeof(AppDbContext));
        var categoryName = value as string;
        if (context.Categories.Any(c => c.name == categoryName))
        {
            return ValidationResult.Success;
        }
        return new ValidationResult("Category does not exist.");
    }
}