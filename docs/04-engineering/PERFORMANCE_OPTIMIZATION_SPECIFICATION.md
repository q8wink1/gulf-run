# Performance Optimization Specification

| Field | Value |
|-------|--------|
| Document ID | P046 |
| Title | Performance Optimization Specification |
| Version | **1.0** |
| Status | Approved (performance targets, principles & rules scope only) |
| Project | Project GulfRun |
| Location rationale | Performance is an **engineering** concern → lives under `docs/04-engineering/` ("Build, packages, performance") per [DOCUMENTATION_STRUCTURE.md](../00-governance/DOCUMENTATION_STRUCTURE.md) §3, not `Design/GDD/`. Numbered **P046** for continuity with the ongoing specification brief sequence. |
| Authority | Official source of truth for **target/minimum frame rate**, **optimization principles**, and the **performance rules** stated herein |
| Relates to (engineering, existing) | [MOBILE_OPTIMIZATION.md](MOBILE_OPTIMIZATION.md) — pre-existing detailed engineering strategy (device tiers, budgets, rendering/CPU/memory/network/app-size/profiling detail); this document is the requirements-level companion, analogous to how [ANTI_CHEAT_SPECIFICATION.md](../05-security/ANTI_CHEAT_SPECIFICATION.md) (P043) relates to [ANTI_CHEAT.md](../05-security/ANTI_CHEAT.md) | [CODING_STANDARDS.md](../03-standards/CODING_STANDARDS.md) §8 (Performance — references MOBILE_OPTIMIZATION.md budgets) |
| Relates to (GDD) | [P001](../../Design/GDD/P001-GAME-VISION-v1.0.md) Pillar 6 "Mobile First", Pillar 7 "High Performance"; [03-platforms-and-constraints.md](../../Design/GDD/00-front-matter/03-platforms-and-constraints.md) §3.1 (iOS/Android, High Performance pillar) |
| Last updated | 2026-07-31 |

**Rules:** Documentation only. No implementation. Do not invent gameplay. Do not add implementation details beyond this brief. Missing detail → **TODO**.

**Next milestone:** Sprint 1 — do not start until explicitly instructed.

---

## 1. Purpose

Define the performance requirements for Project GulfRun: target/minimum frame rate, loading time intent, optimization principles, graphics/memory/network/loading requirements, and the rules governing optimization — without device support matrix, graphics presets, memory budget, texture compression, LOD strategy, asset streaming, shader variants, or network compression detail (those remain engineering implementation, tracked separately in [MOBILE_OPTIMIZATION.md](MOBILE_OPTIMIZATION.md)).

---

## 2. System Overview

| Field | Value |
|-------|--------|
| Platform focus | Project GulfRun is **designed primarily for mobile devices** |
| Priority | **Performance is a core pillar of the project** |
| Device range | The game must provide a **smooth experience across a wide range of Android and iOS devices** |

### Alignment

- Matches [P001](../../Design/GDD/P001-GAME-VISION-v1.0.md) Pillar 6 "Mobile First" and Pillar 7 "High Performance" — this document is the requirements-level detail behind those pillars.
- No conflict with [MOBILE_OPTIMIZATION.md](MOBILE_OPTIMIZATION.md); its Low/Mid/High tier target FPS (30 / 30–60 / 60) is consistent with the Target 60 FPS / Minimum 30 FPS stated in this brief (§3).

---

## 3. Performance Targets

| Field | Value |
|-------|--------|
| **Target Frame Rate** | **60 FPS** |
| **Minimum Supported Frame Rate** | **30 FPS** |
| **Loading Times** | Optimized for fast loading; **exact targets are not defined** |

### TODO — Performance Targets (not provided)

- [ ] Exact loading time targets (see [MOBILE_OPTIMIZATION.md](MOBILE_OPTIMIZATION.md) §3 "Cold start to interactive" — currently TBD there too)

---

## 4. Optimization Principles

| Principle | Status |
|-----------|--------|
| **Mobile First** | Defined |
| **Efficient Rendering** | Defined |
| **Efficient Memory Usage** | Defined |
| **Minimal Battery Consumption** | Defined |
| **Stable Network Performance** | Defined |

---

## 5. Graphics

| Field | Value |
|-------|--------|
| Scalability | The game must **support scalable graphics quality** |
| Player control | **Graphics settings may be adjusted by the player** — SoT for the settings surface: [P034 Settings System](../../Design/GDD/P034-SETTINGS-SYSTEM-v1.0.md) §Graphics |
| Quality levels | **Not defined** |

### TODO — Graphics (not provided)

- [ ] Graphic quality level definitions (see [MOBILE_OPTIMIZATION.md](MOBILE_OPTIMIZATION.md) §4 Rendering strategy for existing engineering detail — Low/Mid/High tiers)

---

## 6. Memory

| Field | Value |
|-------|--------|
| Rule | Memory usage **must remain optimized** |
| Asset handling | **Unused assets should be released** |
| Budget | **Not defined** |

### TODO — Memory (not provided)

- [ ] Memory budget numbers (see [MOBILE_OPTIMIZATION.md](MOBILE_OPTIMIZATION.md) §3 "Peak working set" — currently TBD there too)

---

## 7. Network

| Field | Value |
|-------|--------|
| Rule | Network traffic **must be minimized** |
| Data policy | **Only required data should be transmitted** |
| Bandwidth | **Bandwidth optimization is required** |

### TODO — Network (not provided)

- [ ] Network compression strategy (see [MOBILE_OPTIMIZATION.md](MOBILE_OPTIMIZATION.md) §7 Networking & battery for existing engineering detail)

