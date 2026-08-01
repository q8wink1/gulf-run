# Sprint 2 — Player Controller Foundation — Sprint Report

**Role:** Lead Unity Engineer
**Scope:** Player controller foundation (movement, input, camera, animation, physics, debug). No final art.
**Status:** Complete, under the same environment constraint documented in [SPRINT-01-PROJECT-FOUNDATION.md](SPRINT-01-PROJECT-FOUNDATION.md) §0 (no licensed Unity Editor on this machine).

---

## 0. Continuation check

Verified before starting: `Client/Assets/_Project/Scripts` contained no `Player`, `Movement`, or `Camera` scripts, and `Client/Assets/_Project/Prefabs` was empty. No partial Sprint 2 work existed, so this sprint was implemented from scratch — nothing was overwritten or duplicated.

## 1. Scripts Created

All new scripts are hand-authored (same environment constraint as Sprint 1) and organized into a **pure Domain layer** plus **two Feature modules**, so movement-state logic is enginge-independent/testable and features stay decoupled from each other per [FOLDER_ARCHITECTURE.md](../02-architecture/FOLDER_ARCHITECTURE.md).

**`Scripts/Domain/`** (no `UnityEngine` dependency — confirmed by source inspection, not just by convention):

| File | Purpose |
|---|---|
| `PlayerMovementState.cs` | Enum: `Idle, Running, Jumping, Falling, Landing` |
| `PlayerInputIntent.cs` | Device-agnostic input snapshot (`JumpRequested`) — the seam where networked input will plug in later |
| `PlayerMovementStateResolver.cs` | Pure function deriving movement state from grounded/velocity facts; unit-testable and reusable on a future authoritative server |

**`Scripts/Features/PlayerController/`** (new assembly `GulfRun.Features.PlayerController`, references `GulfRun.Core`, `GulfRun.Domain`, `Unity.InputSystem`):

| File | Purpose |
|---|---|
| `PlayerMovementConfig.cs` | `ScriptableObject` — auto-run speed, jump/double-jump force, max jump count, ground check radius/layer, gravity scale. No hardcoded movement values anywhere in behaviour scripts. |
| `PlayerGroundDetector.cs` | `Physics2D.OverlapCircle`-based ground/landing detection; exposes `IsGrounded` + `Landed` event; draws a Gizmo ring in the Scene view |
| `PlayerMotor.cs` | Drives `Rigidbody2D`: constant auto-run, jump/double jump, resolves `PlayerMovementState` every `FixedUpdate` via the Domain resolver |
| `PlayerInputReader.cs` | Polls Touch / Mouse / Keyboard via the **Input System package** (not legacy `Input`), raises one `JumpPressed` event; keyboard path is compiled only under `UNITY_EDITOR`/`DEVELOPMENT_BUILD` |
| `PlayerAnimatorDriver.cs` | Maps motor state → Animator parameters (`IsGrounded`, `Speed`, `VerticalVelocity`, `JumpTrigger`) |
| `PlayerController.cs` | Composition root — wires input → motor → animator via C# events instead of the components referencing each other, so any one is swappable (e.g. a future networked input reader) |
| `PlayerDebugView.cs` | `OnGUI` readout of state / grounded / speed / vertical velocity; compiled only in Editor/dev builds |

**`Scripts/Features/CameraSystem/`** (new assembly `GulfRun.Features.CameraSystem`, references `GulfRun.Core`, `GulfRun.Domain`):

| File | Purpose |
|---|---|
| `CameraFollowConfig.cs` | `ScriptableObject` — offset, smooth time, follow-Y toggle, opt-in X/Y bounds |
| `SideScrollCameraFollow.cs` | `Vector3.SmoothDamp` follow with optional bounds clamp; takes a plain `Transform` target (falls back to the built-in `"Player"` tag at runtime) rather than referencing the PlayerController feature directly, keeping the two features decoupled and the camera multiplayer-ready (a future local-player spawner assigns `Target` explicitly) |

No script exceeds a single responsibility; cross-component wiring lives only in `PlayerController`, not scattered across the other five scripts.

