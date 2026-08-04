# Sprint 21.3 — Ready System UI

**Scope:** LobbyScreen Ready System UI only (visual placeholders + local Ready button chrome toggle). No multiplayer logic, sync, networking, SessionManager ready state, or room logic.

**Status:** Complete.

## UI added / enhanced

| Element | Role |
|---------|------|
| `ReadyButton` | Bottom-left, large premium (340×96). Default **Ready** (GulfRun gold). Local click → **Ready ✓** + green success style. Visual only. |
| Slot `ReadyLabel` / `ReadyStatus` | Placeholder statuses: Ready / Not Ready / Connecting (Sprint 21.2 chrome kept). Optional local slot 0 chrome follows Ready button toggle. |
| `StatusRoot` / `LobbyStatusPanel` | Status band **above** footer buttons. Primary: `Waiting for everyone to be Ready...` |
| `StatusMsg_*` | Inactive example copy: Waiting for players... / Players joining... / Waiting for everyone to be Ready... / Ready to Start |
| `PlayerCountText` | Header: **Players: 3 / 4** (matches 3 occupied slots mock) |
| `CountdownPlaceholder` | `Starting in: 00:10` — **hidden by default** (`m_IsActive: 0`) |
| `PlayButton` | Still disabled visual placeholder |

## Hierarchy (delta)

```
LobbyScreenCanvas (+ LobbyScreenController)
├── HeaderRoot
│   └── PlayerCountText          // "Players: 3 / 4"
├── SlotsRoot                    // Sprint 21.2 polish unchanged
├── StatusRoot                   // NEW — above footer
│   ├── LobbyStatusPanel
│   │   ├── LobbyStatusText      // primary active
│   │   ├── StatusMsg_WaitingForPlayers   // inactive
│   │   ├── StatusMsg_PlayersJoining      // inactive
│   │   ├── StatusMsg_WaitingReady        // inactive
│   │   └── StatusMsg_ReadyToStart        // inactive
│   └── CountdownPlaceholder     // inactive
└── FooterRoot
    ├── ReadyButton              // premium local visual toggle
    └── PlayButton               // disabled
```

Canvas: Overlay, CanvasScaler **1920×1080**, match **0.5**.

## Scripts

- `LobbyScreenController` — Back → Play Menu; Ready click swaps button text/color (+ optional local slot chrome). No SessionManager / network ready calls.

## Rebuild

- Menu: `GulfRun/Play Flow/Build Lobby Screen (Sprint 21.3)`
- Batch: `GulfRun.Editor.PlayFlowSceneBuilder.BuildLobbyScreenBatch`
- Fallback (Editor lock): `_tools/gen_lobby_screen_scene.py`

## Constraints honored

- No ready / sync / networking logic beyond local visual toggle
- Pre-race `Lobby.unity` untouched
- Main Menu layout unchanged
- `DefaultNetworkPrefabs.asset` and root `Assets/Btn_*.png` left uncommitted
