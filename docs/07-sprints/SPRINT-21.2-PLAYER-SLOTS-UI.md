# Sprint 21.2 — Player Slots UI

**Scope:** Polish LobbyScreen player slots only (visual placeholders). No ready logic, networking, host permissions, or multiplayer sync.

**Status:** Complete.

## Mock layout

| Slot | State | Notes |
|------|-------|-------|
| `PlayerSlot_0` | Occupied — Ready + Host | Circular avatar (Mask), online dot, host badge **visible** (design mock) |
| `PlayerSlot_1` | Occupied — Not Ready | Full slot chrome; host badge inactive |
| `PlayerSlot_2` | Occupied — Connecting | Amber connecting status placeholder |
| `PlayerSlot_3` | Empty | `+ Waiting for Player` + faint plus ring; host badge inactive |

Header `PlayerCountText` mock: **3/4**. Header / Footer / Back unchanged in role.

## Slot chrome (occupied)

- Circular avatar (`Avatar` + `Mask` + `AvatarImage`)
- `PlayerName`, `CountryFlag` / `CountryCode`, `LevelBadge` / `LevelText`
- `OnlineIndicator` (green / muted)
- `ReadyStatus` + `ReadyLabel` (Ready / Not Ready / Connecting — visuals only)
- `HostBadge` (`HOST`) — active only on slot 0

## Empty slot

- Softer border / fill
- `EmptyPlusRing` + `EmptyPlusMark`
- `EmptySlotLabel`: `+ Waiting for Player`

## Layout

- SlotsRoot ~980×680; slot height **148**, gap **22** (touch-friendly)
- CanvasScaler **1920×1080**, match **0.5** (unchanged)

## Rebuild

- Menu: `GulfRun/Play Flow/Build Lobby Screen (Sprint 21.2)`
- Batch: `GulfRun.Editor.PlayFlowSceneBuilder.BuildLobbyScreenBatch`

## Constraints honored

- No SessionManager / ready / host logic on LobbyScreen
- Pre-race `Lobby.unity` untouched
- Main Menu / QuickPlay / InviteFriends not rebuilt by this batch
- `DefaultNetworkPrefabs.asset` and root `Assets/Btn_*.png` left uncommitted
