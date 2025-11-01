public class DomainCalculatorTests
{

    [Fact]
    public void Weekend_And_July_Are_Free()
    {
        var (calculator, city, bands, holidays) = TestHelpers.Gothenburg();


        DateTime sun = new(2013, 9, 1, 14, 0, 0);
        TaxResult result = calculator.CalculateDailyTax(VehicleType.Car, new[] { sun }, city, bands, holidays);
        Assert.Equal(0, result.Total);


        DateTime jul = new(2013, 7, 10, 8, 5, 0);
        TaxResult result2 = calculator.CalculateDailyTax(VehicleType.Car, new[] { jul }, city, bands, holidays);
        Assert.Equal(0, result2.Total);
    }

    [Fact]
    public void Day_Before_Holiday_Is_Free()
    {
        var (calc, city, bands, holidays) = TestHelpers.Gothenburg();

        DateTime date = new(2013, 3, 28, 14, 7, 27);
        TaxResult result = calc.CalculateDailyTax(VehicleType.Car, new[] { date }, city, bands, holidays);
        Assert.Equal(0, result.Total);
    }

    [Fact]
    public void SingleCharge_Within_60_Min_Takes_Max()
    {
        var (calc, city, bands, holidays) = TestHelpers.Gothenburg();

        DateTime date = new(2013, 2, 6);
        TaxResult result = calc.CalculateDailyTax(
            VehicleType.Car,
            new[] { date.AddHours(6).AddMinutes(35), date.AddHours(7).AddMinutes(10) },
            city, bands, holidays);
        Assert.Equal(18, result.Total);
        Assert.Single(result.Windows);
        Assert.Equal(18, result.Windows[0].AppliedFee);
    }
}