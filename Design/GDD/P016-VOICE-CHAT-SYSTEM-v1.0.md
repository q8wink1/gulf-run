# P016 — Voice Chat System Specification

| Field | Value |
|-------|--------|
| Document ID | P016 |
| Title | Voice Chat System Specification |
| Version | **1.0** |
| Status | Approved (voice chat system scope only) |
| Project | Project GulfRun |
| Authority | Official source of truth for **Voice Chat availability**, **channels**, **player controls**, **quality priorities**, **status indicators**, **settings**, and **safety actions** stated herein |
| Depends on | [P001](P001-GAME-VISION-v1.0.md), [P004](P004-MAIN-MENU-v1.0.md), [P014](P014-FRIENDS-SYSTEM-v1.0.md), [P015](P015-CLAN-SYSTEM-v1.0.md), [P010](P010-RACE-RULES-v1.0.md) |
| Last updated | 2026-07-31 |

**Rules:** Documentation only. No implementation. Do not invent features beyond this brief. Missing detail → **TODO**.

**Next milestone:** Sprint 1 — do not start until explicitly instructed.

---

## 1. Purpose

Define the Voice Chat System for Project GulfRun: when voice is available, which channels exist, player controls and settings, status display, quality priorities, and safety actions — without defining moderation, recording, or networking implementation.

---

## 2. System Overview

| Field | Value |
|-------|--------|
| System | The game supports **Real-Time Voice Chat** |
| Availability | Voice Chat is available **only during supported multiplayer sessions** |
| Optional | Voice Chat is **optional** |
| Disable | Players may **disable Voice Chat at any time** |

### Alignment

- P001 Social Multiplayer pillar: voice communication is core to the experience — **system specified here**.  
- P015: Voice Chat handled by a separate specification — **this document**.  
- P014: Voice Calls not defined as a Friends feature — Party/session channels covered here instead.

### TODO — Overview (not provided)

- [ ] Exact list of “supported multiplayer sessions” beyond channels below  
- [ ] Behavior when disabled mid-session  

---

## 3. Voice Channels

Supported Voice Channels:

| Channel | Status |
|---------|--------|
| **Party Voice Chat** | Defined |
| **Private Room Voice Chat** | Defined |
| **Clan Voice Chat** | Defined |
| **Public Match Voice Chat** | **Future** |

### TODO — Channels (not provided)

- [ ] Whether Party and Private Room are mutually exclusive with race match voice  
- [ ] Who is in Party Voice Chat (party size **TODO** elsewhere)  
- [ ] Clan Voice Chat: all members vs online-only  

---

## 4. Player Controls

Players can:

| Control | Status |
|---------|--------|
| **Enable Voice Chat** | Defined |
| **Disable Voice Chat** | Defined |
| **Mute Self** | Defined |
| **Unmute Self** | Defined |
| **Mute Other Players** | Defined |
| **Adjust Voice Volume** | Defined |

### TODO — Controls (not provided)

- [ ] UI placement (in-race HUD vs Settings)  
- [ ] Per-player vs master volume for “Adjust Voice Volume”  

---

## 5. Voice Quality

Voice communication should prioritize:

| Priority |
|----------|
| **Low Latency** |
| **Clear Audio** |
| **Stable Connection** |
| **Low Mobile Data Usage** |

### TODO — Quality (not provided)

- [ ] Numeric targets (bitrate, latency budgets) — engineering may define later without inventing design numbers here  

---

## 6. Voice Status (Player Status Display)

Display:

| Indicator | Status |
|-----------|--------|
| **Speaking Indicator** | Defined |
| **Muted Indicator** | Defined |
| **Microphone Disabled** | Defined |
| **Connection Status** | Defined |

### TODO — Status (not provided)

- [ ] Where indicators appear (roster, HUD, clan UI)  
- [ ] Connection Status values  

---

## 7. Safety Rules

Players may:

| Action | Status |
|--------|--------|
| **Mute Any Player** | Defined |
| **Report Voice Abuse** | **Future** |
| **Block Player** | **Future** |

| Field | Value |
|-------|--------|
| Voice Moderation | **Not defined** |

