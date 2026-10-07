# Configuration and Secrets

Configuration is environment-specific and follows standard .NET provider precedence.

## Base configuration

The committed `src/Server/appsettings.json` contains only non-secret foundation defaults.

Standard precedence is preserved:
1. base appsettings;
2. environment-specific appsettings;
3. environment variables and deployment-provided configuration.

This prevents a committed foundation file from accidentally overriding production environment variables.

## GameNet configuration

Required categories:
- database connection;
- business timezone;
- currency;
- Server DataRoot;
- Server BackupRoot;
- trusted proxies;
- authentication issuer, audience and signing key when authentication is enabled;
- PostgreSQL backup tool paths;
- Agent lease/heartbeat timing.

When DataRoot or BackupRoot is omitted, the Server uses a service-safe path under Windows Common Application Data rather than the executable directory.

HTTP binding is controlled by the host/deployment environment (for example ASPNETCORE_URLS) and is not mixed into business configuration.

## Secrets

Production secrets are supplied by deployment provisioning and never committed.

Changing a configuration key requires documenting compatibility impact when the key affects persisted state, protocol behavior or recovery.
