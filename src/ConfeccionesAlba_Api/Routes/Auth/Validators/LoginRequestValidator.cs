using ConfeccionesAlba_Api.Routes.Auth.Endpoints;
using FluentValidation;

namespace ConfeccionesAlba_Api.Routes.Auth.Validators;

public class LoginRequestValidator : AbstractValidator<LoginRequest>
{
    public LoginRequestValidator()
    {
        RuleFor(x => x.Email).NotEmpty().EmailAddress();
        RuleFor(x => x.Password).NotEmpty();
    }
}