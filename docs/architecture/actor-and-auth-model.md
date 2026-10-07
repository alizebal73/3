# Actor, Identity and Authorization Model

Identity types are separate by design.

## Human Actor
Owner, Admin and Operator authenticate to the Server and receive a server-issued identity.

## Device Actor
Each Agent machine has a durable DeviceId and revocable credentials. IP is never identity.

## Customer Login
A customer login is an authenticated use of a customer identity on one Agent Device. It is not the same as a human user or realtime connection.

## Session Ownership
A Session is owned by the Server and binds Station + Agent Device + CustomerLogin under one authoritative transaction.

## Connection
A realtime ConnectionId is ephemeral and can change after reconnect. It cannot be durable identity, authorization or ownership.

## Authorization sequence
Authenticated actor -> server permission -> target state/ownership -> invariant checks -> transaction -> audit -> post-commit publication.

Desktop may display permissions but never becomes the source of authorization truth.

## Approval
High-risk actions may require a separate approving actor. Approval records are durable and audited.
