# Sprint 5 — Weapons, Item Boxes & Gulf Combat System — Sprint Report

**Role:** Lead Gameplay Engineer
**Scope:** Complete weapon architecture — Mystery Item Boxes (pooled, respawning, animated), 2-slot Weapon Inventory (no replacement), 9 Standard + 1 Legendary weapon, 5 targeting types, host-authoritative pickup/use/hit networking, status-effect application on the player, and debug tooling. No final art/audio/animation assets and no networked `Player.prefab` instance (per the running "no final gameplay logic without a real Editor" constraint — see §12).
**Status:** Complete, under the same environment constraint documented in [SPRINT-01-PROJECT-FOUNDATION.md](SPRINT-01-PROJECT-FOUNDATION.md) §0 (no licensed Unity Editor on this machine — Unity Hub is installed but no Editor version is downloaded).

---

## 0. Continuation check

Sprints 1–4 (Project Foundation, Player Controller Foundation, Endless Runner Core, Multiplayer Foundation) are complete and were **not** rewritten. This sprint only extends four existing files additively (`AudioManager`, `IMatchTransport`/`LocalLoopbackTransport`, `SpawnCategory`, `ChunkContentSpawner`, `PlayerMotor` — see §9) to give the new Weapons feature the seams it needs, exactly the same "extend the interface, never touch the implementation contract of unrelated features" pattern used in Sprint 4.

## 1. Weapon System Architecture

A new, isolated **`GulfRun.Features.Weapons`** assembly (references only `GulfRun.Core` + `GulfRun.Domain`, same "Features never reference other Features" rule as `Features.Multiplayer`) owns everything weapon-related:

| Layer | Type | Responsibility |
|---|---|---|
| Domain (pure, no UnityEngine) | `WeaponId`, `WeaponRarity`, `WeaponTargetingType`, `WeaponEffectFlags` (`[Flags]`) | Identity, rarity, targeting, and combinable gameplay-effect vocabulary — shared by client and any future server process. |
| Domain | `PlayerStatusEffect`, `WeaponEffectResolver` | A resolved, ready-to-apply effect (flags/duration/magnitude/source) and the pure function that resolves it, including the Falcon Feather "marked target" bonus. |
| Domain | `WeaponInventory` | The 2-slot, no-replacement inventory rule itself (`TryCollect`/`TryConsume`) — fully unit-testable with zero engine dependency. |
| Domain | `WeaponSpawnRoll` | Pure legendary-vs-standard roll, given an `IRandomSource`, the configured chance, and "already granted this match" — same determinism discipline as Sprint 3's `SeededRandom`. |
| Domain | `WeaponPickupRequest`/`WeaponPickupEvent`, `WeaponUseRequest`, `WeaponHitEvent` | Readonly network-message structs (client request ⇄ host-confirmed), same shape as Sprint 4's `NetworkPlayerSnapshot`. |
| Configuration (ScriptableObject) | `WeaponDefinition` | Per-weapon tuning: id, display name, rarity, targeting type, effect flags, magnitude, duration, standard spawn weight, cooldown, and presentation hooks (icon/sound/particle/animation trigger). |
| Configuration (ScriptableObject) | `WeaponCatalogConfig` | Single registry of all 10 `WeaponDefinition`s + `LegendarySpawnChance01`; the only "no hardcoded values" source every manager reads from. |
| Inventory | `WeaponInventoryManager` (persistent `Singleton`) | Per-connection inventory bookkeeping purely from `IMatchTransport` confirmed events, plus local "use slot N" → request + cooldown guard. |
| Authority | `WeaponAuthority` (persistent `Singleton`) | The host-only validator/roller for every pickup/use/hit (§4). |
| Effects | `WeaponEffectApplicator` (`SceneSingleton`, Gameplay scene) | Resolves and applies confirmed hits to the target's `IPlayerStatusEffectReceiver`, and plays impact feedback (particle/sound). |
| Player | `IPlayerStatusEffectReceiver` / `PlayerStatusEffectRegistry` (Core.Services) | The decoupling seam so `Features.Weapons` can affect a player without referencing `Features.PlayerController`. |
| Player | `PlayerStatusEffectController` (`Features.PlayerController`) | Implements the receiver; drives `PlayerMotor`'s new external speed multiplier / movement-lock hooks (§9) to actually slow, boost, pause, or stun. |
| World | `ItemBox` (`IPoolable`) | Item Box prefab behaviour (§2). |
| Debug | `WeaponsDebugView` | `OnGUI` panel (§8). |

