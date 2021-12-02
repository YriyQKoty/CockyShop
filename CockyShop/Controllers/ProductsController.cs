using System.Collections.Generic;
using System.Threading.Tasks;
using CockyShop.Models.DTO;
using CockyShop.Models.Requests;
using CockyShop.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CockyShop.Controllers
{
    [Authorize]
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
        
      
        [HttpPost]
        [Authorize(Policy = "RequireAdministratorRole")]
        public async Task<ActionResult<ProductDto>> CreateProduct([FromBody] ProductRequest request)
        {
            return Ok(await _productsService.CreateProduct(request));
        }

        [HttpPut("{id}")]
        [Authorize(Policy = "RequireAdministratorRole")]
        public async Task<ActionResult<ProductDto>> UpdateProductById([FromRoute] int id,
            [FromBody] ProductRequest request)
        {
            return Ok(await _productsService.UpdateProductById(id, request));
        }
        
        [HttpDelete("{id}")]
        [Authorize(Policy = "RequireAdministratorRole")]
        public async Task<ActionResult> DeleteProductById([FromRoute]int id)
        {
            await _productsService.DeleteProductById(id);
            return NoContent();
        }
        
   
    }
}