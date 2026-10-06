namespace GameNet.Server.Composition;

public static class Routing
{
    public static void MapGameNetRoutes(this WebApplication app)
    {
        app.MapGet("/health", (Infrastructure.StartupState state) => Results.Ok(state.ToResponse()))
            .WithName("Health");

        app.MapGet("/", () => Results.Ok(new
        {
            service = "GameNet.Server",
            status = "running"
        }));
    }
}
