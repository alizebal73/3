# Initial Permission Matrix

Authorization is server-side. The Desktop only renders the permission state returned by the Server.

| Area | Owner | Admin | Operator | Agent | Customer |
|---|---|---|---|---|---|
| Dashboard/read-only views | yes | yes | yes | no | no |
| Stations/status | yes | yes | yes | machine actions only | own login only |
| Customer management | yes | yes | limited by policy | no | own profile only |
| Session control | yes | yes | yes | execute authorized command | own session scope |
| Tariffs | yes | yes | no by default | no | no |
| Wallet/payments | yes | yes | policy/approval | no | own wallet operations only |
| Discounts/free credit | yes | policy | approval required | no | no |
| Inventory | yes | yes | policy | no | no |
| Buffet sales | yes | yes | yes | no | no |
| Users/permissions | yes | policy | no | no | no |
| Security/pairing | yes | policy | no | own device protocol only | no |
| Backup/restore | yes | policy | no | no | no |
| Reports/audit | yes | yes | read policy | no | no |

Sensitive actions such as payment reversal, manual debt change, permission change, tariff change, destructive restore and audit-impacting operations require explicit permissions and may require owner approval.

The exact permission constants are added to Shared V1 before each vertical slice. No UI is allowed to invent permission names.