---

## 8. Loading

| Field | Value |
|-------|--------|
| Rule | Loading screens **should remain short** |
| Asset loading | **Assets should load efficiently** |
| Streaming strategy | **Not defined** |

### TODO — Loading (not provided)

- [ ] Asset streaming strategy (see [MOBILE_OPTIMIZATION.md](MOBILE_OPTIMIZATION.md) §8 App size & updates for existing engineering detail)

---

## 9. Rules

| Rule ID | Rule |
|---------|------|
| PERF-001 | Performance optimizations **must never change gameplay balance**. |
| PERF-002 | Optimization **should be transparent to players**. |
| PERF-003 | **Performance testing is required before release**. |

### Alignment

- PERF-001 aligns with the no-Pay-to-Win / fairness posture already established for other systems ([P013](../../Design/GDD/P013-SHOP-SYSTEM-v1.0.md), [P029](../../Design/GDD/P029-BATTLE-PASS-SYSTEM-v1.0.md), [P045](../../Design/GDD/P045-MONETIZATION-SYSTEM-v1.0.md)) — no gameplay-relevant behavior may differ by device tier or optimization level.
- PERF-003 aligns with [MOBILE_OPTIMIZATION.md](MOBILE_OPTIMIZATION.md) §9 Profiling ritual and §13 Exit criteria linkage (existing engineering process); this document does not add new implementation detail beyond restating the brief.

---

## 10. Dependencies

| Dependency | Note |
|------------|------|
| P001 Game Vision | Mobile First / High Performance pillars |
| 03-platforms-and-constraints.md | iOS/Android platform support |
| MOBILE_OPTIMIZATION.md | Existing detailed engineering strategy (tiers, budgets, rendering/CPU/memory/network/app-size/profiling) |
| CODING_STANDARDS.md §8 | Performance budget enforcement reference |
| P034 Settings System | Graphics settings player-facing surface |

---

## 11. Future Specifications

| Topic | Status |
|-------|--------|
| Device Support Matrix | Not defined |
| Graphics Presets | Not defined |
| Memory Budget | Not defined |
| Texture Compression | Not defined |
| LOD Strategy | Not defined |
| Asset Streaming | Not defined |
| Shader Variants | Not defined |
| Network Compression | Not defined |
| Exact Loading Time Targets | Not defined |

---

## 12. Explicitly Not Defined (P046)

- Device Support Matrix
- Graphics Presets
- Memory Budget
- Texture Compression
- LOD Strategy
- Asset Streaming
- Shader Variants
- Network Compression

---

## 13. Open Questions

| ID | Question |
|----|----------|
| Q-P046-001 | Exact loading time targets? |
| Q-P046-002 | Graphic quality level definitions (names / count)? |
| Q-P046-003 | Memory budget numbers per device tier? |
| Q-P046-004 | Network compression / bandwidth optimization strategy? |
| Q-P046-005 | Asset streaming strategy? |
| Q-P046-006 | Device Support Matrix, Texture Compression, LOD Strategy, Shader Variants — ADR/timeline? |
| Q-P046-007 | Formal ownership boundary between this document (requirements) and MOBILE_OPTIMIZATION.md (engineering strategy) for future updates? |

---

## 14. Acceptance Criteria

P046 v1.0 is satisfied when all of the following are true:

1. Mobile-first design confirmed; performance is a core pillar; smooth experience across a wide range of Android and iOS devices.
2. Target Frame Rate 60 FPS; Minimum Supported Frame Rate 30 FPS; Loading Times optimized for fast loading, exact targets not defined.
3. Optimization Principles: Mobile First, Efficient Rendering, Efficient Memory Usage, Minimal Battery Consumption, Stable Network Performance.
4. Graphics: scalable quality supported; player-adjustable; quality levels not defined.
5. Memory: usage optimized; unused assets released; budget not defined.
6. Network: traffic minimized; only required data transmitted; bandwidth optimization required.
7. Loading: screens short; assets load efficiently; streaming strategy not defined.
8. Rules: optimizations never change gameplay balance; optimization transparent to players; performance testing required before release.
9. Device Support Matrix, Graphics Presets, Memory Budget, Texture Compression, LOD Strategy, Asset Streaming, Shader Variants, and Network Compression are not invented.
10. No gameplay mechanics invented; no conflict introduced with existing MOBILE_OPTIMIZATION.md engineering strategy.
11. Document version is **P046 v1.0**.

---

## 15. Document Queue (cross-reference to GDD specification sequence)

| Order | Document ID | Title | Status |
|-------|-------------|-------|--------|
| 1–45 | P001–P045 | (prior specs) | Approved as previously recorded |
| 46 | P046 | Performance Optimization Specification (`docs/04-engineering/`) | v1.0 Approved |
| 47 | P047 | UI / UX Design System Specification (`Design/GDD/`) | v1.0 Approved |
| 48 | P048 | Art Direction & Visual Style Specification (`Design/GDD/`) | v1.0 Approved |
| 49 | P049 | Technical Architecture Specification (`docs/02-architecture/`) | v1.0 Approved |
| 50 | P050 | Master Design Bible Specification (`Design/GDD/`) | v1.0 Approved |
| — | Sprint 1 | _(await instructions)_ | Not started |

---

## 16. Change Log

| Version | Date | Summary | Author |
|---------|------|---------|--------|
| **1.0** | 2026-07-31 | Initial Performance Optimization Specification | Documentation Engineer (from brief) |

---

*End of document.*
