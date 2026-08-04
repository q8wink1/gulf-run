# Sprint 21.1 — Lobby UI Foundation

**Scope:** Premium Lobby screen visual layout + temporary navigation only. No matchmaking, host, kick, ready logic, networking, or multiplayer sync.

**Status:** Complete.

## Naming split

| Scene | Role |
|-------|------|
| `Lobby.unity` | Pre-race matchmaking lobby (Sprint 14 OnGUI) — **unchanged** |
| `LobbyScreen.unity` | New premium UGUI foundation (this sprint) |

`SceneManager.LoadLobby()` still loads `Lobby`. New `LoadLobbyScreen()` loads `LobbyScreen`.

## Navigation

| From | Action | To |
|------|--------|----|
| QuickPlay | After placeholder search completes (`GoToLobby`) | **LobbyScreen** (temporary; was `Lobby`) |
| LobbyScreen | Back (top-left) | **PlayMenu** |
| QuickPlay | Back / Cancel | PlayMenu (unchanged) |

**Back choice:** LobbyScreen → **PlayMenu** (not QuickPlay). Returning to QuickPlay would re-fire `StartQuickMatch`. Back also calls `MatchLobbySummaryService.CancelOrLeaveMatch()` so a Quick Play session started before this UI-only screen does not immediately bounce the player back into LobbyScreen.

Ready / Play footer buttons are visual placeholders only (Play disabled). No gameplay logic.

## Scene

| Scene | Path |
|-------|------|
| LobbyScreen | `Client/Assets/_Project/Scenes/LobbyScreen.unity` |

### Hierarchy

```
LobbyScreenCanvas (+ LobbyScreenController)
├── Background          // MainMenuBackground sprite
├── SafeArea
├── BackButton
├── HeaderRoot          // Public Lobby / 1/4 / GULF-4821
├── SlotsRoot
│   ├── PlayerSlot_0    // occupied placeholder (avatar, name, flag, level, ready)
│   ├── PlayerSlot_1..3 // empty “Waiting for player...”
└── FooterRoot
    ├── ReadyButton     // visual only
    ├── LobbyStatusText // “Waiting for players...”
    └── PlayButton      // disabled visual only
EventSystem
```

Canvas: Overlay, CanvasScaler **1920×1080**, match **0.5**. GulfRun gold/dark panels, shadows, fonts match Play Menu / Quick Play.

## Scripts

- `Features/LobbyScreen/LobbyScreenController.cs` — Back only; static placeholder data lives in the scene
- Asmdef: `GulfRun.Features.LobbyScreen` → Core + UI

## Rebuild

- Menu: `GulfRun/Play Flow/Build Lobby Screen (Sprint 21.1)`
- Batch: `GulfRun.Editor.PlayFlowSceneBuilder.BuildLobbyScreenBatch`

## Constraints honored

- Main Menu layout unchanged
- Pre-race `Lobby.unity` kept in build; not gutted
- No SessionManager binding on LobbyScreen slots
- `DefaultNetworkPrefabs.asset` and root `Assets/Btn_*.png` left uncommitted
