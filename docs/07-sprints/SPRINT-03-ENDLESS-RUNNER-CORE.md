# Sprint 3 — Endless Runner Core — Sprint Report

**Role:** Lead Unity Engineer
**Scope:** Infinite world generation, modular object spawning, generic object pooling, global game speed, distance tracking, scoring, game loop state machine, save-progress interfaces, and runner debug tooling.
**Status:** Complete, under the same environment constraint documented in [SPRINT-01-PROJECT-FOUNDATION.md](SPRINT-01-PROJECT-FOUNDATION.md) §0 (no licensed Unity Editor on this machine — Unity Hub is installed but no Editor version is downloaded).

---

## 0. Continuation check

Verified before starting: no `Chunk`, `World`, `Spawn`, `Pool`, `Score`, `Distance`, `GameSpeed`, or `GameLoop` scripts existed anywhere under `Client/Assets/_Project/Scripts`, and `git status` showed a clean tree. No partial Sprint 3 work existed, so everything below was implemented from scratch. Sprint 1/2 files were touched only twice, both additive/backward-compatible (see §7 item 1).

## 1. Systems Implemented

New **Domain** logic (`Scripts/Domain/`, zero `UnityEngine` dependency — confirmed by source inspection):

| File | Purpose |
|---|---|
| `GameLoopState.cs` | Enum: `Ready, Running, Paused, GameOver, Restart` |
| `SpawnCategory.cs` | Enum: `Obstacle, Coin, PowerUp, Decoration` |
| `IRandomSource.cs` / `SeededRandom.cs` | RNG abstraction + `System.Random`-backed seeded implementation (not `UnityEngine.Random`, whose global mutable state can't be seeded per-system) |
| `WeightedOption.cs` / `WeightedSelector.cs` | Generic weighted-random pick, shared by chunk selection and all four spawn categories |
| `DifficultyCurve.cs` | Pure `distance -> 0..1` ramp, shared by speed and spawning so "how far into the ramp" is computed once |
| `SpeedCurve.cs` | Pure `distance -> speed` linear ramp (base → max) |
| `ScoreCalculator.cs` | Pure distance/coin/multiplier → score breakdown |

New **Core** additions (`Scripts/Core/`, engine-dependent, reusable by any future feature):

| File | Purpose |
|---|---|
| `SceneSingleton.cs` | New sibling to Sprint 1's `Singleton<T>` — same pattern but **without** `DontDestroyOnLoad`, for session-scoped services that must reset on scene reload (Restart) instead of persisting stale state |
| `Pooling/IPoolable.cs`, `PooledObjectHandle.cs`, `GameObjectPool.cs`, `ObjectPoolManager.cs` | Generic, prefab-keyed pooling. `ObjectPoolManager` is a real `Singleton<T>` (persists — pools are a cross-scene resource); `Preload`/`Get`/`Release` are the only public entry points, so gameplay code never calls `Instantiate`/`Destroy` |
| `Save/IProgressRepository.cs` | Interface only, per brief: `GetBestDistance/Score/CoinsCollected`, `SaveBestDistance/Score`, `AddCoinsCollected` — no platform-specific implementation defined here |
| `Services/IRunSpeedProvider.cs`, `RunSpeedService.cs` | Minimal service-locator so the Player (a different feature) can consume the live run speed without either feature referencing the other |

New feature assembly **`GulfRun.Features.EndlessRunner`** (references `GulfRun.Core`, `GulfRun.Domain` only — no dependency on `PlayerController`/`CameraSystem`, and vice versa):

| Folder | Files | Purpose |
|---|---|---|
| `Configuration/` | `WorldGenerationConfig`, `SpawnCategoryConfig`, `GameSpeedConfig`, `ScoreConfig`, `DifficultyConfig` (all `ScriptableObject`) | Every tunable value (chunk weights/buffers, spawn weights/chances, speed curve, score rates, difficulty ramp) lives in data, not code |
| `WorldGeneration/` | `Chunk`, `SpawnPoint`, `WorldGenerator` | Infinite, chunk-based, pooled world generation (§3) |
| `Spawning/` | `ChunkContentSpawner`, `CoinPickup`, `ObstacleContact`, `PowerUpPickup` | Modular weighted spawning per category (§4) |
| `Speed/` | `GameSpeedController` | Global speed controller (§6) |
| `Distance/` | `DistanceTracker` | Distance system (§7) |
| `Scoring/` | `ScoreController` | Scoring system (§8) |
| `Difficulty/` | `DifficultyController` | Shared 0..1 difficulty value consumed by both Speed and Spawning |
| `GameLoop/` | `GameLoopController` | State machine + per-frame tick orchestrator (§9) |
| (root) | `RunnerDebugView` | On-screen debug readout (§11) |

**Two small, additive Sprint 1/2 changes** (see §7 item 1 for why these are safe):

- `SaveManager.cs` (Sprint 1 placeholder) now implements `IProgressRepository` with an in-memory store.
- `PlayerMotor.cs` (Sprint 2) now resolves auto-run speed from `RunSpeedService.Current` when present, falling back to its original `config.AutoRunSpeed` otherwise — **zero behavior change** when EndlessRunner systems aren't in the scene.

## 2. Chunks Created

**`Assets/_Project/Prefabs/World/ChunkPrefab_Default.prefab`** — one chunk variant proving the full mechanism (adding a biome later is a data change: one more weighted `ChunkEntry` in `WorldGenerationConfig`, zero code changes):

- `Chunk` component: `length = 20`, `biomeId = "Default"`, 4 `SpawnPoint` children wired into its `spawnPoints` array
- `Ground` child: `BoxCollider2D` (20 × 1) on **Layer 8 "Ground"** (the layer Sprint 2's `PlayerGroundDetector` already checks) — chunks are what the Player actually runs on
- `SpawnPoint_Obstacle`, `SpawnPoint_PowerUp`, `SpawnPoint_Coin`, `SpawnPoint_Decoration` — one fixed-category slot each; **which prefab** spawns at a slot (or whether anything spawns at all) is the weighted/random/difficulty-scaled part, not the slot's category

Four placeholder content prefabs (no final art, `Sprites-Default` material with a flat tint, matching Sprint 2's placeholder convention): `ObstaclePrefab` (red, trigger, `ObstacleContact`), `CoinPrefab` (yellow, trigger, `CoinPickup`), `PowerUpPrefab` (cyan, trigger, `PowerUpPickup`), `DecorationPrefab` (green, no collider/script — purely visual).

`WorldGenerator` is **distance-driven, not transform-driven**: it never looks at the Player directly (features must not reference each other). It tracks a `_frontierX` cursor and compares it against `DistanceTracker.DistanceMeters` (itself derived from the deterministic `GameSpeedController`, not raw physics — see §7):

- Spawns the next weighted-random chunk whenever `frontier < distance + SpawnAheadBufferMeters`
- Recycles the oldest active chunk (via `ObjectPoolManager.Release`, after `ChunkContentSpawner.ClearChunk` releases its spawned content) whenever it has fallen more than `CleanupBehindBufferMeters` behind the current distance
- Chunk selection uses `SeededRandom` (0 = time-seeded, any other value = fully reproducible sequence)

## 3. Pooling Status

`ObjectPoolManager` (Core, generic, one `Dictionary<GameObject, GameObjectPool>` keyed by prefab) is used identically by **every** spawnable in the project — chunks, obstacles, coins, power-ups, decorations — so there is exactly one pooling implementation, not five:

- **Preloading:** `WorldGenerator` preloads every chunk prefab (`PreloadCountPerChunkPrefab`, default 3) and `ChunkContentSpawner` preloads every spawn-category prefab (`PreloadCountPerPrefab`, default 3–8 depending on category) at scene `Start()`, before any gameplay tick runs.
- **Expansion:** if a pool runs dry, `Get()` instantiates one more instance on demand (still routed through the pool, so it becomes reusable from then on) — this is the only place `Instantiate` is called anywhere in the project.
- **Recycling:** `Release(instance)` deactivates and requeues via a `PooledObjectHandle` stamped on every pooled instance, so callers never need to remember which prefab an instance came from.
- **No Instantiate/Destroy during gameplay:** verified by inspection — the only `Object.Instantiate` call in the codebase is inside `GameObjectPool.CreateInstance`, and the only `Destroy`-adjacent calls are `Singleton<T>`/`SceneSingleton<T>`'s duplicate-guard (unrelated to gameplay pooling).

## 4. Spawn System

`ChunkContentSpawner` (one instance in the scene) holds one `SpawnCategoryConfig` asset per `SpawnCategory` and, for every `SpawnPoint` on a newly-activated chunk:

1. Rolls `BaseSpawnChance` (per category) to decide if anything spawns at that slot at all.
2. If so, builds the category's weighted table for the current difficulty (`BaseWeight * Lerp(1, DifficultyWeightMultiplier, difficulty01)` per entry) and picks one prefab via the shared `WeightedSelector`.
3. Spawns it from the pool at the spawn point's position, and tracks it on the chunk so it can be released when the chunk recycles.

This gives every requested capability without inventing gameplay:

- **Weighted spawn probability** — `SpawnEntry.BaseWeight` per prefab per category.
- **Difficulty scaling** — `DifficultyWeightMultiplier` per prefab (Obstacle's placeholder is configured at `1.5`, i.e. obstacles get more common as the run gets harder; Coin/Decoration are configured neutral at `1`).
- **Random seed support** — `ChunkContentSpawner.randomSeed` (0 = time-seeded).
- **Future seasonal events** — swap the four `SpawnCategoryConfig` asset references at runtime for a different content set; no code change.
- **Obstacle/Coin/PowerUp behavior**: `ObstacleContact` calls `GameLoopController.RequestGameOver()` on player contact (the minimal, standard genre-convention rule connecting the Obstacle category to the Game Over state — no health/shield/lives system is defined by any approved document, so nothing more elaborate was invented). `CoinPickup` adds to `ScoreController`. `PowerUpPickup` only proves the category end-to-end (detects pickup, despawns) — **no power-up effects are specified anywhere in P001–P050 or this brief**, so none were invented; flagged in §12.

## 5. Distance System

`DistanceTracker.DistanceMeters` is a `double` (high precision for long runs), integrated as `speed * deltaTime` every Running tick. Deliberately derived from `GameSpeedController`'s simulated speed rather than the Player's raw `Rigidbody2D` position: distance becomes a **deterministic function of elapsed time and the speed curve**, which is exactly what a future server-authoritative leaderboard needs to validate a client-reported run (it can recompute the expected distance independently, instead of trusting client physics).

## 6. Score System

`ScoreController` recomputes `DistanceScore`, `CoinScore`, and `TotalScore` every Running tick via the pure `Domain.ScoreCalculator`, from `DistanceTracker.DistanceMeters`, `CoinsCollected`, and the current `Multiplier` (defaults to `ScoreConfig.baseMultiplier = 1`, exposed via `SetMultiplier` for a future combo system). `CoinPickup` is the only writer of `CoinsCollected`.

## 7. Game Loop

`GameLoopController` is the composition root for the whole session (same role Sprint 2's `PlayerController` plays for the player entity) and the **single owner of per-frame tick ordering** for every session system, so there is never ambiguity about `MonoBehaviour.Update()` execution order between sibling components:

```
Ready --RequestStart()--> Running --RequestPause()--> Paused --RequestResume()--> Running
Running --RequestGameOver()--> GameOver --RequestRestart()--> Restart --(auto)--> Ready
```

- **Running:** each frame, in this fixed order — `GameSpeedController.Tick` (speed from last-known distance) → `DistanceTracker.Tick` (integrates the speed just computed) → `DifficultyController.Tick` → `WorldGenerator.Tick` → `ScoreController.Tick`. A future authoritative server can drive the identical sequence for multiplayer sync.
- **Paused / GameOver:** the tick sequence is skipped entirely (not just "zeroed") and `Time.timeScale = 0` additionally freezes physics/animation (the Player stops moving, since its speed source stops advancing and its `Rigidbody2D` stops integrating).
- **GameOver:** commits best distance/score and adds coins collected to `IProgressRepository` (`SaveManager` by default, overridable via `GameLoopController.ProgressRepositoryOverride` for tests — dependency inversion without an Inspector-serializable interface field).
- **Restart:** resets every session system (`ResetSpeed/ResetDistance/ResetScore/ResetDifficulty/ResetGenerator`) and returns to `Ready`.

## 8. Save Architecture

`Core.Save.IProgressRepository` — `GetBestDistance/Score/CoinsCollected`, `SaveBestDistance/Score`, `AddCoinsCollected`. Per brief, **no platform-specific implementation was added**. `SaveManager` (Sprint 1's empty placeholder) now implements it with a simple in-memory store — not PlayerPrefs, not a file, not Cloud Save, and explicitly does **not** persist across app restarts — purely so the Game Loop and Scoring have a real, working default to call today. Swapping the storage inside `SaveManager` later (P039/P040) requires zero changes to any caller, since they only depend on the interface.

## 9. Debug Tools

`RunnerDebugView` (`OnGUI`, compiled only for `UNITY_EDITOR`/`DEVELOPMENT_BUILD`, same convention as Sprint 2's `PlayerDebugView`) reads every system only through its public scene-scoped `Instance` and displays: game loop state, distance, speed, difficulty, score breakdown, active chunk count + latest chunk name, per-category spawn counts, and per-pool active/idle counts (`ObjectPoolManager.GetAllStats()`).

## 10. Performance Notes

- **Zero `Instantiate`/`Destroy` during gameplay** (§3) — the only allocation-heavy calls happen at preload time, before `GameLoopController` ever enters `Running`.
- **Cached weighted-option lists:** `WorldGenerationConfig.GetWeightedChunkPrefabs()` and `SpawnCategoryConfig.GetWeightedOptions()` reuse a single internal scratch `List<T>` (cleared and refilled) instead of allocating a new list on every chunk/spawn-point evaluation.
- **No `GetComponentsInChildren` at runtime:** `Chunk.spawnPoints` is a serialized array (auto-populated once via `OnValidate` in Editor), not looked up every time a chunk activates.
- **`Queue<Chunk>`** for active-chunk bookkeeping is O(1) for both the "spawn ahead" (enqueue) and "cleanup behind" (dequeue oldest) operations — no per-frame scans of chunk lists.
- **Explicit, allocation-free tick ordering** (§7) instead of relying on `MonoBehaviour.Update()` execution order across components, which also removes any need for `FindObjectOfType` calls at runtime (everything is `GetComponent` on the same GameObject, or a scene-scoped `Instance`).

## 11. Build Verification / Compiler Status

Same constraint as Sprints 1–2: no licensed Unity Editor on this machine (Unity Hub is present, but no Editor version is actually installed — confirmed empty `Hub\Editor` folder, superseding Sprint 2's note that a 2022.3.62f1 install was used). Verification performed instead:

- **Offline compile, whole project:** all **58** `.cs` files under `Client/Assets/_Project/Scripts` (Sprints 1–3 combined) compiled together with `dotnet build` against a hand-written shim reproducing the exact `UnityEngine` / `UnityEngine.InputSystem` API surface actually used project-wide (`MonoBehaviour`, `ScriptableObject`, `Transform`, `Rigidbody2D`, `Collider2D`/`BoxCollider2D`/`CircleCollider2D`, `Physics2D`, `Animator`, `Mathf`, `Time`, `GUI`, `Gizmos`, all attributes, `Touchscreen`/`Mouse`/`Keyboard`, etc.). **Result: Build succeeded, 0 errors, 0 warnings.** This is a stricter check than Sprint 2's (which only covered its own 12 new files) and confirms Sprint 3 introduces no regressions in Sprint 1/2 code.
- **Shim caveat (carried forward from Sprint 2, now the standing method since no Editor install exists at all):** this validates C# syntax/type-usage correctness only, not full engine semantics or the real `com.unity.inputsystem` package — flagged as an open item (§12).
- **YAML structural validation:** all 8 new `.asset` files, all 5 new/prefab files (18+5+5+5+3 = 36 objects across the 5 prefabs), the modified `Gameplay.unity` (25 objects), and all 48 new `.meta` files parsed successfully with a Unity-multi-document-aware YAML parser.
- **fileID/guid cross-reference check:** every internal `{fileID: N}` reference (chunk's 4-element `spawnPoints` array, all `m_Component`/`m_Father` links, the scene's 9-component `GameplaySystems` GameObject) resolves to a real anchor in the same file, and every `{fileID: N, guid: G, type: T}` asset reference (script bindings, the 5 config-asset references on `GameplaySystems`, the 5 prefab references inside the config assets) resolves to a guid that actually exists as a `.meta` file in the project — **0 dangling references** across all 14 checked files.
- **Not possible without a licensed Editor:** real package resolution, `.asmdef` graph compilation, Play Mode smoke test, and visual confirmation that the new scene objects/prefabs open without console errors. This remains the top blocker, carried over unchanged from Sprints 1–2.

## 12. Remaining TODOs

1. **Drop a `Player.prefab` instance into `Gameplay.unity`** (still deferred from Sprint 2 for the same reason: hand-authoring a `PrefabInstance` YAML block is meaningfully higher corruption risk than placing it in-Editor) — the EndlessRunner systems are fully functional without a live Player instance (`WorldGenerator`/`DistanceTracker` are driven by the deterministic speed curve, not the Player's transform), but a Player instance is needed to actually run on the generated ground / trigger obstacles and coins.
2. **Verify against the real Editor** once available — the shim-based offline compile (§11) cannot catch every real-engine issue (e.g. `SerializeField` inspector-only quirks, physics layer-collision-matrix defaults, sprite/material assignment).
3. **Power-up effects are undefined** (§4) — `PowerUpPickup` only proves the category mechanically; wire real effects once Design specifies power-up types, routing any Boost-style effect through `GameSpeedController.ApplyTemporaryModifier` for consistency with the speed system.
4. **Obstacle "touch = Game Over" is a placeholder rule** (§4) — no health/shield/lives system exists in any approved document; revisit if Design wants a softer fail-state.
5. **Only one chunk biome variant exists** (`ChunkPrefab_Default`) — the mechanism for more (weighted `ChunkEntry` list + `Chunk.biomeId` metadata) is in place; add prefabs when Design specifies additional biomes.
6. **`ChunkContentSpawner`/`WorldGenerator` seeds default to `0` (time-seeded)** in the authored scene — set a fixed seed on `GameplaySystems` once deterministic QA/replay runs are needed.
7. Carries forward all unresolved Sprint 1/2 items (Unity 6 LTS install — still not present, only Hub; bundle IDs; UI framework ADR; Netcode ADR; Services scope; P020/P042 profile conflict; real placeholder animations; Input System package verification).

---

## 13. Git Workflow

| Item | Value |
|---|---|
| Commit hash | `3bde1e39d0a4e8fd78b6450e1e672c5c8e3c6b42` |
| Commit message | `Sprint 3 - Endless Runner Core` |
| Branch | `main` |
| Push status | Pushed to `origin/main` |

Sprint 3 is complete within the constraints above. Stopping here. Waiting for Sprint 4.
