public interface IRulesProvider
{
    Task<RulesSnapshot?> GetRulesAsync(string cityCode, CancellationToken ct = default);
}