# Sprint 13 — Main Menu & Lobby — Sprint Report

**Role:** Lead UI/UX Designer and Frontend Engineer
**Scope:** The complete Main Menu / Lobby screen — an animated per-session background (random launch map + random Morning/Sunset/Night, moving clouds, flying birds, swaying palm trees), a centered breathing player preview (outfit/character/country/flag), a Top Bar (Name/Level/League/World Rank/Country Rank/Coins/Gems/Settings/Notifications), a Left Menu (Friends/Clan/Leaderboard/Missions/Battle Pass/Mail) and Right Menu (Store/Characters/Customize/Inventory/Events/Championships), a large glowing golden PLAY button with map/mode/matchmaking-ETA readout, a Daily Missions widget, a Login Reward popup, a rotating Event Banner, a Social panel (Friends Online/Clan Online/Invite/Room Code), a Voice Chat widget, a Settings panel, floating gold-dust particles, a "Modern Gulf Identity" theme (gold accents, sand neutrals, rounded-panel + gold-underline motif) shared by every element, and the first-ever `Boot → MainMenu` scene transition — see §12.
**Status:** Complete, under the same environment constraint documented in [SPRINT-01-PROJECT-FOUNDATION.md](SPRINT-01-PROJECT-FOUNDATION.md) §0 (no licensed Unity Editor on this machine — Unity Hub is installed but no Editor version is downloaded).

---

## 0. Continuation check

