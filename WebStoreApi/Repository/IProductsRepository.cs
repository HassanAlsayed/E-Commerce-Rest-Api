using WebStoreApi.DTO;
using WebStoreApi.Models;

namespace WebStoreApi.Repository
{
    public interface IProductsRepository
    {
        public Task<IEnumerable<Product>> GetAllProduct(int? page, string? search);
        public Task<Product> GetProductById(Guid id);
        public Task<Product> CreateProduct(ProductDTO product);
        public Task<Product> UpdateProduct(ProductDTO product,Guid id);
        public Task<Product> DeleteProduct(Guid id);
    }
}
