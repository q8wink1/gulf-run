# Client

Unity mobile game client for GulfRun.

**Status:** Sprint 4 — Multiplayer Foundation complete. Transport-agnostic Match Flow (Create/Join/Leave/Cancel), Lobby + Ready System, host-authoritative shared countdown, player-state sync + network interpolation, and Connection/Lobby/Match/Session/Spawn managers are implemented on top of a `LocalLoopbackTransport` default pending a ratified Netcode ADR; no final gameplay logic (no networked player avatars, no real remote transport) yet.

**Engine:** Targets Unity **2022.3.62f1 LTS**. As of Sprint 3, this machine has **Unity Hub installed but no Editor version downloaded** (superseding earlier Sprint reports' assumption of an installed Editor) — see Sprint 1 Report open item. The approved [Technical Stack](../docs/02-architecture/TECHNICAL_STACK.md) targets **Unity 6 LTS** long-term.

**Owner:** Client Lead

## Rules

- All first-party content under `Assets/_Project/`
- Assembly definitions mandatory
- No trusted economy logic — server authority only
- Follow [Coding Standards](../docs/03-standards/CODING_STANDARDS.md) and [Unity Packages](../docs/04-engineering/UNITY_PACKAGES.md)

## Sprint 1 notes

- See [`docs/07-sprints/SPRINT-01-PROJECT-FOUNDATION.md`](../docs/07-sprints/SPRINT-01-PROJECT-FOUNDATION.md) for the full Sprint Report.
- The Unity Editor on this machine has no activated license, so the project could not be opened, package-resolved, or batch-mode-compiled through Unity itself. All files were hand-authored to Unity's on-disk project format and cross-checked (YAML structure validation; C# compiled offline against the installed Editor's own reference assemblies). **First real Editor open + package resolution is a required next step before Sprint 2.**

## Sprint 2 notes

- See [`docs/07-sprints/SPRINT-02-PLAYER-CONTROLLER-FOUNDATION.md`](../docs/07-sprints/SPRINT-02-PLAYER-CONTROLLER-FOUNDATION.md) for the full Sprint Report.
- Same licensing constraint as Sprint 1: `Player.prefab`, the Animator Controller, and the two new ScriptableObject config assets were all hand-authored to Unity's on-disk YAML format, then structurally validated and cross-reference-checked offline. **First real Editor open remains required** to resolve the Input System package, assign real placeholder animations, and run a Play Mode smoke test.

## Sprint 3 notes

- See [`docs/07-sprints/SPRINT-03-ENDLESS-RUNNER-CORE.md`](../docs/07-sprints/SPRINT-03-ENDLESS-RUNNER-CORE.md) for the full Sprint Report.
- No Unity Editor is installed on this machine at all (only Hub) — all 5 new prefabs, 8 new ScriptableObject config assets, and the `Gameplay.unity` scene changes were hand-authored to Unity's on-disk YAML format, then structurally validated and fileID/guid cross-reference-checked offline. All 58 project `.cs` files (Sprints 1–3) were compiled together offline against a hand-written UnityEngine API shim. **First real Editor open remains required** before Play Mode can be exercised.
- A `Player.prefab` instance was deliberately **not** dropped into `Gameplay.unity` yet (same reasoning as Sprint 2) — see Sprint 3 Report §12 item 1.

## Sprint 4 notes

- See [`docs/07-sprints/SPRINT-04-MULTIPLAYER-FOUNDATION.md`](../docs/07-sprints/SPRINT-04-MULTIPLAYER-FOUNDATION.md) for the full Sprint Report, and [`docs/adr/0001-multiplayer-transport-abstraction.md`](../docs/adr/0001-multiplayer-transport-abstraction.md) for the (Proposed) Netcode-abstraction ADR.
- Still no licensed Unity Editor on this machine (only Hub) — the new `NetworkSyncConfig.asset`, the new `GulfRun.Features.Multiplayer` assembly definition, and the `Boot.unity`/`Gameplay.unity` scene changes were hand-authored to Unity's on-disk YAML format, then structurally validated and fileID/guid cross-reference-checked offline. All 93 project `.cs` files (Sprints 1–4) were compiled together offline against a hand-written UnityEngine API shim. **First real Editor open remains required** before Play Mode can be exercised.
- `Boot.unity` gained its **first-ever** manager GameObject (`MultiplayerSystems`) — none of Sprint 1's ten `Core.Managers.*` singletons were previously placed in any scene; this sprint does not retroactively fix that for the other ten (out of scope) — see Sprint 4 Report §18 item 2.
- No real multiplayer transport exists yet — `LocalLoopbackTransport` (in-process, no sockets) is the default so the Lobby/Ready/Countdown/Spawn/Sync architecture is fully testable via `MultiplayerDebugView`'s buttons today; swapping in a real transport once ADR-0001 is ratified requires no gameplay-facing code changes.
