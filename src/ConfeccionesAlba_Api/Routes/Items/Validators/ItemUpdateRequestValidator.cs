using ConfeccionesAlba_Api.Routes.Items.Endpoints;
using FluentValidation;

namespace ConfeccionesAlba_Api.Routes.Items.Validators;

public class ItemUpdateRequestValidator : AbstractValidator<ItemUpdateRequest>
{
    public ItemUpdateRequestValidator()
    {
        RuleFor(x => x.Id).NotEmpty().GreaterThan(0);
        RuleFor(x => x.Description).NotEmpty().MaximumLength(5000);
    }
}