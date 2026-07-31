# Sprint 1 — Project Foundation — Sprint Report

**Role:** Lead Unity Engineer
**Scope:** Production foundation only. No gameplay implemented.
**Status:** Complete, with one environment blocker documented below (see §7).

---

## 0. Environment blocker (read first)

The Unity Editor installed on this machine (**2022.3.62f1 LTS**) has **no activated license**. Running it in batch mode fails immediately with:

```
[Licensing::Client] Error: Code 404 ... Found 0 entitlement groups and 0 free entitlements
No valid Unity Editor license found. Please activate your license.
```

Consequences and mitigations:

- **Unity 6 LTS is not installed** on this machine at all (only 2022.3.62f1 LTS). The project was built against **2022.3.62f1 LTS** instead, since it is the only Editor actually present. Upgrading to Unity 6 LTS is an open item (§7).
- The Editor could not be launched to create the project, resolve packages, generate scenes/settings, or run an in-Editor batch-mode compile. Every project file was therefore **hand-authored directly in Unity's on-disk project format** (YAML `.asset`/`.unity` files, `.asmdef` JSON, `Packages/manifest.json`).
- **Compilation was verified offline**, not through the Unity Editor: the 11 new C# scripts were compiled with the .NET SDK against the real `UnityEngine.dll` / `UnityEngine.CoreModule.dll` reference assemblies shipped inside the installed Editor (`Editor/Data/Managed/UnityEngine/`). Result: **0 errors, 0 warnings**. This confirms the scripts are syntactically and semantically valid C# against the real Unity API, but it is **not** a substitute for a full in-Editor import/compile/package-resolution pass.
- All hand-authored YAML files were structurally validated (parsed successfully as YAML; scene files use Unity's standard multi-document-with-single-header-directive convention, matching genuine Unity output).
- **Required next step before Sprint 2:** open `Client/` in a **licensed Unity 6 LTS Editor** (per [TECHNICAL_STACK.md](../02-architecture/TECHNICAL_STACK.md)) to let Unity re-serialize/upgrade the settings files, resolve/download the packages in `manifest.json`, generate `.meta` files, and run a real compile + Play Mode smoke test.

---

## 1. Folders Created

All folders below now exist under `Client/` (empty folders were given a `README.md` marker so Git preserves them — Git does not track empty directories).

**`Assets/_Project/`** (per the sprint brief's list, and per the pre-approved [FOLDER_ARCHITECTURE.md](../02-architecture/FOLDER_ARCHITECTURE.md)):

Art, Animations, Audio, Fonts, Materials, Models, Prefabs, Resources, Scenes, Scripts, Shaders, Sprites, Textures, UI, VFX, Addressables, Documentation, Editor, Settings.

**`Assets/` root** (placed here rather than under `_Project/`, per Unity engine rules and the pre-approved folder doc, which already specifies these locations):

- `Plugins/` — Unity treats this specially for native/platform plugins; must be a top-level `Assets/Plugins/` to work as intended.
- `StreamingAssets/` — Unity requires this exact path (`Assets/StreamingAssets/`) for raw build-time file copying.
- `Tests/` — split into `Tests/EditMode/` and `Tests/PlayMode/`, matching [FOLDER_ARCHITECTURE.md](../02-architecture/FOLDER_ARCHITECTURE.md) §4.
- `ThirdParty/` — also specified in the approved folder doc for vendored packages that can't be UPM packages; added for consistency even though not explicitly named in the sprint brief.

**`Scripts/` internal structure** (per the mandatory assembly table in [FOLDER_ARCHITECTURE.md](../02-architecture/FOLDER_ARCHITECTURE.md)): `Core/` (+ `Core/Managers/`), `Infrastructure/`, `Domain/`, `Features/` (placeholder, no feature modules yet), `Presentation/`, `Debug/`.

> **Note on reconciliation:** the sprint brief's folder list is the classic flat Unity `Assets/` layout. The repo already has an **approved** `FOLDER_ARCHITECTURE.md` that nests first-party content/code under `Assets/_Project/` with a layered `Scripts/` structure. Both are satisfied simultaneously: every named folder from the brief exists, placed according to the already-approved architecture rather than inventing a second, conflicting convention.

## 2. Managers Created

All ten managers exist as empty, production-ready `MonoBehaviour` singletons in `Assets/_Project/Scripts/Core/Managers/`, sharing a common `GulfRun.Core.Singleton<T>` base (persistent instance, `DontDestroyOnLoad`, safe duplicate-destroy, `OnInitialize()` template method) to avoid duplicated boilerplate across ten classes:

| Manager | File | Notes |
|---|---|---|
| GameManager | `GameManager.cs` | Also applies the 60 FPS performance target on startup (see §5) |
| SceneManager | `SceneManager.cs` | Named per brief; doc comment flags future collision with `UnityEngine.SceneManagement.SceneManager` |
| AudioManager | `AudioManager.cs` | References P035/P036 |
| UIManager | `UIManager.cs` | References P047 |
| InputManager | `InputManager.cs` | Deliberately package-free for now so the project compiles before Input System package resolution |
| SaveManager | `SaveManager.cs` | References P034/P039/P040 |
| NetworkManager | `NetworkManager.cs` | Doc comment flags future collision with `Unity.Netcode.NetworkManager` |
| BackendManager | `BackendManager.cs` | References P039/P040/P041 |
| EconomyManager | `EconomyManager.cs` | References P012/P045 |
| AnalyticsManager | `AnalyticsManager.cs` | References P044 |

Each manager contains only a doc-commented class shell, a `[DisallowMultipleComponent]` attribute, and an `OnInitialize()` stub with a `TODO` pointing at the sprint/spec where real logic is expected — no gameplay or backend logic was invented.

## 3. Scenes Created

Five scenes exist in `Assets/_Project/Scenes/`, registered in that order in `ProjectSettings/EditorBuildSettings.asset`:

1. `Boot.unity` — Main Camera only (minimal bootstrap scene).
2. `MainMenu.unity` — Main Camera + Directional Light (Unity's standard default scene setup).
3. `Loading.unity` — same default setup as MainMenu.
4. `Gameplay.unity` — same default setup as MainMenu.
5. `Results.unity` — same default setup as MainMenu.

No manager GameObjects or gameplay content were placed in any scene — that is deliberately deferred until scene-loading/bootstrap behavior is specified.

## 4. Packages Installed

Declared in `Packages/manifest.json` (see blocker in §0 — **not yet resolved/downloaded** since the Editor could not run):

| Package | Version | Requested by brief |
|---|---|---|
| `com.unity.inputsystem` | 1.7.0 | Input System |
| `com.unity.textmeshpro` | 3.0.6 | TextMeshPro |
| `com.unity.addressables` | 1.21.19 | Addressables |
| `com.unity.localization` | 1.4.5 | Localization |
| `com.unity.services.core` | 1.12.5 | Unity Services (base) |
| `com.unity.services.authentication` | 3.0.0 | Unity Services (auth, per P041) |
| `com.unity.netcode.gameobjects` | 1.7.1 | Netcode preparation |
| `com.unity.transport` | 1.3.4 | Netcode preparation (dependency) |
| `com.unity.ai.navigation`, `com.unity.test-framework`, `com.unity.timeline`, `com.unity.visualscripting`, `com.unity.collab-proxy`, `com.unity.ide.*` | — | Standard Unity 2022.3 project defaults, plus built-in engine modules (audio, physics, UI, etc.) |

Versions were chosen for compatibility with the **installed** 2022.3.62f1 LTS. They will need to be re-validated (and likely bumped) once the project is opened in Unity 6 LTS.

## 5. Settings Changed

All under `Client/ProjectSettings/` (hand-authored; see §0):

| Setting | Value | File |
|---|---|---|
| Orientation | Landscape only (both Left and Right auto-rotate; Portrait and Portrait-Upside-Down disabled) | `ProjectSettings.asset` |
| 60 FPS target | `QualitySettings.vSyncCount = 0` + `Application.targetFrameRate = 60`, applied at runtime by `GameManager.OnInitialize()` (there is no dedicated "target frame rate" project setting field in Unity) | `QualitySettings.asset` + `GameManager.cs` |
| Input System | `activeInputHandler: 1` (new Input System package only) | `ProjectSettings.asset` |
| Quality | 3 mobile-appropriate tiers (Low/Medium/High) replacing the 6-tier desktop default; Android/iPhone default to Medium; vSync off on every tier | `QualitySettings.asset` |
| Layers | Left at Unity's built-in defaults (0–7); no gameplay-specific layers added, since this sprint is architecture-only | `TagManager.asset` |
| Tags | Left at Unity's built-in defaults; no custom tags added | `TagManager.asset` |
| Physics | Default 3D `PhysicsManager` (gravity -9.81, standard solver iterations) — this project uses 3D physics per the "3D Low Poly" art direction (P048) | `DynamicsManager.asset` |
| Time | Fixed Timestep set to `0.016667` (60 Hz) instead of Unity's default `0.02` (50 Hz), to align the physics tick with the 60 FPS target | `TimeManager.asset` |
| Serialization | Force Text (`m_SerializationMode: 2`) for clean version control diffs | `EditorSettings.asset` |

Also created: `AudioManager.asset`, `GraphicsSettings.asset`, `InputManager.asset` (legacy axes, kept for compatibility even though the new Input System is active), `ProjectVersion.txt` (records the real installed Editor version).

**Not set (explicitly deferred, not invented):** company/bundle identifiers use placeholder values (`GulfRun Studio` / `com.gulfrun.game`) since no legal entity or store listing identifiers have been specified in any approved specification — flagged as an open item below.

## 6. Compilation Verification

- **What was done:** all 11 new `.cs` files (`Singleton.cs` + 10 managers) were compiled with `dotnet build` against a throwaway project referencing the real `UnityEngine.dll`/`UnityEngine.CoreModule.dll` from the installed 2022.3.62f1 Editor. **Result: Build succeeded, 0 Warning(s), 0 Error(s).**
- **What could not be done:** a genuine Unity Editor `-batchmode -quit` import/compile pass (would also validate `.asmdef` graph resolution, package-dependent code, and scene/asset integrity) — blocked by the missing Editor license (§0).
- All hand-authored YAML (`.unity` scenes, `ProjectSettings/*.asset`) and JSON (`manifest.json`, `.asmdef`) files were parsed to confirm they are structurally well-formed.

## 7. Remaining TODOs / Open Items

1. **Install and activate Unity 6 LTS** on a build/dev machine, then open `Client/` to let the Editor re-serialize settings, resolve packages, generate `.meta` files, and run a real compile + Play Mode check. This is the single most important next step before Sprint 2.
2. **Company name / bundle identifiers are placeholders** (`GulfRun Studio`, `com.gulfrun.game`) — no specification defines the legal entity or store bundle IDs; needs Producer/Design Owner input.
3. **UI framework choice (UI Toolkit vs. UGUI)** is still open per [TECHNICAL_STACK.md](../02-architecture/TECHNICAL_STACK.md) §11 ("ADR required before M1 close"); `UIManager` is intentionally UI-framework-agnostic until that ADR lands.
4. **Netcode ADR** — [UNITY_PACKAGES.md](../04-engineering/UNITY_PACKAGES.md) marks Netcode packages "Conditional" pending an ADR; Netcode for GameObjects + Transport were added per the brief's explicit "Netcode preparation" instruction, but should be confirmed/ratified by that ADR.
5. **Unity Services scope** — only `core` and `authentication` were added; Analytics/Cloud Save/etc. remain for the Backend Integration phase (P050 Future Phase 3), per [UNITY_PACKAGES.md](../04-engineering/UNITY_PACKAGES.md) ("ADR — avoid lock-in for economy").
6. **Gameplay-specific Layers/Tags** intentionally left untouched (no gameplay yet); expect this to be revisited once P002/P003 core gameplay implementation begins.
7. **P020 vs. P042 Player Profile System conflict** (flagged in P050) remains unresolved and is unrelated to this sprint's scope, but will block `PlayerProfile`-related work whenever it starts.
8. The `NetworkManager` / `SceneManager` naming collisions noted in code comments (with `Unity.Netcode.NetworkManager` and `UnityEngine.SceneManagement.SceneManager`) should be kept in mind by whoever writes the real implementations.

---

Sprint 1 is complete within the above constraints. Stopping here. Waiting for Sprint 2.
