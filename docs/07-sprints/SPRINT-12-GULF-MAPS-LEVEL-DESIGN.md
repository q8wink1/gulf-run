# Sprint 12 — Gulf Maps & Level Design — Sprint Report

**Role:** Lead Level Designer
**Scope:** Six beautiful, Gulf-inspired launch race maps (Kuwait City, Riyadh, Dubai, Doha, Manama, Muscat) sharing one fair, reusable 11-section-type level-structure vocabulary (Flat Sections/Small Hills/Slopes/Bridges/Wood Platforms/Stone Platforms/Jump Platforms/Short Tunnels/Open Areas/Small Drops/Small Climbs); a data-driven per-match environment resolver (Map/Weather/Time of Day + Trap/Item-Box seeds, all randomized fresh at every Countdown); background-only landmark identity per city; ambient day/night audio per city; animated-background-element data flags; on-screen debug (Current Map/Weather/Time/Trap Seed/Item Box Seed); and the same "no final art/audio yet" honest scope every prior sprint carries — see §12.
**Status:** Complete, under the same environment constraint documented in [SPRINT-01-PROJECT-FOUNDATION.md](SPRINT-01-PROJECT-FOUNDATION.md) §0 (no licensed Unity Editor on this machine — Unity Hub is installed but no Editor version is downloaded).

---

## 0. Continuation check

Sprints 1–11 (Project Foundation through Daily Missions & Login Rewards) are complete and were **not** rewritten. This sprint extends four existing files additively (`Chunk` gained a `LevelSectionType` field; `AudioManager` gained a dedicated ambient channel; `TrapAuthority`/`ChunkContentSpawner` now re-seed from the new per-match environment seeds; `WorldGenerationConfig.asset` gained 10 new weighted chunk-prefab entries) — the same "extend the interface/vocabulary, never touch the implementation contract of unrelated features" pattern used since Sprint 4.

## 1. Architecture

A new, isolated **`GulfRun.Features.Maps`** assembly (references only `GulfRun.Core` + `GulfRun.Domain`, same "Features never reference other Features" rule as every prior Features assembly) owns map/weather/time-of-day resolution, ambient audio, animated background elements, and the debug overlay. Because `Features.EndlessRunner`'s `TrapAuthority`/`ChunkContentSpawner` must re-seed from the resolved match seeds without ever referencing `Features.Maps`, one purely-additive `Core`-layer seam keeps that dependency one-directional, the exact shape Sprints 9–11 established for Online/Store/Progression:

