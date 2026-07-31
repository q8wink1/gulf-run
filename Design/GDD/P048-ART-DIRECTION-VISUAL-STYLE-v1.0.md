# P048 — Art Direction & Visual Style Specification

| Field | Value |
|-------|--------|
| Document ID | P048 |
| Title | Art Direction & Visual Style Specification |
| Version | **1.0** |
| Status | Approved (art direction philosophy, style pillars & rules scope only) |
| Project | Project GulfRun |
| Authority | Official source of truth for **art style pillars**, **graphics approach**, **character/environment/animation style rules**, **visual effects rules**, and **optimization/consistency rules** stated herein |
| Depends on | [P001](P001-GAME-VISION-v1.0.md) (Graphics Style, Gulf Identity), [P005](P005-CHARACTER-SYSTEM-v1.0.md), [P006](P006-MAP-SYSTEM-v1.0.md), [P047](P047-UI-UX-DESIGN-SYSTEM-v1.0.md) |
| Last updated | 2026-07-31 |

**Rules:** Documentation only. No implementation. Do not invent gameplay. Do not invent art assets. Do not add implementation details beyond this brief. Missing detail → **TODO**.

**Next milestone:** Sprint 1 — do not start until explicitly instructed.

---

## 1. Purpose

Define the visual identity of Project GulfRun: art style pillars, graphics approach, character/environment/animation style rules, visual effects rules, and optimization/consistency rules — without character concepts, map concepts, color palette, material library, animation library, visual effect library, lighting rules, or shader library.

---

## 2. System Overview

| Field | Value |
|-------|--------|
| Identity | Project GulfRun uses a **unique Gulf-inspired visual identity** |
| Originality | The visual style **must be original** |
| Non-imitation | The game **must not imitate or copy existing commercial games** |
| Cultural respect | The game **must represent Gulf culture respectfully** |

### Alignment

- [P001](P001-GAME-VISION-v1.0.md) §3.4 Gulf Identity — "celebrate Gulf culture respectfully through original characters, maps, music, environments and cosmetics"; Graphics Style = **Stylized Low Poly Cartoon** — fully consistent with §3/§4 below.
- [P006](P006-MAP-SYSTEM-v1.0.md) MAP-001/MAP-002/MAP-003 (unique visual identity, respectful Gulf culture, original environments) — reinforced, not redefined.
- [P047](P047-UI-UX-DESIGN-SYSTEM-v1.0.md) §4 Visual Style (Stylized Cartoon, Bright Colors, High Contrast) — this document covers the broader art-direction pillars (characters/environments/3D graphics); no conflict.

---

## 3. Art Direction

### 3.1 Art Style

| Pillar | Status |
|--------|--------|
| **Stylized** | Defined |
| **Cartoon** | Defined |
| **Colorful** | Defined |
| **Readable** | Defined |
| **Expressive** | Defined |
| **High Visibility** | Defined |
| **Mobile Optimized** | Defined |

### 3.2 Graphics Approach

| Element | Value |
|---------|-------|
| **Geometry** | 3D Low Poly |
| **Materials** | Stylized Materials |
| **Geometry cleanliness** | Clean Geometry |
| **Models** | Optimized Models |
| **Lighting** | Consistent Lighting |

### 3.3 Colors

| Element | Value |
|---------|-------|
| **Colors** | Bright Colors |
| **Palette theme** | Warm Desert Palette |
| **Contrast** | High Contrast |
| **UI** | Readable User Interface |
| **Future** | Future Color Palette Specification |

### TODO — Art Direction (not provided)

- [ ] Exact Color Palette (hex values / tokens — Future Color Palette Specification)

---

## 4. Character Style

| Rule | Value |
|------|-------|
| Proportions | Characters **use exaggerated cartoon proportions** |
| Readability in motion | Characters **must remain readable while running** |
| Animation clarity | **Animations must clearly communicate movement** |

### Alignment

Consistent with [P005](P005-CHARACTER-SYSTEM-v1.0.md), which does not define art style (avatar/portrait art references remain TODO there) — this document supplies the style pillars without inventing specific character concepts.

### TODO — Character Style (not provided)

- [ ] Character Concepts (explicitly not defined)

---

## 5. Environment Style

| Rule | Value |
|------|-------|
| Inspiration | Maps **are inspired by Gulf cities** |
| Uniqueness | **Every map has a unique visual identity** |
| Readability priority | **Environment readability is prioritized over realism** |

### Alignment

Consistent with [P006](P006-MAP-SYSTEM-v1.0.md) MAP-001 (unique visual identity per map) and MAP-002 (respectful Gulf culture representation) — reinforced, not redefined.

### TODO — Environment Style (not provided)

- [ ] Map Concepts (explicitly not defined)

---

## 6. Animation Style

Animations should feel:

| Quality | Status |
|---------|--------|
| **Responsive** | Defined |
| **Fun** | Defined |
| **Expressive** | Defined |
| **Fluid** | Defined |
| **Readable** | Defined |

### TODO — Animation Style (not provided)

- [ ] Animation Library (explicitly not defined)

---

