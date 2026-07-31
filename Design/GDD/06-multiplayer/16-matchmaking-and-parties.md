# 16 — Matchmaking & Parties

**GDD chapter:** 16  
**Status:** Partial — synced to P017 / P018  
**Design Owner:** `[TBD]`  
**Last updated:** 2026-07-31

> Matchmaking SoT: [P017](../P017-MATCHMAKING-SYSTEM-v1.0.md).  
> Private Room SoT: [P018](../P018-PRIVATE-ROOM-SYSTEM-v1.0.md).

---

## 16.1 Party rules

| Field | Value |
|-------|-------|
| Friend Party | Supported match type (P017) |
| Max party size | **TODO** |
| Invite methods | P014 Invite Friend; P004 Invite Friend |
| Leader powers | **TODO** (Private Room host: P018) |

## 16.2 Matchmaking goals

| Goal | Priority | Notes |
|------|----------|-------|
| Fair races | Required | Algorithm not defined (P017) |
| Fast as possible while fair | Quick Match | P017 |
| Exactly 4 players | Required | P017 / P010 |
| Private Rooms not in public MM | Required | P018 |

## 16.3 Matchmaking inputs (design)

| Signal | Used? | How | Status |
|--------|-------|-----|--------|
| Skill / MMR | **Not defined** | — | P017 |
| Latency / region | **Not defined** | — | P017 |
| Party size | **TODO** | Friend Party | P017 |

## 16.4 Queue UX

Search status: Searching..., Players Found, Connecting, Loading Match, Match Ready (P017). Cancel before confirm allowed.

## 16.5 Backfill / AI fillers

Bot Filling **not defined** (P017 / P018).

## 16.6 Private Room

| Field | Value |
|-------|--------|
| Capacity | Exactly **4** (P018) |
| Visibility | Not in public matchmaking (P018) |
| Join | Room Code; Friend Invitation (P018) |
| Host | Start Match, Invite, Remove, Close, Transfer Host (P018) |
| Player | Join, Leave, Ready, Not Ready, View Players (P018) |
| States | Waiting For Players; Ready; Starting Match; In Match; Closed |
| Min to start | **Not defined** (TODO) |

## 16.7 Open questions

See P017 §13 and P018 §12.
