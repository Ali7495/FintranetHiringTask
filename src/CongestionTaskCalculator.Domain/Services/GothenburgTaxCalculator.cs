public sealed class GothenburgTaxCalculator : ITaxCalculator
{
    public TaxResult CalculateDailyTax(
        VehicleType vehicleType,
        IEnumerable<DateTime> passes,
        CitySettings citySettings,
        IEnumerable<TollBand> tollBands,
        ISet<DateOnly> holidays2013,
        bool includeDayBeforeHolidayRule = true)
    {
        List<TaxWindow> windows = new List<TaxWindow>();

        if (passes is null) return new TaxResult(0, false, windows);
        if (IsTollFreeVehicle(vehicleType, citySettings)) return new TaxResult(0, false, windows);

        List<DateTime> orderedPasses = passes.OrderBy(p => p).ToList();
        if (orderedPasses.Count == 0) return new TaxResult(0, false, windows);

        var day = DateOnly.FromDateTime(orderedPasses[0]);
        orderedPasses = orderedPasses.Where(p => DateOnly.FromDateTime(p) == day).ToList();
        if (orderedPasses.Count == 0) return new TaxResult(0, false, windows);

        if (IsTollFreeDate(day, holidays2013, includeDayBeforeHolidayRule, citySettings.JulyExempt))
            return new TaxResult(0, false, windows);

        TollBand[] bands = (tollBands ?? Enumerable.Empty<TollBand>()).ToArray();

        decimal total = 0;
        var windowStart = orderedPasses[0];
        var lastInWindow = orderedPasses[0];
        decimal windowMax = GetFeeForTime(windowStart, bands);

        for (int i = 1; i < orderedPasses.Count; i++)
        {
            var current = orderedPasses[i];
            var diffMinutes = (current - windowStart).TotalMinutes;
            decimal fee = GetFeeForTime(current, bands);

            if (diffMinutes <= citySettings.SingleChargeWindowMinutes)
            {
                lastInWindow = current;
                if (fee > windowMax) windowMax = fee;
            }
            else
            {
                total += windowMax;
                windows.Add(new TaxWindow(windowStart, lastInWindow, windowMax));

                windowStart = current;
                lastInWindow = current;
                windowMax = fee;
            }
        }

        total += windowMax;
        windows.Add(new TaxWindow(windowStart, lastInWindow, windowMax));

        bool capped = false;
        if (total > citySettings.DailyCap)
        {
            total = citySettings.DailyCap;
            capped = true;
        }

        return new TaxResult(total, capped, windows);
    }

    private static bool IsTollFreeVehicle(VehicleType vehicleType, CitySettings city)
        => city.ExemptVehicles.Contains(vehicleType);

    private static bool IsTollFreeDate(DateOnly date, ISet<DateOnly> holidays2013, bool includeDayBefore, bool julyExempt)
    {
        if (date.DayOfWeek is DayOfWeek.Saturday or DayOfWeek.Sunday) return true;
        if (julyExempt && date.Month == 7) return true;
        holidays2013 ??= new HashSet<DateOnly>();
        if (holidays2013.Contains(date)) return true;
        if (includeDayBefore && holidays2013.Contains(date.AddDays(1))) return true;
        return false;
    }

    private static decimal GetFeeForTime(DateTime time, TollBand[] bands)
    {
        TimeSpan t = time.TimeOfDay;
        decimal fee = 0;
        for (int i = 0; i < bands.Length; i++)
            if (bands[i].IsTimeValid(t)) fee = Math.Max(fee, bands[i].Fee);
        return fee;
    }
}