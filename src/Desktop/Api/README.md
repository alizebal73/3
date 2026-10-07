# API Boundary

The native Desktop communicates with the headless Server only through this boundary.

Rules:
- Desktop UI and ViewModels do not create HttpClient instances.
- Request/response DTOs are owned by Shared/Contracts; Desktop does not define duplicate DTO, Request or Response types.
- No Desktop code may access Server Persistence, EF Core, PostgreSQL or Server implementation namespaces.
- Connection settings are explicit and validated before a client is constructed.
- Correlation and operation headers are created at the API boundary.
