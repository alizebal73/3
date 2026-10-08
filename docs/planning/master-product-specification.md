# GameNet Manager 3 — Master Product Specification & UI Contract

> وضعیت: Pre-Coding Product Lock
>
> این سند قرارداد محصول قبل از شروع Business UI/Coding است. Foundation می‌تواند بدون اجرای Business تغییر کند، اما هیچ قابلیت تجاری یا صفحه UI نباید خارج از این قرارداد وارد محصول شود.
>
> مرجع محصول نهایی: این سند + `docs/planning/product-capability-map.md` + `docs/planning/requirements-traceability.md`.
>
> اصل: **هیچ صفحه‌ای به‌تنهایی یک قابلیت محسوب نمی‌شود.** قابلیت زمانی کامل است که Operator Workflow، Server Authority، Persistence، Contract، Permission، Audit، Failure/Retry، Recovery، Tests و Release Evidence داشته باشد.

## 1. هدف محصول

GameNet Manager 3 یک سیستم مدیریت کامل GameNet/CyberCafe است که عملیات روزانه، مشتری، Station، Session، Agent، بازی، تعرفه، VIP، کیف پول، پرداخت، بدهی، Buffet/Inventory، صندوق، شیفت، کاربران، Permission، گزارش، شبکه، سلامت، Backup و Update/Rollback را در یک سیستم واحد مدیریت می‌کند.

معماری نهایی:

- Native Windows WPF Desktop: سطح اصلی Operator/Admin
- Server/API: تنها منبع حقیقت و مالک Mutation/Authorization
- PostgreSQL: منبع پایدار داده
- Windows Agent: مجری دستورات روی PC و منبع مشاهده وضعیت Client
- SignalR: ارتباط real-time Agent
- Audit/Outbox/Idempotency/Fencing/Lease: زیرساخت قابلیت اطمینان
- Browser/Preview: هرگز Runtime اصلی محصول نیست

اصل سه‌لایه:
**Desktop تصمیم/فرمان می‌دهد → Server اعتبارسنجی و مالکیت را اعمال می‌کند → Agent اجرا و نتیجه را گزارش می‌کند.**

---

## 2. کاربران و نقش‌ها

### Owner
کنترل کامل کسب‌وکار، مالی، تنظیمات، امنیت، کاربران، Recovery و Release.

### Manager/Admin
مدیریت عملیاتی/مالی/تعرفه/مشتری/بوفه طبق Permission.

### Operator
عملیات روزانه GameNet طبق Permission:
Session، Station، Customer، Buffet، دریافت وجه، گزارش شیفت و کنترل‌های مجاز.

### Customer
حساب مشتری، PIN/identity، Wallet/Debt، VIP، Session و در صورت فعال بودن Client Shell.

Permission واقعی همیشه Server-side است؛ UI فقط وضعیت مجاز/غیرمجاز را نمایش می‌دهد.

---

## 3. Information Architecture و Navigation

Navigation اصلی Desktop:

1. داشبورد
2. مشتریان
3. بوفه و موجودی
4. تعرفه‌ها و VIP
5. بازی‌ها
6. کلاینت‌ها / Stationها
7. اکانت‌ها
8. گزارش‌ها
9. کاربران و شیفت
10. صندوق و مالی
11. رزرو و صف (در صورت نیاز می‌تواند زیر Dashboard/Operations باشد، مقصد مالک مشخص باشد)
12. تنظیمات
13. سلامت/پشتیبانی (برای Admin/Owner)

قانون:
- Dashboard مرکز عملیات پرتکرار است.
- صفحات مدیریتی محل CRUD و تنظیمات کم‌تکرارند.
- «مدیریت» منوی مبهم و سطل قابلیت‌ها نیست.
- هر قابلیت یک Owner Navigation دارد.
- قابلیت نباید هم‌زمان در چند منوی مستقل صاحب داشته باشد.
- Search/Command Palette باید برای عملیات سریع وجود داشته باشد.

---

## 4. Dashboard / Control Center

Dashboard باید Station-centric و عملیات‌محور باشد.

### Station Card
نمایش پایه:
- شماره/نام
- وضعیت
- مشتری/username
- نام کامل
- زمان باقی‌مانده
- بدهی
- یادداشت کوتاه
- VIP/Normal
- Network 1/2
- Agent/connection health در صورت نیاز
- وضعیت رزرو/maintenance

قیمت ساعتی در کارت اصلی نمایش داده نمی‌شود؛ در Start/Session/Settlement نمایش داده می‌شود.