This mirrors Sprint 4's layering exactly (Domain = rules, Core.Services = cross-feature seam, Features.Weapons = the feature itself, host-authoritative `IMatchTransport` = the only network path) — no new architectural pattern was invented for this sprint.

## 2. Item Boxes

| Requirement | Implementation |
|---|---|
| Appear randomly throughout the race | New `SpawnCategory.ItemBox` reuses Sprint 3's existing chunk-based procedural spawner (`ChunkContentSpawner`/`SpawnCategoryConfig`) exactly like Obstacle/Coin/PowerUp/Decoration — no parallel spawn system was written. |
| Spawn locations change every match | Inherited for free from the existing per-chunk `SeededRandom` roll in `ChunkContentSpawner` — every match reshuffles which `SpawnPoint`s actually receive a box. |
| Respawn using configurable rules | `SpawnCategoryConfig_ItemBox.asset` (`baseSpawnChance: 0.12`, `preloadCountPerPrefab: 3`) — same config type as the other three categories, so "configurable" means "designer edits the asset," not "edit code." |
| Object Pooling | `ItemBox` implements `Core.Pooling.IPoolable` (`OnSpawned`/`OnDespawned`) and is drawn from `ObjectPoolManager` by the existing spawner path — zero `Instantiate`/`Destroy` for boxes. |
| Opening animation | `ItemBox.OnTriggerEnter2D` → plays `openSound` (`AudioManager.PlayOneShot`, new — §9) and, if an `Animator` is assigned, triggers `openingAnimatorTrigger` (default `"Open"`), then despawns back to the pool after `openingAnimationSeconds` via a coroutine — the box's collider is disabled immediately on first pickup so a second overlapping player can't double-claim it while the animation plays. |
| Prefab | `ItemBoxPrefab.prefab` — `SpriteRenderer` (gold placeholder square) + trigger `CircleCollider2D` + `ItemBox`, wired into `ChunkPrefab_Default`'s new `SpawnPoint_ItemBox` child (`category: 4`) at `x: 10`. |

Picking up a box **requests** a pickup (`IMatchTransport.RequestWeaponPickup`); it does not locally decide anything — see §4 for why.

## 3. Weapon Inventory (2 slots, no replacement)

`Domain.WeaponInventory`: fixed `WeaponId?[2]` array.

- `TryCollect(weapon)` fails (returns `false`, no slot written) if both slots are occupied — **the box is simply lost, nothing is replaced**, exactly as specified.
- `TryConsume(weapon)` clears the one slot holding that weapon and returns `true`, or `false` if the player doesn't have it.
- `WeaponInventoryManager` is the single source of truth per connection ID, updated **only** by confirmed network events (`WeaponPickupConfirmed`/`WeaponUseConfirmed`) — never optimistically on the requesting client, so every player's view of every inventory (their own and others') is derived the same way.
- `WeaponsDebugView` exposes "Simulate Item Box Pickup" and reads back `IsFull`/slot contents live, so the "inventory full → box lost" rule is directly observable without a scene UI.

## 4. Weapon Usage & Networking (host-authoritative)

Every weapon action is **request (client) → validate (host) → confirm (broadcast to all)** — the same authority pattern Sprint 4 established for match state:

