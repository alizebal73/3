# Repo 2 → Repo 3 Foundation Gap Matrix

| Repo 2 root cause/pattern | What must exist before Repo 3 business work | Repo 3 current state | Gate |
|---|---|---|---|
| Multiple DTO/model authorities | Shared versioned contracts + client contract tests | Implemented | Guard + contract tests |
| Browser UI/toolchain drift | Native WPF boundary + browser artifact ban | Implemented | Skeleton/architecture guard |
| Giant Server composition file | Composition-only Program + module boundaries | Implemented | Architecture guard |
| Giant CI as second program | One canonical local script + thin workflow | Implemented | Workflow/supply-chain guards |
| Migration/model drift | Versioned migrations + bundle + clean/upgrade test | Implemented tooling; runtime certification pending | Local PostgreSQL/release gate |
| Agent ownership races | Durable DeviceId + ConnectionId + LeaseToken fencing | Implemented | Real Agent/PG certification pending |
| Business-state duplication | Server source of truth + no client DB | Implemented as architecture rule | Architecture guard |
| Installer/runtime coupling | Independent Server/Desktop/Agent install boundaries + manifest | Implemented foundation; update runner certification pending | Deployment gate |
| Mock/server drift | Explicit fake-boundary rule + integration tests | Implemented | Quality gate |
| Late security controls | Threat model + permissions + supply-chain + secure design review | Implemented | Security/release gate |
| Desktop lifetime/DI drift | WPF Generic Host + DI + configuration | Implemented | Desktop launch certification |
| Realtime reconnection uncertainty | Selected SignalR transport + reconnect + lease + reconciliation | Implemented platform path | Agent runtime certification |
| Hidden assumptions | Open-decision register + ADR rule | Implemented | Planning gate |
| Large-file complexity | Source-size threshold + composition guards | Implemented | Source-size guard |
| Feature-first development | Foundation certification gate before business modules | Implemented | Pre-coding/foundation gate |
