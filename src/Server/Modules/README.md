# Server Modules

Every business module owns:
- Domain
- Application
- Infrastructure
- Api
- tests/contract coverage as appropriate

Planned modules:
Agents, Approvals, Auth, Backup, Billing, Buffet, Customers, Inventory, Reports, Sessions, Settings, Stations, Tariffs, Users, VIP, Wallet.

No business implementation is allowed during Foundation.

Modules may not access one another's internal namespaces or database sets. Cross-module collaboration uses explicit contracts.
