# 08 — Interaction Rules

**GDD chapter:** 08  
**Status:** Partial — synced to P003 + P003A + P007  
**Design Owner:** `[TBD]`  
**Last updated:** 2026-07-31

> Sources: [P003](../P003-CORE-GAMEPLAY-DESIGN-v1.0.md), [P007](../P007-OBSTACLE-SYSTEM-v1.0.md). Do not invent obstacle types or collision effects.

---

## 8.1 Interaction model overview

Real-time side-scrolling race; automatic forward run; Jump / Double Jump / Use collected item; **item boxes per P008** (types undefined); obstacles per P007.

## 8.2 Resolution rules

| Interaction | Who initiates | How resolved | Outcome types | Status |
|-------------|---------------|--------------|---------------|--------|
| Reach finish line | Player (by racing) | First to finish wins; others by finish order | Win / ranked placements | P003 RAC-* |
| Collect item box | Player touching box | One random item; hold one at a time; collect-while-holding **not defined** | Hold item (types TBD) | **P008** |
| Use collected item | Player | Hold one; activate one at a time; **consumed on use**; new item needs another box | Consume item | **P009** + P008 boxes |
| Avoid obstacles | Player during race | Jump / Double Jump over; future items if defined later | Avoidance | P007 |
| Obstacle collision | Contact with obstacle | **Collision exists**; effects **not defined** | **TODO** | P007 COL-* |
| Damage | — | **Not defined** | — | P007 |

## 8.3 Damage / scoring / success metrics

| Metric | Definition | Where it applies | Status |
|--------|------------|------------------|--------|
| Finish order | First = win; others ranked by finish order | Race results | P003 |
| Damage | **Not defined** | — | P007 |

## 8.4 Stacking, immunities, counters

**TODO** / not defined.

## 8.5 Death / failure / respawn / recovery

Recovery and respawn **not defined** (P007).

## 8.6 Fairness principles (design)

Obstacle placement fair; never impossible; always avoidable; no unfair advantage from placement (P007 OBS-*). Fair Competition pillar (P001).

## 8.7 Open questions

See P007 §8. CFL-003 **resolved** (P003A + P007).
