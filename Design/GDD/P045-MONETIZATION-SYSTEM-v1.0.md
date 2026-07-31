# P045 — Monetization System Specification

| Field | Value |
|-------|--------|
| Document ID | P045 |
| Title | Monetization System Specification |
| Version | **1.0** |
| Status | Approved (monetization sources, principles & rules scope only) |
| Project | Project GulfRun |
| Authority | Official source of truth for **monetization sources**, **design principles**, **cosmetics/premium currency/Battle Pass monetization roles**, **limited offers existence**, **purchase restoration**, and **fairness / no-P2W rules** stated herein |
| Depends on | [P001](P001-GAME-VISION-v1.0.md), [P012](P012-ECONOMY-SYSTEM-v1.0.md), [P013](P013-SHOP-SYSTEM-v1.0.md), [P022](P022-COSMETICS-SYSTEM-v1.0.md), [P029](P029-BATTLE-PASS-SYSTEM-v1.0.md) |
| Last updated | 2026-07-31 |

**Rules:** Documentation only. No implementation. Do not invent gameplay. Do not add implementation details beyond this brief. Missing detail → **TODO**.

**Next milestone:** Sprint 1 — do not start until explicitly instructed.

---

## 1. Purpose

Define the Monetization System for Project GulfRun: monetization sources, design principles, the monetization role of Cosmetics/Premium Currency/Battle Pass, the existence of Limited Offers, purchase restoration support, and the no-Pay-to-Win rules — without prices, bundles, discounts, subscriptions, starter packs, welcome offers, regional pricing, taxes, or refund policy.

---

## 2. System Overview

| Field | Value |
|-------|--------|
| Model | Project GulfRun uses a **fair and player-friendly monetization model** |
| Constraint | Monetization **must never create Pay-to-Win gameplay** |
| Gameplay basis | **All gameplay remains skill-based** |

### Alignment

- [P001](P001-GAME-VISION-v1.0.md) Fair Competition — monetization must not violate the skill-based competitive premise.
- [P012](P012-ECONOMY-SYSTEM-v1.0.md) Economy — Premium Currency (Gems) rules are defined there; this document does not redefine currency mechanics.
- [P013](P013-SHOP-SYSTEM-v1.0.md) Shop — the Shop is the access point for cosmetic and currency purchases; Shop FAIR-001/002/003 rules are reinforced here.
- [P029](P029-BATTLE-PASS-SYSTEM-v1.0.md) Battle Pass — Premium Battle Pass purchase is a monetization source; BP-003 (never Pay-to-Win) is reinforced here.
- [Design/GDD/12-monetization/26-monetization.md](12-monetization/26-monetization.md) — chapter placeholder; populated by this document (§12).

---

## 3. Monetization Sources

| Source | Status |
|--------|--------|
| **Gem Purchases** | Defined |
| **Battle Pass** | Defined |
| **Cosmetic Purchases** | Defined |
| **Limited Time Cosmetic Bundles** | Defined (existence only) |
| **Future Monetization Features** | Future |

### TODO — Sources (not provided)

- [ ] Limited Time Cosmetic Bundles — contents / pricing / cadence
- [ ] Future Monetization Features — scope

---

## 4. Design Principles

| Principle | Status |
|-----------|--------|
| **Fair** | Defined |
| **Transparent** | Defined |
| **Optional** | Defined |
| **Player Friendly** | Defined |
| **Long-Term Sustainability** | Defined |
| **No Gameplay Advantage** | Defined |

---

## 5. Purchase Flow

### 5.1 Cosmetics

| Field | Value |
|-------|--------|
| Purchase | Players **may purchase cosmetic content** |
| Balance impact | Cosmetics **never affect gameplay balance** |

```
Player browses Shop (P013)
↓
Selects cosmetic content
↓
Purchase with Coins / Gems (P012) or real-money (Gem purchase)
↓
Cosmetic granted — permanently owned (P013 OWN-001)
↓
Gameplay balance unaffected
```

### 5.2 Premium Currency

| Field | Value |
|-------|--------|
| Existence | **Premium Currency exists** |
| Acquisition | Premium Currency **is purchased using platform payment systems** |
| Rules authority | Premium Currency rules are **defined by the Economy System** — SoT **[P012](P012-ECONOMY-SYSTEM-v1.0.md)** (Gems) |

### 5.3 Battle Pass

| Field | Value |
|-------|--------|
| Existence | **Premium Battle Pass exists** |
| Rewards | Battle Pass rewards **are defined separately** — SoT **[P029](P029-BATTLE-PASS-SYSTEM-v1.0.md)** |

### 5.4 Limited Offers

| Field | Value |
|-------|--------|
| Existence | **Limited-time offers exist** |
| Rules | **Offer rules are not defined** |

### 5.5 Purchase Restoration

| Field | Value |
|-------|--------|
| Support | **Supported** |
| Implementation | **Platform-specific implementation is not defined** |

### TODO — Purchase Flow (not provided)

- [ ] Limited offer trigger conditions / duration / catalog
- [ ] Purchase restoration platform-specific flow (App Store / Google Play / other)

---

## 6. Rules

| Rule ID | Rule |
|---------|------|
| MON-001 | **No Pay-to-Win.** |
| MON-002 | **No gameplay advantages may be purchased.** |
| MON-003 | Purchases must be **securely validated by the backend**. |
| MON-004 | **Platform billing rules must be respected.** |

### Alignment

