# UI Foundation

## Purpose

The Desktop is a native operator application. Business features plug into one shell rather than creating independent windows or their own navigation/state patterns.

## Fixed shell responsibilities

- navigation catalog and selection;
- current operator/session connection status;
- global refresh and language commands;
- busy/loading state;
- offline/error presentation;
- confirmation/dialog policy;
- keyboard command registration;
- localization and RTL/LTR direction;
- consistent theme, spacing, typography and control styles;
- accessibility baseline;
- feature placeholder/content host.

## UI project boundaries

```text
Desktop/
  Infrastructure/Hosting/
  Api/
  Localization/
  UI/
    Commands/
    Navigation/
    Services/
    Shell/
    State/
  Resources/
    Styles/
    Languages/
  Features/
    <BusinessArea>/
```

`Features/<BusinessArea>` may contain presentation models, views and feature-specific commands only after the Foundation gate. It may not introduce a second shell, second API client, second state authority or direct database access.

## State model

Every screen must represent at least:
- Loading/Busy;
- Ready/Online;
- Empty when the authoritative query has no records;
- Error with a correlation/operation reference when applicable;
- Offline/Disconnected when Server authority is unavailable;
- Permission denied when authorization rejects the action.

The UI does not fabricate business state while offline.

## Navigation

Navigation items are defined centrally. Feature pages do not mutate the global navigation catalog ad hoc.

Navigation must remain stable across fa-IR and en-US. Only presentation text and direction change.

## Commands

Global commands are registered once. Keyboard shortcuts must invoke the same command as a button/menu action; there may not be two implementations for the same operator action.

## Design system

Theme.xaml is the single source for the baseline palette, brushes, radii and base window/text styles. Controls.xaml owns shared Button/Card/toolbar/navigation styles. New business features must reuse those resources instead of inventing local design tokens.

## Accessibility baseline

- keyboard navigation is mandatory for operator-critical flows;
- focus order follows the visual/semantic workflow;
- state is not communicated by color alone;
- controls expose AutomationProperties names where the visible label is insufficient;
- text must remain readable when Windows display scaling is increased;
- destructive operations require an explicit confirmation step.

## Acceptance before business UI

The UI foundation is considered implemented when:
1. the native WPF executable launches without Server availability;
2. navigation renders from the central catalog;
3. fa-IR and en-US switch direction and labels through the shared localization service;
4. offline/error/busy state uses the central state service;
5. global commands use one command implementation;
6. shared resources are loaded application-wide;
7. no feature folder contains a second shell or alternate API authority.
## Automated guard

`scripts/check-ui-foundation.ps1` validates required WPF dictionaries, DynamicResource keys in the shell, bilingual resource coverage, Navigation resource keys, and the prohibition on Desktop feature database/server implementation references.
