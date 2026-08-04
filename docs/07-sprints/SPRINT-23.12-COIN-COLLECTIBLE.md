# Sprint 23.12 — Coin & Collectible System

**Scope:** First playable on-track Coin / Gem collectibles. Spawned through `SpawnManager` at `TrackSpawnMarker` points with Object Pooling, Single / Line / Arc patterns, HUD counter updates, and a short collect animation. No shop economy, rewards, missions, or multiplayer sync.

**Status:** Complete.

## Architecture

| Layer | Type | Responsibility |
|-------|------|----------------|
| Domain | `CollectibleType` | Coin / Gem |
| Domain | `CollectiblePattern` | Single / Line / Arc |
| Features.Gameplay | `Collectible` | Rotate, radius trigger, collect anim, HUD credit, pool Release |
| Features.Gameplay | `CoinCollectible` / `GemCollectible` | Typed subclasses |
| Features.Gameplay | `CollectibleCatalog` | Prefab map + `WarmPools` preload |
| Features.Gameplay | `SpawnManager` | Plan Coin/Gem markers → pattern layout → `ObjectPoolManager.Get` |
| Features.GameplayHud | `GameplayHudController` | Session coin/gem counters + chip labels |

## Inspector (`Collectible` base)

| Field | Role |
|-------|------|
| Coin Value | Credit when type is Coin |
| Gem Value | Credit when type is Gem |
| Rotation Speed | Degrees/sec Y spin on `Visual` |
| Collection Radius | Sphere trigger radius + distance gate |

## Spawn flow

```
EndlessTrackGenerator SegmentActivated
        → SpawnManager.RegisterSegment
        → plan Coin / Gem markers (probability / density / spacing)
        → for each Coin plan:
              pick CollectiblePattern (Single / Line / Arc)
              ObjectPoolManager.Get(coinPrefab) × pattern
              Collectible.ApplyWorldPose
        → for each Gem plan:
              Single (unless allowGemPatterns)
              ObjectPoolManager.Get(gemPrefab)

Player Capsule ∩ Collectible SphereCollider (trigger)
        → credit GameplayHudController (coins / gems)
        → onCollectedSound UnityEvent (placeholder)
        → scale-up collect animation
        → ObjectPoolManager.Release

SegmentReleased
        → Release remaining active pooled collectibles for that segment
```

## Patterns

| Pattern | Layout |
|---------|--------|
| Single | One at marker pose / lane |
| Line | `lineCount` along +Z at marker lane (`lineSpacingZ`) |
| Arc | Left / Center / Right at marker Z with `arcHeight` parabola |

## Sample assets

- `Settings/Collectibles/CollectibleCatalog_Default`
- `Prefabs/Track/Coin_Placeholder`
- `Prefabs/Track/Gem_Placeholder`
- TrackSegment_A / B: Coin Center / Left / Right + Gem center markers

## Scene wiring

- `GameplaySpawnManager`: collectible catalog, `executeCollectiblePlans = true`, random coin patterns
- `GameplayHudController`: `coinsText` / `gemsText` refs, starts at 0
- Requires existing `ObjectPoolManager`

## Rebuild / patch

- Fallback: `_tools/patch_gameplay_sprint_23_12_collectibles.py`
- Menu: `GulfRun/Play Flow/Validate Gameplay Runner (Sprint 23.4–23.12)`

## Constraints honored

- No shop economy / rewards / missions / multiplayer sync
- No Main Menu UI changes
- Mobile: object pooling only, no LINQ, reused lists, logs off by default
- `DefaultNetworkPrefabs` / `Btn_*` left uncommitted
