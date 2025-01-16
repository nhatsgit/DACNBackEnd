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
        public async Task<IEnumerable<Product>> QueryProducts(string? keyword, int? categoryId = null, int? brandId = null, int? shopId = null, decimal? minPrice = null, decimal? maxPrice = null, bool? daAn = null, bool? daHet = null)
        {
            var query = _context.Products
                .Include(p => p.ProductCategory)
                .Include(p => p.Shop)
                .Include(p => p.Brand)
                .AsQueryable();

            // Nếu có từ khóa tìm kiếm, xử lý loại bỏ dấu và so sánh không phân biệt dấu sau khi lấy dữ liệu
            if (!string.IsNullOrEmpty(keyword))
            {
                var normalizedKeyword = keyword.RemoveDiacritics().ToLower(); // Loại bỏ dấu và chuyển sang chữ thường

                // Lấy toàn bộ dữ liệu và lọc sau khi đã loại bỏ dấu
                var products = await query.ToListAsync();  // Lấy tất cả sản phẩm vào bộ nhớ
                products = products.Where(p => p.TenSp.RemoveDiacritics().ToLower().Contains(normalizedKeyword)).ToList();  // Lọc dữ liệu sau khi đã loại bỏ dấu

                // Tiến hành các lọc khác nếu có
                if (categoryId.HasValue)
                {
                    products = products.Where(p => p.ProductCategoryId == categoryId.Value).ToList();
                }
                if (brandId.HasValue)
                {
                    products = products.Where(p => p.BrandId == brandId.Value).ToList();
                }
                if (shopId.HasValue)
                {
                    products = products.Where(p => p.ShopId == shopId.Value).ToList();
                }
                if (minPrice.HasValue)
                {
                    products = products.Where(p => p.GiaBan >= minPrice.Value).ToList();
                }
                if (maxPrice.HasValue)
                {
                    products = products.Where(p => p.GiaBan <= maxPrice.Value).ToList();
                }
                if (daAn.HasValue)
                {
                    products = products.Where(p => p.DaAn == daAn.Value).ToList();
                }
                if (daHet.HasValue)
                {
                    if (daHet.Value)
                    {
                        products = products.Where(p => p.SoLuongCon <= 0).ToList();
                    }
                    else
                    {
                        products = products.Where(p => p.SoLuongCon > 0).ToList();
                    }
                }

                return products.OrderByDescending(p => p.ProductId).ToList();
            }

            // Nếu không có keyword, tiếp tục với các filter còn lại
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
                query = query.Where(p => p.DaAn == daAn.Value);
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

            var productsResult = await query.OrderByDescending(p => p.ProductId).ToListAsync();
            return productsResult;
        }


        public async Task<IEnumerable<Product>> GetRandomProducts(int numberOfProducts)
        {
            var totalProducts = await _context.Products
     .CountAsync(p => p.DaAn != true && p.Shop.BiChan != true);

            var randomSkip = new Random().Next(0, totalProducts - numberOfProducts);

            var products = await _context.Products
                 .Include(p => p.ProductCategory)
                 .Include(p => p.Shop)
                 .Where(p => p.DaAn != true && p.Shop.BiChan != true)
                 .Skip(randomSkip)
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

        public async Task<List<Product>> AddRangeProduct(List<Product> products)
        {
            _context.Products.AddRange(products);
            await _context.SaveChangesAsync();
            return products;
        }

        public async Task<IEnumerable<Product>> GetProductsByIds(List<int>? ids)
        {
            // Kiểm tra danh sách ids có hợp lệ không
            if (ids == null || !ids.Any())
            {
                return Enumerable.Empty<Product>(); // Trả về danh sách rỗng nếu ids không hợp lệ
            }

            try
            {
                var products = await _context.Products
                    .Where(p => ids.Contains(p.ProductId)) // Lọc sản phẩm theo ids
                    .Include(p => p.ProductCategory)
                    .Include(p => p.Shop)
                    .Where(p => p.DaAn != true && p.Shop.BiChan != true)
                    .ToListAsync(); // Lấy dữ liệu từ cơ sở dữ liệu

                // Sắp xếp theo thứ tự của ids (client-side sorting)
                var orderedProducts = products.OrderBy(p => ids.IndexOf(p.ProductId)).ToList();

                return orderedProducts;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                return Enumerable.Empty<Product>(); // Trả về danh sách rỗng nếu có lỗi
            }
        }
    }
}
