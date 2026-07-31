# Risk Assessment

**Last updated:** 2026-07-31  
**Owner:** Technical Director + Producer  
**Audience:** Leadership, Tech Leads, Security, LiveOps

---

## 1. Purpose

Identify risks that can threaten GulfRun’s ability to ship, retain players, protect revenue, and scale to millions of players. This register drives mitigation work in the roadmap and phases.

## 2. Scoring

| Dimension | Scale |
|-----------|-------|
| Impact | 1 (minor) – 5 (existential / major revenue or trust loss) |
| Likelihood | 1 (rare) – 5 (expected without mitigation) |
| Score | Impact × Likelihood |
| Priority | Critical ≥ 16 · High 10–15 · Medium 6–9 · Low ≤ 5 |

## 3. Risk register

### 3.1 Technical

| ID | Risk | I | L | Score | Mitigation | Owner |
|----|------|---|---|-------|------------|-------|
| T-01 | Client-authoritative economy / progression enables widespread cheating | 5 | 4 | 20 | Server authority from day one; see Anti-Cheat & Multiplayer | Architect |
| T-02 | Late multiplayer retrofit causes rewrite | 5 | 4 | 20 | Online-capable architecture in Phase 1; no offline-only economy | Architect |
| T-03 | Unity version / package churn breaks long-lived branches | 4 | 3 | 12 | Pin LTS; package allowlist; upgrade ADR windows | Eng Manager |
| T-04 | Binary bloat / Addressables misuse → install & update failures | 4 | 4 | 16 | Asset budgets, CI size gates, content CDN | Client Lead |
| T-05 | Backend hotspot (matchmaking, inventory) under launch spike | 5 | 3 | 15 | Load tests, autoscaling, queue shedding, chaos drills | Server Lead |
| T-06 | Cross-play / platform SDK divergence | 3 | 3 | 9 | Abstraction layer; platform adapters; certification schedule | Client Lead |
| T-07 | Data migration failures on LiveOps schema changes | 4 | 3 | 12 | Versioned schemas, expand/contract migrations, canaries | Server Lead |
| T-08 | Inadequate observability → slow incident response | 4 | 3 | 12 | OpenTelemetry, SLOs, runbooks before Soft Launch | DevOps |

### 3.2 Security & trust

| ID | Risk | I | L | Score | Mitigation | Owner |
|----|------|---|---|-------|------------|-------|
| S-01 | Account takeover / credential stuffing | 5 | 3 | 15 | Device binding, MFA options, anomaly detection | Security |
| S-02 | IAP fraud / receipt replay | 5 | 3 | 15 | Server-side receipt validation; replay cache | Security |
| S-03 | Secrets leaked in client or git | 5 | 2 | 10 | Secret scanning, vault, no long-lived client secrets | Security |
| S-04 | Privacy / regional compliance failure (GDPR, etc.) | 5 | 2 | 10 | Privacy by design; DPA review; data residency plan | Legal + Security |
| S-05 | Bot farms / automated farming | 4 | 4 | 16 | Rate limits, behavioral signals, economic sinks | Anti-Cheat |

### 3.3 Product & economy

| ID | Risk | I | L | Score | Mitigation | Owner |
|----|------|---|---|-------|------------|-------|
| P-01 | Soft currency inflation kills retention | 5 | 3 | 15 | Economy simulation, sinks, remote config kill-switches | Economy Designer |
| P-02 | Pay-to-win perception damages brand | 4 | 3 | 12 | Competitive integrity rules; cosmetic-first monetization bias | Design Lead |
| P-03 | Content pipeline too slow for LiveOps cadence | 4 | 3 | 12 | Tooling investment Phase 2; content freeze calendars | Producer |
| P-04 | Genre / market misfit after Soft Launch | 5 | 2 | 10 | Instrumented Soft Launch; kill/pivot criteria | Producer |

### 3.4 Production & org

| ID | Risk | I | L | Score | Mitigation | Owner |
|----|------|---|---|-------|------------|-------|
| O-01 | Scope creep before vertical slice | 4 | 4 | 16 | Phase gates; milestone exit criteria | Producer |
| O-02 | Key-person dependency on architecture | 4 | 3 | 12 | ADRs, pairing, documented ownership | Tech Director |
| O-03 | Vendor lock-in (BaaS, analytics) without exit | 3 | 3 | 9 | Abstraction interfaces; data export drills | Architect |
| O-04 | Store rejection delays (privacy, IAP, UGC) | 4 | 2 | 8 | Certification checklist; Legal review buffer | Producer |

### 3.5 Live operations

| ID | Risk | I | L | Score | Mitigation | Owner |
|----|------|---|---|-------|------------|-------|
| L-01 | Bad remote config ships economy break | 5 | 3 | 15 | Staged rollouts, freeze windows, auto-rollback | LiveOps |
| L-02 | DDoS / abuse during seasonal events | 4 | 3 | 12 | Edge protection, capacity reservations | DevOps |
| L-03 | Support backlog after launch | 3 | 4 | 12 | Self-serve tools, CRM, tiered support | Player Support |

## 4. Top critical risks (must track on roadmap)

1. **T-01 / T-02** — Authority model and online architecture  
2. **T-04** — Client size and content delivery  
3. **S-05** — Farming / bots  
4. **O-01** — Scope before slice  
5. **L-01** — Config-driven economy incidents  

## 5. Risk review process

- Update this register at each milestone gate.
- New Critical/High risks require a mitigation owner and date within one sprint of identification.
- Residual risk acceptance requires Tech Director (technical) or Producer (product) sign-off.

## 6. Assumptions

- Primary platforms: iOS and Android (phones first; tablets later).
- Competitive modes exist and require integrity.
- Monetization includes IAP and possibly ads in non-competitive surfaces (ADR later).
- Backend will be cloud-hosted with multi-region ambition by Year 2.

## 7. Out of scope for this register

Legal contract risk, financing, and hiring plans are tracked in studio PMO tools; only risks that change engineering architecture are listed here.
