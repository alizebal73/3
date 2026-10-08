using GameNet.Server.Infrastructure.Outbox;
using GameNet.Server.Infrastructure.Time;
using Microsoft.Extensions.Logging.Abstractions;

namespace GameNet.Server.UnitTests;

public sealed class OutboxDeliveryFoundationTests
{
    [Fact]
    public void Missing_handler_is_not_registered_as_a_success_path()
    {
        var registry = new OutboxDeliveryRegistry([]);

        Assert.False(registry.TryGet("external.email.send", out var handler));
        Assert.Null(handler);
    }

    [Fact]
    public void Duplicate_message_type_handlers_are_rejected()
    {
        var first = new TestHandler("external.email.send");
        var second = new TestHandler("external.email.send");

        var exception = Assert.Throws<InvalidOperationException>(
            () => new OutboxDeliveryRegistry([first, second]));

        Assert.Contains("Duplicate Outbox delivery handler", exception.Message, StringComparison.Ordinal);
    }

    [Fact]
    public async Task Successful_handler_is_the_only_path_to_mark_published()
    {
        var dispatcher = new FakeDispatcher();
        var registry = new OutboxDeliveryRegistry([new TestHandler("internal.test")]);
        var clock = new FixedClock(new DateTimeOffset(2026, 10, 8, 0, 0, 0, TimeSpan.Zero));
        var processor = new OutboxDeliveryProcessor(
            dispatcher, registry, clock, NullLogger<OutboxDeliveryProcessor>.Instance);

        var published = await processor.ProcessBatchAsync(10, TimeSpan.FromSeconds(30));

        Assert.Equal(1, published);
        Assert.Equal(1, dispatcher.MarkPublishedCalls);
        Assert.Equal("lease-1", dispatcher.MarkedLeaseToken);
    }

    [Fact]
    public async Task Handler_failure_does_not_mark_published()
    {
        var dispatcher = new FakeDispatcher();
        var registry = new OutboxDeliveryRegistry([new TestHandler("internal.test", true)]);
        var clock = new FixedClock(new DateTimeOffset(2026, 10, 8, 0, 0, 0, TimeSpan.Zero));
        var processor = new OutboxDeliveryProcessor(
            dispatcher, registry, clock, NullLogger<OutboxDeliveryProcessor>.Instance);

        var published = await processor.ProcessBatchAsync(10, TimeSpan.FromSeconds(30));

        Assert.Equal(0, published);
        Assert.Equal(0, dispatcher.MarkPublishedCalls);
    }

    private sealed class FakeDispatcher : IOutboxDispatcher
    {
        public int MarkPublishedCalls { get; private set; }
        public string? MarkedLeaseToken { get; private set; }

        public Task<IReadOnlyList<OutboxMessage>> ClaimBatchAsync(
            int batchSize, TimeSpan leaseDuration, CancellationToken cancellationToken = default)
        {
            var message = new OutboxMessage { Id = 1, Type = "internal.test" };
            message.Claim("lease-1", DateTimeOffset.UtcNow.AddMinutes(1));
            return Task.FromResult<IReadOnlyList<OutboxMessage>>([message]);
        }

        public Task MarkPublishedAsync(
            long messageId, string leaseToken, DateTimeOffset publishedAtUtc,
            CancellationToken cancellationToken = default)
        {
            MarkPublishedCalls++;
            MarkedLeaseToken = leaseToken;
            return Task.CompletedTask;
        }
    }

    private sealed class TestHandler(string messageType, bool shouldFail = false) : IOutboxDeliveryHandler
    {
        public string MessageType => messageType;

        public Task DeliverAsync(OutboxMessage message, CancellationToken cancellationToken = default)
        {
            if (shouldFail)
                throw new InvalidOperationException("forced delivery failure");
            return Task.CompletedTask;
        }
    }

    private sealed class FixedClock(DateTimeOffset now) : IGameClock
    {
        public DateTimeOffset UtcNow => now;
        public DateTimeOffset LocalNow => now;
        public DateOnly BusinessDate => DateOnly.FromDateTime(now.DateTime);
    }
}