```
Client:  ItemBox trigger  →  IMatchTransport.RequestWeaponPickup(WeaponPickupRequest)
Host:    WeaponAuthority.HandlePickupRequested
           → is inventory already full for that connection?  → Granted = false (box lost, no weapon)
           → else roll WeaponSpawnRoll (legendary chance, "already granted this match" gate) → Granted = true
         IMatchTransport.ConfirmWeaponPickup(WeaponPickupEvent)  (broadcast)
Client:  WeaponInventoryManager.HandlePickupConfirmed  →  inventory.TryCollect(...)  →  InventoryChanged

Client:  "Use Slot N" (debug button today, real input binding later)
           → WeaponInventoryManager.TryUseLocalSlot: cooldown guard, resolves a default target
             (self for Defensive/SelfBuff, first other participant for NearestOpponent,
              broadcast-resolved for AreaEffect/Forward — §5)
           → IMatchTransport.RequestWeaponUse(WeaponUseRequest)
Host:    WeaponAuthority.HandleUseRequested
           → does the user actually own that weapon? (re-checked host-side, not trusted from the client)
           → ResolveHits: for AreaEffect/Forward, iterate every other participant and roll a hit per weapon rule;
             for single-target types, the requested target is the hit
           → IMatchTransport.ConfirmWeaponUse(...)  (clears the slot on every client)
           → IMatchTransport.ConfirmWeaponHit(WeaponHitEvent)  per affected target (broadcast)
Client:  WeaponEffectApplicator.HandleWeaponHitConfirmed
           → PlayerStatusEffectRegistry.TryGet(target) → WeaponEffectResolver.Resolve(...) → receiver.TryApplyEffect(effect)
           → clears the target's Mark (Falcon Feather bonus is one-shot) and plays impact feedback
```

The server validates **pickup** (full-inventory / already-full check + the actual legendary/standard roll), **usage** (ownership re-check, cooldown is a client-side spam guard only — not trusted), **hit detection** (`ResolveHits` runs exclusively on the host), and **removal** (the slot is only cleared on `WeaponUseConfirmed`, never optimistically) — directly satisfying "The server validates: pickup / usage / hit detection / removal." As with Sprint 4's `LocalLoopbackTransport`, this all runs today in-process (no real sockets) but through the same `IMatchTransport` seam a real transport will use unmodified.

`IMatchTransport` gained `Participants` (so `WeaponAuthority`/`WeaponInventoryManager` can enumerate other connections for `NearestOpponent`/`AreaEffect` without depending on `Features.Multiplayer.Lobby` directly) and the six weapon events/methods listed above; `LocalLoopbackTransport` implements all of them as plain event re-raises, identical in spirit to its existing match-state broadcasts.

## 5. Weapon Rarity, Roster & Targeting

**10 weapons total, exactly as specified** — 9 `WeaponRarity.Standard` + 1 `WeaponRarity.Legendary` — each a `WeaponDefinition` asset under `Settings/Weapons/`, registered in `WeaponCatalogConfig.asset`:

| # | Weapon | Targeting | Effect(s) | Notes |
|---|---|---|---|---|
| 1 | Sand Storm | Area Effect | Slow + Vision Reduced | Hits every opponent in range at once. |
| 2 | Dust Cloud | Area Effect | Vision Reduced | Longer, weaker-than-Sand-Storm cloud (no slow). |
| 3 | Arabic Coffee | Nearest Opponent | Pause (~3.5s) | Humorous "forced stop" — `PlayerMotor.SetMovementLocked(true)` for the duration. |
| 4 | Desert Boost | Self Buff | Speed Boost | Large, short self-only speed multiplier. |
| 5 | Flying Agal | Nearest Opponent | Stun (short) | Classic single-target disable. |
| 6 | Protection Shield | Defensive | Shield | Consumed automatically on the *next* incoming hit of any kind — see §6. |
| 7 | Oil Spill | Forward | Traction Loss | Placed ahead of the user; anyone who crosses it slips. |
| 8 | Date Energy | Self Buff | Cleanse + Speed Boost | Removes active negative effects (Slow/Vision/Traction) then gives a short boost. |
| 9 | Falcon Feather | Nearest Opponent | Mark | Applies no direct effect — sets `IsMarked`; the *next* hit that lands on that target from anyone gets `WeaponEffectResolver`'s magnitude/duration bonus, then the mark clears. |
| 10 (Legendary) | Royal Camel Charge | Forward (area, whole lane) | Knockdown | Extremely low `LegendarySpawnChance01` (`0.03` = 3%); `WeaponAuthority` additionally gates it to at most one grant per match via `_legendaryGrantedThisMatch` (reset by `ResetForNewMatch()`), so "usually appears only once per race" is enforced, not just unlikely. |

