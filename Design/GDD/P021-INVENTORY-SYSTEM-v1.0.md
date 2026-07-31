# P021 — Inventory System Specification

| Field | Value |
|-------|--------|
| Document ID | P021 |
| Title | Inventory System Specification |
| Version | **1.0** |
| Status | Approved (Inventory system scope only) |
| Project | Project GulfRun |
| Authority | Official source of truth for **personal Inventory**, **cosmetic categories stored**, **ownership**, **player actions**, **equipping**, **backend sync**, and **no gameplay advantage** rules stated herein |
| Depends on | [P001](P001-GAME-VISION-v1.0.md), [P005](P005-CHARACTER-SYSTEM-v1.0.md), [P012](P012-ECONOMY-SYSTEM-v1.0.md), [P013](P013-SHOP-SYSTEM-v1.0.md), [P020](P020-PLAYER-PROFILE-SYSTEM-v1.0.md) |
| Last updated | 2026-07-31 |

**Rules:** Documentation only. No implementation. Do not invent features beyond this brief. Missing detail → **TODO**.

**Next milestone:** Sprint 1 — do not start until explicitly instructed.

---

## 1. Purpose

Define the Inventory System for Project GulfRun: personal account-linked storage of permanently owned cosmetic content, categories, ownership, browse/equip actions, equipment rules, and backend synchronization — without capacity, trading, or loadouts.

---

## 2. System Overview

| Field | Value |
|-------|--------|
| Ownership | Every player owns a **personal Inventory** |
| Contents | The Inventory stores all **permanently owned cosmetic content** |
| Account | The Inventory is **linked to the player's account** |

### Alignment

- P013: purchased cosmetics permanently owned — stored here after unlock.  
- P005: cosmetic-only characters / customization — inventory holds owned cosmetics.  
- P020: Profile Cosmetics customization — inventory category **Profile Cosmetics**.  
- P022: Cosmetics System SoT for categories, defaults, equip timing, rarity existence.  
- P008 race hold-item “inventory” — **not** this system (race item hold is separate; P008).

---

## 3. Inventory Categories

| Category | Status |
|----------|--------|
| **Characters** | Defined |
| **Outfits** | Defined |
| **Headwear** | Defined |
| **Footwear** | Defined |
| **Accessories** | Defined |
| **Trails** | Defined |
| **Visual Effects** | Defined |
| **Victory Celebrations** | Defined |
| **Profile Cosmetics** | Defined |
| **Future Cosmetic Categories** | Future |

### Alignment

- P013 Shop cosmetic categories (Characters, Outfits, Headwear, Footwear, Accessories, Victory Celebrations, Trails, Visual Effects) — **aligned**; Gem/Coin Packs are currency packs, not inventory cosmetics.  
- P005 customization includes **Animations** — **Animations as an Inventory category is not stated in P021** (do not add).  
- P020 Avatar / future Banner, Frame, Badge — fall under **Profile Cosmetics** only as stated; mapping **TODO**.

### TODO — Categories (not provided)

- [ ] Mapping of P020 Avatar / Banner / Frame / Badge into Profile Cosmetics  
- [ ] Mapping P022 **Profile Avatar** / **Profile Frame** ↔ Profile Cosmetics  
- [ ] Whether P005 Animations are stored under an existing category or a future category  
- [ ] List of Future Cosmetic Categories when defined  

---

## 4. Ownership Rules

| Rule ID | Rule |
|---------|------|
| INV-OWN-001 | **Unlocked** cosmetics become **permanently owned**. |
| INV-OWN-002 | Owned items remain available **across all devices** after **account synchronization**. |
| INV-OWN-003 | **Duplicate ownership rules are not defined**. |

### Alignment

- P013 OWN-001 permanent ownership after purchase — **consistent**.  
- Unlock methods beyond Shop — **TODO** (P005 unlock method undefined).

### TODO — Ownership (not provided)

- [ ] Duplicate ownership / repurchase behavior  
- [ ] Non-Shop unlock paths that grant inventory items  

---

## 5. Player Actions

| Action | Status |
|--------|--------|
| **Open Inventory** | Defined |
| **Browse Categories** | Defined |
| **Equip Item** | Defined |
| **Unequip Item** | Defined |
| **Preview Item** | Defined |
| **Sort Items** | Defined |
| **Filter Items** | Defined |

### TODO — Player actions (not provided)

