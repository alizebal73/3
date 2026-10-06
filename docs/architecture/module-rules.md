# Module Boundary Rules

Dependency direction:
- Domain -> Shared
- Application -> Domain + Shared + explicit cross-module contracts
- Infrastructure -> Application/Domain + Persistence + Shared
- Api -> Application + Shared
- Composition -> registration and routing only

Forbidden:
- Domain, Application or Api code accessing Entity Framework or DbContext.
- Domain depending on Infrastructure.
- A module referencing another module's Domain, Infrastructure or Api namespace.
- HTTP endpoints containing business rules or direct database queries.
- Client Agent or Dashboard referencing Server persistence namespaces.
- Wall-clock statics outside Time infrastructure.

Cross-module collaboration is expressed through explicit contracts, never shared database access. The architecture guard is a CI gate.
