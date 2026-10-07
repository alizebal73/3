# Operator Workflow Foundation

The Desktop is an operator tool, not a collection of independent screens.

## Global shell rules

The shell must define once:
- navigation;
- current operator identity;
- connection/server status;
- notifications;
- global search where supported;
- command shortcuts;
- confirmation dialogs;
- busy/loading state;
- global error handling;
- language/direction;
- update/restart state.

Feature pages consume the shell. They do not redefine it.

## High-frequency workflows

Before implementation, model each workflow from start to finish:

1. Identify/choose customer.
2. Start a station/session.
3. Pause/resume/transfer session.
4. End and settle session.
5. Take payment or record debt.
6. Apply allowed discount/free credit.
7. Sell buffet inventory.
8. Handle return/reversal.
9. View station/device health.
10. Resolve Agent disconnect/reconnect.
11. Perform privileged action/approval.
12. Backup/restore/update.

For each workflow define:
- entry point;
- operator steps;
- allowed keyboard path;
- required permission;
- server request(s);
- expected state change;
- success feedback;
- validation error;
- conflict error;
- offline/disconnected behavior;
- audit impact;
- recovery path.

## UX invariants

- The UI never shows a locally invented authoritative value.
- A disabled command must explain whether it is a permission, state or connectivity restriction.
- Destructive financial/security actions require explicit confirmation and show the effect before commit.
- Busy indicators are tied to the actual request, not arbitrary delays.
- Errors preserve the correlation/operation context for support.
- Persian RTL and English LTR are tested as complete workflows, not just strings.

## Accessibility baseline

Keyboard navigation, focus order, readable status changes, non-color-only status indication and scalable text are required for operator-critical screens.
