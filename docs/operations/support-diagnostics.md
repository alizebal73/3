# Support Diagnostics

The product must be diagnosable without asking an operator to inspect source code.

## Diagnostic bundle

A support diagnostic bundle may include:
- ProductVersion;
- SchemaVersion;
- ApiContractVersion;
- AgentProtocolVersion;
- Server readiness/health;
- Desktop connection state;
- Agent connection/lease state;
- recent error identifiers;
- correlation/operation identifiers;
- service states;
- non-sensitive configuration summary;
- relevant logs with secrets and unnecessary personal data redacted.

## Never include

- passwords;
- JWT/access tokens;
- pairing secrets;
- signing keys;
- database passwords;
- raw payment credentials;
- unnecessary customer personal data.

## Operator rule

A failure message should tell the operator:
- what happened;
- what is blocked;
- whether retry is safe;
- whether the Server or Agent is the authority;
- what safe next action is available.

## Developer rule

Every high-severity failure should be traceable through:
operation → correlation → actor/device → Server decision → persistence result → side effect/reconciliation state.

## Evidence

Support diagnostics must be tested against:
- Server unavailable;
- PostgreSQL unavailable;
- Agent offline;
- permission denial;
- concurrency conflict;
- duplicate operation;
- update failure.


## Local bundle collector

The repository provides `scripts/create-diagnostic-bundle.ps1` for the first-line operator/support bundle.

It collects only non-secret operational evidence:
- machine/tool metadata;
- Server health/readiness;
- correlation/operation identifiers returned by the health endpoint;
- build/contract/schema information;
- selected Windows service state.

The collector explicitly records that passwords, access tokens, pairing secrets, signing keys and payment credentials are excluded.

Example:

```powershell
.\scripts\create-diagnostic-bundle.ps1 -ServerUrl "http://127.0.0.1:5080"
```

Scenario-specific evidence for PostgreSQL outage, Agent offline, permission denial, concurrency conflicts, duplicate operations and update failures remains part of the corresponding runtime certification/vertical-slice tests.
