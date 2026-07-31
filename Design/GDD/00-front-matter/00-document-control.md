# 00 — Document Control

**GDD chapter:** 00  
**Status:** Partial  
**Design Owner:** `[TBD]`  
**Last updated:** 2026-07-31

---

## 0.1 Purpose of this GDD

Hold all gameplay and product design specifications for Project GulfRun. Vision is governed by **P001**.

## 0.2 Scope of authority

| Domain | Source of truth | Status |
|--------|-----------------|--------|
| Project vision & pillars (named) | [P001 Game Vision v1.0](../P001-GAME-VISION-v1.0.md) | Approved (vision scope) |
| Gameplay mechanics | Future GDD specs / this tree when Approved | Not yet defined |
| UI screens & flows | This GDD when Approved | Not yet defined |
| Game modes | This GDD when Approved | Partial fact: 4 players per match (P001) |
| Progression & economy *rules* | Dedicated specs later (P001 Non Goals) | Not defined |
| Monetization *design* | Dedicated specs later | Not defined |
| Multiplayer *design intent* | This GDD when Approved | Partial: real-time multiplayer racing (P001) |
| Technical architecture | `docs/02-architecture/` | External |
| Implementation standards | `docs/03-standards/` | External |

## 0.3 Document status model

| Status | Meaning |
|--------|---------|
| Template | Structure only |
| Draft | Being written by Design Owner |
| Review | Ready for cross-discipline review |
| Approved | Implementation may be planned against this section |
| Deprecated | Superseded; link to replacement |

## 0.4 Versioning

| Field | Value |
|-------|-------|
| GDD / Vision version | P001 Vision **1.1** |
| Product codename | Project GulfRun |
| Working title (player-facing) | **TODO** |
| Project type | Real-time Multiplayer Mobile Racing Game |
| Screen orientation | Landscape only |
| Next milestone | **Sprint 1 — await instructions** |
| P050 note | Master Design Bible Specification — filed in `Design/GDD/`; consolidates P001–P049 only; does not resolve the P020/P042 conflict; specification-brief phase now transitions to **Sprint 1** |
| P047 note | UI / UX Design System Specification — filed in `Design/GDD/`; populates Chapter 19 (design-language layer) and Chapter 29 §29.1/29.2 (accessibility) |
| P048 note | Art Direction & Visual Style Specification — filed in `Design/GDD/`; no prior chapter placeholder existed for this topic |
| P049 note | Technical Architecture Specification — **engineering doc**, filed at `docs/02-architecture/TECHNICAL_ARCHITECTURE.md` per the Gameplay/Engineering SoT split, not in `Design/GDD/`; requirements-level companion to existing `FOLDER_ARCHITECTURE.md` and `CODING_STANDARDS.md` |
| P044 note | Analytics System Specification — **engineering doc**, filed at `docs/02-architecture/ANALYTICS_SYSTEM.md` per the Gameplay/Engineering SoT split, not in `Design/GDD/` |
| P045 note | Monetization System Specification — filed in `Design/GDD/`; populates Chapter 26 (`12-monetization/26-monetization.md`) |
| P046 note | Performance Optimization Specification — **engineering doc**, filed at `docs/04-engineering/PERFORMANCE_OPTIMIZATION_SPECIFICATION.md` per the Gameplay/Engineering SoT split, not in `Design/GDD/`; requirements-level companion to existing `MOBILE_OPTIMIZATION.md` |
| P043 note | Anti-Cheat System Specification — **engineering doc**, filed at `docs/05-security/ANTI_CHEAT_SPECIFICATION.md` per the Gameplay/Engineering SoT split, not in `Design/GDD/`; complements (does not conflict with) existing `docs/05-security/ANTI_CHEAT.md` strategy doc |
| P039 note | Backend Architecture Specification — **engineering doc**, filed at `docs/02-architecture/BACKEND_ARCHITECTURE.md` per the Gameplay/Engineering SoT split, not in `Design/GDD/` |
| P040 note | Database Architecture Specification — **engineering doc**, filed at `docs/02-architecture/DATABASE_ARCHITECTURE.md` per the Gameplay/Engineering SoT split, not in `Design/GDD/` |
| P041 note | Authentication System Specification — **engineering doc**, filed at `docs/02-architecture/AUTHENTICATION_SYSTEM.md` per the Gameplay/Engineering SoT split, not in `Design/GDD/` |
| P042 note | ⚠ **[CONFLICT]** Player Profile System Specification — same title/system as already-Approved **P020**; content differs (Profile Frame/Background status, Display Name editability, extra fields). Escalated to Design Owner; neither document deprecated. See P042 §0. |

## 0.5 Review & approval process

**TODO**

## 0.6 Related documents

| Document | Link / path | Notes |
|----------|-------------|-------|
| Game Vision Document | [P001-GAME-VISION-v1.0.md](../P001-GAME-VISION-v1.0.md) | Official vision SoT |
| Engineering docs | `docs/` | Architecture & standards |

## 0.7 Open questions

| ID | Question | Owner | Status |
|----|----------|-------|--------|
| Q-00-001 | `[QUESTION]` Player-facing title? | Design Owner | Open |
| Q-00-002 | `[QUESTION]` Who is Design Owner of record? | Producer | Open |
