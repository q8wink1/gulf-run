# Sprint 14 — Matchmaking, Room & Pre-Race Lobby — Sprint Report

**Role:** Lead Multiplayer Gameplay Engineer and UI/UX Engineer  
**Scope:** Complete Matchmaking / Private Room / Pre-Race Lobby: Quick Play search → Match Found → Lobby scene, Private Room create/join/code share, Ready System, Bot Fill, Auto Start 5-4-3-2-1-GO, player cards, owner controls, voice/quick chat, reconnect/host-migration/latency indicators, debug overlay.  
**Status:** Complete under the same environment constraint as prior sprints (no licensed Unity Editor on this machine — offline `dotnet build` against the UnityEngine shim).  
**Note:** Brand Intro was already shipped under [`SPRINT-14-GULFRUN-BRAND-INTRO.md`](SPRINT-14-GULFRUN-BRAND-INTRO.md). This report covers the interrupted Matchmaking track that resumes from uncommitted WIP (Session/Lobby/BotFill/HostMigration seams) and finishes the Pre-Race Lobby scene + UI.

---

## 0. Continuation check

Did **not** restart. Extended existing Sprint 4 Multiplayer + Sprint 13 PLAY seams:

| Already present (WIP / prior) | Finished this pass |
|---|---|
| `IMatchLobbySummaryProvider` expanded API | `GulfRun.Features.Matchmaking` Pre-Race Lobby UI assembly |
| `SessionManager` QuickPlay/PrivateRoom/Kick/QuickChat | Quick Play ~2.2s Searching delay + auto opponent fill |
| `BotFillController`, `HostMigrationController` | Boot wiring for both singletons |
| Transport Kick / QuickChat | `Lobby.unity` + EditorBuildSettings entry |
| `SceneManager.LoadLobby` | `PrivateRoomPanelView` + Social panel entry |
| NetworkSyncConfig countdown = 5s | Matchmaking debug at `panelX: 4960` |

Sprints 15 (Race HUD) and 16 (Character Locker) remained untouched except shared seams already on `main`.

---

## 1. Quick Play

- PLAY → `StartQuickMatch` holds `IsMatchmaking` for ~2.2s (Searching… + ETA).
- Cancel clears the pending search.
- On match found: hosts lobby, fills remaining seats with always-Ready simulated opponents under loopback, `PlayButtonView` auto-`LoadLobby()`.
- Match Found premium popup on Lobby entry.

## 2. Private Room

- Social panel **Private Room** → Create / Join by Room Code / Copy / Share.
- Max 4 / min 2 from `NetworkSyncConfig`.
- Room Code via existing `RoomCodeGenerator` (6-char, ambiguous chars excluded).

## 3. Ready System & Auto Start

- Per-player Ready / Not Ready.
- Host Start Match gated on `AllPlayersReady` (min players + everyone Ready).
- `MatchManager` still auto-starts countdown when ready; duration **5s** → 5-4-3-2-1-GO overlay, fade to Gameplay.

## 4. Bots

- Private-Room-only Bot Fill toggle (P017 excludes bot fill from public MM).
- ON fills empty slots via loopback simulate-join; OFF removes bots.
- Quick Play uses separate simulated “Racer N” opponents (not BotFillController).

## 5. Matchmaking / Networking

| Item | Behavior |
|---|---|
| Reconnect | Existing `ConnectionManager` timeout/reconnect events |
| Host migration | `HostMigrationController` promotes earliest remaining join on host leave |
| Latency | `ConnectionQuality` buckets + ping ms (0 under loopback) |
| Kick | Host `KickParticipant` → `DisconnectReason.Kicked` |

## 6. UI (Pre-Race Lobby)

`GulfRun.Features.Matchmaking` (Core + Domain only):

- Theme (gold/sand), animated Gulf background + launch city label
- Player cards: character silhouette, flag, name, league/trophies (local profile), quality, ready, voice, OWNER/BOT, Kick
- Owner: Copy/Share/Invite/Bot Fill/Start Match
- Ready Up / Leave, Quick Chat (Ready/Good Luck/Wait/Hello), Voice widget
- Match Found popup, Auto Start countdown + fade
- Join/leave SFX hooks (null-safe clips)

## 7. Performance

- Target 60 FPS unchanged (`GameManager`).
- OnGUI lobby widgets; no per-frame roster rebuild beyond existing provider reads.
- Minimal redraw: event-driven audio director; countdown only when phase is Countdown.

## 8. Build Verification

| Check | Result |
|---|---|
| Offline `dotnet build` `.compile_check/CompileCheck.csproj` | **0 errors / 0 warnings** |
| YAML structure (`Lobby`/`Boot`/`MainMenu`/`EditorBuildSettings`) | OK |
| Real Unity Editor batch build | **Not run** — no licensed Editor on this machine (same constraint as Sprints 1–16) |

## 9. Compiler Status

Clean offline compile of all project `.cs` files against the UnityEngine shim (includes `GUIUtility.systemCopyBuffer` stub for Room Code copy).

## 10. Git

| Item | Value |
|---|---|
| Commit hash | `8bd8460` |
| Commit message | `Sprint 14 - Matchmaking, Room & Pre-Race Lobby` |
| Branch | `main` |
| Push status | Pushed to `origin/main` (verified after push) |

## 11. Remaining TODOs

1. Real multi-client transport (loopback JoinAsClient remains a stub; Private Room join-by-code cannot complete against a remote host yet).
2. Final lobby art/audio clips (music, ready/countdown/join/leave SFX still Inspector-null).
3. OS share sheet (Share currently copies invite text to clipboard).
4. Real microphone capture/transport for Voice Chat (UI mode only).
5. First licensed Unity Editor open for Play Mode validation of Lobby scene.

---

*End of Sprint 14 Matchmaking report.*
