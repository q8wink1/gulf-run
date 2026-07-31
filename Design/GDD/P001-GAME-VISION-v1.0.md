# P001 — Project GulfRun — Game Vision Document

| Field | Value |
|-------|--------|
| Document ID | P001 |
| Title | Game Vision Document |
| Version | **1.1** |
| Status | Approved (vision scope only) |
| Project | Project GulfRun |
| Authority | Official source of truth for **project vision** |
| Last updated | 2026-07-31 |

**Documentation rules applied:** No gameplay invented beyond facts supplied for this document. Missing detail is marked **TODO**. Systems listed under Non Goals remain out of scope for full specification here.

**Next official specification (queued — not started):** P002 — Core Gameplay Loop

---

## 1. Project Overview

| Field | Value |
|-------|--------|
| **Project Name** | Project GulfRun |
| **Project Type** | Real-time Multiplayer Mobile Racing Game |
| **Platforms** | iOS, Android |
| **Graphics Style** | Stylized Low Poly Cartoon |
| **Camera** | Side Scrolling |
| **Screen Orientation** | Landscape only |
| **Target Players** | 4 Players per Match |

---

## 2. Project Vision

Project GulfRun aims to create an **original multiplayer racing game** with a **strong Gulf identity**.

The product is designed for **long-term live service**.

Gameplay must be **easy to learn** and **difficult to master**.

The experience must be **competitive**, **social**, and **fun**.

The game should feel **modern** while **representing Gulf culture respectfully**.

### TODO — Vision detail (not yet provided)

- [ ] Elevator pitch (single sentence, Design Owner)
- [ ] Expanded vision narrative beyond the facts above
- [ ] Success definition in Design Owner’s words

---

## 3. Core Pillars

Only the following pillars are in force. **No additional pillars** may be added without an explicit Design Owner revision of this document.

### 3.1 Fast Gameplay

Every match must feel fast, exciting and finish within a few minutes.

### 3.2 Fair Competition

Victory should depend primarily on player skill. Random elements should enhance variety without dominating outcomes.

### 3.3 Social Multiplayer

Playing with friends, voice communication and community interaction are core parts of the experience.

### 3.4 Gulf Identity

The game should celebrate Gulf culture respectfully through original characters, maps, music, environments and cosmetics.

### 3.5 Long-Term Progression

Players should always have meaningful goals through levels, ranks, cosmetics and seasonal content.

### 3.6 Mobile First

Everything must be designed primarily for touch controls and mobile performance.

### 3.7 High Performance

The game must run smoothly on a wide range of Android and iOS devices with optimized graphics and networking.

### TODO — Pillar expansion (optional; not yet provided)

- [ ] Explicit anti-examples (what violates each pillar)
- [ ] Numeric match-length target (only “a few minutes” is defined)

---

## 4. Target Audience

### 4.1 Primary Audience

Players aged **12–35** who enjoy **competitive multiplayer games**, **party racing games**, and **social mobile games**.

### 4.2 Secondary Audience

**Casual players**, **families**, and **Gulf culture enthusiasts**.

### 4.3 Age Groups

| Group | Definition | Source |
|-------|------------|--------|
| Primary age range | 12–35 | §4.1 |
| Other age bands | **TODO** — not separately defined | — |

### 4.4 Player Types

| Type | Audience | Source |
|------|----------|--------|
| Competitive multiplayer players | Primary | §4.1 |
| Party racing players | Primary | §4.1 |
| Social mobile players | Primary | §4.1 |
| Casual players | Secondary | §4.2 |
| Families | Secondary | §4.2 |
| Gulf culture enthusiasts | Secondary | §4.2 |

### 4.5 Regions

**Launch priority (in order):**

1. GCC countries  
2. Middle East  
3. Global expansion afterward  

---

## 5. Unique Identity

The following identity elements are established. **No additional identity elements** are implied beyond Design Owner updates.

| Element | Statement |
|---------|-----------|
| Characters | Original Gulf-inspired cartoon characters |
| Maps | Original Gulf-inspired maps |
| Customization | Original cosmetic customization |
| Competition | Competitive multiplayer races |
| Live content | Live events |
| Cadence | Seasonal updates |
| Music & environments | Affirmed under Gulf Identity pillar (§3.4) as vehicles for respectful Gulf culture |

