# Runtime Topology

## Components

Desktop/WPF <-> Server/API <-> PostgreSQL

Server <-> authenticated Agent 1
Server <-> authenticated Agent 2
Server <-> authenticated Agent N

Local Update Folder -> Desktop/Server/Agent through manifest, checksum and compatibility validation.

Server -> Verified Backup Storage.

## Authority

- PostgreSQL stores authoritative business state.
- Server owns business decisions and transaction boundaries.
- Desktop is a human-facing client.
- Agent is a constrained executor on the client machine.
- Update folder distributes packages; it is not an authority for business state.

## Foundation traffic classes

### Operator traffic
Desktop -> Server.

### Device traffic
Agent <-> Server.

### Persistence traffic
Server <-> PostgreSQL.

### Side effects
Server transaction -> Outbox -> approved dispatcher/transport.

## Prohibited paths

- Desktop -> PostgreSQL.
- Agent -> PostgreSQL.
- Desktop -> Agent direct authoritative commands.
- Agent -> another Agent.
- UI -> alternate business implementation.
