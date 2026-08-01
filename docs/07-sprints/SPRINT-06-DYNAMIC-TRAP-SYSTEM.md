# Sprint 6 — Dynamic Trap System — Sprint Report

**Role:** Lead Gameplay Engineer
**Scope:** Complete map-owned Dynamic Trap System — 15 distinct traps, host-authoritative randomized spawn/trigger/expiration networking, object pooling, difficulty-scaled randomization (positions/combinations/timing/progression), and debug tooling. No final art/audio assets and no networked `Player.prefab` instance (same running "no final gameplay logic without a real Editor" constraint as every prior sprint — see §12).
**Status:** Complete, under the same environment constraint documented in [SPRINT-01-PROJECT-FOUNDATION.md](SPRINT-01-PROJECT-FOUNDATION.md) §0 (no licensed Unity Editor on this machine — Unity Hub is installed but no Editor version is downloaded).

---

## 0. Continuation check

Sprints 1–5 (Project Foundation, Player Controller Foundation, Endless Runner Core, Multiplayer Foundation, Weapons/Item Boxes/Combat) are complete and were **not** rewritten. This sprint extends five existing files additively (`WeaponEffectFlags`, `PlayerStatusEffect`/`WeaponEffectResolver`, `IMatchTransport`/`LocalLoopbackTransport`, `PlayerMotor`, `PlayerStatusEffectController`, `DifficultyController` — see §9) to give the new Traps feature the seams it needs, exactly the same "extend the interface/vocabulary, never touch the implementation contract of unrelated features" pattern used in Sprints 4 and 5.

## 1. Trap System Architecture

A new, isolated **`GulfRun.Features.Traps`** assembly (references only `GulfRun.Core` + `GulfRun.Domain`, same "Features never reference other Features" rule as `Features.Weapons`/`Features.Multiplayer`) owns everything trap-related:

| Layer | Type | Responsibility |
|---|---|---|
| Domain (pure, no UnityEngine) | `TrapId` | Identity for the 15 traps. |
| Domain | `WeaponEffectFlags` (extended, +`LateralPush`) | The gameplay-effect vocabulary — **deliberately reused from Sprint 5** rather than a parallel `TrapEffectFlags`, so traps and weapons drive the exact same status-effect pipeline (see §1.1). |
| Domain | `PlayerStatusEffect` (generalized) + `EffectSourceKind` | A resolved, ready-to-apply effect now records *which feature* caused it (`Weapon`/`Trap`) instead of hard-coding a `WeaponId`, with zero behaviour change for existing weapon code. |
| Domain | `TrapSpawnEvent` / `TrapTriggerEvent` | Readonly network-message structs — a host-broadcast "trap appeared" event (no client request counterpart; traps belong to the map) and a client-reports/host-confirms trigger event, same shape as Sprint 5's `WeaponHitEvent`. |
| Domain | `TrapPositionRoll` | Pure spawn-position math: a random distance ahead of the local player, between configurable min/max — makes "positions must never be identical" true by construction. |
| Domain | `TrapDifficulty` | Pure spawn-interval/concurrency scaling from the shared 0..1 difficulty ramp — the "difficulty progression" half of Randomization. |
| Core.Services | `IDifficultyProvider` / `DifficultyService` (new) | Decoupling seam so `Features.Traps` can read the endless-runner's existing distance-based difficulty value without referencing `Features.EndlessRunner` — same pattern as `IRunSpeedProvider`/`IGameStateProvider`. |
| Configuration (ScriptableObject) | `TrapDefinition` | Per-trap tuning: id, display name, effect flags/magnitude/duration, world lifetime, spawn weight, continuous-contact/movement behaviour, and presentation hooks. |
| Configuration (ScriptableObject) | `TrapCatalogConfig` | Single registry of all 15 `TrapDefinition`s + every spawn rule (interval range, concurrency, spawn-ahead distance range, preload count) — the "no hardcoded values" source every manager reads from. |
| Authority | `TrapAuthority` (persistent `Singleton`) | The host-only spawn-timer/validator for every spawn/expire/trigger (§4). |
| Spawning | `TrapSpawnController` (`SceneSingleton`, Gameplay scene) | Client-side materializer: pools/configures/releases the actual `Trap` GameObjects purely from broadcast events. |
| Hazards | `Trap` (`IPoolable`) | The pooled trap instance behaviour (§2). |
| Effects | `TrapEffectApplicator` (`SceneSingleton`, Gameplay scene) | Resolves and applies confirmed triggers to the target's `IPlayerStatusEffectReceiver` — reuses Sprint 5's receiver/registry with zero duplicated effect-application code. |
| Debug | `TrapsDebugView` | `OnGUI` panel (§8). |

