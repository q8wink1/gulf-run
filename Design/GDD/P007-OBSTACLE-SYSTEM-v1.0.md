# P007 — Obstacle System Specification

| Field | Value |
|-------|--------|
| Document ID | P007 |
| Title | Obstacle System Specification |
| Version | **1.0** |
| Status | Approved (obstacle system scope only) |
| Project | Project GulfRun |
| Authority | Official source of truth for **obstacle existence**, **fairness rules**, **player interaction with obstacles**, and **collision existence** (effects undefined) |
| Depends on | [P001](P001-GAME-VISION-v1.0.md), [P002](P002-CORE-GAMEPLAY-LOOP-v1.0.md), [P003](P003-CORE-GAMEPLAY-DESIGN-v1.0.md) (+ P003A), [P006](P006-MAP-SYSTEM-v1.0.md) |
| Supersedes placeholder | Prior queue label **P008 — Obstacle System** (P003A) is fulfilled by **this document (P007)** |
| Last updated | 2026-07-31 |

**Rules:** Documentation only. No implementation. Do not invent obstacle types or collision effects. Missing detail → **TODO**.

**Next milestone:** Sprint 1 — do not start until explicitly instructed.

---

## 1. Purpose

Define the Obstacle System at the level of presence, fairness, allowed player interactions, and the fact that collision exists without defining its effects.

---

## 2. Obstacle System Overview

| Field | Value |
|-------|--------|
| Presence | Obstacles **exist on every map** |
| Role | Obstacles are **part of the race challenge** |
| Player requirement | Players must **avoid obstacles** while racing |
| Placement | Obstacle placement **varies between maps** |
| Future modes | Obstacle **positions may change** in future game modes |

### Alignment

- P003A / P006: obstacles in every race / every map — confirmed here.  
- P002 Stage 8: “Avoid obstacles” — confirmed here.  
- Obstacle **types** remain **not defined** (§7).

### TODO — Overview (not provided)

- [ ] Density / count guidelines per map  
- [ ] Authority for placement (design vs tooling) — not stated  

---

## 3. Obstacle Rules

| Rule ID | Rule |
|---------|------|
| OBS-001 | Obstacles **never make a race impossible**. |
| OBS-002 | Every obstacle must be **avoidable**. |
| OBS-003 | Obstacle placement must remain **fair**. |
| OBS-004 | **No player receives an unfair advantage** because of obstacle placement. |
| OBS-005 | Obstacles exist on **every map** and are part of the **race challenge**. |
| OBS-006 | Placement **varies between maps**; positions **may change** in future game modes. |

### TODO — Rules (not provided)

- [ ] How fairness of placement is reviewed or tested  
- [ ] Mode-specific position override list (future modes)  

---

## 4. Player Interaction

Players may:

| Interaction | Status |
|-------------|--------|
| **Jump** over obstacles | Defined (allowed) |
| **Double Jump** over obstacles | Defined (allowed) |
| **Use future items** to interact with obstacles **if defined later** | Placeholder only — item interactions **not** defined in P007 |

**Nothing else is defined.**

Controls for Jump / Double Jump: [P003](P003-CORE-GAMEPLAY-DESIGN-v1.0.md).

### TODO — Interaction (not provided)

- [ ] Which future items (if any) interact with obstacles  
- [ ] Whether sliding / ducking / other moves exist — **not defined; do not invent**  

---

## 5. Collision Rules

| Rule ID | Rule |
|---------|------|
| COL-001 | Obstacle **collision exists**. |
| COL-002 | Collision **effects are not defined**. |
| COL-003 | **Damage is not defined**. |
| COL-004 | **Recovery behavior is not defined**. |
| COL-005 | **Respawn behavior is not defined**. |

### TODO — Collision (not provided)

- [ ] Dedicated future specification for collision effects / damage / recovery / respawn (document ID **TODO**)

---

## 6. Future Dependencies

| Dependency | Note |
|------------|------|
| P003 | Jump, Double Jump controls |
| P006 | Every map contains obstacles; placement varies by map |
| Item system | Future items may interact with obstacles if defined later — item/weapon **rules**: [P009](P009-ITEM-WEAPON-SYSTEM-v1.0.md); catalog still TBD |
| Future game modes | May change obstacle positions |
| Collision effects spec | Not defined — future |
| Fair Competition (P001) | OBS-003, OBS-004 |

---

## 7. Explicitly Not Defined (P007)

- Obstacle Types  
- Obstacle Damage  
- Obstacle Animations  
- Obstacle Physics  
- Moving Obstacles  
- Environmental Hazards  
- Weather  
- Map Events  
- Collision effects  
- Damage  
- Recovery behavior  
- Respawn behavior  

---

## 8. Open Questions

| ID | Question |
|----|----------|
| Q-P007-001 | Which document will define collision effects, damage, recovery, and respawn? |
| Q-P007-002 | Will any item types be allowed to interact with obstacles (and in which spec)? |
| Q-P007-003 | Obstacle density / count targets per map? |
| Q-P007-004 | Which future modes change obstacle positions? |

---

## 9. Acceptance Criteria

P007 v1.0 is satisfied when all of the following are true:

