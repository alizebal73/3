# GameNet Manager 3 — Pre-Coding Master Roadmap

این فایل نقشه مرجع پیش از ورود به Business Implementation است.
این سند باید با معماری، نقشه قابلیت‌ها، ماتریس ردیابی نیازمندی‌ها، Definition of Ready/Done، Foundation Closure Matrix و Foundation Stabilization Action Matrix هم‌راستا بماند.

## 1. قانون شروع ساخت

تا وقتی Foundation Certification طبق شواهد واقعی Windows/PostgreSQL/Agent/Desktop تکمیل نشده، هیچ قابلیت تجاری جدیدی وارد پیاده‌سازی نمی‌شود.

وجود فایل، README، interface، guard یا تست واحد به‌تنهایی «certified» محسوب نمی‌شود.

زنجیره اجباری هر قابلیت:

Problem → Requirement → Operator outcome → Acceptance → Ownership/Invariants → Use Case → Transaction → Contract → Permission → Audit → Failure/Retry → Recovery/Reconciliation → Tests → Observability → Release/Rollback Evidence

---

## 2. درس‌های استخراج‌شده از Repo 2 — 27 مورد

این 27 درس، معیارهای مهندسی GameNet 3 هستند و نباید در مراحل بعدی حذف یا نادیده گرفته شوند.

| ID | درس Repo 2 | پاسخ اجباری در Repo 3 |
|---|---|---|
| L01 | چند مرجع برای یک DTO/مدل ایجاد شد | Shared V1 مرجع قرارداد انتقال است؛ کلاینت‌ها DTO مستقل اختراع نمی‌کنند |
| L02 | UI مرورگر به Runtime دوم تبدیل شد | Desktop بومی WPF تنها سطح اپراتوری است؛ browser artifact ممنوع |
| L03 | Program.cs بیش از حد بزرگ شد | Program فقط Composition؛ منطق داخل Module/Application |
| L04 | Workflow CI به یک برنامه دوم تبدیل شد | یک verify.ps1 مرجع + workflow نازک self-hosted |
| L05 | migrationها هم‌زمان با featureها تغییر کردند | migration review، bundle/SQL، clean/upgrade verification |
| L06 | Server در startup مسئول migration شد | Production migration خارج از startup و در deployment boundary |
| L07 | مالکیت Session دیر تعریف شد | Session/Station/Device ownership از Foundation با invariant و fencing تعریف شده |
| L08 | Agent ownership با reconnect دچار race شد | DeviceId پایدار + ConnectionId موقت + Lease/Fencing |
| L09 | mock و Server از هم فاصله گرفتند | Server source of truth + explicit fake boundary + integration/contract tests |
| L10 | Preview مسیر اجرای دوم شد | Runtime واقعی همان Desktop executable است؛ preview جای محصول نیست |
| L11 | Installer با internals برنامه coupling پیدا کرد | Server/Desktop/Agent نصب مستقل + manifest + compatibility |
| L12 | payment/free-credit semantics دیر اصلاح شد | Money/ledger/discount/bonus از قبل rule-based و Server-authoritative |
| L13 | فایل‌ها مسئولیت‌های متعدد گرفتند | module/layer boundaries + source-size guard |
| L14 | schema و transport model هم‌زمان drift کردند | Shared contract authority + migration compatibility |
| L15 | authentication با authorization مخلوط می‌شد | Actor identity جدا از permission policy |
| L16 | IP به‌عنوان هویت Device قابل اتکا نبود | DeviceId هویت است؛ IP فقط اتصال است |
| L17 | permission در UI قابل دست‌کاری بود | authorization فقط Server-side؛ UI صرفاً state را نمایش می‌دهد |
| L18 | money با display state مخلوط می‌شد | immutable ledger و تراکنش/Idempotency |
| L19 | زمان در نقاط مختلف متفاوت محاسبه می‌شد | TimeProvider/IGameClock + business timezone |
| L20 | side effect داخل transaction قرار گرفت | Outbox و post-commit dispatch |
| L21 | retry بدون idempotency باعث duplicate شد | operation/idempotency ownership و retry-safe rules |
| L22 | audit بعداً اضافه می‌شد | audit در mutation design و در همان transaction |
| L23 | recovery بیشتر مستنداتی بود تا قابل اثبات | backup/restore/reconnect/update rollback smoke |
| L24 | configuration و secret lifecycle پراکنده شد | validated Server configuration + protected credential lifecycle |
| L25 | observability بعد از خطا اضافه می‌شد | correlation/operation IDs + structured diagnostics + health |
| L26 | security/supply-chain دیر وارد چرخه شد | threat model + permission review + dependency pinning + SBOM |
| L27 | توسعه feature-first باعث rework شد | Foundation gate → vertical slice؛ DoR/DoD و traceability اجباری |

---

## 3. دوازده محور ساختاری اجباری

تمام ساخت محصول باید زیر این 12 محور قابل ردیابی باشد:

1. Repository/Tooling — SDK، package pinning، solution و canonical verify
2. Product Governance — charter، scope، requirements، DoR/DoD، roadmap و change control
3. Architecture — topology، ownership، module boundaries، ADR و fitness guards
4. Contracts — Shared V1، API envelope، Agent protocol، compatibility/versioning
5. Persistence — PostgreSQL، migrations، constraints، transactions، concurrency
6. Reliability — idempotency، outbox، fencing، retry، reconciliation، recovery
7. Security — actor identity، auth، permission، credential lifecycle، threat model، supply chain
8. Desktop/UX — WPF shell، navigation، commands، localization، RTL/LTR، states، accessibility
9. Agent Platform — DeviceId، pairing، SignalR، lease، heartbeat، command acknowledgement
10. Observability/Support — health، correlation، diagnostics، version/build evidence
11. Delivery/Recovery — installer، manifest، checksum/signing، local update، rollback، backup/restore
12. Quality/Scale — unit/integration/contract/E2E، concurrency، load envelope، regression gates

