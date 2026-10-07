namespace GameNet.Desktop.Api;

public sealed record ServerConnectionOptions(Uri BaseAddress)
{
    public ServerConnectionOptions
    {
        ArgumentNullException.ThrowIfNull(BaseAddress);

        if (!BaseAddress.IsAbsoluteUri)
            throw new ArgumentException("Server BaseAddress must be absolute.", nameof(BaseAddress));

        if (!string.Equals(BaseAddress.Scheme, Uri.UriSchemeHttp, StringComparison.OrdinalIgnoreCase) &&
            !string.Equals(BaseAddress.Scheme, Uri.UriSchemeHttps, StringComparison.OrdinalIgnoreCase))
        {
            throw new ArgumentException("Server BaseAddress must use HTTP or HTTPS.", nameof(BaseAddress));
        }
    }
}
