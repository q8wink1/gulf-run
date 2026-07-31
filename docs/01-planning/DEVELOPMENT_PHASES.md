# Development Phases

**Last updated:** 2026-07-31  
**Owner:** Technical Director  
**Audience:** Engineering, Production

---

## 1. Phase model

GulfRun uses **gated phases**. Each phase has a purpose, allowed work, forbidden work, and exit criteria (mapped to [MILESTONES.md](MILESTONES.md)).

| Phase | Name | Primary milestone |
|-------|------|-------------------|
| 0 | Foundation | M0 |
| 1 | Engineering Bootstrap | M1 |
| 2 | Vertical Slice | M2 |
| 3 | Online Economy MVP | M3 |
| 4 | Production Systems | M4 (+ content production) |
| 5 | Soft Launch | M5–M6 |
| 6 | Global Launch | M7 |
| 7 | Scale & LiveOps Maturity | M8 |

## 2. Phase 0 — Foundation (current)

**Purpose:** Establish rules that millions-scale development cannot retrofit cheaply.

**Allowed**

- Documentation (this library)
- Repo scaffolding and ownership markers
- ADR process
- Tooling *specifications* (not shipping gameplay tools)

**Forbidden**

- Gameplay implementation
- Unapproved SDK adoption
- One-off folder layouts that contradict architecture

**Exit:** M0

## 3. Phase 1 — Engineering Bootstrap

**Purpose:** Create a compileable, CI-gated Unity client and service skeletons with shared contracts.

**Focus areas**

- Unity LTS project + assembly definitions
- Logging, config, environment switching
- Auth stub + telemetry stub
- Server skeleton (gateway + one domain service)
- IaC skeleton for `dev` environment

**Exit:** M1

## 4. Phase 2 — Vertical Slice

**Purpose:** Prove fun *and* architecture on target devices with server authority.

**Focus areas**

- One mode/loop end-to-end
- Netcode path for that loop
- Art/audio pipeline sample through Addressables
- Automated smoke tests for slice
- Performance capture on Low/Mid/High tiers

**Exit:** M2

## 5. Phase 3 — Online Economy MVP

**Purpose:** Make progression and money trustworthy.

**Focus areas**

- Accounts, inventory, wallets
- IAP validation
- Grant/spend ledgers
- Basic fraud signals
- Privacy deletion / export design

**Exit:** M3

## 6. Phase 4 — Production Systems

**Purpose:** Build the factory for content and systems at production quality.

**Focus areas**

- Full meta systems per Design
- LiveOps remote config / events / offers
- Localization
- QA automation expansion
- Matchmaking/session scale-out design validation
- Security hardening pass

**Exit:** M4 + production content checklist (owned by Producer)

## 7. Phase 5 — Soft Launch

**Purpose:** Learn from real markets with production-like ops.

**Focus areas**

- Certification
- Load & soak tests
- Support readiness
- Economy tuning
- Cheat response drills
- Go/No-Go package

**Exit:** M5 then M6

## 8. Phase 6 — Global Launch

**Purpose:** Controlled worldwide scale-up.

**Focus areas**

- Staged rollouts
- Capacity & cost watch
- Incident command
- Store ops
- Season 0/1 execution

**Exit:** M7

## 9. Phase 7 — Scale & LiveOps Maturity

**Purpose:** Sustainable multi-year operation.

**Focus areas**

- Multi-region maturity
- Cost optimization
- Anti-cheat sophistication
- Platform expansion ADRs
- Scheduled tech-debt phases

**Exit:** M8 (then continuous improvement cycles)

## 10. Engineering modes within phases

| Mode | When | Rules |
|------|------|-------|
| Spike | Uncertain tech | Time-boxed; no merge of spike hacks without cleanup |
| Feature | Normal delivery | Branch + PR + tests |
| Hotfix | Live incidents | Fast path per Git strategy; postmortem required |
| Freeze | Pre-cert / launch | Only approved fixes |

## 11. Definition of Ready / Done (feature level)

**Ready**

- Design brief + tech brief linked
- Ownership and folder location known
- Telemetry and remote-config needs listed
- Security impact checked (checklist)

**Done**

- Server authority respected
- Tests at agreed layer
- Docs/ADR updated
- Budgets not regressing beyond threshold
- Feature flagged if player-facing post Soft Launch

## 12. Phase transition checklist (generic)

1. Milestone exit criteria signed  
2. Risk register updated  
3. Open Sev-1/Sev-2 on phase scope = 0 (or waived)  
4. Next phase staffing confirmed  
5. Announce freeze/unfreeze rules to studio  
