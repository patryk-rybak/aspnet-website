
using System.ComponentModel.DataAnnotations;
namespace shop.Models;
public class CategoryModel
{
    [Required]
    public string Name { get; set; }
}