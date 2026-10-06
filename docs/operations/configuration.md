# Configuration and Secrets

Configuration is environment-specific.

Required categories:
- database connection
- business timezone
- currency
- DataRoot
- BackupRoot
- server binding
- trusted proxies
- authentication issuer, audience and key

Production secrets are supplied by deployment provisioning, never committed.

appsettings.foundation.json contains safe defaults only.

Changing a configuration key requires documenting compatibility impact when the key affects persisted state or protocol behavior.
