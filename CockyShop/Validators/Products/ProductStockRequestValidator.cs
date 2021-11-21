using CockyShop.Models.Requests;
using FluentValidation;

namespace CockyShop.Validators
{
    public class ProductStockRequestValidator : AbstractValidator<ProductStockRequest>
    {
        public ProductStockRequestValidator()
        {
            RuleFor(psr => psr.Price).GreaterThan(0).WithMessage("Price cannot be negative!");
            
            RuleFor(psr => psr.CityId).GreaterThan(0).WithMessage("City ID should greater than 0!");
            
            RuleFor(psr => psr.ProductId).GreaterThan(0).WithMessage("Product ID should greater than 0!");
            
            RuleFor(psr => psr.QtyOnStock).GreaterThanOrEqualTo(0).WithMessage("Product quantity cannot be negative!");
        }
    }
}