# Configuration and Secrets

Configuration is environment-specific.

## GameNet configuration

Required categories:
- database connection;
- business timezone;
- currency;
- Server DataRoot;
- Server BackupRoot;
- trusted proxies;
- authentication issuer, audience and signing key when authentication is enabled;
- PostgreSQL backup tool paths.

The committed `appsettings.foundation.json` is loaded explicitly by the Server and contains only non-secret foundation defaults.

When DataRoot or BackupRoot is omitted, the Server uses a service-safe path under the Windows common application data directory rather than the executable directory.

HTTP binding is controlled by the host/deployment environment (for example `ASPNETCORE_URLS`) and is not mixed into business configuration.

## Secrets

Production secrets are supplied by deployment provisioning and never committed.

Changing a configuration key requires documenting compatibility impact when the key affects persisted state, protocol behavior or recovery.
