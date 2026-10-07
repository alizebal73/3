# Assumptions and Open Decisions Register

A hidden assumption is a future rework candidate.

Every material unknown must be recorded here or converted into an ADR/requirement before implementation.

| ID | Topic | Current assumption/question | Impact if wrong | Owner | Decision date | Status |
|---|---|---|---|---|---|---|
| O-001 | Scale | Tier S/M/L envelope is the initial engineering proposal | performance/rearchitecture | Product/Engineering | pending | Open |
| O-002 | RPO/RTO | Business recovery targets need explicit approval | backup/recovery design | Owner/Engineering | pending | Open |
| O-003 | Retention | Customer/audit/log retention periods need policy approval | storage/privacy | Owner/Engineering | pending | Open |
| O-004 | Agent transport | ASP.NET Core SignalR over authenticated /hubs/agent is the selected production transport | protocol implementation | Engineering | 2026-10-07 | Accepted |
| O-005 | Update signing | Production signing key ownership/process must be defined before release | release security | Owner/Engineering | pending | Open |
| O-006 | Payment integration | Payment method/provider scope must be defined before payment implementation | billing architecture | Product | pending | Open |
| O-007 | Network topology | Supported LAN/WAN failure model must be confirmed | Agent/realtime behavior | Engineering | pending | Open |
| O-008 | Multi-shop | Multi-tenant/multi-branch is out of first release unless requirements change | data model | Product | pending | Open |
| O-009 | Reporting | Heavy reports must not block operational writes | DB/reporting architecture | Engineering | pending | Open |
| O-010 | Language | fa-IR is first-class; en-US remains supported | UX/localization | Product | accepted | Accepted |

## Rule

Open decisions are allowed during Foundation planning, but no business code may silently assume a value marked Open.

A decision that changes architecture, schema, security, compatibility or release behavior becomes an ADR.
