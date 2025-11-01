using FluentValidation;

public class CalculateTaxRequestValidator : AbstractValidator<CalculateTaxRequest>
{
    public CalculateTaxRequestValidator()
    {
        RuleFor(x => x.City).NotEmpty();
        RuleFor(x => x.VehicleType).NotEmpty();
        RuleFor(x => x.DateTimes)
            .NotEmpty()
            .Must(ds => ds.All(d => d.Year == 2013))
            .WithMessage("All dates must be in 2013")
            .Must(ds => ds.Select(d => d.Date).Distinct().Count() == 1)
            .WithMessage("All dates must be on the same day");
    }
}