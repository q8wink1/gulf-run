# Client

Unity mobile game client for GulfRun.

**Status:** Sprint 2 — Player Controller Foundation complete. Auto-run, jump/double-jump, ground detection, side-scroll camera, and a placeholder Animator Controller are implemented; no final art/animations yet.

**Engine:** Unity **2022.3.62f1 LTS** (installed Editor on this machine). The approved [Technical Stack](../docs/02-architecture/TECHNICAL_STACK.md) targets **Unity 6 LTS**; that Editor version is not installed here — see Sprint 1 Report open item.

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
