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
