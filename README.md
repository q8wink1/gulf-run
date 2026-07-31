# Project GulfRun

**Commercial mobile game · Multi-year studio production · Target: millions of concurrent and monthly active players**

GulfRun is a long-term live-service mobile title. This repository holds the **engineering foundation, governance, and architecture** that all future implementation must follow. Gameplay systems are intentionally not implemented in this phase.

---

## Current status

| Item | Status |
|------|--------|
| Foundation documentation | Complete |
| Unity project bootstrap | Deferred (post-architecture sign-off) |
| Gameplay implementation | Not started — blocked until Phase 0 gate |
| Live services / backends | Design-only |

**Authority rules:**

- **Gameplay** (mechanics, UI, modes, entities, progression, economy rules, monetization design, multiplayer design intent): [`Design/GDD/`](Design/GDD/README.md) is the single source of truth. Do not invent gameplay; ask if missing.
- **Engineering / architecture:** [`docs/`](docs/README.md). If implementation conflicts with `docs/`, documentation wins until an ADR amends it.
- **Conflicts between approved GDD and `docs/`:** log and escalate; do not silently invent a compromise.

---

## Documentation map

All studio foundation material lives under [`docs/`](docs/README.md).

| # | Topic | Document |
|---|--------|----------|
| 1 | Project roadmap | [docs/01-planning/ROADMAP.md](docs/01-planning/ROADMAP.md) |
| 2 | Milestones | [docs/01-planning/MILESTONES.md](docs/01-planning/MILESTONES.md) |
| 3 | Development phases | [docs/01-planning/DEVELOPMENT_PHASES.md](docs/01-planning/DEVELOPMENT_PHASES.md) |
| 4 | Folder architecture | [docs/02-architecture/FOLDER_ARCHITECTURE.md](docs/02-architecture/FOLDER_ARCHITECTURE.md) |
| 5 | Coding standards | [docs/03-standards/CODING_STANDARDS.md](docs/03-standards/CODING_STANDARDS.md) |
| 6 | Naming conventions | [docs/03-standards/NAMING_CONVENTIONS.md](docs/03-standards/NAMING_CONVENTIONS.md) |
| 7 | Asset organization | [docs/03-standards/ASSET_ORGANIZATION.md](docs/03-standards/ASSET_ORGANIZATION.md) |
| 8 | Git branching strategy | [docs/03-standards/GIT_BRANCHING_STRATEGY.md](docs/03-standards/GIT_BRANCHING_STRATEGY.md) |
| 9 | Documentation structure | [docs/00-governance/DOCUMENTATION_STRUCTURE.md](docs/00-governance/DOCUMENTATION_STRUCTURE.md) |
| 10 | Risk assessment | [docs/00-governance/RISK_ASSESSMENT.md](docs/00-governance/RISK_ASSESSMENT.md) |
| 11 | Technical stack | [docs/02-architecture/TECHNICAL_STACK.md](docs/02-architecture/TECHNICAL_STACK.md) |
| 12 | External services | [docs/06-operations/EXTERNAL_SERVICES.md](docs/06-operations/EXTERNAL_SERVICES.md) |
| 13 | Unity packages | [docs/04-engineering/UNITY_PACKAGES.md](docs/04-engineering/UNITY_PACKAGES.md) |
| 14 | CI/CD | [docs/04-engineering/CI_CD.md](docs/04-engineering/CI_CD.md) |
| 15 | Multiplayer architecture | [docs/02-architecture/MULTIPLAYER_ARCHITECTURE.md](docs/02-architecture/MULTIPLAYER_ARCHITECTURE.md) |
| 16 | Mobile optimization | [docs/04-engineering/MOBILE_OPTIMIZATION.md](docs/04-engineering/MOBILE_OPTIMIZATION.md) |
| 17 | Security strategy | [docs/05-security/SECURITY_STRATEGY.md](docs/05-security/SECURITY_STRATEGY.md) |
| 18 | Anti-cheat strategy | [docs/05-security/ANTI_CHEAT.md](docs/05-security/ANTI_CHEAT.md) |
| 19 | Live Operations | [docs/06-operations/LIVE_OPERATIONS.md](docs/06-operations/LIVE_OPERATIONS.md) |
| 20 | Scalability plan | [docs/02-architecture/SCALABILITY_PLAN.md](docs/02-architecture/SCALABILITY_PLAN.md) |

---

## Repository layout (high level)

```
GulfRun/
├── README.md                 # This file
├── CONTRIBUTING.md           # How engineers contribute
├── docs/                     # Studio foundation (source of truth)
├── Client/                   # Unity mobile client (future)
├── Server/                   # Authoritative services (future)
├── Shared/                   # Cross-cutting contracts & schemas
├── Tools/                    # Build, CI, content pipelines
├── Infrastructure/           # IaC, environments, observability
├── Art/                      # Source art (DCC), not runtime
├── Design/                   # GDD / economy / LiveOps design
└── QA/                       # Test plans, automation harnesses
```

See [Folder Architecture](docs/02-architecture/FOLDER_ARCHITECTURE.md) for the full tree and ownership rules.

---

## Non-negotiables

1. **Server authority** for competitive, economy, progression, and inventory outcomes.
2. **No secrets in git** — credentials live in vault / CI secret stores only.
3. **Feature flags** for all player-facing rollouts after Soft Launch.
4. **ADR required** for stack, protocol, economy, or security changes.
5. **Performance budgets** enforced in CI for target devices (see Mobile Optimization).
6. **Documentation before gameplay** for new major systems (design + tech brief).

---

## Getting started (engineers)

1. Read [`Design/GDD/README.md`](Design/GDD/README.md) — gameplay SoT (may be empty/template).
2. Read [`docs/README.md`](docs/README.md) — engineering SoT.
3. Read [Technical Stack](docs/02-architecture/TECHNICAL_STACK.md) and [Coding Standards](docs/03-standards/CODING_STANDARDS.md).
4. Follow [CONTRIBUTING.md](CONTRIBUTING.md) and the [Git branching strategy](docs/03-standards/GIT_BRANCHING_STRATEGY.md).
5. Do not implement gameplay until the relevant GDD chapters are **Approved** and Phase gates allow it.

---

## License & confidentiality

Internal studio project. Distribution of this repository outside authorized personnel is prohibited unless Legal approves.
