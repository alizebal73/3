# Repo 2 → Repo 3 UI/UX Lessons and Anti-Regression Contract

## Purpose

This document records the UI/UX lessons extracted from GameNet Manager 2 so Repo 3 does not repeat the same structural mistakes.

Repo 2 is a **reference and lessons source only**. Repo 3 must not copy its implementation, runtime, framework or accidental coupling.

## 1. What Repo 2 got right and should be preserved conceptually

Repo 2 eventually established useful operator patterns:

- Dashboard as the daily operations center.
- Station-centric live cards.
- Card/Compact/List views.
- Status/VIP/Remaining Time/Internet grouping.
- Customer master-detail layout.
- Buffet product grid + cart.
- Reports with common date range.
- Users + permissions + shift.
- Settings grouped by domain.
- Right-click Station/Client context menus.
- Command palette / keyboard-first direction.
- F1/context help direction.
- Persian RTL terminology.
- Explicit offline/last-known-state treatment.
- Dense operator mode.
- Accessibility and reduced-motion considerations.
- Server-backed settings rather than pretending local UI state is authoritative.

These patterns are product lessons, not a mandate to reproduce the old React/browser implementation.

## 2. Repo 2 problems that Repo 3 must prevent

### UI/runtime boundary
Repo 2 accumulated Browser Preview, Dashboard and prototype artifacts. This created a second runtime and confusion about which UI was the real product.

**Repo 3 rule:** WPF Desktop executable is the only operator runtime. Any prototype/preview is disposable design/test tooling and cannot become an alternate production path.

### UI-first / backend-late drift
Repo 2 repeatedly exposed cases where UI expected fields or behavior before Server/DTO/migration ownership was authoritative.

Examples included:
- UI reading Network while Server did not yet own/return it.
- settings appearing local-only before Server-backed persistence.
- DTO/endpoint mismatch.
- mock behavior surviving too long.

**Repo 3 rule:** every UI capability is traceable to a Server contract, owner, persistence/invariant and acceptance test before implementation is called complete.

### Information architecture drift
Repo 2 had a temporary Operations/Management hub and later migrated features to their true owner pages.

**Repo 3 rule:** no permanent catch-all Management page. Every capability has one canonical navigation owner from the start.

### Settings fragmentation
Repo 2 required a later Settings IA hardening pass to group categories and add navigation/search.

**Repo 3 rule:** Settings taxonomy is designed before implementation, with category ownership, authority, validation, permission and audit defined.

### Dashboard overload
Repo 2 needed later hardening for dense cards, grouping, responsive workspace and last-known-state semantics.

**Repo 3 rule:** Dashboard is designed around operator tasks, with deliberate Card/Compact/List modes, grouping/filtering and controlled density from the beginning.

### Network field drift
Repo 2 had UI support for Station.Network before Server authoritative support was complete.

**Repo 3 rule:** Network assignment is Server-owned; UI never invents a station/network field.

### Accessibility came late
Repo 2 added skip navigation, aria/current state, semantic connection states and reduced-motion later.

**Repo 3 rule:** keyboard navigation, focus, semantic status, reduced motion, DPI/font scaling and accessible commands are part of the UI foundation, not polish.

### Right-click and keyboard paths were late/fragile
Repo 2 eventually accumulated Dashboard/Client context menus and keyboard/command behavior.

**Repo 3 rule:** every high-frequency Station/Client action must define:
- primary button path
- double-click path if appropriate
- right-click path
- keyboard/command path
- confirmation path
- permission behavior
- error/recovery behavior

### Responsive/dense behavior was patched late
Repo 2 required a later responsive hardening pass.

**Repo 3 rule:** 1366×768, dense operator mode, normal mode, DPI/font scaling and large-data scrolling are acceptance criteria from the first UI implementation.

### Settings authority was easy to misrepresent
Repo 2 had local-only settings that needed explicit UI disclosure before Server-backed Settings was completed.

**Repo 3 rule:** every setting declares Local / Server / Hybrid authority and effective scope.

### Reports required late correction
Repo 2 needed later report IA and overflow corrections.

**Repo 3 rule:** common date-range control, report-specific filters, summary + detail table, export permission and controlled inner scrolling are part of the Report Center contract.

### Fake capability risk
Repo 2 had mock/placeholder paths during development.

**Repo 3 rule:** a disabled/deferred capability must be visibly marked as unavailable/not implemented; it must never appear to perform a real action.

## 3. UI elements that must exist in Repo 3 design contract

- Dashboard
- Station Card/Compact/List
- Station right-click menu
- Client right-click menu
- Customer master-detail
- Session workspace
- Billing/settlement modal
- Buffet cart
- Report Center
- Users/Permission/Shift
- Cash Register
- Settings navigation/search
- Notification Center
- Command Palette
- F1/context help
- Global Search
- Loading/Empty/Error/Offline/Stale/Permission states
- Data Tables for comparison-heavy data
- Info Panels for Profile/Session/Tariff/Shift
- Summary Cards for KPIs
- Controlled drawers/modals for complex operations

## 4. Anti-regression gates before any Business UI is accepted

For every screen:
1. It has a canonical owner in Navigation.
2. It has a defined operator job-to-be-done.
3. It has Server contract/data owner.
4. It has permission model.
5. It has error/offline/stale state.
6. It has keyboard path where operationally relevant.
7. It has context action where operationally relevant.
8. It respects fa-IR RTL and en-US LTR.
9. It works at 1366×768 without uncontrolled page overflow.
10. It uses real Server data when the feature is declared implemented.
11. It has no hidden MockService.
12. It does not create a second accounting/authority model.
13. Its sensitive mutations are audited.
14. Its destructive/reversible actions have confirmation/reverse semantics.
15. Its UI test covers the critical workflow.

## 5. Specific lessons from Repo 2 UI that are intentionally NOT copied

- React/Vue/browser Dashboard runtime.
- Prototype-as-contract implementation coupling.
- Mock DTOs treated as a permanent transport contract.
- OperationsPage as a long-lived feature destination.
- Giant CSS/App file growth.
- UI patching ahead of Server authority.
- Late discovery of missing settings categories.
- Late responsive/accessibility hardening.
- Browser Preview as a production-like validation environment.

## 6. Repo 3 UI acceptance principle

The final Desktop must feel like one coherent application, not a collection of feature pages.

Operator mental model:

**Dashboard → Station/Customer → Session → Money → Client → Audit**

Manager mental model:

**Customers → Tariffs/VIP → Buffet/Inventory → Users/Shift → Reports**

Owner mental model:

**Finance → Reports → Security/Permissions → Settings → Backup/Recovery → Release**

All three use the same Server authority and data model.
