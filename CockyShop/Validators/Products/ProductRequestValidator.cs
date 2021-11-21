using CockyShop.Models.Requests;
using FluentValidation;

namespace CockyShop.Validators
{
    public class ProductRequestValidator : AbstractValidator<ProductRequest>
    {
        public ProductRequestValidator()
        {
            RuleFor(p => p.Name).MinimumLength(3).WithMessage("Name is too short. Should be in range [3-255] characters!");
            RuleFor(p => p.Name).MaximumLength(255).WithMessage("Name is too long. Should be in range [3-255] characters!");
        }
    }
}