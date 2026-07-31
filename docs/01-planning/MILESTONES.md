# Milestones

**Last updated:** 2026-07-31  
**Owner:** Producer  
**Audience:** All leads

---

## 1. Purpose

Milestones are **binary gates**. A milestone is complete only when all exit criteria are met and signed by the listed approvers. Partial feature lists do not count as done.

## 2. Milestone catalog

### M0 — Foundation Complete

| Field | Value |
|-------|-------|
| Goal | Studio-ready documentation and repo scaffolding |
| Approvers | Tech Director, Producer |
| Target | Immediate (current phase) |

**Exit criteria**

- [x] All 20 foundation topics documented under `docs/`
- [x] Folder architecture scaffolding present with ownership READMEs
- [x] CONTRIBUTING + git strategy published
- [ ] Risk register v1 accepted by Tech Director _(awaiting sign-off)_
- [x] ADR process live (`docs/adr/`)

---

### M1 — Engineering Bootstrap

| Field | Value |
|-------|-------|
| Goal | Compileable Unity client skeleton + CI + shared contracts |
| Approvers | Tech Director, Client Lead, DevOps |

**Exit criteria**

- [ ] Unity LTS project under `Client/` matching [TECHNICAL_STACK](../02-architecture/TECHNICAL_STACK.md)
- [ ] Empty assembly definition layout matching folder architecture
- [ ] CI: docs lint + client compile (dev) on PR
- [ ] `Shared/` schema toolchain chosen (ADR)
- [ ] Secret scanning enabled
- [ ] Device tier matrix checked into `QA/`

---

### M2 — Vertical Slice (Online-Capable)

| Field | Value |
|-------|-------|
| Goal | One playable loop proving net architecture and budgets |
| Approvers | Tech Director, Design Lead, Client Lead, Server Lead |

**Exit criteria**

- [ ] Slice design locked (Design brief + tech brief)
- [ ] Session start → play → result → reward path works with **server authority**
- [ ] Performance budgets met on Low and Mid tiers (see Mobile Optimization)
- [ ] Telemetry events for slice KPIs validated
- [ ] No Critical bugs open on slice path
- [ ] Anti-cheat baseline validators for slice rewards

---

### M3 — Economy & Account MVP

| Field | Value |
|-------|-------|
| Goal | Trusted accounts, inventory, IAP path |
| Approvers | Server Lead, Security, Economy Designer |

**Exit criteria**

- [ ] Account create / login / link (platform + guest upgrade)
- [ ] Inventory service authoritative
- [ ] IAP receipt validation (sandbox + production path designed)
- [ ] Soft currency grant/spend only via server
- [ ] Abuse rate limits on grant endpoints
- [ ] GDPR/privacy data deletion request path designed

---

### M4 — LiveOps Tooling Alpha

| Field | Value |
|-------|-------|
| Goal | Ship an event without a full client rebuild |
| Approvers | LiveOps Lead, Tech Director |

**Exit criteria**

- [ ] Remote config + feature flags in staging
- [ ] Event schedule + offer definition pipeline
- [ ] Content pack via CDN Addressables
- [ ] Staged rollout + kill switch demonstrated
- [ ] Runbook: rollback bad config

---

### M5 — Soft Launch Ready

| Field | Value |
|-------|-------|
| Goal | Certifiable build for limited markets |
| Approvers | Producer, Tech Director, Security, QA Lead |

**Exit criteria**

- [ ] Store compliance packets complete (iOS/Android)
- [ ] Crash-free session target met on soak builds
- [ ] Load test ≥ Soft Launch peak × 10 for critical paths
- [ ] Penetration test findings remediations accepted
- [ ] Support macros + player lookup tooling
- [ ] Soft Launch KPI dashboard live
- [ ] Legal / privacy review signed

---

### M6 — Soft Launch Complete

| Field | Value |
|-------|-------|
| Goal | Data-backed go/no-go for Global |
| Approvers | Studio leadership |

**Exit criteria**

- [ ] Pre-agreed KPI gates (retention, revenue, stability, cost) reviewed
- [ ] Top economy / cheat vectors patched or accepted
- [ ] Content calendar for Global Season 0/1 locked
- [ ] Capacity plan for Global peak signed by DevOps
- [ ] Go / No-Go / Pivot decision recorded (ADR or leadership memo)

---

### M7 — Global Launch

| Field | Value |
|-------|-------|
| Goal | Staged worldwide availability |
| Approvers | Studio leadership, Tech Director, LiveOps |

**Exit criteria**

- [ ] Staged country/store rollout complete
- [ ] Error budgets held for first 72 hours (or incident process followed)
- [ ] On-call rotations staffed
- [ ] Marketing / store page parity with live build
- [ ] Post-launch war-room closed with action items filed

---

### M8 — LiveOps Steady State

| Field | Value |
|-------|-------|
| Goal | Repeatable seasonal operations at scale |
| Approvers | LiveOps Lead, Tech Director |

**Exit criteria**

- [ ] Two full seasons operated without Sev-1 caused by process gaps
- [ ] Quarterly DR / failover drill completed
- [ ] Cost per DAU within target band
- [ ] Anti-cheat ops SLAs met for 90 days
- [ ] Tech debt quarter scheduled on roadmap

---

## 3. Sign-off template

```markdown
## Milestone Mx Sign-off
Date:
Approver / Role:
Exit criteria met: Yes / No (list exceptions)
Residual risks accepted:
Conditions for next milestone:
Signature:
```

## 4. Slippage policy

- Slipping a date is allowed; **weakening exit criteria is not**, unless leadership explicitly accepts reduced scope via ADR.
- Parallel work on the next milestone may start at risk, but merge to `main`/`release` trains still requires prior gate.
