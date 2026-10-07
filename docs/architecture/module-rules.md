# Module Boundary Rules

Dependency direction:

- Domain -> Shared
- Application -> Domain + Shared + explicit cross-module contracts
- Infrastructure -> Application/Domain + Persistence + Shared
- Api -> Application + Shared
- Composition -> registration and routing only
- Desktop -> Shared + approved Server API contracts
- Agent -> Shared + approved Server contracts

Forbidden:

- Domain, Application or Api code accessing Entity Framework or DbContext.
- Domain depending on Infrastructure.
- A module referencing another module's Domain, Infrastructure or Api namespace.
- HTTP endpoints containing business rules or direct database queries.
- Desktop referencing Server Persistence, Modules or Infrastructure.
- Agent referencing Server Persistence, Modules or Infrastructure.
- Wall-clock statics outside Time infrastructure.
- UI code performing authoritative business calculations.
- External side effects inside authoritative DB transactions.

Cross-module collaboration is expressed through explicit contracts, never shared database access.

The architecture guard and platform-skeleton guard are local certification gates.
