# Foundation Verification Hardening

## Why this stage exists

The Foundation architecture and skeleton were designed before business coding, but local execution exposed a distinction that must remain explicit:

- Structural closure means the repository has the intended boundaries, projects, contracts and infrastructure.
- Verification closure means the actual Windows machine can execute every repository gate and the produced binaries/tests pass.

Business coding is blocked until both are closed.

## Findings from first real Windows run

The first approved Windows run exposed several defects in the verification layer and initial compile baseline:

1. PowerShell guards were not robust when a command returned zero, one or multiple objects under StrictMode.
2. A Desktop architecture guard incorrectly classified an HTTP API endpoint as a browser dependency.
3. The UI guard confused shared WPF resources with localization resources.
4. Server/Agent/Desktop project files manually duplicated SDK-default appsettings content items.
5. Shared IdempotencyKey code was syntactically invalid C#.
6. The original verify wrapper could continue after a failed dotnet build/test and print a misleading success message.

These findings are not business-feature failures, but they are Foundation defects because the certification system itself must be trustworthy.

## Hardening rules

1. Every repository guard must be executable under PowerShell StrictMode on Windows.
2. Every build/test/package command must fail closed on non-zero exit codes.
3. No guard may forbid a valid native Desktop API URL merely because it contains http:// or https://.
4. Shared WPF theme resources and localization resources must be validated separately.
5. SDK implicit items must be updated rather than duplicated.
6. Every new value object must have direct invariant tests.
7. A green message is only printed after every previous gate has actually completed successfully.

## Certification sequence

No business module implementation starts until:

- scripts/verify.ps1 passes on the approved Windows environment;
- native Desktop launch passes;
- real PostgreSQL certification passes;
- authenticated Agent reconnect/fencing passes;
- install/update/rollback and recovery smoke pass;
- release package signing/SBOM evidence is produced where required.

## Current status

Foundation is **not yet certified**.

The current work is verification hardening plus correction of the baseline compile defects discovered by the first real Windows run.

This status intentionally prevents the project from repeating the Repo 2 failure mode of treating a green-looking structure as proof of runtime readiness.