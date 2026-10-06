using System.Security.Claims;

namespace GameNet.Server.Infrastructure.Security;

public interface ICurrentActor
{
    bool IsAuthenticated { get; }
    string ActorType { get; }
    string? ActorId { get; }
    string? DeviceId { get; }
}

public sealed class HttpCurrentActor(IHttpContextAccessor accessor) : ICurrentActor
{
    private ClaimsPrincipal User =>
        accessor.HttpContext?.User ?? new ClaimsPrincipal(new ClaimsIdentity());

    public bool IsAuthenticated => User.Identity?.IsAuthenticated == true;

    public string ActorType =>
        User.FindFirstValue("actor_type")
        ?? User.FindFirstValue(ClaimTypes.Role)
        ?? "unknown";

    public string? ActorId =>
        User.FindFirstValue(ClaimTypes.NameIdentifier)
        ?? User.FindFirstValue("sub");

    public string? DeviceId =>
        User.FindFirstValue("device_id");
}
