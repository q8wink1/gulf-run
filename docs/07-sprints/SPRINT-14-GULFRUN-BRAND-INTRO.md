# Sprint 14 — GulfRun Brand Intro — Sprint Report

**Role:** Lead UI/UX Designer and Frontend Engineer
**Scope:** The official GulfRun animated Brand Intro — a 2.65-second `Boot → Intro → MainMenu` sequence (moving desert sand dunes, soft wind particles, a falcon flying across the screen then circling above the dunes, a slowly-fading-in palm tree silhouette, the GulfRun logo fading in with a premium golden shine), skippable from the device's second launch onward, with a smooth fade-to-black handoff and a real music crossfade into the Lobby; plus the one official, reusable GulfRun logo mark (Desert Dunes + Falcon + Palm Tree + Forward Motion) now placed everywhere the brief requires it inside this Unity client — Loading screen, Main Menu Top Bar, Store header, Battle Pass header — see §7.
**Status:** Complete, under the same environment constraint documented in [SPRINT-01-PROJECT-FOUNDATION.md](SPRINT-01-PROJECT-FOUNDATION.md) §0 (no licensed Unity Editor on this machine — Unity Hub is installed but no Editor version is downloaded).

---

## 0. Continuation check

Sprints 1–13 (Project Foundation through Main Menu & Lobby) are complete and were **not** rewritten. This sprint is additive everywhere except a small, deliberate set of extension points on existing files: `GameManager` now hands off Boot to the new Intro scene instead of straight to Main Menu; `SceneManager` gained `LoadIntro()`; `AudioManager` gained a music-fade primitive; `SaveManager` gained the project's first genuine cross-restart persistence (one boolean); `MainMenuBootstrapper` now fades its lobby music in instead of snapping it; and `TopBarView`/`StoreView`/`BattlePassView` each gained a few lines to draw the new shared logo mark. Same "extend the interface/vocabulary, never touch the implementation contract of unrelated features" pattern used since Sprint 4.

## 1. Architecture

Two small additions carry the whole sprint:

| Layer | Type | Responsibility |
|---|---|---|
| `Core.Branding` (new) | `GulfRunBrandMark` | The **one** official drawing routine for the GulfRun logo (medallion frame + dunes + palm tree + falcon + forward-motion chevron + optional golden shine sweep), entirely scaled off the `Rect` it is given — the same call works from a 28px Store-header badge up to a full-screen Intro reveal. Lives in `Core` (not a Feature) specifically so every screen that must show the brand draws through this one method, per the brief's own "every future design must follow the GulfRun Brand Identity" rule. |
| `GulfRun.Features.Intro` (new assembly, references only `Core`/`Domain`) | `IntroTimeline` | Every cue's timing as named constants (Dunes fade-in, Falcon fly-across/circle, Palm Tree fade-in, Logo fade-in, Shine sweep, sequence end, fade-to-black) — one source of truth every Intro view reads instead of duplicating magic numbers. |
| `Features.Intro` | `IntroSequenceController` | `SceneSingleton` composition root for `Intro.unity`: owns the shared clock (`ElapsedSeconds`), fires the three sound cues (startup/falcon-wing/shimmer) at the right time, exposes `IsSkipAvailable`/`RequestSkip()`, draws the Skip button + fade-to-black overlay, and performs the final hand-off (`SaveManager.MarkIntroSeen()` → `SceneManager.LoadMainMenu()`). |
| `Features.Intro` | `IntroBackgroundView` | Moving desert sand dunes (3 parallax layers), soft wind dust particles (18 seeded motes), and the slowly-fading-in palm tree silhouette — all reading `IntroSequenceController.ElapsedSeconds` independently, the same "widgets read shared state independently" shape Sprint 13's Main Menu views use. |
| `Features.Intro` | `IntroFalconView` | The falcon: a straight fly-across path, then a circling loop above the dunes, with a wing-flap animation — both phases pure functions of elapsed time. |
| `Features.Intro` | `IntroLogoView` | Fades `GulfRunBrandMark` in and drives its golden shine sweep at the right cue, plus the "GULFRUN" wordmark beneath it. |
| `Features.Intro` | `LoadingBrandView` | A small static placement of `GulfRunBrandMark` (+ a "Loading..." label) for `Loading.unity` — satisfies the branding requirement ahead of that scene's still-pending real async-loading flow (carried-forward TODO, §8). |

