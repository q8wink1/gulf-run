# Sprint 8 — Characters, Countries & Customization System — Sprint Report

**Role:** Lead Character System Engineer
**Scope:** A scalable Character & Customization system: 12 unlocked-from-launch playable characters, a one-time Account Creation flow that permanently links a player's Country (auto-applying that Country's free Traditional Outfit), a Gem-funded Premium Cosmetics system with slots reserved for future Hats/Glasses/Shoes/Accessories/Back Items/Trails/Pets/Victory Poses/Emotes, an extended Win/Lose/Celebrate animation vocabulary, full networking of Character/Country/Outfit/Cosmetics, and debug tooling. No final art/audio/animation assets and no networked `Player.prefab` instance (same running "no final gameplay logic without a real Editor" constraint as every prior sprint — see §11).
**Status:** Complete, under the same environment constraint documented in [SPRINT-01-PROJECT-FOUNDATION.md](SPRINT-01-PROJECT-FOUNDATION.md) §0 (no licensed Unity Editor on this machine — Unity Hub is installed but no Editor version is downloaded).

---

## 0. Continuation check

Sprints 1–7 (Project Foundation, Player Controller Foundation, Endless Runner Core, Multiplayer Foundation, Weapons/Item Boxes/Combat, Dynamic Trap System, Race Finish/Ranking/Victory Ceremony) are complete and were **not** rewritten. This sprint extends five existing files additively (`GulfCountry`, `EconomyManager`, `IMatchTransport`/`LocalLoopbackTransport`, `SessionManager`, `SaveManager`, the `PlayerAnimatorController` asset, plus the offline shim — see §12) to give the new Character feature the seams it needs, the same "extend the interface/vocabulary, never touch the implementation contract of unrelated features" pattern used since Sprint 4. Sprint 7's `Features.RaceFinish.Configuration.FlagCatalogConfig` is deliberately left untouched (see §3).

## 1. Architecture

A new, isolated **`GulfRun.Features.Character`** assembly (references only `GulfRun.Core` + `GulfRun.Domain`, same "Features never reference other Features" rule as every prior Features assembly) owns everything character/customization-related:

