using shop.Data;
using shop.Models;
using Microsoft.EntityFrameworkCore;

namespace shop.Services
{
    public class ProdCatService
    {
        private readonly AppDbContext _context;

        public ProdCatService(AppDbContext context)
        {
            _context = context;
        }

        // niekonsekwencja w zwracaniu rzeczy - w AddCategory zwracamy Category a tutaj nic
        public async Task AddProduct(ProductModel model)
        {
            Category category = await _context.Categories.SingleOrDefaultAsync(c => c.name == model.CategoryName);
            if (category == null)
            {
                category = await AddCategory(new CategoryModel { Name = model.CategoryName });
            }
            var product = new Product {
                name = model.Name,
                description = model.Description,
                price = model.Price,
                category_id = category.id
            };
            await _context.Products.AddAsync(product);
            await _context.SaveChangesAsync();
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
                product.category_id = category.id;
            }
            _context.Products.Update(product);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteProduct(int id)
        {
            var product = await _context.Products.SingleOrDefaultAsync(p => p.id == id);
            if (product != null)
            {
                _context.Products.Remove(product);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<Product?> GetProduct(int id)
        {
            return await _context.Products.SingleOrDefaultAsync(p => p.id == id);
        }

        public async Task<Category> AddCategory(CategoryModel model)
        {
            var category = await _context.Categories.SingleOrDefaultAsync(c => c.name == model.Name);
            if (category == null)
            {
                category = new Category { name = model.Name };
                await _context.Categories.AddAsync(category);
                await _context.SaveChangesAsync();
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
                return new List<Category>();
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

        public async Task<Tuple<List<Product>, bool, int>> SearchProductsByQuery(string query)
        {
            bool hasMatches = false;
            int howMany = 0;

            var queryWords = query.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            var products = await _context.Products.ToListAsync();

            var matchedNameProducts = products
                .Where(p => queryWords.Any(qw => p.name.Contains(qw, StringComparison.OrdinalIgnoreCase)))
                .ToList();

            howMany = matchedNameProducts.Count;;
            if (howMany > 0)
                hasMatches = true;

            var kindaMatchedNameProducts = products
                .Where(p => !matchedNameProducts.Contains(p))
                .Select(p => new
                {
                    Product = p,
                    Score = queryWords.Sum(qw => 
                        p.name.Split(' ', StringSplitOptions.RemoveEmptyEntries).Min(pw => LevenshteinDistance(pw, qw))
                    )
                })
                .OrderBy(sp => sp.Score)
                .Select(sp => sp.Product)
                .Take(10) // limit to 10
                .ToList();

            var resultProducts = matchedNameProducts.Concat(kindaMatchedNameProducts).ToList();
            return new Tuple<List<Product>, bool, int>(resultProducts, hasMatches, howMany);
        }

        private int LevenshteinDistance(string a, string b)
        {
            if (string.IsNullOrEmpty(a)) return string.IsNullOrEmpty(b) ? 0 : b.Length;
            if (string.IsNullOrEmpty(b)) return a.Length;

            var costs = new int[b.Length + 1];

            for (int j = 0; j < costs.Length; j++)
                costs[j] = j;

            for (int i = 1; i <= a.Length; i++)
            {
                costs[0] = i;
                int nw = i - 1;
                for (int j = 1; j <= b.Length; j++)
                {
                    int cj = Math.Min(1 + Math.Min(costs[j], costs[j - 1]), a[i - 1] == b[j - 1] ? nw : nw + 1);
                    nw = costs[j];
                    costs[j] = cj;
                }
            }
            return costs[b.Length];
        }
    }
}
