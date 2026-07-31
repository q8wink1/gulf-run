# 29 — Accessibility & Localization

**GDD chapter:** 29  
**Status:** Partial — Localization synced to P037 v1.0; Accessibility synced to P047 v1.0 (partial)  
**Design Owner:** `[TBD]`  
**Last updated:** 2026-07-31

> Localization SoT: [P037](../P037-LOCALIZATION-SYSTEM-v1.0.md). Do not invent localization scope beyond the approved spec.  
> Accessibility (UI) SoT: [P047](../P047-UI-UX-DESIGN-SYSTEM-v1.0.md) §11. Do not invent accessibility scope beyond the approved spec.

---

## 29.1 Accessibility requirements

| Requirement | Priority | Notes | Status |
|-------------|----------|-------|--------|
| Scalable text | Defined | UI must support scalable text | **P047 v1.0** |
| Color-friendly design | Defined | UI must support color-friendly design | **P047 v1.0** |
| Additional options | `[TBD]` | Not defined — motor, cognitive, screen reader, etc. | **P047 v1.0** (not defined) |
| Accessibility Audio | `[TBD]` | Explicitly not defined | [P035](../P035-AUDIO-SYSTEM-v1.0.md), [P037](../P037-LOCALIZATION-SYSTEM-v1.0.md) |

## 29.2 Color, text, motion, input assists

Text: scalable text supported (P047 §11). Color: color-friendly design supported (P047 §11). Motion / input assists: **not defined**.

## 29.3 Localization scope

| Locale | Priority | Ship gate | Status |
|--------|----------|-----------|--------|
| Arabic | Official Launch Language | Launch | **P037 v1.0** |
| English | Official Launch Language | Launch | **P037 v1.0** |
| Future languages | Post-launch | TBD | **P037 v1.0** (list not defined) |

Localized content scope: User Interface, Menus, Buttons, Dialogs, Notifications, Challenges, Achievements, Battle Pass, Events, Settings, Tutorial, Store, Player Messages, System Messages. See **[P037](../P037-LOCALIZATION-SYSTEM-v1.0.md)** §4.

Text rules: no hardcoded user-facing text; all text loaded from localization resources; every text entry requires a localization key (**P037** §7).

## 29.4 Cultural / regional content rules

Arabic must **fully support Right-To-Left (RTL)**; layouts adapt correctly; text alignment adjusts automatically (**[P037](../P037-LOCALIZATION-SYSTEM-v1.0.md)** §5).  
Regional Dialects — **not defined** (P037).  
`[QUESTION]` Any content that must differ by region beyond RTL? — see Q-P037-002.

## 29.5 Text expansion / layout constraints

RTL layout adaptation required per **P037** §5. Specific component-level constraints — **TODO** (Q-P037-004).

## 29.6 Fonts

Fonts must support Arabic, English, and future languages (**[P037](../P037-LOCALIZATION-SYSTEM-v1.0.md)** §6). Specific font families / fallback chains — **TODO** (Q-P037-005).

## 29.7 Fallback & maintainability rules

Missing translations safely fall back to English. Localization data must be maintainable and scalable. Localization must not affect gameplay. (**P037** §9)

## 29.8 Voice localization

Exists as a **future system**; voice languages not defined (**P037** §10).

## 29.9 Open questions

| ID | Question | Status |
|----|----------|--------|
| Q-29-001 | `[QUESTION]` Accessibility requirements (motor, cognitive, screen reader) beyond scalable text / color-friendly design (P047)? | Open |
| Q-P037-001 to Q-P037-010 | See [P037](../P037-LOCALIZATION-SYSTEM-v1.0.md) §14 | Open |
