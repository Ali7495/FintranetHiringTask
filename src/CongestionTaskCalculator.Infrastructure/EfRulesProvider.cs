
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;
using Microsoft.Extensions.Caching.Memory;

public class EfRulesProvider : IRulesProvider
{
    private readonly RulesDbContext _dbContext;
    private readonly IMemoryCache _memory;

    public EfRulesProvider(RulesDbContext dbContext, IMemoryCache memory)
    {
        _dbContext = dbContext;
        _memory = memory;
    }

    public async Task<RulesSnapshot?> GetRulesAsync(string cityCode, CancellationToken cancellationToken = default)
    {
        string key = $"rules:{cityCode}";
        if (_memory.TryGetValue(key, out RulesSnapshot? cached)) return cached;

        City city = await _dbContext.Cities
            .Include(c => c.ExemptVehicles)
            .Include(c => c.Holidays)
            .Include(c => c.TollBands)
            .FirstOrDefaultAsync(c => c.Code == cityCode, cancellationToken);

        if (city == null) return null;

        CitySettings settings = new(
            city.Code,
            city.DailyCap,
            city.SingleChargeWindowMinutes,
            city.JulyExempt,
            city.ExemptVehicles.Select(e => (VehicleType)e.VehicleType)
            );


        List<TollBand> tollBands = city.TollBands.OrderBy(t => t.Start)
            .Select(t => new TollBand(t.Start, t.End, t.Fee)).ToList();

        HashSet<DateOnly> holidays = city.Holidays.Select(h => h.Date).ToHashSet();

        RulesSnapshot rulesSnapshot = new(settings, tollBands, holidays);

        _memory.Set(key, rulesSnapshot, TimeSpan.FromMinutes(30));

        return rulesSnapshot;

    }
}