using CockyShop.Models.Requests;
using FluentValidation;

namespace CockyShop.Validators.Users
{
    public class RegisterUserRequestValidator : AbstractValidator<RegisterUserRequest>
    {
        public RegisterUserRequestValidator()
        {
            RuleFor(lu => lu.Email)
                .Matches(@"^[\w-\.]+@([\w-]+\.)+[\w-]{2,4}$")
                .WithMessage("Email should be like: test@mail.com");
            RuleFor(lu => lu.Password)
                .Matches(@"^(?=.*\d)(?=.*[a-z])(?=.*[A-Z])(?=.*[a-zA-Z]).{8,}$")
                .WithMessage("Password should have at least 8 chars, at least 1 capital letter and 1 digit!");
            
            RuleFor(lu => lu.UserName).MinimumLength(3)
                .WithMessage("User name length should be in range [3-100]!");
            RuleFor(lu => lu.UserName).MaximumLength(100)
                .WithMessage("User name length should be in range [3-100]!");

        }
    }
}