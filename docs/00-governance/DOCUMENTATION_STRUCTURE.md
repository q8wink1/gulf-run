# Documentation Structure

**Last updated:** 2026-07-31  
**Owner:** Technical Director  
**Audience:** All disciplines

---

## 1. Purpose

Define how GulfRun documentation is organized, owned, reviewed, and kept authoritative across a multi-year commercial production serving millions of players.

## 2. Principles

1. **Split sources of truth** — **Gameplay design** lives only in [`Design/GDD/`](../../Design/GDD/README.md). **Engineering/architecture** lives under `docs/`. Wikis and chat are ephemeral.
2. **No invented gameplay** — Documentation and engineering MUST NOT invent mechanics, modes, UI screens, entities, progression, economy rules, or multiplayer features. If GDD content is missing, ask the Design Owner.
3. **Docs before systems** — Major features require approved GDD coverage + a short tech brief before implementation.
4. **ADRs for decisions** — Stack, protocol, economy *authority implementation*, and security posture changes need ADRs (not gameplay invention).
5. **Audience-first** — Each doc states owner, audience, and last-updated date.
6. **Living but gated** — Docs evolve; normative changes require review (see Change control in [docs/README.md](../README.md)).

## 3. Taxonomy

```
docs/
├── README.md                          # Index
├── 00-governance/                     # Process, risk, doc rules
├── 01-planning/                       # Roadmap, milestones, phases
├── 02-architecture/                   # System & scale architecture
├── 03-standards/                      # Engineering conventions
├── 04-engineering/                    # Build, packages, performance
├── 05-security/                       # Security & integrity
├── 06-operations/                     # LiveOps & vendors
├── adr/                               # Architecture Decision Records
├── runbooks/                          # (Future) incident & ops runbooks
├── api/                               # (Future) OpenAPI / proto docs
└── design-briefs/                     # (Future) per-feature tech briefs
```

### Adjacent non-`docs/` sources

| Location | Content | Normative? |
|----------|---------|------------|
| `Design/GDD/` | Game Design Document | **Gameplay single source of truth** (when sections are Approved) |
| `Design/` (other) | Working papers (economy, UX, calendars) | Supporting; must not contradict approved GDD |
| `QA/` | Test plans, device matrix | Operational |
| `Infrastructure/` | IaC comments + runbooks | Operational |
| Root `README.md` | Entry point only | Points to docs + GDD |

## 4. Document types

| Type | Location | Required sections |
|------|----------|-------------------|
| Foundation policy | `docs/0x-*` | Purpose, Owner, Normative rules, Exceptions |
| ADR | `docs/adr/NNNN-*.md` | Context, Decision, Consequences, Status |
| Tech brief | `docs/design-briefs/` | Problem, Constraints, Approach, Risks, Open questions |
| Runbook | `docs/runbooks/` | Symptoms, Diagnosis, Mitigation, Escalation |
| API reference | `docs/api/` | Generated from schemas; do not hand-edit forever |

## 5. Ownership matrix

| Area | Primary owner | Backup |
|------|---------------|--------|
| **GDD content (gameplay)** | **Design Owner** | Design delegates |
| GDD structure / conflicts log | Documentation engineer | Tech Director |
| Planning docs | Producer + Tech Director | Lead Producer |
| Architecture | Principal Architect | Tech Director |
| Standards | Engineering Manager | Principal Engineer |
| Security / anti-cheat | Security Lead | Tech Director |
| LiveOps | LiveOps Lead | Producer |
| CI/CD / infra docs | DevOps Lead | Tech Director |
| ADRs | Authoring engineer | Architect on-call |

## 6. Review cadence

| Cadence | Activity |
|---------|----------|
| Every PR | Touch related docs when behavior changes |
| Bi-weekly | Tech Director spot-check of open ADR drafts |
| Monthly | Standards & mobile budgets revisit |
| Per milestone | Full foundation doc audit vs. actual practice |
| Post-incident | Runbook + risk register update within 5 business days |

## 7. Writing standards

- Prefer short normative statements (“MUST / SHOULD / MAY” per RFC 2119).
- Link rather than duplicate.
- Prefer tables for matrices; diagrams as Mermaid in Markdown when possible.
- No vendor lock-in language without an ADR.
- Do not embed secrets, internal URLs with credentials, or player PII examples from production.

## 8. Localization of docs

Engineering docs remain **English**. Player-facing localization is a product pipeline (see Asset Organization and LiveOps), not a docs concern.

## 9. Deprecation

Deprecated documents:

1. Add `Status: Deprecated` and link to replacement.
2. Keep file for 2 milestones for history.
3. Remove only after ADR or Tech Director approval.

## 10. Future expansions

When Unity and services land, add:

- `docs/api/` — generated from protobuf / OpenAPI
- `docs/runbooks/` — page, outage, economy emergency
- `docs/design-briefs/` — one brief per major system
- Per-package `README.md` under `Client/`, `Server/`, `Tools/`
