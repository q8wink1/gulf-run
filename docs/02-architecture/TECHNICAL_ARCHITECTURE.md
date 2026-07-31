# Technical Architecture Specification

| Field | Value |
|-------|--------|
| Document ID | P049 |
| Title | Technical Architecture Specification |
| Version | **1.0** |
| Status | Approved (architecture principles, layers, core systems & rules scope only) |
| Project | Project GulfRun |
| Location rationale | Software architecture is an **engineering** concern → lives under `docs/02-architecture/` ("System & scale architecture") per [DOCUMENTATION_STRUCTURE.md](../00-governance/DOCUMENTATION_STRUCTURE.md) §3, not `Design/GDD/`. Numbered **P049** for continuity with the ongoing specification brief sequence. |
| Authority | Official source of truth for **architecture principles**, **project layers**, **core system managers (named list)**, **dependency rules**, **code quality principles**, and the **architecture rules** stated herein |
| Relates to (engineering, existing) | [FOLDER_ARCHITECTURE.md](FOLDER_ARCHITECTURE.md) — existing repository layout / assembly definitions (implements the layering described here at the folder level); [CODING_STANDARDS.md](../03-standards/CODING_STANDARDS.md) — existing code-quality / SOLID-adjacent rules; [BACKEND_ARCHITECTURE.md](BACKEND_ARCHITECTURE.md) (P039), [DATABASE_ARCHITECTURE.md](DATABASE_ARCHITECTURE.md) (P040), [MULTIPLAYER_ARCHITECTURE.md](MULTIPLAYER_ARCHITECTURE.md), [TECHNICAL_STACK.md](TECHNICAL_STACK.md), [SCALABILITY_PLAN.md](SCALABILITY_PLAN.md) |
| Last updated | 2026-07-31 |

**Rules:** Documentation only. No implementation. Do not invent gameplay. Do not invent implementation details. Do not change previous specifications. Missing detail → **TODO**.

**Next milestone:** Sprint 1 — do not start until explicitly instructed.

---

## 1. Purpose

Define the software architecture of Project GulfRun: architecture principles, project layers, named core system managers, dependency rules, code quality principles, configuration/testing intent, and architecture rules — without dependency injection framework, folder structure, code generation, build pipeline, testing framework, CI, CD, or plugin strategy choices (those remain engineering implementation, tracked separately where they already exist).

---

## 2. System Overview

| Field | Value |
|-------|--------|
| Approach | Project GulfRun follows a **modular architecture** |
| Independence | **Every major system is independent** |
| Communication | Systems **communicate through well-defined interfaces** |
| Longevity | The architecture **must remain scalable for many years** |

### Alignment

- No conflict with existing [FOLDER_ARCHITECTURE.md](FOLDER_ARCHITECTURE.md), which already implements feature-module independence (`GulfRun.Features.*`, "Features must not reference other Features directly") and well-defined boundaries (Domain/Core/Infrastructure/Presentation assemblies) — this document states the requirements-level principle; the folder doc remains the implementation.
- Per the brief's explicit instruction, **no previous specification is changed** by this document.

---

## 3. Architecture Principles

| Principle | Status |
|-----------|--------|
| **Modular** | Defined |
| **Maintainable** | Defined |
| **Scalable** | Defined |
| **Reusable** | Defined |
| **Testable** | Defined |
| **Readable** | Defined |
| **Secure** | Defined |
| **Performance Oriented** | Defined |

---

## 4. Project Layers

| Layer | Status |
|-------|--------|
| **Presentation Layer** | Defined (existence only) |
| **Game Logic Layer** | Defined (existence only) |
| **Gameplay Systems** | Defined (existence only) |
| **Networking Layer** | Defined (existence only) |
| **Backend Layer** | Defined (existence only) |
| **Data Layer** | Defined (existence only) |
| **Persistence Layer** | Defined (existence only) |
| **Tools Layer** | Defined (existence only) |

### TODO — Project Layers (not provided)

