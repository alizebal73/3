# Initial Project Map

This is the approved Foundation skeleton. A feature is not allowed to invent a new top-level project or architectural layer without an architecture decision.

## Source tree
- src/Server: Composition, Infrastructure, Persistence, Modules
- Server modules: Agents, Approvals, Auth, Backup, Billing, Buffet, Customers, Inventory, Reports, Sessions, Settings, Stations, Tariffs, Users, Vip, Wallet
- src/Desktop: Api, Infrastructure, Localization, Navigation, Shell, Theme, Features and Resources/Languages
- Desktop features: Home, Agents, Stations, Customers, Sessions, Billing, Wallet, Inventory, Buffet, Tariffs, Vip, Reports, Settings, Users, Approvals, Backup, Audit
- src/Client/Agent
- src/Shared: Contracts/V1, Primitives, Api, Errors, Results
- tests: Server.UnitTests, Server.IntegrationTests, Desktop.Tests, Client.UnitTests, Agent.Tests, Shared.Tests, ContractTests, E2E
- tests/Postgres.CertificationTests: isolated real-PostgreSQL certification suite, intentionally outside the ordinary solution gate

## Deployment source boundaries
- src/Installer: future Windows package/installer boundary
- deploy: deployment documentation and service/update scripts
- scripts: local engineering/certification commands

## Forbidden Foundation artifacts
- retired browser-based operator UI
- Node/browser build pipeline
- business C# files under src/Server/Modules before Foundation certification
- direct DB access from Desktop/Agent
- remote CI as the source of test truth
