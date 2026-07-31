# 25 — Seasons, Events & Calendars

**GDD chapter:** 25  
**Status:** Partial — synced to P030 / P031 / P019 / P025 / P029  
**Design Owner:** `[TBD]`  
**Last updated:** 2026-07-31

> Season System SoT: [P030](../P030-SEASON-SYSTEM-v1.0.md).  
> Live Events SoT: [P031](../P031-LIVE-EVENTS-SYSTEM-v1.0.md).  
> Season Leaderboard: [P019](../P019-LEADERBOARD-SYSTEM-v1.0.md).  
> Competitive Rank seasons: [P025](../P025-RANK-SYSTEM-v1.0.md).  
> Battle Pass: [P029](../P029-BATTLE-PASS-SYSTEM-v1.0.md).  
> Do not invent season duration, event rewards, missions, or event shops.

---

## 25.1 Season model

| Field | Value |
|-------|-------|
| Model | Game operates using Seasons (**P030**) |
| Period | Fixed period of game progression (**P030**) |
| Active seasons | **Only one** at a time (**P030**) |
| Participation | Automatic; no manual join (**P030**) |
| Season length intent | **Not defined** (Season Duration — P030) |
| What resets | **Not defined** (Season Reset Rules — P030) |
| What persists | **Not defined** |
| Season pass? | Battle Pass may be contained (**P029** / **P030**) |
| Seasonal Leaderboards | May be contained (**P019** / **P030**) |
| Seasonal Competitive Rank | **Exist** (**P025**) |
| Season Challenges / Cosmetics / Events | May be contained (**P030**); Events detail **P031** |
| Historical Seasons / Archive | **Not defined** (**P030**) |
| Transition | Season ends → new Season begins; backend-synced (**P030**) |

## 25.2 Live Events

| Field | Value |
|-------|-------|
| Limited-time content | **P031** |
| Visibility | Only **active** events (**P031**) |
| Types | Season, Holiday, National Day, Ramadan, Special Collaboration; Future types (**P031**) |
| Participation | Automatic; some may have minimum requirements (**not defined**) |
| Timers | Start / End / Remaining Time / Event Status (**P031**) |

| Event type | Frequency | Rewards intent | Status |
|------------|-----------|----------------|--------|
| Season Events | Limited-time | **Not defined** | **P031** |
| Holiday Events | Limited-time | **Not defined** | **P031** |
| National Day Events | Limited-time | **Not defined** | **P031** |
| Ramadan Events | Limited-time | **Not defined** | **P031** |
| Special Collaboration Events | Limited-time | **Not defined** | **P031** |
| Event Leaderboard | **TODO** | **Not defined** | Future (P019); Event Leaderboards not defined in P031 |

## 25.3 Calendar placeholders

| Period | Theme | Modes featured | Notes | Status |
|--------|-------|----------------|-------|--------|
| `[TBD]` | **Not defined** (P030) | `[TBD]` | Season Names/Themes TBD | Template |

## 25.4 Display (design)

Season: Current Season Name, Season Number, Season Remaining Time, Current Season Progress (**P030**).  
Events: Start Time, End Time, Remaining Time, Event Status (**P031**).

## 25.5 Open questions

See P030 §12, P031 §11, P029 §13, P025 §12, P019 §12.
