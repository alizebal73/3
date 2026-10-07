# Client Capability Contract Map

The Agent is an untrusted executor. These contracts define the stable shape before implementation.

| Capability | Server authority | Shared contract | Agent responsibility | Desktop responsibility |
|---|---|---|---|---|
| Game launch | Server validates Session + Station + Game + Account allocation + permission | LaunchGameCommand | Validate lease/command, resolve approved local launch profile, start process, report observation/ack | Request command and show authoritative result |
| Game stop | Server validates ownership/permission | StopGameCommand | Stop only the execution bound to the command/session | Request command and show authoritative result |
| Lock/unlock | Server permission/approval | ClientControlCommand | Apply local lock state | Request and display state |
| Restart/shutdown | Server permission/approval + maintenance rule | ClientControlCommand | Execute OS action only after lease/command validation | Request and display pending/result |
| Maintenance | Server owns Station state | ClientControlCommand | Enter/exit local maintenance behavior | Manage workflow/status |
| Client policy | Server owns versioned policy | ApplyClientPolicyCommand / ClientPolicySnapshot | Apply and report local policy | Edit through Server Settings only |
| Game catalog refresh | Server owns catalog | RefreshClientCatalogCommand | Refresh local execution catalog/cache | Manage catalog through Games module |
| Game account allocation | Server owns allocation | Game account contracts | Apply/display assigned account context, never allocate authoritatively | Manage allocation through GameAccounts module |
| Diagnostics | Server owns operation/audit | CollectDiagnostics + state contracts | Collect redacted diagnostics | Request/download/display according to permissions |

## Cross-cutting invariants

- Every executable command is bound to DeviceId + current authoritative Agent lease.
- Command expiry is mandatory.
- Commands are idempotent where execution can be retried.
- Agent observations are never financial/session authority.
- Local cached policy can be replaced by reconciliation/update.
- A stale connection cannot execute a command for a newer lease.
