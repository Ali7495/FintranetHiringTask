
public sealed class GothenburgTaxCalculator : ITaxCalculator
{
    public decimal CalculateDailyTax(VehicleType vehicleType, IEnumerable<DateTime> passes, CitySettings citySettings, IEnumerable<TollBand> tollBands, ISet<DateOnly> holidays2013, bool includesDayBeforeHolidayRule = true)
    {
        if (passes is null) return 0;

        if (IsTollFreeVehicle(vehicleType, citySettings)) return 0;

        List<DateTime> orderedPasses = passes.OrderBy(p => p).ToList();

        if (orderedPasses.Count == 0) return 0;

        DateOnly day = DateOnly.FromDateTime(orderedPasses[0]);

        orderedPasses = orderedPasses.Where(p => DateOnly.FromDateTime(p) == day).ToList();

        if (IsTollFreeDate(day, holidays2013, includesDayBeforeHolidayRule, citySettings.JulyExempt)) return 0;


        var bands = (tollBands ?? Enumerable.Empty<TollBand>()).ToArray();

        var windowMinutes = citySettings.SingleChargeWindowMinutes;
        decimal total = 0;

        DateTime windowStart = orderedPasses[0];
        decimal windowMax = GetFeeForTime(orderedPasses[0], bands);

        for (int i = 1; i < orderedPasses.Count; i++)
        {
            var current = orderedPasses[i];
            var diffMinutes = (current - windowStart).TotalMinutes;

            var fee = GetFeeForTime(current, bands);

            if (diffMinutes <= windowMinutes)
            {
                if (fee > windowMax) windowMax = fee;
            }
            else
            {
                total += windowMax;

                windowStart = current;
                windowMax = fee;
            }
        }

        total += windowMax;

        if (total > citySettings.DailyCap) total = citySettings.DailyCap;

        return total;
    }

    private static bool IsTollFreeVehicle(VehicleType vehicleType, CitySettings citySettings)
        => citySettings.ExemptVehicles.Contains(vehicleType);

    private static bool IsTollFreeDate(
        DateOnly date,
        ISet<DateOnly> holidays2013,
        bool includeDayBeforeHolidayRule,
        bool julyExempt)
    {
        var dayOfWeek = date.DayOfWeek;
        if (dayOfWeek is DayOfWeek.Saturday or DayOfWeek.Sunday) return true;


        if (julyExempt && date.Month == 7) return true;

        holidays2013 ??= new HashSet<DateOnly>();


        if (holidays2013.Contains(date)) return true;


        if (includeDayBeforeHolidayRule)
        {
            var nextDay = date.AddDays(1);
            if (holidays2013.Contains(nextDay)) return true;
        }

        return false;
    }

    private static decimal GetFeeForTime(DateTime time, TollBand[] bands)
    {
        var t = time.TimeOfDay;


        decimal fee = 0;
        for (int i = 0; i < bands.Length; i++)
        {
            if (bands[i].IsTimeValid(t))
                fee = Math.Max(fee, bands[i].Fee);
        }
        return fee;
    }
}