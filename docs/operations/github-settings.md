# GitHub Source-Control Policy

GitHub is source control, review and history for GameNet 3, not the certification environment.

Repository policy:
- main remains the only release line.
- Existing `develop` and older `foundation/*` branches are historical/legacy branches during this stabilization; no new work is to be started from them.
- normal work uses short-lived feature/*, fix/*, refactor/* or chore/* branches.
- Foundation branches are temporary stabilization/certification branches and must not become permanent release lines.
- squash merge is preferred.
- force-pushes to shared branches should be disabled.
- pull requests are preferred.

The Foundation stage permits exactly one GitHub Actions workflow:
.github/workflows/foundation-local.yml

It runs only on the approved self-hosted Windows runner and always calls the canonical local verification script. It supports manual dispatch plus automatic execution on the Foundation branch and pull requests targeting main. It must never become a second build/test implementation or certification authority.

The repository should never contain secrets, machine-local credentials or generated release artifacts.

GitHub-side branch protection is an operational setting, not application correctness. Before Foundation certification, reconcile the live branch inventory so `main` is the only release line, legacy integration/stage branches are retired or explicitly archived, and main is protected against force-push with pull-request review where repository plan permits it.