This mirrors Sprint 5's layering exactly (Domain = rules, Core.Services = cross-feature seams, Features.Traps = the feature itself, host-authoritative `IMatchTransport` = the only network path) — no new architectural pattern was invented for this sprint.

### 1.1 Why traps reuse `WeaponEffectFlags` instead of a new `TrapEffectFlags`

Every one of the 15 traps' described effects (slow, brief stun/interrupt, knockdown, jump-disabled/"trapped", traction loss, vision reduced, sideways push) is already expressible with Sprint 5's flag vocabulary plus exactly **one** new bit (`LateralPush`). Introducing a second, parallel `TrapEffectFlags` enum would have meant either (a) duplicating `PlayerStatusEffectController`'s entire `Recompute()`/active-effect logic for a second flag type, or (b) making that controller take a union of two enums — both violate "no duplicated logic." Instead, `WeaponEffectFlags` (now documented as the shared vocabulary) and a generalized `PlayerStatusEffect.SourceKind`/`SourceId` (replacing the old weapon-only `SourceWeapon` field, which nothing actually branched on) let both features drive **one** effect-application pipeline. Trap → effect mapping:

| Trap | Flags | Rationale |
|---|---|---|
| Sand Pit, Scorpion Area, Hot Sand | `Slow` | Movement-speed multiplier; Hot Sand additionally re-applies on a refresh timer while standing (`ContinuousWhileStanding`). |
| Fishing Net | `Stun` | "Trapped briefly, jump disabled" — `Stun` already fully locks movement *and* jump via `PlayerMotor.SetMovementLocked`, exactly matching the brief. |
| Angry Camel | `Knockdown` | Same strong-stun flag Royal Camel Charge uses. |
| Rolling Barrel, Falling Palm, Loose Rocks, Broken Cart, Collapsed Bridge, Goat Herd, Construction Barrier | `Stun` | All seven are "obstacle you must jump/avoid or get briefly stopped" — one flag, one code path, seven data-only `TrapDefinition` assets (see §5 for how movement/collider behaviour still differs per trap). |
| Water Barrel | `TractionLoss` | Same "slippery" flag Oil Spill uses. |
| Wind Gust | `LateralPush` (new) | See §1.2. |
| Dust Tornado | `VisionReduced` + `LateralPush` | Combines two existing flags — no new code, just a data combination, exactly the "no hardcoded values / easy expansion" pattern `WeaponEffectFlags`'s own doc comment describes. |

### 1.2 `LateralPush`: an honest limitation, not a fake mechanic

The brief describes avoidance via "jumping" *and* "choosing another lane," and Wind Gust/Dust Tornado "push players sideways." This project's runner (per `Design/GDD/P003-CORE-GAMEPLAY-DESIGN-v1.0.md` and `01-core-experience/05-camera-controls-and-feel.md`) is a **side-scrolling, single-line 2D runner**: `PlayerMotor` only ever has a forward run axis (X, `Rigidbody2D.velocity.x`) and a jump axis (Y). There is no lane-change axis anywhere in Sprints 1–5, and the GDD itself still marks "the relationship between automatic run and camera framing" as an open TODO — a lane system does not exist at the design layer, not just the code layer. Rather than fake an invisible/no-op "lane" dimension, `LateralPush` is implemented as a **real, measurable, instantaneous setback**: `PlayerMotor.ApplyLateralImpulse(magnitude)` moves the player backward along the run axis by `magnitude` meters (clamped at 0), so getting hit by wind genuinely costs hard-won progress — a real gameplay consequence, not a decorative flag. "Choosing another lane" as an avoidance method for the seven `Stun`-obstacle traps is likewise honestly limited today to "jump over it" (real, tested, skill-based) until a lane system exists; this is called out explicitly in §13 rather than asserted as done.

