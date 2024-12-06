using FluentValidation;

namespace Seedysoft.Libs.GasStationPrices.ViewModels;

/// <summary>
/// A standard AbstractValidator which contains multiple rules and can be shared with the back end API
/// </summary>
/// <typeparam name="TravelQueryModel"></typeparam>
public class TravelQueryModelFluentValidator : AbstractValidator<TravelQueryModel>
{
    public TravelQueryModelFluentValidator()
    {
        _ = RuleFor(x => x.Origin)
            .Must(x => !string.IsNullOrWhiteSpace(x))
            .WithMessage($"{nameof(TravelQueryModel.Origin)} must not be empty");

        _ = RuleFor(x => x.Destination)
            .Must(x => !string.IsNullOrWhiteSpace(x))
            .WithMessage($"{nameof(TravelQueryModel.Destination)} must not be empty");

        _ = RuleFor(static x => x.MaxDistanceInKm)
            .InclusiveBetween(1, 50);

        _ = RuleFor(static x => x.PetroleumProductsSelectedIds)
            .Must(static x => x.Count != 0)
            .WithMessage("At least one product must be selected");
    }

    public Func<object, string, Task<IEnumerable<string>>> ValidateValue => async (model, propertyName) =>
    {
        FluentValidation.Results.ValidationResult result = await ValidateAsync(
            ValidationContext<TravelQueryModel>.CreateWithOptions((TravelQueryModel)model, x => x.IncludeProperties(propertyName)));

        return result.IsValid ? [] : result.Errors.Select(e => e.ErrorMessage);
    };
}
