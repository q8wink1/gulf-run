# P020 ? Player Profile System Specification

| Field | Value |
|-------|--------|
| Document ID | P020 |
| Title | Player Profile System Specification |
| Version | **1.0** |
| Status | Approved (Player Profile system scope only) — **[CONFLICT]** see §0: a later brief, [P042](P042-PLAYER-PROFILE-SYSTEM-v1.0.md), also claims this system title with differing content; unresolved, escalated to Design Owner |
| Project | Project GulfRun |
| Authority | Official source of truth for **Player Profile** identity, displayed information, statistics, customization, player actions, privacy default, and profile rules stated herein |
| Depends on | [P001](P001-GAME-VISION-v1.0.md), [P004](P004-MAIN-MENU-v1.0.md), [P005](P005-CHARACTER-SYSTEM-v1.0.md), [P014](P014-FRIENDS-SYSTEM-v1.0.md), [P015](P015-CLAN-SYSTEM-v1.0.md), [P019](P019-LEADERBOARD-SYSTEM-v1.0.md) |
| Last updated | 2026-07-31 |

**Rules:** Documentation only. No implementation. Do not invent features beyond this brief. Missing detail ? **TODO**.

**Next milestone:** Sprint 1 — do not start until explicitly instructed.

---

## 0. ⚠ Conflict Notice — P020 vs. P042

