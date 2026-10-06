namespace GameNet.Server.Infrastructure.Hosting;

public sealed class ServerReadiness
{
    private int _ready;

    public bool IsReady => Volatile.Read(ref _ready) == 1;

    public void MarkReady() => Volatile.Write(ref _ready, 1);
}
