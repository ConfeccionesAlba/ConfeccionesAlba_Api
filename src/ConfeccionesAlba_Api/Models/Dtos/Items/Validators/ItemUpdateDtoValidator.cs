using FluentValidation;

namespace ConfeccionesAlba_Api.Models.Dtos.Items.Validators;

public class ItemUpdateDtoValidator : AbstractValidator<ItemUpdateDto>
{
    public ItemUpdateDtoValidator()
    {
        RuleFor(x => x.Id).NotEmpty().GreaterThan(0);
        RuleFor(x => x.Description).NotEmpty().MaximumLength(5000);
    }
}