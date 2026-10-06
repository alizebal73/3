# GitHub repository settings

The codebase defines the desired repository policy, but the current GitHub connector exposes repository/rules reads and not the write operation for branch protection.

Apply once in GitHub:

## main
- Require a pull request before merging.
- Require the `GameNet 3 CI / quality-gate` check to pass.
- Require the branch to be up to date before merging.
- Block force-pushes.
- Do not allow direct pushes for normal development.

## develop
- Require pull request for feature/fix branches.
- Require `GameNet 3 CI / quality-gate`.
- Block force-pushes.

## Merge strategy
Prefer squash merge for feature/fix branches.

## CI
The repository intentionally has one workflow: `.github/workflows/ci.yml`.
It targets the self-hosted Windows/X64 runner.
