# Games — Infrastructure

Foundation boundary only. Business implementation is added through a vertical slice after Foundation certification.

Rules:
- authoritative state remains on Server;
- cross-module access goes through approved contracts/application boundaries;
- persistence belongs only to Infrastructure;
- API exposes application use cases, not domain internals.
