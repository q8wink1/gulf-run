# P003 ? Core Gameplay Design Specification

| Field | Value |
|-------|--------|
| Document ID | P003 |
| Title | Core Gameplay Design Specification |
| Version | **1.0** (+ **P003A** conflict resolution) |
| Status | Approved (core race control & race rules scope only) |
| Project | Project GulfRun |
| Authority | Official source of truth for **how the player controls the character during a race** and the **race / item-box rules** stated herein |
| Depends on | [P001](P001-GAME-VISION-v1.0.md), [P002](P002-CORE-GAMEPLAY-LOOP-v1.0.md) |
| Last updated | 2026-07-31 |

**Rules:** Documentation only. No implementation. No invented mechanics. Missing detail ? **TODO**.

**Next milestone:** Sprint 1 — do not start until explicitly instructed.

---

## 1. Purpose

Define how the player controls the character during a race, plus the official movement, control, race, and item-box rules supplied for P003.

---

## 2. Gameplay Overview

| Field | Value |
|-------|--------|
| Genre | Real-time Multiplayer Side Scrolling Racing Game |
| Players | 4 |
| Platform | iOS, Android |
| Orientation | Landscape |
| Camera | Side Scrolling Camera |
| Match structure | Four players compete simultaneously in a race toward a finish line |

**Control summary (only what is defined):**

- Movement along the course is **automatic continuous running** (see �3).
- Player-triggered actions are limited to **Jump**, **Double Jump**, and **Use collected item** (see �4).
- **Item boxes** may be collected under the rules in �6; **item types are not defined**.

---

## 3. Movement Rules

| Rule ID | Rule |
|---------|------|
| MOV-001 | The player **continuously runs automatically**. |
| MOV-002 | The player **cannot stop**. |
| MOV-003 | The player **cannot move backwards**. |
| MOV-004 | The player **always moves toward the finish line**. |

### TODO ? Movement (not provided)

- [ ] Run speed (constant vs variable)
- [ ] Lane / vertical movement (if any)
- [ ] Collision response with world (Physics not defined ? �8)
- [ ] Relationship between automatic run and camera framing

---

## 4. Player Controls

### 4.1 Defined controls

| Control ID | Action | Status |
|------------|--------|--------|
| CTL-001 | **Jump** | Defined (existence only; feel/tuning TODO) |
| CTL-002 | **Double Jump** | Defined (existence only; feel/tuning TODO) |
| CTL-003 | **Use collected item** | Defined (existence only; item effects TODO ? item types not in P003) |

### 4.2 Explicit boundary

**Nothing else is defined yet** as a player control in P003.

### TODO ? Controls (not provided)

- [ ] Touch layout / button placement (Landscape)
- [ ] Input timing windows for Jump / Double Jump
- [ ] Whether Double Jump requires a prior Jump mid-air (assumed common pattern ? **not stated**; leave as TODO, do not assume)
- [ ] How ?Use collected item? is triggered on touch
- [ ] Whether a collected item is held in a single slot (implied by item-box ?one box at a time? ? slot UI **TODO**)

---

## 5. Race Rules

| Rule ID | Rule |
|---------|------|
| RAC-001 | **Four players** compete **simultaneously**. |
| RAC-002 | The **first player reaching the finish line wins** (First Place). |
| RAC-003 | The **remaining players are ranked according to finish order** (Second / Third / Fourth Place). |

> **Race rules SoT:** [P010 ? Race Rules Specification](P010-RACE-RULES-v1.0.md) (start, during-race actions, end condition, Disconnection/AFK existence).

### TODO ? Race (not provided in P003; see P010)

- [ ] Tie rules ? still open (P010 Q-P010-003)  
- [ ] Disconnection / AFK **rules** ? systems exist; rules not defined (P010)  
- [ ] Future alternate end rules ? placeholder in P010  

Detail for countdown, equal start, ?all finish or future rule?: **P010**.

---

## 6. Item Box Rules

| Rule ID | Rule |
|---------|------|
| ITM-001 | **Random item boxes** appear throughout the race. |
| ITM-002 | Their **positions change between matches**. |
| ITM-003 | Each player can **collect one box at a time** / hold **one item at a time** (see P008). |
| ITM-004 | The **item received is random**. |
| ITM-005 | **Actual item types are NOT defined** in this document. |

> **Item box SoT:** [P008 ? Item Box System Specification](P008-ITEM-BOX-SYSTEM-v1.0.md). P003 ITM-* remain summary rules; P008 is the dedicated specification. Collect-while-holding and activation method are **not defined** (P008).

### TODO ? Item boxes

See P008 / P009. Item/weapon **lists** remain a **future specification**.

---

## 7. Future Dependencies

Referenced only as future work. **Not defined** by P003.

