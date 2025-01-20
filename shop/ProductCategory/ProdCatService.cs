using shop.Data;
using shop.Models;
using Microsoft.EntityFrameworkCore;

namespace shop.ProductCategory
{
    public class ProdCatService
    {
        private readonly AppDbContext _context;

        public ProdCatService(AppDbContext context)
        {
            _context = context;
        }

        public async Task AddProduct(ProductModel model)
        {
            Category category = await _context.Categories.SingleOrDefaultAsync(c => c.name == model.CategoryName);
            if (category == null)
            {
                Console.WriteLine("Dodaje kategorie");
                category = await AddCategory(new CategoryModel { Name = model.CategoryName });
                // category = await _context.Categories.SingleOrDefaultAsync(c => c.name == model.CategoryName);
            }
            var product = new Product {
                name = model.Name,
                description = model.Description,
                price = model.Price,
                category_id = category.id // ciekawe czy sie samo wypelni xd
            };
            Console.WriteLine("Dodaje produkt");
            await _context.Products.AddAsync(product);
            await _context.SaveChangesAsync();
            Console.WriteLine("Dodano produkt");

            // a moze zwracac nowodoadany produkt?
        }

        public async Task ModifyProduct(ProductModel model)
        {
            var product = await _context.Products.SingleOrDefaultAsync(p => p.id == model.Id);
            if (product != null) 
            {
                Category category = await _context.Categories.SingleOrDefaultAsync(c => c.name == model.CategoryName);
                if (category == null)
                {
                    category = await AddCategory(new CategoryModel { Name = model.CategoryName });
                }
                product.name = model.Name;
                product.description = model.Description;
                product.price = model.Price;
                product.category_id = category.id;// cikeawe czy sie samo wypelnilo
            }
            _context.Products.Update(product);
            await _context.SaveChangesAsync();
        }

        public async Task<Category> AddCategory(CategoryModel model)
        {
            Console.WriteLine("in");
            var category = await _context.Categories.SingleOrDefaultAsync(c => c.name == model.Name);
            Console.WriteLine("pocz");
            Console.WriteLine(category);
            Console.WriteLine("kon");
            if (category == null)
            {
                Console.WriteLine("in");
                category = new Category { name = model.Name };
                await _context.Categories.AddAsync(category);
                await _context.SaveChangesAsync();
                Console.WriteLine("in");
                return await _context.Categories.SingleOrDefaultAsync(c => c.name == model.Name);
            }
            else
            {
                throw new Exception("Kategoria o podanej nazwie juz istnieje.");
            }
            // nwm czy zracan nowa kateegorie
        }

        public async Task<List<Category>> GetCategories()
        {
            List<Category> categories = await _context.Categories.ToListAsync();
            if (categories == null || categories.Count == 0)
            {
                throw new Exception("Brak kategorii w bazie danych.");
            }
            return categories;
        }

        public async Task<List<Product>> GetFrontProducts(int N = 2)
        {
            List<Product> products = await _context.Products.Take(N).ToListAsync();
            if (products == null || products.Count == 0)
            {
                return new List<Product>();
            }
            return products;
        }

        public async Task<List<Product>> GetProductsByCategory(int categoryId)
        {
            List<Product> products = await _context.Products.Where(p => p.category_id == categoryId).ToListAsync();
            if (products == null || products.Count == 0)
            {
                return new List<Product>();
            }
            return products;
        }

        public async Task<List<Product>> SearchProductsByQuery(string query)
        {
            // List<Product> products = await _context.Products.Where(p => p.name.Contains(query)).ToListAsync();
            // if (products == null || products.Count == 0)
            // {
            //     return new List<Product>();
            // }
            
            return new List<Product>();
            // return products;
        }
    }
}
