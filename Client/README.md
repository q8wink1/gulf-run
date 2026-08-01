# Client

Unity mobile game client for GulfRun.

**Status:** Sprint 3 — Endless Runner Core complete. World generation (chunk pooling/cleanup), modular obstacle/coin/power-up/decoration spawning, a generic object pool, global game speed, distance, scoring, and the Ready/Running/Paused/GameOver/Restart game loop are implemented; no final art/animations yet.

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