All **5 requested targeting types** are represented (`NearestOpponent`, `AreaEffect`, `Forward`, `Defensive`, `SelfBuff`), each backed by real branching logic in `WeaponInventoryManager.ResolveDefaultTarget` (client-side default target) and `WeaponAuthority.ResolveHits` (host-side actual resolution) — not just an enum with no consumer.

## 6. Protection Shield & Falcon Feather (special-cased effects)

- **Protection Shield** — `PlayerStatusEffectController` tracks a `Shield` flag as an *active effect* like any other; `TryApplyEffect` checks "does the incoming effect have any negative flag and am I shielded?" first (`ConsumeShieldIfPresent`) — if so, the shield is removed and the incoming effect is entirely blocked (one hit absorbed, then gone), matching "Blocks one incoming weapon; disappears after absorbing one hit" exactly. `WeaponEffectApplicator` still plays impact feedback (with `blocked: true`) so the attacker gets visual confirmation the shield ate the hit.
- **Falcon Feather** — `IPlayerStatusEffectReceiver.IsMarked`/`ClearMark()` is a boolean side-channel independent of the timed-effect list. `WeaponEffectResolver.Resolve(..., targetIsMarked)` boosts magnitude/duration only when marked, and `WeaponEffectApplicator` clears the mark on the *target* right after applying that boosted hit — so the bonus is consumed by exactly the next successful attack against that opponent, never lingering or stacking.

## 7. Balancing

No weapon deals direct damage or removes a player from the race — every effect is either a temporary movement/vision debuff (Slow/Vision Reduced/Pause/Stun/Traction Loss), a temporary buff (Speed Boost/Cleanse), a single consumable block (Shield), or a knockdown that (like a stun) is recoverable — satisfying "never permanently eliminate players." All timings/magnitudes/weights/cooldowns live in data (`WeaponDefinition` fields), so balance is a designer-editable spreadsheet-equivalent, not a code change; the Legendary's `0.03` spawn chance plus the one-per-match authority gate keeps Royal Camel Charge from being spammable, and its own targeting (Forward/whole-lane) still requires timing and positioning to land — skill remains the deciding factor, not weapon RNG alone.

## 8. Visual/Audio Feedback & Debug Tools

