using CockyShop.Models.Requests;
using FluentValidation;

namespace CockyShop.Validators
{
    public class OrderStatusRequestValidator : AbstractValidator<OrderStatusRequest>
    {
        public OrderStatusRequestValidator()
        {
            RuleFor(os => os.Name).MaximumLength(100).WithMessage("Status name length should be in range [3-100]!");
            RuleFor(os => os.Name).MinimumLength(3).WithMessage("Status name length should be in range [3-100]!");
        }
    }
}