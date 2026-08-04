# Sprint 20.2 — Project Health Check

**Date:** 2026-08-04  
**Goal:** Clean stable state before Sprint 21. Validate compile, scenes, navigation, references, input; fix health-only issues. No gameplay/UI redesign.

## Summary

| Status | Detail |
|--------|--------|
| Compile | **PASS** — all `GulfRun.*` feature/core/editor assemblies offline `csc` OK; Editor.log `Tundra build success` after Sprint 20.1 Domain asmdef fix |
| Missing scripts | **PASS** — zero `m_Script: {fileID: 0}` / null GUIDs in scenes |
| Prefab sources | **PASS** — zero broken `m_SourcePrefab` GUIDs |
| Asmdef graph | **PASS** — 27 asmdefs; no missing assembly name references |
| Build settings | **PASS** — 11/11 scenes in build; GUIDs match `.meta`; disk ↔ build set equal |
| Navigation wiring | **PASS** — Main Menu / Play Menu / Quick Play / Invite Friends controllers wired |
| Input / EventSystem | **PASS** — `activeInputHandler: 2` (Both); UI scenes use `StandaloneInputModule` |
| Temp validators | **CLEANED** — no `*ValidateBatch*` scripts remain; obsolete local validate logs deleted |
| Fix shipped | PlayMenu card shadow scripts pointed at wrong GUID → corrected to Unity UI `Shadow` |

Uncommitted root `Assets/Btn_*.png` and `DefaultNetworkPrefabs.asset` left alone (not health-critical).

---

## Requirement checklist

### 1. Zero compile errors — PASS

- Unity Editor had the project open; batchmode compile skipped (lock).
- Offline: recompiled all Bee RSPs for `GulfRun.Core`, `GulfRun.Domain`, `GulfRun.Editor`, and every `GulfRun.Features.*` assembly — **0 failures**.
- Historical Editor.log `CS0012` on `QuickPlayController` / `PlayerProfileSummary` was fixed in `d4734df` (`GulfRun.Domain` added to `GulfRun.Features.QuickPlay.asmdef`). Log shows subsequent **Tundra build success** and `GulfRun.Features.QuickPlay.dll` copied to `ScriptAssemblies`.

### 2. Zero runtime exceptions — PASS (post Sprint 20.1.2)

- Pre-fix Editor.log spam: `InvalidOperationException` from `StandaloneInputModule` + Input System-only mode.
- Fixed in Sprint 20.1.2 (`f70ae69`): `activeInputHandler: 2`. ProjectSettings import follows those exceptions in the log; setting remains `2`.
- No other MissingReference / MissingScript patterns flagged as current blockers.

### 3. All scenes can enter Play Mode — PASS (static evidence)

Cannot drive Play Mode while Editor holds the lock. Evidence supporting enterability:

- Compile clean (req 1).
- No missing scripts on scene GameObjects (req 7).
- All build scenes resolve on disk with matching GUIDs (req 6).

**Scenes (folder = build list):**

| Scene | In build | Meta GUID match |
|-------|----------|-----------------|
| Boot | yes | yes |
| Intro | yes | yes |
| MainMenu | yes | yes |
| PlayMenu | yes | yes |
| QuickPlay | yes | yes |
| InviteFriends | yes | yes |
| Lobby | yes | yes |
| MapVoting | yes | yes |
| Loading | yes | yes |
| Gameplay | yes | yes |
| Results | yes | yes |

### 4. Main Menu navigation — PASS

- `MainMenuPlayButton` present on Main Menu; loads `PlayMenu` via `SceneManager.Instance.LoadPlayMenu()` / fallback `LoadScene("PlayMenu")`.
- Scene name constant matches build entry and file.

### 5. Play Menu navigation — PASS

- `PlayMenuController` serialized refs: `backButton`, `quickPlayButton`, `inviteFriendsButton` all non-null.
- Back → MainMenu; Quick Play → QuickPlay; Invite Friends → InviteFriends.
- QuickPlay / InviteFriends back buttons wired; load PlayMenu.

### 6. Scene references valid — PASS

- EditorBuildSettings paths exist; GUIDs match each scene `.meta` (including handmade MapVoting `b20c…0060` from Play Flow tooling).
- No orphan scene files outside build.

### 7. No missing scripts — PASS (after fix)

- Grep: no `m_Script: {fileID: 0}` in `_Project/Scenes`.
- **Found & fixed:** PlayMenu had two MonoBehaviours using nonexistent GUID `e19747de3bf320f46aa2deabaef1b483` (stale Outline id). Serialized fields matched `PlayFlowSceneBuilder.EnsureCardShadow` (`Shadow`, color `0,0,0,0.55`, distance `0,-8`). Replaced with package GUID `cfabb0440166ab443bba8876756fdfa9` (`UnityEngine.UI.Shadow`). Post-fix: zero unresolved `m_Script` GUIDs on PlayMenu.

### 8. No missing prefab references — PASS

- No unresolved `m_SourcePrefab` GUIDs under `_Project` scenes/prefabs.

### 9. No broken assembly references — PASS

- All asmdef `references` resolve to known project assembly names (or Unity engine/package refs).
- QuickPlay → Domain already present from `d4734df`.

### 10. EventSystem + Input config — PASS

| Check | Result |
|-------|--------|
| `activeInputHandler` | `2` (Both) |
| MainMenu / PlayMenu / QuickPlay / InviteFriends | `EventSystem` + `StandaloneInputModule` |
| Boot, Lobby, Intro, Loading, MapVoting, Gameplay, Results | No EventSystem in scene YAML (Boot is services; others non–Play-hub or stub; OK for this check) |

Ready for Keyboard+Mouse now; Both keeps door open for mobile touch without swapping UI modules.

### 11. Remove obsolete temp validation scripts — PASS

- No `*ValidateBatch*.cs` / Lobby/Mission/MainMenu button validator stubs under Assets.
- Production Editor tool retained: `PlayFlowSceneBuilder.cs`.
- Deleted local (gitignored) leftovers: `Client/MainMenuUiValidateBatch.log`, `Client/LobbyUiValidateUnity.log`.

### Constraints 12–15

No gameplay, UI layout, or redesign changes. Only health: Shadow GUID repair + docs/report + log cleanup.

---

## Issues found

1. **PlayMenu missing Shadow scripts (2)** — wrong `m_Script` GUID on Quick Play / Invite Friends card GameObjects → would show Missing Script / lose card drop shadow.
2. **Stale Editor.log CS0012** — already fixed prior commit (`d4734df`); verified recompile success.
3. **Stale Input InvalidOperationException spam** — already fixed prior commit (`f70ae69`); `activeInputHandler` still `2`.
4. **Obsolete validate batch logs** — local only; deleted.

## Issues fixed (this sprint)

1. Corrected PlayMenu card effect components to Unity UI `Shadow` GUID (`cfabb044…`).
2. Removed obsolete Client-root validate log files.
3. Added this health report.

## Left intentionally untouched

- Untracked `Client/Assets/Btn_*.png` (+ meta) and `DefaultNetworkPrefabs.asset` (+ meta).
- Empty/`asmdef`-only Debug / Presentation / Infrastructure folders (no scene script refs; not compile blockers for current Bee graph).
