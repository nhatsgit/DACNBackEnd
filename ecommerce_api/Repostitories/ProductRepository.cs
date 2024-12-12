using AutoMapper;
using ecommerce_api.DTO;
using ecommerce_api.Helper;
using ecommerce_api.Models;
using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore;

namespace ecommerce_api.Repostitories
{
    public class ProductRepository : IProductRepository
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
                .Include (p => p.ProductImages)
                .Include (p => p.Reviews)
                .Include(p=>p.Shop)
                .FirstOrDefaultAsync(p => p.ProductId == id);
            if (product == null)
                throw new KeyNotFoundException("Product not found");
            return product;
        }
        public async Task<Product> AddProduct(Product product, List<IFormFile> listImages)
        {
            
            await _context.Products.AddAsync(product);
            await _context.SaveChangesAsync();
            if (listImages != null)
            {
                foreach (var file in listImages)
                {
                    if (file.Length > 0)
                    {
                        using (var memoryStream = new MemoryStream())
                        {
                            await file.CopyToAsync(memoryStream);
                            var image = new ProductImage
                            {
                                Url = await UploadImage.SaveImage(file),
                                ProductId = product.ProductId
                            };
                            _context.ProductImages.Add(image);
                            await _context.SaveChangesAsync();
                        }
                    }
                }
            }

            return product;  
        }

        public async Task<Product> SwapHideShowProduct(int id)
        {
            var product = await _context.Products
                .Include(p => p.ProductCategory)
                .Include(p => p.Brand)
                .FirstOrDefaultAsync(p => p.ProductId == id);
            if (product == null)
                throw new KeyNotFoundException("Product not found");
            product.DaAn = !product.DaAn;
            _context.Products.Update(product);
            await _context.SaveChangesAsync();
            return product;
        }

        public async Task<Product> UpdateProduct(int id, Product product, List<IFormFile> listImages)
        {
            if (listImages.Count > 0)
            {
                var imagesToDelete = _context.ProductImages.Where(p => p.ProductId == product.ProductId);

                if (imagesToDelete.Any())
                {
                    _context.ProductImages.RemoveRange(imagesToDelete);
                    await _context.SaveChangesAsync();
                }
                foreach (var file in listImages)
                {
                    if (file.Length > 0)
                    {
                        using (var memoryStream = new MemoryStream())
                        {
                            await file.CopyToAsync(memoryStream);
                            var image = new ProductImage
                            {
                                Url = await UploadImage.SaveImage(file),
                                ProductId = product.ProductId
                            };
                            _context.ProductImages.Add(image);
                            await _context.SaveChangesAsync();
                        }
                    }
                }
            }
            _context.Products.Update(product);
            await _context.SaveChangesAsync();
            return product; 
        }
        public async Task<IEnumerable<Product>> QueryProducts(string? keyword, int? categoryId=null,int? brandId=null,int? shopId = null, decimal? minPrice=null, decimal? maxPrice=null, bool? daAn=null, bool? daHet=null)
        {
            var query = _context.Products
                .Include(p => p.ProductCategory)
                .Include(p => p.Shop)
                .Include(p => p.Brand)
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
            if (daAn.HasValue)
            {
                query = query.Where(p => p.DaAn==daAn.Value);
            }
            if (daHet.HasValue)
            {
                if (daHet.Value)
                {
                    query = query.Where(p => p.SoLuongCon <= 0);
                }
                else
                {
                    query = query.Where(p => p.SoLuongCon > 0);

                }
            }

            var products = await query.OrderByDescending(p=>p.ProductId).ToListAsync();
            return products;
        }
        public async Task<IEnumerable<Product>> GetRandomProducts(int numberOfProducts)
        {
            var products = await _context.Products
                .Include(p => p.ProductCategory)
                .Include(p=>p.Shop)
                .Where(p => p.DaAn != true&&p.Shop.BiChan!=true) 
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

        public async Task<IEnumerable<ProductImage>> GetProductImagesById(int id)
        {
            var productImages=await _context.ProductImages.Where(pi=>pi.ProductId == id).ToListAsync();
            return productImages;
        }

        public async Task<IEnumerable<Category>> GetCategoryFromQuerry(string? keyword, int? brandId, int? shopId, decimal? minPrice, decimal? maxPrice, bool? daAn, bool? daHet)
        {
            var products = await QueryProducts(keyword,null, brandId, shopId, minPrice, maxPrice,daAn,daHet);
            var categoryIds=products.Select(p => p.ProductCategoryId).Distinct().ToList();
            var categories = await _context.Categories
            .Where(c => categoryIds.Contains(c.ProductCategoryId)) 
            .ToListAsync(); 
            return categories;
        }
    }
}
