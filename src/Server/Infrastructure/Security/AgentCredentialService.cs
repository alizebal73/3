using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using GameNet.Server.Infrastructure.Audit;
using GameNet.Server.Infrastructure.Time;
using GameNet.Server.Infrastructure.Transactions;
using GameNet.Server.Persistence;
using GameNet.Server.Persistence.Entities;
using GameNet.Shared.Contracts.V1.Security;
using Microsoft.EntityFrameworkCore;

namespace GameNet.Server.Infrastructure.Security;

public sealed class AgentCredentialService(
    GameNetDbContext dbContext,
    ITransactionCoordinator transactions,
    IAuditWriter auditWriter,
    IGameClock clock) : IAgentCredentialService
{
    public Task<AgentCredentialSecretResponse> ProvisionAsync(
        AgentCredentialProvisionRequest request,
        CancellationToken cancellationToken = default) =>
        transactions.ExecuteSerializableAsync(
            async ct =>
            {
                ValidateDeviceId(request.DeviceId);

                var exists = await dbContext.AgentCredentials.AnyAsync(
                    x => x.DeviceId == request.DeviceId && x.RevokedAtUtc == null,
                    ct);

                if (exists)
                    throw new InvalidOperationException("An active Agent credential already exists for this DeviceId.");

                var now = clock.UtcNow;
                var secret = GenerateSecret();

                var entity = new AgentCredential
                {
                    DeviceId = request.DeviceId,
                    SecretHash = HashSecret(secret),
                    CreatedAtUtc = now
                };

                dbContext.AgentCredentials.Add(entity);
                auditWriter.Append(new AuditEntry
                {
                    OccurredAtUtc = now,
                    ActorType = "System",
                    ActorId = request.DeviceId,
                    Operation = "AgentCredential.Provisioned",
                    ReferenceType = "AgentCredential",
                    ReferenceId = entity.Id.ToString("N"),
                    Reason = "Agent credential provisioned.",
                    CorrelationId = Guid.NewGuid().ToString("N"),
                    AfterJson = JsonSerializer.Serialize(new
                    {
                        request.DeviceId,
                        entity.Id
                    })
                });

                await Task.CompletedTask;

                return new AgentCredentialSecretResponse(
                    entity.DeviceId,
                    entity.Id,
                    secret,
                    entity.CreatedAtUtc);
            },
            cancellationToken);

    public Task<AgentCredentialSecretResponse> RotateAsync(
        AgentCredentialRotateRequest request,
        CancellationToken cancellationToken = default) =>
        transactions.ExecuteSerializableAsync(
            async ct =>
            {
                ValidateDeviceId(request.DeviceId);

                var current = await dbContext.AgentCredentials
                    .Where(x => x.DeviceId == request.DeviceId && x.RevokedAtUtc == null)
                    .SingleOrDefaultAsync(ct);

                if (current is null)
                    throw new InvalidOperationException("No active Agent credential exists for this DeviceId.");

                var now = clock.UtcNow;
                var secret = GenerateSecret();
                current.Revoke(now);

                var replacement = new AgentCredential
                {
                    DeviceId = request.DeviceId,
                    SecretHash = HashSecret(secret),
                    CreatedAtUtc = now
                };

                dbContext.AgentCredentials.Add(replacement);

                auditWriter.Append(new AuditEntry
                {
                    OccurredAtUtc = now,
                    ActorType = "System",
                    ActorId = request.DeviceId,
                    Operation = "AgentCredential.Rotated",
                    ReferenceType = "AgentCredential",
                    ReferenceId = replacement.Id.ToString("N"),
                    Reason = "Agent credential rotated; previous credential revoked.",
                    CorrelationId = Guid.NewGuid().ToString("N"),
                    BeforeJson = JsonSerializer.Serialize(new { current.Id }),
                    AfterJson = JsonSerializer.Serialize(new { replacement.Id })
                });

                return new AgentCredentialSecretResponse(
                    replacement.DeviceId,
                    replacement.Id,
                    secret,
                    replacement.CreatedAtUtc);
            },
            cancellationToken);

    public Task<bool> RevokeAsync(
        AgentCredentialRevokeRequest request,
        CancellationToken cancellationToken = default) =>
        transactions.ExecuteSerializableAsync(
            async ct =>
            {
                ValidateDeviceId(request.DeviceId);

                var current = await dbContext.AgentCredentials
                    .Where(x => x.DeviceId == request.DeviceId && x.RevokedAtUtc == null)
                    .SingleOrDefaultAsync(ct);

                if (current is null)
                    return false;

                var now = clock.UtcNow;
                current.Revoke(now);

                auditWriter.Append(new AuditEntry
                {
                    OccurredAtUtc = now,
                    ActorType = "System",
                    ActorId = request.DeviceId,
                    Operation = "AgentCredential.Revoked",
                    ReferenceType = "AgentCredential",
                    ReferenceId = current.Id.ToString("N"),
                    Reason = request.Reason,
                    CorrelationId = Guid.NewGuid().ToString("N")
                });

                return true;
            },
            cancellationToken);

    public Task<bool> AuthenticateAsync(
        string deviceId,
        string secret,
        CancellationToken cancellationToken = default) =>
        transactions.ExecuteSerializableAsync(
            async ct =>
            {
                ValidateDeviceId(deviceId);

                if (string.IsNullOrWhiteSpace(secret))
                    return false;

                var credential = await dbContext.AgentCredentials
                    .Where(x => x.DeviceId == deviceId && x.RevokedAtUtc == null)
                    .SingleOrDefaultAsync(ct);

                var suppliedHash = HashSecret(secret);
                var valid = credential is not null &&
                    CryptographicOperations.FixedTimeEquals(
                        Convert.FromHexString(credential.SecretHash),
                        Convert.FromHexString(suppliedHash));

                if (!valid)
                {
                    auditWriter.Append(new AuditEntry
                    {
                        OccurredAtUtc = clock.UtcNow,
                        ActorType = "System",
                        ActorId = deviceId,
                        Operation = "AgentCredential.AuthenticationRejected",
                        ReferenceType = "AgentCredential",
                        ReferenceId = credential?.Id.ToString("N"),
                        Reason = "Invalid or revoked Agent credential.",
                        CorrelationId = Guid.NewGuid().ToString("N")
                    });

                    return false;
                }

                credential!.LastAuthenticatedAtUtc = clock.UtcNow;
                return true;
            },
            cancellationToken);

    private static void ValidateDeviceId(string deviceId)
    {
        if (string.IsNullOrWhiteSpace(deviceId) || deviceId.Length > 128)
            throw new ArgumentException(
                "DeviceId must be 1-128 non-whitespace characters.",
                nameof(deviceId));
    }

    private static string GenerateSecret()
    {
        var bytes = RandomNumberGenerator.GetBytes(32);
        return Convert.ToBase64String(bytes)
            .TrimEnd('=')
            .Replace('+', '-')
            .Replace('/', '_');
    }

    private static string HashSecret(string secret)
    {
        var hash = SHA256.HashData(Encoding.UTF8.GetBytes(secret));
        return Convert.ToHexString(hash);
    }
}
