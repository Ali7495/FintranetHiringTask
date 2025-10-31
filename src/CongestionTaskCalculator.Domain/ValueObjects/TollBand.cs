public sealed class TollBand
{
    public TimeSpan StartTime { get; set; }
    public TimeSpan EndTime { get; set; }
    public decimal Fee { get; set; }

    public TollBand(TimeSpan startTime, TimeSpan endTime, decimal fee)
    {
        if (startTime > endTime) throw new ArgumentException("End time must be greater than start time!");

        if (fee < 0) throw new ArgumentOutOfRangeException(nameof(fee));

        StartTime = startTime;
        EndTime = endTime;
        Fee = fee;
    }

    public bool IsTimeValid(TimeSpan time) => time >= StartTime && time <= EndTime;
}