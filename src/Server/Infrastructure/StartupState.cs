using GameNet.Shared.Contracts.System;

namespace GameNet.Server.Infrastructure;

public sealed class StartupState
{
    public const string Version = "0.1.0-foundation";
    public HealthResponse ToResponse() => new("ok", "GameNet.Server", Version);
}
