# P009 — Item & Weapon System Specification

| Field | Value |
|-------|--------|
| Document ID | P009 |
| Title | Item & Weapon System Specification |
| Version | **1.0** |
| Status | Approved (item & weapon *system rules* scope only) |
| Project | Project GulfRun |
| Authority | Official source of truth for **how items are obtained and used**, **weapons as an item category**, **balancing principles**, and **player hold/activation rules** stated herein |
| Depends on | [P001](P001-GAME-VISION-v1.0.md), [P003](P003-CORE-GAMEPLAY-DESIGN-v1.0.md), [P007](P007-OBSTACLE-SYSTEM-v1.0.md), [P008](P008-ITEM-BOX-SYSTEM-v1.0.md) |
| Last updated | 2026-07-31 |

**Rules:** Documentation only. No implementation. Do not invent weapons, item lists, or effects. Missing detail → **TODO**.

**Next milestone:** Sprint 1 — do not start until explicitly instructed.

---

## 1. Purpose

Define system-level rules for Items and Weapons (as an Item category): acquisition, carry limit, consumption, competitive intent, and balancing principles — **without** defining any specific item or weapon.

---

## 2. Item System Overview

| Field | Value |
|-------|--------|
| Acquisition | Items are obtained **only from Item Boxes during races** |
| Carry limit | A player can carry **only one Item at a time** |
| On use | Items are **consumed immediately after use** |
| After use | Players must **collect another Item Box** to obtain a new Item |

### Alignment

- [P008](P008-ITEM-BOX-SYSTEM-v1.0.md): boxes grant items; one item hold; collect-while-holding still **not defined** in P008.  
- P003 control **Use collected item**: activation method still **not defined** (P008); consumption-on-use is defined **here**.

### TODO — Item system (not provided)

- [ ] Catalog of item types (explicitly not defined — §7)  
- [ ] Activation input method  

---

## 3. Weapon System Overview

| Field | Value |
|-------|--------|
| Relationship to items | Weapons are a **category of Items** |
| Purpose | Weapons are designed to **affect opponents during races** |
| Elimination | Weapons must **never permanently eliminate** a player from a race |
| Experience | Weapons must keep the race **competitive and enjoyable** |

### TODO — Weapon system (not provided)

- [ ] Weapon list (explicitly not defined)  
- [ ] How “affect opponents” is realized per weapon (effects not defined)  

---

## 4. Balancing Rules

| Rule ID | Rule |
|---------|------|
| BAL-001 | Every Item must have **advantages and limitations**. |
| BAL-002 | **No Item should guarantee victory**. |
| BAL-003 | Randomness should create **variety without deciding the winner alone**. |
| BAL-004 | **Player skill remains the primary factor**. |

### Alignment

- P001 Fair Competition pillar: skill-primary; random elements enhance variety without dominating outcomes — **reinforced** here.  

### TODO — Balancing (not provided)

- [ ] Numeric balancing values (explicitly not defined)  
- [ ] Probability tables (explicitly not defined)  

---

## 5. Player Rules

| Rule ID | Rule |
|---------|------|
| PLR-001 | **Only one Item** may be held at any time. |
| PLR-002 | **Only one Item** may be activated at a time. |
| PLR-003 | Item activation **consumes the Item**. |
| PLR-004 | After consumption, a new Item requires collecting **another Item Box**. |

### TODO — Player rules (not provided)

- [ ] Whether an activated item’s lingering effect counts as “still activated” (effects not defined)  

---

## 6. Future Dependencies

| Dependency | Note |
|------------|------|
| P008 Item Box System | Sole race acquisition path for items |
| Item / weapon catalog | **Not defined** — future specification |
| Activation UX / controls | Not defined |
| P007 | Future items may interact with obstacles if defined later |
| P001 Fair Competition | BAL-002–BAL-004 |

---

## 7. Explicitly Not Defined (P009)

- Weapon List  
- Power Ups  
- Defensive Items  
- Offensive Items  
- Trap Items  
- Cooldowns  
- Damage  
- Effects  
- Visual Effects  
- Audio  
- Probability  
- Balancing Values  

---

## 8. Open Questions

| ID | Question |
|----|----------|
| Q-P009-001 | Which document will list specific Items and Weapons? |
| Q-P009-002 | Item activation input method? |
| Q-P009-003 | Are all Items weapons, or are there non-weapon Item categories? *(Weapons are a category of Items; other categories **not stated** — do not invent.)* |
| Q-P009-004 | Relationship to P008 collect-while-holding (still undefined)? |

---

## 9. Acceptance Criteria

P009 v1.0 is satisfied when all of the following are true:

1. Items obtained only from Item Boxes during races; one item carry; consumed on use; new item requires another box.  
2. Weapons are an Item category; affect opponents; never permanently eliminate a player; keep races competitive and enjoyable.  
3. Balancing: advantages and limitations; no guaranteed victory; randomness not sole decider; skill primary.  
4. Player rules: one held; one activated at a time; activation consumes.  
5. Weapon list, power ups, defensive/offensive/trap item lists, cooldowns, damage, effects, VFX, audio, probability, and balancing values are not invented. Weapon audio existence confirmed in **[P035](P035-AUDIO-SYSTEM-v1.0.md)**; specific sound details remain future.  
6. Future dependencies, open questions, and acceptance criteria are present.  
7. Document version is **P009 v1.0**.

---

## 10. Document Queue

| Order | Document ID | Title | Status |
|-------|-------------|-------|--------|
| 1–8 | P001–P008 | (prior specs) | Approved as previously recorded |
| 9 | P009 | Item & Weapon System Specification | **v1.0 Approved** |
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
| **1.0** | 2026-07-31 | Initial Item & Weapon System Specification (rules only; no item list) | Documentation Engineer (from Design Owner brief) |

---

*End of document.*
