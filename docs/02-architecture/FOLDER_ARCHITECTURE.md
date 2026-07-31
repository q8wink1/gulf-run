# Folder Architecture

**Last updated:** 2026-07-31  
**Owner:** Principal Architect  
**Audience:** All engineers, Tech Art, DevOps

**Requirements-level companion:** [TECHNICAL_ARCHITECTURE.md](TECHNICAL_ARCHITECTURE.md) (P049 v1.0) is the official source of truth for architecture principles, project layers, named core system managers, and dependency rules. This document remains the detailed repository layout / assembly definition implementation.

---

## 1. Purpose

Define the long-term repository layout for a multi-year mobile live-service title. Every future implementation MUST place code and assets according to this map.

## 2. Repository strategy

**Default:** Single **monorepo** (`GulfRun`) with clear top-level domains.  
**Exception:** Split repos only via ADR (e.g., extremely large art LFS, or separately versioned public SDKs).

Rationale: atomic PRs across client/server contracts, unified CI policy, simpler compliance scanning.

## 3. Top-level tree

```
GulfRun/
├── README.md
├── CONTRIBUTING.md
├── .gitignore
├── .gitattributes
├── docs/                          # Foundation & ADRs (source of truth)
├── Client/                        # Unity mobile game client
├── Server/                        # Authoritative backend services
├── Shared/                        # Contracts: protos, schemas, constants
├── Tools/                         # Pipelines, editors, generators, CLI
├── Infrastructure/                # IaC, envs, observability as code
├── Art/                           # DCC sources (not Unity-imported raw dumps)
├── Design/                        # GDD, economy, UX (non-code)
├── QA/                            # Test plans, device matrix, automation specs
└── ThirdParty/                    # Vendored legal-reviewed binaries (rare)
```

## 4. Client/ (Unity)

```
Client/
├── README.md
├── ProjectSettings/
├── Packages/
├── Assets/
│   ├── _Project/                  # All first-party content & code
│   │   ├── Scripts/
│   │   │   ├── Core/              # Boot, DI, logging, time, math
│   │   │   ├── Infrastructure/    # HTTP, WS, persistence adapters
│   │   │   ├── Domain/            # Pure game rules usable in tests
│   │   │   ├── Features/          # Feature modules (vertical slices)
│   │   │   │   ├── _Template/
│   │   │   │   ├── Auth/
│   │   │   │   ├── Meta/
│   │   │   │   ├── Session/
│   │   │   │   ├── Shop/
│   │   │   │   └── ...
│   │   │   ├── Presentation/      # UI views/presenters (no server trust)
│   │   │   └── Debug/             # Dev-only; stripped from release
│   │   ├── Art/                   # Runtime-ready art (imported)
│   │   ├── Audio/
│   │   ├── UI/
│   │   ├── Prefabs/
│   │   ├── Scenes/
│   │   │   ├── Boot/
│   │   │   ├── Meta/
│   │   │   ├── Session/
│   │   │   └── Debug/
│   │   ├── Settings/              # ScriptableObject configs
│   │   ├── Addressables/          # Group schemas / layouts
│   │   └── Resources/             # FORBIDDEN except tiny boot strap
│   ├── Plugins/                   # Platform plugins
│   ├── ThirdParty/                # Imported packages needing Assets/
│   └── Tests/
│       ├── EditMode/
│       └── PlayMode/
└── Builds/                        # Local only; gitignored
```

### Assembly definitions (mandatory)

| Assembly | Responsibility |
|----------|----------------|
| `GulfRun.Core` | No Unity scene deps where possible |
| `GulfRun.Infrastructure` | Network, platform SDKs |
| `GulfRun.Domain` | Deterministic rules / DTOs mapping |
| `GulfRun.Features.*` | One asmdef per feature (or bounded context) |
| `GulfRun.Presentation` | UI |
| `GulfRun.Debug` | Editor/dev only |
| `GulfRun.Tests.*` | Tests reference code, not vice versa |

**Rule:** Features must not reference other Features directly; share via Domain/Core or explicit shared contracts.

## 5. Server/

```
Server/
├── README.md
├── gateway/                       # Edge API / BFF
├── services/
│   ├── identity/
│   ├── inventory/
│   ├── economy/
│   ├── matchmaking/
│   ├── session/                   # Authoritative game sessions
│   ├── social/
│   ├── liveops/                   # Config, events, offers
│   ├── purchase/                  # IAP validation
│   └── moderation/
├── workers/                       # Async jobs, ledger processors
├── libs/                          # Internal shared server libs
└── tests/
```

**Rule:** Domain services own their data. Cross-service writes go through APIs/events — no shared DB freestyle.

## 6. Shared/

```
Shared/
├── README.md
├── protobuf/                      # Or equivalent IDL
├── openapi/                       # External/partner APIs if any
├── json-schemas/                  # LiveOps configs, content manifests
├── constants/                     # Enums mirrored client/server
└── generated/                     # CI-generated; do not hand-edit
```

## 7. Tools/

```
Tools/
├── ci/
├── codegen/
├── content-pipeline/
├── economy-sim/
├── localization/
└── developer-cli/
```

## 8. Infrastructure/

```
Infrastructure/
├── terraform/   # or chosen IaC
├── environments/
│   ├── dev/
│   ├── staging/
│   ├── softlaunch/
│   └── prod/
├── observability/
└── network/
```

## 9. Art/ vs Client Art

| Path | Contents |
|------|----------|
| `Art/` | Maya/Blender/PSD/Substance sources, naming per Asset Organization |
| `Client/.../Art/` | Only imported, budget-compliant runtime assets |

Source art MAY use Git LFS or an art vault (ADR). Never commit multi-gig unoptimized caches.

## 10. Design/ and QA/

```
Design/
├── GDD/
├── economy/
├── ux/
└── liveops-calendars/

QA/
├── device-matrix/
├── test-plans/
├── automation/
└── certification/
```

## 11. Ownership

| Path | Owner |
|------|-------|
| `docs/` | Tech Director |
| `Client/` | Client Lead |
| `Server/` | Server Lead |
| `Shared/` | Architect |
| `Tools/` | Eng Manager / Tech Art as applicable |
| `Infrastructure/` | DevOps Lead |
| `Art/` | Art Director |
| `Design/` | Design Lead |
| `QA/` | QA Lead |

## 12. Forbidden patterns

- Gameplay scripts at `Assets/` root
- New top-level folders without ADR
- `Resources/` dumping ground
- Circular feature references
- Server business logic inside Unity client for trusted outcomes
- Committing `Library/`, `Temp/`, `Builds/`, `.env` with secrets

## 13. Scaffolding in this repo

Top-level domains are created with `README.md` markers describing purpose. Unity project files are **not** created in Phase 0 (deferred to M1).
