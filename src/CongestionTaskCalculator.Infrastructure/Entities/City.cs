public sealed class City : BaseEntity
{
    public string Code { get; set; } = default!; 
    public int DailyCap { get; set; }     
    public int SingleChargeWindowMinutes { get; set; } 
    public bool JulyExempt { get; set; }   

    public ICollection<TollBandEntity> TollBands { get; set; } = new List<TollBandEntity>();
    public ICollection<Holiday> Holidays { get; set; } = new List<Holiday>();
    public ICollection<ExemptVehicle> ExemptVehicles { get; set; } = new List<ExemptVehicle>();
}