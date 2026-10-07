# Product Charter

## Product

GameNet Manager 3 is a native Windows GameNet/CyberCafe management system for operating a shop from a central authoritative Server.

## Primary outcomes

1. Run the shop reliably while the owner is away.
2. Keep money, sessions, tariffs, inventory and permissions authoritative on the Server.
3. Let operators perform routine work quickly without granting owner-level financial/security power.
4. Make client PCs controllable through a Server-authorized Agent.
5. Preserve a durable audit trail and recover correctly after failures.
6. Make future capabilities additive rather than requiring architectural rewrites.

## Primary actors

- Owner
- Admin/Manager
- Operator
- Agent/device
- Customer

The exact permission matrix is authoritative in docs/security/permission-matrix.md.

## Non-goals

- Multi-tenant SaaS architecture in the first product.
- Microservices for their own sake.
- Browser-based operator UI.
- Client-side authoritative money/session state.
- Direct Desktop/Agent database access.
- Distributed infrastructure before measured need.

## First usable release principle

The first release is not “all screens implemented”.

It is the smallest product that can safely operate a shop end-to-end with:
- authentication and permissions;
- station/device management;
- customer/session lifecycle;
- tariff resolution;
- financial settlement;
- required inventory/buffet flow;
- audit;
- backup/recovery;
- update/rollback;
- operator-visible diagnostics.

## Product change rule

A feature may be accepted because it is useful, but it may not bypass the architecture, security, compatibility, recovery or testing gates.

Business priority can change. Authority and integrity rules cannot change silently.
