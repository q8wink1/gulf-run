# 06 — Gameplay Systems Catalog

**GDD chapter:** 06  
**Status:** Partial — confirmed systems only  
**Design Owner:** `[TBD]`  
**Last updated:** 2026-07-31

> Do not invent systems. List only systems the Design Owner confirms.

---

## 6.1 System inventory

| System ID | Name | Purpose (1 line) | Depends on | Owner | Status |
|-----------|------|------------------|------------|-------|--------|
| SYS-001 | Item boxes | Boxes in every race; touch collect; one item hold; random item | Race / P008 | Design Owner | **P008 v1.0** |
| SYS-012 | Item & weapon system | Items from boxes only; consume on use; weapons = item category affecting opponents (no permanent eliminate) | P009 | Design Owner | **P009 v1.0** (catalog TBD) |
| SYS-013 | Race rules | 4p simultaneous equal start; countdown; finish order 1st–4th | P010 | Design Owner | **P010 v1.0** |
| SYS-014 | Disconnection | System exists | Race | Design Owner | Exists — rules TBD |
| SYS-016 | Post-race results | Results Screen; server-authoritative; rewards placeholder | P011 | Design Owner | **P011 v1.0** |
| SYS-019 | Matchmaking | Auto online MM; Quick Match / Friend Party / Private Room; 4p | P017 | Design Owner | **P017 v1.0** (algorithm TBD) |
| SYS-020 | Private Room | Invite-based; not public MM; code + friend invite; host controls; 4p | P018 | Design Owner | **P018 v1.0** |
| SYS-021 | Leaderboard | Global/Regional/Friends/Season; read-only; backend-authoritative | P019 | Design Owner | **P019 v1.0** (formula TBD) |
| SYS-022 | Player Profile | Unique account-linked identity; stats; cosmetic customize; public | P020 | Design Owner | **P020 v1.0** |
| SYS-023 | Inventory | Personal cosmetic inventory; equip slots; backend sync; no P2W | P021 | Design Owner | **P021 v1.0** |
| SYS-024 | Cosmetics | Visual-only personalization; defaults; equip; rarity exists (undefined) | P022 | Design Owner | **P022 v1.0** |
| SYS-025 | Player Progression | Level, Rank, XP, Season Progress, Achievements; backend sync; no P2W | P023 | Design Owner | **P023 v1.0** (formulas TBD) |
| SYS-026 | Player Level | Level 1 start; XP carry-over; display; level-up notify; no gameplay power | P024 | Design Owner | **P024 v1.0** (XP formula TBD) |
| SYS-027 | Competitive Rank | Seasonal competitive rank; promo/demo exist; separate from Level; never purchasable | P025 | Design Owner | **P025 v1.0** (names/formulas TBD) |
| SYS-028 | Daily Challenges | Daily challenges; 24h reset; progress/claim; rewards TBD | P026 | Design Owner | **P026 v1.0** (objectives/rewards TBD) |
| SYS-029 | Weekly Challenges | Weekly challenges; 7-day reset; longer-term than Daily; rewards TBD | P027 | Design Owner | **P027 v1.0** (objectives/rewards TBD) |
| SYS-030 | Achievements | Long-term one-time accomplishments; permanent; claim rewards TBD | P028 | Design Owner | **P028 v1.0** (list/rewards TBD) |
| SYS-031 | Battle Pass | Seasonal Free+Premium tracks; simultaneous progress; no P2W | P029 | Design Owner | **P029 v1.0** (tiers/prices TBD) |
| SYS-032 | Season System | Fixed-period seasons; auto participate; content containers; no P2W | P030 | Design Owner | **P030 v1.0** (duration/names TBD) |
| SYS-033 | Live Events | Limited-time events; types incl. Season/Holiday/Ramadan; active-only | P031 | Design Owner | **P031 v1.0** (rewards/missions TBD) |
| SYS-034 | Notifications | In-game + push; typed; read/delete; configurable push categories | P032 | Design Owner | **P032 v1.0** |
| SYS-035 | Inbox (Mail) | Backend-synced Inbox; system messages; claim attachments once; expiry auto-remove | P033 | Design Owner | **P033 v1.0** (attachment types/TTL TBD) |
| SYS-036 | Settings | 10 categories; account-linked + device-local; auto-saved | P034 | Design Owner | **P034 v1.0** (many option values TBD) |
| SYS-037 | Audio | 8 categories; UI/Gameplay/Character/Environment/Weapons/Voice/Music/Ambience; mobile-optimized | P035 | Design Owner | **P035 v1.0** (sound design, weapon audio TBD) |
| SYS-038 | Music | 12 categories; dynamic; Gulf-identity; loops seamlessly; mobile-optimized | P036 | Design Owner | **P036 v1.0** (tracks/duration TBD) |
| SYS-039 | Localization | Arabic + English launch; full RTL; no hardcoded text; fallback to English | P037 | Design Owner | **P037 v1.0** (future languages/voice TBD) |
| SYS-040 | Tutorial | FTUE; auto-runs for new players; skippable/replayable; auto-saved progress | P038 | Design Owner | **P038 v1.0** (rewards/practice mode TBD) |
| SYS-002 | Obstacle system | Every map; avoid Jump/Double Jump; collision exists | Race / P007 | Design Owner | **P007 v1.0** (types/effects TBD) |
| SYS-003 | Main Menu / hub UI | Post-login hub; Play + social/meta buttons | P004 | Design Owner | P004 v1.0 |
| SYS-004 | Friends | Friends List; requests; invites to QM/Private Room | P014 | Design Owner | **P014 v1.0** |
| SYS-005 | Clans | One clan per player; Leader/Co-Leader/Member; text chat | P015 | Design Owner | **P015 v1.0** |
| SYS-006 | Shop / Store | Cosmetic shop from Main Menu; Coins or Gems; no P2W | P013 | Design Owner | **P013 v1.0** (prices TBD) |
| SYS-007 | Challenges | Daily (**P026**) + Weekly (**P027**); hub UX TBD | P026 / P027 | Design Owner | **P026 / P027 v1.0** |
| SYS-009 | Character system | Cosmetic characters; select one before race; identical stats | P005 | Design Owner | P005 v1.0 |
| SYS-011 | Map system | One map per race; random select; six official maps | P006 | Design Owner | P006 v1.0 |

