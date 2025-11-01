using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("RulesDb");


        var dbPath = Path.Combine(Directory.GetCurrentDirectory(), "rules.db");
        services.AddDbContext<RulesDbContext>(options =>
            options.UseSqlite($"Data Source={dbPath}"));

        services.AddDbContext<RulesDbContext>(r => r.UseSqlite());

        services.AddMemoryCache();

        services.AddScoped<IRulesProvider, EfRulesProvider>();

        return services;
    }

    public static async Task MigrationAndSeedAsync(this IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        RulesDbContext db = scope.ServiceProvider.GetRequiredService<RulesDbContext>();
        await db.Database.MigrateAsync();

        if (!db.Cities.Any())
        {
            City gothenburg = new City()
            {
                Code = "GOTH",
                DailyCap = 60,
                SingleChargeWindowMinutes = 60,
                JulyExempt = true
            };

            gothenburg.TollBands = new[]
            {
                Band("06:00","06:29",8),
                Band("06:30","06:59",13),
                Band("07:00","07:59",18),
                Band("08:00","08:29",13),
                Band("08:30","14:59",8),
                Band("15:00","15:29",13),
                Band("15:30","16:59",18),
                Band("17:00","17:59",13),
                Band("18:00","18:29",8)
            }.ToList();

            var holidays2013 = new[]
            {
                new DateOnly(2013,1,1),   // New Year
                new DateOnly(2013,3,29),  // Good Friday
                new DateOnly(2013,4,1),   // Easter Monday
                new DateOnly(2013,5,1),   // May Day
                new DateOnly(2013,5,9),   // Ascension Day
                new DateOnly(2013,6,6),   // National Day
                new DateOnly(2013,6,21),  // Midsummer Eve
                new DateOnly(2013,11,2),  // All Saints' Day (Sat; weekend anyway)
                new DateOnly(2013,12,24), // Christmas Eve
                new DateOnly(2013,12,25), // Christmas Day
                new DateOnly(2013,12,26), // Boxing Day
                new DateOnly(2013,12,31)
            };
            gothenburg.Holidays = holidays2013.Select(h => new Holiday { Date = h }).ToList();

            gothenburg.ExemptVehicles = new[]
            {
                1,2,3,4,5,6,7
            }.Select(v => new ExemptVehicle { VehicleType = v }).ToList();

            db.Cities.Add(gothenburg);

            await db.SaveChangesAsync();
        }
    }

    static TollBandEntity Band(string startTime, string endTime, decimal fee)
    {
        return new() { Start = TimeSpan.Parse(startTime), End = TimeSpan.Parse(endTime), Fee = fee };
    }
}