- [ ] Formal mapping of each layer to existing `Client/` / `Server/` / `Shared/` / `Tools/` folders ([FOLDER_ARCHITECTURE.md](FOLDER_ARCHITECTURE.md)) — no such mapping is stated in the brief; not invented here

---

## 5. Core Systems

Named core system managers:

| Manager | Status |
|---------|--------|
| **Game Manager** | Defined (existence only) |
| **Scene Manager** | Defined (existence only) |
| **Player Manager** | Defined (existence only) |
| **UI Manager** | Defined (existence only) |
| **Audio Manager** | Defined (existence only) |
| **Input Manager** | Defined (existence only) |
| **Network Manager** | Defined (existence only) |
| **Backend Manager** | Defined (existence only) |
| **Economy Manager** | Defined (existence only) |
| **Analytics Manager** | Defined (existence only) |
| **Future Managers** | Future |

### TODO — Core Systems (not provided)

- [ ] Manager responsibilities / interfaces / lifecycle
- [ ] Relationship between named Managers and existing `GulfRun.Features.*` asmdefs ([FOLDER_ARCHITECTURE.md](FOLDER_ARCHITECTURE.md) §4)

---

## 6. Dependency Rules

| Rule ID | Rule |
|---------|------|
| DEP-001 | Systems **must remain loosely coupled**. |
| DEP-002 | **Avoid circular dependencies**. |
| DEP-003 | **Core systems must not directly depend on UI**. |
| DEP-004 | **Gameplay systems should remain independent from rendering**. |

### Alignment

Consistent with existing [FOLDER_ARCHITECTURE.md](FOLDER_ARCHITECTURE.md) §12 Forbidden patterns ("Circular feature references") and [CODING_STANDARDS.md](../03-standards/CODING_STANDARDS.md) §3 Architecture ("Domain code MUST NOT reference UnityEngine APIs...", "Features communicate via Domain interfaces / messages; no circular feature refs") — reinforced, not redefined.

---

## 7. Code Quality

| Rule | Status |
|------|--------|
| **Follow SOLID principles** | Defined |
| **Avoid duplicated logic** | Defined |
| **Keep functions small** | Defined |
| **Use clear naming** | Defined |
| **Maintain consistent coding standards** | Defined |

### Alignment

Consistent with existing [CODING_STANDARDS.md](../03-standards/CODING_STANDARDS.md) (naming conventions, one-PR-one-purpose, structured logging, etc.) — this document states the requirements-level principles; `CODING_STANDARDS.md` remains the detailed engineering standard.

---

## 8. Configuration

| Field | Value |
|-------|--------|
| Intent | Game configuration **should support centralized management** |
| Implementation | **Not defined** |

### TODO — Configuration (not provided)

- [ ] Configuration implementation approach

---

## 9. Testing

| Field | Value |
|-------|--------|
| Intent | Systems **should support automated testing** |
| Strategy | **Not defined** |

### Alignment

[CODING_STANDARDS.md](../03-standards/CODING_STANDARDS.md) §7 Testing expectations already states per-layer testing expectations (Domain unit tests, API contract tests, Unity EditMode/PlayMode, Economy simulation tests) — this document does not redefine that; formal "Testing Framework" choice remains not defined per this brief (§13).

### TODO — Testing (not provided)

- [ ] Testing strategy / framework choice (Testing Framework explicitly not defined)

---

## 10. Rules

| Rule ID | Rule |
|---------|------|
| ARCH-001 | Architecture decisions **must prioritize long-term maintainability**. |
| ARCH-002 | Performance optimizations **must not reduce code quality**. |
| ARCH-003 | Future systems **should integrate without major refactoring**. |

---

## 11. Dependencies

