# Sprint 23.1 — Pre-Race Intro UI

**Scope:** Pre-Race Intro screen UI only (after Loading Screen, before race). No player movement, networking, race logic, or sync. *(Sprint 23.2 adds countdown overlay on this scene.)*

**Status:** Complete (superseded for navigation by Sprint 23.2).

## Screen

| Area | Detail |
|------|--------|
| Scene | `Client/Assets/_Project/Scenes/PreRaceIntro.unity` |
| Canvas | Overlay, CanvasScaler **1920×1080**, match **0.5** |
| SafeArea | Main Menu insets: **48 L/R**, **52 top**, **34 bottom** (`sizeDelta -96/-86`, `y -9`) |
| Background | Selected-map artwork placeholder + slow UI pan (`BackgroundPanRoot`) |
| Banner | **Get Ready** — premium GulfRun gold styling |
| Map Info | Map Name, Country, Difficulty, Race Distance (placeholders) |
| Players | 4 starting-line slots — Character silhouette, Player Name, Country Flag |
| Audio | Inactive `AudioPlaceholders` → IntroMusicSource + CountdownSoundSource (+ GoSoundSource in 23.2) |
| Continue | Inactive in 23.2 (auto countdown); optional skip-hold stub |

## Hierarchy

```
PreRaceIntroCanvas (+ PreRaceIntroController + PreRaceIntroPanAnimation [+ RaceCountdownController in 23.2])
├── BackgroundPanRoot → Background
├── DimOverlay
├── SafeArea
├── IntroBannerRoot → BannerFill, BannerText ("Get Ready")
├── MapInfoPanel → Fill, MapName, Country, Difficulty, RaceDistance
├── PlayersRoot
│   ├── PlayerSlot_01 → Fill, Character, PlayerName, CountryFlag
│   ├── PlayerSlot_02 → …
│   ├── PlayerSlot_03 → …
│   └── PlayerSlot_04 → …
├── AudioPlaceholders (inactive) → IntroMusicSource, CountdownSoundSource [, GoSoundSource]
├── CountdownOverlay / TransitionFade  ← Sprint 23.2
└── ContinueButton (inactive in 23.2)
```

## Navigation

| From | Control | To |
|------|---------|----|
| LoadingScreen | **Continue** | PreRaceIntro |
| PreRaceIntro | **Auto countdown** (Sprint 23.2) | Gameplay |

- `SceneManager.LoadPreRaceIntro()` / `PreRaceIntroSceneName`
- EditorBuildSettings: PreRaceIntro inserted **after** LoadingScreen

## Scripts

- `PreRaceIntroController` — optional Continue skips intro hold
- `PreRaceIntroPanAnimation` — Ping-pong lerp on `BackgroundPanRoot` (UI-only pan)
- `RaceCountdownController` — Sprint 23.2 countdown → Gameplay

## Rebuild

- Menu: `GulfRun/Play Flow/Build Race Countdown (Sprint 23.2)` (rebuilds this scene)
- Batch: `GulfRun.Editor.PlayFlowSceneBuilder.BuildRaceCountdownBatch`
- Fallback (Editor lock): `_tools/gen_pre_race_intro_scene.py`

## Constraints honored

- UI only — no movement / networking / race logic (countdown is visual-only in 23.2)
- Main Menu layout untouched
- `DefaultNetworkPrefabs.asset` and root `Assets/Btn_*.png` left uncommitted
