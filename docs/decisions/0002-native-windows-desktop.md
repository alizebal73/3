# ADR 0002 — Native Windows Desktop Operator Application

Status: Accepted

## Decision

The Operator/Admin application is a native Windows desktop executable built with WPF on .NET 10.

The product will not use a browser dashboard, React/Vite frontend, embedded WebView or Node-based UI build.

## Reasons

- The GameNet operator experience is a dedicated Windows application.
- Desktop deployment must work from a shortcut on the Windows desktop/Start Menu.
- The application needs reliable local Windows integration for the operator environment.
- Keeping UI code in the Desktop project prevents browser/runtime concerns from contaminating the Server boundary.
- WPF provides explicit localization and FlowDirection support needed for Persian/English UI.

## Consequences

- Server remains headless.
- Desktop consumes versioned Server contracts/API.
- Installer/deployment is a first-class architecture boundary.
- UI tests and release verification run on Windows.
- Future web UI is a new architecture decision, not a hidden fallback.
