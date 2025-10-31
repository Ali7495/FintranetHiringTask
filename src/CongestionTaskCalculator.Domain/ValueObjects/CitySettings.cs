public sealed class CitySettings
{
    public string CityCode { get; set; }
    public decimal DailyCap { get; set; }                  
    public int SingleChargeWindowMinutes { get; set; }  
    public bool JulyExempt { get; }              
    public IReadOnlySet<VehicleType> ExemptVehicles { get; set; }


    public CitySettings(string cityCode, decimal dailyCap, int singleChargeWindowMinutes, bool julyExempt,
    IEnumerable<VehicleType> exemptVehicles)
    {
        if (string.IsNullOrEmpty(cityCode)) throw new ArgumentException("City code cannot be null!");

        if (dailyCap < 0) throw new ArgumentOutOfRangeException(nameof(dailyCap));

        if (singleChargeWindowMinutes <= 0) throw new ArgumentOutOfRangeException(nameof(singleChargeWindowMinutes));

        CityCode = cityCode;
        DailyCap = dailyCap;
        SingleChargeWindowMinutes = singleChargeWindowMinutes;
        JulyExempt = julyExempt;

        ExemptVehicles = new HashSet<VehicleType>(exemptVehicles ?? Array.Empty<VehicleType>());
    }
}