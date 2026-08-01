# GulfRun Documentation Index

**Source of truth split**

- **Gameplay design:** [`Design/GDD/`](../Design/GDD/README.md) (Design Owner fills; never invent)
- **Engineering / architecture / ops:** this `docs/` tree

All future client, server, tools, and LiveOps *implementation* must conform to engineering docs and to **Approved** GDD sections. Missing GDD content → ask questions; do not assume.

---

## How to use this library

| Role | Start here |
|------|------------|
| New engineer | [Coding Standards](03-standards/CODING_STANDARDS.md) → [Folder Architecture](02-architecture/FOLDER_ARCHITECTURE.md) → [Technical Stack](02-architecture/TECHNICAL_STACK.md) |
| Tech lead | [Roadmap](01-planning/ROADMAP.md) → [Phases](01-planning/DEVELOPMENT_PHASES.md) → [Risk](00-governance/RISK_ASSESSMENT.md) |
| Producer / TD | [Milestones](01-planning/MILESTONES.md) → [LiveOps](06-operations/LIVE_OPERATIONS.md) |
| Security | [Security](05-security/SECURITY_STRATEGY.md) → [Anti-Cheat](05-security/ANTI_CHEAT.md) |
| DevOps | [CI/CD](04-engineering/CI_CD.md) → [Scalability](02-architecture/SCALABILITY_PLAN.md) → [External Services](06-operations/EXTERNAL_SERVICES.md) |

---

## Document catalog

### 00 — Governance

| Document | Purpose |
|----------|---------|
| [DOCUMENTATION_STRUCTURE.md](00-governance/DOCUMENTATION_STRUCTURE.md) | Doc taxonomy, ownership, review cadence |
| [RISK_ASSESSMENT.md](00-governance/RISK_ASSESSMENT.md) | Technical, product, ops, and compliance risks |

### 01 — Planning

| Document | Purpose |
|----------|---------|
| [ROADMAP.md](01-planning/ROADMAP.md) | Multi-year product and technology roadmap |
| [MILESTONES.md](01-planning/MILESTONES.md) | Gateable milestones with exit criteria |
| [DEVELOPMENT_PHASES.md](01-planning/DEVELOPMENT_PHASES.md) | Phase definitions from foundation to LiveOps scale |

### 02 — Architecture

| Document | Purpose |
|----------|---------|
| [FOLDER_ARCHITECTURE.md](02-architecture/FOLDER_ARCHITECTURE.md) | Monorepo / multi-repo layout and ownership |
| [TECHNICAL_STACK.md](02-architecture/TECHNICAL_STACK.md) | Client, server, data, cloud stack |
| [MULTIPLAYER_ARCHITECTURE.md](02-architecture/MULTIPLAYER_ARCHITECTURE.md) | Netcode, authority, matchmaking, scale |
| [SCALABILITY_PLAN.md](02-architecture/SCALABILITY_PLAN.md) | Path from thousands to millions of players |
| [BACKEND_ARCHITECTURE.md](02-architecture/BACKEND_ARCHITECTURE.md) | Backend responsibilities, authority, client/backend split, sync & security principles (P039 v1.0) |
| [DATABASE_ARCHITECTURE.md](02-architecture/DATABASE_ARCHITECTURE.md) | Data categories, principles, ownership, sync & security statements, backup requirement (P040 v1.0) |
| [AUTHENTICATION_SYSTEM.md](02-architecture/AUTHENTICATION_SYSTEM.md) | Account types, auth flow, linking, session management, error handling (P041 v1.0) |
| [ANALYTICS_SYSTEM.md](02-architecture/ANALYTICS_SYSTEM.md) | Analytics categories, tracked data, technical analytics, privacy rules (P044 v1.0) |
| [PERFORMANCE_OPTIMIZATION_SPECIFICATION.md](04-engineering/PERFORMANCE_OPTIMIZATION_SPECIFICATION.md) | Target/minimum frame rate, optimization principles, graphics/memory/network/loading rules (P046 v1.0) |
| [TECHNICAL_ARCHITECTURE.md](02-architecture/TECHNICAL_ARCHITECTURE.md) | Architecture principles, project layers, core system managers, dependency rules, code quality (P049 v1.0) |

### 03 — Standards

| Document | Purpose |
|----------|---------|
| [CODING_STANDARDS.md](03-standards/CODING_STANDARDS.md) | Language rules, patterns, quality bars |
| [NAMING_CONVENTIONS.md](03-standards/NAMING_CONVENTIONS.md) | Code, assets, branches, IDs |
| [ASSET_ORGANIZATION.md](03-standards/ASSET_ORGANIZATION.md) | Art/audio/UI/content pipelines |
| [GIT_BRANCHING_STRATEGY.md](03-standards/GIT_BRANCHING_STRATEGY.md) | Branch model, reviews, releases |

### 04 — Engineering

| Document | Purpose |
|----------|---------|
| [UNITY_PACKAGES.md](04-engineering/UNITY_PACKAGES.md) | Approved and deferred Unity packages |
| [CI_CD.md](04-engineering/CI_CD.md) | Pipelines, environments, release trains |
| [MOBILE_OPTIMIZATION.md](04-engineering/MOBILE_OPTIMIZATION.md) | Device tiers, budgets, profiling |

### 05 — Security

| Document | Purpose |
|----------|---------|
| [SECURITY_STRATEGY.md](05-security/SECURITY_STRATEGY.md) | App, API, data, account security |
| [ANTI_CHEAT.md](05-security/ANTI_CHEAT.md) | Detection, prevention, response |
| [ANTI_CHEAT_SPECIFICATION.md](05-security/ANTI_CHEAT_SPECIFICATION.md) | Requirements-level spec: protected systems, validation scope, principles (P043 v1.0) |

### 06 — Operations

| Document | Purpose |
|----------|---------|
| [LIVE_OPERATIONS.md](06-operations/LIVE_OPERATIONS.md) | Content cadence, economy, incident ops |
| [EXTERNAL_SERVICES.md](06-operations/EXTERNAL_SERVICES.md) | Third-party integrations roadmap |

### 07 — Sprints

| Document | Purpose |
|----------|---------|
| [SPRINT-01-PROJECT-FOUNDATION.md](07-sprints/SPRINT-01-PROJECT-FOUNDATION.md) | Sprint 1 report: folders, managers, scenes, packages, settings, and open items |
| [SPRINT-02-PLAYER-CONTROLLER-FOUNDATION.md](07-sprints/SPRINT-02-PLAYER-CONTROLLER-FOUNDATION.md) | Sprint 2 report: player prefab, movement/input/camera/animator scripts, physics config, and open items |

### Architecture Decision Records

| Path | Purpose |
|------|---------|
| [adr/](adr/README.md) | Numbered ADRs; template for all major decisions |

---

## Change control

1. Editorial fixes (typos, clarity): PR by any engineer, one reviewer.
2. Normative changes (standards, architecture): PR + **Tech Director** approval + ADR if scope warrants.
3. Security / anti-cheat / economy authority changes: PR + Security + Tech Director.

Versioning: documents use `Last updated` dates. Breaking policy changes require an ADR linking to the amended section.
