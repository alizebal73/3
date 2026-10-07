# PostgreSQL Certification Tests

These tests intentionally require a real PostgreSQL database and are not part of the ordinary source/build gate.

Set GAMENET_TEST_DATABASE to a disposable certification database, apply migrations with scripts/certify-postgresql.ps1, then run this project.

The suite must never be changed to silently skip when PostgreSQL is unavailable.
