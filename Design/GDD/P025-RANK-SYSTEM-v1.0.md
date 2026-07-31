# P025 — Rank System Specification

| Field | Value |
|-------|--------|
| Document ID | P025 |
| Title | Rank System Specification |
| Version | **1.0** |
| Status | Approved (Competitive Rank system scope only) |
| Project | Project GulfRun |
| Authority | Official source of truth for **Competitive Rank**, **rank progression existence**, **rank display fields**, **seasonal ranks**, **design principles**, and **sync** stated herein |
| Depends on | [P001](P001-GAME-VISION-v1.0.md), [P023](P023-PLAYER-PROGRESSION-SYSTEM-v1.0.md), [P024](P024-LEVEL-SYSTEM-v1.0.md), [P019](P019-LEADERBOARD-SYSTEM-v1.0.md), [P020](P020-PLAYER-PROFILE-SYSTEM-v1.0.md) |
| Last updated | 2026-07-31 |

**Rules:** Documentation only. No implementation. Do not invent features beyond this brief. Missing detail → **TODO**.

**Next milestone:** Sprint 1 — do not start until explicitly instructed.

---

## 1. Purpose

Define the Competitive Rank System for Project GulfRun: one Competitive Rank per player, separation from Player Level, progression via competitive races (including promotion/demotion existence), display fields, seasonal ladders, fairness principles, and backend synchronization — without names, formulas, MMR, or reset rules.

---

## 2. System Overview

| Field | Value |
|-------|--------|
| Count | Every player has **one Competitive Rank** |
| Meaning | Rank represents **competitive performance** |
| Sync | Rank is **synchronized with the backend** |
| Separation | **Player Level** and **Player Rank** are **separate systems** |

### Alignment

- P024 Player Level — long-term account progression; **separate** from Competitive Rank (this document).  
- P023 Player Rank component — **detailed** here as Competitive Rank.  
- P020 / P004 “Player Rank” display — Competitive Rank from this system.  
- P019 leaderboard column **Rank** (position) vs **Player Rank** (competitive) — distinct concepts; integration **not defined** (§10).

---

## 3. Rank Structure

| Rule ID | Rule |
|---------|------|
| RNK-001 | **Ranks exist**. |
| RNK-002 | **Official rank names will be defined later**. |
| RNK-003 | Players move through **multiple competitive ranks**. |
| RNK-004 | **Maximum Rank is not defined**. |

### TODO — Rank structure (not provided)

- [ ] Official rank names  
- [ ] Maximum Rank  
- [ ] Number of ranks / tier structure  

---

## 4. Rank Progression

| Rule ID | Rule |
|---------|------|
| RNK-PRG-001 | Players earn **Rank Progress** through **competitive races**. |
| RNK-PRG-002 | Players may **increase or decrease** in Rank. |
| RNK-PRG-003 | **Promotion** and **Demotion** exist. |
| RNK-PRG-004 | **Exact formulas are not defined**. |

### TODO — Rank progression (not provided)

- [ ] Which match types count as competitive races (Quick Match vs Ranked — P017 Future Ranked Match)  
- [ ] Promotion Rules  
- [ ] Demotion Rules  
- [ ] Rank Formula / MMR  

---

## 5. Rank Display

Display:

| Field | Status |
|-------|--------|
| **Current Rank** | Defined |
| **Rank Icon** | Defined |
| **Rank Progress** | Defined |
| **Next Rank** | Defined |
| **Current Season Rank** | Defined |

### TODO — Rank display (not provided)

- [ ] Rank Icon art / naming (icons not defined)  
- [ ] Screens showing Rank Display (Profile, Main Menu, Results)  
- [ ] Relationship of Current Rank vs Current Season Rank UI  

---

## 6. Seasonal Ranks

| Rule ID | Rule |
|---------|------|
| RNK-SEA-001 | Ranks are **seasonal** (Season SoT **[P030](P030-SEASON-SYSTEM-v1.0.md)**). |
| RNK-SEA-002 | Each season has an **independent competitive ladder**. |
| RNK-SEA-003 | **Season reset rules are not defined**. |

### Alignment

- P023 Season Progress — related component; formula / reset still **TODO**.  
- P019 Season Leaderboard — Leaderboard Integration **not defined** (§10).  
- P001 seasonal updates intent — **aligned** at principle level.

