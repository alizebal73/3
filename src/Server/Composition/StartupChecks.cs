namespace GameNet.Server.Composition;

public static class StartupChecks
{
    public static void Validate(WebApplication app)
    {
        if (app.Environment.IsProduction())
        {
            // Production-only startup validation will be added before persistence is enabled.
        }
    }
}