| Layer | Type | Responsibility |
|---|---|---|
| Domain (pure, no UnityEngine) | `CharacterId`, `CosmeticId` | String-wrapped `readonly struct` identifiers (not enums) — "prepare support for unlimited future characters/cosmetics" means adding #13 (or cosmetic #500) must be a **data change**, never a code change. This deliberately departs from `WeaponId`/`TrapId`'s enum pattern, whose fixed launch sets don't have that requirement. |
| Domain | `GulfCountry` (extended) | Now 8 values (added `Iraq`, `Egypt` to Sprint 7's original 6) — the permanent, one-time account field. |
| Domain | `CosmeticSlot` | Outfit/Hat/Glasses/Shoes/Accessory/BackItem/Trail/Pet/VictoryPose/Emote — every future slot the brief calls out exists today, even though only Outfit ships wearable content this sprint. |
| Domain | `PlayerAccount` | Immutable Display Name + Country pair — the permanent record Account Creation produces exactly once. |
| Domain | `PlayerLoadout` | One player's live state: `CharacterId`, `GulfCountry` (read-only after construction), and one `CosmeticId` per `CosmeticSlot`. `Clone()` supports safe network hand-off. |
| Domain | `CosmeticInventory` | Permanent ownership set (`HashSet<string>` under a typed API) — COS-OWN-001, "unlocked cosmetics are permanently owned." |
| Domain | `CosmeticUnlockRules` | Pure `CanAfford(currentGems, priceGems)` predicate. |
| Domain | `CharacterGenderPresentation` | Male/Female — supports a shared rig/animation set per presentation for the "Shared Animation Controller" performance requirement (§9). |
| Domain | `CharacterAnimationState` | Idle/Run/Jump/DoubleJump/Fall/Win/Lose/Celebrate — the complete animation vocabulary the brief requires. |
| Domain | `CharacterAnimationResolver` | Pure mapping from the existing `PlayerMovementState` (Sprint 2) and `FinishReason` (Sprint 7) to this new vocabulary — no parallel state machine. |
| Core.Save | `IAccountRepository` (new) | One-time, immutable account creation/retrieval contract, implemented by `SaveManager` alongside its existing `IProgressRepository` — same "manager implements the feature's repository interface" pattern already used for progress. |
| Core.Countries | `CountryCatalogConfig` (new ScriptableObject) | Single source of truth for per-country code/display name/flag, shared by `Features.Character` (Account Creation, Character Menu) and any future `Features.Multiplayer` lobby screen without either depending on the other (§3). |
| Core.Services | `CharacterAnimationCueService` (new) | Static one-shot event bus (`RaiseLocalCue`/`LocalCueRaised`) so `Features.RaceFinish` can trigger Win/Lose/Celebrate on the local player's Animator without a RaceFinish→PlayerController or PlayerController→RaceFinish coupling — the same decoupling shape as `IRaceProgressProvider`. |
| Configuration (ScriptableObject) | `CharacterDefinition` | One character's identity + presentation (id, display name, gender presentation, preview prefab, portrait icon, placeholder color) — zero gameplay-affecting fields (CHR-005, "identical gameplay statistics"). |
| Configuration (ScriptableObject) | `CharacterCatalogConfig` | The ordered list of all characters — "12 at launch" is a data fact (12 authored assets), not a code constant. |
| Configuration (ScriptableObject) | `CosmeticCatalogConfig` | Every cosmetic across every slot as embedded `CosmeticEntry` data (not one asset per item, unlike `WeaponDefinition`/`TrapDefinition`) — deliberately, since this list is expected to grow into the hundreds and one-asset-per-item doesn't scale for content authors the way `FlagCatalogConfig`'s nested-entry approach already proved out in Sprint 7. |
| Account | `AccountCreationView` | `OnGUI` one-time Display Name + Country screen, self-gating on `SaveManager.HasAccount`. |
| Loadout | `PlayerLoadoutManager` (persistent `Singleton`) | Composition root for the whole feature — owns the local `PlayerLoadout`/`CosmeticInventory`, applies the "country auto-equips its Traditional Outfit" rule the instant an account exists, brokers Gem unlocks through `EconomyManager`, and mirrors every other participant's loadout via `IMatchTransport.LoadoutChanged` (§2, §7). |
| Menu | `CharacterMenuView` | `OnGUI` Character Menu (§8). |
| Debug | `CharacterDebugView` | `OnGUI` panel (§10). |

This mirrors Sprints 5–7's layering exactly (Domain = rules, Core.Services/Core.Save/Core.Countries = cross-feature seams, Features.Character = the feature itself, `IMatchTransport` = the only network path) — no new architectural pattern was invented for this sprint.

## 2. Account Creation & the Permanent Country

- `AccountCreationView` collects **Display Name** (a `GUI.TextField`, new to the offline shim — see §12) and **Country** (a button grid over all 8 `GulfCountry` values, labelled via `CountryCatalogConfig`) and is visible **only** while `SaveManager.HasAccount` is false — the instant `SaveManager.CreateAccount(displayName, country)` succeeds, the screen is gone for the rest of the session.
- `SaveManager` (extended, alongside its existing `IProgressRepository`) now implements **`IAccountRepository`**: `CreateAccount` is **idempotent** — calling it again after `HasAccount` is true just returns the existing `PlayerAccount` untouched, which is the actual enforcement of "Country cannot be changed later," not merely a UI restriction.
- **Country determines four presentation surfaces**, all sourced from the same permanent `PlayerAccount.Country`: National Flag / Profile Flag / Lobby Flag / Podium Flag are one and the same value read from `CountryCatalogConfig`, and the Traditional Outfit is auto-granted/auto-equipped by `PlayerLoadoutManager` the moment the account exists (§4). There is exactly one place a country is ever set — `SaveManager.CreateAccount` — so all four "determines" bullets are structurally guaranteed to agree, not four independent settings that could drift.
- **`SessionManager.LocalPlayerCountry`** (changed from a freely-settable serialized field to a computed property): now reads `SaveManager.Instance.GetAccount().Country` once an account exists, falling back to a `fallbackPlayerCountry` serialized field only if a match is somehow created/joined before Account Creation has run (should never happen in normal flow). `SessionManager.SetLocalPlayerCountry` (Sprint 7's placeholder setter) was **removed** — there is no longer any code path, anywhere in the project, that can change a player's Country after account creation.

## 3. Characters (12, All Unlocked, Unlimited Future Room)

- **12 `CharacterDefinition` assets** (`Settings/Characters/Character01..12.asset`), each with a unique `id` (`character_01`..`character_12`), display name, alternating `CharacterGenderPresentation`, and a distinct placeholder color — referenced by one `CharacterCatalogConfig.asset` (`Settings/CharacterCatalogConfig.asset`).
- **All unlocked at launch**: there is no lock/ownership check anywhere in `PlayerLoadoutManager.SelectCharacter` — it only validates that the requested `CharacterId` exists in the catalog. Compare this to `TryUnlockCosmetic`, which *does* gate on ownership/Gems — the asymmetry is intentional and matches the brief exactly ("All characters are unlocked from the beginning" vs. "Only traditional outfits are free [for cosmetics]").
- **Changing character never changes Country**: `SelectCharacter` calls `PlayerLoadout.SetCharacter` only — `PlayerLoadout.Country` has no setter at all (it is fixed at construction from the account), so there is no method in the entire codebase capable of changing it after the loadout is created.
- **Unlimited future characters**: adding #13 is authoring one new `CharacterDefinition` asset and adding it to `CharacterCatalogConfig`'s list — zero code changes, because `CharacterId` is a free-form string, not an enum.
- Country and Character are stored in the same `PlayerLoadout` object for convenience but are otherwise fully decoupled data: nothing reads one to influence the other except the one-time auto-equip in §4.

## 4. Countries & Free Traditional Outfits

8 launch countries, each with one free, auto-applied Traditional Outfit — both catalogued as data, not code:

| Country | `CountryCatalogConfig` code | Traditional Outfit (`CosmeticCatalogConfig` entry) |
|---|---|---|
| Saudi Arabia | KSA | Thobe, Shemagh & Agal |
| Kuwait | KWT | Dishdasha, Ghutra & Agal |
| United Arab Emirates | UAE | Kandura & Ghutra |
| Qatar | QAT | White Thobe & Maroon Ghutra |
| Bahrain | BHR | Traditional Bahraini Outfit |
| Oman | OMN | Omani Dishdasha & Kumma |
| Iraq | IRQ | Traditional Iraqi Outfit |
| Egypt | EGY | Traditional Egyptian Galabeya |

- **Auto-apply mechanism**: `CosmeticCatalogConfig.GetTraditionalOutfitId(GulfCountry)` finds the one `CosmeticEntry` with `isTraditionalOutfit = true` and `requiredCountry` matching. `PlayerLoadoutManager.TryInitializeFromAccount` calls this exactly once (right after `SaveManager.HasAccount` first becomes true), grants it into `CosmeticInventory` for free, and equips it into `CosmeticSlot.Outfit` — "selecting a country automatically applies its national clothing to any selected character" is satisfied structurally: the outfit is equipped before the player ever opens the Character Menu, regardless of which of the 12 characters is currently selected.
- **Free forever, for that one country only**: `CosmeticEntry.GemPrice` returns `0` whenever `IsTraditionalOutfit` is true (the `gemPrice` field is ignored in that case), and `CharacterMenuView`'s outfit list filters out every other country's Traditional Outfit entirely (`entry.RequiredCountry == manager.LocalLoadout.Country`) — a player can never see, unlock, or accidentally equip another nation's free outfit.
- **New `Core.Countries.CountryCatalogConfig`** (not a Feature type) intentionally duplicates the *shape* of Sprint 7's `Features.RaceFinish.Configuration.FlagCatalogConfig` rather than replacing it: Country now determines National/Profile/Lobby/**Podium** Flag (this sprint's brief), so both `Features.Character` and a future `Features.Multiplayer` lobby screen need the exact same code/display-name/flag data without depending on each other or on `Features.RaceFinish`. Migrating the tested, working Sprint 7 podium-flag code onto this new catalog is called out as a low-risk follow-up (§13), not done here — "do not rewrite a prior sprint" applies to `FlagCatalogConfig` the same way it applied to Sprint 7's own untouched Sprints 1–6 code.

## 5. Customization & Premium Cosmetics

- **`CosmeticSlot`** has all 10 values the brief lists (Outfit + the 9 "Future Support" categories) so the enum itself never needs to grow again — only `CosmeticCatalogConfig.entries` grows as new content ships.
- **`CosmeticCatalogConfig.asset`** ships 20 entries this sprint: the 8 free Traditional Outfits (§4), 9 example Premium Outfits covering every category the brief names (Football Club Kit, National Team Kit, Sportswear, Casual Clothing, Ramadan Collection, Eid Collection, National Day Collection, Seasonal Event, Limited Edition), plus 3 entries deliberately authored in **non-Outfit** slots (a Hat, a Victory Pose, an Emote) specifically to prove the slot system already works end-to-end for content that has zero visual representation yet.
- **Cosmetics never affect gameplay**: no `CosmeticEntry` field, no `PlayerLoadout` field, and no code path anywhere in `Features.Character` touches speed, jump height, hitboxes, or any other gameplay value — the type system enforces this the same way `CharacterDefinition` does (§3).
- **Gem-funded unlocking**: `PlayerLoadoutManager.TryUnlockCosmetic` — returns `true` immediately if already owned (idempotent); refuses Traditional Outfits (they are never Gem-unlocked, only auto-granted); otherwise calls `EconomyManager.TrySpendGems(entry.GemPrice)` and only grants ownership into `CosmeticInventory` if the spend succeeds. `EquipCosmetic` separately refuses to equip anything not owned (COS-OWN-001).
- **`EconomyManager`** gained a real (in-memory, non-persistent — same status as Sprint 7's `Coins`) **`Gems`** currency this sprint: `Gems` property, `GemsChanged` event, `AddGems`, `TrySpendGems`, and a `startingGems` inspector field (500, wired into the `EconomyManager` component in `Boot.unity`) so the unlock flow is exercisable today with no Shop/IAP system yet.

## 6. Animation

`CharacterAnimationState` unifies the full required vocabulary (Idle, Run, Jump, DoubleJump, Fall, Win, Lose, Celebrate):

- **Idle/Run/Jump/DoubleJump/Fall** already existed as `PlayerMovementState` (Sprint 2/3) driving `PlayerAnimatorDriver`'s existing parameters — `CharacterAnimationResolver.FromMovementState` is the single mapping function, so there is exactly one source of truth for "which locomotion animation plays," not two parallel enums drifting apart.
- **Win/Lose** are resolved from Sprint 7's existing `FinishReason` via `CharacterAnimationResolver.FromFinishReason` and raised by `RaceStandingsTracker.HandlePlayerRaceResultReported` (only for the local connection) through the new `CharacterAnimationCueService.RaiseLocalCue`.
- **Celebrate** is raised by `PodiumCeremonyView` the instant its local view enters `RaceEndPhase.Podium`, but **only if the local player actually finished top 3** (`RaiseCelebrateCueIfLocalTopThree`, checking `FinishPosition` 1–3 against `RaceStandingsTracker.FinalResults`) — a 4th-place-or-worse finisher's character never plays the Celebrate animation, matching the Podium Ceremony's existing "top 3 only" rule (Sprint 7 §5).
- **`PlayerAnimatorDriver`** (Sprint 2, extended) subscribes to `CharacterAnimationCueService.LocalCueRaised` and sets three new Animator triggers (`WinTrigger`/`LoseTrigger`/`CelebrateTrigger`) — completely decoupled from `RaceFinish`/`Ceremony` by construction, since it only ever talks to the static cue service, never to those features' types directly.
- **`PlayerAnimatorController.controller`** (hand-authored YAML, extended): 3 new Trigger parameters, 3 new placeholder states (`Win`, `Lose`, `Celebrate`), `AnyState → {state}` transitions for each, and `{state} → Idle` return transitions — the controller graph itself now has a state for every value in `CharacterAnimationState`, even though the actual motion clips are still unassigned placeholders (§11 TODO).
- **Shared Animation Controller (performance)**: one `PlayerAnimatorController` asset continues to drive every character regardless of which of the 12 is selected or which `CharacterGenderPresentation` it has — no per-character Animator Controller was created, satisfying "Shared Animation Controller" directly rather than by convention.

## 7. Networking

| Requirement | Mechanism |
|---|---|
| Character | `PlayerLoadout.Character`, part of the payload broadcast by `IMatchTransport.SetLocalLoadout` |
| Country | `PlayerLoadout.Country` — same payload; immutable for the life of the loadout object, consistent with §2 |
| Current Outfit | `PlayerLoadout.GetEquipped(CosmeticSlot.Outfit)` — same payload |
| Current Cosmetics | Every other `CosmeticSlot`'s equipped `CosmeticId` — same payload (all 10 slots, not just Outfit, travel together) |
| Victory Pose | `CosmeticSlot.VictoryPose` is equippable today (`victorypose_falcon_01` ships as example content, §5) and syncs identically to every other slot — no separate "victory pose" network message was needed |

- **`IMatchTransport`** gained `event Action<PlayerLoadout> LoadoutChanged` and `void SetLocalLoadout(PlayerLoadout)`; **`LocalLoopbackTransport`** implements both plus a `SimulateRemoteLoadout` test helper — the same "extend the interface, one loopback implementation" pattern as every prior sprint's networking addition.
- **Client-authoritative broadcast, not host-validated request**: `PlayerLoadoutManager.BroadcastLocalLoadoutIfActive` calls `SetLocalLoadout` directly (mirroring how Sprint 4's Ready System already broadcasts `PlayerReadyState` client-side) rather than routing through a host-approval round-trip — appropriate here because, unlike weapon pickups or trap triggers, a loadout choice has no gameplay-fairness stake to protect (§5, "cosmetics never affect gameplay"), so there is nothing for a host to validate.
- **When broadcasts fire**: on `SelectCharacter`/`EquipCosmetic` (immediate), on `ParticipantJoined` (so a just-joined participant immediately learns this client's loadout), and on `MatchStateChanged → Waiting` (so a loadout chosen before a match existed is (re)announced the moment one starts) — `PlayerLoadoutManager.HandleLoadoutChanged` stores every remote sender's loadout (cloned) in `RemoteLoadouts`, ignoring echoes of the local connection's own broadcast.
- Every loadout field syncs together as one payload; there is no per-field message, so "Character, Country, Current Outfit, Current Cosmetics, Victory Pose" are all synchronized by the same single mechanism, never five different ones.

## 8. Character Menu

`CharacterMenuView` (`OnGUI`, toggled by a corner button, same "OnGUI placeholder, real UI Toolkit screen later" posture as every prior sprint's UI — `docs/02-architecture/TECHNICAL_STACK.md`):

- **Current Character** — display name + a colored-box **Character Preview** (the character's `CharacterDefinition.PlaceholderColor`, standing in for a real 3D/sprite preview until art exists) with **Prev/Next** buttons that call `SelectCharacter` to cycle through the full 12-character catalog live.
- **Current Country** — resolved via `CountryCatalogConfig`, labelled "(permanent)" so the UI itself communicates the one-time-choice rule, not just the backend.
- **Current Outfit** — the currently equipped `CosmeticSlot.Outfit` entry's display name.
- **Gems** — the live `EconomyManager.Gems` balance.
- **Owned / Locked Cosmetics with Gem Price** — `DrawOutfitList` iterates every Outfit-slot `CosmeticCatalogConfig` entry ownable by this account (this country's Traditional Outfit + every Premium Outfit — other countries' Traditional Outfits are filtered out per §4), showing **EQUIPPED** / **Owned** (with an Equip button) / **`{price} Gems`** (with an Unlock button) per entry — exactly the three states ("owned," "locked with gem price," "currently equipped") the brief's Character Menu section calls for.

## 9. Performance

- **Mobile optimized / minimal memory**: `CharacterCatalogConfig`/`CosmeticCatalogConfig` build their `Dictionary`/index lookups lazily and only once (`EnsureIndexed`, invalidated via `OnValidate` in-editor), matching `WeaponCatalogConfig`/`TrapCatalogConfig`'s existing pattern; `PlayerLoadout` is a small fixed-size array (one `CosmeticId` per `CosmeticSlot`, 10 total) with no per-frame allocation.
- **Shared Animation Controller** — one controller asset for all 12 characters (§6), not 12 separate controllers.
- **Reusable assets** — `CosmeticCatalogConfig` embeds cosmetic data as plain serialized entries in one asset (not one `ScriptableObject` per item), and `CharacterGenderPresentation` groups characters into two presentation buckets so future rig/animation-set reuse (male rig shared across all male-presentation characters, same for female) is a data grouping already in place rather than a refactor waiting to happen.
- **No new per-frame work**: `PlayerLoadoutManager.Update` only runs its (cheap, early-out) `TryInitializeFromAccount` check until an account exists, then does nothing every frame thereafter.

## 10. Debug Tools

`CharacterDebugView` (`OnGUI`, Editor/dev-build only, `panelX: 10, panelY: 10` — Boot.unity's own corner, distinct from `MultiplayerDebugView`'s `panelX: 460` on the same GameObject tier):

- **Has Account** — live `SaveManager.HasAccount`.
- **Character ID** — the raw `CharacterId` plus its resolved display name.
- **Country ID** — the raw `GulfCountry` value.
- **Current Outfit** — the resolved display name of the equipped Outfit-slot cosmetic.
- **Loaded Cosmetics** — every one of the 10 `CosmeticSlot` values with its equipped `CosmeticId` (or "(none)"), so unused future slots are visibly "(none)" today rather than silently absent.
- **Gems**, **Owned Cosmetics** count, and **Remote Loadouts Tracked** count (exercisable today via `LocalLoopbackTransport.SimulateRemoteLoadout`, the same "debug-only simulate helper" pattern as Sprint 7's `SimulateRemoteRaceProgress`).

## 11. Code Quality

- **SOLID**: `PlayerLoadoutManager` (composition root / state owner) is separate from `AccountCreationView`/`CharacterMenuView`/`CharacterDebugView` (presentation) and `SaveManager`/`EconomyManager` (persistence/economy) — five responsibilities, five classes, same shape as Sprint 7's `RaceFinishAuthority`/`RaceStandingsTracker`/views split. Dependency Inversion: `Features.Character` depends only on `Core`/`Domain` interfaces (`IAccountRepository`, `IMatchTransport`), never on `Features.Multiplayer` or `Features.RaceFinish` concrete types; `CharacterAnimationCueService` means `PlayerController` never references `RaceFinish` and vice versa. Open/Closed: every Character/Country/Cosmetic fact is `ScriptableObject` data — adding content never requires a code change (§3, §4, §5).
- **No hardcoded values**: 12 characters, 8 countries, and 20 cosmetics are 100% `ScriptableObject`-authored data; `startingGems` is a serialized `EconomyManager` field, not a literal used elsewhere.
- **No duplicated logic**: `CharacterAnimationResolver` is the single place `PlayerMovementState`/`FinishReason` become `CharacterAnimationState` — `PlayerAnimatorDriver` never re-implements that mapping; the Traditional Outfit lookup (`GetTraditionalOutfitId`) exists in exactly one place (`CosmeticCatalogConfig`), called by exactly one initializer (`TryInitializeFromAccount`).
- **Modular / easy future expansion**: adding character #13, cosmetic #21, or country #9 is authoring one new asset (two for a country, since `CountryCatalogConfig` and the Traditional Outfit `CosmeticEntry` are separate data). Adding a real Hat/Glasses/Shoes item requires zero new code — `CosmeticSlot` already has the value and `CharacterMenuView`/`CharacterDebugView` already iterate all slots generically.

## 12. Build Verification / Compiler Status

- **Offline compile:** all **179** project `.cs` files (up from 159 after the Sprint 7 addendum) recompiled together via `dotnet build .compile_check/CompileCheck.csproj` against `.compile_check/Shims/UnityEngineShim.cs`, extended this sprint with `GUI.TextField` (two overloads) — the Account Creation Display Name field needed it, not a workaround for anything wrong with the actual game code. Two real bugs were caught and fixed by this build, not shim gaps: (1) `EconomyManager`/`CosmeticCatalogConfig` used the shim's `Mathf.Max(float, float)` overload against `int` arguments (`CS0266`) — both now use a plain ternary clamp instead; (2) `SessionManager.CreateMatch`/`JoinMatch` still referenced the old `localPlayerCountry` field name after it was renamed to `fallbackPlayerCountry` — both call sites now correctly read the new `LocalPlayerCountry` property (§2). **Result: Build succeeded, 0 errors, 0 warnings.**
- **YAML/GUID validation:** `.compile_check/validate_yaml.py` — `Boot.unity`, all 3 new catalog assets, all 12 new `CharacterXX.asset` files, and `PlayerAnimatorController.controller` all **OK**. `.compile_check/validate_yaml_refs.py` — **241** unique project `.meta` GUIDs (up from 203 after the Sprint 7 addendum); **no duplicates**; the only flagged references are the same pre-existing Unity built-in `RenderSettings` skybox/spot-cookie references documented as expected false positives since Sprint 4. `.compile_check/check_fileid_refs.py` re-run against `Boot.unity`, `Gameplay.unity`, and all 3 new catalog assets plus the Animator Controller — **"ALL 6 FILES: fileID/guid references OK (241 known guids in project)."**
- **A pre-existing gap closed, not introduced, by this sprint**: `SaveManager.cs` and `EconomyManager.cs` had existed since Sprint 1/7 respectively but had **never had a `.meta` file generated or been placed in any scene** (`Boot.unity` had no Core Manager GameObject at all before this sprint — see Sprint 4 Report §18 item 2, which explicitly deferred this). Both now have `.meta` files and are wired into `Boot.unity`'s new `CharacterSystems` GameObject (§13) because `Features.Character` is the first feature to actually depend on both at runtime; the other eight `Core.Managers.*` singletons (`GameManager`, `SceneManager`, `AudioManager`, `AnalyticsManager`, `BackendManager`, `InputManager`, `NetworkManager`, `UIManager`) remain unwired, unchanged, out of scope for this sprint (§13).

## 13. Scene & Asset Wiring

- **`Boot.unity`** — new `CharacterSystems` GameObject (root order 5), alongside (not replacing) Sprints 4–7's `MultiplayerSystems`/`WeaponSystems`/`TrapSystems`/`RaceFinishSystems`, holding: `SaveManager` (newly wired, §12), `EconomyManager` (newly wired, `startingGems: 500`), `PlayerLoadoutManager` (pointed at `CharacterCatalogConfig.asset` + `CosmeticCatalogConfig.asset`), `AccountCreationView` and `CharacterMenuView` (both pointed at `CountryCatalogConfig.asset`), and `CharacterDebugView`.
- **New assets:** `Settings/CountryCatalogConfig.asset` (8 entries), `Settings/CharacterCatalogConfig.asset` (references all 12 character assets), `Settings/CosmeticCatalogConfig.asset` (20 entries), `Settings/Characters/Character01.asset` .. `Character12.asset`.
- **`PlayerAnimatorController.controller`** — extended in place with the 3 new Win/Lose/Celebrate trigger parameters and states (§6); no new Animator Controller asset was created (§9).
- As with every prior sprint, a networked `Player.prefab` instance is still **not** placed in `Gameplay.unity` — the Character Menu's preview is a placeholder colored box (§8), and cosmetic slots have no visual attachment points yet, since there is no avatar prefab to attach them to.

## 14. Remaining TODOs

1. **No final art/audio/animation assets** — every `CharacterDefinition.PreviewPrefab`/`PortraitIcon` and every `CosmeticEntry.Icon` is unassigned; the Win/Lose/Celebrate Animator states have no real motion clips yet (same "functional now, assets later" status as every prior sprint).
2. **`FlagCatalogConfig` (Sprint 7) and `CountryCatalogConfig` (this sprint) are two separate catalogs with overlapping per-country data** (§4) — a natural, low-risk consolidation candidate once a real Lobby screen needs both flag sources reconciled; deliberately not done this sprint to avoid touching tested Sprint 7 code.
3. **`SaveManager`'s account storage is in-memory only** — not yet persisted to disk or a backend (same category of TODO as `EconomyManager.Coins`/`Gems`, both in-memory-only); a real save/backend layer is required before an account or a Gem balance survives an app restart.
4. **Eight of ten `Core.Managers.*` singletons remain unwired in any scene** (`GameManager`, `SceneManager`, `AudioManager`, `AnalyticsManager`, `BackendManager`, `InputManager`, `NetworkManager`, `UIManager`) — `SaveManager`/`EconomyManager` were wired this sprint purely because `Features.Character` needed them; wiring the rest is out of scope here (carried forward from Sprint 4 Report §18 item 2).
5. **No unicast network channel** (carried forward from Sprint 7) — `SetLocalLoadout` broadcasts to everyone; there is no privacy concern for loadout data (it is meant to be seen by every participant), so this is a non-issue for this feature specifically, but the underlying transport limitation is still worth tracking project-wide.
6. **No networked `Player.prefab` instance exists in any scene** (carried forward from Sprints 2–7) — Character Menu preview and Loaded Cosmetics have nothing to visually attach to yet.
7. **`CharacterMenuView`'s Owned/Locked list only covers the Outfit slot** — the other 9 `CosmeticSlot` values are fully modeled in `PlayerLoadout`/`CosmeticCatalogConfig`/`CharacterDebugView` (which does show all 10), but the Menu's unlock/equip UI would need one more generalization pass (loop over all slots, not just Outfit) once non-Outfit cosmetics ship real content beyond this sprint's 3 example entries (§5).
8. Carries forward all unresolved Sprint 1–7 items (Unity 6 LTS install still only Hub; ADR-0001 still Proposed, not Accepted; no Lobby/Waiting Room UI scene; ping always 0 under the loopback transport; bundle IDs; UI framework ADR; no real "use weapon" input binding; no lane-change axis; `FlagCatalogConfig.FlagEntry.FlagSprite`/`RaceFinishConfig.ChampionFanfareClip` unassigned).

## 15. Git Workflow

| Item | Value |
|---|---|
| Commit hash | recorded in the follow-up commit that captures this table's final value (same "commit, then a small follow-up commit to record its own hash" pattern used for prior sprint reports) |
| Commit message | `Sprint 8 - Characters, Countries & Customization System` |
| Branch | `main` |
| Push status | Pending push to `origin/main` (`https://github.com/q8wink1/gulf-run.git`) at report-authoring time — see the follow-up commit for final confirmation |

Sprint 8 is complete within the constraints above. Stopping here.
