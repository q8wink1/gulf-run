# Recommended Unity Packages

**Last updated:** 2026-07-31  
**Owner:** Client Lead  
**Status:** Allowlist — packages not listed require ADR before adoption

---

## 1. Policy

- Prefer **Unity official** packages pinned to versions compatible with the locked LTS.
- Every third-party package needs: license review, update owner, exit plan.
- Remove unused packages each milestone to fight bloat and compile time.

## 2. Core allowlist (expected at M1+)

| Package | Purpose | Priority |
|---------|---------|----------|
| Unity Addressables | Content delivery | Required |
| Unity Input System | Input | Required |
| Unity UI Toolkit + UI Toolkit Extensions as needed | Meta UI | Required (default) |
| Unity Localization | Strings / assets loc | Required before Soft Launch |
| Unity Analytics **or** none (custom) | Only if not replaced by external | Optional |
| Unity Advertisement / Mediation | Only if ads approved | Optional ADR |
| Unity Purchasing (IAP) | Store IAP client | Required for monetization |
| Unity Profile Analyzer / Memory Profiler | Perf | Dev only |
| Unity Test Framework | Edit/Play tests | Required |
| Collections / Mathematics / Burst | Perf where justified | Allowed |
| Netcode packages | Only if ADR selects Unity netcode | Conditional |

## 3. Recommended supporting packages

| Package | Purpose | Notes |
|---------|---------|-------|
| Scriptable Build Pipeline | Build determinism | With Addressables |
| Mobile Notifications | Local notifications | Abstraction still required |
| Adaptive Performance | Thermal/scalability hooks | Android/iOS as supported |
| Services / Authentication | Evaluate vs custom identity | ADR — avoid lock-in for economy |

## 4. Common third-party categories (examples, not endorsements)

Adopt only via ADR:

| Category | Examples of class | Constraint |
|----------|-------------------|------------|
| DI | VContainer, Zenject/Extenject | One only |
| Tween | DOTween | License compliance |
| JSON | Newtonsoft (if needed) | Prefer Unity JsonUtility/System.Text.Json where enough |
| Crash | Sentry SDK | Interface wrapper |
| Push | Firebase Messaging | Interface wrapper |
| Attr | Adjust/AppsFlyer | Privacy review |

## 5. Explicitly discouraged

- Multiple overlapping UI systems without ADR
- Packages that embed outdated networking assuming host authority for ranked play
- Asset-store “complete multiplayer kits” that fight our architecture
- Heavy editor tools committed into runtime player builds

## 6. Version pinning

- `Packages/manifest.json` and lock file committed.
- Upgrades: changelog review + soak on `develop` + perf smoke on Low tier.

## 7. IL2CPP & stripping

- Packages MUST be compatible with IL2CPP and managed stripping.
- link.xml changes require Client Lead review (attack surface / size).

## 8. Review checklist for new package PR

- [ ] Why not existing allowlist?
- [ ] License + redistribution OK
- [ ] Size / cold start impact estimated
- [ ] Owner for upgrades
- [ ] ADR linked if third-party or architectural
- [ ] Privacy manifest / Android dependencies checked
