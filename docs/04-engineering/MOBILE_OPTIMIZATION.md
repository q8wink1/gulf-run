# Mobile Optimization Strategy

**Last updated:** 2026-07-31  
**Owner:** Client Lead + Tech Art  
**Audience:** Client eng, Art, QA

**Requirements-level companion:** [PERFORMANCE_OPTIMIZATION_SPECIFICATION.md](PERFORMANCE_OPTIMIZATION_SPECIFICATION.md) (P046 v1.0) is the official source of truth for target/minimum frame rate, optimization principles, and performance rules. This document remains the detailed engineering strategy (device tiers, budgets, rendering/CPU/memory/network/app-size/profiling).

---

## 1. Philosophy

GulfRun MUST feel stable on **Low-tier** devices common in target markets, while scaling fidelity on High-tier. Optimization is continuous, budget-driven, and enforced in CI — not a pre-launch panic.

## 2. Device tiers

| Tier | Definition (planning) | Target UX |
|------|----------------------|-----------|
| Low | Older chipsets / 4 GB class (exact SKUs in QA matrix) | 30 FPS locked, reduced FX |
| Mid | Mainstream year-1 devices | 30–60 adaptive |
| High | Flagship | 60 FPS where thermal allows |

Exact SKU list lives in `QA/device-matrix/` (created at M1). Soft Launch markets drive Low-tier selection.

## 3. Performance budgets (initial placeholders — lock numbers at M1)

| Metric | Low | Mid | High |
|--------|-----|-----|------|
| Target FPS | 30 | 30/60 | 60 |
| Frame time p95 | ≤33 ms | ≤33/16 ms | ≤16 ms |
| Peak working set | TBD MB | TBD | TBD |
| Cold start to interactive | TBD s | TBD | TBD |
| Thermal throttling after 10 min | Accept mild | Minimal | Minimal |

Budgets MUST be filled with measured numbers before M2 exit.

## 4. Rendering strategy

- Mobile-friendly pipeline (URP recommended) locked via ADR.
- Quality levels: Low/Mid/High driven by Adaptive Performance + user setting.
- Aggressive LOD, occlusion where beneficial, baked GI preferred over heavy realtime.
- Limit realtime lights/shadows on Low.
- ASTC textures; atlas UI; avoid read/write meshes.

## 5. CPU / gameplay

- Avoid GC in hot loops; pool frequently spawned objects.
- Job System / Burst only where profiler shows gain.
- AI/simulation budgets per mode; server does heavy authoritative sim when online.
- UI rebuilds minimized; avoid per-frame layout thrash.

## 6. Memory

- Addressables unload policies per scene transition.
- Texture streaming where applicable.
- Audio memory caps; compress aggressively for SFX banks.
- Watch IL2CPP binary + metadata size.

## 7. Networking & battery

- Batch meta RPCs; cache configs by version.
- Adaptive send rates on poor networks.
- Background: no active session sockets when suspended; reconnect protocol documented.
- Prefer idle-friendly push over polling.

## 8. App size & updates

- Strict first-install budget; large content remote.
- On-demand Addressables packs for seasons/modes.
- CI fails release builds over size budget.
- Track Android compressed download size and iOS install size separately.

## 9. Profiling ritual

| Cadence | Activity |
|---------|----------|
| PR (perf-sensitive) | Profiler capture note |
| Weekly | Low-tier soak of vertical slice / live modes |
| Milestone | Memory + FPS formal report |
| Pre-release | 30-minute thermal session on Low/Mid |

Tools: Unity Profiler, Memory Profiler, Frame Debugger, platform GPU tools, Perfetto/Android Studio as needed.

## 10. Automated guards

- Frame timing smoke on device farm subset for release branches.
- Allocation unit tests for known hot systems where feasible.
- Addressables analyze for duplicate assets / huge bundles.

## 11. Degradation levers (runtime)

Remote/config driven:

- FX density
- Shadow quality
- Crowd/prop density
- Match VFX
- 60→30 FPS soft cap when thermal state critical

## 12. Ownership

| Area | Owner |
|------|-------|
| Budgets & reports | Client Lead |
| Art density | Tech Art |
| Net bandwidth | Gameplay Net eng |
| Binary size | Client Lead + DevOps |
| Device lab | QA |

## 13. Exit criteria linkage

- **M2:** Low + Mid budgets met for slice  
- **M5:** Soak targets met; size budget green  
- **M7:** No unresolved Critical perf bugs on launch modes  
