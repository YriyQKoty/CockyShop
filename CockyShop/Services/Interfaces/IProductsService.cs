using System.Collections.Generic;
using System.Threading.Tasks;
using CockyShop.Models.App;
using CockyShop.Models.DTO;
using CockyShop.Models.Requests;

namespace CockyShop.Services.Interfaces
{
    public interface IProductsService
    {
        Task<List<ProductInStockDto>> GetAllProductsInCity(int cityId);

        Task<ProductInStockDto> GetProductInCityById(int cityId, int productId);

        Task<ProductInStockDto> UpdateProductInCity(int cityId, int productId, ProductStockRequest request);

        Task DeleteProductInCityById(int cityId, int productId);
        
        Task<List<ProductDto>> GetAllProducts();
        
        Task<ProductDto> GetProductById(int id);

        Task<ProductDto> CreateProduct(ProductRequest request);

        Task<ProductDto> UpdateProductById(int id, ProductRequest request);

        Task DeleteProductById(int id);



    }
}