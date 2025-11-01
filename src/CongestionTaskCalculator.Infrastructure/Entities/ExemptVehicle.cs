public sealed class ExemptVehicle : BaseEntity
{
    public int CityId { get; set; }
    public int VehicleType { get; set; } 
    public City City { get; set; } = default!;
}