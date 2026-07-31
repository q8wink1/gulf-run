# P008 — Item Box System Specification

| Field | Value |
|-------|--------|
| Document ID | P008 |
| Title | Item Box System Specification |
| Version | **1.0** |
| Status | Approved (item box system scope only) |
| Project | Project GulfRun |
| Authority | Official source of truth for **item box presence**, **collection**, **hold limit**, **random grant fairness**, and **visual rules** stated herein |
| Depends on | [P001](P001-GAME-VISION-v1.0.md), [P002](P002-CORE-GAMEPLAY-LOOP-v1.0.md), [P003](P003-CORE-GAMEPLAY-DESIGN-v1.0.md), [P006](P006-MAP-SYSTEM-v1.0.md), [P007](P007-OBSTACLE-SYSTEM-v1.0.md) |
| Last updated | 2026-07-31 |

**Rules:** Documentation only. No implementation. Do not invent items or item effects. Missing detail → **TODO**.

**Next milestone:** Sprint 1 — do not start until explicitly instructed.

---

## 1. Purpose

Define the Item Box System: how boxes appear and are collected in races, randomness and fairness constraints, hold limit, and visibility rules — **without** defining item types or effects.

---

## 2. Item Box System Overview

| Field | Value |
|-------|--------|
| Presence | Item Boxes **appear throughout every race** |
| Collection | Players can collect Item Boxes by **touching them** |
| Positions | Item Box positions are **randomized between matches** |
| Hold limit | Players can hold **only one Item at a time** |
| Collect while holding | Collecting another Item Box while already holding an Item is **not defined** |

### Alignment

- P003 ITM-*: random boxes; positions change between matches; one box at a time; random item; types not defined — **refined and confirmed** here.  
- P006 MAP-005: every map contains item boxes — confirmed.  
- P003 control **Use collected item** / P002 “Use obtained items” — activation method **not defined** in P008 (§5).

### TODO — Overview (not provided)

- [ ] Whether boxes despawn after collect or respawn  
- [ ] Shared vs per-player box instances  

---

## 3. Item Box Rules

| Rule ID | Rule |
|---------|------|
| IBX-001 | Item Boxes appear **throughout every race**. |
| IBX-002 | Every Item Box gives **one random Item**. |
| IBX-003 | Item distribution must remain **fair**. |
| IBX-004 | **No player is guaranteed a specific Item**. |
| IBX-005 | Item **probabilities are not defined**. |
| IBX-006 | Positions are **randomized between matches**. |

### TODO — Rules (not provided)

- [ ] How “fair” distribution is measured or enforced  
- [ ] Whether all players see the same box layout within one match (**not stated**)  

---

## 4. Collection Rules

| Rule ID | Rule |
|---------|------|
| COL-IBX-001 | Players collect Item Boxes by **touching them**. |
| COL-IBX-002 | Players can hold **only one Item at a time**. |
| COL-IBX-003 | Collecting another Item Box while already holding an Item is **not defined**. |

### TODO — Collection (not provided)

- [ ] Replace / block / ignore / drop behavior when holding an item and touching a box  
- [ ] Feedback on successful collect  

---

## 5. Player Interaction

| Rule ID | Rule |
|---------|------|
| INT-IBX-001 | Players **collect Item Boxes during races**. |
| INT-IBX-002 | Collected Items **can be used later**. |
| INT-IBX-003 | Item **activation method is not defined**. |

### Cross-document notes

- P003 lists control **Use collected item** — existence of a use action is elsewhere; **how** activation works is **not** defined in P008.  
- P007: future items may interact with obstacles **if defined later** — item catalog still **not** defined here.

### TODO — Interaction (not provided)

- [ ] Input / UI for activation  
- [ ] Timing window for “used later”  

---

## 6. Visual Rules

| Rule ID | Rule |
|---------|------|
| VIS-IBX-001 | Item Boxes must be **clearly visible**. |
| VIS-IBX-002 | Item Boxes must be **easy to recognize**. |
| VIS-IBX-003 | Item Boxes must be **accessible by all players**. |

### TODO — Visual (not provided)

- [ ] Art style / mesh / VFX for the box itself  
- [ ] Colorblind / accessibility specifics beyond “clearly visible”  

---

## 7. Future Dependencies

| Dependency | Note |
|------------|------|
| Item type catalog | **Not defined** as a list — system rules in **[P009](P009-ITEM-WEAPON-SYSTEM-v1.0.md)**; catalog future |
| P003 | Use collected item control; prior ITM rules |
| P006 | Every map contains item boxes |
| P007 | Optional future item–obstacle interaction |
| Fair Competition (P001) | IBX-003, IBX-004 |

---

## 8. Explicitly Not Defined (P008)

- Item Types  
- Weapons  
- Power Ups  
- Item Probabilities  
- Item Icons  
- Item Effects  
- Item Sounds  
- Item Animations  
- Cooldowns  
- Inventory (race hold-slot rules) � cosmetic Inventory is **[P021](P021-INVENTORY-SYSTEM-v1.0.md)**; race item bag rules still **not defined**
- Activation method  
- Collect-while-holding behavior  

---

## 9. Open Questions

| ID | Question |
|----|----------|
| Q-P008-001 | Behavior when collecting a box while already holding an item? |
| Q-P008-002 | Which document defines Item Types and effects? |
| Q-P008-003 | How is item activation performed (touch control mapping)? |
| Q-P008-004 | Do boxes respawn within a race after collection? |
| Q-P008-005 | Same randomized layout for all players in a match, or independent? |

---

## 10. Acceptance Criteria

P008 v1.0 is satisfied when all of the following are true:

1. Item Boxes appear throughout every race; collected by touching; positions randomized between matches; hold only one item at a time; collect-while-holding not defined.  
2. Every box gives one random item; fair distribution; no guaranteed specific item; probabilities not defined.  
3. Collected items can be used later; activation method not defined.  
4. Visual rules: clearly visible, easy to recognize, accessible by all players.  
5. Item types, weapons, power ups, probabilities, icons, effects, sounds, animations, cooldowns, and inventory are not invented.  
6. Future dependencies, open questions, and acceptance criteria are present.  
7. Document version is **P008 v1.0**.

---

## 11. Document Queue

| Order | Document ID | Title | Status |
|-------|-------------|-------|--------|
| 1–7 | P001–P007 | (prior specs) | Approved as previously recorded |
| 8 | P008 | Item Box System Specification | **v1.0 Approved** |
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

## 12. Change Log

| Version | Date | Summary | Author |
|---------|------|---------|--------|
| **1.0** | 2026-07-31 | Initial Item Box System Specification | Documentation Engineer (from Design Owner brief) |

---

*End of document.*
