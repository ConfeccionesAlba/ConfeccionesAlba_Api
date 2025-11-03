using ConfeccionesAlba_Api.Routes.Items.Endpoints;
using FluentValidation;

namespace ConfeccionesAlba_Api.Routes.Items.Validators;

public class ItemCreateRequestValidator : AbstractValidator<ItemCreateRequest>
{
    public ItemCreateRequestValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(250);
        RuleFor(x => x.Description).NotEmpty().MaximumLength(5000);
        RuleFor(x => x.PriceReference).GreaterThanOrEqualTo(0);
        RuleFor(x => x.CategoryId).GreaterThan(0);
    }
}