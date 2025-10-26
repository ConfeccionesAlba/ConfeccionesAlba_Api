using FluentValidation;

namespace ConfeccionesAlba_Api.Models.Dtos.Items.Validators;

public class ItemCreateDtoValidator : AbstractValidator<ItemCreateDto>
{
    public ItemCreateDtoValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(250);
        RuleFor(x => x.Description).NotEmpty().MaximumLength(5000);
        RuleFor(x => x.PriceReference).GreaterThanOrEqualTo(0);
        RuleFor(x => x.CategoryId).GreaterThan(0);
    }
}