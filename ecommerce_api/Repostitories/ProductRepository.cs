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
        private readonly IMapper _mapper;

        public ProductRepository(EcomerceDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<IEnumerable<ProductDTO>> GetAllProducts()
        {
            var products = await _context.Products
                .Include(p => p.ProductCategory) 
                .Include(p => p.Brand) 
                .ToListAsync();
            return _mapper.Map<IEnumerable<ProductDTO>>(products);
        }

        public async Task<ProductDTO> GetProductById(int id)
        {
            var product = await _context.Products
                .Include(p => p.ProductCategory)
                .Include(p => p.Brand)
                .FirstOrDefaultAsync(p => p.ProductId == id);
            if (product == null)
                throw new KeyNotFoundException("Product not found");
            return _mapper.Map<ProductDTO>(product);
        }
        public async Task<ProductDTO> AddProduct(ProductDTO productDto)
        {
            var product = _mapper.Map<Product>(productDto); 
            await _context.Products.AddAsync(product);
            await _context.SaveChangesAsync();

            return _mapper.Map<ProductDTO>(product);  
        }

        public async Task<ProductDTO> DeleteProduct(int id)
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
            return _mapper.Map<ProductDTO>(product);
        }

        public async Task<ProductDTO> UpdateProduct(int id, ProductDTO productDto)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null)
                throw new KeyNotFoundException("Product not found");

            _mapper.Map(productDto, product);  // Cập nhật giá trị từ DTO sang Product
            _context.Products.Update(product);
            await _context.SaveChangesAsync();

            return _mapper.Map<ProductDTO>(product); 
        }
    }
}
