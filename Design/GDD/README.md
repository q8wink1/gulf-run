# GulfRun — Game Design Document (GDD)

**Status:** Template only — not populated  
**Authority:** This GDD is the **single source of truth** for all gameplay, UX, modes, content fantasy, progression, economy rules, monetization design, and multiplayer *design intent*.  
**Engineering docs** (`docs/`) define architecture, standards, and implementation constraints. They MUST NOT invent gameplay.  
**Filling rule:** Sections are completed only by the Design Owner (or delegates). Documentation engineers organize, flag conflicts/risks, and prepare implementation plans — they do not author mechanics.

---

## How to use this GDD

1. Treat every `[TBD]` as intentionally blank.
2. When a section is filled, set its status to `Draft` → `Review` → `Approved`.
3. Approved sections may be referenced by tech briefs and tickets.
4. Conflicting statements across chapters are escalated — do not silently prefer one side.
5. Missing information → ask the Design Owner; never invent.

## Placeholder convention

| Marker | Meaning |
|--------|---------|
| `[TBD]` | Not yet defined by Design Owner |
| `[QUESTION]` | Blocking question for Design Owner |
| `[CONFLICT]` | Clash with another approved section or with `docs/` constraints |
| `[RISK]` | Documented risk (no design invented to “solve” it) |
| `[APPROVED]` | Signed off; safe to plan implementation against |

## Chapter index

