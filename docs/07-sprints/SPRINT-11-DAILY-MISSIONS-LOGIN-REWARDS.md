# Sprint 11 — Daily Missions, Login Rewards & Reward System — Sprint Report

**Role:** Lead Progression Systems Engineer
**Scope:** A complete Daily Engagement / Progression system: 3 randomly-assigned Daily Missions (from a 25-entry configurable pool) that reset every 24 hours across Easy/Medium/Hard difficulty with automatic reward scaling; a 7-day Login Streak with a standard calendar plus 5 authored Special Login Event calendars (Ramadan, Eid, National Days, Summer, Winter); Temporary Cosmetics (2/3/7-day expiring Skins/Emotes/Victory Poses/Effects) with countdown timers, automatic removal on expiry, and a Store "Unlock Permanently" upsell; a "never a duplicate temporary grant" fallback (Coins/Gems) when a player already permanently owns a reward's cosmetic; Battle Pass XP as a first-class reward type; Mission/Login/Temporary-Item notifications wired into the existing notification queue; a mock-but-swappable cloud-ready Progression backend abstraction; and debug tooling. No final reward-popup art/animation/SFX assets and no real backend (same running "no final art/audio, no real Editor" constraint as every prior sprint — see §14).
**Status:** Complete, under the same environment constraint documented in [SPRINT-01-PROJECT-FOUNDATION.md](SPRINT-01-PROJECT-FOUNDATION.md) §0 (no licensed Unity Editor on this machine — Unity Hub is installed but no Editor version is downloaded).

---

## 0. Continuation check

