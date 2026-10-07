# Engineering Workflow Before and During Feature Development

## Branch model

- main is the only release line.
- Foundation branches may exist while the platform is being certified.
- Feature, fix, refactor and chore branches are short-lived.
- No direct development on main.
- Every bug fix includes a regression test or an explicit ADR exception.

This is deliberately simpler than maintaining a long-lived develop branch: the release line stays authoritative and integration happens through small, reviewable branches.

## Required sequence for every vertical slice

1. Product problem and operator outcome.
2. Requirement and acceptance criteria.
3. Definition of Ready.
4. Feature specification.
5. ADR when ownership, schema, contract, security or release semantics change.
6. Domain/Application/Infrastructure/API boundaries.
7. Shared contract change, if required.
8. Migration plan and transaction boundary.
9. Permission + audit design.
10. Failure/retry/recovery behavior.
11. Unit + integration + contract + concurrency tests.
12. Desktop/Agent behavior.
13. Local runtime smoke evidence.
14. Release/rollback impact.
15. Definition of Done and traceability update.

## Small-batch rule

A feature branch should not mix unrelated refactoring, schema redesign for another module, unrelated UI redesign, or unrelated installer changes.

Large work is split into vertical slices that leave a coherent, testable system at every step.

## Review rule

Reviewers evaluate both:
- does the code work?
- does the architecture still obey the Foundation rules?

The second question is mandatory.