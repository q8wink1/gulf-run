# Sprint 23.8 — Race Manager Foundation

**Scope:** Central `RaceManager` for GulfRun race flow coordination. Defines states, events, system references, and speed settings. No finish logic, obstacles, coins, AI, or multiplayer sync.

**Status:** Complete.

## Architecture

| Layer | Type | Responsibility |
|-------|------|----------------|
| Domain | `RaceState` | Waiting / Countdown / Running / Finished (engine-free) |
| Features.Gameplay | `RaceManager` | Scene singleton: state machine, speed targets, refs, events |
| Features.Gameplay | `RunnerPlayerController` | Optional speed scale via `SetSpeedScale` (off by default) |
| Features.CameraSystem | `RunnerCameraFollow` | Serialized ref only this sprint |
| Features.Gameplay | `EndlessTrackGenerator` / `SpawnManager` | Serialized refs for future orchestration |
| Features.GameplayHud | `GameplayHudController` | Serialized ref for future HUD binding |

Distinct from legacy `EndlessRunner.GameLoop.GameLoopController` (older session loop). Play-flow Gameplay uses this RaceManager.

## Flow

```
Scene load
        → RaceManager.CurrentState = Waiting
        → (no auto-start)

Caller (future countdown / tools)
        → BeginCountdown()     Waiting → Countdown
        → StartRace()          Countdown|Waiting → Running + OnRaceStart
        → PauseRace()          IsPaused + OnRacePause (state stays Running)
        → ResumeRace()         clear pause + OnRaceResume
        → FinishRace()         Running → Finished + OnRaceFinish
                               (caller-driven only — no distance auto-finish)
```

## Inspector

| Field | Role |
|-------|------|
| Player Controller | `RunnerPlayerController` |
| Camera Controller | `RunnerCameraFollow` |
| Track Generator | `EndlessTrackGenerator` |
| Spawn Manager | Gameplay `SpawnManager` |
| HUD | `GameplayHudController` |
| Initial Speed | Baseline / reset target (default 12) |
| Maximum Speed | Cap for ramp / `SetRunningSpeed` (default 28) |
| Speed Increase Rate | Per-second rise while Running (default 0.35) |
| Race Distance | Reserved finish distance (default 1000) — unused |
| Apply Speed To Player | Off by default — avoids changing runner feel this sprint |
| On Race Start/Pause/Resume/Finish | Optional Inspector `UnityEvent`s |

## API surface

| API | Purpose |
|-----|---------|
| `CurrentState` / `IsPaused` / `IsRacing` | Read race phase |
| `BeginCountdown` / `StartRace` / `PauseRace` / `ResumeRace` / `FinishRace` | Explicit transitions |
| `TargetSpeed` / `CurrentSpeed` / `SetRunningSpeed` | Speed helpers (no auto-finish) |
| `ApplySpeedToPlayer` | Stub: maps CurrentSpeed → `SetSpeedScale` |
| `OnRaceStart` / `OnRacePause` / `OnRaceResume` / `OnRaceFinish` | C# `event Action` for Features |
| `StateChanged` | `Action<RaceState>` after enum transitions |
| System property accessors | Player / Camera / Track / Spawn / Hud |

## Scene

- `RaceManager` root object on Gameplay
- Serialized refs wired to RunnerPlayer, Main Camera follow, EndlessTrackGenerator, GameplaySpawnManager, GameplayHudCanvas

## Rebuild / patch

- Fallback: `_tools/patch_gameplay_sprint_23_8_race.py`
- Menu: `GulfRun/Play Flow/Validate Gameplay Runner (Sprint 23.4–23.8)`

## Constraints honored

- No obstacles / coins / finish detection / AI / multiplayer sync
- No Main Menu UI changes
- Mobile-friendly: `SceneSingleton`, no LINQ, no Update work unless Running
- `DefaultNetworkPrefabs` / `Btn_*` left uncommitted
