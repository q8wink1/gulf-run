# Sprint 23.4 — Player Controller

**Scope:** First gameplay sprint — core 3-lane runner controller. No obstacles, coins, power-ups, multiplayer, AI, or race logic.

**Status:** Complete.

## Architecture

| Layer | Type | Responsibility |
|-------|------|----------------|
| Domain | `RunnerLane`, `RunnerLaneMath` | Engine-free lane enum + X math |
| Domain | `PlayerMovementState.Sliding`, `CharacterAnimationState.Slide` | Shared locomotion vocabulary |
| Features.Gameplay | `RunnerMovementConfig` | Inspector tuning (speed, lanes, jump, slide, swipe) |
| Features.Gameplay | `RunnerSwipeInput` | Touch swipe + keyboard (A/D/W/S, arrows) |
| Features.Gameplay | `RunnerPlayerController` | Auto-run +Z, lane lerp, jump, slide collider |
| Features.Gameplay | `RunnerAnimatorDriver` | Animator params / placeholder anim enum |

Existing 2D `Features.PlayerController` (Rigidbody2D side-scroller) is unchanged.

## Controls

| Input | Action |
|-------|--------|
| Swipe Left / A / ← | Lane left (ignored while changing lanes) |
| Swipe Right / D / → | Lane right |
| Swipe Up / W / ↑ / Space | Jump (single; blocked while sliding) |
| Swipe Down / S / ↓ | Slide (timed; shrinks capsule height) |

## Behaviour

- **Forward:** continuous +Z at `forwardSpeed * speedMultiplier` (ready for future speed-ups via `SetSpeedScale`)
- **Lanes:** Left / Center / Right via configurable spacing; smooth SmoothStep lerp; no teleport
- **Jump:** vertical velocity from height+gravity; lands at `groundedY`; Landing state placeholder
- **Slide:** duration from config; temporary capsule height/center; auto restore
- **Camera-ready:** tagged `Player`; expose `FollowTarget` transform

## Scene

- `RunnerPlayer` (tag Player) + capsule visual + `RunnerGround` strip in `Gameplay.unity`
- Config: `Settings/RunnerMovementConfig.asset`

## Rebuild / patch

- Menu: `GulfRun/Play Flow/Validate Gameplay Runner (Sprint 23.4/23.5)`
- Fallback: `_tools/patch_gameplay_sprint_23.py --player`

## Constraints honored

- No obstacles / coins / networking / race logic
- Main Menu untouched
- `DefaultNetworkPrefabs` / `Btn_*` uncommitted
