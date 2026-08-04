# Sprint 20.1 — Play Menu UI

**Scope:** Play Menu user interface + navigation only. No matchmaking, lobby, networking, backend, or gameplay logic on this screen.

**Status:** Complete (polish on top of `eb31ee5` / `0f8c3e5` play flow).

## Navigation

| From | Action | To |
|------|--------|----|
| Main Menu | Play Now (`MainMenuPlayButton`) | **PlayMenu** |
| Play Menu | Back (top-left) | Main Menu |
| Play Menu | Quick Play card | **QuickPlay** |
| Play Menu | Invite Friends card | **InviteFriends** |

`PlayMenuController` only loads scenes via `SceneManager`. It does not start matchmaking.

## Scene

| Scene | Path |
|-------|------|
| PlayMenu | `Client/Assets/_Project/Scenes/PlayMenu.unity` |

Canvas: Overlay, CanvasScaler **1920×1080**, match **0.5**. Background reuses Main Menu sprite. Two centered premium mode cards with GulfRun gold/dark panel styling, soft shadow, and mode icons (lightning / friends glyphs).

### Card copy

- **Quick Play** — "Find and join a public multiplayer match instantly."
- **Invite Friends** — "Create a private room and play with your friends."

## Constraints honored

- Main Menu layout/art/background unchanged (existing thin `Button` + `MainMenuPlayButton` only).
- No edits to SessionManager / Lobby / Matchmaking for this sprint.
- QuickPlay / InviteFriends destination scenes left in place (not rebuilt by Sprint 20.1 polish).
- `DefaultNetworkPrefabs.asset` and root `Assets/Btn_*.png` left uncommitted.

## Rebuild / polish

- Full rebuild (also rebuilds QuickPlay + InviteFriends): `GulfRun/Play Flow/Build Scenes + Wire Play`
- Sprint 20.1 polish only: `GulfRun/Play Flow/Polish Play Menu (Sprint 20.1)` or batch `GulfRun.Editor.PlayFlowSceneBuilder.PolishPlayMenuBatch`
