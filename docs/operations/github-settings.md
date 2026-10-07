# GitHub Source-Control Policy

GitHub is source control, review and history for GameNet 3, not the certification environment.

Repository policy:
- main remains releasable.
- develop is the integration branch.
- normal work uses short-lived feature/*, fix/*, refactor/* or chore/* branches.
- Foundation branches are temporary and must not become permanent stage branches.
- squash merge is preferred.
- force-pushes to shared branches should be disabled.
- pull requests are preferred.

The Foundation stage permits exactly one GitHub Actions workflow:
.github/workflows/foundation-local.yml

It is manual-only and runs on the approved self-hosted Windows runner. It calls the canonical local scripts and exists only to make the connected runner useful; it does not become a second build/test implementation or certification authority.

The repository should never contain secrets, machine-local credentials or generated release artifacts.

GitHub-side branch protection is an operational setting, not application correctness. Before Foundation certification, protect main/develop against force-push and require pull-request review where repository plan permits it.
