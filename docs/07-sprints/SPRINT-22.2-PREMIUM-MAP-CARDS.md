# Sprint 22.2 — Premium Map Cards

**Scope:** Enhance Map Voting map cards with premium content and visual states only. No vote counting, countdown, networking, backend, SessionManager, or gameplay.

**Status:** Complete.

## Cards

| Card | Map | Flag | Difficulty | Duration |
|------|-----|------|------------|----------|
| MapCard_0 | Kuwait City | KW (green) | Medium | Est. 3:30 |
| MapCard_1 | Dubai Marina | AE (red) | Hard | Est. 4:15 |
| MapCard_2 | Muscat Coast | OM (red) | Easy | Est. 2:45 |

Each card includes:

- Large map preview placeholder (stacked color panels + caption)
- Map name
- Country flag + country code
- Difficulty badge
- Short description
- Estimated race duration
- Vote button placeholder

## Visual states (UI only)

| State | Behavior |
|-------|----------|
| **Selected** | Gold glowing border, scale 1.05, gold shadow glow, checkmark icon, Vote → Voted |
| **Hover** | Scale 1.03 + deeper drop shadow (`MapCardVisual`) |
| **Locked** | `LockedRoot` with lock icon + Locked label — **inactive by default** on all three |

## Hierarchy (per card)

```
MapCard_N (+ MapCardVisual, Shadow)
├── Fill
├── MapPreview → PreviewTop / PreviewBottom / PreviewAccent / PreviewCaption
├── MetaRow → CountryFlag / CountryCode / DifficultyBadge → DifficultyText
├── MapName
├── Description
├── DurationText
├── VoteButton
├── SelectedCheckmark (inactive until selected)
└── LockedRoot (inactive) → LockIcon / LockedLabel
```

## Scripts

- `MapVotingScreenController` — Back + local Vote selection (border, scale via visual, checkmark)
- `MapCardVisual` — pointer enter/exit scale + shadow only
- Legacy `MapVotingView` / `MapVotingSession` **not** attached

## Rebuild

- Menu: `GulfRun/Play Flow/Build Map Voting Screen (Sprint 22.2)`
- Batch: `GulfRun.Editor.PlayFlowSceneBuilder.BuildMapVotingScreenBatch`
- Fallback: `_tools/gen_map_voting_scene.py`

## Constraints honored

- Scene name remains **MapVoting**
- Header / footer / SafeArea / CanvasScaler (1920×1080 match 0.5) preserved
- Main Menu and SessionManager untouched
- `DefaultNetworkPrefabs.asset` and root `Assets/Btn_*.png` left uncommitted
