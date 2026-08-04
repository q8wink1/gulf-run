# Sprint 22.3 — Voting HUD UI

**Scope:** Complete Map Voting Voting HUD UI only. No vote counting, countdown logic, networking, backend, SessionManager, or gameplay.

**Status:** Complete.

## HUD elements

| Element | Detail |
|---------|--------|
| **TimerPanel** | Top-center premium panel; `TimerText` = `20s`; `ProgressBarTrack` / `ProgressBarFill` (Filled, amount 1.0 placeholder) |
| **StatusPanel** | Below timer; primary `StatusText` = `Players are voting...`; inactive copies for Waiting / Voting / Finalizing |
| **StatsPanel** | Inside footer: `Players Voted 0/4`, `Remaining Votes 4`, `Total Votes 0` |
| **VoteConfirmation** | `✓ Your vote has been submitted.` — **inactive by default** |

## Hierarchy

```
MapVotingCanvas (+ MapVotingScreenController)
├── Background
├── SafeArea
├── BackButton
├── TimerPanel → TimerText + ProgressBarTrack → ProgressBarFill
├── StatusPanel → StatusText + StatusMsg_* (inactive)
├── HeaderRoot → TitleText + SubtitleText
├── CardsRoot → MapCard_0…2 (Sprint 22.2 premium cards)
├── FooterRoot → StatsPanel → PlayersVotedText / RemainingVotesText / TotalVotesText
└── VoteConfirmation (inactive) → ConfirmationText
```

## Visual / mobile

- CanvasScaler **1920×1080**, match **0.5**
- SafeArea Main Menu insets: **48 L/R**, **52 top**, **34 bottom** (`sizeDelta -96/-86`, `y -9`)
- GulfRun gold / dark panel identity (same as Lobby / prior Map Voting sprints)

## Scripts

- `MapVotingScreenController` — Back + local Vote highlight only (unchanged behavior)
- Legacy `MapVotingView` / `MapVotingSession` **not** attached

## Rebuild

- Menu: `GulfRun/Play Flow/Build Map Voting Screen (Sprint 22.3)`
- Batch: `GulfRun.Editor.PlayFlowSceneBuilder.BuildMapVotingScreenBatch`
- Fallback (Editor lock): `_tools/gen_map_voting_scene.py`

## Constraints honored

- Scene name remains **MapVoting**
- Three premium map cards preserved
- Main Menu and SessionManager untouched
- No real voting / countdown / multiplayer sync
- `DefaultNetworkPrefabs.asset` and root `Assets/Btn_*.png` left uncommitted
