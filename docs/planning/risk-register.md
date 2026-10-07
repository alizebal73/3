# Engineering Risk Register

This register is reviewed before each major vertical slice.

| ID | Risk | Trigger | Impact | Mitigation | Evidence |
|---|---|---|---|---|---|
| R-001 | Contract drift | Desktop/Agent invents DTO | Build/runtime mismatch | Shared V1 contracts + architecture guard + contract tests | guard/test |
| R-002 | Database/design drift | schema changed ad hoc | data loss/rework | migration review + clean DB certification | migration evidence |
| R-003 | Hidden authoritative state | Desktop/Agent calculates truth | inconsistency | Server-only authority rule | architecture test |
| R-004 | Retry duplicates money/inventory | timeout/retry | financial loss | idempotency + transactions + concurrency tests | PostgreSQL tests |
| R-005 | Race between Agent connections | reconnect overlap | wrong command target | lease/fencing | Agent certification |
| R-006 | Update leaves mixed versions | partial update | broken installation | manifest + staged update + health check + rollback | update smoke |
| R-007 | Backup looks valid but restore fails | corrupt/unusable backup | data loss | verify + clean restore test | recovery evidence |
| R-008 | Operational blindness | logs lack correlation/state | slow diagnosis | structured diagnostics + health + metrics | observability evidence |
| R-009 | Security added too late | auth/audit retrofitted | expensive rework | secure-by-design gate | security review |
| R-010 | Scope creep | new requirement bypasses roadmap | schedule/architecture churn | change-control + ADR | planning evidence |
| R-011 | Premature scaling complexity | distributed components added too early | coordination burden | modular monolith + measured capacity | ADR/performance |
| R-012 | File/class growth | large files accumulate | hard maintenance | size guard + refactor threshold | source-size guard |
| R-013 | Dependency vulnerability | third-party CVE | security/release risk | dependency inventory/SBOM + review | dependency evidence |
| R-014 | Localization regression | strings/direction break | operator errors | localization tests + bilingual smoke | Desktop tests |
| R-015 | Recovery is theoretical | scenario never exercised | unknown production behavior | recovery drills | recovery evidence |

A risk without a mitigation and verification method is not considered managed.