### View modes
- Card
- Compact/Dense
- List

### Group/Filter
- Status
- VIP/Normal
- Internet 1/2
- Remaining Time
- Station type
- Maintenance/Offline

Grouping فقط Presentation است و هرگز Server Truth را تغییر نمی‌دهد.

### عملیات سریع
- شروع Session
- تمدید
- Pause/Resume
- انتقال
- پایان و تسویه
- شارژ
- بدهی
- Buffet
- پروفایل مشتری
- کنترل Client
- Reservation/Queue
- Maintenance

### Context Menu
Right-click روی Station باید Permission-aware باشد و عملیات مرتبط را بدون خروج از Dashboard ارائه کند.

---

## 5. Station Management

Station generic است و فقط PC نیست.

Types:
- PC
- PS5/Console
- Foosball/Table
- Future station types

هر Station:
- ID
- شماره/نام
- Type
- Zone/Group
- Tariff policy
- Network profile
- Agent binding در صورت وجود
- status
- maintenance
- notes
- health/last seen
- capabilities

Statusهای استاندارد:
Free, Busy, Reserved, Maintenance, Offline, Disabled.

PC:
Agent + software control.

PS5:
Session/time/control محدودتر و وابسته به capability.

Foosball:
Timer/Session بدون Agent.

---

## 6. Customers

Customer profile:
- Customer ID/code
- نام
- موبایل
- PIN/identity
- username در صورت نیاز
- Wallet
- Debt
- Free Money
- Free Time
- VIP
- package
- discount policy
- notes
- history
- status

Customer Center باید Master/Detail داشته باشد:
لیست + Search/Filter + Profile + financial summary + session/history.

---

## 7. Wallet / Ledger / Billing / Money

Wallet و Cash Register موجودیت‌های جدا هستند.

### Wallet/Ledger
- Top-up
- Debit
- Credit
- Free Money
- Debt
- Settlement
- Refund/Reverse
- history

تمام مبالغ در واحد canonical تومان و بدون float نگهداری می‌شوند.

### Free Time ≠ Free Money
- Free Time فقط زمان billable را کاهش می‌دهد.
- Free Money اعتبار مالی جداست.
- هر دو history و consumption مستقل دارند.

### Payment
Payment methods قابل تنظیم:
- Cash
- Card/POS
- Transfer
- Wallet
- ترکیبی در صورت نیاز

Payment integration خارجی در Foundation لازم نیست، اما مدل باید Provider-ready باشد.

### Pricing semantics
محاسبه پایه، تخفیف، Free Credit، rounding و Final Amount جدا ثبت شوند.

---

## 8. Cash Register

صندوق GameNet از Wallet مشتری جداست.

قابلیت‌ها:
- Opening cash
- Cash sales
- Card/POS sales
- Transfers
- Expenses
- Refunds
- Manual adjustment با دلیل
- Closing cash
- Expected vs actual
- Difference
- reconciliation
- Audit

---

## 9. Shifts / Staff

Shift:
- شروع
- Start Cash
- فروش
- پرداخت‌ها
- Refund
- Expense
- Cash End
- Difference
- تحویل شیفت
- توضیح تطبیق

Staff profile:
- نوع حقوق
- نرخ ساعتی
- حقوق ماهانه
- ساعت کاری
- اضافه‌کاری
- پاداش
- کسری
- مساعده
- حساب طلب/بدهی پرسنل
- پرداخت حقوق

Payroll از Shift جداست ولی با آن قابل ردیابی است.

---

## 10. Sessions

Session generic و Station-based است.

Lifecycle:
Reserved → Started → Active → Paused → Resumed → Transferred/Ended → Settled.

قابلیت:
- Start
- Pause
- Resume
- Extend
- Reduce
- Change participants
- Change tariff طبق policy
- Transfer
- End
- Settlement
- Payment/Debt
- Receipt
- Recovery/Reconciliation

Session باید مالکیت روشن Customer + Station + Tariff + Agent capability داشته باشد.

---

## 11. Reservations / Queue

Reservation:
- Customer
- Station/Type
- Start/End
- Tariff
- Status
- cancel/no-show
- extension

Queue:
- customer
- requested station type
- priority/policy
- joined time
- notification
- assignment
- cancellation

این دو باید به Dashboard متصل باشند، نه اینکه اپراتور را از عملیات زنده جدا کنند.

---

## 12. Tariffs

