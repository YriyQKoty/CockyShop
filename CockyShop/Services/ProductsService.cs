using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CockyShop.Exceptions;
using CockyShop.Infrastucture;
using CockyShop.Models.App;
using CockyShop.Models.DTO;
using CockyShop.Models.Requests;
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
                    Id = p.Id,
                    GeneralProductId = p.ProductId,
                    Name = p.Product.Name,
                    Price = p.Price,
                    CityId = p.Stock.City.Id,
                    CityName = p.Stock.City.Name,
                    QtyOnStock = (int)p.QtyOnStock,
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
                    Id = p.Id,
                    GeneralProductId = p.ProductId,
                    Name = p.Product.Name,
                    Price = p.Price,
                    CityId = p.Stock.City.Id,
                    CityName = p.Stock.City.Name,
                    QtyOnStock = (int)p.QtyOnStock,
                    Available = p.QtyOnStock > 0
                }).FirstOrDefaultAsync();

            if (productDto == null)
            {
                throw new EntityNotFoundException($"Product with {productId} not found in City with {cityId}!");
            }

            return productDto;
        }

        public async Task<ProductInStockDto> UpdateProductInCity(ProductStockRequest request)
        {
            await ValidateCity(request.CityId);
            
            var productStock = await _appDbContext.ProductStocks
                .Where(ps => ps.Stock.City.Id == request.CityId && ps.ProductId == request.ProductId)
                .Include(p => p.Product)
                .Include(p => p.Stock.City)
                .FirstOrDefaultAsync();

            if (productStock == null)
            {
                throw new EntityNotFoundException($"Product with id: {request.ProductId} not found in City with id: {request.CityId}!");
            }

            productStock.Price = request.Price;
            productStock.QtyOnStock = request.QtyOnStock;
            _appDbContext.ProductStocks.Update(productStock);
            await _appDbContext.SaveChangesAsync();

            return new ProductInStockDto()
            {
                Id = productStock.Id,
                GeneralProductId = productStock.Id,
                Price = productStock.Price,
                Name = productStock.Product.Name,
                CityId = productStock.Stock.City.Id,
                CityName = productStock.Stock.City.Name,
                QtyOnStock = (int)productStock.QtyOnStock,
                Available = productStock.QtyOnStock > 0
            };

        }

        public async Task<ProductInStockDto> CreateProductInCity(ProductStockRequest request)
        {
            await ValidateCity(request.CityId);
            await ValidateProduct(request.ProductId);

            var product = new ProductStock()
            {
                ProductId = request.ProductId,
                Product = await _appDbContext.Products.FindAsync(request.ProductId),
                Price = request.Price,
                QtyOnStock = request.QtyOnStock,
                Stock = await _appDbContext.Stocks.Where(s => s.City.Id == request.CityId).FirstOrDefaultAsync(),
            };

            await _appDbContext.ProductStocks.AddAsync(product);
            await _appDbContext.SaveChangesAsync();

            return new ProductInStockDto()
            {
                GeneralProductId = product.Id,
                Price = product.Price,
                CityId = product.Stock.City.Id,
                Name = product.Product.Name,
                CityName = product.Stock.City.Name,
                Available = product.QtyOnStock > 0,
                QtyOnStock = (int)product.QtyOnStock
            };
        }


        public async Task DeleteProductInCityById(int cityId, int productId)
        {
            await ValidateCity(cityId);

            var productStock = await _appDbContext.ProductStocks
                .Where(ps => ps.Stock.City.Id == cityId && ps.ProductId == productId)
                .FirstOrDefaultAsync();

            if (productStock == null)
            {
                throw new EntityNotFoundException($"Product with id: {productId} not found in City with id: {cityId}!");
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
                throw new EntityNotFoundException($"Product with id: {id} not found!");
            }

            return productDto;
        }

        public async Task<ProductDto> CreateProduct(ProductRequest request)
        {
            Product product = new Product() {Name = request.Name};

            var productCreated = (await _appDbContext.Products.AddAsync(product)).Entity;
            await _appDbContext.SaveChangesAsync();

            return new ProductDto()
            {
                Id = productCreated.Id,
                Name = productCreated.Name
            };
        }

        public async Task<ProductDto> UpdateProductById(int id, ProductRequest request)
        {
            var productDto = await _appDbContext.Products
                .Where(p => p.Id == id).FirstOrDefaultAsync();

            if (productDto == null)
            {
                throw new EntityNotFoundException($"Product with id: {id} not found!");
            }

            productDto.Name = request.Name;
            _appDbContext.Products.Update(productDto);
            await _appDbContext.SaveChangesAsync();

            return new ProductDto() {Name = productDto.Name};
        }

        public async Task DeleteProductById(int id)
        {
            var product = await _appDbContext.Products
                .Where(p => p.Id == id)
                .FirstOrDefaultAsync();

            if (product == null)
            {
                throw new EntityNotFoundException($"Product with id: {id} not found!");
            }

            _appDbContext.Products.Remove(product);
            await _appDbContext.SaveChangesAsync();
        }

       

        private async Task ValidateCity(int cityId)
        {
            var city = await _appDbContext.Cities.FindAsync(cityId);

            if (city == null)
            {
                throw new DomainException($"City with id: {cityId} does not exist!");
            }
        }
        
        private async Task ValidateProduct(int productId)
        {
            var generalProduct = await _appDbContext.Products.FindAsync(productId);
            if (generalProduct == null)
            {
                throw new DomainException($"Product with such id: {productId} does not exist!");
            }
            
            var productInStock = await _appDbContext.ProductStocks.Where(ps => ps.ProductId == productId).FirstOrDefaultAsync();
            
            if (productInStock != null)
            {
                throw new DomainException($"Product with such id: {productId} already exists in this stock!");
            }
        }
    }
}