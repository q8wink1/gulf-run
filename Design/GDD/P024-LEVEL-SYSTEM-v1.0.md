# P024 — Level System Specification

| Field | Value |
|-------|--------|
| Document ID | P024 |
| Title | Level System Specification |
| Version | **1.0** |
| Status | Approved (Player Level system scope only) |
| Project | Project GulfRun |
| Authority | Official source of truth for **Player Level**, **XP accumulation / carry-over**, **level display fields**, **level-up notification**, **sync**, and **level design principles** stated herein |
| Depends on | [P001](P001-GAME-VISION-v1.0.md), [P023](P023-PLAYER-PROGRESSION-SYSTEM-v1.0.md), [P020](P020-PLAYER-PROFILE-SYSTEM-v1.0.md) |
| Last updated | 2026-07-31 |

**Rules:** Documentation only. No implementation. Do not invent features beyond this brief. Missing detail → **TODO**.

**Next milestone:** Sprint 1 — do not start until explicitly instructed.

---

## 1. Purpose

Define the Player Level System for Project GulfRun: permanent Level, XP gain and level-up structure, display fields, level-up flow, synchronization, and fairness principles — without XP formulas, sources, maximum level, or rewards.

---

## 2. System Overview

| Field | Value |
|-------|--------|
| Count | Every player has **one permanent Player Level** |
| Meaning | Player Level represents **long-term account progression** |
| Sync | Player Level is **synchronized with the backend** |
| Separation | Player Level is **separate** from Competitive Rank — **[P025](P025-RANK-SYSTEM-v1.0.md)** |

### Alignment

- P023: Player Level and Experience (XP) components — **this document** details Level structure and rules.  
- P001 Long-Term Progression — levels as a goal type — **supported**.

---

## 3. Level Structure

| Rule ID | Rule |
|---------|------|
| LVL-001 | Every player starts at **Level 1**. |
| LVL-002 | Players gain **Experience Points (XP)**. |
| LVL-003 | When **enough XP** is earned, the player **levels up**. |
| LVL-004 | **Maximum Level is not defined.** |

### TODO — Level structure (not provided)

- [ ] Maximum Level  
- [ ] XP amount required per level (XP Formula / Level Formula)  
- [ ] Definition of “enough XP” thresholds  

---

## 4. Level Progression

| Rule ID | Rule |
|---------|------|
| LVL-PRG-001 | XP is accumulated **permanently**. |
| LVL-PRG-002 | **Unused XP carries over** after leveling up. |
| LVL-PRG-003 | Players **cannot lose** Player Levels. |

### Alignment

- P023: Progress is permanent; cannot be manually modified — **consistent**.  
- XP Formula / XP Sources — **not defined** (§10).

---

## 5. Level Display

Display:

| Field | Status |
|-------|--------|
| **Current Level** | Defined |
| **Current XP** | Defined |
| **XP Required For Next Level** | Defined |
| **XP Progress Bar** | Defined |

### Alignment

- P023 stores Current Level / Current XP — **aligned**.  
- P020 displays Player Level; Current XP on Profile — **partially addressed** (display fields defined here; screen placement **TODO**).  
- Q-P023-006 (Show Current XP on Profile?) — display of Current XP is required somewhere; Profile placement **TODO**.

### TODO — Level display (not provided)

- [ ] Which screens show Level Display (Profile, Main Menu, Results, dedicated)  
- [ ] Progress bar visual rules  

---

## 6. Level Up Flow

When a player levels up:

| Behavior | Status |
|----------|--------|
| Display a **Level Up notification** | Defined |
| **Future rewards** may exist | Future |
| **Reward rules** are not defined | Not defined |

```
Player earns XP
↓
XP threshold for next level reached
↓
Player Level increases
↓
Unused XP carries over
↓
Level Up notification displayed
↓
(Future rewards — rules not defined)
```

### TODO — Level up (not provided)