هیچ feature تجاری نباید خارج از این 12 محور طراحی یا پیاده‌سازی شود.

---

## 4. Foundation stages

### F0 — Toolchain & Repository
بسته‌شده در ساختار Repository:
- pinned SDK/tooling
- central package versions
- GameNet.slnx
- canonical scripts/verify.ps1
- یک workflow self-hosted
- PowerShell parser/safety guard

### F1 — Product & Traceability
بسته‌شده در اسناد:
- Product Charter
- capability map
- requirements traceability
- Definition of Ready / Done
- risk register
- assumptions/open-decisions
- product validation
- change control

### F2 — Architecture & Domain Boundaries
بسته‌شده:
- runtime topology
- data ownership
- actor/auth model
- module boundary manifest
- state machines
- transaction rules
- source-size and architecture guards
- ADR baseline

### F3 — Contracts & Compatibility
بسته‌شده ساختاری:
- Shared V1 API envelope/errors/headers
- Agent connection/command/state/recovery contracts
- release compatibility
- versioning rules
- contract tests
- duplicate DTO prohibition

### F4 — Persistence & Transaction Foundation
ساختار و implementation ایجاد شده؛ certification محیط واقعی هنوز الزامی است:
- PostgreSQL
- migrations
- naming convention
- transaction coordinator
- idempotency
- append-only audit
- outbox
- concurrency/fencing

### F5 — Security Foundation
ساختار و implementation ایجاد شده؛ runtime proof هنوز الزامی است:
- JWT validation
- permissions
- current actor
- Agent credential provisioning/rotation/revocation
- DPAPI storage
- threat/design review
- supply-chain guard
- SBOM release path

### F6 — Observability
- health/readiness
- correlation IDs
- exception handling
- diagnostics/support policy
- build/version reporting
- metrics/logging policy

### F7 — Agent Platform Proof
قبل از Business implementation باید روی Windows واقعی ثابت شود:
- durable DeviceId
- authenticated SignalR
- one authoritative lease
- heartbeat
- stale lease rejection
- reconnect fencing
- Server restart + Agent reconnect
- command expiry/acknowledgement
- credential rotation/revocation
- no duplicate authoritative command after network loss

### F8 — Desktop Platform Proof
باید روی Windows واقعی ثابت شود:
- native WPF launch
- no browser dependency
- central navigation
- busy/ready/empty/error/offline/permission states
- fa-IR RTL
- en-US LTR
- keyboard/global command path
- Desktop ↔ Server smoke

### F9 — Deployment & Update Proof
باید روی target Windows ثابت شود:
- arbitrary install path
- independent Server/Desktop/Agent installation
- Windows services
- manifest/checksum
- migration compatibility
- local update folder
- side-by-side staging
- health check
- rollback

### F10 — Backup & Recovery Proof
- verified PostgreSQL backup
- isolated restore
- Server restart
- DB interruption
- Agent restart/reconnect
- update rollback
- operator recovery status

### F11 — Quality & Scale Proof
- normal Release build/test
- all normal test projects in solution
- certification tests خارج از normal gate
- concurrency tests
- load scenarios Tier S/M baseline
- regression guard
- no placeholder tests

### F12 — Foundation Certification
خروجی اجباری:
- exact commit SHA
- local verify result
- Desktop evidence
- PostgreSQL evidence
- Agent evidence
- deployment/update evidence
- recovery evidence
- SBOM/release evidence
- no undocumented architecture exception

### F13 — First Business Vertical Slice
فقط بعد از F0–F12:

Station → Customer → Agent → Session

و هر مرحله از Business با DoR/DoD و زنجیره کامل traceability اجرا می‌شود.

---

## 5. First usable product sequence

Authentication/Permissions
→ Customer selection
→ Station selection
→ Tariff/VIP resolution
→ Client/Game readiness
→ Start Session
→ Agent/Station state
→ Pause/Resume/Transfer
→ End/Settlement
→ Payment/Debt/Wallet
→ Audit
→ Recovery

سپس:
Inventory/Buffet → Reports → Settings → Approvals → Backup/Support → Games/GameAccounts/ClientControl expansion

ترتیب دقیق sliceها می‌تواند با dependency تغییر کند، ولی ownership/invariants/authority نباید تغییر پنهانی کند.

---

## 6. Business modules reserved by Foundation

The following module boundaries are already established but remain intentionally business-deferred until Foundation certification:

Agents, Approvals, Auth, Backup, Billing, Buffet, ClientControl, Customers, GameAccounts, Games, Inventory, Reports, Sessions, Settings, Stations, Tariffs, Users, Vip, Wallet.

Business module folders may not accumulate implementation before certification.

---

## 7. Certification truth

Statusها باید بین این سطوح تفکیک شوند:

Designed
→ Implemented
→ Tested
→ Verified
→ Runtime Certified
→ Released

Structural green ≠ runtime certified.

The authoritative completion record is:
- docs/architecture/foundation-closure-matrix.md
- docs/roadmap/01-foundation-stabilization.md
- docs/operations/foundation-local-certification.md
- exact certified commit/evidence record

Until those agree, the product is not considered ready for business construction.
