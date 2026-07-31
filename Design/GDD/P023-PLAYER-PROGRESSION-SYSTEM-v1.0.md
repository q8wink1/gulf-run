# P023 ? Player Progression System Specification

| Field | Value |
|-------|--------|
| Document ID | P023 |
| Title | Player Progression System Specification |
| Version | **1.0** |
| Status | Approved (Player Progression system scope only) |
| Project | Project GulfRun |
| Authority | Official source of truth for **progression profile**, **components**, **progression rules**, **stored progress fields**, **sync**, and **design principles** stated herein |
| Depends on | [P001](P001-GAME-VISION-v1.0.md), [P002](P002-CORE-GAMEPLAY-LOOP-v1.0.md), [P012](P012-ECONOMY-SYSTEM-v1.0.md), [P019](P019-LEADERBOARD-SYSTEM-v1.0.md), [P020](P020-PLAYER-PROFILE-SYSTEM-v1.0.md) |
| Last updated | 2026-07-31 |

**Rules:** Documentation only. No implementation. Do not invent features beyond this brief. Missing detail ? **TODO**.

**Next milestone:** Sprint 1 — do not start until explicitly instructed.

---

## 1. Purpose

Define how players progress throughout Project GulfRun: permanent account-linked progression, named components, participation-based rules, stored progress fields, backend synchronization, and design principles ? without XP/level/rank formulas.

---

## 2. System Overview

| Field | Value |
|-------|--------|
| Profile | Every player has a **permanent progression profile** |
| Account | Progression is **linked to the player's account** |
| Sync | Progression is **synchronized with the backend** |

### Alignment

- P001 Long-Term Progression pillar (levels, ranks, cosmetics, seasonal content as goal types) ? **system specification** provided here for Level, Rank, XP, Season Progress, Achievements existence.  
- P020 Player Level / Player Rank display ? values come from this system (**formulas TODO**).  
- P019 leaderboards based on future ranking systems ? Rank relationship **TODO**.  
- P024 Level System ? Player Level / XP detail.  
- P025 Rank System ? Competitive Rank detail.  
- P028 Achievement System ? Achievements detail.

---

## 3. Progression Components

| Component | Status |
|-----------|--------|
| **Player Level** | Defined ? **[P024](P024-LEVEL-SYSTEM-v1.0.md)** |
| **Player Rank** | Defined ? Competitive Rank **[P025](P025-RANK-SYSTEM-v1.0.md)** |
| **Experience (XP)** | Defined ? **[P024](P024-LEVEL-SYSTEM-v1.0.md)** |
| **Season Progress** | Defined � Season SoT **[P030](P030-SEASON-SYSTEM-v1.0.md)** (calculation **TODO**); Battle Pass Progress **[P029](P029-BATTLE-PASS-SYSTEM-v1.0.md)** (relationship **TODO**) |
| **Achievements** | Defined ? **[P028](P028-ACHIEVEMENT-SYSTEM-v1.0.md)** |
| **Future Progression Systems** | Future |

### TODO ? Components (not provided)

- [ ] Achievement catalog / unlock rules ? list still **TODO**; system rules **[P028](P028-ACHIEVEMENT-SYSTEM-v1.0.md)**  
- [ ] Season Progress relationship to P019 Season Leaderboard / P025 seasonal ranks / P029 Battle Pass Progress  
- [x] Player Rank = Competitive Rank (**P025**); separate from Player Level  

---

## 4. Progression Rules

| Rule ID | Rule |
|---------|------|
| PROG-001 | Players progress by **participating in gameplay**. |
| PROG-002 | Progress is **permanent**. |
| PROG-003 | Progress is **saved automatically**. |
| PROG-004 | Progress **cannot be manually modified** by players. |

### TODO ? Rules (not provided)

- [ ] Which gameplay activities grant XP / Rank / Season Progress / Achievements  
- [ ] Whether ?permanent? excludes seasonal resets (Season Reset Rules not defined elsewhere)  

---

## 5. Player Progress (Player Profile Storage)

The following information is stored:

| Field | Status |
|-------|--------|
| **Current Level** | Defined |
| **Current XP** | Defined |
| **Current Rank** | Defined |
| **Progress History** | **Future** |

### Alignment

- P020 displays Player Level / Player Rank ? **aligned** with Current Level / Current Rank.  
- Current XP display on profile ? **TODO** (P020 does not list XP as a display field); Level Display fields in **P024**.

### TODO ? Player progress (not provided)

- [ ] Whether Current XP is shown on Player Profile UI  
- [ ] Progress History contents when future  

---

## 6. Synchronization

| Field | Value |
|-------|--------|
| Sync | Progression data is **synchronized with the backend** |
| Consistency | **Data consistency is required** across all supported devices |

### Alignment

