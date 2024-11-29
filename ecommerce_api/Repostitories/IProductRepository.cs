using ecommerce_api.DTO;
using ecommerce_api.Models;

namespace ecommerce_api.Repostitories
{
    public interface IProductRepository
    {
         Task<IEnumerable<Product>> GetAllProducts();
         Task<IEnumerable<Product>> QueryProducts(string? keyword, int? categoryId,int? brandId,int? shopId, decimal? minPrice, decimal? maxPrice, bool? daAn, bool? daHet);
         Task<IEnumerable<Product>> GetRandomProducts(int numberOfProducts);
         Task<IEnumerable<Category>> GetCategoryFromQuerry(string? keyword, int? brandId, int? shopId, decimal? minPrice, decimal? maxPrice, bool? daAn, bool? daHet);
         Task<Product> GetProductById(int id);
         Task<IEnumerable<ProductImage>> GetProductImagesById(int id);
         Task<Product> AddProduct(Product product, List<IFormFile> listImages);
         Task<Product> DeleteProduct(int id);
         Task<Product> UpdateProduct(int id,Product product);
         Task<List<string>> GetSearchSuggestions(string keyword);

    }
}
