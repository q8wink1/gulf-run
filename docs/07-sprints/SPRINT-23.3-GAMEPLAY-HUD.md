# Sprint 23.3 — Gameplay HUD

**Scope:** Complete in-game HUD after countdown, before gameplay systems. UI only — no player movement, coins, obstacles, scoring, networking, or race logic.

**Status:** Complete.

## Screen

| Area | Detail |
|------|--------|
| Scene | `Client/Assets/_Project/Scenes/Gameplay.unity` (additive) |
| Canvas | `GameplayHudCanvas` — Overlay, CanvasScaler **1920×1080**, match **0.5**, sort order 20 |
| SafeArea | Main Menu insets: **48 L/R**, **52 top**, **34 bottom** (`sizeDelta -96/-86`, `y -9`) |
| Top Left | Position (`1st`) + Lap/Progress (`LAP 1/3`) placeholders |
| Top Center | Distance Traveled (`125 m`) placeholder |
| Top Right | Coins + Gems premium chips |
| Pause | Upper-right → visual-only `PauseMenu` (show/hide; no `timeScale`) |
| Boost | Bottom center meter (UI fill placeholder) |
| Notifications | Mini toasts: `+10 Coins` / `Mission Completed` / `New Record` (demo fade/slide) |

## Hierarchy

```
GameplayHudCanvas (+ GameplayHudController)
└── SafeArea
    ├── PositionPanel → Fill, PositionText, LapText
    ├── DistancePanel → Fill, DistanceText
    ├── CurrencyPanel → Fill, CoinsText, GemsText
    ├── PauseButton
    ├── BoostMeter → Track, Fill, BoostLabel
    ├── NotificationRoot → Fill, NotificationText
    └── PauseMenu (inactive) → PausePanel → Title, Hint, ResumeButton
```

## Scripts

- `GulfRun.Features.GameplayHud.GameplayHudController` — Pause toggles panel; optional notification demo timer

## Legacy Race HUD

Sprint 15 OnGUI `RaceHudView` / `CountdownHudView` / `EmoteBarView` on `RunnerHUD` are **disabled** so they do not overlap the premium Canvas HUD. Other RaceHud systems remain.

## Rebuild / patch

- Menu: `GulfRun/Play Flow/Build Gameplay HUD (Sprint 23.3)` (validate)
- Fallback: `_tools/patch_gameplay_sprint_23.py --hud`

## Constraints honored

- UI only — no gameplay systems
- Main Menu layout untouched
- `DefaultNetworkPrefabs.asset` and root `Assets/Btn_*.png` left uncommitted
