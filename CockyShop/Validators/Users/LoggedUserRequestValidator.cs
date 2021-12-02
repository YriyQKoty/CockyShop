using CockyShop.Models.Requests;
using FluentValidation;

namespace CockyShop.Validators.Users
{
    public class LoggedUserRequestValidator : AbstractValidator<LoggedUserRequest>
    {
        public LoggedUserRequestValidator()
        {
            RuleFor(lu => lu.Email)
                .Matches(@"^[\w-\.]+@([\w-]+\.)+[\w-]{2,4}$")
                .WithMessage("Email should be like: test@mail.com");
            RuleFor(lu => lu.Password)
                .Matches(@"^(?=.*\d)(?=.*[a-z])(?=.*[A-Z])(?=.*[a-zA-Z]).{8,}$")
                .WithMessage("Password should have at least 8 chars, at least 1 capital letter, 1 small letter and 1 digit!");
        }
    }
}