## 7. Visual Effects

| Rule ID | Rule |
|---------|------|
| VFX-001 | Visual effects **must improve gameplay readability**. |
| VFX-002 | Effects **should never block player visibility**. |

### TODO — Visual Effects (not provided)

- [ ] Visual Effect Library (explicitly not defined)

---

## 8. Optimization Rules

| Rule ID | Rule |
|---------|------|
| ART-OPT-001 | Assets **must be optimized for mobile devices**. |
| ART-OPT-002 | **Memory usage should remain efficient**. |
| ART-OPT-003 | **Draw calls should be minimized**. |

### Alignment

Consistent with [P046](../../docs/04-engineering/PERFORMANCE_OPTIMIZATION_SPECIFICATION.md) (Performance Optimization Specification) and existing [MOBILE_OPTIMIZATION.md](../../docs/04-engineering/MOBILE_OPTIMIZATION.md) engineering strategy — this document adds the art-asset-level optimization rules; no conflict.

---

## 9. Rules (Consistency)

| Rule ID | Rule |
|---------|------|
| ART-001 | **Visual consistency is required**. |
| ART-002 | **Every new asset must follow the same artistic direction**. |
| ART-003 | **Performance has priority over excessive visual complexity**. |

---

## 10. Dependencies

| Dependency | Note |
|------------|------|
| P001 Game Vision | Graphics Style (Stylized Low Poly Cartoon), Gulf Identity pillar |
| P005 Character System | Character content this style applies to |
| P006 Map System | Map content this style applies to; MAP-001/002/003 reinforced |
| P047 UI/UX Design System | UI-layer visual style (Stylized Cartoon, Bright Colors, High Contrast) |
| PERFORMANCE_OPTIMIZATION_SPECIFICATION.md (P046) / MOBILE_OPTIMIZATION.md | Engineering performance context for asset optimization rules |

---

## 11. Future Specifications

| Topic | Status |
|-------|--------|
| Character Concepts | Not defined |
| Map Concepts | Not defined |
| Color Palette | Not defined |
| Material Library | Not defined |
| Animation Library | Not defined |
| Visual Effect Library | Not defined |
| Lighting Rules | Not defined |
| Shader Library | Not defined |

---

## 12. Explicitly Not Defined (P048)

- Character Concepts
- Map Concepts
- Color Palette
- Material Library
- Animation Library
- Visual Effect Library
- Lighting Rules
- Shader Library

---

## 13. Open Questions

| ID | Question |
|----|----------|
| Q-P048-001 | Character Concepts — document ID / timeline? |
| Q-P048-002 | Map Concepts — document ID / timeline? |
| Q-P048-003 | Exact Color Palette (Future Color Palette Specification)? |
| Q-P048-004 | Material Library / Shader Library / Lighting Rules — ADR or art bible timeline? |
| Q-P048-005 | Animation Library / Visual Effect Library — document ID? |

---

## 14. Acceptance Criteria

P048 v1.0 is satisfied when all of the following are true:

1. Unique Gulf-inspired visual identity confirmed; original; must not imitate existing commercial games; represents Gulf culture respectfully.
2. Art Style: Stylized, Cartoon, Colorful, Readable, Expressive, High Visibility, Mobile Optimized.
3. Graphics: 3D Low Poly, Stylized Materials, Clean Geometry, Optimized Models, Consistent Lighting.
4. Characters: exaggerated cartoon proportions; readable while running; animations clearly communicate movement.
5. Environments: inspired by Gulf cities; every map has a unique visual identity; readability prioritized over realism.
6. Colors: Bright Colors, Warm Desert Palette, High Contrast, Readable UI; Future Color Palette Specification noted as future.
7. Animations should feel: Responsive, Fun, Expressive, Fluid, Readable.
8. Visual effects improve gameplay readability; never block player visibility.
9. Optimization: assets optimized for mobile; memory efficient; draw calls minimized.
10. Rules: visual consistency required; every new asset follows the same artistic direction; performance priority over excessive visual complexity.
11. Character Concepts, Map Concepts, Color Palette, Material Library, Animation Library, Visual Effect Library, Lighting Rules, and Shader Library are not invented.
12. No gameplay or art assets invented beyond this brief.
13. Document version is **P048 v1.0**.

---

## 15. Document Queue

| Order | Document ID | Title | Status |
|-------|-------------|-------|--------|
| 1–46 | P001–P046 | (prior specs) | Approved as previously recorded |
| 47 | P047 | UI / UX Design System Specification | v1.0 Approved |
| 48 | P048 | Art Direction & Visual Style Specification | **v1.0 Approved** |
| 49 | P049 | Technical Architecture Specification | **v1.0 Approved** |
| 50 | P050 | Master Design Bible Specification | **v1.0 Approved** |
| — | Sprint 1 | _(await instructions)_ | Not started |

---

## 16. Change Log

| Version | Date | Summary | Author |
|---------|------|---------|--------|
| **1.0** | 2026-07-31 | Initial Art Direction & Visual Style Specification | Documentation Engineer (from Design Owner brief) |

---

*End of document.*
