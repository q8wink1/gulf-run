# 18 — Meta, Social & Communication

**GDD chapter:** 18  
**Status:** Partial — synced to P014–P016 / P018–P020 / P032 / P033  
**Design Owner:** `[TBD]`  
**Last updated:** 2026-07-31

> Friends SoT: [P014](../P014-FRIENDS-SYSTEM-v1.0.md).  
> Notifications SoT: [P032](../P032-NOTIFICATION-SYSTEM-v1.0.md).  
> Inbox (Mail) SoT: [P033](../P033-INBOX-MAIL-SYSTEM-v1.0.md). Do not invent messaging beyond approved specs.

---

## 18.1 Meta game overview

**TODO** beyond social features listed in approved specs.

## 18.2 Social features inventory

| Feature ID | Name | Intent | Launch? | Status |
|------------|------|--------|---------|--------|
| SOC-001 | Friends System | Personal Friends List; requests; invites | Main Menu | **P014** |
| SOC-002 | Clans | One clan per player; roles; text chat | Main Menu | **P015** |
| SOC-003 | Voice Chat | Real-time; optional; Party/Private Room/Clan channels | Supported MP sessions | **P016** |
| SOC-004 | Private Room | Invite-based lobby; Room Code; host controls | Play path | **P018** |
| SOC-005 | Leaderboard | Global/Regional/Friends/Season; read-only | Meta | **P019** |
| SOC-006 | Player Profile | Unique identity; stats; public; cosmetic edit | Main Menu / social | **P020** |
| SOC-007 | Notifications | In-game + push; Friend/Clan/Season/Event/etc. types | Meta | **P032** |
| SOC-008 | Inbox (Mail) | Backend-synced system messages; optional attachments | Meta | **P033** |

## 18.3 Communication channels

| Channel | Available when | Status |
|---------|----------------|--------|
| Clan Text Chat | Clan members | **Exists** (P015) |
| Voice Chat | Supported multiplayer sessions; optional | **P016** — Party / Private Room / Clan; Public Match future |
| Messaging (general) | — | **Not defined** (P014) |
| Notifications | In-game + push | **P032** |
| Inbox (Mail) | System messages; claim attachments | **P033** |

## 18.4 Clans / guilds / crews

**[P015 — Clan System Specification](../P015-CLAN-SYSTEM-v1.0.md)**. Voice Chat separate. Clan Wars/Missions/etc. not defined.

## 18.5 Presence & privacy defaults

Online / Offline / In Match (P014). Do Not Disturb **future**. Block/Report **future**.

## 18.6 Open questions

See P014 §15.
