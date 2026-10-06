# Session invariants

A Station may have at most one active Session.
A CustomerLogin may not be assigned to incompatible concurrent Sessions.
A Session cannot move to an unavailable Station.
A transfer must move Agent/identity ownership together with Station ownership.
Transfer must be atomic under concurrency.
