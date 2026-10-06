# Branching Policy

Permanent:
- main = releasable
- develop = integration

Short-lived:
- feature/<area>-<name>
- fix/<area>-<name>
- refactor/<area>-<name>
- chore/<name>

Never develop directly on main.
No merge while CI is red.
Every bug fix adds a regression test.
