using Microsoft.EntityFrameworkCore;

public class RulesDbContext : DbContext
{
    public RulesDbContext(DbContextOptions options) : base(options)
    {

    }

    public DbSet<City> Cities => Set<City>();
    public DbSet<TollBandEntity> TollBands => Set<TollBandEntity>();
    public DbSet<Holiday> Holidays => Set<Holiday>();
    public DbSet<ExemptVehicle> ExemptVehicles => Set<ExemptVehicle>();


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<City>().HasIndex(e => e.Code).IsUnique();

        modelBuilder.Entity<TollBandEntity>().HasOne(e => e.City).WithMany(e => e.TollBands).HasForeignKey(e => e.CityId);

        modelBuilder.Entity<Holiday>().HasOne(e => e.City).WithMany(e => e.Holidays).HasForeignKey(e => e.CityId);
        
        modelBuilder.Entity<ExemptVehicle>().HasOne(e => e.City).WithMany(e => e.ExemptVehicles).HasForeignKey(e => e.CityId);
    }
}