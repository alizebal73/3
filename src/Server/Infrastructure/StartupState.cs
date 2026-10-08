namespace GameNet.Server.Infrastructure;

public sealed class StartupState
{
    public const string Version = "0.2.0-foundation";
    public string ApplicationVersion => Version;
    public string Status => "ok";
    public string Service => "GameNet.Server";
}
