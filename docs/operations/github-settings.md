# GitHub Source-Control Policy

GitHub is source control for GameNet 3, not the execution environment.

Repository policy:
- main remains releasable.
- develop is the integration branch.
- normal work uses short-lived feature/*, fix/*, refactor/* or chore/* branches.
- Foundation branches are temporary and must not become permanent stage branches.
- squash merge is preferred.
- force-pushes to shared branches should be disabled.
- pull requests are preferred for integration.

There is intentionally no GitHub Actions workflow. The authoritative build/test result comes from the approved Windows machine running scripts/verify.ps1.

The repository should never contain secrets, machine-local credentials or generated release artifacts.

GitHub-side branch protection is an operational setting, not application correctness. Before Foundation certification, protect main/develop against force-push and require pull-request review where repository plan permits it.