- MON-001/002 reinforce [P013](P013-SHOP-SYSTEM-v1.0.md) FAIR-002/FAIR-003 and [P029](P029-BATTLE-PASS-SYSTEM-v1.0.md) BP-003.
- MON-003 aligns with [SECURITY_STRATEGY.md](../../docs/05-security/SECURITY_STRATEGY.md) §7 (IAP & payments — server-only receipt validation, replay caches, immutable grant ledger) and the `Server/services/purchase/` component (receipt validation, replay protection, grant orchestration). This document does not add new implementation detail beyond restating the brief and pointing to the existing engineering source.
- MON-004 defers to platform store policy; no platform-specific rule is invented here.

---

## 7. Dependencies

| Dependency | Note |
|------------|------|
| P001 Game Vision | Fair competition / skill-based premise |
| P012 Economy System | Premium Currency (Gems) rules |
| P013 Shop System | Purchase access point; cosmetic-only sales; fairness rules |
| P022 Cosmetics System | Cosmetic content being monetized |
| P029 Battle Pass System | Premium Battle Pass purchase; reward definitions |
| SECURITY_STRATEGY.md §7 | Backend purchase validation posture (engineering) |
| Server/services/purchase | Receipt validation, replay protection, grant orchestration (engineering, existing) |

---

## 8. Future Specifications

| Topic | Status |
|-------|--------|
| Prices | Not defined |
| Bundles | Not defined |
| Discounts | Not defined |
| Subscription | Not defined |
| Starter Packs | Not defined |
| Welcome Offers | Not defined |
| Regional Pricing | Not defined |
| Taxes | Not defined |
| Refund Policy | Not defined |
| Future Monetization Features | Future |
| Limited Time Cosmetic Bundles (contents) | Not defined |
| Offer rules | Not defined |
| Purchase restoration (platform-specific) | Not defined |

---

## 9. Explicitly Not Defined (P045)

- Prices
- Bundles
- Discounts
- Subscription
- Starter Packs
- Welcome Offers
- Regional Pricing
- Taxes
- Refund Policy

---

## 10. Open Questions

| ID | Question |
|----|----------|
| Q-P045-001 | Limited Time Cosmetic Bundles — contents, cadence, pricing? |
| Q-P045-002 | Limited Offer rules (trigger, duration, catalog)? |
| Q-P045-003 | Purchase restoration platform-specific implementation (iOS/Android/other)? |
| Q-P045-004 | Prices, Bundles, Discounts, Subscription, Starter Packs, Welcome Offers, Regional Pricing, Taxes, Refund Policy — timeline / document ID? |
| Q-P045-005 | Future Monetization Features scope? |

---

## 11. Acceptance Criteria

P045 v1.0 is satisfied when all of the following are true:

1. Fair, player-friendly monetization model confirmed; never Pay-to-Win; all gameplay remains skill-based.
2. Monetization Sources: Gem Purchases, Battle Pass, Cosmetic Purchases, Limited Time Cosmetic Bundles; Future Monetization Features future.
3. Design Principles: Fair, Transparent, Optional, Player Friendly, Long-Term Sustainability, No Gameplay Advantage.
4. Cosmetics: purchasable; never affect gameplay balance.
5. Premium Currency exists; purchased via platform payment systems; rules defined by Economy System (P012).
6. Premium Battle Pass exists; rewards defined separately (P029).
7. Limited-time offers exist; offer rules not defined.
8. Purchase Restoration supported; platform-specific implementation not defined.
9. Rules: no Pay-to-Win; no gameplay advantages purchasable; purchases securely validated by backend; platform billing rules respected.
10. Prices, Bundles, Discounts, Subscription, Starter Packs, Welcome Offers, Regional Pricing, Taxes, and Refund Policy are not invented.
11. No gameplay mechanics invented beyond this brief.
12. Document version is **P045 v1.0**.

---

## 12. GDD Chapter Alignment — 26 Monetization

This document is the detail specification backing [Design/GDD/12-monetization/26-monetization.md](12-monetization/26-monetization.md). That chapter file remains the chapter-level index; §26.1–26.5 status:

| Chapter section | Status after P045 |
|------------------|--------------------|
| 26.1 Monetization philosophy | Fair / Transparent / Optional / Player Friendly / Long-Term Sustainability / No Gameplay Advantage; never P2W; skill-based gameplay — see §4 above |
| 26.2 Monetization pillars / constraints | MON-001–MON-004 — see §6 above |
| 26.3 Revenue instruments inventory | Gem Purchases, Battle Pass, Cosmetic Purchases, Limited Time Cosmetic Bundles defined; Subscriptions and Ads **not defined** (do not assume either exists) |
| 26.4 Fairness vs monetization | No Pay-to-Win; no gameplay advantage purchasable — aligned with Chapter 17 fairness principles (P013/P029 cross-refs) |
| 26.5 Open questions | Q-26-001 (power-affecting purchases) — answered: **none**, no gameplay advantage may be purchased (MON-002). Q-26-002 (ads) — **remains open**, ads are not mentioned in this brief; not defined |

---

## 13. Document Queue

| Order | Document ID | Title | Status |
|-------|-------------|-------|--------|
| 1–38 | P001–P038 | (prior specs) | Approved as previously recorded |
| 39 | P039 | Backend Architecture Specification (engineering doc — docs/02-architecture/) | v1.0 Approved |
| 40 | P040 | Database Architecture Specification (engineering doc — docs/02-architecture/) | v1.0 Approved |
| 41 | P041 | Authentication System Specification (engineering doc — docs/02-architecture/) | v1.0 Approved |
| 42 | P042 | Player Profile System Specification [CONFLICT with P020] | v1.0 Approved-per-brief |
| 43 | P043 | Anti-Cheat System Specification (engineering doc — docs/05-security/) | v1.0 Approved |
| 44 | P044 | Analytics System Specification (engineering doc — docs/02-architecture/) | v1.0 Approved |
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
| **1.0** | 2026-07-31 | Initial Monetization System Specification | Documentation Engineer (from Design Owner brief) |

---

*End of document.*
