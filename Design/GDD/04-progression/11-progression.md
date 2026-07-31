# 11 — Progression

**GDD chapter:** 11  
**Status:** Partial — synced to P023 / P024 / P025 / P028  
**Design Owner:** `[TBD]`  
**Last updated:** 2026-07-31

> Progression SoT: [P023](../P023-PLAYER-PROGRESSION-SYSTEM-v1.0.md).  
> Player Level SoT: [P024](../P024-LEVEL-SYSTEM-v1.0.md).  
> Competitive Rank SoT: [P025](../P025-RANK-SYSTEM-v1.0.md).  
> Achievements SoT: [P028](../P028-ACHIEVEMENT-SYSTEM-v1.0.md).  
> Season System SoT: [P030](../P030-SEASON-SYSTEM-v1.0.md).  
> **Player Level and Player Rank are separate systems** (P025). Do not invent formulas or achievement lists.

---

## 11.1 Progression philosophy

| Principle | Status |
|-----------|--------|
| Encourage long-term engagement | **P023** / **P028** |
| Reward active participation | **P023** |
| Never create Pay-to-Win advantages | **P023** / **P025** |
| Level motivates continued play; fair; no gameplay advantages | **P024** |
| Ranks reflect skill; fair; never purchasable; never P2W | **P025** |
| Achievements reward long-term accomplishments; completed once | **P028** |

## 11.2 Progression tracks

| Track ID | Name | What increases | Cap / reset rules | Status |
|----------|------|----------------|-------------------|--------|
| PROG-LVL | Player Level | Level via XP | Max **not defined**; cannot lose levels; XP carries over | **P024** |
| PROG-RNK | Competitive Rank (Player Rank) | Rank Progress via competitive races | Seasonal; may increase/decrease; promo/demo exist; max **not defined**; reset **not defined** | **P025** |
| PROG-XP | Experience (XP) | XP (permanent accumulation) | Formula / sources **not defined** | **P024** / **P023** |
| PROG-SEA | Season Progress | Season Progress | Calculation **not defined**; Season SoT **P030** | **P030** / **P023** |
| PROG-ACH | Achievements | Achievement progress | Completed once; permanently completed; list **not defined** | **P028** |
| — | Future Progression Systems | **TODO** | **TODO** | Future |

## 11.3 XP / points / ranks (if any)

| Field | Value |
|-------|-------|
| Level currency | **XP** (P024) |
| Rank progress | Rank Progress via competitive races (P025); formula **not defined** |
| Level vs Rank | **Separate systems** (P025) |
| Achievement Points | **Not defined** (P028) |
| Rank display | Current Rank, Rank Icon, Rank Progress, Next Rank, Current Season Rank (P025) |
| Level display | Current Level, Current XP, XP Required, XP Progress Bar (P024) |
| Achievement display | Name, Description, Progress, Completion Status, Reward Status (P028) |

## 11.4 Prestige / season reset (if any)

Prestige **not defined**. Competitive ranks are **seasonal** with independent ladders; Season Reset **not defined** (P025). Achievements do **not** reset (P028).

## 11.5 Power vs cosmetics boundary

| Affects power? | Examples (when defined) | Rule |
|----------------|-------------------------|------|
| No (required) | Cosmetics (P022); Shop (P013); Player Level (P024); Competitive Rank not purchasable (P025) | Never P2W / never buy rank |
| Yes | — | **Not defined** — do not invent power progression |

## 11.6 Level up

Level Up notification required (P024). Future rewards may exist; reward rules **not defined**.

## 11.7 Open questions

See P023 §11, P024 §12, P025 §12, and P028 §12.
