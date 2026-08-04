# Sprint 22.4 — Winning Map Reveal UI

**Scope:** Winning Map Reveal screen UI only. No winner calculation, vote counting, networking, backend, loading logic, or gameplay.

**Status:** Complete.

## Screen

| Area | Detail |
|------|--------|
| Scene | `Client/Assets/_Project/Scenes/WinningMapReveal.unity` |
| Canvas | Overlay, CanvasScaler **1920×1080**, match **0.5** |
| SafeArea | Main Menu insets: **48 L/R**, **52 top**, **34 bottom** (`sizeDelta -96/-86`, `y -9`) |
| Status | **Winning Map** + **Preparing Match...** |
| Card | Large center winner card — Kuwait City placeholder (artwork, KW flag, description) |
| Animation | UI-only scale-up / glow / dim (no winner logic) |
| Progress | **Loading... 0%** placeholder only |
| Confetti | `ConfettiPlaceholder` inactive |

## Hierarchy

```
WinningMapRevealCanvas (+ WinningMapRevealScreenController + WinningMapRevealAnimation)
├── Background
├── DimOverlay
├── SafeArea
├── StatusRoot → WinningMapLabel + PreparingText
├── WinningCardRoot → Glow, Fill, MapArtwork, MetaRow (CountryFlag/KW), MapName, Description
├── ConfettiPlaceholder (inactive) → Burst
├── LoadingProgressRoot → LoadingProgressText
└── ContinueButton
```

## Navigation (temporary / placeholder)

| From | Control | To |
|------|---------|----|
| MapVoting | **Back** | LobbyScreen |
| MapVoting | **Next** (temporary Sprint 22.4) | WinningMapReveal |
| WinningMapReveal | **Continue** | Loading |

- `SceneManager.LoadWinningMapReveal()` / `WinningMapRevealSceneName`
- EditorBuildSettings: WinningMapReveal inserted **after** MapVoting

## Scripts

- `WinningMapRevealScreenController` — Continue → Loading only
- `WinningMapRevealAnimation` — OnEnable lerp: card scale, dim overlay, golden glow, light canvas zoom
- MapVoting: `nextButton` wired to `LoadWinningMapReveal` (no vote tally)

## Rebuild

- Menu: `GulfRun/Play Flow/Build Winning Map Reveal (Sprint 22.4)`
- Batch: `GulfRun.Editor.PlayFlowSceneBuilder.BuildWinningMapRevealBatch`
- Fallback (Editor lock): `_tools/gen_winning_map_reveal_scene.py` (+ MapVoting Next via `_tools/gen_map_voting_scene.py`)

## Constraints honored

- UI only — no winner calc / networking / SessionManager / loading progress logic
- Main Menu layout untouched
- Minimal MapVoting change (temporary Next button only)
- `DefaultNetworkPrefabs.asset` and root `Assets/Btn_*.png` left uncommitted
