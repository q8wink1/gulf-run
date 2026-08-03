# Sprint 17.2 — Import Final Main Menu Art — Sprint Report

**Role:** Lead UI Systems Engineer  
**Scope:** Production-ready empty Image placeholders on MainMenuCanvas with art folder structure. No gameplay/script changes.  
**Status:** Complete.

## 0. Continuation

Sprint 17.1 layout anchors preserved via containers (TopLeft, LeftMenu, TopRight, RightMenu, PopupRoot, SafeArea). Sprint 17.2 Image naming applied.

## 1. Hierarchy

```
MainMenuCanvas                    (Canvas Overlay, sortingOrder 10)
├── Background                    stretch full screen (Image, empty, preserveAspect)
├── GulfRunLogo                   center
├── CharacterImage                center-bottom
├── TopLeft                       top-left fixed
│   └── PlayButtonImage           raycast on
├── LeftMenu                      left middle
│   ├── LobbyButtonImage
│   ├── FriendsButtonImage
│   └── ClanButtonImage
├── TopRight                      top-right fixed
│   └── PlayerCardImage
├── RightMenu                     right middle
│   ├── MissionsButtonImage
│   ├── StoreButtonImage
│   ├── SettingsButtonImage
│   └── RankingsButtonImage
├── PopupRoot                     stretch overlay
└── SafeArea                      stretch
```

## 2. Image rules

- Empty sprite (`None`)
- Preserve Aspect enabled
- Raycast Target: buttons on; non-buttons off
- No Button onClick / no system wiring

## 3. Art folders

- `Assets/_Project/UI/MainMenu/Background`
- `Assets/_Project/UI/MainMenu/Buttons`
- `Assets/_Project/UI/MainMenu/Character`
- `Assets/_Project/UI/MainMenu/Logo`

## 4. Constraints honored

- No gameplay `.cs` changes committed
- Existing OnGUI `MainMenuUI` / `MainMenuScreens` preserved
- Obsolete Sprint 17 nodes removed (CurrencyBar, CharacterRoot, etc.)

## 5. Verification

- Unity batchmode: hierarchy + CanvasScaler + folders — PASS, 0 console failures
- StandaloneWindows64 build: Succeeded, 0 errors

## 6. Git

| Item | Value |
|---|---|
| Branch | `main` |
| Commit | (filled after push) |
| Push | `origin/main` |
