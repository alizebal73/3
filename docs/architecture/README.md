# Architecture Baseline

GameNet 3 is a modular monolith with a native Windows desktop client.

## Runtime boundaries

### Server

src/Server

The authoritative application boundary. It owns:
- business rules and use cases;
- PostgreSQL persistence;
- authorization and actor identity;
- audit;
- idempotency;
- transaction coordination;
- durable Outbox;
- authoritative realtime/Agent decisions.

The Server is headless. It does not render the operator UI.

### Desktop

src/Desktop

GameNet.Manager.Desktop is the native Windows Operator/Admin application built with WPF on .NET 10.

It owns:
- shell/window/navigation;
- localization;
- presentation state;
- API client boundary;
- user interaction.

It does not own:
- business rules;
- database access;
- authoritative timers;
- money calculations;
- Session ownership;
- inventory truth.

### Agent

src/Client

The Windows Agent runs on client PCs and performs server-authorized machine actions.

### Shared

src/Shared

Contains only reusable platform primitives and versioned contracts. It must not become a dumping ground for business logic.

## Desktop/localization rule

The product UI is not a website and is not rendered in a browser. The desktop executable is the primary operator surface.

Supported UI cultures from Foundation:
- fa-IR — Persian, RTL;
- en-US — English, LTR.

UI text must be separated from executable code and stored in localizable resources. WPF supports this localization model and explicit FlowDirection handling.

## Feature module rule

Every business module starts with:
- Domain
- Application
- Infrastructure
- Api
- tests
- explicit contracts where cross-module collaboration is required.

No module gets business implementation during Foundation.