Sprints 1–10 (Project Foundation through Store, Economy & Battle Pass) are complete and were **not** rewritten. This sprint extends six existing files additively (`RewardType`, `NotificationType`, `CosmeticInventory`, `ICosmeticGrantService`, `PlayerStatEventService`, `PlayerLoadoutManager`, `BattlePassManager`, `WeaponInventoryManager`, `NotificationManager`, plus `Features.Store`'s `StoreManager`/`StoreView`/`InventoryManager`/`InventoryView` — see §5, §12) to give the new Progression feature the seams it needs, the same "extend the interface/vocabulary, never touch the implementation contract of unrelated features" pattern used since Sprint 4.

## 1. Architecture

A new, isolated **`GulfRun.Features.Progression`** assembly (references only `GulfRun.Core` + `GulfRun.Domain`, same "Features never reference other Features" rule as every prior Features assembly) owns Daily Missions and Login Rewards. Because both must ultimately affect Coins/Gems/Battle-Pass-XP and grant real (temporary) cosmetics, three purely-additive `Core`-layer seams keep those dependencies one-directional, mirroring the exact shape Sprint 9/10 established for Online/Store:

| Layer | Type | Responsibility |
|---|---|---|
| Domain (pure, no UnityEngine) | `MissionId` | String-wrapped `readonly struct` (mirrors `StoreItemId`/`CosmeticId`) identifying a mission-pool entry. |
| Domain | `MissionType`, `MissionDifficulty` | The 9 brief-listed trackable actions and the 3 difficulty tiers. |
| Domain | `TemporaryCosmeticDuration` (+ `ToSeconds()` extension) | The 3 brief-mandated durations (2/3/7 days) as authored data, not a raw float. |
| Domain | `ActiveMission` | A player's live mission slot (definition + mutable progress + claimed flag) — owned by the backend, not by `MissionManager`. |
| Domain | `LoginStreakStatus`, `LoginStreakCalculator` | The player's streak state and the **pure** civil-day-boundary math (same-day / consecutive-day / missed-day → reset) — zero Unity dependency, fully unit-testable. |
| Domain | `TemporaryCosmeticOwnership` | An active temporary grant's id + granted/expiry timestamps + `RemainingSeconds()` helper, shared by Inventory, Store, and the debug view. |
| Domain | `ProgressionRewardLedgerEntry` | Generic owned-item ledger row (mirrors Sprint 10's `OwnedStoreItem`) for Title/Badge/ProfileFrame/ChampionEffect rewards with no dedicated inventory slot. |
| Domain | `RewardType` (extended) | Sprint 10's 11-value vocabulary plus a new `BattlePassXp` value (appended — every existing catalog's serialized ordinal is unaffected). |
| Domain | `NotificationType` (extended) | Plus the 4 new Sprint 11 categories (New Missions Available, Mission Completed, Daily Reward Available, Temporary Item Expiring Soon), also appended. |
| Domain | `CosmeticInventory` (extended) | Gained a parallel temporary-ownership map (`GrantTemporary`/`OwnsTemporarily`/`TryGetTemporaryExpiry`/`RemoveExpired`) alongside the existing permanent `HashSet`; `Grant` (permanent) now also clears any temporary grant for the same id so a later permanent purchase always wins. |
| Core.Services | `ICosmeticGrantService` (extended) | Gained `OwnsCosmeticPermanently` (the "already owns permanent?" check the fallback rule needs), `GrantTemporaryCosmetic`, and `GetTemporaryCosmetics()` — implemented by the same `PlayerLoadoutManager` Sprint 10 already wired in. |
| Core.Services | `IBattlePassXpGrantService` / `BattlePassXpGrantService` (new) | Lets `Features.Progression` grant Battle Pass XP without ever referencing `Features.Store` — implemented by `BattlePassManager`, same "implement the Core interface, don't reference the Feature" shape as `ICosmeticGrantService`. |
| Core.Services | `ProgressionNotificationBridge` (new, static event bridge) | Lets `Features.Progression` raise a notification without referencing `Features.Online.Notifications.NotificationManager` — the same bridge shape as `StoreNotificationBridge`/`FriendRequestBridge`. |
| Core.Services | `PlayerStatEventService` (extended) | Gained `LocalItemBoxOpened` (raised by `WeaponInventoryManager`) and `LocalTrapAvoided` (raised by the new `TrapAvoidanceTracker`) so Daily Missions can observe those two actions with zero new Feature-to-Feature references. |
| Core.Backend | `IProgressionBackendService` (new interface) | Active-mission generation/progress/claim, Login Streak status/claim, and the generic Progression reward ledger — mirrors `IOnlineBackendService`/`IStoreBackendService`'s "abstract the entire remote system behind one interface" pattern (ADR-0001) so a real backend is a drop-in `Current` swap. |
| Core.Backend | `LocalProgressionBackendService` (new, mock) | In-memory implementation — owns the actual 3 `ActiveMission` slots and the `LoginStreakStatus`, so `MissionManager`/`LoginRewardManager` never hold authoritative state themselves. |
| Core.Backend | `ProgressionBackendService` (new, static locator) | `Current` property, self-initializing to `LocalProgressionBackendService` — same shape as `StoreBackendService`/`OnlineBackendService`. |
| Configuration (ScriptableObject) | `MissionPoolCatalogConfig`, `LoginRewardCalendarConfig` | Every mission's target/difficulty/reward and every login day's reward (+ optional bonus reward, + optional weighted Mystery reward) is authored data (§2–§4) — no balance number lives in code, continuing Sprint 9/10's catalog pattern. |
| Progression | `MissionManager` (persistent `Singleton`) | Composition root: generates 3 daily missions, listens to `PlayerStatEventService` for progress, claims rewards (§2). |
| Progression | `LoginRewardManager` (persistent `Singleton`) | Composition root: owns the standard + Special-Event calendars, resolves/claims the daily login reward (§3). |
| Progression | `TemporaryCosmeticExpiryWatcher` (persistent `Singleton`) | Polls `ICosmeticGrantService.GetTemporaryCosmetics()` and raises one "expiring soon" notification per grant (§4, §6). |
| Progression | `RewardApplication` (internal static helper) | Shared by `MissionManager`/`LoginRewardManager` — applies Coins/Gems/BattlePassXp/temporary-or-permanent-cosmetic/ledger rewards, including the "already owns permanent → fallback" rule (§4). |
| Progression | `MissionsView`, `LoginRewardView` | `OnGUI` screens (§2, §3). |
| Progression | `ProgressionDebugView` | `OnGUI` panel (§13). |
| Traps | `TrapAvoidanceTracker` (new, scene-scoped `SceneSingleton`) | Raises `PlayerStatEventService.LocalTrapAvoided` for every active trap instance that expires without ever hitting the local player (§9). |

This mirrors Sprints 5–10's layering exactly (Domain = rules, Core.Services/Core.Backend = cross-feature seams, `Features.Progression` = the feature itself, `IProgressionBackendService` = the only "network" path) — no new architectural pattern was invented for this sprint; it is ADR-0001's abstraction applied a fourth time, to the daily-engagement layer.

## 2. Daily Missions

`MissionManager` (persistent `Singleton`) draws **3** distinct missions at random every 24 hours from `MissionPoolCatalogConfig`'s **25**-entry pool (`IProgressionBackendService.NeedsNewMissions` checked once per frame; `SetActiveMissions` records the new set and its own 24h reset timestamp) — never weekly missions, per the brief. The pool covers every brief-listed mission type across all 3 difficulties:

| Mission Type | Easy | Medium | Hard |
|---|---|---|---|
| Finish Races | 2 races → 100 Coins | 3 races → 150 Coins | 5 races → 200 Coins |
| Win Races | 1 race → 150 Coins | 1 race → 10 Gems | 2 races → 15 Gems |
| Collect Coins | 50 → 50 Coins | 100 → 100 Coins | 250 → 100 Battle Pass XP |
| Open Item Boxes | 3 → 80 Coins | 5 → 8 Gems | 8 → **Temporary Emote** (2 days) |
| Use Weapons | 5 → 80 Coins | 8 → 120 Coins | 12 → 150 Battle Pass XP |
| Avoid Traps | 5 → 80 Coins | 10 → 8 Gems | 15 → **Temporary Victory Pose** (3 days) |
| Perform Jumps | 15 → 60 Coins | 30 → 100 Coins | 50 → 10 Gems |
| Reach Top 3 | 1× → 100 Coins | 2× → 12 Gems | 3× → **Temporary Outfit** (7 days) |
| Login Today | 1× → 50 Coins | — | — |

`MissionPoolCatalogConfig.GetRewardMultiplier` scales every Coins/Gems/BattlePassXp base reward by a per-difficulty multiplier (**1.0× Easy / 1.5× Medium / 2.0× Hard**, itself authored data, not hardcoded) — "Reward scales automatically" per the brief; cosmetic/ledger rewards are not amount-scaled (there is no "amount" to scale). Progress is observed entirely through `PlayerStatEventService` (`LocalMatchCompleted` → Finish/Win/Top-3/Coins, `LocalWeaponUsed`, `LocalTrapAvoided`, `LocalJumpPerformed`, `LocalItemBoxOpened`) — zero polling, zero direct references into `Features.Weapons`/`Features.Traps`/`Features.RaceFinish`/`Features.EndlessRunner`. `MissionsView` (`x: 1410`) lists all 3 slots with a live progress bar, resets-in countdown, and a Claim button once complete.

## 3. Login Rewards & Streak

`LoginRewardManager` (persistent `Singleton`) owns one always-active **standard 7-day calendar** plus **5 authored Special Login Event calendars** — every one the brief names (Ramadan, Eid, National Days, Summer, Winter); "Future Events" is satisfied structurally (adding a 6th calendar asset is authoring data, never a code change — see `LoginRewardCalendarConfig`'s own doc comment). The standard calendar matches the brief's example exactly:

| Day | Reward |
|---|---|
| 1 | 100 Coins |
| 2 | 150 Coins |
| 3 | 200 Battle Pass XP |
| 4 | 10 Gems |
| 5 | **Temporary Outfit** (3 days, `outfit_casual_01`) |
| 6 | 200 Coins **+ 15 Gems** (`hasBonusReward` — a second flat grant on top of the primary one) |
| 7 | **Large Mystery Reward** — weighted random from {600 Coins 40%, 50 Gems 30%, 300 Battle Pass XP 20%, Temporary 7-day Outfit 10%} via the existing `WeightedSelector`/`WeightedOption<T>` (Sprint 9's Reward-catalog mechanism, reused rather than duplicated) |

Each Special Event calendar reuses the same 7-day shape with a themed cosmetic on Day 5/the Mystery pool (`outfit_ramadan_01`/`outfit_eid_01`/`outfit_national_day_01`/`outfit_seasonal_event_01`) and a small event-bonus reward bump — `LoginRewardManager.SetActiveSpecialEvent(string)` is the manual activation switch (§14 item 1). **Login Streak**: `LoginStreakCalculator.ResolveNextStreakDay` is pure civil-day-boundary math — same day → no-op (already claimed), exactly-next day → streak+1, any gap of ≥2 days → **resets to Day 1**, per the brief's "Login Streak resets, reward cycle restarts from Day 1." `LoginRewardView` (`x: 1590`) shows the 7-cell calendar strip (current day highlighted), the active calendar's label, and the Claim button.

## 4. Temporary Cosmetics & Permanent Purchase

`CosmeticInventory` now tracks permanent and temporary ownership in two independent stores; `PlayerLoadoutManager` (`ICosmeticGrantService`) exposes `GrantTemporaryCosmetic`/`GetTemporaryCosmetics()`/`OwnsCosmeticPermanently` and, every `Update()` (throttled), calls `CosmeticInventory.RemoveExpired` and **automatically unequips** any cosmetic that just expired — "Item is automatically removed" is real, not just a flag flip. `RewardApplication.ApplyCosmeticReward` enforces the brief's fallback rule before ever granting a temporary duplicate:

```
if cosmeticId is already owned PERMANENTLY:
    grant fallbackCoinsAmount Coins instead   // "Never reward temporary duplicate. Instead reward: Coins/Small Gems/Alternative reward."
else:
    grant a TEMPORARY cosmetic expiring at (now + duration.ToSeconds())
```

`TemporaryCosmeticExpiryWatcher` polls every temporary grant every 30s and raises one `TemporaryItemExpiringSoon` notification per item the first time its remaining time drops to ≤6 hours (never twice for the same grant). **Store integration** (`Features.Store`, extended — not duplicated): `StoreManager.TryGetTemporaryCosmeticExpiry` reports a temporarily-owned linked cosmetic's remaining time; `StoreManager.OwnsStoreItemEntry` now checks `OwnsCosmeticPermanently` (not `OwnsCosmetic`) so a temporary owner still sees the item as purchasable. `StoreView` shows `"<Item> [Temporary: <Xh Ym> left]"` and swaps the Buy button's label to **"Unlock"** for these rows — the brief's "Remaining Time / Unlock Permanently / Gem Price" trio, all backed by real state. `InventoryView` gained its own "Temporary Items" section listing every active grant with a live countdown.

## 5. Reward System

`RewardApplication` (internal to `Features.Progression`, shared by `MissionManager` and `LoginRewardManager` — intentionally **not** shared with `Features.Store.BattlePass.BattlePassManager`'s own copy, per this project's "each Feature owns its own reward application" convention) is the single place every Sprint 11 reward resolves:

| Reward category (brief) | `RewardType` | Applied via |
|---|---|---|
| Coins | `Coins` | `EconomyManager.AddCoins` |
| Small amount of Gems | `Gems` | `EconomyManager.AddGems` |
| Battle Pass XP | `BattlePassXp` (**new**) | `IBattlePassXpGrantService.AddXp` → `BattlePassManager` → `IStoreBackendService.AddBattlePassXp` |
| Temporary Skins/Emotes/Victory Poses/Effects | `ExclusiveSkin`/`ExclusiveEmote`/`VictoryPose`/`LimitedCosmetic` + `isTemporaryCosmeticReward: true` | `ICosmeticGrantService.GrantTemporaryCosmetic` (or the fallback Coins path, §4) |
| Future cosmetic rewards | Same switch, any existing `RewardType` value | Adding reward type #13 is a `RewardType` enum append, same as Sprint 9/10 |

Every reward — mission or login — is tagged with a stable ledger key (`"mission_<id>_<slot>"` / `"login_day_<n>"` / `"login_mystery_day<n>_<timestamp>"`) so Title/Badge/ProfileFrame/ChampionEffect-typed rewards (no dedicated inventory slot yet, same Sprint 10 §14 gap) are still tracked as real, queryable ownership via `IProgressionBackendService`'s ledger, not silently dropped.

## 6. Reward Animations

"Opening animation / Sound effects / Reward popup / rarity colors" are represented structurally but not with final art/audio (same "no final art yet" status every prior sprint's placeholder UI carries): `MissionsView`'s Claim button immediately renders a green inline confirmation label (stand-in reward popup), `LoginRewardView`'s calendar strip color-codes the **current** day in yellow (stand-in rarity color), and every reward flows through the same `NotificationType`-driven queue Sprint 9 built (stand-in for a dedicated animation/SFX trigger). Wiring real popup prefabs/animation timelines/SFX is tracked in §14.

## 7. Player Progression

Mission completion already feeds every brief-listed target with zero extra plumbing, because Sprint 11 deliberately reuses existing sinks rather than inventing parallel ones: **Coins** → `EconomyManager.AddCoins` (same wallet Sprint 7 built); **Battle Pass** → `IBattlePassXpGrantService.AddXp` → the exact Sprint 10 `BattlePassManager`/`IStoreBackendService.AddBattlePassXp` XP pipeline; **Daily Progress** → each `ActiveMission.CurrentAmount`/`IsCompleted`/`IsClaimed` state, persisted in `IProgressionBackendService`; **Player Activity** → every mission-progress report and login claim raises a real notification (§8) and updates `ProgressionDebugView`'s live counters (§13).

## 8. Notifications

`NotificationManager` (Sprint 9, extended) now also subscribes to `ProgressionNotificationBridge`, so all 4 brief-listed events raise a real notification through the exact same capped queue/UI every other sprint's notifications use — no parallel notification system:

| Brief event | Trigger |
|---|---|
| New missions available | `MissionManager.EnsureMissionsFresh` right after generating a fresh 3-mission set |
| Mission completed | `MissionManager.ReportProgress` the instant any mission's `CurrentAmount` crosses `TargetAmount` |
| Daily reward available | Once per app session, if `LoginRewardManager.HasClaimedToday()` is false; also re-raised as a claim confirmation on `TryClaimDailyLogin` |
| Temporary item expiring soon | `TemporaryCosmeticExpiryWatcher`, first time a grant's remaining time drops to ≤6h |

## 9. Mission-Tracking Hooks (new this sprint)

Two brief mission types had no existing observation point and needed new, single-responsibility hooks — both raise through `PlayerStatEventService`, never a direct `Features.Progression` reference:

- **Open Item Boxes**: `WeaponInventoryManager.HandlePickupConfirmed` now raises `LocalItemBoxOpened` for the local player's own confirmed pickup, regardless of whether a weapon was actually granted — "opening" the box is what counts, not its contents.
- **Avoid Traps**: this project has no spatial "was nearby but didn't touch it" concept and no networked remote-avatar collider yet, so "avoided" is honestly and simply defined as *an active trap instance's lifetime expired without ever confirming a hit against the local player* — the new `TrapAvoidanceTracker` (scene-scoped `SceneSingleton`, lives in `Gameplay.unity`'s existing `TrapSystems` GameObject alongside `TrapSpawnController`/`TrapEffectApplicator`) listens to `IMatchTransport.TrapSpawned`/`TrapExpired`/`TrapTriggerConfirmed` and raises `LocalTrapAvoided` accordingly.

## 10. Backend

`IProgressionBackendService` owns every piece of authoritative Progression state a real backend would: the 3 active `ActiveMission` slots + their 24h reset timestamp, `LoginStreakStatus` (current day / last claim / total logins ever), and the generic reward ledger — `MissionManager`/`LoginRewardManager` never hold this state themselves, only orchestrate against it (same "the backend owns state, the feature manager orchestrates" split `IOnlineBackendService`/`IStoreBackendService` established). `LocalProgressionBackendService` is an honest, clearly-labelled in-memory mock; swapping in a real backend later is a single `ProgressionBackendService.Current` assignment, zero call-site changes anywhere in `Features.Progression`. Mission pool, difficulty multipliers, reward tables, temporary durations, and the login calendar(s) are **100% `ScriptableObject`-authored data** under `Client/Assets/_Project/Settings/Progression/` — "Backend controls..." is satisfied today by local catalog assets (no live-tunable remote config service exists yet, same honest scope as every prior sprint's catalogs), with the call sites already backend-shaped for a future swap.

## 11. Performance

Mission/login state reads (`ActiveMissions`, `Status`) are cheap `IReadOnlyList`/struct accessors over already-computed backend state — no per-frame allocation or recomputation. `MissionManager.EnsureMissionsFresh` and `TemporaryCosmeticExpiryWatcher`'s expiry sweep are both cheap timestamp comparisons gated behind a reset/interval check, not unconditional per-frame work; the watcher is additionally throttled to once per 30 real-world seconds via `Time.time`. `PlayerLoadoutManager`'s new expiry tick reuses the same "check on an interval, not every frame" pattern already established for other polling code in this project. No new catalog is loaded more than once (`ScriptableObject` assets, referenced once at scene load, same as every prior sprint's catalogs).

## 12. Code Quality (SOLID / No Hardcoded Values / Modularity)

- **SOLID**: `MissionManager` (mission lifecycle), `LoginRewardManager` (streak/calendar lifecycle), and `TemporaryCosmeticExpiryWatcher` (expiry notifications) are three separate single-responsibility managers, not one god-object — the same split Sprint 9/10 used for League/Championship/Statistics and Store/BattlePass/Inventory. Dependency Inversion: `Features.Progression` depends only on `Core`/`Domain` interfaces (`IProgressionBackendService`, `ICosmeticGrantService`, `IBattlePassXpGrantService`, `PlayerStatEventService`), never on `Features.Store`/`Features.Character`/`Features.Weapons`/`Features.Traps` concrete types.
- **No hardcoded values**: every mission's target/difficulty/reward, every difficulty multiplier, every login day's reward (+ bonus + Mystery weights), and every temporary duration is `ScriptableObject`-authored data (§10) — the only literal constants in code are structural (`DailyMissionCount = 3`, the 24h reset interval), matching the brief's own fixed "3 Daily Missions" / "resets every 24 hours" requirements, not tunable balance numbers.
- **Modular / future expansion ready**: adding mission pool entry #26, a 6th Special Login Event calendar, or reward type #13 is authoring one new catalog row/asset — zero code changes, exactly as `MissionPoolCatalogConfig`'s and `LoginRewardCalendarConfig`'s own doc comments state.
- **`Features.Store` extension, not duplication** (§4): `StoreManager`/`StoreView`/`InventoryManager`/`InventoryView` gained the minimum surface needed for the Permanent-Purchase upsell — no Progression-specific logic leaked into `Features.Store`, and no Store-specific logic leaked into `Features.Progression`.
- **Offline shim**: no new shim APIs were required this sprint — every `OnGUI`/`GUIStyle`/`GUI.Box` call needed already existed after Sprint 9/10's shim extensions.

## 13. Debug Tools

`ProgressionDebugView` (`OnGUI`, Editor/dev-build only, `panelX: 3160, panelY: 10` — `Gameplay.unity`'s next free slot after Sprint 10's `StoreDebugView` at `panelX: 2710`), covering every brief-listed field:

- **Mission IDs** — each active slot's `ActiveMission.SourceMissionId`.
- **Mission Progress** — `CurrentAmount/TargetAmount` + Claimed/Ready status per slot, plus the reset countdown.
- **Reward IDs** — each mission's resolved `RewardType` × amount; the generic Progression reward ledger's `LedgerKey` + `RewardType` list.
- **Temporary Item Timers** — every active temporary cosmetic grant's id + live remaining time (`FormatSeconds`).
- **Login Streak** — current streak day, total logins ever, claimed-today flag, and the active calendar (standard or which Special Event).

## 14. Remaining TODOs

1. **Special Login Event activation is manual only** — `LoginRewardManager.SetActiveSpecialEvent(string)` exists and is fully functional, but no live calendar/scheduler auto-activates Ramadan/Eid/National Days/Summer/Winter by real-world date yet (same category of TODO already flagged for Sprint 9/10's Championship/Event/Special-Offer scheduling).
2. **No final reward-popup art/animation/SFX assets** (§6) — every reward's "opening animation / sound effects / reward popup / rarity colors" is represented by placeholder `OnGUI` feedback text/color, same "no final art yet" status carried forward from every previous sprint.
3. **Title/Badge/ProfileFrame/ChampionEffect rewards still have no dedicated inventory slot** — tracked as real ownership in `IProgressionBackendService`'s ledger (§5), same category of gap Sprint 10 §14 item 5 already flagged for Store-purchased Visual Effects/Profile Frames.
4. **"Avoid Traps" is honestly scoped, not spatial** (§9) — defined as "trap expired without hitting the local player" rather than a true proximity/near-miss check, because no networked remote-avatar collider and no lane-change axis exist yet (same underlying limitation as Sprint 6 Report §12 item 1).
5. **`LocalProgressionBackendService` is in-memory only** — resets on Play Mode restart, same category of TODO carried forward from every prior sprint's economy/backend mocks (`LocalStoreBackendService`, `LocalOnlineBackendService`, `EconomyManager`).
6. **Mystery Reward weighting reuses Sprint 9's `WeightedSelector`** rather than a bespoke gacha/loot-box system — intentional reuse (§3), but there is no pity-timer/bad-luck-protection mechanic yet if a live economy ever needs one.
7. **No live remote config for mission pool/reward tables/login calendar** (§10) — "Backend controls..." is satisfied by local `ScriptableObject` catalogs today; a real LiveOps config service is a drop-in swap at the catalog-reference level, not a rewrite.
8. **No networked `Player.prefab` instance exists in any scene** (carried forward from Sprints 2–10).
9. Carries forward all unresolved Sprint 1–10 items (see those reports' own Remaining TODOs sections).

## 15. Build Verification / Compiler Status

- **Offline compile:** all **280** project `.cs` files (up from 254 after Sprint 10) recompiled together via `dotnet build .compile_check/CompileCheck.csproj` against `.compile_check/Shims/UnityEngineShim.cs`. Two issues surfaced and were fixed during this sprint: a missing `using UnityEngine;` in the new `TrapAvoidanceTracker.cs` (needed for `[DisallowMultipleComponent]`), and a definite-assignment compiler error in `StoreView.cs` (`out double expiresAtSeconds` used outside the `&&` short-circuit that assigned it — fixed by declaring it with a `0d` default beforehand). **No shim extensions were required this sprint** — every Unity API this sprint's code needed already existed after Sprint 9/10. **Result: Build succeeded, 0 errors, 0 warnings** (after the two fixes above).
- **YAML/GUID validation:** `.compile_check/validate_yaml.py` — `Boot.unity`, `Gameplay.unity`, and all 7 new Progression catalog assets all **OK**. `.compile_check/validate_yaml_refs.py` — **361** unique project `.meta` GUIDs (up from 327 after Sprint 10; 26 new script metas + 1 new asmdef meta + 7 new asset metas, generated via `.compile_check/generate_metas.ps1`); **no duplicates**; the only flagged references are the same pre-existing Unity built-in `RenderSettings` skybox/spot-cookie references documented as expected false positives since Sprint 4. `.compile_check/check_fileid_refs.py` re-run against `Boot.unity`/`Gameplay.unity` (which properly excludes those same built-in GUIDs) — **"ALL 2 FILES: fileID/guid references OK (361 known guids in project)."**

## 16. Scene & Asset Wiring

- **`Boot.unity`** — new `ProgressionSystems` GameObject (root order 8), alongside (not replacing) every prior sprint's systems GameObject, holding the 3 persistent Progression singletons: `MissionManager` (pointed at `MissionPoolCatalogConfig.asset`), `LoginRewardManager` (pointed at the standard calendar + all 5 Special Event calendars), `TemporaryCosmeticExpiryWatcher`.
- **`Gameplay.unity`** — `TrapAvoidanceTracker` added as a 5th component onto the existing `TrapSystems` GameObject (alongside `TrapSpawnController`/`TrapEffectApplicator`/`TrapsDebugView`); two new GameObjects: `ProgressionUI` (`MissionsView` at `x: 1410`, `LoginRewardView` at `x: 1590` — scene-scoped `OnGUI` screens) and `ProgressionDebug` (`ProgressionDebugView`, `panelX: 3160`, this project's rightmost debug panel to date).
- **New assets:** `Settings/Progression/MissionPoolCatalogConfig.asset` (25 missions), `Settings/Progression/LoginRewardCalendarConfig_Standard.asset` (7 days), plus 5 Special Event variants — `LoginRewardCalendarConfig_Ramadan.asset`, `_Eid.asset`, `_NationalDay.asset`, `_Summer.asset`, `_Winter.asset` (7 days each).
- As with every prior sprint, a networked `Player.prefab` instance is still **not** placed in `Gameplay.unity`.

## 17. Git Workflow

| Item | Value |
|---|---|
| Commit hash | _see below_ |
| Commit message | `Sprint 11 - Daily Missions, Login Rewards & Reward System` |
| Branch | `main` |
| Push status | _see below_ |

Sprint 11 is complete within the constraints above. Stopping here.