[P042](P042-PLAYER-PROFILE-SYSTEM-v1.0.md) — issued later, also titled "Player Profile System Specification" — describes overlapping but differing content for this same system (e.g., Profile Frame/Background listed as currently customizable there vs. Future here; Display Name editability asserted there vs. unresolved here under this document's cosmetic-only rule PP-003). See [P042](P042-PLAYER-PROFILE-SYSTEM-v1.0.md) §0 for the full comparison table. **This document (P020) remains Approved and unchanged** pending a Design Owner decision on which spec is authoritative; downstream references (P004, P014, P019) continue pointing here.

## 1. Purpose

Define the Player Profile System for Project GulfRun: unique account-linked identity, displayed profile fields, player statistics, cosmetic customization, view/edit actions, public visibility, and backend-authoritative statistics rules.

---

## 2. System Overview

| Field | Value |
|-------|--------|
| Ownership | Every player owns a **unique Player Profile** |
| Role | The Player Profile represents the player's **identity** inside the game |
| Account link | The profile is linked **permanently** to the player's account |

### Alignment

- P004 Main Menu **Profile** button / �6 fields ? hub entry; full profile system **this document**.  
- P019 **View Player Profile** ? destination is this system.  
- P014 **View Profile** ? same system (**TODO** exact screen identity if Main Menu subset vs full profile).

---

## 3. Profile Information

Display:

| Field | Status |
|-------|--------|
| **Player ID** | Defined |
| **Player Name** | Defined |
| **Player Avatar** | Defined |
| **Selected Character** | Defined |
| **Player Level** | Defined ? values from **[P023](P023-PLAYER-PROGRESSION-SYSTEM-v1.0.md)** / Level detail **[P024](P024-LEVEL-SYSTEM-v1.0.md)** |
| **Player Rank** | Defined ? values from **[P023](P023-PLAYER-PROGRESSION-SYSTEM-v1.0.md)** / Competitive Rank **[P025](P025-RANK-SYSTEM-v1.0.md)** |
| **Clan (If Joined)** | Defined |
| **Country** | **Future** |
| **Online Status** | Defined |

### Alignment

- P004 �6: Player Name, Level, Rank, Avatar, Selected Character ? **subset** of this list; P020 adds Player ID, Clan, Online Status; Country future.  
- P005: Selected Character cosmetic.  
- P015: Clan if joined.  
- P014: Online Status values Online / Offline / In Match (Last Seen future) ? status values **not redefined** here.

### TODO ? Profile information (not provided)

- [ ] Player ID format / visibility rules beyond Copy Player ID  
- [ ] Player Name edit rules / uniqueness  
- [ ] Exact Online Status enum on profile vs Friends List  
- [ ] Country field rules when future  

---

## 4. Player Statistics

Display:

| Statistic | Status |
|-----------|--------|
| **Total Matches** | Defined |
| **Wins** | Defined |
| **Losses** | Defined |
| **Win Rate** | Defined |
| **First Place Finishes** | Defined |
| **Favorite Character** | **Future** |
| **Favorite Map** | **Future** |

### TODO ? Statistics (not provided)

- [ ] Definitions of Win vs Loss vs First Place Finishes for a 4-player race  
- [ ] Win Rate calculation formula  
- [ ] Whether statistics appear on Main Menu Profile hub or full profile only  

---

## 5. Customization

Players may customize:

| Element | Status |
|---------|--------|
| **Avatar** | Defined |
| **Profile Banner** | **Future** |
| **Profile Frame** | **Future** |
| **Profile Badge** | **Future** |

### TODO ? Customization (not provided)

- [ ] Avatar source (upload vs catalog)  
- [ ] Which Shop categories unlock Avatar cosmetics (P013)  
- [ ] Profile Cosmetics storage / equip via **[P021](P021-INVENTORY-SYSTEM-v1.0.md)**  
- [ ] Profile Avatar / Profile Frame categories ? **[P022](P022-COSMETICS-SYSTEM-v1.0.md)**  

---

## 6. Player Actions

| Action | Status |
|--------|--------|
| **View Own Profile** | Defined |
| **View Another Player Profile** | Defined |
| **Edit Profile** | Defined |
| **Copy Player ID** | Defined |
| **Share Profile** | **Future** |

### TODO ? Player actions (not provided)

- [ ] Which fields Edit Profile may change (only cosmetics per �8; name edit **TODO**)  
- [ ] Entry points (Main Menu Profile, Friends, Leaderboard, Results)  

---

## 7. Profile Privacy

| Rule ID | Rule |
|---------|------|
| PP-PRIV-001 | Every player profile is **publicly viewable**. |
| PP-PRIV-002 | **Future privacy settings are not defined**. |

---

## 8. Profile Rules

| Rule ID | Rule |
|---------|------|
| PP-001 | Player statistics are **generated by the backend**. |
| PP-002 | Players **cannot manually edit** statistics. |
| PP-003 | **Only cosmetic profile elements** may be customized. |

---

## 9. Dependencies

| Dependency | Note |
|------------|------|
| P004 | Main Menu Profile entry; �6 display fields |
| P005 | Selected Character |
| P014 | Online Status; View Profile from Friends |
| P015 | Clan (If Joined) |
| P019 | View Player Profile from leaderboards |
| Backend | Statistics generation; account permanent link |
| Account / Login | Profile permanently linked; backend auth flow ? **[docs/02-architecture/AUTHENTICATION_SYSTEM.md](../../docs/02-architecture/AUTHENTICATION_SYSTEM.md)** (P041); login screen UI/UX **TODO** elsewhere |

---

## 10. Future Specifications

| Topic | Status |
|-------|--------|
| Country | Future profile field |
| Favorite Character | Future statistic |
| Favorite Map | Future statistic |
| Profile Banner | Future customization |
| Profile Frame | Future customization |
| Profile Badge | Future customization |
| Share Profile | Future action |
| Privacy Settings | Not defined |
| Profile Biography | Not defined |
| Social Links | Not defined |
| Followers / Following | Not defined |
| Likes / Comments | Not defined |
| Achievements Display | **Partial:** Achievement system **[P028](P028-ACHIEVEMENT-SYSTEM-v1.0.md)**; Profile placement **TODO** |
| Profile Themes | Not defined |
| Verification Badges | Not defined |

---

## 11. Explicitly Not Defined (P020)

- Profile Biography  
- Social Links  
- Followers  
- Following  
- Likes  
- Comments  
- Achievements Display � system **[P028](P028-ACHIEVEMENT-SYSTEM-v1.0.md)**; on-Profile display **TODO**
- Profile Themes  
- Privacy Settings  
- Verification Badges  
- Win / Loss / Win Rate formulas  
- Avatar catalog rules  

---

## 12. Open Questions

| ID | Question |
|----|----------|
| Q-P020-001 | Is P004 �6 Main Menu Profile the same screen as full Player Profile? |
| Q-P020-002 | May Edit Profile change Player Name, or cosmetics only? |
| Q-P020-003 | Win / Loss / Win Rate definitions for 4-player races? |
| Q-P020-004 | Player ID format? |
| Q-P020-005 | Avatar customization source (Shop / upload)? |
| Q-P020-006 | Document ID for future privacy settings? |
| Q-P020-007 | Online Status values identical to P014? |

---

## 13. Acceptance Criteria

P020 v1.0 is satisfied when all of the following are true:

1. Every player has a unique Player Profile representing in-game identity, permanently linked to the account.  
2. Display information: Player ID, Name, Avatar, Selected Character, Level, Rank, Clan (If Joined), Online Status; Country future.  
3. Statistics: Total Matches, Wins, Losses, Win Rate, First Place Finishes; Favorite Character / Map future.  
4. Customization: Avatar; Banner / Frame / Badge future.  
5. Actions: View Own Profile, View Another Player Profile, Edit Profile, Copy Player ID; Share Profile future.  
6. Profiles publicly viewable; future privacy settings not defined.  
7. Statistics backend-generated and not manually editable; only cosmetic elements customizable.  
8. Biography, social links, followers, likes, comments, achievements display, themes, privacy settings, and verification badges are not invented.  
9. Document version is **P020 v1.0**.

---

## 14. Document Queue

| Order | Document ID | Title | Status |
|-------|-------------|-------|--------|
| 1?19 | P001?P019 | (prior specs) | Approved as previously recorded |
| 20 | P020 | Player Profile System Specification | **v1.0 Approved** |
| 21 | P021 | Inventory System Specification | **v1.0 Approved** |
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
| **1.0** | 2026-07-31 | Initial Player Profile System Specification | Documentation Engineer (from Design Owner brief) |

---

*End of document.*
