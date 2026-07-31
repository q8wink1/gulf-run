# Project Roadmap

**Last updated:** 2026-07-31  
**Owner:** Producer + Technical Director  
**Horizon:** 36+ months from foundation  
**Audience:** Leadership, discipline leads

---

## 1. Vision (engineering lens)

GulfRun is a **live-service commercial mobile game** designed to retain and monetize at **millions of MAU**, with competitive integrity, regional relevance, and a content pipeline that can ship weekly events without client rebuilds for every change.

This roadmap sequences **capability**, not feature marketing copy. Gameplay pillars are owned in `Design/`; this document owns when technical capacity must exist.

## 2. Strategic pillars

| Pillar | Outcome |
|--------|---------|
| Integrity | Server-authoritative outcomes; anti-cheat capable of LiveOps scale |
| Performance | Stable 30/60 FPS on defined device tiers; tight install/update budgets |
| Operability | Feature flags, remote config, observability, 24/7 incident posture |
| Content velocity | Data-driven events, seasons, offers without full app store wait when possible |
| Scale | Horizontal services, regionalization path, cost-aware architecture |
| Platform reach | iOS + Android certification-ready; store compliance continuous |

## 3. Multi-year timeline (overview)

```mermaid
gantt
    title GulfRun Capability Roadmap
    dateFormat  YYYY-QQ
    axisFormat  %Y-%q

    section Foundation
    Phase 0 Docs & scaffolding           :done, p0, 2026-Q3, 2026-Q3
    Phase 1 Engine & service skeleton    :p1, 2026-Q3, 2026-Q4

    section Vertical Slice
    Phase 2 Core loop slice              :p2, 2026-Q4, 2027-Q1
    Phase 3 Online & economy MVP         :p3, 2027-Q1, 2027-Q2

    section Pre-Production → Production
    Phase 4 Content & systems production :p4, 2027-Q2, 2027-Q4
    Phase 5 Soft Launch                  :p5, 2027-Q4, 2028-Q1

    section Live
    Phase 6 Global Launch                :p6, 2028-Q1, 2028-Q2
    Phase 7 Scale & LiveOps maturity     :p7, 2028-Q2, 2029-Q3
```

Dates are **planning anchors**, not contracts. Milestone exit criteria in [MILESTONES.md](MILESTONES.md) gate advancement.

## 4. Year 0–1 — Foundation through vertical slice

### Y0 Q3–Q4 (now → engine bootstrap)

- Complete foundation documentation (this library) — **done as of this commit set**
- Unity LTS project creation under `Client/` per stack ADR
- CI skeleton: lint/docs checks → then compile gates
- Shared contracts repo layout (`Shared/`) for schemas
- Identity, telemetry, crash reporting stubs (no PII abuse)
- Device tier matrix v1 and performance budgets v1

### Y1 Q1–Q2

- Vertical slice: one complete session loop (online-capable)
- Account linking, inventory skeleton, IAP receipt validation path
- Matchmaking prototype (if PvP in slice) or asynchronous competition path
- Addressables + CDN pipeline for one content pack
- Anti-cheat baseline: server validation + basic anomaly hooks
- First Soft Launch candidate build criteria defined

## 5. Year 1–2 — Production and Soft Launch

### Y1 Q3–Q4

- Full meta-game systems (progression, seasons scaffolding)
- LiveOps tooling: remote config, offers, event schedules
- Localization pipeline (AR/EN minimum assumed; expand per Design)
- Load testing to 10× Soft Launch peak estimate
- Security review + penetration test before Soft Launch
- Store compliance (privacy manifests, data safety, age ratings)

### Y2 Q1

- Soft Launch in 1–3 markets
- KPI instrumentation review (D1/D7, revenue, crash-free, latency)
- Economy tuning with kill-switches
- Scale rehearsal (black Friday / event simulation)

## 6. Year 2 — Global launch and LiveOps

- Global launch train with staged rollouts
- Multi-region active-active or active-passive per Scalability Plan
- Season 1–N content factory operational
- Anti-cheat ops team playbooks live
- Cost optimization program (egress, DB, compute)

## 7. Year 3+ — Scale and platform expansion

- Additional platforms / form factors only via ADR
- Advanced social graph, clans, UGC (if approved)
- ML-assisted LiveOps (offers, churn) behind privacy review
- Tech debt burn-down quarters on calendar (not “someday”)

## 8. Capability roadmap (must-have before Global Launch)

| Capability | Soft Launch | Global |
|------------|-------------|--------|
| Server-authoritative progression | Required | Required |
| IAP validation | Required | Required |
| Feature flags / remote config | Required | Required |
| Crash + performance telemetry | Required | Required |
| CDN content updates | Required | Required |
| Matchmaking / session scale | MVP | Multi-region |
| Anti-cheat ops | Baseline | Full playbooks |
| Support tooling | Basic | CRM-integrated |
| Chaos / DR drills | One drill | Quarterly |

## 9. Explicit non-goals (near term)

- Shipping gameplay in the foundation phase
- Building custom engine
- Supporting every storefront or PC/console at launch
- Client-side “trust the client” competitive logic
- Unbounded UGC without moderation plan

## 10. Dependency map

```
Docs foundation
    → Unity + CI + Shared schemas
        → Vertical slice (online)
            → Economy + LiveOps tools
                → Soft Launch
                    → Global + multi-region scale
```

## 11. Review

Roadmap reviewed at every milestone gate and at least quarterly by Producer + Tech Director.
