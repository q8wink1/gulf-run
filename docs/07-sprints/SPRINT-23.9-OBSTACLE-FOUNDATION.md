# Sprint 23.9 — Obstacle Foundation

**Scope:** Obstacle system foundation for the 3-lane endless track. Defines base behaviour, categories, configurable data, catalog hooks, and editor gizmos. No random spawning, collision consequences, damage, coins, multiplayer, or AI.

**Status:** Complete.

## Architecture

| Layer | Type | Responsibility |
|-------|------|----------------|
| Domain | `ObstacleType` | Static / Moving / Jump / Slide (engine-free) |
| Domain | `ObstacleRequiredAction` | None / Jump / Slide / SwitchLane (authoring only) |
| Features.Gameplay | `ObstacleData` | ScriptableObject: name, size, difficulty, weight, action |
| Features.Gameplay | `Obstacle` | Abstract MonoBehaviour: collider, visual, lane, Inspector placement |
| Features.Gameplay | `StaticObstacle` / `MovingObstacle` / `JumpObstacle` / `SlideObstacle` | Category subclasses |
| Features.Gameplay | `IObstaclePlacementTarget` | Plan → instance bind contract (no Instantiate) |
| Features.Gameplay | `ObstacleCatalog` | Data → prefab map + `WarmPools` preload hooks |
| Features.Gameplay | `SpawnManager` | Catalog ref, `TryGetObstaclePrefab`, stub `TryExecuteObstacleSlot` |
| Features.Gameplay | `RaceManager` | Optional catalog ref for future orchestration |
| Features.Gameplay | `TrackSegment` | `CopyObstacleMarkers` query hook |

Distinct from legacy 2D `EndlessRunner.Spawning.ObstacleContact` (Game Over on contact).

## ObstacleData fields

| Field | Type | Notes |
|-------|------|-------|
| Display Name | string | Designer-facing label |
| Obstacle Type | `ObstacleType` | Static / Moving / Jump / Slide |
| Width | float | Footprint / gizmo X |
| Height | float | Footprint / gizmo Y |
| Difficulty | int 1–5 | Relative challenge tier |
| Spawn Weight | float | Future weighted pick (unused this sprint) |
| Required Action | `ObstacleRequiredAction` | Expected clear action (no consequences yet) |

## Inspector (`Obstacle` base)

| Field | Role |
|-------|------|
| Data | `ObstacleData` asset |
| Lane | Left / Center / Right |
| Placement Euler Angles | Local rotation |
| Placement Scale | Local scale |
| Obstacle Enabled | Enable/disable collider + visual |
| Lane Spacing / Center X | Aligns X with runner lanes (default 2.2 / 0) |
| Obstacle Collider | Cached `Collider` |
| Visual Model | Child transform (typically `Visual`) |

`MovingObstacle` also exposes future motion fields (speed / axis / range) without Update motion.

## Compatibility hooks

```
TrackSegment.CopyObstacleMarkers
        → buffer of Obstacle-category TrackSpawnMarkers (no spawn)

SpawnManager.WarmPools
        → ObstacleCatalog.WarmPools → ObjectPoolManager.Preload (optional)

SpawnManager.TryGetObstaclePrefab(data)
        → catalog lookup

SpawnManager.TryExecuteObstacleSlot(slot, data, lane)
        → always false this sprint (no Instantiate / no random)

IObstaclePlacementTarget.ApplyPlannedSlot
        → pose + lane on an existing instance (future pool Get caller)
```

`RaceManager.ObstacleCatalog` mirrors the spawn catalog for later race-flow wiring.

## Sample assets

Under `Settings/Obstacles/`:

- `ObstacleData_StaticBarrier` / `ObstacleData_LowBeam` / `ObstacleData_Curb`
- `ObstacleCatalog_Default` (wired on Gameplay SpawnManager + RaceManager)

Placeholder prefabs under `Prefabs/Track/`:

- `Obstacle_Static_Placeholder`
- `Obstacle_Slide_Placeholder`
- `Obstacle_Jump_Placeholder`

## Rebuild / patch

- Fallback: `_tools/patch_gameplay_sprint_23_9_obstacles.py`
- Menu: `GulfRun/Play Flow/Validate Gameplay Runner (Sprint 23.4–23.9)`

## Constraints honored

- No random spawning / collision damage / coins / multiplayer / AI
- No Main Menu UI changes
- Mobile-friendly: no Update on obstacles, editor-only gizmos, pool hooks, no LINQ
- `DefaultNetworkPrefabs` / `Btn_*` left uncommitted
