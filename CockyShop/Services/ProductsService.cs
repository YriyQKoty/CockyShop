using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CockyShop.Exceptions;
using CockyShop.Infrastucture;
using CockyShop.Models.DTO;
using CockyShop.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CockyShop.Services
{
    public class ProductsService : IProductsService
    {
        private AppDbContext _appDbContext;

        public ProductsService(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }

        public async Task<List<ProductInStockDto>> GetAllProductsInCity(int cityId)
        {
            await ValidateCity(cityId);

            var productsDtos = await _appDbContext.ProductStocks
                .Where(ps => ps.Stock.City.Id == cityId)
                .Select(p => new ProductInStockDto()
                {
                    Id = p.ProductId,
                    Name = p.Product.Name,
                    Price = p.Price,
                    Available = p.QtyOnStock > 0
                }).ToListAsync();

            return productsDtos;
        }

        public async Task<ProductInStockDto> GetProductInCityById(int cityId, int productId)
        {
            await ValidateCity(cityId);

            var productDto = await _appDbContext.ProductStocks
                .Where(ps => ps.Stock.City.Id == cityId && ps.ProductId == productId)
                .Select(p => new ProductInStockDto()
                {
                    Id = p.ProductId,
                    Name = p.Product.Name,
                    Price = p.Price,
                    Available = p.QtyOnStock > 0
                }).FirstOrDefaultAsync();

            if (productDto == null)
            {
                throw new EntityNotFoundException($"Product with {productId} not found in City with {cityId}!");
            }

            return productDto;
        }

        public async Task DeleteProductInCityById(int cityId, int productId)
        {
            await ValidateCity(cityId);

            var productStock = await _appDbContext.ProductStocks
                .Where(ps => ps.Stock.City.Id == cityId && ps.ProductId == productId)
                .FirstOrDefaultAsync();

            if (productStock == null)
            {
                throw new EntityNotFoundException($"Product with {productId} not found in City with {cityId}!");
            }

            _appDbContext.ProductStocks.Remove(productStock);
            await _appDbContext.SaveChangesAsync();
        }

        public async Task<List<ProductDto>> GetAllProducts()
        {
            var productDtos = await _appDbContext.Products.Select(p => new ProductDto()
            {
                Id = p.Id,
                Name = p.Name
            }).ToListAsync();
            return productDtos;
        }
        
        public async Task<ProductDto> GetProductById(int id)
        {
            var productDto = await _appDbContext.Products.Where(p => p.Id == id)
                .Select(p => new ProductDto()
                {
                    Id = p.Id,
                    Name = p.Name
                }).FirstOrDefaultAsync();

            if (productDto == null)
            {
                throw new EntityNotFoundException($"Product with {id} not found!");
            }

            return productDto;
        }

        public async Task DeleteProductById(int id)
        {
            var product = await _appDbContext.Products
                .Where(p => p.Id == id)
                .FirstOrDefaultAsync();

            if (product == null)
            {
                throw new EntityNotFoundException($"Product with {id} not found!");
            }

            _appDbContext.Products.Remove(product);
            await _appDbContext.SaveChangesAsync();
        }

       

        private async Task ValidateCity(int cityId)
        {
            var city = await _appDbContext.Cities.FindAsync(cityId);

            if (city == null)
            {
                throw new DomainException($"City with {cityId} does not exist!");
            }
        }
    }
}