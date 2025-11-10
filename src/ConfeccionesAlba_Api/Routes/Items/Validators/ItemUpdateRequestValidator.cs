using ConfeccionesAlba_Api.Routes.Items.Endpoints;
using FluentValidation;

namespace ConfeccionesAlba_Api.Routes.Items.Validators;

public class ItemUpdateRequestValidator : AbstractValidator<ItemUpdateRequest>
{
    public ItemUpdateRequestValidator()
    {
        RuleFor(x => x.Id).GreaterThan(0);
        RuleFor(x => x.CategoryId).GreaterThan(0);
        RuleFor(x => x.Description).MaximumLength(5000);
        RuleFor(x => x.File).Must(file => file == null || file.Length > 0)
            .WithMessage("If a file is provided, its size must be greater than 0");
    }
}