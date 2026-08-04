# Sprint 21.5 — Lobby Final Polish

**Scope:** Visual polish of existing LobbyScreen UI only. No networking, ready logic, matchmaking, backend, multiplayer sync, or gameplay. Main Menu and SessionManager untouched.

**Status:** Complete.

## Polish applied

| Area | Change |
|------|--------|
| SafeArea | Main Menu insets: **48 L/R**, **52 top**, **34 bottom** (`sizeDelta -96/-86`, `y -9`) |
| Canvas | Overlay, CanvasScaler **1920×1080**, match **0.5** (unchanged) |
| Header | **1200×120**, top `-56`; balanced Host / Players / Room Code columns; Room Type **30** |
| Player slots | All four **960×148**; gap **24**; SlotsRoot height **664** (exact fit); fill inset **3** |
| Ready / Play | Identical **400×104** touch targets; labels **30**; footer **1400×120** @ `y=40`, margins **48** |
| Status / messages | Status @ `214`; message strip **1200×36** @ `156`; panel **980×72** |
| Shadows | Unity UI **Shadow** only (`cfabb044…`), softer **0.42 / (0,-6)** — no Outline GUIDs |
| Back | Top-left `(48, -52)` within SafeArea top inset |

## Hierarchy (unchanged structure)

```
LobbyScreenCanvas (+ LobbyScreenController)
├── Background
├── SafeArea                         // inset marker (Main Menu–matched)
├── BackButton
├── HeaderRoot
├── SlotsRoot → PlayerSlot_0…3
├── StatusRoot → LobbyStatusPanel + CountdownPlaceholder
├── MessageFooterRoot
└── FooterRoot → ReadyButton + PlayButton
```

## Scripts

- `LobbyScreenController` — Back → Play Menu; Ready / Play prepared chrome demos only.
- No SessionManager / networking changes.

## Rebuild

- Menu: `GulfRun/Play Flow/Build Lobby Screen (Sprint 21.5)`
- Batch: `GulfRun.Editor.PlayFlowSceneBuilder.BuildLobbyScreenBatch`
- Fallback (Editor lock): `_tools/gen_lobby_screen_scene.py`

## Verification

- Equal slot `sizeDelta`; Ready/Play identical size; SafeArea insets match Main Menu
- Controller GUID `b20c…0072` valid; Shadow GUID `cfabb044…` only (no dead Outline)
- Solid-color Images may use `m_Sprite: {fileID: 0}` by design; Background + knobs resolve
- `DefaultNetworkPrefabs.asset` and root `Assets/Btn_*.png` left uncommitted

## Constraints honored

- No redesign of Lobby layout language
- No networking / ready validation / match start
- Pre-race `Lobby.unity` untouched
- Main Menu layout unchanged
