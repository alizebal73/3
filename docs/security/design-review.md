# Security Design Review Checklist

This is a mandatory design gate before any vertical slice that crosses a trust boundary, stores sensitive data, changes authorization, or introduces a new external dependency.

## Required review

### Trust and identity
- What is the trusted boundary?
- What identifies the actor/device?
- Can identity be replayed, forged, copied or confused with IP address?
- How is credential issuance, rotation, revocation and expiry handled?

### Authorization
- Which exact Server-side permission authorizes the action?
- Is approval required?
- Can the Desktop hide the action without becoming the authorization authority?
- Can an Agent execute only commands that the Server authorized for its DeviceId/lease?

### Data
- Classification: Public / Internal / Sensitive / Financial-Security Critical.
- Storage location and owner.
- Retention policy or explicit policy-open status.
- Logging/redaction rules.
- Backup/restore implications.
- Export/report implications.

### Concurrency and replay
- Is the operation idempotent?
- What is the uniqueness key?
- What transaction/isolation boundary protects it?
- Which lease/fencing token prevents stale ownership?
- What happens after timeout with an unknown outcome?

### External effects
- Is the effect inside the database transaction, or after commit via Outbox?
- Can the effect be duplicated?
- Can a stale Agent or reconnect trigger it twice?
- What is the reconciliation path?

### Supply chain
- New package/vendor/source?
- Exact version and support status?
- Vulnerability/license impact?
- SBOM impact?
- Does the dependency change the runtime trust boundary?

### Recovery
- What happens after Server restart?
- Database interruption?
- Agent restart?
- Network partition?
- Partially applied deployment?
- Failed migration/update?
- Which data can be reconstructed, and which must come from authoritative state?

## Evidence

The feature specification must link:
- threat-model change;
- permission;
- contract;
- audit event;
- failure/retry semantics;
- recovery rule;
- regression/concurrency tests;
- release impact.

A green unit-test run alone is not security evidence.