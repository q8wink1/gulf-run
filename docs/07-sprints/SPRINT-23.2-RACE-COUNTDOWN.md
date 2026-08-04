# Sprint 23.2 — Race Countdown

**Scope:** Visual countdown after Pre-Race Intro, then transition to Gameplay. No player movement, runner controls, camera follow, obstacles, coins, or multiplayer networking.

**Status:** Complete.

## Approach

**Option B** — countdown overlay on `PreRaceIntro` (same scene, same camera/background/players). No separate RaceCountdown scene.

## Sequence

| Step | Behavior |
|------|----------|
| 1 | LoadingScreen → PreRaceIntro |
| 2 | Brief intro hold (~1.75s) with Get Ready / map / starting-line players |
| 3 | Auto-start overlay: **3 → 2 → 1 → GO!** (scale + fade) |
| 4 | Soft full-screen `TransitionFade` (visual gameplay handoff placeholder) |
| 5 | `SceneManager.LoadGameplay()` |

Players remain on the starting line; pan animation keeps running; no movement or controls.

## Screen additions (on PreRaceIntro)

| Area | Detail |
|------|--------|
| Overlay | `CountdownOverlay` (starts inactive) → `GoGlow` + `CountdownText` |
| GO! | Gold glow pulse behind large centered text |
| Digits | Premium GulfRun gold; smooth scale-in / fade-out coroutines |
| Fade | `TransitionFade` — alpha 0→1 after GO (camera stays; visual only) |
| Audio | Inactive placeholders: `CountdownSoundSource` (beep), `GoSoundSource` (GO) — **not played** |
| Continue | Inactive (auto flow). Optional stub still wired to skip intro hold |

## Hierarchy delta

```
PreRaceIntroCanvas (+ PreRaceIntroController + PreRaceIntroPanAnimation + RaceCountdownController)
├── … (Sprint 23.1 content unchanged)
├── AudioPlaceholders (inactive)
│   ├── IntroMusicSource
│   ├── CountdownSoundSource
│   └── GoSoundSource          ← Sprint 23.2
├── CountdownOverlay (inactive)
│   ├── GoGlow
│   └── CountdownText
├── TransitionFade
└── ContinueButton (inactive)
```

## Scripts

- `RaceCountdownController` — intro hold → digit coroutines → fade → `LoadGameplay()`
- `PreRaceIntroController` — optional Continue skips hold via `SkipHoldAndStart()`

## Navigation

| From | Control | To |
|------|---------|----|
| LoadingScreen | Continue | PreRaceIntro |
| PreRaceIntro | Auto countdown | Gameplay |

## Rebuild

- Menu: `GulfRun/Play Flow/Build Race Countdown (Sprint 23.2)`
- Batch: `GulfRun.Editor.PlayFlowSceneBuilder.BuildRaceCountdownBatch`
- Fallback (Editor lock): `_tools/gen_pre_race_intro_scene.py`

## Constraints honored

- No player movement / controls / obstacles / collectibles / networking
- Main Menu layout untouched
- `DefaultNetworkPrefabs.asset` and root `Assets/Btn_*.png` left uncommitted
