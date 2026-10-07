namespace GameNet.Desktop.Infrastructure.Hosting;

public sealed class DesktopOptions
{
    public const string SectionName = "GameNet:Desktop";

    public string ServerBaseUrl { get; init; } = "http://127.0.0.1:5080";
}
