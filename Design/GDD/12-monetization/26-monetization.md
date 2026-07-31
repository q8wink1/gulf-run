# 26 — Monetization

**GDD chapter:** 26  
**Status:** Populated (per [P045](../P045-MONETIZATION-SYSTEM-v1.0.md) v1.0)  
**Design Owner:** `[TBD]`  
**Last updated:** 2026-07-31

**Detail specification:** [P045-MONETIZATION-SYSTEM-v1.0.md](../P045-MONETIZATION-SYSTEM-v1.0.md) is the source of truth for this chapter.

---

## 26.1 Monetization philosophy

Fair and player-friendly monetization model. Must never create Pay-to-Win gameplay. All gameplay remains skill-based. Principles: Fair, Transparent, Optional, Player Friendly, Long-Term Sustainability, No Gameplay Advantage. See P045 §2, §4.

## 26.2 Monetization pillars / constraints

| Constraint | Rule | Status |
|------------|------|--------|
| No Pay-to-Win | MON-001 | Defined |
| No gameplay advantage purchasable | MON-002 | Defined |
| Backend purchase validation | MON-003 | Defined |
| Platform billing rules respected | MON-004 | Defined |

## 26.3 Revenue instruments inventory

| Instrument | Used? | Notes | Status |
|------------|-------|-------|--------|
| IAP consumables | Yes | Gem Purchases (Premium Currency) — [P012](../P012-ECONOMY-SYSTEM-v1.0.md) | Defined |
| IAP durables / cosmetics | Yes | Cosmetic Purchases, Limited Time Cosmetic Bundles — [P013](../P013-SHOP-SYSTEM-v1.0.md) | Defined |
| Battle pass / season pass | Yes | Premium Battle Pass — [P029](../P029-BATTLE-PASS-SYSTEM-v1.0.md) | Defined |
| Subscriptions | `[TBD]` | Not mentioned in P045 — not defined | Not defined |
| Ads | `[TBD]` | Not mentioned in P045 — not defined | Not defined |
| Other | Future Monetization Features | Scope not defined | Future |

## 26.4 Fairness vs monetization

Cosmetics never affect gameplay balance; no gameplay advantage may be purchased (MON-002); reinforces P013 FAIR-002/003 and P029 BP-003. Must align with Chapter 17 — alignment confirmed at the principle level; no conflicting content found.

## 26.5 Open questions

| ID | Question | Status |
|----|----------|--------|
| Q-26-001 | What may be purchased that affects power? | **Resolved** — none; no gameplay advantage may be purchased (P045 MON-002) |
| Q-26-002 | Are ads allowed in any surface? | Open — not mentioned in P045; not defined |
