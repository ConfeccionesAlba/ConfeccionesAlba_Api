using ConfeccionesAlba_Api.Routes.Categories.Endpoints;
using FluentValidation;

namespace ConfeccionesAlba_Api.Routes.Categories.Validators;

public class CategoryUpdateRequestValidator : AbstractValidator<CategoryUpdateRequest>
{
    public CategoryUpdateRequestValidator()
    {
        RuleFor(x => x.Id).NotEmpty().GreaterThan(0);
        RuleFor(x => x.Description).NotEmpty().MaximumLength(500);
    }
}