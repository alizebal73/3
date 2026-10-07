[object Object]

## F13 — Client/Game capability foundation closure

Before the first business vertical slice, the Foundation must expose explicit boundaries for:
- Games catalog/launch policy;
- GameAccounts allocation lifecycle;
- ClientControl authorization;
- Agent game execution;
- lock/unlock;
- restart/shutdown;
- maintenance;
- client policy;
- diagnostics;
- game-runtime observation.

Shared V1 contracts and Server/Agent/Desktop folder ownership are established before implementation. The first actual command handler is implemented only inside the vertical slice that needs it.
