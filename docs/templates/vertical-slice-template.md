# Vertical Slice Plan

Slice ID:
Capability:
Goal:
Target release:

## 1. User outcome

Describe the smallest complete user-visible outcome.

## 2. Preconditions

Foundation gates required:
- architecture
- contracts
- persistence
- security
- observability
- testing
- recovery
- update compatibility

## 3. Slice boundaries

Server:
Desktop:
Agent:
Shared:
Persistence:

## 4. End-to-end path

1. Operator/customer action
2. Desktop request
3. Server authorization
4. Application use case
5. Domain decision
6. Transaction/persistence
7. Outbox side effect
8. Agent action if applicable
9. Acknowledgement/reconciliation
10. Desktop state update

## 5. Failure paths

- duplicate request
- timeout
- disconnect
- concurrency conflict
- restart
- database failure
- partial side effect

## 6. Evidence

Build:
Unit:
Integration:
Contract:
E2E:
Recovery:
Performance:
Operator acceptance:

## 7. Exit condition

The slice is complete only when the entire path works without hidden alternate sources of truth.