### TODO — Seasonal ranks (not provided)

- [ ] Season Reset rules  
- [ ] Soft reset vs hard reset  
- [ ] Interaction with permanent progression (P023)  

---

## 7. Design Principles

| Principle ID | Principle |
|--------------|-----------|
| RNK-DP-001 | Ranks should **reflect player skill**. |
| RNK-DP-002 | Ranks must encourage **fair competition**. |
| RNK-DP-003 | Ranks must **never be purchasable**. |
| RNK-DP-004 | Ranks must **never** create **Pay-to-Win** advantages. |

### Alignment

- P001 Fair Competition; P013 / P023 no P2W — **reinforced**.  
- Shop cannot sell ranks (implied by never purchasable + P013 cosmetics-only).

---

## 8. Synchronization

| Field | Value |
|-------|--------|
| Sync | Rank data is **synchronized with the backend** |
| Consistency | Rank information must remain **consistent across all supported devices** |

---

## 9. Dependencies

| Dependency | Note |
|------------|------|
| P023 Player Progression | Player Rank component; Season Progress |
| P024 Level System | Separate from Competitive Rank |
| P020 Player Profile | Displays Player Rank |
| P019 Leaderboards | Player Rank field; Season Leaderboard; integration TBD |
| P017 Matchmaking | Future Ranked Match; competitive race entry TBD |
| P001 | Fair Competition; seasonal live service |
| Backend | Authoritative rank; sync |

---

## 10. Future Specifications

| Topic | Status |
|-------|--------|
| Official rank names | Later (not defined now) |
| Rank Icons (art) | Not defined |
| Rank Formula | Not defined |
| MMR | Not defined |
| Promotion Rules | Not defined |
| Demotion Rules | Not defined |
| Placement Matches | Not defined |
| Season Reset | Not defined |
| Rank Rewards | Not defined |
| Leaderboard Integration | Not defined |
| Maximum Rank | Not defined |

---

## 11. Explicitly Not Defined (P025)

- Rank Names  
- Rank Icons  
- Rank Formula  
- MMR  
- Promotion Rules  
- Demotion Rules  
- Placement Matches  
- Season Reset  
- Rank Rewards  
- Leaderboard Integration  

---

## 12. Open Questions

| ID | Question |
|----|----------|
| Q-P025-001 | Official rank names and Maximum Rank? |
| Q-P025-002 | Which modes count as competitive races? |
| Q-P025-003 | Rank Formula / MMR / promotion & demotion rules? |
| Q-P025-004 | Season Reset rules? |
| Q-P025-005 | Leaderboard Integration with Competitive Rank? |
| Q-P025-006 | Placement Matches — yes/no and rules? |
| Q-P025-007 | Rank Rewards document ID? |
| Q-P025-008 | Current Rank vs Current Season Rank relationship? |

---

## 13. Acceptance Criteria

P025 v1.0 is satisfied when all of the following are true:

1. Every player has one Competitive Rank representing competitive performance; backend-synced; separate from Player Level.  
2. Ranks exist; official names later; multiple ranks; Maximum Rank not defined (TODO present).  
3. Rank Progress via competitive races; rank may increase or decrease; Promotion and Demotion exist; formulas not defined.  
4. Display: Current Rank, Rank Icon, Rank Progress, Next Rank, Current Season Rank.  
5. Ranks are seasonal; each season has an independent competitive ladder; season reset not defined.  
6. Principles: reflect skill; fair competition; never purchasable; never Pay-to-Win.  
7. Rank data synced; consistent across supported devices.  
8. Rank Names, Icons, Formula, MMR, Promotion/Demotion Rules, Placement Matches, Season Reset, Rank Rewards, and Leaderboard Integration are not invented.  
9. Document version is **P025 v1.0**.

---

## 14. Document Queue

| Order | Document ID | Title | Status |
|-------|-------------|-------|--------|
| 1–24 | P001–P024 | (prior specs) | Approved as previously recorded |
| 25 | P025 | Rank System Specification | **v1.0 Approved** |
| 26 | P026 | Daily Challenges System Specification | **v1.0 Approved** |
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
| **1.0** | 2026-07-31 | Initial Rank System Specification (Competitive Rank) | Documentation Engineer (from Design Owner brief) |

---

*End of document.*