Sprints 1–12 (Project Foundation through Gulf Maps & Level Design) are complete and were **not** rewritten. This sprint is additive everywhere except a small, deliberate set of extension points on existing files (`IMapContextProvider` gained two new methods; `AudioManager` gained category volumes; `SceneManager` gained its first two real scene-load calls; `GameManager` gained the project's first-ever automatic Boot→MainMenu transition; nine existing `Features.*` manager/view classes gained a one-interface seam each so the Main Menu can read/open them) — the same "extend the interface/vocabulary, never touch the implementation contract of unrelated features" pattern used since Sprint 4.

## 1. Architecture

A new, isolated **`GulfRun.Features.MainMenu`** assembly (references only `GulfRun.Core` + `GulfRun.Domain`) is the one deliberate exception to "Features never reference other Features": it is this project's **UI composition root**, and a lobby screen that shows Friends/Missions/Store/Characters/Events data by definition needs to read from every one of those Features. Rather than referencing those assemblies directly (which would violate the dependency rule for everyone else), **11 new `Core.Services` seams** were added — the exact same static-locator shape every prior sprint's cross-feature seam already uses (`ICosmeticGrantService`, `IBattlePassXpGrantService`, `IMapContextProvider`, etc.) — so `Features.MainMenu` only ever depends on `Core`/`Domain`, and every other Feature keeps depending on nothing new:

| Seam | Purpose |
|---|---|
| `ILocalProfileProvider` / `LocalProfileProviderService` | Player Name/Level/League/Ranks/Coins/Gems/Outfit/Country for the Top Bar + Player Preview. Implemented by `Features.Online.Profile.ProfileManager`. |
| `IDailyMissionsPreviewProvider` / `DailyMissionsPreviewService` | The 3 active missions + claim, for the Daily Missions widget. Implemented by `Features.Progression.Missions.MissionManager`. |
| `ILoginRewardStatusProvider` / `LoginRewardStatusService` | Streak status + claim, for the Login Reward popup. Implemented by `Features.Progression.Login.LoginRewardManager`. |
| `IFriendsSummaryProvider` / `FriendsSummaryService` | Total/Online friend counts, for the Social panel. Implemented by `Features.Online.Friends.FriendManager`. |
| `IMatchLobbySummaryProvider` / `MatchLobbySummaryService` | Lobby state + Start/Cancel match, for the PLAY button and Room Code. Implemented by `Features.Multiplayer.Session.SessionManager`. |
| `INotificationSummaryProvider` / `NotificationSummaryService` | Unread badge count, for the Top Bar bell icon. Implemented by `Features.Online.Notifications.NotificationManager`. |
| `IEventBannerSource` / `EventBannerRegistry` | Many-to-one: any manager can contribute rotating banner text. Implemented by `LoginRewardManager` (Ramadan/National Day/etc. Special Events), `ChampionshipManager` (Championships/Country Events), `BattlePassManager` (current season), `StoreManager` (active Special Offers). |
| `IMenuScreenOpener` / `MenuScreenRouter` | Many-to-one registry so a Left/Right Menu button can open the right existing screen (`FriendListView`, `LeaderboardView`, `MissionsView`, `StoreView`, `InventoryView`, `CharacterMenuView`, `NotificationView`, the new `ChampionshipsView`, the new `BattlePassView`) without `Features.MainMenu` ever holding a reference to any of their concrete types. |
| `IMapContextProvider` (extended) | Two new methods — `ResolveMapDisplayName(MapId)` and `ResolveNewEnvironment()` — let the lobby background and bottom info strip reuse Sprint 12's real map/weather/time-of-day resolver as their single source of truth instead of a second, disconnected random picker. |

| Layer | Type | Responsibility |
|---|---|---|
| Domain (pure, no UnityEngine) | `RoomCode` (+ `RoomCodeGenerator`) | 6-character alphanumeric private-match code (visually unambiguous alphabet — no `0/O/1/I`). |
| Domain | `MatchmakingEtaEstimator` | Honest heuristic estimated-wait-time formula for the bottom info strip. |
| Domain | `PlayerLevelRules` | Placeholder Level/XP/XP-to-next-level formula derived from `PlayerMatchStatistics.MatchesPlayed` (no real XP-earning economy exists yet — see §13). |
| Domain | `GameMode` | `QuickRace` today — the enum vocabulary the bottom info strip's "Current game mode" reads from, ready for more modes with zero code changes elsewhere. |
| Domain | `VoiceChatMode` | Muted / OpenMic / PushToTalk. |
| Domain (extended) | `PlayerProfileSummary`, `NotificationType` | `PlayerProfileSummary` gained `Level`/`CurrentXp`/`XpRequiredForNextLevel`; `NotificationType` gained nothing structurally but is now fully covered by the Top Bar's badge count path. |
| Features.MainMenu.UI | `MainMenuTheme` | The single "Modern Gulf Identity" palette (gold/sand/desert-night) + shared `GUIStyle`/panel/gold-underline factory every view in this sprint draws from — the same "one design-system file" pattern this project already applies via `Core.Managers`. |
| Features.MainMenu.UI | `ButtonPressAnimator` | Reusable 0.12s "compress-then-release" click-feedback value type — one struct instead of copy-pasted press-timer fields on every button-owning view. |
| Features.MainMenu | `MainMenuBootstrapper` | Composition-root `Start()`: kicks off lobby music + ambient city sound through `AudioManager` (both clip fields intentionally unassigned — no audio assets exist in this repo yet, see §13). |
| Features.MainMenu | `MainMenuDebugView` | FPS / Current Lobby / Current Background / Player ID / Network Status — this sprint's brief-mandated Debug list. |

This mirrors Sprints 5–12's layering exactly (Domain = rules, Core.Services = the cross-feature seams, `Features.MainMenu` = the feature itself) — no new architectural pattern was invented; it is ADR-0001's abstraction applied a sixth time, with one explicit, documented exception (the composition root) rather than a silent rule violation.

## 2. Background

`LobbyBackgroundView` draws a full-screen animated sky using `MainMenuTheme`'s per-`TimeOfDay` gradient pair (Morning/Sunset/Night — same enum Sprint 12 introduced), a sun-or-moon disc, 4 clouds drifting at staggered lanes/speeds, 3 birds with a small sine-wave bob, and 4 swaying palm trees (canopy sways, trunk stays planted) over a sand/night ground strip — same "no final art yet" placeholder posture (flat `GUI.Box` shapes) every prior sprint's `OnGUI` screens carry. Deliberately reuses Sprint 12's real `IMapContextProvider` instead of rolling its own random map: the very first `ResolveNewEnvironment()` call this Main Menu session makes becomes both "what the lobby looks like" *and* "BOTTOM: Current selected map" (`PlayButtonView`) — one honest source of truth instead of two disconnected random pickers that could disagree, and re-entering the menu mid-session (e.g. after Leave Match) does not re-roll the background under the player's feet.

`FloatingParticlesView` renders a field of small drifting gold-dust motes ("Floating particles" — brief's Animations section), each with its own seeded looping drift path and gentle fade, using the existing `SeededRandom`/`CelebrationAnimation.EvaluateOffset` utilities — no new animation primitive was introduced.

## 3. Player

`PlayerPreviewView` stands a placeholder silhouette centered in the lower-middle of the screen with a slow vertical "breathing" scale (`CelebrationAnimation.EvaluateOffset`) and an occasional small horizontal step every 2.5–5s ("random small movements," not a continuous wobble) — reading the player's current Character/Outfit/Country through `ILocalProfileProvider` so `Features.MainMenu` never references `Features.Character`/`Features.Online` directly. The national flag is a colored placeholder resolved from `CountryCatalogConfig` (the same catalog `CharacterMenuView`'s own flag picker already uses), labeled with the country's short code. A silhouette box stands in for the real stylized-3D character model — same "no final art yet" placeholder every other screen's character/outfit preview already uses (§13).

## 4. Top Bar

`TopBarView` spans the full screen width: Player Name + Level/XP (left), League + World/Country Rank (center-left), Coins/Gems chips with colored icon placeholders (center-right), and Settings (⚙) / Notifications (🔔, live unread badge) buttons (right) — reading exclusively through `ILocalProfileProvider`/`INotificationSummaryProvider`, opening Settings directly (`SettingsView.Instance?.Open()` — both live in `Features.MainMenu`, no seam needed) and Notifications through `MenuScreenRouter`.

## 5. Left Menu & Right Menu

`LeftMenuView` (Friends/Clan/Leaderboard/Missions/Battle Pass/Mail) and `RightMenuView` (Store/Characters/Customize/Inventory/Events/Championships) are two vertical button stacks anchored to the screen edges. Every button with a real screen behind it routes through `MenuScreenRouter.TryOpen(MenuScreen.X)`; **Clan** and **Mail** have no backend system anywhere in the project (no Clan/Mail Feature exists — §13), so their buttons open a small, honest 2.5-second "— Coming Soon!" toast instead of silently doing nothing. **Customize** intentionally opens the same screen as **Characters** — `CharacterMenuView` already covers outfit selection (Sprint 8), so this is not a second near-duplicate panel.

## 6. Bottom / PLAY Button

`PlayButtonView` draws a centered info strip ("Map: *X* • Mode: QuickRace • *ETA*") above a large gold `PLAY` button with an animated pulsing glow (`CelebrationAnimation.EvaluateOffset`-driven halo, brighter/larger on the beat) and the shared `ButtonPressAnimator` compress-on-click feedback. Clicking it drives `IMatchLobbySummaryProvider` end-to-end with zero compile-time reference to `Features.Multiplayer`:

1. **Not in a match:** `PLAY` → `lobby.StartQuickMatch(nickname)` (creates/joins via `SessionManager`); label becomes `SEARCHING...` while matchmaking, with a `Cancel` secondary button.
2. **In a match (lobby filled):** label becomes `START RACE` → `SceneManager.Instance.LoadGameplay()` — the first real scene-load call this project has ever made from gameplay-adjacent UI.

The ETA line uses Domain's new `MatchmakingEtaEstimator` — an honest heuristic (there is no real backend matchmaking queue yet, §13), not a fabricated countdown.

## 7. Daily Missions & Login Reward

`DailyMissionsWidget` (bottom-left, above the Left Menu) lists up to 3 active missions with a name, a gold progress bar, and — once complete — a `Claim` button, reading/writing exclusively through `IDailyMissionsPreviewProvider`. `LoginRewardPopup` shows a centered modal with a bobbing gold "chest" placeholder, the current streak day (plus any active Special Event label — Ramadan/Eid/National Day/Summer/Winter, from Sprint 11), and a `Claim Reward` button, reading/writing through `ILoginRewardStatusProvider`; it is dismissible without claiming (an `X` button) so a player is never blocked from the rest of the lobby, and — like every other info popup in this assembly — simply re-appears next session until actually claimed.

## 8. Event Banner

`EventBannerView` is a thin rotating strip beneath the Top Bar cycling every ~3.5s through whatever messages are currently registered — a brand-new **many-producer, one-consumer** `IEventBannerSource`/`EventBannerRegistry` pattern lets `LoginRewardManager` (Ramadan/National Days/other Special Events), `ChampionshipManager` (active Championships + Country Events), `BattlePassManager` (current season), and `StoreManager` (active Special Offers) each contribute their own messages with zero knowledge of each other or of `Features.MainMenu` — satisfying the brief's full "Ramadan / National Days / Battle Pass / Limited Offers / Special Events" list from four independent, already-existing systems rather than one new hardcoded list.

## 9. Social & Voice Chat

`SocialPanelView` (bottom-right, above the Right Menu) shows "Friends Online: *X*/*Y*" (`IFriendsSummaryProvider`), an honest "Clan Online: 0/0" (no Clan Feature exists — same gap as the Left Menu's Clan button, §13), the local player's `RoomCode` when hosting (`IMatchLobbySummaryProvider`), and an `Invite Friends` button that opens the Friends screen (no push-notification/deep-link channel exists for an offline friend yet — a gap already flagged in Sprint 9's report, still unresolved). `VoiceChatWidget` (bottom-left corner) shows a mic icon that cycles Muted → Open Mic → Push-to-Talk on click, backed by the new `SettingsManager.VoiceChatMode` — a real, wired UI control surface with **no actual microphone capture/transport** behind it yet (§13), the same "UI wired, backend still a TODO" honesty this project already applies to Friend Invites and Special Offers.

## 10. Settings

`SettingsView` (opened from the Top Bar gear icon) is a centered panel with 4 volume sliders (Master/Music/SFX/Ambient, each live-forwarded to the new `SettingsManager`/`AudioManager` category-volume plumbing, §11) and a "Cycle Mode" button for Voice Chat Mode. Settings are in-memory only for this sprint — no `PlayerPrefs`/save-file wiring exists in the project yet for settings specifically (§13).

## 11. Animations & Audio

- **Smooth transitions / soft scaling / button feedback**: the shared `ButtonPressAnimator` struct (0.12s ease-out compress-then-release) is used identically by every clickable element across all 9 new interactive views (Top Bar's 2 icon buttons, both side menus' 12 buttons, PLAY + Cancel, 3 mission Claim buttons, the Login Reward Claim button, the Invite Friends button, the mic toggle) — one small reusable value type, not 20 copy-pasted press timers.
- **Character breathing**: `PlayerPreviewView`'s silhouette scale (§3). **Floating particles**: `FloatingParticlesView` (§2). **Animated glow**: the PLAY button's pulsing halo (§6). All four reuse the exact same `CelebrationAnimation.EvaluateOffset` sine helper Sprint 7 introduced for Victory Ceremony bounce/pulse — no new oscillation primitive was written this sprint.
- **Audio**: `AudioManager` (Sprint 1, extended this sprint) gained **Master/Music/SFX/Ambient category volumes** — every source's live `.volume` is always `requestedVolume × categoryVolume × masterVolume`, so a Settings-panel slider change takes effect immediately on whatever is already playing, with zero per-call-site volume math for the many pre-existing `PlayOneShot`/`PlayMusic`/`PlayAmbient` callers. `MainMenuBootstrapper.Start()` calls `PlayMusic`/`PlayAmbient` for "Soft Gulf inspired music" / "ambient city sounds," but both `AudioClip` fields are intentionally unassigned in the scene — no audio assets exist anywhere in this repo yet (§13), so this safely no-ops rather than throwing or playing silence indefinitely.

## 12. Scene Flow (Boot → MainMenu → Gameplay)

This is the **first sprint in the project to make any scene actually load another scene**. `SceneManager` (Sprint 1, a `Singleton` stub since then) gained its first two real bodies — `LoadMainMenu()`/`LoadGameplay()`, both a direct `UnityEngine.SceneManagement.SceneManager.LoadScene(name)` call (no Loading-scene/async/Addressables handoff yet, §13) — and `GameManager` (also a Sprint 1 stub) gained a `Start()` that calls `LoadMainMenu()` exactly once, but only when the active scene is `"Boot"` (so it can never accidentally re-fire if `GameManager` is ever inspected mid-session). Because every `Singleton<T>` on `Boot.unity`'s existing `CharacterSystems`/`OnlineSystems`/`StoreSystems`/`ProgressionSystems`/`MapSystems`/`MultiplayerSystems` GameObjects already calls `DontDestroyOnLoad(gameObject)` (Sprint 1's `Singleton<T>` base class marks the **whole GameObject**, not just the calling component), every persistent manager — plus the plain-`MonoBehaviour` `CharacterMenuView` that happens to share a GameObject with `Singleton`s — survives the Boot→MainMenu transition automatically, with zero new wiring required for them. `MainMenu.unity`'s own scene-scoped screens (`FriendListView`, `LeaderboardView`, `NotificationView`, `MissionsView`, `StoreView`, `InventoryView`, the new `ChampionshipsView`, the new `BattlePassView`, `SettingsView`) are each a **fresh, scene-local instance** — the same "one instance per scene, `SceneSingleton`/plain `MonoBehaviour`, never `DontDestroyOnLoad`" pattern `Gameplay.unity`'s existing `OnlineUI`/`StoreUI`/`ProgressionUI` GameObjects already use, so the Main Menu is fully self-sufficient and does not depend on `Gameplay.unity` ever having been loaded.

Two new reused-screen views were added this sprint specifically to cover Right Menu entries with no existing screen: `ChampionshipsView` (`Features.Online.Championships`, tabs for "Championships" + "Events," reading `ChampionshipManager` — the same live data the Event Banner already surfaces) and `BattlePassView` (`Features.Store.BattlePass`, tier list + claim, reading `BattlePassManager`). Both implement `IMenuScreenOpener` exactly like every other reused screen this sprint touched (`FriendListView`, `LeaderboardView`, `NotificationView`, `MissionsView`, `StoreView`, `InventoryView`, `CharacterMenuView`), each of which gained a minimal public `Open()`/`Close()` pair plus `MenuScreenRouter` registration this sprint (they previously only had an internal `_open` flag with no external control surface).

## 13. Remaining TODOs

1. **No final UI/character/background art** — every panel, button, icon (Coins/Gems/mic/bell/gear), silhouette, landmark, and particle is a flat placeholder shape, same "no final art yet" status every previous sprint carries.
2. **No final audio assets** — `AudioManager`'s Music/Ambient/SFX category-volume plumbing is real and wired end-to-end, but every clip reference (`MainMenuBootstrapper.lobbyMusic`/`.ambientCitySound`) is `{fileID: 0}` (unassigned); button-click SFX has no clip wiring yet either.
3. **No real microphone capture/transport** — `VoiceChatWidget`/`SettingsManager.VoiceChatMode` is a fully wired UI control surface with nothing behind it (§9).
4. **No Clan Feature and no Mail Feature exist anywhere in the project** — `LeftMenuView`'s Clan/Mail buttons show an honest "Coming Soon" toast, and `SocialPanelView`'s "Clan Online" always reads `0/0`, rather than either silently doing nothing or fabricating fake data.
5. **Settings do not persist across app restarts** — no `PlayerPrefs`/save-file wiring exists for `SettingsManager` yet (Sprint 1's `SaveManager` covers Account/Progress only).
6. **`PlayerLevelRules` is a placeholder formula** (Matches Played → Level), not a real XP-earning economy — no gameplay system currently grants XP toward a level (carried forward alongside Sprint 9's own "no networked player avatar" gap).
7. **No Loading-scene/async/Addressables scene-transition flow** — `SceneManager.LoadGameplay/LoadMainMenu` are direct synchronous `LoadScene` calls; the already-declared `Loading.unity` scene is not yet wired into the flow.
8. **No push-notification/deep-link channel for offline Friend Invites** (carried forward from Sprint 9's own Remaining TODOs, still unresolved).
9. **No networked `Player.prefab` instance exists in any scene** (carried forward from Sprints 2–12).
10. Carries forward all unresolved Sprint 1–12 items (see those reports' own Remaining TODOs sections).

## 14. Build Verification / Compiler Status

- **Offline compile:** all **340** project `.cs` files (up from 298 after Sprint 12; 42 new this sprint — 17 in the new `Features.MainMenu` assembly, 17 new `Core.Services` seam files, 5 new `Domain` types, `Core.Managers.SettingsManager`, and the new `ChampionshipsView`/`BattlePassView`) recompiled together via `dotnet build .compile_check/CompileCheck.csproj` against `.compile_check/Shims/UnityEngineShim.cs`. Shim extensions required this sprint (the first sprint to touch scene loading and several new `GUI`/`GUIStyle` members): a minimal `UnityEngine.SceneManagement.SceneManager` stub (`LoadScene` × 2, `GetActiveScene`) + a `Scene` struct, `GUIStyle.wordWrap`, `GUI.HorizontalSlider`, `GUI.Toggle`, and `Mathf.Approximately`. Two small compile fixes were needed along the way: a missing `using UnityEngine;` in the new `SettingsManager` (`DisallowMultipleComponent`), and `PlayButtonView`/`MainMenuDebugView` initially read `MatchEnvironmentSelection.MapId` instead of its real property name, `.Map`. **Result: Build succeeded, 0 errors, 0 warnings.**
- **YAML/GUID validation:** `.compile_check/validate_yaml.py` — `Boot.unity` (63 objects) and `MainMenu.unity` (38 objects) both **OK**. `.compile_check/validate_yaml_refs.py` (extended this sprint to also cover `MainMenu.unity`) — **415** unique project `.meta` GUIDs (up from 393 after Sprint 12; 22 new script metas generated for every new component-bearing script), **no duplicates**; the only flagged references in all three scenes are the same pre-existing Unity built-in `RenderSettings` skybox/spot-cookie references documented as expected false positives since Sprint 4. `.compile_check/check_fileid_refs.py`, re-run against `Boot.unity`/`MainMenu.unity`/`Gameplay.unity` — **"ALL 3 FILES: fileID/guid references OK (415 known guids in project)."** A local file-ID-uniqueness pass additionally confirmed zero duplicate `&fileID` anchors within either touched scene file, and every `m_Component` list's entry count was hand-verified against its GameObject's actual component block count.

## 15. Scene & Asset Wiring

- **`Boot.unity`** — new `CoreSystems` GameObject (root order 10): the project's first-ever placement of `AudioManager`, `GameManager`, `UIManager`, `SceneManager`, and the new `SettingsManager` into any scene (all five existed as `Singleton` stubs since Sprint 1 but had never been instantiated anywhere — the same category of gap Sprint 8's report flagged for `SaveManager`/`EconomyManager` before that sprint fixed it). Component order is deliberately `AudioManager → GameManager → UIManager → SceneManager → SettingsManager` so `SettingsManager.OnInitialize()`'s `AudioManager.Instance` read is never null. `OnlineSystems` also gained a 9th component, `ProfileManager` (existed since Sprint 9 but was never placed in a scene either) — the Top Bar/Player Preview's `ILocalProfileProvider` seam needs a live instance to read from.
- **`MainMenu.unity`** (previously a stub scene with only a Camera + Directional Light) — two new root GameObjects: **`MainMenuUI`** (14 components — `MainMenuBootstrapper`, `LobbyBackgroundView`, `FloatingParticlesView`, `PlayerPreviewView`, `TopBarView`, `LeftMenuView`, `RightMenuView`, `PlayButtonView`, `DailyMissionsWidget`, `EventBannerView`, `SocialPanelView`, `VoiceChatWidget`, `LoginRewardPopup`, `MainMenuDebugView` at `panelX: 4060`, this project's rightmost debug panel to date since Boot.unity's persistent `CharacterDebugView` at `panelX: 10` is also visible here) and **`MainMenuScreens`** (9 components — fresh scene-local instances of `FriendListView`, `LeaderboardView`, `NotificationView`, `MissionsView`, `StoreView`, `InventoryView`, the new `ChampionshipsView`, the new `BattlePassView`, and `SettingsView`).
- As with every prior sprint, a networked `Player.prefab` instance is still **not** placed in `Gameplay.unity`.

## 16. Git Workflow

| Item | Value |
|---|---|
| Commit hash | `eb607f140169f64def6a79610704b0fc2b32ea59` |
| Commit message | `Sprint 13 - Main Menu & Lobby Design` |
| Branch | `main` |
| Push status | Pushed to `origin/main` (`9ef226b..eb607f1`); `git status` confirms "Your branch is up to date with 'origin/main'" and a clean working tree. |

Sprint 13 is complete within the constraints above. Stopping here.
