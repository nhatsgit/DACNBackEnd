using AutoMapper;
using ecommerce_api.DTO;
using ecommerce_api.Models;
using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore;

namespace ecommerce_api.Repostitories
{
    public class ProductRepository:IProductRepository
    {
        private readonly EcomerceDbContext _context;

        public ProductRepository(EcomerceDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Product>> GetAllProducts()
        {
            var products = await _context.Products
                .Include(p => p.ProductCategory) 
                .Include(p => p.Brand) 
                .ToListAsync();
            return products;
        }

        public async Task<Product> GetProductById(int id)
        {
            var product = await _context.Products
                .Include(p => p.ProductCategory)
                .Include(p => p.Brand)
                .FirstOrDefaultAsync(p => p.ProductId == id);
            if (product == null)
                throw new KeyNotFoundException("Product not found");
            return product;
        }
        public async Task<Product> AddProduct(Product product)
        {
            
            await _context.Products.AddAsync(product);
            await _context.SaveChangesAsync();

            return product;  
        }

        public async Task<Product> DeleteProduct(int id)
        {
            var product = await _context.Products
                .Include(p => p.ProductCategory)
                .Include(p => p.Brand)
                .FirstOrDefaultAsync(p => p.ProductId == id);
            if (product == null)
                throw new KeyNotFoundException("Product not found");
            product.DaAn = true;
            _context.Products.Update(product);
            await _context.SaveChangesAsync();
            return product;
        }

        public async Task<Product> UpdateProduct(int id, Product product)
        {
              
            _context.Products.Update(product);
            await _context.SaveChangesAsync();
            return product; 
        }
        public async Task<IEnumerable<Product>> QueryProducts(string? keyword, int? categoryId=null,int? brandId=null,int? shopId = null, decimal? minPrice=null, decimal? maxPrice=null)
        {
            var query = _context.Products
                .Include(p => p.ProductCategory)
                .Include(p => p.Shop)
                .Include(p => p.Brand)
                .Where(p => p.DaAn == false) 
                .AsQueryable();

  
            if (!string.IsNullOrEmpty(keyword))
            {
                query = query.Where(p => p.TenSp.Contains(keyword));
            }
            if (categoryId.HasValue)
            {
                query = query.Where(p => p.ProductCategoryId == categoryId.Value);
            }
            if (brandId.HasValue)
            {
                query = query.Where(p => p.BrandId == brandId.Value);
            }
            if (shopId.HasValue)
            {
                query = query.Where(p => p.ShopId == shopId.Value);
            }
            if (minPrice.HasValue)
            {
                query = query.Where(p => p.GiaBan >= minPrice.Value);
            }
            if (maxPrice.HasValue)
            {
                query = query.Where(p => p.GiaBan <= maxPrice.Value);
            }

           
            var products = await query.OrderBy(p=>p.ProductId).ToListAsync();
            return products;
        }
        public async Task<IEnumerable<Product>> GetRandomProducts(int numberOfProducts)
        {
            var products = await _context.Products
                .Include(p => p.ProductCategory)
                .Where(p => p.DaAn == false) 
                .OrderBy(p => Guid.NewGuid())
                .Take(numberOfProducts) 
                .ToListAsync();

            return products;
        }

        public async Task<List<string>> GetSearchSuggestions(string keyword)
        {
            var products=await QueryProducts(keyword:keyword);
            var suggestions = products.Take(5).Select(p => p.TenSp);
            return suggestions.ToList();
        }
    }
}
