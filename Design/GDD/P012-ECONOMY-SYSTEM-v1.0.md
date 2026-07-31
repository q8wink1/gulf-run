# P012 — Economy System Specification

| Field | Value |
|-------|--------|
| Document ID | P012 |
| Title | Economy System Specification |
| Version | **1.0** |
| Status | Approved (currency & wallet scope only) |
| Project | Project GulfRun |
| Authority | Official source of truth for **official currencies**, **wallet structure**, and **economy rules** stated herein |
| Depends on | [P001](P001-GAME-VISION-v1.0.md), [P011](P011-POST-RACE-RESULTS-v1.0.md) |
| Last updated | 2026-07-31 |

**Rules:** Documentation only. No implementation. Do not invent economy features beyond this brief. Missing detail → **TODO**.

**Next milestone:** Sprint 1 — do not start until explicitly instructed.

---

## 1. Purpose

Define the two official currencies, how each is characterized, player wallet structure, and account storage / sync rules — **without** defining rewards, prices, store, or offers.

---

## 2. Economy Overview

The game contains **two official currencies**:

| Currency ID | Name |
|-------------|------|
| Currency 01 | **Coins** |
| Currency 02 | **Gems** |

No other currencies are defined in P012.

### Alignment

- P001 Long-Term Progression / LiveOps: economy systems deferred — **currencies now named** here; reward amounts still deferred.  
- P011: Coins/Gems were “not defined” as **reward displays** — P012 defines them as **currencies**; post-race **reward amounts** remain not defined (P011 placeholder / P012 §7).

---

## 3. Currencies

### 3.1 Coins (Currency 01)

| Field | Value |
|-------|--------|
| Role | **Primary in-game currency** |
| Earned | Coins are **earned through gameplay** |
| Spend (intent) | Coins are used for **future cosmetic purchases** |

### TODO — Coins (not provided)

- [ ] Which gameplay actions grant Coins (amounts → not defined)  
- [ ] Cosmetic purchase catalog / prices (Store / cosmetics future)  

### 3.2 Gems (Currency 02)

| Field | Value |
|-------|--------|
| Role | **Premium currency** |
| Purchase | Gems **may be purchased** |
| Other acquisition | **Additional acquisition methods are not defined** |

### TODO — Gems (not provided)

- [ ] Purchase packages / prices (not defined)  
- [ ] Platform IAP mapping (engineering / future Store spec)  

---

## 4. Wallet Structure

| Field | Value |
|-------|--------|
| Ownership | Every player owns an **individual wallet** |
| Coins | **Coins Wallet** |
| Gems | **Gems Wallet** |

### TODO — Wallet (not provided)

- [ ] UI display locations (e.g. Main Menu / Shop)  
- [ ] Wallet visibility on Profile (**not stated**)  

---

## 5. Economy Rules

| Rule ID | Rule |
|---------|------|
| ECO-001 | Coins and Gems are **permanently stored on the player's account**. |
| ECO-002 | Currency balances are **synchronized with the backend**. |
| ECO-003 | **Negative balances are not allowed**. |
| ECO-004 | Official currencies are only **Coins** and **Gems** (as named in §2). |

### Alignment

- Server / backend authority for balances matches engineering posture in `docs/` and P011 RES-002 spirit (server as source of truth for durable account data).

---

## 6. Future Dependencies

| Dependency | Note |
|------------|------|
| Store specification | **[P013](P013-SHOP-SYSTEM-v1.0.md)** — categories & rules; prices still TBD |
| Cosmetic purchases | Coins spend intent; catalog not defined |
| Reward / grant tables | Coin/Gem **rewards** not defined |
| P011 Results placeholder | May later show currency grants — amounts TBD |
| P005 cosmetics | Likely spend target for Coins — not wired here |
| IAP / Purchase service | Gems may be purchased — implementation later |

---

## 7. Explicitly Not Defined (P012)

- Coin Rewards  
- Gem Rewards  
- Prices  
- Bundles  
- Offers  
- Discounts  
- Store  
- Battle Pass  
- Daily Rewards  
- Events  
- Achievements  
- Refund Rules  

---

## 8. Open Questions

| ID | Question |
|----|----------|
| Q-P012-001 | Which gameplay actions grant Coins (and where are amounts specified)? |
| Q-P012-002 | Document ID for Store / cosmetic purchase specification? |
| Q-P012-003 | Where are Coins/Gems wallets shown in UI? |
| Q-P012-004 | Can Gems be earned by any non-purchase method later, or purchase-only until then? *(Additional methods not defined — do not assume.)* |
| Q-P012-005 | Refund / chargeback handling document? |

---

## 9. Acceptance Criteria

P012 v1.0 is satisfied when all of the following are true:

1. Exactly two official currencies: Coins and Gems.  
2. Coins: primary; earned through gameplay; used for future cosmetic purchases.  
3. Gems: premium; may be purchased; other acquisition methods not defined.  
4. Each player has an individual wallet with Coins Wallet and Gems Wallet.  
5. Currencies permanently on account; backend sync; no negative balances.  
6. Coin/Gem rewards, prices, bundles, offers, discounts, Store, Battle Pass, Daily Rewards, Events, Achievements, and Refund Rules are not invented.  
7. Future dependencies, open questions, and acceptance criteria are present.  
8. Document version is **P012 v1.0**.

---

## 10. Document Queue

| Order | Document ID | Title | Status |
|-------|-------------|-------|--------|
| 1–11 | P001–P011 | (prior specs) | Approved as previously recorded |
| 12 | P012 | Economy System Specification | **v1.0 Approved** |
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

## 11. Change Log

| Version | Date | Summary | Author |
|---------|------|---------|--------|
| **1.0** | 2026-07-31 | Initial Economy System Specification (Coins, Gems, wallets) | Documentation Engineer (from Design Owner brief) |

---

*End of document.*
