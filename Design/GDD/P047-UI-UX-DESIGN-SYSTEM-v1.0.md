# P047 — UI / UX Design System Specification

| Field | Value |
|-------|--------|
| Document ID | P047 |
| Title | UI / UX Design System Specification |
| Version | **1.0** |
| Status | Approved (design philosophy, principles, style & interaction rules scope only) |
| Project | Project GulfRun |
| Authority | Official source of truth for **UI/UX design principles**, **visual style**, **navigation/button/popup rules**, **HUD existence**, **responsiveness/animation/accessibility requirements** stated herein |
| Depends on | [P001](P001-GAME-VISION-v1.0.md) (Graphics Style, Screen Orientation), [P004](P004-MAIN-MENU-v1.0.md), [08-ui-ux/19-ui-ux-screens-and-flows.md](08-ui-ux/19-ui-ux-screens-and-flows.md) |
| Last updated | 2026-07-31 |

**Rules:** Documentation only. No implementation. Do not invent gameplay. Do not invent UI features. Do not add implementation details beyond this brief. Missing detail → **TODO**.

**Next milestone:** Sprint 1 — do not start until explicitly instructed.

---

## 1. Purpose

Define the visual interface philosophy for Project GulfRun: design principles, visual style, navigation/button/popup rules, HUD existence, responsiveness, animation, and accessibility requirements — without color palette, typography, icon library, spacing rules, animation timing, UI grid, design tokens, or dark mode.

---

## 2. System Overview

| Field | Value |
|-------|--------|
| Interface approach | Project GulfRun uses a **modern mobile-first interface** |
| Quality bar | The interface must remain **clean, responsive and easy to understand** |
| Consistency | **Every screen must follow one consistent design language** |

### Alignment

- [P001](P001-GAME-VISION-v1.0.md): Graphics Style = **Stylized Low Poly Cartoon**; Screen Orientation = **Landscape only** — consistent with §4 Visual Style ("Stylized Cartoon") and §3 Design Principles ("Landscape First") below; no conflict.
- [P004](P004-MAIN-MENU-v1.0.md): existing Main Menu button/navigation definitions are **not redefined** here; this document adds the interaction-rule layer (button states, popup rules) that P004 did not specify at that level.
- [08-ui-ux/19-ui-ux-screens-and-flows.md](08-ui-ux/19-ui-ux-screens-and-flows.md): screen inventory / flows chapter — **not redefined**; this document is the design-language/system layer, not a new screen list.

---

## 3. Design Principles

| Principle | Status |
|-----------|--------|
| **Simple** | Defined |
| **Fast** | Defined |
| **Modern** | Defined |
| **Readable** | Defined |
| **Consistent** | Defined |
| **Accessible** | Defined |
| **Mobile First** | Defined |
| **Landscape First** | Defined |

---

## 4. Visual Style

| Element | Value |
|---------|-------|
| **Style** | Stylized Cartoon |
| **Colors** | Bright Colors |
| **Contrast** | High Contrast |
| **UI Elements** | Rounded UI Elements |
| **Shadows** | Soft Shadows |
| **Animations** | Smooth Animations |

### TODO — Visual Style (not provided)

- [ ] Color Palette (explicit hex/tokens)
- [ ] Typography
- [ ] Icon Library
- [ ] Design Tokens

---

## 5. Navigation Rules

| Rule ID | Rule |
|---------|------|
| NAV-001 | Navigation **must require the minimum number of taps**. |
| NAV-002 | **Every important feature should be reachable quickly**. |
| NAV-003 | **Back navigation must always be predictable**. |

### Alignment

Reinforces [P004](P004-MAIN-MENU-v1.0.md) §8 Navigation Flow and [19-ui-ux-screens-and-flows.md](08-ui-ux/19-ui-ux-screens-and-flows.md) §19.5 Navigation model — no new screens or flows added here.

---

## 6. Button Rules

| Rule ID | Rule |
|---------|------|
| BTN-001 | Buttons **must clearly communicate their purpose**. |
| BTN-002 | Buttons **must provide visual feedback**. |
| BTN-003 | Buttons **must have pressed, focused and disabled states**. |

### TODO — Buttons (not provided)

- [ ] Exact state visuals (color/animation per state — Design Tokens / Animation Timing not defined)

---

## 7. Popup Rules

| Rule ID | Rule |
|---------|------|
| POP-001 | Popups **must never block gameplay unexpectedly**. |
| POP-002 | **Confirmation dialogs must be used for destructive actions**. |

---

## 8. HUD Overview

| Field | Value |
|-------|-------|
| Existence | **The gameplay HUD exists** |
| Elements | **HUD elements are defined in future specifications** |

### TODO — HUD (not provided)

- [ ] HUD element list and layout (future specification)

---

## 9. Responsiveness

| Field | Value |
|-------|-------|
| Screen sizes | **UI must support different screen sizes** |
| Tablets | **UI must support tablets** |
| Scaling strategy | **Not defined** |

### Alignment

- [03-platforms-and-constraints.md](00-front-matter/03-platforms-and-constraints.md) §3.3 previously listed tablet support as **Open** (Q-03-003) — this document confirms **tablets must be supported**; UI scaling strategy remains open (see §13).

