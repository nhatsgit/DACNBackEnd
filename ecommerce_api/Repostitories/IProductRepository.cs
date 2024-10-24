using ecommerce_api.DTO;

namespace ecommerce_api.Repostitories
{
    public interface IProductRepository
    {
        public Task<IEnumerable<ProductDTO>> GetAllProducts();
        public Task<ProductDTO> GetProductById(int id);
        public Task<ProductDTO> AddProduct(ProductDTO productDto);
        public Task<ProductDTO> DeleteProduct(int id);
        public Task<ProductDTO> UpdateProduct(int id,ProductDTO productDto);

    }
}