## 2. The 15 Traps

| # | Trap | Effect | Lifetime | Notes |
|---|---|---|---|---|
| 1 | Sand Pit | Slow (0.6×, 2.5s) | 15s | Sinks/slows on contact. |
| 2 | Fishing Net | Stun (2.0s) | 14s | "Trapped, jump disabled." |
| 3 | Angry Camel | Knockdown (2.5s) | 12s | Drifts along the track at 6 m/s (`MovesAlongTrack`) — a moving hazard, not a static one. |
| 4 | Rolling Barrel | Stun (1.5s) | 15s | Moves at 4 m/s; must be jumped or reacted to in time. |
| 5 | Falling Palm | Stun (1.8s) | 16s | Static; blocks the lane until it expires. |
| 6 | Loose Rocks | Stun (1.2s) | 15s | Static, shortest stun of the "obstacle" group (a stumble, not a stop). |
| 7 | Broken Cart | Stun (1.5s) | 15s | Static temporary obstacle. |
| 8 | Scorpion Area | Slow (0.5×, 2.0s) | 14s | Poison-flavoured slow. |
| 9 | Hot Sand | Slow (0.7×, refreshed every 0.5s) | 18s | Only trap with `ContinuousWhileStanding` — re-applies for as long as a player stands on it via `OnTriggerStay2D`, and naturally stops the instant they leave. |
| 10 | Collapsed Bridge | Stun (2.0s) | 16s | Static; failing to jump it costs the longest "obstacle" stun. |
| 11 | Goat Herd | Stun (1.5s) | 13s | Moves at 5 m/s across the track. |
| 12 | Water Barrel | TractionLoss (0.5×, 2.0s) | 15s | Slippery ground. |
| 13 | Construction Barrier | Stun (1.5s) | 17s | Static, longest-lived "obstacle" (more time to react, but blocks longer). |
| 14 | Wind Gust | LateralPush (2m setback) | 12s | Instantaneous, no duration — see §1.2. |
| 15 | Dust Tornado | VisionReduced (3.0s) + LateralPush (1.5m) | 15s | Moves slowly (2 m/s) across the map, per the brief. |

Every value above is a field on a `TrapDefinition` asset under `Settings/Traps/` — none of it is hardcoded in `Trap.cs`/`TrapAuthority.cs`. Lifetimes deliberately vary (12–18s) around the brief's "approximately 15 seconds" for character, not a fixed constant.

## 3. General Rules (map-owned, no immunity)

- Traps are spawned/tracked entirely by `TrapAuthority` (host) and materialized identically on every client — no `WeaponId`/player identity is ever attached to a spawned trap, satisfying "traps belong to the map, players do NOT own traps."
- `Trap.OnTriggerEnter2D`/`OnTriggerStay2D` never disables the trap's collider after a hit (unlike the single-use `ItemBox`) — the same instance can legitimately trigger again if a player re-enters it, and there is no per-player exclusion list, satisfying "every player can be affected equally, no immunity." (A player who happens to be holding a Sprint 5 Protection Shield will still block the next negative effect regardless of source — that is an intentional cross-system skill/item interaction, not a trap-specific immunity.)

## 4. Trap Lifetime, Randomization & Networking (host-authoritative)

