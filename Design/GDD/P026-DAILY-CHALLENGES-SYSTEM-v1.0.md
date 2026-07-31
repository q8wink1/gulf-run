# P026 — Daily Challenges System Specification

| Field | Value |
|-------|--------|
| Document ID | P026 |
| Title | Daily Challenges System Specification |
| Version | **1.0** |
| Status | Approved (Daily Challenges system scope only) |
| Project | Project GulfRun |
| Authority | Official source of truth for **Daily Challenges** existence, **24-hour reset**, **player actions**, **progress fields**, **reward existence**, and **completion rules** stated herein |
| Depends on | [P001](P001-GAME-VISION-v1.0.md), [P004](P004-MAIN-MENU-v1.0.md), [P012](P012-ECONOMY-SYSTEM-v1.0.md), [P023](P023-PLAYER-PROGRESSION-SYSTEM-v1.0.md) |
| Last updated | 2026-07-31 |

**Rules:** Documentation only. No implementation. Do not invent features beyond this brief. Missing detail → **TODO**.

**Next milestone:** Sprint 1 — do not start until explicitly instructed.

---

## 1. Purpose

Define the Daily Challenges System for Project GulfRun: daily engagement challenges, reset cadence, player actions, progress tracking fields, reward flow existence, and reset/completion rules — without challenge objectives, reward types, or challenge lists.

---

## 2. System Overview

| Field | Value |
|-------|--------|
| Support | The game supports **Daily Challenges** |
| Intent | Daily Challenges encourage players to **play every day** |
| Reset cadence | Daily Challenges **reset every 24 hours** |

### Alignment

- P004 Main Menu **Challenges** button — Challenges system exists; Daily Challenges detail **this document**; Weekly Challenges **[P027](P027-WEEKLY-CHALLENGES-SYSTEM-v1.0.md)**. Whether Challenges opens Daily only, Weekly, or both — **TODO**.  
- P027 Weekly Challenges — longer-term counterpart (7-day reset).  
- P001 Long-Term Progression / live service — daily engagement intent **aligned** at principle level.  
- P011 Challenge Progress / Daily Missions — post-race amounts still **not defined**; system existence here.

---

## 3. Challenge Types

| Field | Value |
|-------|--------|
| Challenge types | **Exist** |
| Specific challenge objectives | **Not defined** |

### TODO — Challenge types (not provided)

- [ ] Specific challenge objectives  
- [ ] Challenge List  
- [ ] Challenge Categories  

---

## 4. Challenge Flow

```
Player opens Daily Challenges
↓
View Daily Challenges
↓
Play / participate in gameplay
↓
Track Progress
↓
Challenge reaches Completion Status
↓
Claim Rewards (if available)
↓
(After 24 hours) Daily Challenges reset
```

```mermaid
flowchart TD
    A[View Daily Challenges] --> B[Track Progress]
    B --> C{Completion Status}
    C -->|Complete| D[Claim Rewards]
    C -->|Incomplete| B
    D --> E[Reward Status updated]
    F[24h backend reset] --> A
```

### Player Actions

| Action | Status |
|--------|--------|
| **View Daily Challenges** | Defined |
| **Track Progress** | Defined |
| **Claim Rewards** | Defined |

### TODO — Challenge flow (not provided)

- [ ] Entry point UI (Main Menu Challenges vs dedicated Daily Challenges)  
- [ ] How Track Progress is shown (in-challenge vs Results)  

---

## 5. Progress Tracking

Each challenge has:

| Field | Status |
|-------|--------|
| **Progress** | Defined |
| **Completion Status** | Defined |
| **Reward Status** | Defined |

### TODO — Progress (not provided)

- [ ] Progress units / thresholds per challenge (objectives not defined)  
- [ ] Completion Status and Reward Status value sets  

---

## 6. Reward Flow

| Field | Value |
|-------|--------|
| Rewards | Daily Challenges **provide rewards** |
| Reward types | **Not defined** |
| Claim | Players may **Claim Rewards** |

```
Challenge completed
↓
Reward available (Reward Status)
↓
Player Claims Rewards
↓
Reward granted (types not defined)
```

### Rules related to rewards

