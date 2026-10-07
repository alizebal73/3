# Station Foundation

GameNet supports multiple station categories under one Station aggregate.

Initial categories:
- PC
- PS5
- Foosball

A Station has stable identity and number, category, display name, availability/maintenance state and optional Agent Device binding.

The tariff selected for a Station is never inferred from UI text. The Server resolves pricing from the Station category, tariff version, customer/VIP eligibility and effective business time.

Agent health is distinct from Station availability. A healthy Agent does not automatically mean a Station is available if the Server marks it under maintenance.

Station transfer/release is a Server command with concurrency protection. The operator UI may select a station, but the final state is accepted only from the Server transaction.