No new `Core.Services` seam was needed this sprint — unlike Sprint 13's Main Menu (a composition root that must read every other Feature's data), the Intro only ever talks to `Core.Managers` (`AudioManager`, `SaveManager`, `SceneManager`), which every Feature is already allowed to depend on directly.

## 2. Intro Animation

`Intro.unity` is a new scene inserted between `Boot` and `MainMenu` in Build Settings. Its single `IntroUI` GameObject carries `IntroSequenceController` + `IntroBackgroundView` + `IntroFalconView` + `IntroLogoView`, each reading the shared clock and drawing its own layer — nothing here needs cross-view coupling beyond the one shared `ElapsedSeconds` value:

1. **0.0s–0.3s** — desert-night sky + sand dunes + wind dust fade in together.
2. **0.1s–1.2s** — the falcon flies straight across the upper sky (`falconWingSound` fires the instant it appears).
3. **0.5s–1.1s** — a palm tree silhouette slowly fades in on the left.
4. **1.2s–2.3s** — the falcon circles in a gentle loop above the dune skyline.
5. **1.2s–2.0s** — the GulfRun logo fades in; **1.9s–2.55s** a bright highlight sweeps across it ("premium golden shine," `logoShimmerSound` fires at the sweep's start) and the "GULFRUN" wordmark appears beneath it.
6. **2.65s** (or Skip) — a 0.35s fade-to-black, then `SaveManager.MarkIntroSeen()` and a hand-off to `MainMenu.unity`.

Total runtime is **2.65 seconds**, inside the brief's "2–3 seconds." Every shape is the same flat, colored `GUI.Box`/`GUI.color` placeholder language every prior sprint's `OnGUI` screens already use — no rotation primitive exists in this project's drawing style, so the falcon's wings and the shine sweep are both deliberately flat/axis-aligned approximations rather than angled art (documented in code, §8 item 1).

## 3. Skip ("The player may skip it after the first launch")

`SaveManager` gained exactly **one** genuinely-persistent value this sprint: `HasSeenIntro` / `MarkIntroSeen()`, backed by `UnityEngine.PlayerPrefs` — a deliberate, narrow exception to this class's otherwise fully in-memory posture (documented in its own remarks), because "has this device ever launched the game before" is precisely the device-local, non-account use case `PlayerPrefs` exists for, and it is the only way to honestly satisfy "skip after the first launch" across real app restarts rather than only within one process's lifetime. On launch, `IntroSequenceController.Awake()` reads this flag once; if true, a small "Skip »" button appears (after a 0.3s grace period, so it can never be tapped by accident on the very first frame) that jumps straight to the fade-to-black. The flag is set (idempotently) the moment the Intro ends or is skipped, so it is always true from the **second** launch onward — exactly the brief's wording.

## 4. Sound

| Cue | Trigger | Field |
|---|---|---|
| Premium startup sound | Scene start | `IntroSequenceController.startupSound` |
| Soft desert wind | Scene start, loops for the whole Intro | `IntroSequenceController.desertWindAmbience` (via `AudioManager.PlayAmbient`) |
| Falcon wing sound | The instant the falcon appears (0.1s) | `IntroSequenceController.falconWingSound` |
| Golden logo shimmer | The instant the shine sweep starts (1.9s) | `IntroSequenceController.logoShimmerSound` |
| Intro music (optional) | Scene start, non-looping | `IntroSequenceController.introMusic` |

All five `AudioClip` fields are intentionally unassigned in the scene — no audio assets exist anywhere in this repo yet (§8), so every cue safely no-ops via `AudioManager`'s existing "null clip = no-op" contract rather than throwing.

**"Music fades naturally into the Lobby music"** is a real crossfade, not a description: `AudioManager` gained `FadeMusicTo(targetVolume, duration)` — a per-frame lerp of the music source's *requested* volume (added via a new `Update()` on the `Singleton`, since `AudioManager` survives the scene load and its `AudioSource` is never destroyed). `IntroSequenceController` calls `FadeMusicTo(0f, 0.35s)` the instant the transition begins; `MainMenuBootstrapper` (Sprint 13) now starts its lobby music at volume 0 and immediately calls `FadeMusicTo(lobbyMusicVolume, 0.8s)` on arrival. Because both fades run on the exact same persistent `AudioSource`, the handoff is one continuous fade rather than two hard cuts — genuinely real audio behavior, not a placeholder.

## 5. Transition ("Smooth fade into the Main Lobby. No loading stutter.")

`IntroSequenceController` draws a full-screen black `GUI.Box` whose alpha ramps 0→1 over the last 0.35s before the scene load call fires, masking the moment of the actual `SceneManager.LoadMainMenu()` call. This is the same honest caveat Sprint 13's report already carries forward: `LoadScene` here is still a direct, synchronous call (no Loading-scene/async/Addressables flow yet, §8 item 4) — the fade masks any visual popping, but a true zero-hitch guarantee needs the still-pending async flow.

## 6. Logo Design

`GulfRunBrandMark.Draw(rect, alpha01, shineProgress01)` composes, in order: a dark medallion frame with a gold ring (keeps the mark "readable even as an App Icon" — a self-contained badge shape rather than loose elements), three overlapping stepped-mound dune silhouettes, a palm tree (trunk + 3-stroke leaf fan), a falcon (body + 3-step wing chevrons on each side — the same flat, iconic silhouette language as every other bird already drawn in this project), and a small forward-motion gold chevron beneath the dunes. Everything is derived from fractions of the input `Rect`, so the exact same call produces a crisp small badge or a large hero reveal with zero separate "compact" variant to keep in sync — one drawing routine is the single source of truth for the whole brand, per the brief's Design Rule.

## 7. Branding — "Use this official logo everywhere"

| Surface | Status |
|---|---|
| Brand Intro | Full animated reveal with golden shine — `IntroLogoView` (§2). |
| Loading Screen | Small static mark + "Loading..." — `LoadingBrandView`, newly wired into `Loading.unity` (previously an empty stub scene, §8 item 4 carried forward re: the scene not yet being part of the load flow). |
| Main Lobby | A small badge to the left of the player's name in `TopBarView` — the one spot in the Top Bar with no existing content at any screen width, so it never collides with Name/Level/Rank/Currency text. |
| Store | A small badge next to the "STORE" title in `StoreView`. |
| Battle Pass | A small badge next to the season title in `BattlePassView`. |
| App Icon, Website, Social Media, Marketing Materials | **Out of scope for this Unity client** — there is no texture/sprite asset pipeline, marketing-asset pipeline, or website/social codebase in this repository to wire a logo into (the same "no final art assets" limitation every prior sprint's report already carries, just at the edge of what a game client's own repo can satisfy). `PlayerSettings`' App Icon slots specifically also require the licensed Unity Editor (§0/Sprint 1 constraint) to assign. Tracked as a Remaining TODO (§8 item 3). |

## 8. Remaining TODOs

1. **No final art/audio assets** — the logo, falcon, dunes, palm tree, and shine sweep are all flat placeholder shapes (same "no final art yet" status every previous sprint carries), and all five Intro `AudioClip` fields are unassigned.
2. **Falcon/shine are flat, axis-aligned approximations, not angled art** — this project's whole `OnGUI` drawing style has no rotation primitive (`GUIUtility.RotateAroundPivot` is not used anywhere in this codebase); a true diagonal wing/shine needs either that primitive or real sprite art.
3. **App Icon / Splash Screen / Website / Social Media / Marketing Materials are outside this repo's scope** — no texture/sprite pipeline and no marketing/website codebase exists here to wire the mark into (§7); assigning `PlayerSettings`' App Icon also needs the still-missing licensed Editor.
4. **No Loading-scene/async/Addressables scene-transition flow** — carried forward from Sprint 13 §13 item 7; `LoadIntro`/`LoadMainMenu` remain direct synchronous `LoadScene` calls, and `Loading.unity` now has a brand mark but is still not part of any actual load sequence.
5. Carries forward all unresolved Sprint 1–13 items (see those reports' own Remaining TODOs sections).

## 9. Build Verification / Compiler Status

- **Offline compile:** all **347** project `.cs` files (up from 340 after Sprint 13; 7 new this sprint — `Core.Branding.GulfRunBrandMark` and 6 new files in the new `Features.Intro` assembly) recompiled together via `dotnet build .compile_check/CompileCheck.csproj` against `.compile_check/Shims/UnityEngineShim.cs`. Shim extensions required this sprint: a minimal in-memory `UnityEngine.PlayerPrefs` stub (`GetInt`/`SetInt`/`HasKey`/`Save` — the first sprint to touch persistence that must survive across a real app restart) and `Mathf.PI`. **Result: Build succeeded, 0 errors, 0 warnings.**
- **YAML/GUID validation:** `.compile_check/validate_yaml.py` — `Intro.unity` (14 objects), `Loading.unity` (14 objects), `MainMenu.unity` (38 objects), and `EditorBuildSettings.asset` (1 object) all **OK**. `.compile_check/validate_yaml_refs.py` (extended this sprint to also cover `Intro.unity`/`Loading.unity`) — **424** unique project `.meta` GUIDs (up from 415 after Sprint 13; 9 new — 8 new script metas + the new `Intro.unity.meta`), **no duplicates**; the only flagged references across every scene are the same pre-existing Unity built-in `RenderSettings` skybox/spot-cookie references documented as expected false positives since Sprint 4. `.compile_check/check_fileid_refs.py`, re-run against `Intro.unity`/`Loading.unity`/`MainMenu.unity` — **"ALL 3 FILES: fileID/guid references OK (424 known guids in project)."**

## 10. Scene & Asset Wiring

- **`Intro.unity`** (new scene, inserted into `EditorBuildSettings.asset` immediately after `Boot.unity`) — a Main Camera (no Directional Light needed; every visual is `OnGUI`) plus one `IntroUI` GameObject carrying `IntroSequenceController`, `IntroBackgroundView`, `IntroFalconView`, `IntroLogoView`.
- **`Loading.unity`** (previously an empty stub scene — Camera + Directional Light only) — gained a `LoadingUI` GameObject carrying the new `LoadingBrandView`.
- **`MainMenu.unity`** — no new GameObjects; `MainMenuBootstrapper`'s existing component gained one new serialized field (`lobbyMusicFadeInSeconds: 0.8`).
- **`Boot.unity`** — unchanged. `GameManager`'s `Start()` now calls `SceneManager.Instance.LoadIntro()` instead of `LoadMainMenu()` — a code-only change, no scene YAML edit needed.
- **`EditorBuildSettings.asset`** — scene order is now `Boot → Intro → MainMenu → Loading → Gameplay → Results`.

## 11. Git Workflow

| Item | Value |
|---|---|
| Commit hash | _see final push verification below_ |
| Commit message | `Sprint 14 - GulfRun Brand Intro` |
| Branch | `main` |
| Push status | _see final push verification below_ |

Sprint 14 is complete within the constraints above.