| Layer | Type | Responsibility |
|---|---|---|
| Domain (pure, no UnityEngine) | `MapId` | String-wrapped `readonly struct` (mirrors `CharacterId`/`CosmeticId`/`StoreItemId`) identifying one of the six launch maps. |
| Domain | `TimeOfDay` | Morning / Sunset / Night. |
| Domain | `WeatherType` | Sunny / Cloudy / LightWind / DustySky (live) + Rain / Fog / Sandstorm (future support, present in the enum and the catalog with `0` selection weight so they can never be rolled today, per the brief's "Future support" wording). |
| Domain | `LevelSectionType` (+ `LevelSectionTypeExtensions.IsPlatform()`) | The 11 brief-listed section types every map's shared chunk library is built from. |
| Domain | `BackgroundElementFlags` (`[Flags]`) | Clouds / Birds / PalmTrees / WavingFlags / CityLightsAtNight / Traffic / SeaWaves — which animated elements a given map supports, as authored data. |
| Domain | `RaceEnvironmentSeeds` | The two per-match random seeds (Trap, Item Box) the brief's Debug section requires to be displayed. |
| Domain | `MatchEnvironmentSelection` | Immutable bundle of `MapId` + `TimeOfDay` + `WeatherType` + `RaceEnvironmentSeeds` — the single value every other system reacts to. |
| Core.Services | `IMapContextProvider` / `MapContextService` (new) | Lets `Features.EndlessRunner`/`Features.Traps` read the resolved environment (and re-seed from it) without ever referencing `Features.Maps` — same static-locator shape as every other `Core.Services` seam in this project. |
| Configuration (ScriptableObject) | `MapCatalogConfig`, `WeatherCatalogConfig`, `TimeOfDayCatalogConfig` | Every map's display identity/country/palette/background-element flags/landmarks/ambient clips, every weather's selection weight/tint/fog density, and every time-of-day's selection weight/ambient light/sky colors are authored data — no map/weather/lighting number lives in code, continuing Sprint 9–11's catalog pattern. |
| Maps | `MapEnvironmentManager` (persistent `Singleton`, implements `IMapContextProvider`) | Composition root: rolls a brand-new weighted Map + Weather + Time of Day + two fresh random seeds the instant a match enters `MatchState.Countdown`, applies the resolved Time of Day/Weather to ambient lighting immediately, and raises `EnvironmentResolved` (§3, §5, §6). |
| Maps.Audio | `MapAmbientAudioController` (scene-scoped) | Plays the active map's day/night ambient clip through `AudioManager`'s new dedicated ambient channel (§7). |
| Maps.Background | `BackgroundAmbienceController` (scene-scoped) | Exposes which animated elements the active map supports and toggles night-only City Lights layers with the resolved Time of Day (§8). |
| Maps.Background | `ParallaxLayer` (reusable, generic) | Every animated background element (clouds/birds/palm trees/flags/traffic/sea waves) is just another `ParallaxLayer` instance with a different depth/speed — zero per-element special-casing (§8, Code Quality). |
| Maps | `MapDebugView` (scene-scoped `OnGUI`) | Current Map / Current Weather / Current Time / Trap Seed / Item Box Seed — the brief's exact Debug list (§10). |

This mirrors Sprints 5–11's layering exactly (Domain = rules, Core.Services = the one cross-feature seam, `Features.Maps` = the feature itself) — no new architectural pattern was invented for this sprint; it is ADR-0001's abstraction applied a fifth time, to per-match presentation/environment state.

## 2. Level Design & Level Structure

Every launch map shares **one** reusable chunk-prefab library (`WorldGenerationConfig.asset`, Sprint 3's existing weighted chunk-selection system) covering all 11 brief-listed section types — "per-map identity is presentation, not track geometry," which is what keeps six maps "fair and balanced" for free and is the Code Quality section's "reusable level components" applied literally:

| # | `LevelSectionType` | Prefab | Weight | Visual/gameplay cue |
|---|---|---|---|---|
| 1 | Flat Section | `ChunkPrefab_Default` | 3 | Baseline straight run — obstacle/coin/power-up/decoration spawn points (unchanged from Sprint 3). |
| 2 | Open Area | `ChunkPrefab_OpenArea` (**new**) | 3 | Sparse coin + two decoration spawn points only — a breathing-room straightaway between denser sections. |
| 3 | Small Hill | `ChunkPrefab_SmallHill` (**new**) | 2 | Obstacle + power-up + coin + decoration spawn points suggesting a gentle rise. |
| 4 | Slope | `ChunkPrefab_Slope` (**new**) | 2 | A visual-only ascending 4-coin trail (y: 0→1.5) tracing the slope's rise. |
| 5 | Bridge | `ChunkPrefab_Bridge` (**new**) | 2 | 3 coins + a `Decoration_Support` marker (a crossing's visual support beam). |
| 6 | Small Drop | `ChunkPrefab_SmallDrop` (**new**) | 2 | A visual-only descending 4-coin trail + decoration, mirroring the Slope. |
| 7 | Small Climb | `ChunkPrefab_SmallClimb` (**new**) | 2 | Two ascending power-up/coin pairs (y: 0.4→1.2) rewarding the climb. |
| 8 | Short Tunnel | `ChunkPrefab_ShortTunnel` (**new**) | 1.5 | 3 obstacle spawn points (denser than Flat) + a `Decoration_Roof` marker standing in for an overhead tunnel roof. |
| 9 | Wood Platform | `ChunkPrefab_WoodPlatform` (**new**) | 1 | A real elevated `BoxCollider2D` platform (§3) with a power-up on top and coin/obstacle on the ground beneath it. |
| 10 | Stone Platform | `ChunkPrefab_StonePlatform` (**new**) | 1 | A wider, lower elevated platform with two on-platform coins and a ground-level item box. |
| 11 | Jump Platform | `ChunkPrefab_JumpPlatform` (**new**) | 1 | A small, higher platform (a deliberate double-jump target) carrying an item box, plus ground coin/obstacle. |

Every prefab (new and pre-existing) keeps the exact same **continuous, unbroken flat ground `BoxCollider2D`** (`20m × 1m`, `layer 8`) Sprint 3 established — platforms and slope/hill/drop visual cues are purely additive geometry and spawn-point placement on top of that same safe floor, never a gap or a floor that could softlock/unfairly eliminate a runner. `WorldGenerationConfig`'s existing `WeightedSelector` (Sprint 3) now picks from **11** weighted entries instead of 1; Flat/Open Area are weighted highest (3) as the "everything must be smooth and fun" baseline, tunnels/platforms lowest (1–1.5) as intentional variety/challenge beats — all weights are asset data, not code.

## 3. Platform System

Wood/Stone/Jump Platforms are real physics, not decoration: each is its own `BoxCollider2D` (`layer 8`, same collision layer as the ground) positioned above the ground collider at a jumpable/double-jumpable height, with spawn points placed both **on top of** the platform (reward for taking it) and **on the ground beneath it** (the always-available, never-blocked alternate path):

| Platform | Height above ground | Width | On-platform reward | Ground-level alternative |
|---|---|---|---|---|
| Wood | 1.15m (single-jump) | 7m | Power-Up | Coin + Obstacle |
| Stone | 0.75m (low, wide) | 10m | 2× Coin | Item Box |
| Jump | 1.85m (double-jump target) | 3m (narrow, deliberate) | Item Box | Coin + Obstacle |

"Players jump onto them. Run across them. Jump down safely. Platforms never block the race unfairly" is satisfied structurally: the ground collider directly beneath every platform is never removed or gapped, so a runner who does not (or cannot) reach a platform simply keeps running underneath it exactly as on a Flat Section — platforms are always a **bonus route**, never the only route.

## 4. Map Layout & Random Systems

`MapEnvironmentManager` resolves a brand-new `MatchEnvironmentSelection` the instant a match enters `MatchState.Countdown` (strictly before `MatchState.Running`, which is when `TrapAuthority`/`ChunkContentSpawner` react — see §5 — so the resolution is always visible to them with no same-frame ordering risk):

1. **Map** — weighted-random pick across all 6 `MapCatalogConfig` entries (currently equal weight `1` each → a uniform rotation).
2. **Weather** — weighted-random pick from `WeatherCatalogConfig` (Sunny **4**, Cloudy **2**, Light Wind **2**, Dusty Sky **1**; Rain/Fog/Sandstorm present at weight **0** — "Future support," never rolled today, zero code change needed to enable them later).
3. **Time of Day** — weighted-random pick from `TimeOfDayCatalogConfig` (Morning/Sunset/Night, equal weight `1` each).
4. **Seeds** — two fresh `int` seeds (`RaceEnvironmentSeeds.TrapSeed` / `.ItemBoxSeed`) drawn from `MapEnvironmentManager`'s own `SeededRandom.FromTime()` instance.

"Trap locations change every match. Weapon boxes change every match. Players should never memorize one perfect route" is satisfied by re-seeding, not by re-randomizing weights: `TrapAuthority.ResetForNewMatch()` and `ChunkContentSpawner`'s `MatchStateChanged` handler both now construct a brand-new `SeededRandom` from `MapContextService.Current.Current.Seeds` on every `MatchState.Running` transition, so every match's actual trap/item-box spawn timing and roll sequence is fresh — same physical spawn-point layout (fair/tested), different occupant and timing every time.

Map selection is presentation-only exactly as the brief specifies ("per-map identity is presentation, not track geometry" — §2): the shared chunk library, `WeightedSelector`, and `TrapAuthority`/`ChunkContentSpawner` never branch on which map was rolled — a race on Kuwait City and a race on Muscat run the identical fair chunk-generation/trap/item-box logic, differing only in landmarks/palette/audio/lighting (§5–§8).

## 5. Landmarks & Visual Identity

`MapCatalogConfig` (6 entries, one per launch map) carries every brief-named landmark as a background-only `LandmarkEntry` (name + placeholder color + parallax depth) — explicitly never read by gameplay/collision code, only by `Features.Maps`' own presentation components:

| Map | Country | Palette | Landmarks (background only) |
|---|---|---|---|
| Kuwait City | Kuwait | Deep teal-green | Kuwait Towers, Corniche, Modern Skyline, Palm Trees |
| Riyadh | Saudi Arabia | Sandstone gold | Kingdom Tower, Al Faisaliah, Modern Riyadh Skyline |
| Dubai | United Arab Emirates | Desert amber | Burj Khalifa, Museum of the Future, Modern Dubai Skyline |
| Doha | Qatar | Maroon | Doha Corniche, Pearl Qatar, Modern Skyline |
| Manama | Bahrain | Crimson | Bahrain World Trade Center, Historic Architecture, Modern Skyline |
| Muscat | Oman | Burnt orange | Sultan Qaboos Grand Mosque, Mountains, Palm Trees |

Every map also carries a `BackgroundElementFlags` bitmask of which animated elements it supports (§8) — e.g. only the four coastal/seafront cities (Kuwait City, Dubai, Doha, Muscat) include `SeaWaves`; Riyadh (inland) and Manama (bitmask `123`, Sea Waves intentionally omitted) do not — matching the real geography the brief itself describes ("Doha Corniche," "Sultan Qaboos ... Palm Trees," Riyadh with no coastline).

## 6. Time of Day & Lighting

`TimeOfDayCatalogConfig` authors Morning/Sunset/Night as ambient-light color + intensity + sky-gradient hint colors — "Randomly choose before every race... Lighting changes only. Gameplay remains identical" is enforced structurally, not just by convention: `MapEnvironmentManager.ApplyLighting` only ever writes to `RenderSettings.ambientLight`/`.fog`/`.fogColor`/`.fogDensity`, and nothing in `Features.EndlessRunner`/`Features.Traps`/`Features.Weapons`/`Features.RaceFinish` reads any lighting/weather state at all — there is no code path by which Time of Day or Weather could affect physics, spawn timing, or race outcome.

| Time of Day | Weight | Ambient intensity | Character |
|---|---|---|---|
| Morning | 1 | 1.1 (brightest) | Warm, high-key daylight; light blue-to-cream sky gradient. |
| Sunset | 1 | 0.9 | Warm orange ambient light; purple-to-orange sky gradient. |
| Night | 1 | 0.35 (dimmest) | Cool blue ambient light; near-black sky gradient — this is also the trigger for City Lights At Night background elements (§8). |

## 7. Weather

`WeatherCatalogConfig` authors Sunny/Cloudy/Light Wind/Dusty Sky as a tint color + a `0–1` fog-density hint, applied via the same `ApplyLighting` call (§6) — purely a `RenderSettings.fog`/`.fogColor`/`.fogDensity` cosmetic change, never a gameplay multiplier:

| Weather | Weight | Fog density hint |
|---|---|---|
| Sunny | 4 (most common) | 0 (no fog) |
| Cloudy | 2 | 0.1 |
| Light Wind | 2 | 0.05 |
| Dusty Sky | 1 | 0.35 (haziest) |
| Rain / Fog / Sandstorm | 0 each | Present in the catalog, authored with sensible tint/fog values, but weight `0` — "Future support" per the brief; enabling one later is a single-number asset edit, never a code change. |

## 8. Background

`BackgroundElementFlags` (`[Flags]`) is the single per-map, per-element on/off switch — `BackgroundAmbienceController` (scene-scoped) reads the active map's flags and additionally gates `CityLightsAtNight` behind `TimeOfDay == Night`, so city-lights layers are only ever visible after dark, exactly as the brief specifies. Every actual moving element (Clouds, Birds, Palm Trees, Waving Flags, Traffic, Sea Waves) is deliberately just another instance of the one generic, reusable `ParallaxLayer` component (parallax factor 0 = fixed distant skyline → 1 = full world-scroll speed, plus an optional wrap distance for a seamless infinite scroll) reading world speed from the existing `RunSpeedService` seam the Player already consumes — zero per-element special-casing, satisfying "reusable level components" literally a second time (§2). Placing the actual `ParallaxLayer` GameObjects with final art per city is tracked in §12 (no final background art exists yet, same "data slot now, asset later" policy as every prior sprint's cosmetics/landmarks).

## 9. Audio

`AudioManager` (Sprint 1, extended) gained a **dedicated ambient `AudioSource`** (`PlayAmbient`/`StopAmbient`), independent of both the existing looping Music source (Sprint 7) and one-shot SFX — a city-ambience swap can never cut short a Victory Ceremony track, and is never itself cut short by a pickup/impact one-shot. `MapCatalogConfig.MapEntry` carries a `DayAmbientClip` and `NightAmbientClip` per map (currently unassigned placeholders — no final audio yet); `MapAmbientAudioController` (scene-scoped) swaps to the correct clip the instant `EnvironmentResolved` fires, selecting Night's clip only when the resolved `TimeOfDay == Night`, otherwise Day's — "Ambient sounds per city... Day and night variations" is real, wired, swap-ready plumbing, just without final Birds/Wind/Sea/City-ambience recordings yet (§12).

## 10. Debug Tools

`MapDebugView` (`OnGUI`, Editor/dev-build only, `panelX: 3610` — `Gameplay.unity`'s next free slot after Sprint 11's `ProgressionDebugView` at `panelX: 3160`), covering every brief-listed field exactly:

- **Current Map** — resolved `MapId`, resolved to its `DisplayName (Country)` via `MapCatalogConfig.TryGetEntry`.
- **Current Weather** — `MatchEnvironmentSelection.Weather`.
- **Current Time** — `MatchEnvironmentSelection.TimeOfDay`.
- **Trap Seed** — `MatchEnvironmentSelection.Seeds.TrapSeed`.
- **Item Box Seed** — `MatchEnvironmentSelection.Seeds.ItemBoxSeed`.

Before the first `MatchState.Countdown` of a session, the panel honestly reports "Environment not resolved yet (waiting for Match Countdown)" rather than showing stale/default values.

## 11. Performance

- **Sprite Atlases / minimal draw calls**: no new sprite art was authored this sprint (§12), so there is nothing new to atlas yet; the architecture is atlas-ready — every landmark/background element is a single flat-colored placeholder `SpriteRenderer` today, trivially swappable for one atlas-packed sprite later with zero code changes (same "data slot now, asset later" policy as Sprint 5's weapon icons).
- **Object Pooling**: no new pooled object type was introduced — `ParallaxLayer` instances are static scene decoration (created once per scene load, never spawned/despawned during a race), and trap/item-box pooling is unchanged from Sprints 5–6's existing `ObjectPoolManager` usage, now simply re-seeded per match (§4) rather than re-architected.
- **Minimal per-frame cost**: `ParallaxLayer.Update()` is a single `Vector3` subtraction + an optional wrap check, no allocation; `MapEnvironmentManager.ResolveNewEnvironment()` runs at most once per match (on `Countdown`, not per-frame); `MapDebugView.OnGUI()` follows the exact same cheap conditional-early-return pattern as every other `*DebugView` in this project.
- **60 FPS target on mobile**: no new physics, no new per-frame allocation, no new Update-tick-heavy system was added — the 10 new chunk prefabs reuse the identical `BoxCollider2D`/`SpawnPoint` shape Sprint 3 already validated at scale, just with more variety in the weighted pool.

## 12. Code Quality (SOLID / No Hardcoded Values / Modularity)

- **SOLID**: `MapEnvironmentManager` (environment resolution + lighting), `MapAmbientAudioController` (audio), `BackgroundAmbienceController` (background element on/off), and `MapDebugView` (debug) are four separate single-responsibility components, not one god-object — the same split every prior sprint's Feature composition uses. Dependency Inversion: `Features.Maps` depends only on `Core`/`Domain` (`IMatchTransport`, `IMapContextProvider`, `RunSpeedService`), never on `Features.EndlessRunner`/`Features.Traps` concrete types; conversely, `TrapAuthority`/`ChunkContentSpawner` depend only on the new `IMapContextProvider` interface, never on any `Features.Maps` concrete type.
- **No hardcoded map logic**: every map's identity/landmarks/background flags/ambient clips, every weather's tint/fog/weight, and every time-of-day's lighting/weight is `ScriptableObject`-authored data (§4–§7) — the only literal constants in code are structural (the 3 `TimeOfDay` values, the 11 `LevelSectionType` values), matching the brief's own fixed vocabulary, not tunable content.
- **Reusable level components**: one shared chunk-prefab library serves all six maps (§2); one generic `ParallaxLayer` serves every animated background element (§8); adding launch map #7 or chunk-prefab variant #12 is authoring one new catalog entry/prefab — zero code changes, exactly as `MapCatalogConfig`'s own doc comment states.
- **No final art/audio assets yet**: every landmark is a flat placeholder color, every ambient clip field is unassigned, and no `ParallaxLayer` GameObjects with final sprites are placed in `Gameplay.unity` yet — same "no final art yet" status every prior sprint's placeholder UI/cosmetics carries (§12 TODOs).
- **Offline shim**: this sprint required two small additions to `.compile_check/Shims/UnityEngineShim.cs` — a `RenderSettings` static class (`ambientLight`/`fog`/`fogColor`/`fogDensity`) and a 4-argument `Color` constructor — both minimal, additive, and exercised only by `MapEnvironmentManager.ApplyLighting`.

## 13. Remaining TODOs

1. **No final landmark/background art** (§5, §8) — every landmark and animated background element is a flat placeholder color/`ParallaxLayer` slot, not final Kuwait-Towers/Burj-Khalifa/etc. sprite art, same "no final art yet" status carried forward from every previous sprint.
2. **No `ParallaxLayer` GameObjects are placed in `Gameplay.unity` yet** — the component is built, tested-by-review, and reusable, but wiring six maps' worth of actual background layers (with real depth/speed tuning per element) is an art-asset-dependent follow-up task, tracked separately from this sprint's systems work.
3. **No final per-city ambient audio clips** (§9) — `DayAmbientClip`/`NightAmbientClip` fields exist and are wired end-to-end through `AudioManager.PlayAmbient`, but every clip reference in `MapCatalogConfig.asset` is currently `{fileID: 0}` (unassigned).
4. **Sprite Atlas packing has nothing to pack yet** (§11) — the architecture is atlas-ready, but there are no final sprites to atlas until items 1–3 above are resolved.
5. **Map selection has no player-facing vote/select screen** — `MapEnvironmentManager.ResolveNewEnvironment()` is `public` specifically so a future host-only map-vote UI can call it directly instead of only reacting to `MatchState.Countdown`, but no such UI exists yet (same category of "manual/automatic trigger only" gap as Sprint 11 §14 item 1's Special Login Event activation).
6. **No live remote config for map/weather/time-of-day weights** — "Backend controls..." is satisfied today by local `ScriptableObject` catalogs (§4), a real LiveOps config service is a drop-in swap at the catalog-reference level, not a rewrite (same category of gap Sprint 11 §14 item 7 flagged for Progression).
7. **No networked `Player.prefab` instance exists in any scene** (carried forward from Sprints 2–11).
8. Carries forward all unresolved Sprint 1–11 items (see those reports' own Remaining TODOs sections).

## 14. Build Verification / Compiler Status

- **Offline compile:** all **298** project `.cs` files (up from 280 after Sprint 11) recompiled together via `dotnet build .compile_check/CompileCheck.csproj` against `.compile_check/Shims/UnityEngineShim.cs`. Two minimal shim extensions were required this sprint (§12): a new `RenderSettings` static class and a 4-argument `Color` constructor — both needed by `MapEnvironmentManager.ApplyLighting`, the first sprint to touch scene lighting. **Result: Build succeeded, 0 errors, 0 warnings.**
- **YAML/GUID validation:** `.compile_check/validate_yaml.py` — `Boot.unity`, `Gameplay.unity`, `WorldGenerationConfig.asset`, all 3 new Maps catalog assets, and all 11 chunk prefabs (1 modified + 10 new) all **OK**. `.compile_check/validate_yaml_refs.py` — **393** unique project `.meta` GUIDs (up from 361 after Sprint 11; 18 new script metas + 1 new asmdef meta + 3 new catalog-asset metas + 10 new prefab metas, generated via `.compile_check/generate_metas.ps1`); **no duplicates**; the only flagged references are the same pre-existing Unity built-in `RenderSettings` skybox/spot-cookie references documented as expected false positives since Sprint 4. `.compile_check/check_fileid_refs.py`, re-run against `Boot.unity`/`Gameplay.unity`/`WorldGenerationConfig.asset`/the 3 Maps catalogs (which properly excludes those same built-in GUIDs) and separately against all 11 chunk prefabs — **"ALL FILES: fileID/guid references OK (393 known guids in project)"** in both runs.

## 15. Scene & Asset Wiring

- **`Boot.unity`** — new `MapSystems` GameObject (root order 9), alongside (not replacing) every prior sprint's systems GameObject, holding the single persistent `MapEnvironmentManager` (pointed at `MapCatalogConfig.asset` / `WeatherCatalogConfig.asset` / `TimeOfDayCatalogConfig.asset`).
- **`Gameplay.unity`** — new `MapSystems` GameObject holding the 3 scene-scoped Maps components: `MapAmbientAudioController` (`ambientVolume: 0.6`), `BackgroundAmbienceController`, `MapDebugView` (`panelX: 3610`, this project's rightmost debug panel to date).
- **New assets:** `Settings/Maps/MapCatalogConfig.asset` (6 maps), `Settings/Maps/WeatherCatalogConfig.asset` (7 weather entries, 3 future-support at weight 0), `Settings/Maps/TimeOfDayCatalogConfig.asset` (3 entries); 10 new chunk prefabs under `Prefabs/World/` (`ChunkPrefab_OpenArea`, `_SmallHill`, `_Slope`, `_Bridge`, `_ShortTunnel`, `_SmallDrop`, `_SmallClimb`, `_WoodPlatform`, `_StonePlatform`, `_JumpPlatform`), all registered as weighted entries in `WorldGenerationConfig.asset` alongside the pre-existing `ChunkPrefab_Default` (now also tagged `sectionType: 0`).
- As with every prior sprint, a networked `Player.prefab` instance is still **not** placed in `Gameplay.unity`.

## 16. Git Workflow

| Item | Value |
|---|---|
| Commit hash | _see below_ |
| Commit message | `Sprint 12 - Gulf Maps & Level Design` |
| Branch | `main` |
| Push status | _see below_ |

Sprint 12 is complete within the constraints above. Stopping here.