| Dependency | Note |
|------------|------|
| FOLDER_ARCHITECTURE.md | Existing repository layout implementing modular/layered structure at the folder level |
| CODING_STANDARDS.md | Existing code-quality / naming / testing-expectation detail |
| BACKEND_ARCHITECTURE.md (P039) | Backend Layer detail |
| DATABASE_ARCHITECTURE.md (P040) | Data Layer / Persistence Layer detail |
| MULTIPLAYER_ARCHITECTURE.md | Networking Layer detail |
| TECHNICAL_STACK.md | Stack choices underlying these layers |
| SCALABILITY_PLAN.md | Long-term scalability principle reinforcement |

---

## 12. Future Specifications

| Topic | Status |
|-------|--------|
| Dependency Injection Framework | Not defined |
| Folder Structure | Not defined *(by this brief — existing FOLDER_ARCHITECTURE.md is a separate, already-approved document; not superseded)* |
| Code Generation | Not defined |
| Build Pipeline | Not defined |
| Testing Framework | Not defined |
| Continuous Integration | Not defined |
| Continuous Deployment | Not defined |
| Plugin Strategy | Not defined |
| Future Managers | Future |

---

## 13. Explicitly Not Defined (P049)

- Dependency Injection Framework
- Folder Structure
- Code Generation
- Build Pipeline
- Testing Framework
- Continuous Integration
- Continuous Deployment
- Plugin Strategy

---

## 14. Open Questions

| ID | Question |
|----|----------|
| Q-P049-001 | Formal mapping of the 8 Project Layers to existing `Client/`/`Server/`/`Shared/`/`Tools/` folders? |
| Q-P049-002 | Core Manager responsibilities / interfaces / lifecycle, and relationship to existing `GulfRun.Features.*` asmdefs? |
| Q-P049-003 | Configuration implementation approach (centralized management mechanism)? |
| Q-P049-004 | Testing strategy / framework choice? |
| Q-P049-005 | Dependency Injection Framework, Code Generation, Build Pipeline, CI, CD, Plugin Strategy — ADR timeline? |

---

## 15. Acceptance Criteria

P049 v1.0 is satisfied when all of the following are true:

1. Modular architecture confirmed; every major system independent; systems communicate through well-defined interfaces; scalable for many years.
2. Architecture Principles: Modular, Maintainable, Scalable, Reusable, Testable, Readable, Secure, Performance Oriented.
3. Project Layers: Presentation, Game Logic, Gameplay Systems, Networking, Backend, Data, Persistence, Tools.
4. Core Systems named: Game Manager, Scene Manager, Player Manager, UI Manager, Audio Manager, Input Manager, Network Manager, Backend Manager, Economy Manager, Analytics Manager; Future Managers noted as future.
5. Dependency Rules: loose coupling; no circular dependencies; core systems independent from UI; gameplay systems independent from rendering.
6. Code Quality: SOLID principles; no duplicated logic; small functions; clear naming; consistent coding standards.
7. Configuration: centralized management intent; implementation not defined.
8. Testing: automated testing supported; strategy not defined.
9. Rules: long-term maintainability prioritized; performance optimizations must not reduce code quality; future systems integrate without major refactoring.
10. Dependency Injection Framework, Folder Structure, Code Generation, Build Pipeline, Testing Framework, CI, CD, and Plugin Strategy are not invented.
11. No gameplay or implementation details invented beyond this brief; no previous specification altered.
12. Document version is **P049 v1.0**.

---

## 16. Document Queue (cross-reference to GDD specification sequence)

| Order | Document ID | Title | Status |
|-------|-------------|-------|--------|
| 1–48 | P001–P048 | (prior specs) | Approved as previously recorded |
| 49 | P049 | Technical Architecture Specification (`docs/02-architecture/`) | v1.0 Approved |
| 50 | P050 | Master Design Bible Specification (`Design/GDD/`) | v1.0 Approved |
| — | Sprint 1 | _(await instructions)_ | Not started |

---

## 17. Change Log

| Version | Date | Summary | Author |
|---------|------|---------|--------|
| **1.0** | 2026-07-31 | Initial Technical Architecture Specification | Documentation Engineer (from brief) |

---

*End of document.*