Tariff قابل تنظیم:
- Station Type
- Normal/VIP
- price
- time unit
- minimum duration
- rounding
- schedule/time band
- holiday/weekend policy در صورت نیاز
- night pricing
- participant policy
- overflow policy

Session باید Pricing Snapshot مناسب داشته باشد تا تغییر آینده تعرفه Session قبلی را خراب نکند.

---

## 13. VIP

VIP Plan:
- title
- price
- duration
- daily hour cap
- overflow rule
- eligible stations/tariffs
- buffet discount
- perks
- start/end
- remaining days
- daily consumption

VIP فقط badge نیست؛ entitlement و consumption دارد.

---

## 14. Games / Game Accounts

Game Catalog:
- name
- executable/App ID
- launch policy
- station compatibility
- version/status

Game Account Pool:
- provider
- account
- allocation
- state
- current Station/Session
- audit/security

Secrets نباید خام در UI/DB پخش شوند.

---

## 15. ClientControl / Agent

Agent روی PC:
- durable DeviceId
- authenticated transport
- heartbeat
- lease
- reconnect
- reconciliation
- command acknowledgement
- credential lifecycle
- local protected storage

Server authority:
- Launch
- Stop
- Lock/Unlock
- Logout
- Restart
- Shutdown
- Message
- Screenshot/diagnostics در صورت policy
- Internet switch 1/2 در صورت network integration
- Maintenance
- Client policy/config
- Update

هیچ Client نمی‌تواند با تغییر UI/Request روی Device دیگر command اجرا کند.

ConnectionId موقت است؛ DeviceId هویت پایدار است.

---

## 16. Buffet / Inventory

### Products
- name
- category
- sell price
- buy price
- unit
- stock
- minimum stock
- active

### Inventory
- purchase/in
- sale/out
- adjustment
- waste
- return
- stock count
- provenance
- low-stock alert

### Buffet
- quick sale
- sale to active Session
- sale to Customer account
- standalone sale
- return/reversal
- cart
- payment/debt/wallet
- receipt

Inventory mutation باید atomic باشد.

### Suppliers
- supplier
- purchase
- invoice
- purchase price
- payable
- payment

---

## 17. Maintenance / Assets

Maintenance:
- reason
- severity
- reported by
- assigned technician
- parts
- cost
- status
- start/end
- return to service

Assets may include:
PC, Console, Controller, TV, Headset, Keyboard/Mouse, Printer, POS, Router, Switch, AP, Server.

---

## 18. Network

Network model:
- Internet 1
- Internet 2
- Gateway/profile
- DNS/policy
- Station assignment
- exceptions
- health/diagnostics

GameNet Manager owns network policy/assignment visibility. Router automation is an explicit integration boundary, not hidden router logic inside the UI.

---

## 19. Reports

Report Center:
- Financial
- Sessions/Stations
- Customers/VIP
- Buffet/Inventory
- Users/Shift/Payroll
- Audit
- Expenses
- Profitability

Common date control:
Today, Yesterday, 7 days, 30 days, 6 months, Year, Custom date/time.

Large tables use controlled inner scrolling; whole window must not overflow horizontally at 1366×768.

Reports are read-only unless an explicitly defined command is present.

Export:
CSV/Excel/PDF only where requirement/permission allows.

---

## 20. Audit / Approvals

Audit:
- actor
- operation
- target
- before/after where appropriate
- timestamp
- operation/correlation ID
- result
- reason

Append-only.

Approval:
- sensitive action
- requester
- approver
- separation of duties
- decision
- reason
- expiry if needed
- execution linked to approval

Examples:
large discount, refund, destructive adjustment, sensitive permission change.

---

## 21. Notifications

Notification Center:
- critical/warning/info
- read/unread
- timestamp
- source
- target
- action/deep link
- deduplication
- retry semantics

Examples:
Agent offline, low stock, backup failure, Session ending, debt, health failure.

---

## 22. Settings

Settings must be categorized, searchable and permission-aware.

Required groups:
1. GameNet/General
2. Display/Layout
3. Localization/Time/Currency
4. Session/Settlement/Rounding
5. Tariffs/VIP
6. Billing/Payment
7. Alerts/Sound/Popup
8. Stations
9. Network/Internet 1-2
10. Client Policy/WOL/Agent
11. Games/Game Accounts
12. Buffet/Inventory
13. Users/Roles/Permissions
14. Shifts/Payroll
15. Security
16. Backup/Recovery
17. Printing/Receipts
18. Updates/Release
19. Notifications
20. Keyboard/Hotkeys
21. Appearance/Accessibility
22. Advanced/Diagnostics

