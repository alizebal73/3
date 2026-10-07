using System.Diagnostics;
using System.Diagnostics.Metrics;

namespace GameNet.Server.Infrastructure.Observability;

public sealed class PlatformMetrics : IDisposable
{
    private readonly Meter _meter = new("GameNet.Server", "0.2.0");
    private readonly Counter<long> _requestCount;
    private readonly Counter<long> _errorCount;
    private readonly Histogram<double> _requestDurationMs;

    private long _totalRequests;
    private long _totalErrors;
    private double _lastRequestDurationMs;
    private double _maxRequestDurationMs;

    public PlatformMetrics()
    {
        _requestCount = _meter.CreateCounter<long>(
            "gamenet.server.requests",
            unit: "{request}");

        _errorCount = _meter.CreateCounter<long>(
            "gamenet.server.errors",
            unit: "{error}");

        _requestDurationMs = _meter.CreateHistogram<double>(
            "gamenet.server.request.duration",
            unit: "ms");
    }

    public void RecordRequest(
        string method,
        string path,
        int statusCode,
        double durationMs)
    {
        _requestCount.Add(
            1,
            new KeyValuePair<string, object?>("method", method),
            new KeyValuePair<string, object?>("path", path),
            new KeyValuePair<string, object?>("status_code", statusCode));

        _requestDurationMs.Record(
            durationMs,
            new KeyValuePair<string, object?>("method", method),
            new KeyValuePair<string, object?>("path", path));

        Interlocked.Increment(ref _totalRequests);
        Interlocked.Exchange(ref _lastRequestDurationMs, durationMs);

        while (true)
        {
            var current = Volatile.Read(ref _maxRequestDurationMs);
            if (durationMs <= current)
                break;

            if (Interlocked.CompareExchange(
                    ref _maxRequestDurationMs,
                    durationMs,
                    current) == current)
            {
                break;
            }
        }

        if (statusCode >= 500)
        {
            _errorCount.Add(
                1,
                new KeyValuePair<string, object?>("method", method),
                new KeyValuePair<string, object?>("path", path),
                new KeyValuePair<string, object?>("status_code", statusCode));

            Interlocked.Increment(ref _totalErrors);
        }
    }

    public PlatformMetricsSnapshot Snapshot() =>
        new(
            Volatile.Read(ref _totalRequests),
            Volatile.Read(ref _totalErrors),
            Volatile.Read(ref _lastRequestDurationMs),
            Volatile.Read(ref _maxRequestDurationMs));

    public void Dispose() => _meter.Dispose();
}

public sealed record PlatformMetricsSnapshot(
    long TotalRequests,
    long TotalErrors,
    double LastRequestDurationMs,
    double MaxRequestDurationMs);