- [ ] Entry point UI (Main Menu, Profile, Shop, pre-race)  
- [ ] Sort / Filter criteria  
- [ ] Unequip: empty slot vs revert to default  

---

## 6. Equipment Rules

| Rule ID | Rule |
|---------|------|
| INV-EQ-001 | **Only one cosmetic** may be equipped **per supported slot**. |
| INV-EQ-002 | Equipping a cosmetic **does not affect gameplay**. |
| INV-EQ-003 | Equipped cosmetics are **automatically used during races**. |

### TODO — Equipping (not provided)

- [ ] Full list of supported slots  
- [ ] Relationship of Character equip vs Selected Character (P005 / P020)  
- [ ] Default cosmetics when a slot is unequipped  

---

## 7. Synchronization

| Field | Value |
|-------|--------|
| Sync | Inventory data is **synchronized with the backend** |
| Account association | Inventory data is **permanently associated** with the player's account |

---

## 8. Rules

| Rule ID | Rule |
|---------|------|
| INV-001 | Inventory items **cannot provide gameplay advantages**. |
| INV-002 | Inventory **only stores owned content**. |
| INV-003 | **Inventory capacity is not defined**. |

### Alignment

- P001 Fair Competition / P013 no P2W / P005 identical stats — **reinforced**.

---

## 9. Dependencies

| Dependency | Note |
|------------|------|
| P013 Shop | Purchase → permanent ownership → Inventory |
| P005 Character / cosmetics | Characters and customization categories |
| P022 Cosmetics | Categories, defaults, equip rules, no gameplay impact |
| P012 Economy | Purchase currencies (Shop), not stored as inventory items |
| Backend | Sync; account association |
| Account / Login | Cross-device availability after sync |

---

## 10. Future Specifications

| Topic | Status |
|-------|--------|
| Future Cosmetic Categories | Future |
| Inventory Capacity | Not defined |
| Favorite Items | Not defined |
| Item Locking | Not defined |
| Duplicate Handling | Not defined |
| Item Trading | Not defined |
| Item Selling | Not defined |
| Item Gifting | Not defined |
| Loadouts | Not defined |
| Collection Progress | Not defined |

---

## 11. Explicitly Not Defined (P021)

- Inventory Capacity  
- Favorite Items  
- Item Locking  
- Duplicate Handling  
- Item Trading  
- Item Selling  
- Item Gifting  
- Loadouts  
- Collection Progress  
- Sort / Filter criteria  
- Supported equip slot list  

---

## 12. Open Questions

| ID | Question |
|----|----------|
| Q-P021-001 | Inventory entry point(s) in UI? |
| Q-P021-002 | Supported equip slots list? |
| Q-P021-003 | Duplicate ownership rules (align P013 Q-P013-002)? |
| Q-P021-004 | P005 Animations vs Inventory categories? |
| Q-P021-005 | Profile Cosmetics ↔ P020 Avatar / Banner / Frame / Badge mapping? |
| Q-P021-006 | Character equip vs Selected Character (P005/P020)? |
| Q-P021-007 | Capacity / Collection Progress future doc? |

---

## 13. Acceptance Criteria

P021 v1.0 is satisfied when all of the following are true:

1. Every player has a personal Inventory linked to the account storing permanently owned cosmetic content.  
2. Categories: Characters, Outfits, Headwear, Footwear, Accessories, Trails, Visual Effects, Victory Celebrations, Profile Cosmetics; Future Cosmetic Categories future.  
3. Unlocked cosmetics permanently owned; available across devices after account sync; duplicate rules not defined.  
4. Actions: Open Inventory, Browse Categories, Equip, Unequip, Preview, Sort, Filter.  
5. One cosmetic per supported slot; equip does not affect gameplay; equipped cosmetics auto-used in races.  
6. Backend sync; permanently associated with account.  
7. No gameplay advantages; only owned content stored; capacity not defined.  
8. Capacity, favorites, locking, duplicates, trading, selling, gifting, loadouts, and collection progress are not invented.  
9. Document version is **P021 v1.0**.

---

## 14. Document Queue

| Order | Document ID | Title | Status |
|-------|-------------|-------|--------|
| 1–20 | P001–P020 | (prior specs) | Approved as previously recorded |
| 21 | P021 | Inventory System Specification | **v1.0 Approved** |
| 22 | P022 | Cosmetics System Specification | **v1.0 Approved** |
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
| **1.0** | 2026-07-31 | Initial Inventory System Specification | Documentation Engineer (from Design Owner brief) |

---

*End of document.*