| Dependency | Note |
|------------|------|
| Weapons | Not defined as a list ? category rules in **[P009](P009-ITEM-WEAPON-SYSTEM-v1.0.md)** |
| Maps | Not defined |
| Characters | Not defined |
| Power Ups | Not defined (distinct from unnamed random items ? types still undefined) |
| Obstacles | **System exists; present in every race.** Players avoid obstacles during races. Overview/fairness/interaction/collision-existence: **[P007](P007-OBSTACLE-SYSTEM-v1.0.md)**. Types, damage, collision *effects*, physics still **not** defined (P007 �7). |
| Damage | Not defined |
| Respawn | Not defined |
| Physics | Not defined |
| Animations | Not defined |
| Economy | Currencies/wallets: **[P012](P012-ECONOMY-SYSTEM-v1.0.md)**. Rewards/prices/store still not defined. |
| XP | Not defined |
| Store | Not defined |
| Voice Chat | Not defined |
| Progression | Not defined |
| Item type catalog / Item Boxes | Boxes: **[P008](P008-ITEM-BOX-SYSTEM-v1.0.md)**. Item/weapon **rules**: **[P009](P009-ITEM-WEAPON-SYSTEM-v1.0.md)**. Specific item/weapon **lists** still future. |
| Friends | Friend system exists (P004); details future | Future specification |
| Clans | Clan system exists (P004); details future | Future specification |
| Shop / Store | Exists (P004); details future | Future specification |
| Challenges | Exists (P004); details future | Future specification |
| Settings | Exists (P004); details future | Future specification |
| Login | Referenced before Main Menu (P004); not specified | TODO / future |

---

## 8. Explicitly Not Defined (P003)

P003 does **not** define detailed rules for:

- Weapons  
- Maps  
- Characters  
- Power Ups  
- **Obstacle types, damage, collision effects, physics, etc.** (see P007 �7 ? still not defined). Existence, fairness, Jump/Double Jump over obstacles, and collision *existence*: **P007**.  
- Damage  
- Respawn  
- Physics  
- Animations  
- Economy  
- XP  
- Store  
- Voice Chat  
- Progression  
- Item types  

---

## 8A. Obstacle system (existence only ? P003A)

| Field | Official decision (P003A) |
|-------|---------------------------|
| Obstacle system | **Exists** |
| Presence | Obstacles are part of **every race** |
| Player activity | Players **avoid obstacles** during races |
| Detailed specification | **P007 ? Obstacle System Specification** (v1.0). Types/damage/collision effects remain undefined per P007. |
| Still undefined | Obstacle types, damage, animations, physics, collision effects, recovery, respawn (P007 �5?�7) |

Do **not** invent obstacle types or collision effects; follow **P007**.

---

## 9. Conflicts & Cross-Document Notes

| ID | Note | Status |
|----|------|--------|
| CFL-003 | P002 ?Avoid obstacles? vs P003 ?Obstacles not defined.? **P003A:** system exists; every race; avoid during race. **P007:** Obstacle System Specification authored (fairness, interaction, collision existence). Types/damage/effects still deferred within P007 Non Goals. Former placeholder ID **P008** for this topic is **superseded by P007**. | P002, P003, P007 | **Resolved (P003A + P007)** |
| CFL-004 | P002 lists **Jump** but not **Double Jump**. P003 adds **Double Jump** as an official control. **P003 controls take precedence** for in-race actions. | Resolved (P003 supersedes for controls) |

---

## 10. Open Questions

| ID | Question |
|----|----------|
| Q-P003-001 | Exact Double Jump rules (timing, mid-air requirement)? |
| Q-P003-002 | Touch control layout for Jump / Double Jump / Use item? |
| Q-P003-003 | If player holds an item and contacts another item box ? replace, block, or other? |
| Q-P003-004 | Tie-break when two players reach the finish line together? |
| Q-P003-006 | Which future document defines item types? |
| Q-P003-007 | Run speed and any allowed vertical/lane movement? |

---

## 11. Acceptance Criteria

P003 v1.0 is satisfied when all of the following are true:

1. Gameplay overview records genre, 4 players, iOS/Android, landscape, side-scrolling camera.  
2. Movement rules include automatic continuous run, cannot stop, cannot move backwards, always toward finish line ? and no extra movement verbs.  
3. Player controls list only Jump, Double Jump, Use collected item.  
4. Race rules state simultaneous 4-player competition, first to finish wins, others ranked by finish order.  
5. Item box rules match ITM-001?ITM-005; item types remain undefined.  
6. Listed ?Not Defined? topics have no invented mechanics.  
7. Future dependencies, open questions, and acceptance criteria are present.  
8. Document version is **P003 v1.0**.  

**P003A addendum:** CFL-003 resolved; obstacle system specified in **P007** (types/effects still undefined per P007).

---

## 12. Document Queue

| Order | Document ID | Title | Status |
|-------|-------------|-------|--------|
| 1 | P001 | Game Vision Document | v1.1 Approved |
| 2 | P002 | Core Gameplay Loop | v1.0 Approved |
| 3 | P003 | Core Gameplay Design | v1.0 Approved (+ **P003A** CFL-003) |
| 4 | P004 | Main Menu Specification | v1.0 Approved |
| 5 | P005 | Character System Specification | v1.0 Approved |
| 6 | P006 | Map System Specification | v1.0 Approved |
| 7 | P007 | Obstacle System Specification | v1.0 Approved |
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

## 13. Change Log

| Version | Date | Summary | Author |
|---------|------|---------|--------|
| 1.0 | 2026-07-31 | Initial Core Gameplay Design Specification | Documentation Engineer (from Design Owner brief) |
| **1.0 + P003A** | 2026-07-31 | Resolve CFL-003: obstacle system exists; every race; avoid during race; details ? P008 | Documentation Engineer (Design Owner decision) |

---

*End of document.*
