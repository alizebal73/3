namespace GameNet.Shared.Contracts.V1.System;

public sealed record HealthResponse(
    string Status,
    string Service,
    string Version,
    string Readiness,
    string CorrelationId);