- [ ] Notification UI / timing (immediate vs Results screen)  
- [ ] Level Rewards catalog when defined  

---

## 7. Synchronization

| Field | Value |
|-------|--------|
| Sync | Level and XP data are **synchronized with the backend** |
| Consistency | Progress must remain **consistent across all supported devices** |

### Alignment

- P023 sync / device consistency — **consistent**.  
- P001 platforms iOS / Android.

---

## 8. Design Principles

| Principle ID | Principle |
|--------------|-----------|
| LVL-DP-001 | Level progression should **motivate continued play**. |
| LVL-DP-002 | Level progression should **remain fair**. |
| LVL-DP-003 | Player Level must **never** provide **gameplay advantages**. |

### Alignment

- P023 no Pay-to-Win; P001 Fair Competition; P022 cosmetics no gameplay advantages — **reinforced** for Level.

---

## 9. Dependencies

| Dependency | Note |
|------------|------|
| P023 Player Progression | Parent progression profile; XP component |
| P020 Player Profile | Displays Player Level; XP UI placement TODO |
| P001 | Long-Term Progression; Fair Competition |
| P011 Post Race Results | May show XP later — amounts **not defined** |
| Backend | Authoritative Level / XP; sync |

---

## 10. Future Specifications

| Topic | Status |
|-------|--------|
| Future level-up rewards | Future (rules not defined) |
| Maximum Level | Not defined |
| XP Formula | Not defined |
| XP Sources | Not defined |
| Level Rewards | Not defined |
| Prestige | Not defined |
| Level Milestones | Not defined |
| Bonus XP | Not defined |
| Catch-up System | Not defined |

---

## 11. Explicitly Not Defined (P024)

- Maximum Level  
- XP Formula  
- XP Sources  
- Level Rewards  
- Prestige  
- Level Milestones  
- Bonus XP  
- Catch-up System  
- Level-up notification UI details  

---

## 12. Open Questions

| ID | Question |
|----|----------|
| Q-P024-001 | Maximum Level value / soft cap? |
| Q-P024-002 | XP Formula and per-level thresholds? |
| Q-P024-003 | XP Sources (which gameplay actions)? |
| Q-P024-004 | Level Rewards document ID? |
| Q-P024-005 | Where is Level Display shown (Profile / Results / HUD)? |
| Q-P024-006 | Level Up notification timing and presentation? |
| Q-P024-007 | Prestige / milestones — future or never? |

---

## 13. Acceptance Criteria

P024 v1.0 is satisfied when all of the following are true:

1. Every player has one permanent Player Level representing long-term account progression; backend-synced.  
2. Start at Level 1; gain XP; level up when enough XP earned; Maximum Level not defined (TODO present).  
3. XP accumulates permanently; unused XP carries over after level-up; players cannot lose Player Levels.  
4. Display: Current Level, Current XP, XP Required For Next Level, XP Progress Bar.  
5. Level up shows Level Up notification; future rewards may exist; reward rules not defined.  
6. Level/XP synced with backend; consistent across supported devices.  
7. Principles: motivate continued play; remain fair; never provide gameplay advantages.  
8. Maximum Level, XP Formula, XP Sources, Level Rewards, Prestige, Level Milestones, Bonus XP, and Catch-up are not invented.  
9. Document version is **P024 v1.0**.

---

## 14. Document Queue

| Order | Document ID | Title | Status |
|-------|-------------|-------|--------|
| 1–23 | P001–P023 | (prior specs) | Approved as previously recorded |
| 24 | P024 | Level System Specification | **v1.0 Approved** |
| 25 | P025 | Rank System Specification | **v1.0 Approved** |
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

## 15. Change Log

| Version | Date | Summary | Author |
|---------|------|---------|--------|
| **1.0** | 2026-07-31 | Initial Level System Specification | Documentation Engineer (from Design Owner brief) |

---

*End of document.*
