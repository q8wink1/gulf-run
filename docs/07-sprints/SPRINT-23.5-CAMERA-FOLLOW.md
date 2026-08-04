# Sprint 23.5 — Camera Follow System

**Scope:** Smooth gameplay follow camera for the 3-lane runner. No obstacles, coins, race logic, multiplayer, AI, or finish system.

**Status:** Complete.

## How it works

`RunnerCameraFollow` on **Main Camera** runs in `LateUpdate`:

1. Desired position = `target.position + cameraOffset + (horizontalOffset, verticalOffset, 0)`
2. Horizontal/XZ via `Vector3.SmoothDamp` (`smoothTime`, scaled by `followSpeed`)
3. Y via a **separate** SmoothDamp — stronger damp near ground (run/slide stability), gentler catch-up in air (jump)
4. Rotation: `Quaternion.LookRotation` toward player look-at height, **roll locked to 0**, `Slerp` by `rotationSpeed`
5. Optional `CameraShake.CurrentOffset` added to position (API ready; nothing triggers yet)
6. `Camera.fieldOfView` driven from Inspector `fieldOfView`

Legacy `SideScrollCameraFollow` on the same camera is **disabled** (2D side-scroll path).

## Inspector fields

| Field | Role |
|-------|------|
| Target | Runner transform (`Player` tag auto-find fallback) |
| Follow Speed | Responsiveness scale on SmoothDamp |
| Rotation Speed | Look-at Slerp rate |
| Camera Offset | Base world offset from target |
| Vertical Offset | Extra Y on offset |
| Horizontal Offset | Extra X on offset |
| Smooth Time | Base position damp |
| Vertical Smooth Time | Airborne Y damp |
| Grounded Vertical Smooth Time | Near-ground Y damp (slide/run) |
| Field Of View | Wired to `Camera.fieldOfView` |

## Components

- `RunnerCameraFollow` — follow + look
- `CameraShake` — `Shake(intensity, duration)` public API (unused triggers)
- `CameraEffectsPlaceholder` — inactive stubs for Speed FOV / Motion Blur / Cinematic transitions

## Scene

- Main Camera wired to `RunnerPlayer` transform; start pose behind runner

## Rebuild / patch

- Menu: `GulfRun/Play Flow/Validate Gameplay Runner (Sprint 23.4/23.5)`
- Fallback: `_tools/patch_gameplay_sprint_23.py --camera`

## Constraints honored

- No gameplay systems beyond camera
- Main Menu untouched
- `DefaultNetworkPrefabs` / `Btn_*` uncommitted
