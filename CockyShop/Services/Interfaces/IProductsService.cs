using System.Collections.Generic;
using System.Threading.Tasks;
using CockyShop.Models.App;
using CockyShop.Models.DTO;

namespace CockyShop.Services.Interfaces
{
    public interface IProductsService
    {
        Task<List<ProductInStockDto>> GetAllProductsInCity(int cityId);

        Task<ProductInStockDto> GetProductInCityById(int cityId, int productId);

        Task DeleteProductInCityById(int cityId, int productId);
        
        Task<List<ProductDto>> GetAllProducts();

        Task DeleteProductById(int id);

        Task<ProductDto> GetProductById(int id);


    }
}