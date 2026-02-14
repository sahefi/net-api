using net_api.DTOs;

namespace net_api.Services
{
    public interface IProductService
    {
        Task<List<ProductResponse>> GetAllProductsAsync();
        Task<ProductResponse?> GetProductByIdAsync(int id);
        Task<ProductResponse> CreateProductAsync(ProductRequest request);
        Task<ProductResponse?> UpdateProductAsync(int id, ProductRequest request);
        Task<bool> DeleteProductAsync(int id);
    }
}
