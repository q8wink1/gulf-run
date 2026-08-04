# Sprint 23.6 — Endless Track System

**Scope:** Modular endless +Z track for the 3-lane runner. World generates ahead of the player and recycles segments behind via object pooling. No obstacles, coins, decorations, multiplayer, race finish, or AI.

**Status:** Complete.

## Architecture

| Layer | Type | Responsibility |
|-------|------|----------------|
| Domain | `SpawnCategory.Npc` | Shared marker vocabulary (+ Obstacle/Coin/PowerUp/Decoration) |
| Features.Gameplay | `TrackSegment` | Fixed-length modular piece: entry/exit, spawn markers, `IPoolable` |
| Features.Gameplay | `TrackSpawnMarker` | Placeholder slot per category (does not spawn content yet) |
| Features.Gameplay | `TrackSegmentSet` | ScriptableObject prefab list per map (weights for future random) |
| Features.Gameplay | `EndlessTrackGenerator` | Follows runner Z; spawn ahead / despawn behind via `ObjectPoolManager` |
| Core | `ObjectPoolManager` | Prefab-keyed pool (preload / get / release — no gameplay Instantiate/Destroy) |

Legacy 2D `Features.EndlessRunner.WorldGenerator` (+X chunks) is unchanged.

## Inspector fields (`EndlessTrackGenerator`)

| Field | Role |
|-------|------|
| Segment Length | Design length for this map set (prefab `TrackSegment.Length` is authoritative at runtime) |
| Active Segments | Target live segment count (preload + initial fill) |
| Spawn Distance | Keep frontier at least this far ahead of player Z |
| Despawn Distance | Recycle when segment exit is this far behind player Z |
| Segment Set | `TrackSegmentSet` asset (swap per Gulf map later) |
| Follow Target | Runner transform (falls back to `Player` tag) |
| Segment Parent | Pool/hierarchy parent |
| Preload Per Prefab | Warm-up count per segment prefab |

## Pooling

1. `Start` → `ObjectPoolManager.Preload` for every prefab in the set.
2. Spawn → `Get(prefab)` → place at frontier Z → enqueue.
3. Despawn → dequeue oldest when `EndZ < playerZ - despawnDistance` → `Release`.
4. Steady-state play does not Instantiate/Destroy; pool expands only if drained.

## Randomization (prepared)

- Generator **alternates** A → B → A → … today.
- `TrackSegmentSet.TrySelectWeighted` is ready for future map random selection without changing the generator core.
- Per-map: assign a different `TrackSegmentSet` on the generator (or from map catalog later).

## Scene / assets

- `EndlessTrackGenerator` on Gameplay (follows `RunnerPlayer`)
- Prefabs: `Prefabs/Track/TrackSegment_A`, `TrackSegment_B` (ground + lane marks + 5 markers)
- Config: `Settings/DefaultTrackSegmentSet.asset`
- Static `RunnerGround` disabled (segments provide the floor)
- Requires existing `ObjectPoolManager` in Gameplay

## Rebuild / patch

- Fallback: `_tools/patch_gameplay_sprint_23_6_track.py`
- Menu: `GulfRun/Play Flow/Validate Gameplay Runner (Sprint 23.4/23.5/23.6)`

## Constraints honored

- No obstacles / coins / decorations / NPC spawning / networking / race finish / AI
- Main Menu untouched
- `DefaultNetworkPrefabs` / `Btn_*` left uncommitted
