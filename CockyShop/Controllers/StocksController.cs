using System.Collections.Generic;
using System.Threading.Tasks;
using CockyShop.Models.DTO;
using CockyShop.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace CockyShop.Controllers
{
    public class StocksController : AppBaseController
    {
        private IProductsService _productsService;

        public StocksController(IProductsService productsService)
        {
            _productsService = productsService;
        }

        [HttpGet("products")]
        public async Task<ActionResult<List<ProductInStockDto>>> GetAllProductsInCity([FromQuery] int cityId)
        {
            return Ok(await _productsService.GetAllProductsInCity(cityId));
        }

        [HttpGet("products/{productId}")]
        public async Task<ActionResult<ProductInStockDto>> GetProductInCityById([FromQuery] int cityId,
            [FromRoute] int productId)
        {
            return Ok(await _productsService.GetProductInCityById(cityId, productId));
        }
        
        [HttpDelete("products/{productId}")]
        public async Task<ActionResult> DeleteProductInCityById([FromQuery] int cityId, [FromRoute] int productId)
        {
            await _productsService.DeleteProductInCityById(cityId, productId);
            return NoContent();
        }
        
    }
}