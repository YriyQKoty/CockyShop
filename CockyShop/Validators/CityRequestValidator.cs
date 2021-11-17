using CockyShop.Models.Requests;
using FluentValidation;

namespace CockyShop.Validators
{
    public class CityRequestValidator : AbstractValidator<CityRequest>
    {
        public CityRequestValidator()
        {
            RuleFor(c => c.Name).MinimumLength(3).WithMessage("City length is too short. Should be in range [3-100]!");
            RuleFor(c => c.Name).MaximumLength(100).WithMessage("City length is too long. Should be in range [3-100]!");
        }
    }
}