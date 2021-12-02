using CockyShop.Models.DTO;
using CockyShop.Models.Requests;
using FluentValidation;

namespace CockyShop.Validators.Users
{
    public class UserRoleRequestValidator : AbstractValidator<UserRoleRequest>
    {
        public UserRoleRequestValidator ()
        {
            RuleFor(lu => lu.Email)
                .Matches(@"^[\w-\.]+@([\w-]+\.)+[\w-]{2,4}$")
                .WithMessage("Email should be like: test@mail.com");
        }
    }
}