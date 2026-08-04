# Sprint 23.11 — Game Rules Foundation

**Scope:** Core Game Rules architecture for how a race starts, progresses, and ends. Defines configurable settings, win/lose vocabulary, and an event hub. No finish detection, elimination gameplay, distance checks, or Main Menu changes.

**Status:** Complete.

## Architecture

| Layer | Type | Responsibility |
|-------|------|----------------|
| Domain | `WinCondition` | FinishLine / HighestProgress / LastPlayerStanding (future) |
| Domain | `LoseCondition` | Disconnect / Elimination / Timeout |
| Features.Gameplay | `GameRulesConfig` | Optional ScriptableObject Inspector presets |
| Features.Gameplay | `GameRulesManager` | Scene singleton: settings + event hub + report stubs |
| Features.Gameplay | `RaceManager` | Flow coordinator — bridged for start/pause/finish events only |

`GameRulesManager` is config + events. It does **not** own race state transitions or finish logic (`RaceManager` remains the flow owner).

## Settings

| Setting | Default | Notes |
|---------|---------|-------|
| Maximum Players | 4 | Session roster cap |
| Race Distance | 1000 | Reserved finish distance — unused by auto-finish |
| Time Limit (seconds) | 0 | `0` = no limit; gates Timeout lose condition |
| Elimination Enabled | false | Soft-gates Elimination reports |
| Respawn Enabled | false | Flag only — no respawn gameplay |
| Win Condition | FinishLine | FinishLine / HighestProgress / LastPlayerStanding |

Optional asset: `Settings/GameRules/GameRulesConfig_Default.asset` (applied on Awake when assigned).

## Win / Lose

**Win conditions (enum only this sprint):**

- `FinishLine` — reach configured distance / finish line
- `HighestProgress` — best progress when race ends
- `LastPlayerStanding` — reserved for future elimination modes

**Lose conditions:**

- `Disconnect`
- `Elimination` (requires Elimination Enabled to report)
- `Timeout` (active when Time Limit > 0)

`EvaluateWin` always returns `false` (no distance / standings evaluation).

## Events

| Event | Signature | Raised by |
|-------|-----------|-----------|
| `RaceStarted` | `Action` | RaceManager `OnRaceStart` bridge, or `NotifyRaceStarted` |
| `RacePaused` | `Action` | RaceManager `OnRacePause` bridge, or `NotifyRacePaused` |
| `RaceFinished` | `Action` | RaceManager `OnRaceFinish` bridge, or `NotifyRaceFinished` |
| `PlayerFinished` | `Action<string>` | `ReportPlayerFinished(playerId)` stub |
| `PlayerEliminated` | `Action<string, LoseCondition>` | `ReportPlayerEliminated` / `ReportLose` stubs |

Optional Inspector `UnityEvent` mirrors for RaceStarted / RacePaused / RaceFinished. Player events are C# only (string + lose reason).

## API surface

| API | Purpose |
|-----|---------|
| Settings properties | `MaximumPlayers`, `RaceDistance`, `TimeLimitSeconds`, `HasTimeLimit`, flags, `WinCondition` |
| `SetRulesConfig` / `SetRaceManager` | Runtime wiring |
| `EvaluateWin(out winnerPlayerId)` | Stub — always false |
| `ReportPlayerFinished` | Fire `PlayerFinished` only |
| `ReportPlayerEliminated` / `ReportLose` | Fire `PlayerEliminated` (soft elimination gate) |
| `NotifyRaceStarted` / `Paused` / `Finished` | Direct hub notifies without RaceManager calls |
| `IsLoseConditionActive` | Config gate helper |

## Scene

- `GameRulesManager` root object on Gameplay
- `raceManager` → existing `RaceManager`
- `rulesConfig` → `GameRulesConfig_Default`

## Rebuild / patch

- Fallback: `_tools/patch_gameplay_sprint_23_11_game_rules.py`
- Menu: `GulfRun/Play Flow/Validate Gameplay Runner (Sprint 23.4–23.11)`

## Constraints honored

- No finish / elimination / timeout / respawn gameplay logic
- No Main Menu UI changes
- Mobile-friendly: `SceneSingleton`, no Update, no LINQ, optional SO
- `DefaultNetworkPrefabs` / `Btn_*` left uncommitted