- P001 platforms: iOS / Android ? consistency across supported devices.  
- P020 / P021 account-linked sync pattern ? **consistent**.

---

## 7. Design Principles

| Principle ID | Principle |
|--------------|-----------|
| PROG-DP-001 | Progression should encourage **long-term engagement**. |
| PROG-DP-002 | Progression should reward **active participation**. |
| PROG-DP-003 | Progression must **never** create **Pay-to-Win** advantages. |

### Alignment

- P001 Long-Term Progression / Fair Competition; P013 no P2W ? **reinforced**.

---

## 8. Dependencies

| Dependency | Note |
|------------|------|
| P001 | Long-Term Progression pillar; Fair Competition |
| P002 | Gameplay participation (races / loop) |
| P020 | Displays Level / Rank |
| P019 | Displays Level / Rank; Season Leaderboard |
| P024 | Player Level / XP |
| P025 | Competitive Rank |
| P028 | Achievements |
| P012 | Economy separate from progression formulas |
| P013 | Shop must not create P2W via progression |
| Backend | Sync; authoritative progress; no manual player edit |

---

## 9. Future Specifications

| Topic | Status |
|-------|--------|
| Future Progression Systems | Future |
| Progress History | Future stored field |
| XP Formula | Not defined ? Level detail **P024** (still TBD) |
| Level Formula | Not defined ? Level detail **P024** (still TBD) |
| Rank Formula | Not defined ? Competitive Rank detail **P025** (still TBD) |
| Season Progress Formula | Not defined |
| Daily XP Bonus | Not defined |
| XP Multipliers | Not defined |
| Prestige System | Not defined |
| Catch-up Mechanics | Not defined |
| Achievement catalog | Not defined ? system **P028**; list TBD |

---

## 10. Explicitly Not Defined (P023)

- XP Formula  
- Level Formula  
- Rank Formula  
- Season Progress Formula  
- Daily XP Bonus  
- XP Multipliers  
- Prestige System  
- Catch-up Mechanics  
- Achievement list / rewards (see **P028** for system rules; list still TBD)  
- Season reset interaction with ?progress is permanent?  

---

## 11. Open Questions

| ID | Question |
|----|----------|
| Q-P023-001 | XP / Level / Rank formulas ? which future doc? | **Partial:** Level/XP structure **P024**; Competitive Rank **P025**; formulas still TODO |
| Q-P023-002 | What gameplay actions grant which progress components? |
| Q-P023-003 | Player Rank vs competitive ranking / leaderboard ranking? | **Partial:** Competitive Rank = Player Rank (**P025**); Leaderboard Integration TBD |
| Q-P023-004 | Season Progress vs Season Leaderboard (P019) / season reset? | **Partial:** Battle Pass **P029**; formula/relationship still TODO |
| Q-P023-005 | Achievements catalog and display (vs P020 Achievements Display not defined)? | **Partial:** system **P028**; list/categories TBD; Profile placement TBD |
| Q-P023-006 | Is Current XP shown on Player Profile? | **Partial:** Current XP is a Level Display field (**P024**); Profile placement TODO |
| Q-P023-007 | How does ?progress is permanent? interact with seasons? |

---

## 12. Acceptance Criteria

P023 v1.0 is satisfied when all of the following are true:

1. Every player has a permanent progression profile linked to the account and synced with the backend.  
2. Components: Player Level, Player Rank, Experience (XP), Season Progress, Achievements; Future Progression Systems future.  
3. Progress via gameplay participation; permanent; auto-saved; not manually modifiable by players.  
4. Stored: Current Level, Current XP, Current Rank; Progress History future.  
5. Backend sync; data consistency required across supported devices.  
6. Principles: long-term engagement; reward active participation; never Pay-to-Win.  
7. XP/Level/Rank/Season formulas, daily XP bonus, multipliers, prestige, and catch-up are not invented.  
8. Document version is **P023 v1.0**.

---

## 13. Document Queue

| Order | Document ID | Title | Status |
|-------|-------------|-------|--------|
| 1?22 | P001?P022 | (prior specs) | Approved as previously recorded |
| 23 | P023 | Player Progression System Specification | **v1.0 Approved** |
| 24 | P024 | Level System Specification | **v1.0 Approved** |
| 25 | P025 | Rank System Specification | **v1.0 Approved** |
| 26 | P026 | Daily Challenges System Specification | **v1.0 Approved** |
| 27 | P027 | Weekly Challenges System Specification | **v1.0 Approved** |
| 28 | P028 | Achievement System Specification | **v1.0 Approved** |
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

## 14. Change Log

| Version | Date | Summary | Author |
|---------|------|---------|--------|
| **1.0** | 2026-07-31 | Initial Player Progression System Specification | Documentation Engineer (from Design Owner brief) |

---

*End of document.*
