public sealed record RulesSnapshot(
    CitySettings City,
    IReadOnlyList<TollBand> TollBands,
    ISet<DateOnly> Holidays
    );