### TODO — Responsiveness (not provided)

- [ ] UI scaling strategy (see also [MOBILE_OPTIMIZATION.md](../../docs/04-engineering/MOBILE_OPTIMIZATION.md) for related device-tier engineering context)

---

## 10. Animation

| Rule ID | Rule |
|---------|------|
| ANIM-001 | UI animations **should feel smooth**. |
| ANIM-002 | Animations **should never delay gameplay**. |

### TODO — Animation (not provided)

- [ ] Animation Timing values

---

## 11. Accessibility

| Field | Value |
|-------|-------|
| Text | **Support scalable text** |
| Color | **Support color-friendly design** |
| Additional options | **Not defined** |

### Alignment

- Feeds [14-accessibility-loc/29-accessibility-and-localization.md](14-accessibility-loc/29-accessibility-and-localization.md) §29.1/§29.2 (Accessibility requirements; Color, text, motion, input assists), previously template-only — see §14 below for reconciliation.

### TODO — Accessibility (not provided)

- [ ] Additional accessibility options (motor, cognitive, screen reader, etc.)

---

## 12. Dependencies

| Dependency | Note |
|------------|------|
| P001 Game Vision | Graphics Style (Stylized Low Poly Cartoon), Screen Orientation (Landscape only) |
| P004 Main Menu | Existing button/navigation definitions |
| 08-ui-ux/19-ui-ux-screens-and-flows.md | Screen inventory / flows (not redefined) |
| 03-platforms-and-constraints.md | Tablet support question (Q-03-003) |
| 14-accessibility-loc/29-accessibility-and-localization.md | Accessibility chapter — populated by this document (§11) |
| MOBILE_OPTIMIZATION.md / PERFORMANCE_OPTIMIZATION_SPECIFICATION.md (P046) | Related device-tier / responsiveness engineering context |

---

## 13. Future Specifications

| Topic | Status |
|-------|--------|
| Color Palette | Not defined |
| Typography | Not defined |
| Icon Library | Not defined |
| Spacing Rules | Not defined |
| Animation Timing | Not defined |
| UI Grid | Not defined |
| Design Tokens | Not defined |
| Dark Mode | Not defined |
| HUD element list | Future specification |
| UI scaling strategy | Not defined |
| Additional accessibility options | Not defined |

---

## 14. Explicitly Not Defined (P047)

- Color Palette
- Typography
- Icon Library
- Spacing Rules
- Animation Timing
- UI Grid
- Design Tokens
- Dark Mode

---

## 15. Open Questions

| ID | Question |
|----|----------|
| Q-P047-001 | Color Palette, Typography, Icon Library, Spacing Rules, Design Tokens — document ID / timeline? |
| Q-P047-002 | Animation Timing values? |
| Q-P047-003 | UI Grid definition? |
| Q-P047-004 | Dark Mode — planned or out of scope? |
| Q-P047-005 | UI scaling strategy for different screen sizes / tablets? |
| Q-P047-006 | HUD element list — which future specification? |
| Q-P047-007 | Additional accessibility options beyond scalable text and color-friendly design? |

---

## 16. Acceptance Criteria

P047 v1.0 is satisfied when all of the following are true:

1. Modern mobile-first interface confirmed; clean, responsive, easy to understand; one consistent design language across every screen.
2. Design Principles: Simple, Fast, Modern, Readable, Consistent, Accessible, Mobile First, Landscape First.
3. Visual Style: Stylized Cartoon, Bright Colors, High Contrast, Rounded UI Elements, Soft Shadows, Smooth Animations.
4. Navigation: minimum taps; important features reachable quickly; predictable back navigation.
5. Buttons: clear purpose; visual feedback; pressed/focused/disabled states.
6. Popups: never block gameplay unexpectedly; confirmation dialogs for destructive actions.
7. HUD exists; elements deferred to future specifications.
8. Responsiveness: different screen sizes and tablets supported; scaling strategy not defined.
9. Animation: smooth; never delays gameplay.
10. Accessibility: scalable text and color-friendly design supported; additional options not defined.
11. Color Palette, Typography, Icon Library, Spacing Rules, Animation Timing, UI Grid, Design Tokens, and Dark Mode are not invented.
12. No gameplay or UI features invented beyond this brief.
13. Document version is **P047 v1.0**.

---

## 17. Document Queue

| Order | Document ID | Title | Status |
|-------|-------------|-------|--------|
| 1–45 | P001–P045 | (prior specs) | Approved as previously recorded |
| 46 | P046 | Performance Optimization Specification (engineering doc — docs/04-engineering/) | v1.0 Approved |
| 47 | P047 | UI / UX Design System Specification | **v1.0 Approved** |
| 48 | P048 | Art Direction & Visual Style Specification | **v1.0 Approved** |
| 49 | P049 | Technical Architecture Specification | **v1.0 Approved** |
| 50 | P050 | Master Design Bible Specification | **v1.0 Approved** |
| — | Sprint 1 | _(await instructions)_ | Not started |

---

## 18. Change Log

| Version | Date | Summary | Author |
|---------|------|---------|--------|
| **1.0** | 2026-07-31 | Initial UI / UX Design System Specification | Documentation Engineer (from Design Owner brief) |

---

*End of document.*
