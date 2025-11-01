public sealed class TollBandEntity : BaseEntity
{
    public int CityId { get; set; }
    public TimeSpan Start { get; set; }
    public TimeSpan End   { get; set; }
    public decimal Fee        { get; set; }
    public City City { get; set; } = default!;
}