| Ch | Title | Path | Status |
|----|-------|------|--------|
| 00 | Document control | [00-front-matter/00-document-control.md](00-front-matter/00-document-control.md) | Partial |
| **P001** | **Game Vision Document** | [P001-GAME-VISION-v1.0.md](P001-GAME-VISION-v1.0.md) | **v1.1 Approved** |
| **P002** | **Core Gameplay Loop** | [P002-CORE-GAMEPLAY-LOOP-v1.0.md](P002-CORE-GAMEPLAY-LOOP-v1.0.md) | **v1.0 Approved** |
| **P003** | **Core Gameplay Design** | [P003-CORE-GAMEPLAY-DESIGN-v1.0.md](P003-CORE-GAMEPLAY-DESIGN-v1.0.md) | **v1.0 + P003A** |
| **P004** | **Main Menu Specification** | [P004-MAIN-MENU-v1.0.md](P004-MAIN-MENU-v1.0.md) | **v1.0 Approved** |
| **P005** | **Character System Specification** | [P005-CHARACTER-SYSTEM-v1.0.md](P005-CHARACTER-SYSTEM-v1.0.md) | **v1.0 Approved** |
| **P006** | **Map System Specification** | [P006-MAP-SYSTEM-v1.0.md](P006-MAP-SYSTEM-v1.0.md) | **v1.0 Approved** |
| **P007** | **Obstacle System Specification** | [P007-OBSTACLE-SYSTEM-v1.0.md](P007-OBSTACLE-SYSTEM-v1.0.md) | **v1.0 Approved** |
| **P008** | **Item Box System Specification** | [P008-ITEM-BOX-SYSTEM-v1.0.md](P008-ITEM-BOX-SYSTEM-v1.0.md) | **v1.0 Approved** |
| **P009** | **Item & Weapon System Specification** | [P009-ITEM-WEAPON-SYSTEM-v1.0.md](P009-ITEM-WEAPON-SYSTEM-v1.0.md) | **v1.0 Approved** |
| **P010** | **Race Rules Specification** | [P010-RACE-RULES-v1.0.md](P010-RACE-RULES-v1.0.md) | **v1.0 Approved** |
| **P011** | **Post Race Results Specification** | [P011-POST-RACE-RESULTS-v1.0.md](P011-POST-RACE-RESULTS-v1.0.md) | **v1.0 Approved** |
| **P012** | **Economy System Specification** | [P012-ECONOMY-SYSTEM-v1.0.md](P012-ECONOMY-SYSTEM-v1.0.md) | **v1.0 Approved** |
| **P013** | **Shop System Specification** | [P013-SHOP-SYSTEM-v1.0.md](P013-SHOP-SYSTEM-v1.0.md) | **v1.0 Approved** |
| **P014** | **Friends System Specification** | [P014-FRIENDS-SYSTEM-v1.0.md](P014-FRIENDS-SYSTEM-v1.0.md) | **v1.0 Approved** |
| **P015** | **Clan System Specification** | [P015-CLAN-SYSTEM-v1.0.md](P015-CLAN-SYSTEM-v1.0.md) | **v1.0 Approved** |
| **P016** | **Voice Chat System Specification** | [P016-VOICE-CHAT-SYSTEM-v1.0.md](P016-VOICE-CHAT-SYSTEM-v1.0.md) | **v1.0 Approved** |
| **P017** | **Matchmaking System Specification** | [P017-MATCHMAKING-SYSTEM-v1.0.md](P017-MATCHMAKING-SYSTEM-v1.0.md) | **v1.0 Approved** |
| **P018** | **Private Room System Specification** | [P018-PRIVATE-ROOM-SYSTEM-v1.0.md](P018-PRIVATE-ROOM-SYSTEM-v1.0.md) | **v1.0 Approved** |
| **P019** | **Leaderboard System Specification** | [P019-LEADERBOARD-SYSTEM-v1.0.md](P019-LEADERBOARD-SYSTEM-v1.0.md) | **v1.0 Approved** |
| **P020** | **Player Profile System Specification** | [P020-PLAYER-PROFILE-SYSTEM-v1.0.md](P020-PLAYER-PROFILE-SYSTEM-v1.0.md) | **v1.0 Approved** |
| **P021** | **Inventory System Specification** | [P021-INVENTORY-SYSTEM-v1.0.md](P021-INVENTORY-SYSTEM-v1.0.md) | **v1.0 Approved** |
| **P022** | **Cosmetics System Specification** | [P022-COSMETICS-SYSTEM-v1.0.md](P022-COSMETICS-SYSTEM-v1.0.md) | **v1.0 Approved** |
| **P023** | **Player Progression System Specification** | [P023-PLAYER-PROGRESSION-SYSTEM-v1.0.md](P023-PLAYER-PROGRESSION-SYSTEM-v1.0.md) | **v1.0 Approved** |
| **P024** | **Level System Specification** | [P024-LEVEL-SYSTEM-v1.0.md](P024-LEVEL-SYSTEM-v1.0.md) | **v1.0 Approved** |
| **P025** | **Rank System Specification** | [P025-RANK-SYSTEM-v1.0.md](P025-RANK-SYSTEM-v1.0.md) | **v1.0 Approved** |
| **P026** | **Daily Challenges System Specification** | [P026-DAILY-CHALLENGES-SYSTEM-v1.0.md](P026-DAILY-CHALLENGES-SYSTEM-v1.0.md) | **v1.0 Approved** |
| **P027** | **Weekly Challenges System Specification** | [P027-WEEKLY-CHALLENGES-SYSTEM-v1.0.md](P027-WEEKLY-CHALLENGES-SYSTEM-v1.0.md) | **v1.0 Approved** |
| **P028** | **Achievement System Specification** | [P028-ACHIEVEMENT-SYSTEM-v1.0.md](P028-ACHIEVEMENT-SYSTEM-v1.0.md) | **v1.0 Approved** |
| **P029** | **Battle Pass System Specification** | [P029-BATTLE-PASS-SYSTEM-v1.0.md](P029-BATTLE-PASS-SYSTEM-v1.0.md) | **v1.0 Approved** |
| **P030** | **Season System Specification** | [P030-SEASON-SYSTEM-v1.0.md](P030-SEASON-SYSTEM-v1.0.md) | **v1.0 Approved** |
| **P031** | **Live Events System Specification** | [P031-LIVE-EVENTS-SYSTEM-v1.0.md](P031-LIVE-EVENTS-SYSTEM-v1.0.md) | **v1.0 Approved** |
| **P032** | **Notification System Specification** | [P032-NOTIFICATION-SYSTEM-v1.0.md](P032-NOTIFICATION-SYSTEM-v1.0.md) | **v1.0 Approved** |
| **P033** | **Inbox (Mail) System Specification** | [P033-INBOX-MAIL-SYSTEM-v1.0.md](P033-INBOX-MAIL-SYSTEM-v1.0.md) | **v1.0 Approved** |
| **P034** | **Settings System Specification** | [P034-SETTINGS-SYSTEM-v1.0.md](P034-SETTINGS-SYSTEM-v1.0.md) | **v1.0 Approved** |
| **P035** | **Audio System Specification** | [P035-AUDIO-SYSTEM-v1.0.md](P035-AUDIO-SYSTEM-v1.0.md) | **v1.0 Approved** |
| **P036** | **Music System Specification** | [P036-MUSIC-SYSTEM-v1.0.md](P036-MUSIC-SYSTEM-v1.0.md) | **v1.0 Approved** |
| **P037** | **Localization System Specification** | [P037-LOCALIZATION-SYSTEM-v1.0.md](P037-LOCALIZATION-SYSTEM-v1.0.md) | **v1.0 Approved** |
| **P038** | **Tutorial System Specification** | [P038-TUTORIAL-SYSTEM-v1.0.md](P038-TUTORIAL-SYSTEM-v1.0.md) | **v1.0 Approved** |
| **P039** | **Backend Architecture Specification** | Engineering doc — [docs/02-architecture/BACKEND_ARCHITECTURE.md](../../docs/02-architecture/BACKEND_ARCHITECTURE.md) (not gameplay; not in this GDD tree) | **v1.0 Approved** |
| **P040** | **Database Architecture Specification** | Engineering doc — [docs/02-architecture/DATABASE_ARCHITECTURE.md](../../docs/02-architecture/DATABASE_ARCHITECTURE.md) (not gameplay; not in this GDD tree) | **v1.0 Approved** |
| **P041** | **Authentication System Specification** | Engineering doc — [docs/02-architecture/AUTHENTICATION_SYSTEM.md](../../docs/02-architecture/AUTHENTICATION_SYSTEM.md) (not gameplay; not in this GDD tree) | **v1.0 Approved** |
| **P042** | **Player Profile System Specification** ⚠ | [P042-PLAYER-PROFILE-SYSTEM-v1.0.md](P042-PLAYER-PROFILE-SYSTEM-v1.0.md) | **v1.0 Approved-per-brief — [CONFLICT] with P020, unresolved** |
| **P043** | **Anti-Cheat System Specification** | Engineering doc — [docs/05-security/ANTI_CHEAT_SPECIFICATION.md](../../docs/05-security/ANTI_CHEAT_SPECIFICATION.md) (not gameplay; not in this GDD tree) | **v1.0 Approved** |
| **P044** | **Analytics System Specification** | Engineering doc — [docs/02-architecture/ANALYTICS_SYSTEM.md](../../docs/02-architecture/ANALYTICS_SYSTEM.md) (not gameplay; not in this GDD tree) | **v1.0 Approved** |
| **P045** | **Monetization System Specification** | [P045-MONETIZATION-SYSTEM-v1.0.md](P045-MONETIZATION-SYSTEM-v1.0.md) | **v1.0 Approved** |
| **P046** | **Performance Optimization Specification** | Engineering doc — [docs/04-engineering/PERFORMANCE_OPTIMIZATION_SPECIFICATION.md](../../docs/04-engineering/PERFORMANCE_OPTIMIZATION_SPECIFICATION.md) (not gameplay; not in this GDD tree) | **v1.0 Approved** |
| **P047** | **UI / UX Design System Specification** | [P047-UI-UX-DESIGN-SYSTEM-v1.0.md](P047-UI-UX-DESIGN-SYSTEM-v1.0.md) | **v1.0 Approved** |
| **P048** | **Art Direction & Visual Style Specification** | [P048-ART-DIRECTION-VISUAL-STYLE-v1.0.md](P048-ART-DIRECTION-VISUAL-STYLE-v1.0.md) | **v1.0 Approved** |
| **P049** | **Technical Architecture Specification** | Engineering doc — [docs/02-architecture/TECHNICAL_ARCHITECTURE.md](../../docs/02-architecture/TECHNICAL_ARCHITECTURE.md) (not gameplay; not in this GDD tree) | **v1.0 Approved** |
| **P050** | **Master Design Bible Specification** | [P050-MASTER-DESIGN-BIBLE-v1.0.md](P050-MASTER-DESIGN-BIBLE-v1.0.md) — consolidates P001–P049 | **v1.0 Approved** |
| **Sprint 1** | _(await instructions)_ | — | Not started |
| 01 | Vision & pillars | [00-front-matter/01-vision-and-pillars.md](00-front-matter/01-vision-and-pillars.md) | Synced to P001 v1.1 |
| 02 | Audience & market | [00-front-matter/02-audience-and-market.md](00-front-matter/02-audience-and-market.md) | Synced to P001 v1.1 |
| 03 | Platforms & product constraints | [00-front-matter/03-platforms-and-constraints.md](00-front-matter/03-platforms-and-constraints.md) | Synced to P001 v1.1 |
| 04 | Core experience | [01-core-experience/04-core-experience.md](01-core-experience/04-core-experience.md) | Synced to P002 v1.0 |
| 05 | Camera, controls & feel | [01-core-experience/05-camera-controls-and-feel.md](01-core-experience/05-camera-controls-and-feel.md) | Synced to P003 v1.0 |
| 06 | Gameplay systems (catalog) | [02-gameplay-systems/06-systems-catalog.md](02-gameplay-systems/06-systems-catalog.md) | Partial — item boxes + obstacles existence |
| 07 | Entities & content types | [02-gameplay-systems/07-entities-and-content-types.md](02-gameplay-systems/07-entities-and-content-types.md) | Synced to P005 v1.0 |
| 08 | Interaction rules | [02-gameplay-systems/08-interaction-rules.md](02-gameplay-systems/08-interaction-rules.md) | Synced to P003 v1.0 |
| 09 | Game modes | [03-game-modes/09-game-modes.md](03-game-modes/09-game-modes.md) | Partial — 4p race facts |
| 10 | Session flow & match rules | [03-game-modes/10-session-flow-and-match-rules.md](03-game-modes/10-session-flow-and-match-rules.md) | Synced to P010 v1.0 |
| 11 | Progression | [04-progression/11-progression.md](04-progression/11-progression.md) | Synced to P023 / P024 / P025 / P028 / P030 |
| 12 | Unlocks, collection & cosmetics | [04-progression/12-unlocks-collection-cosmetics.md](04-progression/12-unlocks-collection-cosmetics.md) | Partial — P013 / P021 / P022 |
| 13 | Economy | [05-economy/13-economy.md](05-economy/13-economy.md) | Synced to P012 v1.0 |
| 14 | Rewards & sinks | [05-economy/14-rewards-and-sinks.md](05-economy/14-rewards-and-sinks.md) | Template |
| 15 | Multiplayer design | [06-multiplayer/15-multiplayer-design.md](06-multiplayer/15-multiplayer-design.md) | Template |
| 16 | Matchmaking & parties | [06-multiplayer/16-matchmaking-and-parties.md](06-multiplayer/16-matchmaking-and-parties.md) | Synced to P017 / P018 v1.0 |
| 17 | Competitive integrity (design) | [06-multiplayer/17-competitive-integrity-design.md](06-multiplayer/17-competitive-integrity-design.md) | Template |
| 18 | Meta, social & communication | [07-meta-social/18-meta-social-and-communication.md](07-meta-social/18-meta-social-and-communication.md) | Synced to P014–P016 / P018–P020 / P032 / P033 |
| 19 | UI / UX screens & flows | [08-ui-ux/19-ui-ux-screens-and-flows.md](08-ui-ux/19-ui-ux-screens-and-flows.md) | Synced to P004 / P020 / P026 / P027 |
| 20 | HUD & in-session UI | [08-ui-ux/20-hud-and-in-session-ui.md](08-ui-ux/20-hud-and-in-session-ui.md) | Template |
| 21 | World / content structure | [09-content/21-world-and-content-structure.md](09-content/21-world-and-content-structure.md) | Synced to P006 v1.0 |
| 22 | Content pipeline (design) | [09-content/22-content-pipeline-design.md](09-content/22-content-pipeline-design.md) | Template |
| 23 | Narrative, audio & presentation | [10-presentation/23-narrative-audio-presentation.md](10-presentation/23-narrative-audio-presentation.md) | Template |
| 24 | Live Operations (design) | [11-liveops/24-liveops-design.md](11-liveops/24-liveops-design.md) | Template |
| 25 | Seasons, events & calendars | [11-liveops/25-seasons-events-calendars.md](11-liveops/25-seasons-events-calendars.md) | Synced to P030 / P031 (+ P019 / P025 / P029) |
| 26 | Monetization | [12-monetization/26-monetization.md](12-monetization/26-monetization.md) | Template |
| 27 | Shop, offers & pricing (design) | [12-monetization/27-shop-offers-pricing.md](12-monetization/27-shop-offers-pricing.md) | Synced to P013 v1.0 |
| 28 | Onboarding, FTUE & retention | [13-player-journey/28-onboarding-ftue-retention.md](13-player-journey/28-onboarding-ftue-retention.md) | Tutorial synced to P038; retention Template |
| 29 | Accessibility & localization | [14-accessibility-loc/29-accessibility-and-localization.md](14-accessibility-loc/29-accessibility-and-localization.md) | Localization synced to P037; Accessibility Template |
| 30 | Analytics & success metrics (design) | [15-analytics/30-analytics-and-success-metrics.md](15-analytics/30-analytics-and-success-metrics.md) | Template |
| 31 | Out of scope & non-goals | [16-appendix/31-out-of-scope-and-non-goals.md](16-appendix/31-out-of-scope-and-non-goals.md) | Synced to P001 |
| 32 | Open questions log | [16-appendix/32-open-questions-log.md](16-appendix/32-open-questions-log.md) | Living |
| 33 | Change log | [16-appendix/33-change-log.md](16-appendix/33-change-log.md) | Living |
| 34 | Glossary | [16-appendix/34-glossary.md](16-appendix/34-glossary.md) | Template |

## Related engineering docs (non-gameplay)

- Architecture & authority constraints: `docs/02-architecture/`
- Security / anti-cheat *engineering*: `docs/05-security/`
- LiveOps *operations*: `docs/06-operations/LIVE_OPERATIONS.md`

If an approved GDD rule conflicts with engineering constraints, record `[CONFLICT]` and escalate — do not invent a gameplay workaround in engineering docs.