Traps have **no client request** step (unlike Item Boxes) — only `TrapAuthority` ever decides to spawn one; every client (including the host's own scene) reacts only to broadcasts:

```
Host:   TrapAuthority.Update() [only runs when IsHost, and only while GameLoopState.Running]
          - ExpireDueTraps(): any active instance whose ExpireAtSeconds has passed
              → IMatchTransport.BroadcastTrapExpired(instanceId)
          - spawn timer elapses (TrapDifficulty.ResolveSpawnIntervalSeconds, scaled by
            IDifficultyProvider.Current01 — shrinks from MaxSpawnIntervalSeconds at race
            start toward MinSpawnIntervalSeconds as difficulty ramps)
              → if active count < TrapDifficulty.ResolveMaxConcurrent(...): 
                  WeightedSelector.TrySelect(catalog.GetWeightedOptions(), random, out TrapId)
                  TrapPositionRoll.NextPosition(random, localPlayerX, minAhead, maxAhead, groundY)
                  → IMatchTransport.BroadcastTrapSpawned(TrapSpawnEvent)  (broadcast to all)

Every client (incl. host): TrapSpawnController.HandleTrapSpawned
          → ObjectPoolManager.Get(definition.Prefab, position, ...) ; Trap.Configure(instanceId, trapId, definition)
        TrapSpawnController.HandleTrapExpired(instanceId)
          → ObjectPoolManager.Release(instance)   // back to the pool, never Destroy

Client: Trap.OnTriggerEnter2D / OnTriggerStay2D (Hot Sand only, throttled)
          → IMatchTransport.ReportTrapTrigger(TrapTriggerEvent)
Host:   TrapAuthority.HandleTrapTriggerReported
          → is this TrapInstanceId still in the active dictionary (not already expired)?
          → IMatchTransport.ConfirmTrapTrigger(...)   (broadcast; late/invalid reports are silently dropped)
Client: TrapEffectApplicator.HandleTrapTriggerConfirmed
          → PlayerStatusEffectRegistry.TryGet(target) → new PlayerStatusEffect(definition.EffectFlags, ...) → receiver.TryApplyEffect(effect)
          → plays trigger feedback (sound + pooled particle)
```

This satisfies every Networking requirement literally: **spawning** is synchronized (one host decision, broadcast to all), **activation** is implicit in that same broadcast (a spawned trap is immediately live), and **expiration** is synchronized (`BroadcastTrapExpired`, driven only by the host's authoritative timer — a client's own `Update()` never expires anything locally). **The server validates all trap events**: the host is the only place a `TrapId` is rolled, a position is rolled, and a reported trigger is checked against the still-active dictionary before being confirmed (a hit reported after `BroadcastTrapExpired` has already fired for that instance is dropped, not confirmed).

**Randomization**, all configurable via `TrapCatalogConfig` (no hardcoded values):
- **Spawn positions** — `TrapPositionRoll`, a fresh `IRandomSource` roll every spawn; "must never be identical every match" is true by construction, not by chance.
- **Trap combinations** — `WeightedSelector.TrySelect(catalog.GetWeightedOptions(), ...)`, the exact same generic weighted-pick algorithm Sprint 3 (chunk prefabs) and Sprint 5 (standard weapons) already use — no third copy of this algorithm was written.
- **Timing** — `MinSpawnIntervalSeconds`/`MaxSpawnIntervalSeconds` define the randomized-by-difficulty window between spawn attempts.
- **Difficulty progression** — reuses the endless-runner's own existing distance-based `DifficultyController.Current01` (now published via the new `IDifficultyProvider`/`DifficultyService` seam) rather than inventing a second, disconnected difficulty curve; both spawn interval and max concurrent trap count scale off that one shared signal.

## 5. Player Interaction (avoidance) & Object Pooling

- **Jumping** — the seven `Stun`-flagged "obstacle" traps (Rolling Barrel, Falling Palm, Loose Rocks, Broken Cart, Collapsed Bridge, Goat Herd, Construction Barrier) and Fishing Net are placed as trigger colliders a player can jump clear of using Sprint 2's existing (real, tested) double-jump — timing/reaction genuinely determines whether the trigger fires.
- **Perfect timing / good reactions** — moving hazards (Angry Camel, Rolling Barrel, Goat Herd, Dust Tornado) drift along the track for their lifetime (`TrapDefinition.MovesAlongTrack`/`MoveSpeedMetersPerSecond`, applied client-side in `Trap.Update()` — deterministic from the same synced spawn position+timestamp, no extra network traffic needed), so reaching a static or moving trap's window at the wrong moment is what actually causes a hit, not an unavoidable roll.
- **Choosing another lane** — honestly scoped to today's single-line runner; see §1.2 and §13 item 1.
- **Object Pooling** — every trap is a single shared `TrapPrefab.prefab` (`Trap` + `IPoolable`), preloaded (`ObjectPoolManager.Preload`, `PreloadCountPerPrefab`) and drawn/returned exclusively through `ObjectPoolManager.Get`/`Release`. **Zero `Instantiate`/`Destroy` calls exist anywhere in `Features.Traps`.** All 15 `TrapDefinition`s reference the one pooled prefab (distinguished today only by a `debugTint` `SpriteRenderer.color`, applied in `Trap.Configure`) — the brief requires Object Pooling and per-trap *mechanics*, not per-trap unique art (unlike Sprint 5's weapons, which explicitly required unique icon/sound/particle/animation per weapon); this keeps the pool trivial (one GameObjectPool) while still giving each trap type a distinct, correct, data-driven identity.

## 6. Performance

- **Mobile-optimized / low memory / low CPU:** one shared pooled prefab (not 15), a single `Dictionary<int, ActiveTrap>`/`Dictionary<int, ActiveTrapView>` per host/client (O(1) lookups, no per-frame allocation in the hot path — `WeightedSelector`/`TrapCatalogConfig.GetWeightedOptions()` reuse a cached scratch list exactly like `WeaponCatalogConfig` does), and `TrapAuthority.Update()` only runs its spawn/expire logic on the host and only while `GameLoopState.Running` (a no-op guard clause on every other client/scene state).
- **60 FPS support:** no physics or gameplay logic in this sprint runs anything more expensive than a `Dictionary` scan bounded by `MaxConcurrentTraps` (≤ 4 with the default difficulty bonus) plus one weighted-selector pass per spawn attempt (every 4–9s, not every frame) — no per-frame cost scales with trap count beyond the already-existing `Trap.Update()` translate for moving hazards.

## 7. Supporting Extensions (additive, no rewrites)

| File | Change | Why |
|---|---|---|
| `Domain/WeaponEffectFlags.cs` | + `LateralPush` bit; doc comment now documents the shared weapon/trap vocabulary. | See §1.1 — no existing weapon's serialized `effectFlags` int value changed (new bit appended, none renumbered). |
| `Domain/PlayerStatusEffect.cs` + `WeaponEffectResolver.cs` | `SourceWeapon: WeaponId` → `SourceKind: EffectSourceKind` + `SourceId: int` (new `EffectSourceKind` enum). | The old field was write-only (nothing ever branched on it) — generalizing it was a zero-risk, purely additive change enabling `TrapEffectApplicator` to construct the same struct Weapons already does. |
| `Features/PlayerController/PlayerMotor.cs` | + `ApplyLateralImpulse(float)`. | The one new movement hook Wind Gust/Dust Tornado need (§1.2). |
| `Features/PlayerController/PlayerStatusEffectController.cs` | `TryApplyEffect` special-cases `LateralPush` as a one-shot call (never added to the duration-based `_active` list). | `LateralPush` has no "recompute every frame" meaning, unlike every other flag. |
| `Core/Networking/IMatchTransport.cs` + `LocalLoopbackTransport.cs` | + 4 trap spawn/expire/trigger events & methods. | The single network seam extension described in §4 — no other multiplayer contract changed. |
| `Core/Services/IDifficultyProvider.cs` + `DifficultyService.cs` (new) | New Core.Services seam. | Lets `TrapAuthority` read the endless-runner's difficulty ramp without a cross-feature reference. |
| `Features/EndlessRunner/Difficulty/DifficultyController.cs` | Implements `IDifficultyProvider`; publishes/unpublishes itself via `DifficultyService.Current` in `OnEnable`/`OnDisable`. | Same publish pattern `GameLoopController`/`IGameStateProvider` and `GameSpeedController`/`IRunSpeedProvider` already use. |
| `.compile_check/Shims/UnityEngineShim.cs` | + `SpriteRenderer` stub; `Vector2` gained `*(float)`/`==`/`!=`/`GetHashCode` operators. | `Trap.cs` needed both for its tint-on-configure and movement-vector-is-zero check; real `UnityEngine.Vector2`/`SpriteRenderer` already have these — the shim was simply incomplete. |

## 8. Debug Tools

`TrapsDebugView` (`OnGUI`, Editor/dev-build only, `panelX: 1360` — clear of `MultiplayerDebugView` (`460`) and `WeaponsDebugView` (`910`), so all three plus `RunnerDebugView`/`PlayerDebugView` can be shown simultaneously):

- **Current trap count** — live `TrapSpawnController.ActiveTraps.Count` (populated identically on every client from broadcasts, not host-only bookkeeping).
- **Spawn positions** — each active instance's live world position (`instance.transform.position`), updating in real time for moving hazards.
- **Lifetime timer** — `ExpireAtSeconds - Time.timeAsDouble` per active instance, counting down to 0.
- **Pool usage** — `ObjectPoolManager.GetAllStats()` filtered to the trap pool (active/inactive counts).
- **Trap IDs** — every catalog entry plus its effective spawn-weight percentage (`WeightedSelector`'s own inputs, so the displayed rate is exactly what will be rolled).

## 9. Scene & Asset Wiring

- **`Boot.unity`** — new `TrapSystems` GameObject (root order 3) with `TrapAuthority`, pointed at `TrapCatalogConfig.asset`; placed alongside (not replacing) Sprint 4/5's `MultiplayerSystems`/`WeaponSystems`.
- **`Gameplay.unity`** — new `TrapSystems` GameObject with `TrapSpawnController` + `TrapEffectApplicator` + `TrapsDebugView`, all pointed at the same catalog asset.
- **New assets:** `Settings/Traps/TrapDefinition_{SandPit,FishingNet,AngryCamel,RollingBarrel,FallingPalm,LooseRocks,BrokenCart,ScorpionArea,HotSand,CollapsedBridge,GoatHerd,WaterBarrel,ConstructionBarrier,WindGust,DustTornado}.asset` (15), `Settings/Traps/TrapCatalogConfig.asset`, `Prefabs/World/TrapPrefab.prefab`.
- Deliberately **not** wired into `ChunkPrefab_Default.prefab`'s `SpawnPoint` system — traps have a fundamentally different lifecycle (host-timed ~15s appear/expire loop) from Sprint 3/5's "baked into the chunk at generation time" Obstacle/Coin/PowerUp/Decoration/ItemBox categories, so §4's independent `TrapAuthority` timer is the correct model, not a sixth `SpawnCategory`.
- As with every prior sprint, a networked `Player.prefab` instance is still **not** placed in `Gameplay.unity` — `Trap.ReportTrigger` and `PlayerStatusEffectController` are both implemented and ready the moment that instance exists (see §13 item 2, carried forward).

## 10. Code Quality

- **SOLID:** `TrapAuthority` (validation/decision) is separate from `TrapSpawnController` (client-side materialization/presentation) is separate from `TrapEffectApplicator` (effect application) — three responsibilities, three classes, same split Sprint 5 used for Weapons. Dependency Inversion: `IDifficultyProvider`/`IPlayerStatusEffectReceiver` mean Traps never references EndlessRunner or PlayerController directly. Open/Closed: adding trap #16 is "add a `TrapDefinition` asset + register it in the catalog," never a code change to any manager.
- **No hardcoded values:** every tunable (effect flags/magnitude/duration, lifetime, spawn weight, continuous-refresh interval, move speed, spawn interval range, concurrency + its difficulty bonus, spawn-ahead distance range, preload count) is a serialized field on a ScriptableObject.
- **No duplicated logic:** trap selection reuses `WeightedSelector` (no third weighted-pick algorithm); trap effects reuse `WeaponEffectFlags`/`IPlayerStatusEffectReceiver` (no parallel effect system); trap spawn-rate scaling reuses the existing `DifficultyController` signal (no second difficulty curve).
- **Modular / easy to extend:** every trap is one data asset; the shared `Trap.cs` behaviour is driven entirely by its `TrapDefinition` (flags, magnitude, duration, lifetime, continuous/movement toggles) rather than per-trap code branches.

## 11. Build Verification / Compiler Status

- **Offline compile:** all **130** project `.cs` files (up from 115 after Sprint 5) recompiled together via `dotnet build .compile_check/CompileCheck.csproj` against `.compile_check/Shims/UnityEngineShim.cs`, extended this sprint with a `SpriteRenderer` stub and missing `Vector2` operators (`*(float)`, `==`, `!=`, `GetHashCode`) — real gaps in the shim, not workarounds for anything wrong with the actual game code. **Result: Build succeeded, 0 errors, 0 warnings.**
- **YAML/GUID validation:** `.compile_check/validate_yaml_refs.py` checked all **171** project `.meta` GUIDs for duplicates (**none found**, up from 138) and cross-referenced every `{fileID, guid}` in `Boot.unity` (22 documents, up from 19), `Gameplay.unity` (40 documents, up from 35), and `NetworkSyncConfig.asset` — **0 real dangling references** (the two flagged hits are the same pre-existing Unity built-in `RenderSettings` skybox/spot-cookie references documented as expected false positives since Sprint 4). `.compile_check/check_fileid_refs.py` was additionally run explicitly against every new/changed asset, prefab, and scene file from this sprint (`TrapCatalogConfig.asset`, all 15 `TrapDefinition` assets, `TrapPrefab.prefab`, `Boot.unity`, `Gameplay.unity`): **"ALL 5 FILES: fileID/guid references OK (171 known guids in project)."**

## 12. Remaining TODOs

1. **No lane-change axis exists in the runner** (design-layer gap, not just code — see §1.2) — "choosing another lane" as a trap-avoidance method, and `LateralPush` as a true sideways displacement, are both honestly scoped today: obstacle traps are avoided by jumping only, and `LateralPush` is a real forward-progress setback instead of a lane nudge. Revisit `PlayerMotor.ApplyLateralImpulse` and the seven `Stun`-obstacle traps' avoidance options the moment a real lane system is designed and built.
2. **No networked `Player.prefab` instance exists in any scene** (carried forward from Sprints 2–5) — `Trap.ReportTrigger` always reports the local connection id (same limitation `Features.Weapons.ItemBoxes.ItemBox` already inherits, since only the local player has a physically simulated collider today); until a real remote avatar exists, only the local player can ever trigger a trap.
3. **Trap positions are rolled relative to the single local player's position** (`LocalPlayerStateService.Current.Position`), not per-player — because no per-participant server-tracked position exists yet (item 2). Once it does, `TrapAuthority.TrySpawnTrap` should roll ahead of the pack leader (or per-player) rather than "the" local player.
4. **`TrapAuthority.ResetForNewMatch()` is not yet called by any match-lifecycle hook** — same carried-forward gap as `WeaponAuthority.ResetForNewMatch()` (Sprint 5 §13 item 5); `Features.Multiplayer.Match.MatchManager`'s new-match transition is the natural future caller for both.
5. **No final art/audio assets** — every `TrapDefinition`'s `icon`/`appearSound`/`triggerSound`/`impactParticlePrefab` fields exist and are wired end-to-end, but are currently unassigned (`{fileID: 0}`) placeholders; `TrapPrefab`'s visual is a flat, per-type-tinted `SpriteRenderer` rectangle with no sprite assigned. First real Editor open + an art pass is required to fill these in (same status as every prior sprint's placeholders).
6. Carries forward all unresolved Sprint 1–5 items (Unity 6 LTS install still only Hub; ADR-0001 still Proposed, not Accepted; no Lobby/Waiting Room UI scene; ping always 0 under the loopback transport; bundle IDs; UI framework ADR; no real "use weapon" input binding).

## 13. Git Workflow

| Item | Value |
|---|---|
| Commit hash | *(filled in after commit — see below)* |
| Commit message | `Sprint 6 - Dynamic Trap System` |
| Branch | `main` |
| Push status | *(filled in after push — see below)* |

Sprint 6 is complete within the constraints above. Stopping here. Waiting for Sprint 7.
