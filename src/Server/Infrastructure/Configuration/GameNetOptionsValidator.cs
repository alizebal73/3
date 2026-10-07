using GameNet.Server.Infrastructure.Time;
using Microsoft.Extensions.Options;

namespace GameNet.Server.Infrastructure.Configuration;

public sealed class GameNetOptionsValidator : IValidateOptions<GameNetOptions>
{
    public ValidateOptionsResult Validate(string? name, GameNetOptions options)
    {
        if (string.IsNullOrWhiteSpace(options.BusinessTimeZone))
            return ValidateOptionsResult.Fail("GameNet:BusinessTimeZone is required.");

        try
        {
            _ = GameTimeZoneResolver.Resolve(options.BusinessTimeZone);
        }
        catch (TimeZoneNotFoundException)
        {
            return ValidateOptionsResult.Fail($"Unknown business timezone: {options.BusinessTimeZone}");
        }

        if (options.Currency.Length is < 3 or > 8)
            return ValidateOptionsResult.Fail("GameNet:Currency must be 3-8 characters.");

        if (string.IsNullOrWhiteSpace(options.DataRoot))
            return ValidateOptionsResult.Fail("GameNet:DataRoot is required.");

        if (string.IsNullOrWhiteSpace(options.BackupRoot))
            return ValidateOptionsResult.Fail("GameNet:BackupRoot is required.");

        if (string.IsNullOrWhiteSpace(options.Backup.PgDumpPath) ||
            string.IsNullOrWhiteSpace(options.Backup.PgRestorePath))
        {
            return ValidateOptionsResult.Fail(
                "GameNet:Backup must define PgDumpPath and PgRestorePath.");
        }

        if (options.Authentication.Enabled &&
            (string.IsNullOrWhiteSpace(options.Authentication.Issuer) ||
             string.IsNullOrWhiteSpace(options.Authentication.Audience) ||
             string.IsNullOrWhiteSpace(options.Authentication.SigningKey) ||
             options.Authentication.SigningKey.Length < 32))
        {
            return ValidateOptionsResult.Fail(
                "Enabled JWT authentication requires Issuer, Audience and a 32+ character SigningKey.");
        }

        return ValidateOptionsResult.Success;
    }
}