Every setting must define:
- owner
- type
- default
- validation
- scope
- who may change it
- effective time
- persistence
- audit requirement
- restart requirement if any
- local vs server authority

No fake local setting may be presented as Server-authoritative.

---

## 23. Printing

Printer subsystem:
- printer selection
- thermal/A4
- paper size
- copies
- auto print
- preview
- GameNet header/logo/contact
- receipt numbering
- payment/session/buffet/shift reports

---

## 24. Diagnostics / Support

Admin/Owner Diagnostic Center:
- Server
- DB
- API
- SignalR
- Agents
- Station
- Backup
- Update
- Network
- version/build
- last seen
- last error
- connectivity test

Support information must be useful without exposing secrets.

---

## 25. Backup / Recovery

- scheduled backup
- manual backup
- verification
- restore
- retention
- target
- status
- last successful
- failure alert

Baseline:
RPO ≤15 min, RTO ≤60 min, 30-day verified backup retention, subject to real measurement/evidence.

---

## 26. Update / Rollback

Release:
- package manifest
- compatibility
- checksum
- signing
- staging
- health check
- update
- rollback

Initial deployment supports arbitrary install paths.

Server/Desktop/Agent installation boundaries remain explicit.

---

## 27. Desktop UI/UX Contract

### Shell
- native WPF
- stable header
- navigation
- primary workspace
- context/action toolbar
- status/connection area
- notification center
- command palette
- keyboard shortcuts
- F1 help/context where useful

### Layout
- RTL-first fa-IR
- LTR en-US
- dense operator mode
- normal mode
- controlled resizing
- 1366×768 must remain usable
- larger/smaller DPI/font must remain usable
- no horizontal page overflow caused by normal reports/settings
- accessibility/reduced-motion/keyboard semantics

### Components
Shared:
- TextInput
- NumericInput
- MoneyInput
- DurationInput
- SearchInput
- Select
- Date/Time range
- DataTable
- EntityCard
- InfoPanel
- SummaryCard
- StatusPill
- Badge
- Modal
- ConfirmModal
- Drawer
- Toast
- EmptyState
- ErrorState
- OfflineState
- LoadingState

### Interaction
- double click for primary open action where appropriate
- right click context menu for Station/Client
- keyboard navigation
- command palette
- F1/context help
- drag/drop only where it improves a defined operation
- dangerous commands require confirmation and permission
- disabled/hidden controls must not create ambiguity

---

## 28. State UX

Every important screen must define:
- Loading
- Ready
- Empty
- Error
- Offline
- Stale/last-known
- Permission denied
- Saving
- Saved
- Conflict
- Recovery/retry

When Server is disconnected, UI must distinguish last valid snapshot from live data.

---

## 29. Product-wide invariants

1. Server is source of truth.
2. UI never owns authorization.
3. Money is ledger-based and idempotent.
4. Free Time and Free Money are separate.
5. Customer Wallet and Cash Register are separate.
6. DeviceId is identity; IP is connection metadata.
7. One authoritative Agent lease per Device.
8. Audit is append-only.
9. Sensitive mutations have permission + audit.
10. Reversible mutations have reverse/refund/cancel semantics.
11. Time uses one authoritative clock/timezone policy.
12. Filtering/grouping never mutates Server state.
13. Mock data can never masquerade as real data.
14. Browser preview is never production runtime.
15. Every critical operation has failure/retry/recovery behavior.
16. Settings declare authority/scope and are not secretly local.
17. Reports do not become a second accounting authority.
18. Business modules cannot bypass shared contracts.

---

## 30. First usable vertical slice

After Foundation Certification:

Authentication/Permission
→ Customer
→ Station
→ Tariff/VIP
→ Agent readiness
→ Start Session
→ Session lifecycle
→ Settlement
→ Wallet/Debt/Payment
→ Audit
→ Recovery

Then:
Buffet/Inventory
→ Reports
→ Users/Shift/Cash
→ Reservations/Queue
→ Games/GameAccounts/ClientControl expansion
→ advanced network/integrations.

---

## 31. Definition of Product Complete

A section is 100% complete only when:
- UI page/workflow exists
- all required inputs/outputs exist
- Server mutation/query exists
- persistence exists
- contract exists
- permission exists
- audit exists where required
- failure/retry exists
- recovery/reconciliation exists
- real data is used
- tests exist
- UI interaction is tested
- accessibility/localization is tested
- release/update impact is addressed
- documentation/traceability is updated

A green build alone is never Product Complete.
