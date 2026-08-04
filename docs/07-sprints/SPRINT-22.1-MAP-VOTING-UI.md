# Sprint 22.1 — Map Voting UI

**Scope:** Complete Map Voting screen UI only. No vote counting, countdown logic, networking, backend, SessionManager match start, or gameplay.

**Status:** Complete.

## Screen

| Area | Detail |
|------|--------|
| Scene | `Client/Assets/_Project/Scenes/MapVoting.unity` |
| Canvas | Overlay, CanvasScaler **1920×1080**, match **0.5** |
| SafeArea | Main Menu insets: **48 L/R**, **52 top**, **34 bottom** (`sizeDelta -96/-86`, `y -9`) |
| Header | **Choose Your Map** + subtitle |
| Cards | Exactly three: Kuwait City, Dubai Marina, Muscat Coast |
| Footer | Timer **20 Seconds Remaining**; **Total Players 4/4**; **Current Votes 0** |

## Hierarchy

```
MapVotingCanvas (+ MapVotingScreenController)
├── Background
├── SafeArea
├── BackButton
├── HeaderRoot → TitleText + SubtitleText
├── CardsRoot → MapCard_0…2 (MapPreview, MapName, Description, VoteButton)
└── FooterRoot → TimerText + PlayersText + VotesText
```

## Navigation (temporary)

- LobbyScreen **Start Match** (Play prepared) → `SceneManager.LoadMapVoting()` only
- MapVoting **Back** → LobbyScreen
- No Ready validation, no `SessionManager.RequestHostStart`

## Scripts

- `MapVotingScreenController` — Back + local Vote highlight only
- Legacy `MapVotingView` / `MapVotingSession` **not** attached to this scene

## Rebuild

- Menu: `GulfRun/Play Flow/Build Map Voting Screen (Sprint 22.1)`
- Batch: `GulfRun.Editor.PlayFlowSceneBuilder.BuildMapVotingScreenBatch`
- Fallback (Editor lock): `_tools/gen_map_voting_scene.py`

## Constraints honored

- Scene name remains **MapVoting** (`LoadMapVoting` unchanged)
- Main Menu layout untouched
- No real voting / countdown / multiplayer
- `DefaultNetworkPrefabs.asset` and root `Assets/Btn_*.png` left uncommitted
