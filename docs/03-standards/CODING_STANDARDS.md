# Coding Standards

**Last updated:** 2026-07-31  
**Owner:** Engineering Manager + Principal Architect  
**Audience:** All engineers

**Requirements-level companion:** [TECHNICAL_ARCHITECTURE.md](../02-architecture/TECHNICAL_ARCHITECTURE.md) (P049 v1.0) is the official source of truth for architecture principles and code quality principles (SOLID, small functions, clear naming). This document remains the detailed engineering standard.

---

## 1. Normative language

MUST / SHOULD / MAY follow RFC 2119 meaning in this document.

## 2. Universal rules

1. **Server trusts nothing from the client** for inventory, currency, match outcomes, or purchases.
2. **No secrets** in source, scenes, ScriptableObjects, or Addressables catalogs.
3. **Fail closed** on security/economy paths; fail soft on cosmetic presentation.
4. **Feature flags** for post–Soft Launch player-facing changes.
5. **One PR, one purpose**; large refactors isolated from features.
6. **Tests travel with behavior** that encodes rules or contracts.
7. **Logs are structured** and contain no PII beyond approved identifiers (player id hashed/internal only as policy allows).

## 3. C# (Unity client)

### Language

- Nullable reference types **enabled**.
- `async`/`await` for I/O; do not block main thread.
- Prefer `readonly` / immutability for DTOs crossing boundaries.
- No public mutable static state for gameplay rules.

### Style

- Follow `.editorconfig` (introduced at M1); until then: Allman or K&R consistently per IDE defaults **team-locked at M1**.
- PascalCase types/methods; camelCase locals; `_camelCase` private fields.
- Interfaces prefixed with `I` (`IInventoryClient`).
- Async methods suffix `Async`.

### Architecture

- **Domain** code MUST NOT reference UnityEngine APIs when testing purity is required (prefer plain C#).
- **Presentation** MUST NOT call purchase/grant APIs except through application services.
- **Features** communicate via Domain interfaces / messages; no circular feature refs.
- Use cancellation tokens for all async feature flows.

### Unity-specific

- Hot path: avoid GC allocations in `Update` (profile-guided).
- Prefer Addressables over `Resources.Load`.
- `[SerializeField]` private fields; avoid public fields for wiring.
- Debug-only code under `GulfRun.Debug` asmdef / `DEVELOPMENT_BUILD` guards.
- IL2CPP release builds for shipping.

### Forbidden

- `GameObject.Find` in production hot paths
- Singletons as hidden service locators for economy
- Trusting `PlayerPrefs` for owned items
- Empty `catch` blocks

## 4. Server standards

### APIs

- Versioned routes or package versions (`/v1/...`).
- Idempotency keys on all grant/spend/purchase commands.
- Authn on all non-public endpoints; authz checked per resource.
- Explicit timeouts; bounded retries with jitter on egress.

### Data

- Migrations reviewed; expand/contract.
- Monetary/progress mutations via ledger pattern.
- Soft deletes where audit requires history.

### Code quality

- Unit tests for domain; contract tests for APIs.
- No shared “god” database login across services in production.
- Structured logging with correlation / trace ids (OpenTelemetry).

## 5. Shared contracts

- IDL is source of truth; generated code is not hand-edited.
- Breaking wire changes require version bump + compatibility window.
- Enums and reward reason codes shared via `Shared/constants`.

## 6. Error handling

| Layer | Behavior |
|-------|----------|
| Client UI | User-safe messages; detailed error codes logged |
| Client net | Retry transient; surface auth failures to re-login |
| Server | Typed error codes; never leak stack traces to clients in prod |

## 7. Testing expectations

| Layer | Expectation |
|-------|-------------|
| Domain rules | Unit tests required |
| API contracts | Contract or integration tests for critical paths |
| Unity | EditMode for pure logic; PlayMode smoke for slice |
| Economy | Simulation tests for grant/spend invariants |

Coverage vanity metrics are secondary to **critical path** tests.

## 8. Performance

- Respect budgets in [MOBILE_OPTIMIZATION.md](../04-engineering/MOBILE_OPTIMIZATION.md).
- New systems that add per-frame work MUST include profiler notes in PR.
- Server endpoints MUST declare expected QPS and p95 in tech briefs for Soft Launch+.

## 9. Code review bar

Reviewers check: authority model, naming, tests, telemetry, flags, secrets, budget impact, docs/ADR links.

## 10. Exceptions

Exceptions require Tech Director approval noted in the PR or an ADR for systemic exceptions.
