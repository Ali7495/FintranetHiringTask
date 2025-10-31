// here I use this interface to make the application available to calculate tax for each city.
// so we can define classes for any cities we want.
// then use their specific ways to calculate tax.

public interface ITaxCalculator
{
    decimal CalculateDailyTax(VehicleType vehicleType, IEnumerable<DateTime> passes, CitySettings citySettings,
    IEnumerable<TollBand> tollBands, ISet<DateOnly> holidays2013, bool includesDayBeforeHolidayRule = true);
}