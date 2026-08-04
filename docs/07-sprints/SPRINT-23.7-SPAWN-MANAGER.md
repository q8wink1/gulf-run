# Sprint 23.7 — Gameplay Spawn Manager

**Scope:** Centralized spawn architecture for the 3-lane endless track. Plans spawn slots from `TrackSpawnMarker`s when segments activate. No obstacles, coins, gems, power-ups, decorations, NPCs, or multiplayer content is instantiated.

**Status:** Complete.

## Architecture

| Layer | Type | Responsibility |
|-------|------|----------------|
| Domain | `SpawnCategory.Gem` | Appended (ordinal 6) for premium currency markers; existing marker ordinals unchanged |
| Features.Gameplay | `SpawnGroupSettings` | Per-category Inspector: probability, density, min/max spacing, enabled |
| Features.Gameplay | `SpawnProfile` | Map-specific ScriptableObject grouping independent category settings |
| Features.Gameplay | `PlannedSpawnSlot` | Dry-run pose + category + segment/marker ids (no live instance) |
| Features.Gameplay | `SpawnManager` | Scene singleton: discovers markers, evaluates groups, stores plans only |
| Features.Gameplay | `EndlessTrackGenerator` | Raises `SegmentActivated` / `SegmentReleased` for planner wiring |
| Core | `ObjectPoolManager` | Future `WarmPools` / execute path — hooks present, unused this sprint |

Distinct from `Features.Multiplayer.Spawning.SpawnManager` (player start slots on `MultiplayerSpawning`).

## Flow

```
EndlessTrackGenerator places TrackSegment
        → SegmentActivated
        → SpawnManager.RegisterSegment
        → per SpawnProfile group: density → min spacing → probability (or max-gap force)
        → PlannedSpawnSlot list (no Instantiate)

Segment recycled
        → SegmentReleased
        → SpawnManager.UnregisterSegment (drop that segment's plans)
```

Late subscribe: `ForEachActiveSegment` catch-up after `ClearAllPlans`.

## Inspector (`SpawnManager`)

| Field | Role |
|-------|------|
| Spawn Profile | Active `SpawnProfile` (swap per Gulf map) |
| Track Generator | `EndlessTrackGenerator` (auto-find fallback) |
| Random Seed | `0` = time-seeded; else deterministic plans |
| Log Plans | Optional debug; default off (mobile) |

## Inspector (`SpawnGroupSettings` per profile group)

| Field | Role |
|-------|------|
| Category | Obstacle / Coin / Gem / PowerUp / Decoration / Npc |
| Enabled | Skip entire group when false |
| Spawn Probability | Chance an eligible marker becomes a plan |
| Spawn Density | Fraction of markers considered before probability |
| Minimum Spacing | Min +Z gap from last planned slot in this group |
| Maximum Spacing | Soft max gap — forces next eligible marker through |

## API surface

| API | Purpose |
|-----|---------|
| `RegisterSegment` / `UnregisterSegment` | Plan / clear for one track piece |
| `SetProfile` / `ClearAllPlans` | Map swap + reset |
| `PlannedSlots` / `PlannedCounts` / `CopyPlannedSlots` | Read-only plan queries |
| `WarmPools` | Future `ObjectPoolManager.Preload` (no-op) |
| `TryExecutePlannedSlot` | Future pool Get (always false this sprint) |

## Map profiles

Assets under `Settings/SpawnProfiles/`:

- `SpawnProfile_Kuwait` (default on Gameplay)
- `SpawnProfile_Dubai`
- `SpawnProfile_Doha`
- `SpawnProfile_Muscat`

Each ships six independent groups (Npc disabled by default).

## Scene

- `GameplaySpawnManager` on Gameplay, wired to `EndlessTrackGenerator` + Kuwait profile
- Requires existing `ObjectPoolManager` (future execute path)

## Rebuild / patch

- Fallback: `_tools/patch_gameplay_sprint_23_7_spawn.py`
- Menu: `GulfRun/Play Flow/Validate Gameplay Runner (Sprint 23.4–23.7)`

## Constraints honored

- No gameplay object spawning (obstacles / coins / gems / power-ups / decorations / NPCs)
- No multiplayer / Main Menu changes
- Mobile-friendly: reused lists, no LINQ, logs off by default
- Object-pool compatible hooks only
- `DefaultNetworkPrefabs` / `Btn_*` left uncommitted
