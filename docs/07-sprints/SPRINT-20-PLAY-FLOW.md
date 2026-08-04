# Sprint 20 — Play Menu / Quick Play / Invite Friends (UI flow)

**Scope:** UI-only play hub and entry screens. No networking, matchmaking, or friend backend.

## Navigation

Main Menu Play Now → **PlayMenu** → **QuickPlay** or **InviteFriends**

- Back from Play Menu → Main Menu
- Back/Cancel from Quick Play → Play Menu
- Back from Invite Friends → Play Menu

## Scenes

| Scene | Path |
|-------|------|
| PlayMenu | `Client/Assets/_Project/Scenes/PlayMenu.unity` |
| QuickPlay | `Client/Assets/_Project/Scenes/QuickPlay.unity` |
| InviteFriends | `Client/Assets/_Project/Scenes/InviteFriends.unity` |

## Notes

- Main Menu layout/art unchanged — only `Button` + `MainMenuPlayButton` on `PlayButtonImage`, plus EventSystem if missing.
- Quick Play auto-advances placeholder status text: Searching → Players Found → Joining Room → Creating Room → Waiting For Players → Ready To Start.
- Invite Friends uses fake friend rows, local copy-buffer invite URL, and stub Send/Share feedback.
- Rebuild via Unity menu `GulfRun/Play Flow/Build Scenes + Wire Play` or batch execute `GulfRun.Editor.PlayFlowSceneBuilder.RunBatch`.
