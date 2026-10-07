# Operations Runbook Index

Before release, every operationally important subsystem must have a short runbook.

Required runbooks:
- Server start/stop/restart;
- PostgreSQL backup;
- PostgreSQL restore;
- failed migration;
- Agent pairing;
- Agent credential rotation/revocation;
- Agent offline/reconnect;
- stale connection lease;
- outbox backlog;
- Desktop cannot connect;
- update failure;
- rollback;
- corrupted/incomplete update package;
- disk full;
- log/diagnostic collection;
- recovery after unexpected power loss.

Each runbook states:
1. symptom;
2. authority/source of truth;
3. safe operator action;
4. unsafe action to avoid;
5. verification;
6. escalation condition;
7. audit impact.
