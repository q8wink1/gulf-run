# Sprint 17 — Main Menu UI Hierarchy — Sprint Report

**Role:** Lead UI Systems Engineer  
**Scope:** Production-ready Main Menu UGUI hierarchy (placeholder containers only) on `MainMenu.unity`, ready for final art integration. No gameplay or script changes.  
**Status:** Complete.

---

## 0. Continuation check

Sprints 1–16 remain untouched. Existing OnGUI Main Menu (`MainMenuUI` / `MainMenuScreens`) is preserved. This sprint only **adds** `MainMenuCanvas` beside those roots.

## 1. Hierarchy

```
MainMenuCanvas                    (Canvas Overlay, sortingOrder 10)
├── Background                    stretch full screen
├── CurrencyBar                   top strip (height 96)
├── Logo                          top center
├── PlayerCard                    top-left
├── LeftMenu                      left middle
│   ├── Friends
│   ├── Clan
│   ├── Missions
│   └── Rankings
├── RightMenu                     right middle
│   ├── Lobby
│   ├── Store
│   └── Settings
├── CharacterRoot                 center character display
│   └── Play                      bottom of character area
└── PopupRoot                     stretch overlay (empty)
```

## 2. CanvasScaler

Mirrors Lobby: **Scale With Screen Size**, reference **1920×1080**, match **0.5**.

## 3. Constraints honored

- No `.cs` script changes
- No gameplay / MissionManager / SessionManager changes
- Existing OnGUI objects not deleted or disabled
- Button placeholders are named RectTransforms with muted Image placeholders for Editor visibility (not wired)

## 4. Scene file

- `Client/Assets/_Project/Scenes/MainMenu.unity` — additive `MainMenuCanvas` tree only

## 5. Verification

- Unity batchmode opened `MainMenu.unity`: hierarchy present, CanvasScaler 1920×1080 match 0.5, existing `MainMenuUI` / `MainMenuScreens` preserved — **0 failures**
- StandaloneWindows64 build: **Succeeded**, **0 errors**, 11 pre-existing warnings

## 6. Git

| Item | Value |
|---|---|
| Branch | `main` |
| Commit | `88fc520e493727f78a15a0ea756779f7d76b704c` |
| Push | `origin/main` |