| Rule ID | Rule |
|---------|------|
| DC-RWD-001 | Daily Challenges provide rewards. |
| DC-RWD-002 | Reward types are **not defined**. |
| DC-RWD-003 | **Unclaimed reward behavior is not defined**. |

### TODO — Rewards (not provided)

- [ ] Reward Types  
- [ ] Unclaimed reward behavior at reset  
- [ ] Relationship to P012 Coins/Gems / P023 XP  

---

## 7. Reset Rules

| Rule ID | Rule |
|---------|------|
| DC-RST-001 | Daily Challenges **automatically reset every day**. |
| DC-RST-002 | Reset timing is **synchronized with the backend**. |
| DC-RST-003 | Daily Challenges reset **every 24 hours**. |
| DC-RST-004 | **Completed challenges cannot be completed again** until the next reset. |
| DC-RST-005 | **Unclaimed reward behavior is not defined**. |

### TODO — Reset (not provided)

- [ ] Exact daily reset clock (UTC vs local)  
- [ ] Refresh Rules beyond automatic daily reset  
- [ ] Behavior of in-progress challenges at reset  

---

## 8. Dependencies

| Dependency | Note |
|------------|------|
| P004 Main Menu | Challenges entry existence |
| P002 / gameplay | Progress via participation (objectives TBD) |
| P012 Economy | Possible reward currencies — types TBD |
| P023 Progression | Possible XP / other progress rewards — TBD |
| P011 Results | Challenge Progress mention — amounts TBD |
| Backend | Reset timing sync; progress / reward status |

---

## 9. Future Specifications

| Topic | Status |
|-------|--------|
| Challenge List / specific objectives | Not defined |
| Reward Types | Not defined |
| Difficulty | Not defined |
| Premium Challenges | Not defined |
| Bonus Challenges | Not defined |
| Refresh Rules | Not defined |
| Skip Challenge | Not defined |
| Challenge Categories | Not defined |
| Unclaimed reward behavior | Not defined |

---

## 10. Explicitly Not Defined (P026)

- Challenge List  
- Reward Types  
- Difficulty  
- Premium Challenges  
- Bonus Challenges  
- Refresh Rules  
- Skip Challenge  
- Challenge Categories  
- Specific challenge objectives  
- Unclaimed reward behavior  

---

## 11. Open Questions

| ID | Question |
|----|----------|
| Q-P026-001 | Specific challenge objectives / Challenge List? |
| Q-P026-002 | Reward Types (Coins, Gems, XP, cosmetics)? |
| Q-P026-003 | Unclaimed rewards at daily reset? |
| Q-P026-004 | Exact reset time zone / clock? |
| Q-P026-005 | Does Main Menu Challenges open Daily Challenges only? | **Partial:** Weekly also exists (**P027**); hub layout TODO |
| Q-P026-006 | Premium / Bonus / Skip — future or never? |
| Q-P026-007 | How many Daily Challenges per day? |

---

## 12. Acceptance Criteria

P026 v1.0 is satisfied when all of the following are true:

1. Daily Challenges supported; encourage daily play; reset every 24 hours.  
2. Challenge types exist; specific objectives not defined (TODO present).  
3. Actions: View Daily Challenges, Track Progress, Claim Rewards.  
4. Each challenge has Progress, Completion Status, Reward Status.  
5. Challenges provide rewards; reward types not defined.  
6. Automatic daily reset; backend-synced timing; completed challenges not repeatable until next reset; unclaimed behavior not defined.  
7. Challenge List, Reward Types, Difficulty, Premium/Bonus, Refresh Rules, Skip Challenge, and Challenge Categories are not invented.  
8. Document version is **P026 v1.0**.

---

## 13. Document Queue

| Order | Document ID | Title | Status |
|-------|-------------|-------|--------|
| 1–25 | P001–P025 | (prior specs) | Approved as previously recorded |
| 26 | P026 | Daily Challenges System Specification | **v1.0 Approved** |
| 27 | P027 | Weekly Challenges System Specification | **v1.0 Approved** |
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

## 14. Change Log

| Version | Date | Summary | Author |
|---------|------|---------|--------|
| **1.0** | 2026-07-31 | Initial Daily Challenges System Specification | Documentation Engineer (from Design Owner brief) |

---

*End of document.*