1. Obstacles exist on every map; part of race challenge; players must avoid them; placement varies by map; future modes may change positions.  
2. Rules: never impossible; always avoidable; fair placement; no unfair advantage from placement.  
3. Player interaction limited to Jump over, Double Jump over, and future items if defined later — nothing else.  
4. Collision exists; effects, damage, recovery, and respawn are explicitly not defined.  
5. Obstacle types, damage, animations, physics, moving obstacles, environmental hazards, weather, and map events are not invented.  
6. Future dependencies, open questions, and acceptance criteria are present.  
7. Document version is **P007 v1.0**.  
8. Prior P008 placeholder for Obstacle System is documented as superseded by P007.

---

## 10. Document Queue

| Order | Document ID | Title | Status |
|-------|-------------|-------|--------|
| 1 | P001 | Game Vision Document | v1.1 Approved |
| 2 | P002 | Core Gameplay Loop | v1.0 Approved |
| 3 | P003 | Core Gameplay Design | v1.0 + P003A |
| 4 | P004 | Main Menu Specification | v1.0 Approved |
| 5 | P005 | Character System Specification | v1.0 Approved |
| 6 | P006 | Map System Specification | v1.0 Approved |
| 7 | P007 | Obstacle System Specification | **v1.0 Approved** |
| 8 | P008 | Item Box System Specification | v1.0 Approved |
| 9 | P009 | Item & Weapon System Specification | v1.0 Approved |
| 10 | P010 | Race Rules Specification | v1.0 Approved |
| 11 | P011 | Post Race Results Specification | v1.0 Approved |
| 12 | P012 | Economy System Specification | v1.0 Approved |
| 13 | P013 | Shop System Specification | v1.0 Approved |
| 14 | P014 | Friends System Specification | v1.0 Approved |
| 15 | P015 | Clan System Specification | v1.0 Approved |
| 16 | P016 | Voice Chat System Specification | v1.0 Approved |
| 17 | P017 | Matchmaking System Specification | v1.0 Approved |
| 18 | P018 | Private Room System Specification | v1.0 Approved |
| 19 | P019 | Leaderboard System Specification | v1.0 Approved |
| 20 | P020 | Player Profile System Specification | v1.0 Approved |
| 21 | P021 | Inventory System Specification | v1.0 Approved |
| 22 | P022 | Cosmetics System Specification | v1.0 Approved |
| 23 | P023 | Player Progression System Specification | v1.0 Approved |
| 24 | P024 | Level System Specification | v1.0 Approved |
| 25 | P025 | Rank System Specification | v1.0 Approved |
| 26 | P026 | Daily Challenges System Specification | v1.0 Approved |
| 27 | P027 | Weekly Challenges System Specification | v1.0 Approved |
| 28 | P028 | Achievement System Specification | v1.0 Approved |
| 29 | P029 | Battle Pass System Specification | v1.0 Approved |
| 30 | P030 | Season System Specification | v1.0 Approved |
| 31 | P031 | Live Events System Specification | v1.0 Approved |
| 32 | P032 | Notification System Specification | v1.0 Approved |
| 33 | P033 | Inbox (Mail) System Specification | **v1.0 Approved** — [P033](P033-INBOX-MAIL-SYSTEM-v1.0.md) |
| 34 | P034 | Settings System Specification | **v1.0 Approved** — [P034](P034-SETTINGS-SYSTEM-v1.0.md) |
| 35 | P035 | Audio System Specification | **v1.0 Approved** — [P035](P035-AUDIO-SYSTEM-v1.0.md) |
| 36 | P036 | Music System Specification | **v1.0 Approved** — [P036](P036-MUSIC-SYSTEM-v1.0.md) |
| 37 | P037 | Localization System Specification | **v1.0 Approved** — [P037](P037-LOCALIZATION-SYSTEM-v1.0.md) |
| 38 | P038 | Tutorial System Specification | **v1.0 Approved** — [P038](P038-TUTORIAL-SYSTEM-v1.0.md) |
| 39 | P039 | Backend Architecture Specification (engineering doc — docs/02-architecture/) | **v1.0 Approved** |
| 40 | P040 | Database Architecture Specification (engineering doc — docs/02-architecture/) | **v1.0 Approved** |
| 41 | P041 | Authentication System Specification (engineering doc — docs/02-architecture/) | **v1.0 Approved** |
| 42 | P042 | Player Profile System Specification [CONFLICT with P020] | **v1.0 Approved-per-brief** |
| 43 | P043 | Anti-Cheat System Specification (engineering doc — docs/05-security/) | **v1.0 Approved** |
| 44 | P044 | Analytics System Specification (engineering doc — docs/02-architecture/) | **v1.0 Approved** |
| 45 | P045 | Monetization System Specification | **v1.0 Approved** |
| 46 | P046 | Performance Optimization Specification | **v1.0 Approved** |
| 47 | P047 | UI / UX Design System Specification | **v1.0 Approved** |
| 48 | P048 | Art Direction & Visual Style Specification | **v1.0 Approved** |
| 49 | P049 | Technical Architecture Specification | **v1.0 Approved** |
| 50 | P050 | Master Design Bible Specification | **v1.0 Approved** |
| — | Sprint 1 | _(await instructions)_ | Not started |

---

## 11. Change Log

| Version | Date | Summary | Author |
|---------|------|---------|--------|
| **1.0** | 2026-07-31 | Initial Obstacle System Specification; fulfills former P008 obstacle placeholder | Documentation Engineer (from Design Owner brief) |

---

*End of document.*
