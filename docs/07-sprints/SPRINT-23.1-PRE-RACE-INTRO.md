# Sprint 23.1 — Pre-Race Intro UI

**Scope:** Pre-Race Intro screen UI only (after Loading Screen, before race). No countdown, player movement, networking, race logic, or sync.

**Status:** Complete.

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
| Audio | Inactive `AudioPlaceholders` → IntroMusicSource + CountdownSoundSource (`AudioSource`, no clips) |
| Continue | Optional Editor stub → Gameplay |

## Hierarchy

```
PreRaceIntroCanvas (+ PreRaceIntroController + PreRaceIntroPanAnimation)
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
├── AudioPlaceholders (inactive) → IntroMusicSource, CountdownSoundSource
└── ContinueButton
```

## Navigation (temporary / placeholder)

| From | Control | To |
|------|---------|----|
| LoadingScreen | **Continue** | PreRaceIntro |
| PreRaceIntro | **Continue** (Editor stub) | Gameplay |

- `SceneManager.LoadPreRaceIntro()` / `PreRaceIntroSceneName`
- EditorBuildSettings: PreRaceIntro inserted **after** LoadingScreen

## Scripts

- `PreRaceIntroController` — Continue → Gameplay only
- `PreRaceIntroPanAnimation` — Ping-pong lerp on `BackgroundPanRoot` (UI-only pan)

## Rebuild

- Menu: `GulfRun/Play Flow/Build Pre-Race Intro (Sprint 23.1)`
- Batch: `GulfRun.Editor.PlayFlowSceneBuilder.BuildPreRaceIntroBatch`
- Fallback (Editor lock): `_tools/gen_pre_race_intro_scene.py`

## Constraints honored

- UI only — no countdown / movement / networking / race logic
- Main Menu layout untouched
- `DefaultNetworkPrefabs.asset` and root `Assets/Btn_*.png` left uncommitted
