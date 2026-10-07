# Assumptions and Open Decisions Register

A hidden assumption is a future rework candidate.

Every material unknown must be recorded here or converted into an ADR/requirement before implementation.

| ID | Topic | Current assumption/question | Impact if wrong | Owner | Decision date | Status |
|---|---|---|---|---|---|---|
| O-001 | Scale | Foundation baseline: up to 50 connected Agents on one shop/server; 100 is a measured future expansion target | performance/rearchitecture | Engineering | 2026-10-07 | Accepted |
| O-002 | RPO/RTO | Baseline RPO <=15 min / RTO <=60 min; release claim requires runtime evidence | backup/recovery design | Engineering | 2026-10-07 | Accepted baseline |
| O-003 | Retention | Foundation baseline: 30 days for verified backups; personal-data retention may be module-specific | storage/privacy | Engineering | 2026-10-07 | Accepted baseline |
| O-004 | Agent transport | ASP.NET Core SignalR over authenticated /hubs/agent is the selected production transport | protocol implementation | Engineering | 2026-10-07 | Accepted |
| O-005 | Update signing | Production packages require organization-controlled code signing plus SHA-256 verification; private key stays off repo/runtime | release security | Engineering | 2026-10-07 | Accepted baseline |
| O-006 | Payment integration | Payment method/provider scope must be defined before payment implementation | billing architecture | Product | pending | Open |
| O-007 | Network topology | Server remains authority; Desktop becomes offline; Agent loses authority after lease expiry and reconciles before commands resume | Agent/realtime behavior | Engineering | 2026-10-07 | Accepted baseline |
| O-008 | Multi-shop | Multi-tenant/multi-branch is out of first release unless requirements change | data model | Product | pending | Open |
| O-009 | Reporting | Reports stay read-only; heavy reporting may move to projections/read replicas only by measured need + ADR | DB/reporting architecture | Engineering | 2026-10-07 | Accepted baseline |
| O-010 | Language | fa-IR is first-class; en-US remains supported | UX/localization | Product | accepted | Accepted |

## Rule

Open decisions are allowed during Foundation planning, but no business code may silently assume a value marked Open.

A decision that changes architecture, schema, security, compatibility or release behavior becomes an ADR.
