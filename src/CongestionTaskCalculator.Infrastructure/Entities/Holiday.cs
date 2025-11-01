public sealed class Holiday : BaseEntity
{
    public int CityId { get; set; }
    public DateOnly Date { get; set; } 
    public City City { get; set; } = default!;
}