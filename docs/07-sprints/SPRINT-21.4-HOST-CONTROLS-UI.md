# Sprint 21.4 — Host Controls UI

**Scope:** LobbyScreen Host Controls UI only (visual placeholders + optional Play prepared chrome demo). No host permissions, kick logic, ready validation, networking, SessionManager, or match start.

**Status:** Complete.

## UI added / enhanced

| Element | Role |
|---------|------|
| `HostBadge` / `HostLabel` | Premium gold **HOST** beside room host name (slot 0). Inactive on non-host slots. |
| `PlayButton` | Bottom-right, large premium (420×96). Default **disabled** + greyed **Waiting for Players...**. |
| `PlayLabel_StartMatch` | Inactive named prepared-state placeholder (**Start Match**). |
| Play prepared demo | `LobbyScreenController.ApplyPlayPreparedVisual` + Editor ContextMenu — visual only, no match start. |
| `KickButton` | Small Kick on non-host slots. **Active** Host-preview mock on slots 1–2; **inactive** on empty slot 3; **absent** on host slot 0. No kick logic. |
| `RoomTypeText` | **Room Type: Public** |
| `HostNameText` | **Host: DesertFox** |
| `PlayerCountText` | **Players: 1 / 4** (placeholder) |
| `MessageFooterRoot` | System message strip: sample **Player joined...**; inactive Player left / Host changed / Searching for player... |
| `ReadyButton` / `StatusRoot` | Sprint 21.3 Ready System UI kept |

## Hierarchy (delta)

```
LobbyScreenCanvas (+ LobbyScreenController)
├── HeaderRoot
│   ├── RoomTypeText             // "Room Type: Public"
│   ├── HostNameText             // "Host: DesertFox"
│   ├── PlayerCountText          // "Players: 1 / 4"
│   └── RoomCodeText
├── SlotsRoot
│   ├── PlayerSlot_0 ... HostBadge (active) — no Kick
│   ├── PlayerSlot_1 ... KickButton (active Host preview)
│   ├── PlayerSlot_2 ... KickButton (active Host preview)
│   └── PlayerSlot_3 ... KickButton (inactive)
├── StatusRoot                   // Sprint 21.3 Ready status band
├── MessageFooterRoot            // NEW — system message placeholders
│   ├── SystemMsg_PlayerJoined   // sample visible
│   ├── SystemMsg_PlayerLeft     // inactive
│   ├── SystemMsg_HostChanged    // inactive
│   └── SystemMsg_Searching      // inactive
└── FooterRoot
    ├── ReadyButton
    └── PlayButton               // Waiting for Players... (disabled)
        └── PlayLabel_StartMatch // inactive prepared state
```

Canvas: Overlay, CanvasScaler **1920×1080**, match **0.5**.

## Scripts

- `LobbyScreenController` — Back → Play Menu; Ready local visual toggle; Play prepared chrome demo only (no match start / SessionManager).

## Rebuild

- Menu: `GulfRun/Play Flow/Build Lobby Screen (Sprint 21.4)`
- Batch: `GulfRun.Editor.PlayFlowSceneBuilder.BuildLobbyScreenBatch`
- Fallback (Editor lock): `_tools/gen_lobby_screen_scene.py`

## Constraints honored

- No host permissions / kick / ready validation / networking / match start
- Pre-race `Lobby.unity` untouched
- Main Menu layout unchanged
- `DefaultNetworkPrefabs.asset` and root `Assets/Btn_*.png` left uncommitted
