# P005 ? Character System Specification

| Field | Value |
|-------|--------|
| Document ID | P005 |
| Title | Character System Specification |
| Version | **1.0** |
| Status | Approved (character system scope only) |
| Project | Project GulfRun |
| Authority | Official source of truth for **character selection**, **default characters**, **cosmetic-only rules**, and **customization categories** listed herein |
| Depends on | [P001](P001-GAME-VISION-v1.0.md), [P002](P002-CORE-GAMEPLAY-LOOP-v1.0.md), [P003](P003-CORE-GAMEPLAY-DESIGN-v1.0.md), [P004](P004-MAIN-MENU-v1.0.md) |
| Last updated | 2026-07-31 |

**Rules:** Documentation only. No implementation. Do not invent characters, stats, or unlock methods. Missing detail ? **TODO**.

**Next milestone:** Sprint 1 — do not start until explicitly instructed.

---

## 1. Purpose

Define the Character System: selection before a race, cosmetic-only identity, default characters, customization categories, and ownership/active rules.

---

## 2. Character System Overview

| Field | Value |
|-------|--------|
| Selection timing | Players **choose one character before entering a race** |
| Gameplay impact | Characters are **cosmetic only** |
| Statistics | All characters have **identical gameplay statistics** |
| Advantage rule | **No character has gameplay advantages** |

### TODO ? Overview (not provided)

- [ ] Exact UI screen for character select (before race) ? placement in P002 flow **TODO**
- [ ] Whether selection can change from Profile ?Selected Character? (P004) vs a dedicated pre-race step
- [ ] Full character roster beyond defaults below

---

## 3. Default Characters

### 3.1 Character 01

| Field | Value |
|-------|--------|
| ID | Character 01 |
| Gender presentation | Male |
| Outfit set | Default Outfit |
| Body / clothing | White Dishdasha |
| Headwear | White Ghutra |
| Agal | Black Agal |
| Footwear | Traditional Sandals |

### 3.2 Character 02

| Field | Value |
|-------|--------|
| ID | Character 02 |
| Gender presentation | Female |
| Outfit set | Default Outfit |
| Body / clothing | Black Abaya |
| Headwear | Black Sheila |
| Footwear | Traditional Sandals |

### TODO ? Default characters (not provided)

- [ ] Official display names (beyond Character 01 / 02)
- [ ] Avatar / portrait art references
- [ ] Default grant to all new players ? **resolved in P022** (every new player receives Male/Female default sets)

---

## 4. Customization Categories

Players can customize their appearance. Categories include:

| Category | Notes |
|----------|--------|
| **Outfits** | Cosmetic only |
| **Headwear** | Cosmetic only |
| **Footwear** | Cosmetic only |
| **Accessories** | Cosmetic only |
| **Animations** | Cosmetic only |
| **Victory Celebrations** | Cosmetic only |
| **Trails** | Cosmetic only |
| **Visual Effects** | Cosmetic only |

**Customization is cosmetic only.** Full cosmetic system SoT: **[P022](P022-COSMETICS-SYSTEM-v1.0.md)**.

### TODO ? Customization (not provided)

- [ ] Item lists per category  
- [ ] Equip slots / stack rules ? also **P022** / **P021**  
- [ ] Preview UI ? **P022** actions defined; UI **TODO**  
- [ ] Whether customization is per-character or account-wide  
- [ ] Animations (P005) vs P022 category list ? **TODO** |

---

## 5. Character Rules

| Rule ID | Rule |
|---------|------|
| CHR-001 | Every player **selects one active character**. |
| CHR-002 | Players **may own multiple characters**. |
| CHR-003 | Players **may own multiple cosmetic items**. |
| CHR-004 | **Only one character can be active during a race**. |
| CHR-005 | Characters are **cosmetic only**; **identical gameplay statistics**; **no gameplay advantages**. |

### TODO ? Rules (not provided)

- [ ] When active character is locked for a match (lobby vs countdown)
- [ ] Behavior if owned character list is empty (**not stated**)

---

## 6. Unlocking

Characters **may be unlocked through future systems**.

**Do not define how** in P005.

