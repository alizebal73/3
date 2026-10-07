# Data Classification and Software Supply Chain

## Data classes

### Public
Non-sensitive application/build metadata intended for operator visibility.

### Internal
Operational configuration and diagnostics that are not public but are not highly sensitive.

### Sensitive
Customer profile data, operator identity data, station/device identity data, audit records and operational diagnostics that could aid misuse.

### Financial/Security Critical
Wallet balances, payment records, debt, approvals, credentials, pairing secrets, signing material and security-sensitive audit information.

## Rules

- Credentials and signing secrets never enter source control.
- Sensitive and financial data are not logged in plaintext unless explicitly required and approved.
- Logs must avoid passwords, tokens, raw secrets and unnecessary personal data.
- Data retention must be defined per class.
- Export/report operations must respect permissions.
- Backup copies inherit the sensitivity of the source data.
- Restore operations are audited.

## Dependency inventory

Before release, dependency inventory must answer:
- package/library;
- exact version;
- license;
- source;
- support status;
- known vulnerabilities;
- production use;
- transitive dependency exposure.

A Software Bill of Materials (SBOM) is required before release so vulnerable components can be found quickly.

## Foundation supply-chain gate

Before the first release:
- generate an SBOM;
- review licenses;
- review known vulnerabilities;
- record approved exceptions;
- ensure release artifacts are traceable to source and dependency versions.

## Secure-development rule

Security requirements, risks and design decisions must be tracked as part of the normal development lifecycle, not added after implementation.