## 2. Prefabs Created

**`Assets/_Project/Prefabs/Player.prefab`** — tag `Player` (built-in Unity tag, no TagManager change needed):

- `Rigidbody2D` + `CapsuleCollider2D` (placeholder capsule bounds, 0.6 × 1.2)
- `Animator` → `PlayerAnimatorController.controller`
- `PlayerController`, `PlayerMotor`, `PlayerGroundDetector`, `PlayerInputReader`, `PlayerAnimatorDriver`, `PlayerDebugView`
- Child `GroundCheck` (empty transform, offset -0.6 on Y) — the ground-detector's overlap origin
- Child `Visual` — `SpriteRenderer`, no sprite assigned, solid placeholder-orange tint (`Sprites-Default` material). Per brief: **placeholder only, no final art.**

`PlayerMotor` and `PlayerGroundDetector` both reference the single shared `PlayerMovementConfig.asset` instance (`Assets/_Project/Settings/PlayerMovementConfig.asset`) — no duplicated tuning values.

## 3. Animator Created

**`Assets/_Project/Animations/PlayerAnimatorController.controller`** — one layer, 5 states, all currently motion-less (`m_Motion: {fileID: 0}` — see §7 item 1), wired with the exact states requested:

- **Parameters:** `IsGrounded` (bool), `Speed` (float), `VerticalVelocity` (float), `JumpTrigger` (trigger)
- **States:** `Idle` (default) → `Run` (Speed > 0.1) → back to `Idle` (Speed < 0.1); **Any State** → `Jump` (JumpTrigger); `Jump` → `Fall` (VerticalVelocity < 0); `Fall` → `Land` (IsGrounded); `Land` → `Idle` (exit-time transition, 0.9)

## 4. Camera Created

`SideScrollCameraFollow` component added to the `Gameplay.unity` scene's existing **Main Camera**, referencing a new `Assets/_Project/Settings/CameraFollowConfig.asset` (offset `(0, 1, -10)`, smooth time `0.15`, Y-follow on, bounds off by default since no level extents are defined yet by any approved spec). `target` is left unassigned in the scene — it resolves automatically at runtime via the `Player` tag, so dropping a `Player.prefab` instance into any scene wires the camera with zero extra setup.

## 5. Physics Configuration

| Setting | Value | File |
|---|---|---|
| Physics mode | **2D** (`Rigidbody2D`), per this sprint's explicit instruction — supersedes Sprint 1's 3D-only assumption; both 2D and 3D settings now coexist harmlessly | `Physics2DSettings.asset` (new) |
| Gravity | `(0, -9.81)` global 2D gravity; Player's own `Rigidbody2D.gravityScale = 3` (exposed via `PlayerMovementConfig`, not hardcoded) | `Physics2DSettings.asset`, `Player.prefab` |
| Collision Detection | `Continuous` (prevents tunneling through ground at auto-run speed) | `Player.prefab` (`Rigidbody2D.m_CollisionDetection: 1`) |
| Rotation | Frozen (`m_Constraints: 4`) so the capsule doesn't tip over — standard hygiene for a runner character, not a gameplay invention | `Player.prefab` |
| Ground Layer | New **User Layer 8 = `Ground`** added to `TagManager.asset` (Sprint 1 explicitly deferred this until it was needed); `PlayerGroundDetector` checks only this layer, avoiding self-detection | `TagManager.asset`, `PlayerMovementConfig.asset` (`groundLayerMask` bit 256) |

## 6. Build Verification / Compiler Status

Same constraint as Sprint 1 (§0): no licensed Unity Editor on this machine, so no in-Editor batch-mode compile was possible. Verification performed instead:

