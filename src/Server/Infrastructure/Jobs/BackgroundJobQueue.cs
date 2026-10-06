using System.Threading.Channels;

namespace GameNet.Server.Infrastructure.Jobs;

public interface IBackgroundJobQueue
{
    ValueTask EnqueueAsync(BackgroundJob job, CancellationToken cancellationToken = default);
    IAsyncEnumerable<BackgroundJob> ReadAllAsync(CancellationToken cancellationToken = default);
}

public sealed class BackgroundJobQueue : IBackgroundJobQueue
{
    private readonly Channel<BackgroundJob> _channel =
        Channel.CreateUnbounded<BackgroundJob>(new UnboundedChannelOptions
        {
            SingleReader = true,
            SingleWriter = false
        });

    public ValueTask EnqueueAsync(
        BackgroundJob job,
        CancellationToken cancellationToken = default) =>
        _channel.Writer.WriteAsync(job, cancellationToken);

    public IAsyncEnumerable<BackgroundJob> ReadAllAsync(
        CancellationToken cancellationToken = default) =>
        _channel.Reader.ReadAllAsync(cancellationToken);
}
