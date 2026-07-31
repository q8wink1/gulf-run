# P013 — Shop System Specification

| Field | Value |
|-------|--------|
| Document ID | P013 |
| Title | Shop System Specification |
| Version | **1.0** |
| Status | Approved (shop categories & rules scope only) |
| Project | Project GulfRun |
| Authority | Official source of truth for **Shop access**, **categories**, **purchase currency rules**, **cosmetic ownership**, and **fairness / no pay-to-win** rules stated herein |
| Depends on | [P001](P001-GAME-VISION-v1.0.md), [P004](P004-MAIN-MENU-v1.0.md), [P005](P005-CHARACTER-SYSTEM-v1.0.md), [P012](P012-ECONOMY-SYSTEM-v1.0.md) |
| Last updated | 2026-07-31 |

**Rules:** Documentation only. No implementation. Do not invent shop features beyond this brief. Missing detail → **TODO**.

**Next milestone:** Sprint 1 — do not start until explicitly instructed.

---

## 1. Purpose

Define the Shop: access from Main Menu, cosmetic-only sales, official categories, Coins/Gems purchase rules, permanent ownership, and anti–pay-to-win constraints — **without** prices, bundles, or offers.

---

## 2. Shop Overview

| Field | Value |
|-------|--------|
| Access | The Shop is accessible from the **Main Menu** |
| Purpose | The Shop allows players to obtain **cosmetic content** |
| Gameplay impact | The Shop **never sells gameplay advantages** |

### Alignment

- P004: Shop button / store exists — **this document** is the Shop system SoT.  
- P005: characters and cosmetics are cosmetic only; identical stats — **reinforced** by Shop rules.  
- P001 Fair Competition / P009 balancing — Shop must not create Pay-to-Win (§6).  
- P012: Coins for future cosmetic purchases; Gems may be purchased — **used** as purchase currencies here.

---

## 3. Shop Categories

Official Shop categories:

| Category |
|----------|
| **Characters** |
| **Outfits** |
| **Headwear** |
| **Footwear** |
| **Accessories** |
| **Victory Celebrations** |
| **Trails** |
| **Visual Effects** |
| **Gem Packs** |
| **Coin Packs** |

### Notes

- Cosmetic categories align with P005 customization categories (plus **Characters**; P005 also listed Animations — **Animations as a Shop category is not stated in P013** — do not add).  
- **Gem Packs** / **Coin Packs**: currency packs for purchase (**TODO** pack contents/prices — not defined).

### TODO — Categories (not provided)

- [ ] Item lists per category  
- [ ] Whether Animations (P005) appear in Shop later  

---

## 4. Purchase Rules

| Rule ID | Rule |
|---------|------|
| SHP-001 | Cosmetic items may require **Coins** **OR** **Gems**. |
| SHP-002 | Some items may become available through **future events**. |
| SHP-003 | The Shop sells **cosmetic content** only (no gameplay advantages). |

### TODO — Purchase (not provided)

- [ ] Which items cost Coins vs Gems  
- [ ] Prices (explicitly not defined)  
- [ ] Event availability rules (future)  

---

## 5. Ownership Rules

| Rule ID | Rule |
|---------|------|
| OWN-001 | Purchased cosmetics become **permanently owned** (stored in **[P021](P021-INVENTORY-SYSTEM-v1.0.md)**). |
| OWN-002 | **Duplicate ownership rules are not defined**. |

### TODO — Ownership (not provided)

- [ ] Duplicate purchase / convert / reject behavior  
- [ ] Relationship to P005 “may own multiple characters / cosmetics”  

---

## 6. Shop Rules (Fairness)

| Rule ID | Rule |
|---------|------|
| FAIR-001 | The Shop must remain **fair**. |
| FAIR-002 | The Shop must **never create Pay-to-Win gameplay**. |
| FAIR-003 | **All gameplay mechanics remain equal for every player**. |

---

## 7. Future Dependencies

| Dependency | Note |
|------------|------|
| P004 Main Menu | Shop entry point |
| P005 Character / cosmetics | Content types; unlock methods may include Shop |
| P012 Economy | Coins / Gems wallets for purchases; Gem/Coin packs |
| P021 Inventory | Permanently owned cosmetics storage |
| P022 Cosmetics | Cosmetic categories / ownership / no gameplay impact |
| Store UI layout | **TODO** |
| Events | May unlock some items (SHP-002) |
| IAP | Gem Packs / Coin Packs purchase flow |

---

## 8. Explicitly Not Defined (P013)

- Prices  
- Bundles  
- Discounts  
- Limited Time Offers  
- Daily Shop Rotation  
- Featured Items  
- Refund Rules  
- Gift System  
- Promo Codes  
- Taxes  

---

## 9. Open Questions

| ID | Question |
|----|----------|
| Q-P013-001 | Prices and which items cost Coins vs Gems? |
| Q-P013-002 | Duplicate ownership when buying an owned cosmetic? |
| Q-P013-003 | Are Coin Packs / Gem Packs real-money IAP only? |
| Q-P013-004 | Shop UI layout / tabs matching categories? |
| Q-P013-005 | Document ID for Events that grant Shop availability? |
| Q-P013-006 | Refund rules document? |

---

## 10. Acceptance Criteria

P013 v1.0 is satisfied when all of the following are true:

1. Shop accessible from Main Menu; cosmetic content only; never sells gameplay advantages.  
2. Categories listed exactly: Characters, Outfits, Headwear, Footwear, Accessories, Victory Celebrations, Trails, Visual Effects, Gem Packs, Coin Packs.  
3. Cosmetics may require Coins or Gems; some items via future events.  
4. Purchased cosmetics permanently owned; duplicate rules not defined.  
5. Fair; no Pay-to-Win; gameplay mechanics equal for every player.  
6. Prices, bundles, discounts, limited offers, daily rotation, featured items, refunds, gifts, promo codes, and taxes are not invented.  
7. Future dependencies, open questions, and acceptance criteria are present.  
8. Document version is **P013 v1.0**.

---

## 11. Document Queue

| Order | Document ID | Title | Status |
|-------|-------------|-------|--------|
| 1–12 | P001–P012 | (prior specs) | Approved as previously recorded |
| 13 | P013 | Shop System Specification | **v1.0 Approved** |
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
| **1.0** | 2026-07-31 | Initial Shop System Specification | Documentation Engineer (from Design Owner brief) |

---

*End of document.*
