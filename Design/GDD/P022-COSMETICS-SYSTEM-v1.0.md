# P022 — Cosmetics System Specification

| Field | Value |
|-------|--------|
| Document ID | P022 |
| Title | Cosmetics System Specification |
| Version | **1.0** |
| Status | Approved (Cosmetic Customization System scope only) |
| Project | Project GulfRun |
| Authority | Official source of truth for **cosmetic personalization**, **categories**, **default cosmetics**, **equipment**, **ownership**, **player actions**, **rarity existence**, and **no gameplay impact** rules stated herein |
| Depends on | [P001](P001-GAME-VISION-v1.0.md), [P005](P005-CHARACTER-SYSTEM-v1.0.md), [P013](P013-SHOP-SYSTEM-v1.0.md), [P020](P020-PLAYER-PROFILE-SYSTEM-v1.0.md), [P021](P021-INVENTORY-SYSTEM-v1.0.md) |
| Last updated | 2026-07-31 |

**Rules:** Documentation only. No implementation. Do not invent features beyond this brief. Missing detail → **TODO**.

**Next milestone:** Sprint 1 — do not start until explicitly instructed.

---

## 1. Purpose

Define the Cosmetic Customization System for Project GulfRun: visual-only personalization, official categories, default grants for new players, equipment and ownership rules, player actions, rarity existence without a rarity system definition, and hard constraints against gameplay impact.

---

## 2. System Overview

| Field | Value |
|-------|--------|
| Purpose | Cosmetics allow players to **personalize their appearance** |
| Nature | Cosmetics are **visual only** |
| Fairness | Cosmetics **never** provide gameplay advantages |

### Alignment

- P001 Fair Competition / P005 CHR-005 / P013 no P2W / P021 INV-001 — **reinforced**.  
- P021 Inventory stores permanently owned cosmetics; this document defines the **cosmetic system** rules and defaults.

---

## 3. Cosmetic Categories

| Category | Status |
|----------|--------|
| **Characters** | Defined |
| **Outfits** | Defined |
| **Headwear** | Defined |
| **Footwear** | Defined |
| **Accessories** | Defined |
| **Victory Celebrations** | Defined |
| **Trails** | Defined |
| **Visual Effects** | Defined |
| **Profile Avatar** | Defined |
| **Profile Frame** | Defined |
| **Future Cosmetic Categories** | Future |

### Alignment

- P005 customization categories include **Animations** — **Animations as a P022 category is not stated** (do not add).  
- P021 Inventory lists **Profile Cosmetics** (bucket); P022 names **Profile Avatar** and **Profile Frame** — mapping **TODO**.  
- P020 lists Profile Banner / Profile Badge as **future** — **not** listed as P022 categories (do not invent).  
- P013 Shop categories omit Profile Avatar / Profile Frame — Shop listing **TODO**.

### TODO — Categories (not provided)

- [ ] Mapping P022 Profile Avatar / Profile Frame ↔ P021 Profile Cosmetics  
- [ ] P005 Animations vs P022 categories  
- [ ] Agal as Headwear accessory vs separate slot (default Male set includes Black Agal)  

---

## 4. Default Cosmetics

Every new player receives **default cosmetics**.

### Male Character

| Default cosmetic |
|------------------|
| Male Character |
| White Dishdasha |
| White Ghutra |
| Black Agal |
| Traditional Sandals |

### Female Character

| Default cosmetic |
|------------------|
| Female Character |
| Black Abaya |
| Black Sheila |
| Traditional Sandals |

### Alignment

- P005 Character 01 / 02 default outfits — **aligned** (Dishdasha/Ghutra/Agal/Sandals; Abaya/Sheila/Sandals).  
- P005 open question whether defaults are granted at account creation — **resolved by P022**: every new player receives these defaults.

### TODO — Defaults (not provided)

- [ ] Official display names beyond Male / Female Character (P005 Character 01 / 02 naming)  
- [ ] Default Profile Avatar / Profile Frame (not listed in default sets)  
- [ ] Default Trails / Visual Effects / Victory Celebrations / Accessories (not listed)  

---

## 5. Equipment Rules

| Rule ID | Rule |
|---------|------|
| COS-EQ-001 | Players may equip **one cosmetic per supported slot**. |
| COS-EQ-002 | Equipped cosmetics are **automatically displayed during races**. |
| COS-EQ-003 | Players may **change cosmetics at any time outside active races**. |

### Alignment

- P021 INV-EQ-001 / INV-EQ-003 — **consistent** (one per slot; auto-used in races).  
- Change timing “outside active races” — **refined** here vs P021 (P021 did not state when change is allowed).

### TODO — Equipment (not provided)

- [ ] Supported slot list  
- [ ] Definition of “active race” (lobby vs countdown vs in-race)  
- [ ] Unequip / revert-to-default behavior  

---

## 6. Ownership Rules

| Rule ID | Rule |
|---------|------|
| COS-OWN-001 | **Unlocked** cosmetics become **permanently owned**. |
| COS-OWN-002 | Ownership is **synchronized with the backend**. |
| COS-OWN-003 | **Duplicate ownership rules are not defined**. |

