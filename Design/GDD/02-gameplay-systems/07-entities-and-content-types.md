# 07 — Entities & Content Types

**GDD chapter:** 07  
**Status:** Partial — synced to P005  
**Design Owner:** `[TBD]`  
**Last updated:** 2026-07-31

> Character SoT: [P005](../P005-CHARACTER-SYSTEM-v1.0.md). Do not invent characters.

---

## 7.1 Entity taxonomy

| Type ID | Type name | Role in fantasy | Player-controllable? | Status |
|---------|-----------|-----------------|----------------------|--------|
| ENT-001 | Character | Cosmetic avatar for races | Yes (select / control runner) | P005 — cosmetic only |
| ENT-002 | Cosmetic item | Appearance customization | Equipped on character/account (**TODO** scope) | P005 categories |

## 7.2 Character 01 — Male default

| Field | Value |
|-------|-------|
| Status | Approved default (P005) |
| Fantasy / fantasy role | Gulf-inspired presentation (P001 identity) |
| Gameplay role | Identical stats; no advantage |
| Default outfit | White Dishdasha; White Ghutra; Black Agal; Traditional Sandals |
| Acquisition | **TODO** — unlock via future systems; starter grant not stated |

## 7.3 Character 02 — Female default

| Field | Value |
|-------|-------|
| Status | Approved default (P005) |
| Gameplay role | Identical stats; no advantage |
| Default outfit | Black Abaya; Black Sheila; Traditional Sandals |
| Acquisition | **TODO** — unlock via future systems; starter grant not stated |

## 7.4 Customization categories (P005)

Outfits · Headwear · Footwear · Accessories · Animations · Victory Celebrations · Trails · Visual Effects — all cosmetic only.

## 7.5 Relationships

| From | Relation | To | Rules | Status |
|------|----------|----|-------|--------|
| Player | selects active | Character | One active per race; may own many | P005 |
| Player | may own | Cosmetic items | Multiple | P005 |

## 7.6 Open questions

See P005 §9.
