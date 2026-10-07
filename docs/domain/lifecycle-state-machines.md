# Foundation State Machines

These states define the allowed shape of future business implementation. No handler may invent a transition ad hoc.

## Agent
Unpaired -> Paired -> Online -> Reconnecting -> Offline -> Revoked.
Pairing and revocation are authenticated Server operations.

## Station
Provisioning -> Available -> InUse -> Paused/Unavailable -> Maintenance -> Available.
A station cannot enter InUse unless the Server has atomically assigned a valid Session.

## CustomerLogin
Created -> Authenticated -> Active -> Locked/Ended.
CustomerLogin cannot outlive its authoritative Agent binding.

## Session
Created -> Active -> Paused -> Active -> Transferring -> Active -> Ending -> Settled.
Terminal state: Cancelled/Ended.
Only the Server may transition a Session. Transfer changes Station/Agent ownership atomically.

## Payment
Initiated -> Processing -> Completed or Failed/Expired.
A retry with the same idempotency key returns the original authoritative result.

## InventoryMovement
Planned -> Committed -> Reversed.
A reversal references the original movement and restores the original StockArea when provenance exists.

## Approval
Requested -> Approved or Rejected -> Executed.
Approval is single-use and bound to a specific operation/idempotency key.

## Backup
Requested -> Running -> Verified -> Retained, or Failed.
A backup is not considered valid until verification succeeds.