### Alignment

- P013 OWN-001 / P021 INV-OWN-* — **consistent**.

---

## 7. Player Actions

| Action | Status |
|--------|--------|
| **Browse Cosmetics** | Defined |
| **Preview Cosmetic** | Defined |
| **Equip Cosmetic** | Defined |
| **Unequip Cosmetic** | Defined |
| **Filter Cosmetics** | Defined |
| **Sort Cosmetics** | Defined |

### Alignment

- P021 Inventory actions (Browse / Preview / Equip / Unequip / Sort / Filter) — **aligned** for owned cosmetics.  
- Open Inventory — Inventory UX (P021); cosmetic browse may use Inventory or dedicated UI — **TODO**.

### TODO — Player actions (not provided)

- [ ] UI entry (Inventory vs dedicated Cosmetics screen)  
- [ ] Sort / Filter criteria  

---

## 8. Rarity

| Field | Value |
|-------|--------|
| Existence | Cosmetic rarity **exists** |
| System | **Rarity system is not defined** |

### TODO — Rarity (not provided)

- [ ] Rarity levels / labels  
- [ ] Effect of rarity on Shop / Inventory UI  

---

## 9. Rules (No Gameplay Impact)

Cosmetics **never** modify:

| System / property |
|-------------------|
| **Speed** |
| **Jump** |
| **Physics** |
| **Collision** |
| **Matchmaking** |
| **Ranking** |
| **Gameplay Balance** |

---

## 10. Dependencies

| Dependency | Note |
|------------|------|
| P005 Character System | Characters; default looks; cosmetic-only stats |
| P021 Inventory | Storage of owned cosmetics; equip slots |
| P013 Shop | Acquisition path for unlocked cosmetics |
| P020 Player Profile | Profile Avatar / Frame display |
| P001 Fair Competition | No gameplay advantages |
| Backend | Ownership sync |

---

## 11. Future Specifications

| Topic | Status |
|-------|--------|
| Future Cosmetic Categories | Future |
| Rarity system / Rarity Levels | Not defined (rarity exists) |
| Limited Editions | Not defined |
| Seasonal Cosmetics | Not defined |
| Exclusive Cosmetics | Not defined |
| Trading | Not defined |
| Selling | Not defined |
| Gifting | Not defined |
| Bundles | Not defined |
| Collection Rewards | Not defined |

---

## 12. Explicitly Not Defined (P022)

- Rarity Levels  
- Limited Editions  
- Seasonal Cosmetics  
- Exclusive Cosmetics  
- Trading  
- Selling  
- Gifting  
- Bundles  
- Collection Rewards  
- Supported slot list  
- Sort / Filter criteria  

---

## 13. Open Questions

| ID | Question |
|----|----------|
| Q-P022-001 | Supported cosmetic slot list? |
| Q-P022-002 | What is “active race” for change lockout? |
| Q-P022-003 | Rarity levels / system document ID? |
| Q-P022-004 | P005 Animations category vs P022 list? |
| Q-P022-005 | Profile Avatar / Frame ↔ P021 Profile Cosmetics / P020 Banner & Badge? |
| Q-P022-006 | Default Profile Avatar / Frame for new players? |
| Q-P022-007 | Cosmetics UI vs Inventory UI? |

---

## 14. Acceptance Criteria

P022 v1.0 is satisfied when all of the following are true:

1. Cosmetics personalize appearance; visual only; never provide gameplay advantages.  
2. Categories: Characters, Outfits, Headwear, Footwear, Accessories, Victory Celebrations, Trails, Visual Effects, Profile Avatar, Profile Frame; Future Cosmetic Categories future.  
3. Every new player receives listed Male and Female default cosmetics.  
4. One cosmetic per supported slot; equipped cosmetics auto-displayed in races; change allowed outside active races.  
5. Unlocked cosmetics permanently owned; ownership backend-synced; duplicates not defined.  
6. Actions: Browse, Preview, Equip, Unequip, Filter, Sort.  
7. Rarity exists; rarity system not defined.  
8. Cosmetics never modify Speed, Jump, Physics, Collision, Matchmaking, Ranking, or Gameplay Balance.  
9. Rarity levels, limited/seasonal/exclusive, trading, selling, gifting, bundles, and collection rewards are not invented.  
10. Document version is **P022 v1.0**.

---

## 15. Document Queue

| Order | Document ID | Title | Status |
|-------|-------------|-------|--------|
| 1–21 | P001–P021 | (prior specs) | Approved as previously recorded |
| 22 | P022 | Cosmetics System Specification | **v1.0 Approved** |
| 23 | P023 | Player Progression System Specification | **v1.0 Approved** |
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

## 16. Change Log

| Version | Date | Summary | Author |
|---------|------|---------|--------|
| **1.0** | 2026-07-31 | Initial Cosmetics System Specification | Documentation Engineer (from Design Owner brief) |

---

*End of document.*
