# Sprint 20 — Character Selection UI Scene — Sprint Report

**Role:** Lead UI Systems Engineer  
**Scope:** New `CharacterSelection` uGUI scene matching Main Menu visual identity; Play Now / Back navigation only. No unlock, buy, animation, stats, or abilities.  
**Status:** Complete.

## 1. Scene

| Item | Value |
|---|---|
| Path | `Client/Assets/_Project/Scenes/CharacterSelection.unity` |
| Canvas | `CharacterSelectionCanvas` — Overlay, Scale With Screen Size **1920×1080**, match **0.5** |
| Background | Same sprite as Main Menu (`MainMenuBackground.png`, GUID `a18b0000000000000000000000000001`) |
| Character | `Runner.png` (temp), height **486** (~45% of 1080), `preserveAspect` |
| Build order | After `MainMenu` in `EditorBuildSettings` |

## 2. Hierarchy

```
CharacterSelectionCanvas
├── Background
├── SafeArea
├── CharacterStage
│   ├── CharacterSlotsRoot   (future multi-character)
│   ├── Platform / PlatformFill
│   └── CharacterImage
├── ArrowLeft / ArrowRight   (UI placeholders)
├── InfoPanel
│   ├── CharacterName
│   ├── Country
│   └── Status
├── BackButton               → Main Menu
└── SelectCharacterButton    (primary; no-op until later)
(+ EventSystem)
```

## 3. Navigation

| Control | Wiring |
|---|---|
| Main Menu **Play Now** | `Button` + `MainMenuPlayButton` on `PlayButtonImage` → `SceneManager.LoadCharacterSelection()` |
| **Back** | `CharacterSelectionController` → `SceneManager.LoadMainMenu()` |
| Select / arrows | Button present; listeners intentionally empty |

## 4. Constraints honored

- No unlock / purchase / animation / stats / abilities systems
- Main Menu layout/art unchanged aside from Play click wiring + EventSystem
- `DefaultNetworkPrefabs.asset` and root `Assets/Btn_*.png` left uncommitted
