# 19 — UI / UX Screens & Flows

**GDD chapter:** 19  
**Status:** Partial — synced to P004 / P020 / P026 / P027  
**Design Owner:** `[TBD]`  
**Last updated:** 2026-07-31

> Main Menu SoT: [P004](../P004-MAIN-MENU-v1.0.md).  
> Player Profile SoT: [P020](../P020-PLAYER-PROFILE-SYSTEM-v1.0.md).
> Daily Challenges SoT: [P026](../P026-DAILY-CHALLENGES-SYSTEM-v1.0.md).
> Weekly Challenges SoT: [P027](../P027-WEEKLY-CHALLENGES-SYSTEM-v1.0.md).
> Design language / principles / visual style / interaction rules SoT: [P047](../P047-UI-UX-DESIGN-SYSTEM-v1.0.md). Do not invent screens.

---

## 19.1 Information architecture

```
Login (TODO / not specified)
  → Main Menu (P004)
       → Play screen → Quick Match | Invite Friend | Private Room → (P002 Matchmaking…)
       → Friends (P014)
       → Clans (P015)
       → Shop (P013)
       → Challenges (P026 Daily / P027 Weekly)
       → Profile (P020)
       → Settings (future spec)
```

## 19.2 Screen inventory

| Screen ID | Screen name | Purpose | Entry points | Exit points | Status |
|-----------|-------------|---------|--------------|-------------|--------|
| UI-001 | Main Menu | Hub after login | After login | Play / Friends / Clans / Shop / Challenges / Profile / Settings | P004 |
| UI-002 | Play screen | Choose match entry | Play button | Quick Match / Invite Friend / Private Room | P004 |
| UI-003 | Profile | Player identity; info, stats, cosmetics | Profile button; Friends; Leaderboard | **TODO** | **P020** |
| UI-004 | Friends | Friends List / requests / invites | Friends button (P004) | **TODO** | **P014** |
| UI-005 | Clans | Clan hub / create / join / manage | Clans button (P004) | **TODO** | **P015** |
| UI-006 | Shop | Obtain cosmetics / currency packs | Shop button (P004) | **TODO** | **P013** |
| UI-007 | Challenges | Daily + Weekly Challenges | Challenges button (P004) | **TODO** | **P026** / **P027** |
| UI-008 | Settings | Settings | Settings button | **TODO** | Future spec |
| UI-010 | Results Screen | Post-race results for all 4 players | After race (P010/P011) | Continue (**TODO**) / Main Menu | **P011** |

## 19.3 Screen template — Main Menu (UI-001)

| Field | Value |
|-------|-------|
| Status | Approved structure (P004) |
| Purpose | First screen after login |
| Orientation | Landscape |
| Primary CTA | Play |
| Secondary CTAs | Friends, Clans, Shop, Challenges, Profile, Settings |
| Key widgets / modules | Buttons listed only — layout **TODO** |
| Empty / error / loading states | **TODO** |
| Analytics events (design names) | **TODO** |

## 19.4 Critical user flows

| Flow ID | Name | Steps (ordered) | Success criteria | Status |
|---------|------|-----------------|------------------|--------|
| FLOW-001 | Enter Quick Match | Main Menu → Play → Quick Match | Auto search; max 4 players | P004 |
| FLOW-002 | Invite Friend | Main Menu → Play → Invite Friend | Invite friends already added | P004 |
| FLOW-003 | Private Room | Main Menu → Play → Private Room | Create room; others join via Room Code | P004 / P018 |
| FLOW-004 | View Own Profile | Main Menu → Profile | P020 fields / actions | P020 |
| FLOW-005 | View Another Profile | Friends / Leaderboard → View Profile | Public profile (P020) | P014 / P019 / P020 |

## 19.5 Navigation model

See P004 §8. Aligns with P002 Stages 2–5 for play paths.

## 19.6 Open questions

See P004 §11 and P020 §12.
