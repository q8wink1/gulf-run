# Design

Product and game design documentation for Project GulfRun.

## Source of truth

| Domain | Path | Rule |
|--------|------|------|
| **All gameplay design** | [`GDD/`](GDD/README.md) | **Single source of truth.** Mechanics, UI screens, modes, entities, progression, economy rules, monetization design, multiplayer design intent — only as filled by the Design Owner. |
| Economy working papers | `economy/` | Supporting analysis; must not contradict approved GDD chapters 13–14, 26–27 |
| UX working papers | `ux/` | Wireframes / flows; must not contradict approved GDD chapters 19–20 |
| LiveOps calendars | `liveops-calendars/` | Schedules; must not contradict approved GDD chapters 24–25 |

## Hard rules

1. **Do not invent gameplay.** Empty `[TBD]` sections stay empty until the Design Owner fills them.
2. Documentation engineers **organize**, **flag risks/conflicts**, and **prepare implementation plans** from approved GDD text only.
3. Engineering architecture lives in `docs/` and constrains *how* systems are built — not *what the game is*.
4. If GDD and `docs/` conflict → log in [GDD Open Questions / Conflicts](GDD/16-appendix/32-open-questions-log.md); escalate; do not silently invent a fix.

## Owner

**Design Owner:** `[TBD]` (product design authority for GDD content)
