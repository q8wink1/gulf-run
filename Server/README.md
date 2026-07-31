# Server

Authoritative backend services for GulfRun.

**Status:** Scaffolding only. Implementations begin at **M1–M3** per phases.

**Owner:** Server Lead

## Layout

| Path | Role |
|------|------|
| `gateway/` | Edge API / BFF |
| `services/*` | Domain services |
| `workers/` | Async processors |
| `libs/` | Internal shared libraries |
| `tests/` | Cross-service tests |

See [Multiplayer Architecture](../docs/02-architecture/MULTIPLAYER_ARCHITECTURE.md) and [Scalability Plan](../docs/02-architecture/SCALABILITY_PLAN.md).

## Rules

- Domain services own their data
- Economy / inventory mutations via ledger + idempotent commands
- No shared “god” production database credentials across services
