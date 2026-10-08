namespace GameNet.Server.Infrastructure.Outbox;

public sealed class OutboxDeliveryOptions
{
    public const string SectionName = "OutboxDelivery";

    public bool Enabled { get; set; }
    public int BatchSize { get; set; } = 100;
    public int LeaseDurationSeconds { get; set; } = 30;
    public int PollIntervalMilliseconds { get; set; } = 1000;
}