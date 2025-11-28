using ConfeccionesAlba_Api.Routes.Products.Endpoints;
using FluentValidation;

namespace ConfeccionesAlba_Api.Routes.Products.Validators;

public class ProductUpdateRequestValidator : AbstractValidator<ProductUpdateRequest>
{
    public ProductUpdateRequestValidator()
    {
        RuleFor(x => x.Id).GreaterThan(0);
        RuleFor(x => x.CategoryId).GreaterThan(0);
        RuleFor(x => x.Description).MaximumLength(5000);
    }
}