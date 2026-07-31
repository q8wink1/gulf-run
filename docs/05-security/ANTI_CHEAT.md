# Anti-Cheat High-Level Strategy

**Last updated:** 2026-07-31  
**Owner:** Security Lead + Server Lead  
**Audience:** Gameplay, Server, LiveOps, Support

> Requirements-level companion: [ANTI_CHEAT_SPECIFICATION.md](ANTI_CHEAT_SPECIFICATION.md) (P043) — protected systems list, backend validation scope, and design principles as briefed. This document remains the implementation-strategy source (layers of defense, technical controls, ops model, tooling roadmap). No conflicts identified between the two.

---

## 1. Premise

At millions of players, **some cheating will occur**. GulfRun’s goal is to make cheating **unprofitable**, **detectable**, and **reversible** without destroying legitimate player experience.

Client obfuscation is a **speed bump**. Server authority and economics design are the **walls**.

## 2. Layers of defense

```
Prevention → Detection → Response → Deterrence
```

| Layer | Examples |
|-------|----------|
| Prevention | Server authority, authoritative RNG, rate limits, attestation where useful |
| Detection | Statistical anomalies, replay review, honeypots, device graphs |
| Response | Soft shadow limits, rollbacks, bans, forced patch |
| Deterrence | Clear ToS, visible enforcement, appeal process |

## 3. Trust boundaries

| Client may | Client must not |
|------------|-----------------|
| Predict motion for feel | Decide match winner |
| Show UI costs | Grant currency |
| Cache inventory display | Authoritatively spawn rare items |
| Send inputs | Send “I dealt 99999 damage” as fact |

## 4. Mode-specific policies

| Mode type | Policy |
|-----------|--------|
| Ranked / competitive | Full authority + strict detection |
| Casual | Authority retained; softer UX on false positives |
| Solo practice | Local OK; rewards still server-claimed |
| Async leaderboards | Server score validation + replay sampling |

## 5. Technical controls (high level)

### Session

- Sanity checks: speed, teleport, resource rates, action frequency
- State hash / checksum sampling
- Secure settlement: session service → economy with service auth
- Optional replay recording for disputed high-value matches

### Meta / economy

- All grants tagged with `reason_code` + `source_id`
- Velocity checks on earn/spend
- Duplicate command rejection
- Trade/gift (if any) delayed + taxed + graph analysis

### Platform signals

- Play Integrity / DeviceCheck / App Attest where available — **risk scores**, not binary trust
- Emulator / automation heuristics
- Account linking reputation

## 6. Botting & farming

- Progressive challenges on suspicious behavior (not only at login)
- Economic sinks and soft caps from Design
- Detection on playtime patterns, perfect inputs, farm routes
- IP / device clustering — careful with shared NAT / cafes (regional reality)

## 7. False positive management

- Prefer **degraded matchmaking** or **reward holds** over instant hard bans for ambiguous cases.
- Human review queue for high-value accounts.
- Appeal path documented for Support.
- Metrics: precision/recall sampled monthly.

## 8. Ops model

| Role | Responsibility |
|------|----------------|
| Anti-Cheat eng | Detectors, pipelines |
| LiveOps | Config thresholds, event-specific rules |
| Support | Player communication, appeals L1 |
| Security | Exploit response, bans policy with Legal |

SLA targets defined before Global (e.g., critical exploit hotfix path < N hours).

## 9. Tooling roadmap

| Phase | Capability |
|-------|------------|
| M2 | Settlement validation for slice rewards |
| M3 | Ledger velocity alerts |
| M5 | Manual ban/compensation tools + audit log |
| M7 | Automated detectors with shadow mode → enforce |
| M8 | Graph analysis, ML-assisted scoring (privacy-reviewed) |

## 10. Privacy constraints

- Minimize raw input retention; define retention windows.
- Detectors that use PII need Legal review.
- Ban reasons stored with policy for disclosure.

## 11. Incident playbook (summary)

1. Detect exploit (internal or external report)  
2. Feature-flag disable affected path if needed  
3. Patch validators  
4. Identify beneficiaries via ledger  
5. Roll back / claw back per policy  
6. Ban wave if warranted  
7. Postmortem + detector added  

## 12. Success metrics

- % of economy mass from anomalous clusters trending down
- Time-to-mitigate Sev-1 exploit
- False positive appeal overturn rate within target
- Ranked integrity sentiment / CS ticket themes
