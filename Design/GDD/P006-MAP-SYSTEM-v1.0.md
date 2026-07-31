# P006 — Map System Specification

| Field | Value |
|-------|--------|
| Document ID | P006 |
| Title | Map System Specification |
| Version | **1.0** |
| Status | Approved (map system scope only) |
| Project | Project GulfRun |
| Authority | Official source of truth for the **map list**, **map design rules**, and **random selection** behavior stated herein |
| Depends on | [P001](P001-GAME-VISION-v1.0.md), [P002](P002-CORE-GAMEPLAY-LOOP-v1.0.md), [P003](P003-CORE-GAMEPLAY-DESIGN-v1.0.md) (+ P003A), [P007](P007-OBSTACLE-SYSTEM-v1.0.md) |
| Last updated | 2026-07-31 |

**Rules:** Documentation only. No implementation. Do not invent maps or map mechanics beyond this brief. Missing detail → **TODO**.

**Next milestone:** Sprint 1 — do not start until explicitly instructed.

---

## 1. Purpose

Define the Map System: one map per race, official map roster, design rules, and random selection (with future mode overrides).

---

## 2. Map System Overview

| Field | Value |
|-------|--------|
| Maps per race | Each race takes place on **one map** |
| Selection (default) | Maps are selected **randomly** unless another game mode defines otherwise |
| Race distance | Every map has the **same race distance** |
| Balance | Maps are **balanced to provide fair competition** |

### TODO — Overview (not provided)

- [ ] Numeric race distance value (same across maps; value not given)
- [ ] How “balanced” is validated or measured
- [ ] When map is revealed to players (lobby / loading / race start)

---

## 3. Official Maps

| Map ID | Name | Country |
|--------|------|---------|
| Map 01 | **Riyadh** | Saudi Arabia |
| Map 02 | **Jahra** | Kuwait |
| Map 03 | **Dubai** | United Arab Emirates |
| Map 04 | **Doha** | Qatar |
| Map 05 | **Manama** | Bahrain |
| Map 06 | **Muscat** | Oman |

### TODO — Official maps (not provided)

- [ ] Per-map visual references / art direction briefs
- [ ] Per-map length layout (distance is equal; layout details **TODO**)
- [ ] Whether all six ships at launch or subset (**not stated** — do not assume)

---

## 4. Map Design Rules

| Rule ID | Rule |
|---------|------|
| MAP-001 | Each map has its **own visual identity**. |
| MAP-002 | Each map **represents Gulf culture respectfully**. |
| MAP-003 | Every map uses **original environments**. |
| MAP-004 | Every map **contains obstacles**. |
| MAP-005 | Every map **contains item boxes**. |
| MAP-006 | Every map has a **start area**. |
| MAP-007 | Every map has a **finish line**. |
| MAP-008 | Every map has the **same race distance**. |
| MAP-009 | Maps are **balanced to provide fair competition**. |

### Cross-document notes

- Obstacles: every map — **[P007](P007-OBSTACLE-SYSTEM-v1.0.md)**. Types and collision effects remain undefined in P007.
- Item boxes: presence on every map; item-box rules in **P003**; item **types** still undefined.
- Finish line / race outcome: **P003** race rules.

---

## 5. Random Selection Rules (Map Rotation)

| Rule ID | Rule |
|---------|------|
| ROT-001 | Maps are **randomly selected**. |
| ROT-002 | **Future game modes may override** this behavior. |
| ROT-003 | Unless a mode defines otherwise, default selection is random (**§2**). |

### TODO — Rotation (not provided)

- [ ] Uniform vs weighted random
- [ ] Repeat avoidance / cooldown between matches
- [ ] Who selects (server authority intent — engineering; design algorithm **TODO**)
- [ ] Which modes override (no modes beyond default race fully specified)

---

## 6. Future Dependencies

| Dependency | Note |
|------------|------|
| P007 — Obstacle System Specification | Fairness, avoid Jump/Double Jump, collision existence; types/effects still TBD in P007 |
| Item type catalog | Every map contains item boxes — box system **[P008](P008-ITEM-BOX-SYSTEM-v1.0.md)**; item types still TBD |
| Additional game modes | May override random map selection |
| Art / audio production | Visual identity per map; music/SFX not defined here |
| Fair Competition (P001) | Balance requirement MAP-009 |

---

## 7. Explicitly Not Defined (P006)

- Obstacle Types  
- Obstacle Positions  
- Background Art  
- Weather  
- Day/Night  
- Interactive Objects  
- Secrets  
- Events  
- Music  
- Sound Effects  

---

## 8. Open Questions

| ID | Question |
|----|----------|
| Q-P006-001 | Numeric value of the shared race distance? |
| Q-P006-002 | Are all six maps available at Soft Launch / Global Launch? |
| Q-P006-003 | Random selection: uniform or weighted? Repeat protection? |
| Q-P006-004 | When is the selected map shown to players? |
| Q-P006-005 | Which future modes override random selection? |

---

## 9. Acceptance Criteria

P006 v1.0 is satisfied when all of the following are true:

1. One map per race; same race distance on every map; maps balanced for fair competition.  
2. Default selection is random; future modes may override.  
3. Official maps 01–06 are documented with name and country exactly as provided.  
4. Design rules MAP-001–MAP-009 match the brief (visual identity, respectful Gulf culture, original environments, obstacles, item boxes, start area, finish line).  
5. Obstacle types/positions, background art, weather, day/night, interactive objects, secrets, events, music, and sound effects are not invented. Environment audio existence (varies per map) confirmed in **[P035](P035-AUDIO-SYSTEM-v1.0.md)**; per-map sound lists remain undefined.  
6. Future dependencies, open questions, and acceptance criteria are present.  
7. Document version is **P006 v1.0**.

---

## 10. Document Queue

| Order | Document ID | Title | Status |
|-------|-------------|-------|--------|
| 1 | P001 | Game Vision Document | v1.1 Approved |
| 2 | P002 | Core Gameplay Loop | v1.0 Approved |
| 3 | P003 | Core Gameplay Design | v1.0 + P003A |
| 4 | P004 | Main Menu Specification | v1.0 Approved |
| 5 | P005 | Character System Specification | v1.0 Approved |
| 6 | P006 | Map System Specification | **v1.0 Approved** |
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

## 11. Change Log

| Version | Date | Summary | Author |
|---------|------|---------|--------|
| **1.0** | 2026-07-31 | Initial Map System Specification | Documentation Engineer (from Design Owner brief) |

---

*End of document.*
