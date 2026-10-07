# Foundation Local Certification

GitHub Actions is orchestration only. The approved Windows self-hosted machine is the certification environment and the exact commit being certified is authoritative.

## 1. Repository gate

Run:

scripts\verify.ps1

This verifies the platform skeleton, architecture boundaries, source-size limits, Foundation completeness, supply-chain guard, restore, Release build and ordinary tests.

## 2. Native Desktop gate

Run:

scripts\certify-desktop.ps1

The script builds and launches the real WPF WinExe. The operator must additionally verify:
- no browser runtime is involved;
- fa-IR is RTL;
- en-US is LTR;
- Server online/offline state is visible and recovers.

## 3. PostgreSQL gate

Set:
- GAMENET_DATABASE = disposable clean certification database;
- GAMENET_RESTORE_DATABASE = separate empty disposable restore target.

Run:

scripts\certify-postgresql.ps1

The script now:
- applies committed migrations;
- runs real PostgreSQL Foundation invariant/concurrency tests;
- creates a real custom-format pg_dump backup;
- restores it into the isolated target;
- probes the restored database.

The evidence is invalid if the restore target is not disposable and isolated.

## 4. Agent gate

Set:
- GAMENET_AGENT_BOOTSTRAP_SECRET = disposable provisioning secret;
- optionally GAMENET_AGENT_SERVER_URL = the certification Server URL.

Run:

scripts\certify-agent.ps1

This builds the Windows Agent, runs Agent tests and performs a real executable/process smoke. The Agent test suite plus runtime evidence must cover:
- durable DeviceId across restart;
- one authoritative connection per DeviceId;
- heartbeat renewal;
- stale lease fencing;
- reconnect;
- Server restart/reconnect;
- provisioning;
- credential rotation;
- credential revocation after short-lived JWT expiry;
- DPAPI persistence under the actual Windows service identity;
- HTTPS in production;
- rejection of unauthorized/non-Agent tokens;
- no duplicate command after network loss.

A process that merely stays alive is not sufficient by itself for Foundation sign-off.

## 5. Combined Foundation gate

Run:

scripts\certify-foundation.ps1

The combined command executes repository, Desktop, PostgreSQL and Agent gates and writes:

artifacts\foundation\foundation-evidence.json

The evidence records exact Git revision, machine, OS, tool versions and SHA-256 hashes of the certified Desktop and Agent binaries.

Environment-variable presence is only prerequisite metadata; it is never accepted as proof that a certification scenario passed.

## 6. Deployment and release gates

Before sign-off, on the same approved Windows environment:
- generate idempotent migration SQL;
- generate the Windows migration bundle;
- create the release package;
- verify Authenticode signatures;
- generate the pinned SPDX SBOM;
- review dependencies/licenses/vulnerabilities;
- perform clean install;
- start Server/Agent services;
- launch Desktop;
- apply a local update package;
- verify health after update;
- execute rollback;
- verify health and service recovery after rollback.

The final package, manifest, migration artifacts, SBOM and evidence must all reference the same certified Git revision.

## 7. Recovery gate

Perform and record:
- Server restart;
- Desktop reconnect;
- Agent restart/reconnect;
- lease/fencing behavior across network interruption;
- backup creation;
- isolated restore;
- update failure/rollback.

Record timestamps, exact commit, machine and result for every scenario.

## Final rule

Foundation is not Certified merely because CI is green or scripts exist.

Foundation Certified means:
1. exact final commit passes scripts\verify.ps1;
2. native Desktop runtime evidence exists;
3. real PostgreSQL migration/concurrency/backup/restore evidence exists;
4. real Agent authentication/lease/reconnect/fencing evidence exists;
5. clean install/update/rollback evidence exists;
6. SBOM/signature/migration release evidence exists;
7. recovery evidence exists;
8. the closure matrix is reviewed and contains no unresolved required row.

Any commit after the certified SHA invalidates the certification and requires re-certification.

No business feature development starts before this rule is satisfied.
