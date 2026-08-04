# Sprint 23.10 — Obstacle Gameplay

**Scope:** First playable obstacle loop on the 3-lane endless track. Spawns Static / Jump / Slide obstacles through `SpawnManager` at `TrackSpawnMarker` points, with pooled instances, lane placement, and placeholder hit feedback. No coins, power-ups, multiplayer, finish logic, or real gameplay penalties.

**Status:** Complete.

## Architecture

| Layer | Type | Responsibility |
|-------|------|----------------|
| Domain | `ObstacleDifficultyLevel` | Easy / Medium / Hard (filter prep only) |
| Domain | `ObstacleDifficultyLevelRules` | Maps tier → max `ObstacleData.Difficulty` |
| Features.Gameplay | `SpawnManager` | Plan markers → weighted catalog pick → `ObjectPoolManager.Get` |
| Features.Gameplay | `ObstacleCatalog` | Prefab map + `TryPickEntry` (weight + difficulty filter) |
| Features.Gameplay | `TrackSpawnMarker` | Category + lane (field or local-X inference) |
| Features.Gameplay | `PlannedSpawnSlot` | Pose + `RunnerLane` |
| Features.Gameplay | `Obstacle` | Trigger hit → `Hit` / `AnyHit` (one per pool life) |
| Features.Gameplay | `ObstacleGameplayBridge` | Hit → anim / shake / SFX / speed-reduction **events** |
| Features.Gameplay | `RaceManager` | Difficulty push + bridge / catalog refs; WarmPools on StartRace |
| Features.Gameplay | `RunnerPlayerController` | Kinematic Rigidbody so triggers register |
| Features.Gameplay | `RunnerAnimatorDriver.PrepareHit` | Optional `HitTrigger` |

## Spawn flow

```
EndlessTrackGenerator SegmentActivated
        → SpawnManager.RegisterSegment
        → plan Obstacle markers (probability / density / spacing)
        → for each Obstacle plan:
              ObstacleCatalog.TryPickEntry(difficulty)
              ObjectPoolManager.Get(prefab) @ marker pose
              Obstacle.ApplyPlannedSlot(slot, lane)

SegmentReleased
        → Release pooled obstacles for that segment
```

**Race gating:** Obstacle plans execute on segment register whenever `executeObstaclePlans` is true (default). This keeps Gameplay playable while `RaceManager` is still `Waiting` (player already auto-runs). `StartRace` only re-applies difficulty + WarmPools; a future sprint can gate on `Running`.

## Lane placement

Track segments ship three Obstacle markers (Left / Center / Right) at distinct Z. `TrackSpawnMarker.ResolveLane()` uses Inspector lane or nearest local-X lane (±`laneSpacing`, default 2.2). `Obstacle.ApplyPlannedSlot` snaps world X via `RunnerLaneMath`.

## Collision / feedback

```
Player Capsule (kinematic RB) ∩ Obstacle BoxCollider (trigger)
        → Obstacle.AnyHit
        → ObstacleGameplayBridge
              PrepareHitAnimation  (Animator HitTrigger if present)
              PrepareCameraShake   (optional light CameraShake.Shake)
              PrepareSoundEffect   (UnityEvent / C# event only)
              PrepareSpeedReduction (stub — does not change speed)
```

No damage, score loss, or forced slowdown this sprint.

## Difficulty (prepared)

| Tier | Max ObstacleData.Difficulty |
|------|-----------------------------|
| Easy | ≤ 2 |
| Medium | ≤ 3 |
| Hard | ≤ 5 |

Default session tier: **Medium**. Weights / spacing are not rebalanced yet.

## Kuwait profile

Obstacle group enabled with probability `0.7`, density `0.85`, spacing `6–18` so markers commonly produce live obstacles in play.

## Scene wiring

- `GameplaySpawnManager`: catalog, Medium difficulty, `executeObstaclePlans = true`
- `RaceManager`: catalog + `ObstacleGameplayBridge` (light shake on)
- TrackSegment_A / B: Left / Center / Right obstacle markers
- Requires existing `ObjectPoolManager`

## Rebuild / patch

- Fallback: `_tools/patch_gameplay_sprint_23_10_obstacles.py`
- Menu: `GulfRun/Play Flow/Validate Gameplay Runner (Sprint 23.4–23.10)`

## Constraints honored

- No coins / power-ups / multiplayer / finish logic
- No Main Menu UI changes
- Mobile: object pooling, no LINQ, reused lists, logs off by default
- `DefaultNetworkPrefabs` / `Btn_*` left uncommitted
