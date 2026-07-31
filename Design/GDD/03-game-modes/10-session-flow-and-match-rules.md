# 10 — Session Flow & Match Rules

**GDD chapter:** 10  
**Status:** Partial — synced to P002 + P010  
**Design Owner:** `[TBD]`  
**Last updated:** 2026-07-31

> Race rules SoT: [P010](../P010-RACE-RULES-v1.0.md). Journey SoT: [P002](../P002-CORE-GAMEPLAY-LOOP-v1.0.md).

---

## 10.1 Pre-session flow

See P002 Stages 1–6 and P004 Play paths. Character select before race: P005 (**TODO** exact stage wiring).

## 10.2 In-session phases

| Phase | Start condition | End condition | Player agency | Status |
|-------|-----------------|---------------|---------------|--------|
| Starting area wait | Match loaded | Countdown begins | Wait | P010 |
| Countdown | Displayed | Race begins | **TODO** input lock | P010 |
| Race | Simultaneous start | Finish crossings / future end rule | Auto-run; Jump; Double Jump; Item Boxes; use one Item; avoid Obstacles | P010 |
| Finish / ranking | Cross finish line | Rank 1st–4th by order | — | P010 |

## 10.3 Post-session flow

| Step | What player sees | Notes | Status |
|------|------------------|-------|--------|
| Results Screen | All 4 players by position; Final Position, Name, Character, Race Time; rewards **placeholder** | Immediate after race | **P011** |
| Actions | Continue (**TODO** effect); Return to Main Menu | — | **P011** |
| Rewards | System later; placeholder only | Not coins/XP/etc. | **P011** |

## 10.4 Disconnect / interrupt / reconnect

| Case | Desired player outcome | Ranked impact | Status |
|------|------------------------|---------------|--------|
| Disconnection | **TODO** | **TODO** | System exists; rules **not defined** (P010) |
| AFK | **TODO** | **TODO** | System exists; rules **not defined** (P010) |

## 10.5 Forfeit / surrender

**TODO** / not defined in P010.

## 10.6 Open questions

See P010 §12.
