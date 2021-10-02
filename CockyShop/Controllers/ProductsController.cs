using System.Collections.Generic;
using System.Threading.Tasks;
using CockyShop.Models.DTO;
using CockyShop.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace CockyShop.Controllers
{
    public class ProductsController : AppBaseController
    {
        private IProductsService _productsService;

        public ProductsController(IProductsService productsService)
        {
            _productsService = productsService;
        }
        
        [HttpGet]
        public async Task<ActionResult<List<ProductDto>>> GetAllProducts()
        {
            return Ok(await _productsService.GetAllProducts());
        }
        
        [HttpGet("{id}")]
        public async Task<ActionResult<List<ProductDto>>> GetProductById([FromRoute]int id)
        {
            return Ok(await _productsService.GetProductById(id));
        }
        
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteProductById([FromRoute]int id)
        {
            await _productsService.DeleteProductById(id);
            return NoContent();
        }
        
   
    }
}