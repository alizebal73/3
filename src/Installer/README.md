# Windows Installation and Packaging Boundary

The product has three independently deployable components:

- **Desktop**: native WPF WinExe for the operator/admin machine.
- **Server**: headless Windows Service and PostgreSQL authority on the server machine.
- **Agent**: Windows Service installed on each client PC.

The current Foundation provides repeatable deployment scripts under `deploy/` and keeps installer packaging separate from product runtime code.

Required installer inputs:

1. Arbitrary installation path selected by the administrator.
2. Component selection (Desktop, Server, Agent).
3. Prerequisite validation.
4. Service creation only for selected Windows Services.
5. Installation manifest containing ProductVersion, SchemaVersion, ApiContractVersion and AgentProtocolVersion.
6. Rollback-safe handling for failed updates.

The first supported update transport is a local update folder. Cloud delivery is a later transport and must not alter the runtime architecture.

No installer may bypass release compatibility, checksum verification, database migration rules or rollback safeguards.