## 6.2 System template (copy per system)

### SYS-XXX — `[TBD: name]`

| Field | Value |
|-------|-------|
| Status | Template |
| Player-facing summary | `[TBD]` |
| Design goals | `[TBD]` |
| Inputs | `[TBD]` |
| Outputs / outcomes | `[TBD]` |
| Failure / edge cases | `[TBD]` |
| Authority (client vs server intent) | `[TBD]` — must align with engineering authority docs |
| Tunables | `[TBD]` |
| Telemetry needs (design) | `[TBD]` |
| Open questions | `[QUESTION]` |

### SYS-002 — Obstacle system

| Field | Value |
|-------|-------|
| Status | **P007 v1.0** — system specified; types/collision effects still undefined |
| Player-facing summary | Players avoid obstacles during races; obstacles on every map; Jump / Double Jump over |
| Spec document | **[P007](../P007-OBSTACLE-SYSTEM-v1.0.md)** — Obstacle System Specification v1.0 |
| Design goals | Fair, avoidable, never impossible; Jump / Double Jump over (P007) |
| Behavior / types / damage / collision effects | **Still undefined** per P007 §5–§7 |

## 6.3 Explicitly not in scope (systems)

Full definitions for weapons, maps, characters, economy, etc. remain deferred per P001/P003 Non Goals.

## 6.4 Open questions

| ID | Question | Status |
|----|----------|--------|
| Q-06-001 | `[QUESTION]` Which systems exist for vertical slice vs launch? | Open |
| — | Obstacle types / collision effects | Still open inside P007 Non Goals |