- **Per-weapon presentation hooks** — `WeaponDefinition` carries `icon` (`Sprite`), `pickupSound`/`activationSound`/`impactSound`/`cooldownSound` (`AudioClip`), `particlePrefab` (pooled, spawned via `ObjectPoolManager` + auto-released after `impactParticleLifetimeSeconds`), and `animationTrigger` (string) — one full set per weapon, ten times over, satisfying "unique icon/sound/particles/animation/activation feedback" and all four audio requirements (pickup/activation/impact/cooldown) structurally. Actual `.png`/`.wav`/`.anim` art assets are **not** included — same "data slot exists, final art is a future-Editor task" caveat as every previous sprint's placeholder policy (see §12).
- **Debug tools** (`WeaponsDebugView`, `OnGUI`, Editor/dev-build only, `panelX: 910` — clear of `MultiplayerDebugView`'s `460`): **Current Inventory** (both slots, live), **Weapon IDs** (every catalog entry listed with rarity), **Spawn Rate** (`WeaponCatalogConfig.LegendarySpawnChance01` and each standard weapon's `StandardSpawnWeight`), **Legendary Spawn Chance** (explicit %, plus whether it's already been granted this match), **Current Weapon State** (cooldown remaining, marked status), and buttons to simulate an Item Box pickup and to use each of the two carried slots — covering every item in the Sprint 5 "Debug" section.

## 9. Supporting Extensions (additive, no rewrites)

| File | Change | Why |
|---|---|---|
| `Core/Managers/AudioManager.cs` | + a lazily-created SFX `AudioSource` and `PlayOneShot(clip, volume)`. | Weapons (and `ItemBox`) needed a one-shot SFX playback path that didn't exist yet; added to the existing manager rather than a new one. |
| `Features/PlayerController/PlayerMotor.cs` | + `SetExternalSpeedMultiplier(float)`, + `SetMovementLocked(bool)`, both folded into the existing `ResolveAutoRunSpeed`/`ResolveRunEnabled` internals. | The generic hook every movement-affecting weapon (Slow/Boost/Pause/Stun/Traction) needed, without weapons touching `PlayerMotor` internals directly. |
| `Core/Networking/IMatchTransport.cs` + `LocalLoopbackTransport.cs` | + `Participants`, + 6 weapon pickup/use/hit events & methods. | The single network seam extension described in §4 — no other multiplayer contract changed. |
| `Domain/SpawnCategory.cs` | + `ItemBox` enum member. | Lets Item Boxes reuse Sprint 3's spawner instead of a parallel system (§2). |
| `Features/EndlessRunner/Spawning/ChunkContentSpawner.cs` | + `itemBoxConfig` field, preload, and `GetConfig` case. | Wires the new category into the existing, unmodified spawn pipeline. |

## 10. Scene & Asset Wiring

- **`Boot.unity`** — new `WeaponSystems` GameObject (root order 2) with `WeaponInventoryManager` + `WeaponAuthority`, both pointed at `WeaponCatalogConfig.asset`; placed alongside (not replacing) Sprint 4's `MultiplayerSystems`.
- **`Gameplay.unity`** — `ChunkContentSpawner`'s `itemBoxConfig` now points at `SpawnCategoryConfig_ItemBox.asset`; `GameplaySystems` gained `WeaponEffectApplicator` and `WeaponsDebugView` (both pointed at the same catalog asset).
- **`ChunkPrefab_Default.prefab`** — new `SpawnPoint_ItemBox` child (`category: 4`) alongside the existing Obstacle/Coin/PowerUp/Decoration spawn points.
- **New assets:** `Settings/Weapons/WeaponDefinition_{SandStorm,DustCloud,ArabicCoffee,DesertBoost,FlyingAgal,ProtectionShield,OilSpill,DateEnergy,FalconFeather,RoyalCamelCharge}.asset` (10), `Settings/Weapons/WeaponCatalogConfig.asset`, `Settings/SpawnCategoryConfig_ItemBox.asset`, `Prefabs/World/ItemBoxPrefab.prefab`.

As with every prior sprint, a networked `Player.prefab` instance is still **not** placed in `Gameplay.unity` — `PlayerStatusEffectController` is written and ready to attach the moment that instance exists (see §12 item 1, carried forward from Sprint 4).

## 11. Code Quality & Performance

- **SOLID:** `IPlayerStatusEffectReceiver` is Dependency Inversion so Weapons never references `Features.PlayerController`; `WeaponAuthority` (validation) is separate from `WeaponInventoryManager` (bookkeeping) is separate from `WeaponEffectApplicator` (presentation) — three responsibilities, three classes. Open/Closed: adding weapon #11 is "add a `WeaponDefinition` asset + register it in the catalog," never a code change to any manager.
- **No hardcoded values:** every tunable (magnitude, duration, cooldown, spawn weight, legendary chance, item-box spawn chance/preload count) is a serialized field on a ScriptableObject.
- **No duplicated logic:** targeting-default resolution and hit resolution both funnel through the same `WeaponTargetingType` switch pattern rather than per-weapon special-case code; `WeaponEffectResolver` is the single place mark bonuses are computed.
- **Object Pooling / mobile:** `ItemBox` and weapon impact particles are pooled (`IPoolable`/`ObjectPoolManager`); no `Instantiate`/`Destroy` call was added anywhere in this sprint's gameplay-hot-path code.

## 12. Build Verification / Compiler Status

- **Offline compile:** all **115** project `.cs` files (up from 93 after Sprint 4) recompiled together via `dotnet build .compile_check/CompileCheck.csproj` against `.compile_check/Shims/UnityEngineShim.cs`, extended this sprint with `AudioClip`, `Sprite`, `AudioSource`, `Coroutine`, `WaitForSeconds`, `Object.GetInstanceID()`, `Animator.SetTrigger(string)`, and a `Collider2D : Behaviour` base-class fix (was incorrectly `: Component`, which doesn't expose `.enabled`). **Result: Build succeeded, 0 errors, 0 warnings.**
- **YAML/GUID validation:** `.compile_check/validate_yaml_refs.py` checked all **138** project `.meta` GUIDs for duplicates (**none found**) and cross-referenced every `{fileID, guid}` in `Boot.unity` (19 documents, up from 15), `Gameplay.unity` (35 documents, up from 33), and `NetworkSyncConfig.asset` — **0 real dangling references** (the two flagged hits are the same pre-existing Unity built-in `RenderSettings` skybox/spot-cookie references documented as expected false positives since Sprint 4). `.compile_check/check_fileid_refs.py` was run explicitly against all 16 new/changed asset, prefab, and scene files from this sprint: **"ALL 16 FILES: fileID/guid references OK (138 known guids in project)."**

## 13. Remaining TODOs

1. **No networked `Player.prefab` instance exists in any scene** (carried forward from Sprints 2–4) — `PlayerStatusEffectController` is implemented and ready to attach the moment a real avatar exists; until then, weapon effects can only be exercised on synthetic connection IDs via `WeaponsDebugView`, not on an actual moving player.
2. **No real input binding for "use weapon"** — `WeaponInventoryManager.TryUseLocalSlot(int)` is fully implemented and network-wired; only `WeaponsDebugView`'s buttons call it today. Wiring a real button/gamepad input is future work once the Input System package is verified (a Sprint 1 open item, still outstanding).
3. **No final art/audio/animation assets** — every `WeaponDefinition`'s `icon`/`*Sound`/`particlePrefab`/`animationTrigger` fields exist and are wired end-to-end, but are currently unassigned (`{fileID: 0}`) placeholders; `ItemBoxPrefab`'s visual is a flat gold-tinted `SpriteRenderer` square with no sprite/animator assigned. First real Editor open + an art pass is required to fill these in.
4. **`NearestOpponent`/`AreaEffect` targeting uses connection order, not real distance** — `WeaponInventoryManager.ResolveDefaultTarget`'s `NearestOpponent` case picks "the first other connected participant," not a true nearest-by-position query, because no networked player-avatar transform exists yet (item 1). `WeaponAuthority.ResolveHits`' `AreaEffect`/`Forward` cases currently affect every other participant rather than filtering by actual in-range/in-lane position, for the same reason. Both are structured to take a real position input the moment player avatars exist, without any interface change.
5. **Legendary "one per match" reset (`WeaponAuthority.ResetForNewMatch()`) is not yet called by any match-lifecycle hook** — `Features.Multiplayer.Match.MatchManager`'s `MatchState.Waiting`/new-match transition is the natural caller once wired; not connected this sprint since `WeaponAuthority` was written after and deliberately kept ignorant of `Features.Multiplayer` (no cross-feature reference).
6. Carries forward all unresolved Sprint 1–4 items (Unity 6 LTS install still only Hub; ADR-0001 still Proposed, not Accepted; no Lobby/Waiting Room UI scene; ping always 0 under the loopback transport; bundle IDs; UI framework ADR).

## 14. Git Workflow

| Item | Value |
|---|---|
| Commit hash | `739f756` (`739f75607e380b6b9069167e99546afe337ffc29`) |
| Commit message | `Sprint 5 - Weapons, Item Boxes & Gulf Combat System` |
| Branch | `main` |
| Push status | Pushed to `origin/main` (`f5dc719..739f756 main -> main`); verified via `git fetch origin main` + `git log origin/main -1` matching the local hash, and `git status` reporting "up to date with 'origin/main'" / "nothing to commit, working tree clean" |

Sprint 5 is complete within the constraints above. Stopping here. Waiting for Sprint 6.
