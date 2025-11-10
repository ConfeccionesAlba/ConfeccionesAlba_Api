using ConfeccionesAlba_Api.Routes.Items.Endpoints;
using FluentValidation;

namespace ConfeccionesAlba_Api.Routes.Items.Validators;

public class ItemCreateRequestValidator : AbstractValidator<ItemCreateRequest>
{
    public ItemCreateRequestValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(250);
        RuleFor(x => x.Description).NotEmpty().MaximumLength(5000);
        RuleFor(x => x.CategoryId).NotNull().GreaterThan(0);
        RuleFor(x => x.PriceReference).NotNull().GreaterThanOrEqualTo(0);
        RuleFor(x => x.IsVisible).NotNull();
        RuleFor(x => x.File).NotNull().Must(file => file.Length > 0).WithMessage("File size must be greater than 0");
    }
}