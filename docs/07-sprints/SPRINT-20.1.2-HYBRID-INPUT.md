# Sprint 20.1.2 — Configure Hybrid Input

**Scope:** Player Settings only. Enable both legacy Input Manager and Input System Package so keyboard/mouse Play Mode works with existing `StandaloneInputModule` UI, while keeping `com.unity.inputsystem` for gameplay (`PlayerInputReader`) and future mobile touch.

## Change

| Setting | Before | After |
|---------|--------|-------|
| `activeInputHandler` | `1` (Input System Package) | `2` (Both) |

File: `Client/ProjectSettings/ProjectSettings.asset`

## Why

With Active Input Handling set to Input System only, `StandaloneInputModule` (Main Menu, Play Menu, Quick Play, Invite Friends) threw every frame:

`InvalidOperationException: You are trying to read Input using the UnityEngine.Input class...`

Both mode restores legacy `UnityEngine.Input` for UI EventSystems without rewriting gameplay or swapping UI modules.

## Constraints honored

- No gameplay script rewrite to Input System
- No UI layout / EventSystem module swaps
- No networking changes
- `com.unity.inputsystem` kept in `Packages/manifest.json`
- `DefaultNetworkPrefabs.asset` and root `Assets/Btn_*.png` left uncommitted