- **Real-DLL offline compile:** all 12 new `.cs` files compiled with `dotnet build` against the actual `UnityEngine.CoreModule.dll`, `UnityEngine.Physics2DModule.dll`, and `UnityEngine.AnimationModule.dll` shipped inside the installed 2022.3.62f1 Editor. **Result: Build succeeded, 0 errors.**
- **Input System caveat:** `PlayerInputReader.cs` calls the Input System package (`Touchscreen`, `Mouse`, `Keyboard`), which is correct per Sprint 1's `activeInputHandler: 1` setting — but `com.unity.inputsystem`'s DLL cannot be resolved offline (no Editor, no package cache present on this machine, confirmed empty at `%LOCALAPPDATA%\Unity\cache\packages`). To still verify this file's syntax, it was compiled against a small **hand-written shim** reproducing only the exact members used (`Touchscreen.current`, `Mouse.current.leftButton.wasPressedThisFrame`, `Keyboard.current.spaceKey.wasPressedThisFrame`). This confirms C# syntax/type-usage correctness but is **not** a substitute for compiling against the real package — flagged as an open item (§7).
- **9 harmless `CS0649` warnings** ("field is never assigned") on every `[SerializeField]` config/reference field — this is an artifact of `dotnet build` not understanding Unity's reflection-based deserialization; the real Unity compiler suppresses `CS0649` for serialized fields, so this is expected to show **0 warnings** once opened in the Editor.
- **YAML structural validation:** `TagManager.asset`, `Physics2DSettings.asset`, `PlayerMovementConfig.asset`, `CameraFollowConfig.asset`, `PlayerAnimatorController.controller` (13 objects), `Player.prefab` (16 objects), `Gameplay.unity` (12 objects), and all 13 new/changed `.meta` files parsed successfully.
- **Cross-reference integrity check:** every internal `{fileID: N}` reference inside the controller, prefab, and scene files was verified to resolve to an actual object anchor in the same file (no dangling references) — this specifically covers the Animator's 6 hand-wired transitions and the prefab's 16-object hierarchy.
- **Not possible without a licensed Editor:** real package resolution/import, `.asmdef` graph compilation, Play Mode smoke test, and visual confirmation that the prefab/controller open without console errors. This remains the top blocker carried over from Sprint 1.

## 7. Remaining TODOs

1. **Assign real placeholder animations** once a licensed Editor is available — states are wired but `m_Motion` is intentionally left empty (`fileID: 0`) rather than hand-authoring `AnimationClip` keyframe YAML blind, which would carry meaningfully higher corruption risk than the value it adds without visual verification.
2. **Verify `PlayerInputReader` against the real `com.unity.inputsystem` package** once packages resolve (see Sprint 1 §7 item 1) — it was only offline-verified against a hand-written API shim (§6).
3. **Drop a `Player.prefab` instance into `Gameplay.unity`** and confirm the camera auto-acquires it via the `Player` tag — a first-Editor-open manual step, deliberately not done via hand-authored `PrefabInstance` YAML (high corruption risk for near-zero added value versus placing it in-Editor).
4. **`Rigidbody2D.velocity`** is used (correct for the installed 2022.3.62f1 LTS); Unity 6 LTS renames this to `linearVelocity` (deprecates but keeps `velocity` as an obsolete alias) — trivial rename to revisit once the project is actually upgraded to Unity 6, per Sprint 1 §7 item 1.
5. **Camera bounds are opt-in and unset** (`useBoundsX/Y = false`) since no map/level system defines world-space extents yet — enable once a level layout exists.
6. **Coyote-time / jump buffering** were not added — not requested by the brief, and adding unspecified feel-tuning would be inventing gameplay; flagged here in case Design wants it in a future sprint.
7. Carries forward all unresolved Sprint 1 items (Unity 6 LTS install, bundle IDs, UI framework ADR, Netcode ADR, Services scope, P020/P042 profile conflict).

---

## 8. Git Workflow

| Item | Value |
|---|---|
| Commit hash | `e0bef9447cf38829f9b7f941fccc46327ede8512` |
| Commit message | `Sprint 2 - Player Controller Foundation` |
| Branch | `main` |
| Push status | Pushed to `origin/main` (`70cad9b..e0bef94`); verified `git status` shows "up to date with 'origin/main'" and a clean working tree |

Sprint 2 is complete within the constraints above. Stopping here. Waiting for Sprint 3.
