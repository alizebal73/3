# Stage 0 — Platform Foundation

Exit gates:
- .NET 10 LTS baseline pinned.
- Node requirement documented.
- warnings are errors.
- Server health has an integration test.
- Agent has a runnable skeleton.
- Shared contracts exist outside HTTP code.
- one self-hosted CI workflow.
- main/develop roles documented.
- module boundaries established.

Next vertical slice:
Station → Customer → Agent → Session.

Do not start reports, settings, buffet, or installers before the core state machine is tested.
