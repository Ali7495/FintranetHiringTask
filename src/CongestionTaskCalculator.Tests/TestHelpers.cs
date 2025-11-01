public static class TestHelpers
{
    public static (ITaxCalculator taxCalculator, CitySettings citySettings, List<TollBand> tollBands, HashSet<DateOnly> holidays)
        Gothenburg()
    {
        GothenburgTaxCalculator gothenburgTaxCalculator = new ();

        var city = new CitySettings(
            cityCode: "GOTH",
            dailyCap: 60,
            singleChargeWindowMinutes: 60,
            julyExempt: true,
            exemptVehicles: new[] { VehicleType.Bus, VehicleType.Motorcycle, VehicleType.Emergency, VehicleType.Diplomat, VehicleType.Military, VehicleType.Tractor, VehicleType.Foreign }
        );

        List<TollBand> bands = new()
        {
            new(TimeSpan.FromHours(6),  new TimeSpan(6,29,59), 8),
            new(new TimeSpan(6,30,0),   new TimeSpan(6,59,59), 13),
            new(new TimeSpan(7,0,0),    new TimeSpan(7,59,59), 18),
            new(new TimeSpan(8,0,0),    new TimeSpan(8,29,59), 13),
            new(new TimeSpan(8,30,0),   new TimeSpan(14,59,59), 8),
            new(new TimeSpan(15,0,0),   new TimeSpan(15,29,59), 13),
            new(new TimeSpan(15,30,0),  new TimeSpan(16,59,59), 18),
            new(new TimeSpan(17,0,0),   new TimeSpan(17,59,59), 13),
            new(new TimeSpan(18,0,0),   new TimeSpan(18,29,59), 8),
        };

        HashSet<DateOnly> holidays = new ()
        {
            new(2013,1,1), new(2013,3,29), new(2013,4,1), new(2013,5,1),
            new(2013,5,9), new(2013,6,6), new(2013,6,21), new(2013,11,2),
            new(2013,12,24), new(2013,12,25), new(2013,12,26), new(2013,12,31),
        };

        return (gothenburgTaxCalculator, city, bands, holidays);
    }
}