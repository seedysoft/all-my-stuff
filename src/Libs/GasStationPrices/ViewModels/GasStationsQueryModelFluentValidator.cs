using FluentValidation;

namespace Seedysoft.Libs.GasStationPrices.ViewModels;

/// <summary>
/// A standard AbstractValidator which contains multiple rules and can be shared with the back end API
/// </summary>
/// <typeparam name="TravelQueryModel"></typeparam>
public class GasStationsQueryModelFluentValidator : AbstractValidator<GasStationsQueryModel>
{
    public GasStationsQueryModelFluentValidator()
    {
        _ = RuleFor(static x => x.MaxDistanceInKm)
            .InclusiveBetween(1, 50);

        _ = RuleFor(static x => x.PetroleumProductsSelectedIds)
            .Must(static x => x != null && x.Any())
            .WithMessage("At least one product must be selected");
    }

    public Func<object, string, Task<IReadOnlyList<string>>> ValidateValue => async (model, propertyName) =>
    {
        FluentValidation.Results.ValidationResult result = await ValidateAsync(
            ValidationContext<GasStationsQueryModel>.CreateWithOptions((GasStationsQueryModel)model, x => x.IncludeProperties(propertyName)));

        return result.IsValid ? [] : [.. result.Errors.Select(e => e.ErrorMessage)];
    };
}