### Alignment

- P014 Report Player / Block Player (Future) — may relate; Voice-specific Report Voice Abuse listed here as Future.  

---

## 8. Voice Settings

Voice Chat Settings include:

| Setting | Status |
|---------|--------|
| **Voice Chat On/Off** | Defined |
| **Microphone On/Off** | Defined |
| **Input Volume** | Defined |
| **Output Volume** | Defined |
| **Push To Talk** | **Future** |
| **Voice Activation** | **Future** |

### Alignment

- Settings screen now specified — **[P034](P034-SETTINGS-SYSTEM-v1.0.md)** Audio category lists Voice Chat Volume; exact wiring of Voice Chat Settings (On/Off, Mic, Input/Output Volume) into P034 categories is **TODO**.
- Audio System now specified — **[P035](P035-AUDIO-SYSTEM-v1.0.md)** lists Voice Chat as an audio category with this document (P016) as its detail source; relationship between P016's Adjust Voice Volume and P035/P034's Voice Chat Volume setting is **TODO**.  

---

## 9. Dependencies

| Dependency | Note |
|------------|------|
| Supported multiplayer sessions | Party / Private Room / Clan (+ Public Match future) |
| P034 Settings | Audio category lists Voice Chat Volume; full wiring TBD |
| P015 Clan | Clan Voice Chat channel |
| P014 Friends / Party | Party Voice Chat (**TODO** party definition) |
| P010 Race / match | Public Match Voice Chat future |
| Mobile / High Performance (P001) | Low data usage priority |

---

## 10. Future Specifications

| Topic | Status |
|-------|--------|
| Public Match Voice Chat | Future channel |
| Report Voice Abuse | Future |
| Block Player (voice context) | Future |
| Push To Talk | Future setting |
| Voice Activation | Future setting |
| Voice Moderation | Not defined / future |
| Full Settings app screen | **[P034](P034-SETTINGS-SYSTEM-v1.0.md)** — specified; exact Voice Chat option wiring TBD |

---

## 11. Explicitly Not Defined (P016)

- Voice Moderation  
- Voice Recording  
- Voice Translation  
- Voice Effects  
- Spatial Audio  
- Regional Servers  
- Age Restrictions  

---

## 12. Open Questions

| ID | Question |
|----|----------|
| Q-P016-001 | Exact session types that enable Voice Chat today? |
| Q-P016-002 | Party definition for Party Voice Chat? |
| Q-P016-003 | Where do Voice Settings live within P034 Settings categories (Audio vs Controls)? |
| Q-P016-004 | Connection Status display values? |
| Q-P016-005 | Document ID for Report Voice Abuse / moderation? |
| Q-P016-006 | Age restrictions for voice (store/compliance)? |

---

## 13. Acceptance Criteria

P016 v1.0 is satisfied when all of the following are true:

1. Real-Time Voice Chat; only in supported multiplayer sessions; optional; can disable anytime.  
2. Channels: Party, Private Room, Clan; Public Match future.  
3. Controls: Enable/Disable, Mute/Unmute Self, Mute Others, Adjust Voice Volume.  
4. Quality priorities: low latency, clear audio, stable connection, low mobile data usage.  
5. Status display: Speaking, Muted, Microphone Disabled, Connection Status.  
6. Safety: Mute Any Player; Report Voice Abuse and Block future; moderation not defined.  
7. Settings: Voice Chat On/Off, Mic On/Off, Input/Output Volume; PTT and Voice Activation future.  
8. Moderation, recording, translation, effects, spatial audio, regional servers, and age restrictions are not invented.  
9. Document version is **P016 v1.0**.

---

## 14. Document Queue

| Order | Document ID | Title | Status |
|-------|-------------|-------|--------|
| 1–15 | P001–P015 | (prior specs) | Approved as previously recorded |
| 16 | P016 | Voice Chat System Specification | **v1.0 Approved** |
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

## 15. Change Log

| Version | Date | Summary | Author |
|---------|------|---------|--------|
| **1.0** | 2026-07-31 | Initial Voice Chat System Specification | Documentation Engineer (from Design Owner brief) |

---

*End of document.*
