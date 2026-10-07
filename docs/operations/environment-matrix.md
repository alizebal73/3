# Environment Matrix

## Developer / review

Purpose: fast local feedback.

Characteristics:
- pinned SDK/tooling;
- isolated test data;
- no production secrets;
- unit/contract/build gates.

## Foundation certification

Purpose: prove the platform on the approved Windows machine.

Requires:
- real PostgreSQL;
- native Desktop process launch;
- Windows service behavior;
- concurrency/recovery scripts;
- update/rollback smoke;
- approved local network behavior.

## Shop staging

Purpose: pre-release verification in a shop-like environment.

Requires:
- actual LAN topology;
- representative Agent count;
- real service installation;
- real PostgreSQL;
- representative tariffs/data;
- backup/restore exercise;
- operator workflow checks.

## Production/shop

Purpose: supported operation.

Requires:
- approved release manifest;
- compatible schema/API/Agent versions;
- verified backup;
- monitoring/diagnostics;
- recovery procedure;
- support/rollback procedure.

## Rule

An environment-specific value belongs in configuration, not source-code forks.
