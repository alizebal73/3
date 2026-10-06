# Persistence Boundary

GameNet uses PostgreSQL with EF Core.

Only the Server persistence and infrastructure layers may reference the DbContext or EF packages.

The clean-slate foundation currently contains:
- AuditEntry
- IdempotencyRecord

Business tables and their migrations are introduced with the first vertical slice after the foundation gate.

Migrations are generated with the repository-local dotnet-ef tool and committed after review.
