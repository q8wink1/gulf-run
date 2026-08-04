# Sprint 22.5 — Premium Loading Screen UI

**Scope:** Loading Screen UI only (after Winning Map Reveal, before race). No scene load progress, networking, multiplayer sync, backend, or gameplay logic.

**Status:** Complete.

## Screen

| Area | Detail |
|------|--------|
| Scene | `Client/Assets/_Project/Scenes/LoadingScreen.unity` |
| Canvas | Overlay, CanvasScaler **1920×1080**, match **0.5** |
| SafeArea | Main Menu insets: **48 L/R**, **52 top**, **34 bottom** (`sizeDelta -96/-86`, `y -9`) |
| Background | Selected-map artwork placeholder + dim overlay (no blur shader) |
| Center | Large GulfRun **Logo** (Main Menu `Logo.png`), **Loading Race...**, spinning indicator |
| Progress | Premium bar + **0%** placeholder (`fillAmount = 0`) |
| Tips | 3 tip placeholders — **TipPrimary** active; TipSecondary/TipTertiary inactive |
| Sync | **Waiting for players...** + **4 / 4 Ready** placeholders |
| Continue | Optional Editor stub → Gameplay (no real load progress) |

## Hierarchy

```
LoadingScreenCanvas (+ LoadingScreenController)
├── Background
├── MapBlurPlaceholder
├── DimOverlay
├── SafeArea
├── CenterRoot → Logo, LoadingText, Spinner
├── ProgressRoot → Track, Fill (0%), PercentText
├── TipsPanel → TipsTitle, TipPrimary, TipSecondary (off), TipTertiary (off)
├── SyncStatusRoot → SyncStatusText, ReadyCountText
└── ContinueButton
```

## Navigation (temporary / placeholder)

| From | Control | To |
|------|---------|----|
| WinningMapReveal | **Continue** | LoadingScreen |
| LoadingScreen | **Continue** (Editor stub) | PreRaceIntro |

- `SceneManager.LoadLoadingScreen()` / `LoadingScreenSceneName`
- Legacy `Loading.unity` + `LoadLoading()` kept for `LoadingTransitionController`
- EditorBuildSettings: LoadingScreen inserted **after** WinningMapReveal

## Scripts

- `LoadingScreenController` — spinner rotate (visual), Continue → PreRaceIntro only
- WinningMapReveal Continue retargeted to `LoadLoadingScreen`

## Rebuild

- Menu: `GulfRun/Play Flow/Build Loading Screen (Sprint 22.5)`
- Batch: `GulfRun.Editor.PlayFlowSceneBuilder.BuildLoadingScreenBatch`
- Fallback (Editor lock): `_tools/gen_loading_screen_scene.py`

## Constraints honored

- UI only — no load progress calc / networking / SessionManager / sync
- Main Menu layout untouched
- `Loading.unity` not gutted
- `DefaultNetworkPrefabs.asset` and root `Assets/Btn_*.png` left uncommitted