---

## 7. Future Dependencies

| Dependency | Note |
|------------|------|
| Unlock / acquisition systems | Future ? method not defined in P005; Shop **[P013](P013-SHOP-SYSTEM-v1.0.md)**; Cosmetics **[P022](P022-COSMETICS-SYSTEM-v1.0.md)** |
| Store | Not defined in P005 (see also P004 Shop existence) |
| Currencies | Not defined |
| Inventory | **[P021](P021-INVENTORY-SYSTEM-v1.0.md)** ? permanently owned cosmetics |
| P004 Profile ? Selected Character | Profile displays Selected Character; full profile **[P020](P020-PLAYER-PROFILE-SYSTEM-v1.0.md)**; sync with pre-race select **TODO** |
| P002 race entry | Character must be chosen before entering a race ? exact stage wiring **TODO** |
| Animation / VFX implementation | Categories named only |
| Additional characters beyond 01 & 02 | Future content |

---

## 8. Explicitly Not Defined (P005)

- Currencies  
- Store  
- Inventory ? **[P021](P021-INVENTORY-SYSTEM-v1.0.md)** (was ?not defined? in P005; now specified)  
- Rarity ? rarity **exists**; system not defined (**[P022](P022-COSMETICS-SYSTEM-v1.0.md)**)  
- Bundles  
- Character Skills  
- Character Stats *(beyond: all identical; cosmetic only)*  
- Character Progression  
- Voice Packs  

---

## 9. Open Questions

| ID | Question |
|----|----------|
| Q-P005-001 | Official names for Character 01 and Character 02? |
| Q-P005-002 | Are Character 01 and 02 owned by all players at account creation? | **Partial (P022):** every new player receives Male/Female default cosmetics |
| Q-P005-003 | Where in the P002 flow is character select (vs P004 Profile only)? |
| Q-P005-004 | Cosmetics equipped per-character or account-wide? |
| Q-P005-005 | Document ID for unlock/acquisition specification? |
| Q-P005-006 | How does Profile ?Selected Character? relate to active race character? |

---

## 10. Acceptance Criteria

P005 v1.0 is satisfied when all of the following are true:

1. Characters are cosmetic only; identical gameplay statistics; no gameplay advantages.  
2. Players choose one character before entering a race; only one active during a race; may own multiple characters and cosmetics.  
3. Character 01 and Character 02 default outfits are documented exactly as provided.  
4. Customization categories listed are exactly: Outfits, Headwear, Footwear, Accessories, Animations, Victory Celebrations, Trails, Visual Effects ? cosmetic only.  
5. Unlocking is stated as via future systems with **no method defined**.  
6. Currencies, Store, Bundles, Character Skills, Character Stats, Character Progression, and Voice Packs are not invented in P005. Inventory ? **P021**; Cosmetics/Rarity existence ? **P022**. Character audio (Footsteps, Emotes, Victory Sounds; Future Character Voices) ? **[P035](P035-AUDIO-SYSTEM-v1.0.md)**.  
7. Future dependencies, open questions, and acceptance criteria are present.  
8. Document version is **P005 v1.0**.

---

## 11. Document Queue

| Order | Document ID | Title | Status |
|-------|-------------|-------|--------|
| 1 | P001 | Game Vision Document | v1.1 Approved |
| 2 | P002 | Core Gameplay Loop | v1.0 Approved |
| 3 | P003 | Core Gameplay Design | v1.0 + P003A |
| 4 | P004 | Main Menu Specification | v1.0 Approved |
| 5 | P005 | Character System Specification | **v1.0 Approved** |
| 6 | P006 | Map System Specification | v1.0 Approved |
| 7 | P007 | Obstacle System Specification | v1.0 Approved |
| 8 | P008 | Item Box System Specification | v1.0 Approved |
| 9 | P009 | Item & Weapon System Specification | v1.0 Approved |
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

## 12. Change Log

| Version | Date | Summary | Author |
|---------|------|---------|--------|
| **1.0** | 2026-07-31 | Initial Character System Specification | Documentation Engineer (from Design Owner brief) |

---

*End of document.*
