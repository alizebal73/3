namespace GameNet.Desktop.Api;

public sealed record ServerConnectionOptions
{
    public ServerConnectionOptions(Uri baseAddress)
    {
        ArgumentNullException.ThrowIfNull(baseAddress);

        if (!baseAddress.IsAbsoluteUri)
            throw new ArgumentException(
                "Server BaseAddress must be absolute.",
                nameof(baseAddress));

        if (!string.Equals(
                baseAddress.Scheme,
                Uri.UriSchemeHttp,
                StringComparison.OrdinalIgnoreCase) &&
            !string.Equals(
                baseAddress.Scheme,
                Uri.UriSchemeHttps,
                StringComparison.OrdinalIgnoreCase))
        {
            throw new ArgumentException(
                "Server BaseAddress must use HTTP or HTTPS.",
                nameof(baseAddress));
        }

        BaseAddress = baseAddress;
    }

    public Uri BaseAddress { get; }
}