### TODO — Identity detail (not yet provided)

- [ ] Character roster and fantasy — **partial:** Character 01 / 02 defaults in P005; further roster TODO
- [ ] Map list and fantasy — **partial:** six official maps in P006; art/audio/weather etc. TODO
- [ ] Cosmetic categories and rules
- [ ] Live event types and rules
- [x] Season structure � **[P030](P030-SEASON-SYSTEM-v1.0.md)** (duration/names/rewards still TODO)

---

## 6. Long Term Goals

Project GulfRun is planned to **grow over multiple years**.

Long-term direction includes:

- **Future content updates**
- **Seasonal content**
- **Community growth**

No additional long-term features, modes, or systems are defined in this Vision document beyond pillar intent in §3.

### TODO — Long-term goals detail (not yet provided)

- [ ] Multi-year thematic roadmap (as supplied by Design Owner)
- [ ] Community growth measures (as defined by Design Owner / Producer)

---

## 7. Non Goals

This document **intentionally does not fully specify** the following. Each will receive its own specification document later.

| Topic | Status in this document |
|-------|-------------------------|
| Weapons | **System rules:** [P009](P009-ITEM-WEAPON-SYSTEM-v1.0.md). Weapon **list** / effects still not defined. |
| Maps | Official list + rules: **P006**. Obstacles: **P007** (types/effects still TBD). |
| Characters | Default roster + cosmetic rules: **P005**. Unlock methods and full catalog still future. |
| Economy | Currencies/wallets: **[P012](P012-ECONOMY-SYSTEM-v1.0.md)**. Rewards/prices/store still not defined. |
| Battle Pass | Not defined |
| Store | Shop system: **P013**. Prices/offers still not defined. |
| Voice Chat | **System specified:** [P016](P016-VOICE-CHAT-SYSTEM-v1.0.md). Moderation / age restrictions still not defined. |
| Backend | Not defined |
| Networking | Not defined (High Performance pillar states networking must be optimized; no network design here) |
| Progression | **[P023](P023-PLAYER-PROGRESSION-SYSTEM-v1.0.md)** — Level **[P024](P024-LEVEL-SYSTEM-v1.0.md)**; Competitive Rank **[P025](P025-RANK-SYSTEM-v1.0.md)**; Season System **[P030](P030-SEASON-SYSTEM-v1.0.md)** (progress calc still **TODO**); Achievements **[P028](P028-ACHIEVEMENT-SYSTEM-v1.0.md)** (list TBD) |

Engineering architecture, networking, and backend remain governed by `docs/` and must not invent gameplay to fill these gaps.

---

## 8. Open Questions

Checklist for future design decisions. Unchecked items are **not** approved design.

- [ ] Elevator pitch (single sentence)
- [ ] Success definition (design lens)
- [ ] Numeric match duration target within “a few minutes”
- [ ] Explicit anti-examples per pillar
- [ ] 
- [ ] 

---

## 9. Document Queue

| Order | Document ID | Title | Status |
|-------|-------------|-------|--------|
| 1 | P001 | Game Vision Document | v1.1 Approved |
| 2 | P002 | Core Gameplay Loop | v1.0 Approved |
| 3 | P003 | Core Gameplay Design | v1.0 Approved (+ P003A) |
| 4 | P004 | Main Menu Specification | **v1.0 Approved** |
| 5 | P005 | Character System Specification | **v1.0 Approved** |
| 6 | P006 | Map System Specification | **v1.0 Approved** |
| 7 | P007 | Obstacle System Specification | **v1.0 Approved** |
| 8 | P008 | Item Box System Specification | **v1.0 Approved** |
| 9 | P009 | Item & Weapon System Specification | **v1.0 Approved** |
| 10 | P010 | Race Rules Specification | **v1.0 Approved** |
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

Do not begin Sprint 1 until explicitly instructed.

---

## 10. Change Log

| Version | Date | Summary | Author |
|---------|------|---------|--------|
| 1.0 | 2026-07-31 | Initial Vision Document | Documentation Engineer (from Design Owner brief) |
| **1.1** | 2026-07-31 | Audience, pillar definitions, landscape orientation; clarified Non Goals vs pillar intent; queued P002 | Documentation Engineer (from Design Owner decisions) |

---

*End of document.*
