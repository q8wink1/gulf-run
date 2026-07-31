# P037 — Localization System Specification

| Field | Value |
|-------|--------|
| Document ID | P037 |
| Title | Localization System Specification |
| Version | **1.0** |
| Status | Approved (Localization system scope only) |
| Project | Project GulfRun |
| Authority | Official source of truth for **supported languages**, **localized content scope**, **text rules**, **RTL support**, **font support**, **player language controls**, and **localization rules** stated herein |
| Depends on | [P001](P001-GAME-VISION-v1.0.md), [P004](P004-MAIN-MENU-v1.0.md), [P026](P026-DAILY-CHALLENGES-SYSTEM-v1.0.md), [P027](P027-WEEKLY-CHALLENGES-SYSTEM-v1.0.md), [P028](P028-ACHIEVEMENT-SYSTEM-v1.0.md), [P029](P029-BATTLE-PASS-SYSTEM-v1.0.md), [P031](P031-LIVE-EVENTS-SYSTEM-v1.0.md), [P032](P032-NOTIFICATION-SYSTEM-v1.0.md), [P033](P033-INBOX-MAIL-SYSTEM-v1.0.md), [P034](P034-SETTINGS-SYSTEM-v1.0.md) |
| Last updated | 2026-07-31 |

**Rules:** Documentation only. No implementation. Do not invent features beyond this brief. Missing detail → **TODO**.

**Next milestone:** Sprint 1 — do not start until explicitly instructed.

---

## 1. Purpose

Define the Localization System for Project GulfRun: launch-language support (Arabic, English), the full localized-content scope, no-hardcoded-text and localization-key rules, mandatory Arabic RTL support, font support requirements, player language controls, and fallback/maintainability rules — without inventing additional languages, regional dialects, voice languages, localized media, or translation methodology.

---

## 2. System Overview

| Field | Value |
|-------|--------|
| Support | Project GulfRun **supports multiple languages** |
| Consistency | Localization **must provide a consistent experience across all supported platforms** |
| Coverage | **Every user-facing text must be localizable** |

### Alignment

- P034 Settings Language category states multiple languages are supported with the list undefined — **this document** resolves the **Official Launch Languages** (Arabic, English) that P034 deferred.
- P001 Gulf Identity pillar and primary/secondary audience (Gulf culture enthusiasts) align with Arabic as an official launch language — no explicit P001 language statement exists beyond this brief; **do not infer further**.
- Chapter 29 (Accessibility & Localization) is a template awaiting this document — synced below.

---

## 3. Supported Languages

| Field | Value |
|-------|-------|
| Official Launch Languages | **Arabic**, **English** |
| Future languages | **Will be added later** |

### TODO — Supported Languages (not provided)

- [ ] Which future languages, and timeline
- [ ] Regional dialects (e.g., Arabic dialect variants)

---

## 4. Localized Content

The following content must support localization:

| Content Area | Status |
|---------------|--------|
| **User Interface** | Defined |
| **Menus** | Defined |
| **Buttons** | Defined |
| **Dialogs** | Defined |
| **Notifications** | Defined — see [P032](P032-NOTIFICATION-SYSTEM-v1.0.md) |
| **Challenges** | Defined — see [P026](P026-DAILY-CHALLENGES-SYSTEM-v1.0.md) / [P027](P027-WEEKLY-CHALLENGES-SYSTEM-v1.0.md) |
| **Achievements** | Defined — see [P028](P028-ACHIEVEMENT-SYSTEM-v1.0.md) |
| **Battle Pass** | Defined — see [P029](P029-BATTLE-PASS-SYSTEM-v1.0.md) |
| **Events** | Defined — see [P031](P031-LIVE-EVENTS-SYSTEM-v1.0.md) |
| **Settings** | Defined — see [P034](P034-SETTINGS-SYSTEM-v1.0.md) |
| **Tutorial** | Defined |
| **Store** | Defined |
| **Player Messages** | Defined |
| **System Messages** | Defined — see [P033](P033-INBOX-MAIL-SYSTEM-v1.0.md) |

### TODO — Localized Content (not provided)

- [ ] Content areas beyond this list (e.g., in-race HUD, character names — not explicitly stated here)
- [ ] "Player Messages" scope definition (chat vs mail vs other)

---

## 5. RTL Support

| Field | Value |
|-------|-------|
| Requirement | **Arabic must fully support Right-To-Left (RTL)** |
| Layouts | **Must adapt correctly** |
| Text alignment | **Must adjust automatically** |

### TODO — RTL Support (not provided)

- [ ] Specific UI components requiring RTL mirroring vs those exempt
- [ ] RTL behavior for mixed Arabic/English/numeric content

---

## 6. Font Support

Fonts must support:

| Requirement | Status |
|-------------|--------|
| **Arabic** | Defined |
| **English** | Defined |
| **Future languages** | Defined (languages not specified) |

### TODO — Font Support (not provided)

- [ ] Specific font families / fallback chains

---

## 7. Text Rules

| Rule ID | Rule |
|---------|------|
| LOC-001 | **No hardcoded user-facing text.** |
| LOC-002 | **All text must be loaded from localization resources.** |
| LOC-003 | **Every text entry requires a localization key.** |

---

## 8. Player Flow

Players may:

| Action | Status |
|--------|--------|
| **Change Language** | Defined |
| **Preview Language** | Defined |

| Field | Value |
|-------|-------|
| Apply timing | **Language changes apply immediately** |

Relates to **[P034](P034-SETTINGS-SYSTEM-v1.0.md)** Settings Language category (Change Language / Preview Language are the player-facing controls for that category).

```
Player opens Settings → Language (P034)
↓
Select Change Language or Preview Language
↓
Language applied immediately (no restart required)
```

### TODO — Player Flow (not provided)

- [ ] Preview Language exact behavior (temporary view vs. commit)
- [ ] Whether Change Language is account-linked or device-local (see P034 §5, unresolved there too)

---

## 9. Localization Rules

| Rule ID | Rule |
|---------|------|
| LOC-004 | Localization **must not affect gameplay**. |
| LOC-005 | Missing translations **safely fall back to English**. |
| LOC-006 | Localization data **must be maintainable and scalable**. |

### TODO — Localization Rules (not provided)

- [ ] Fallback chain if English itself is missing a key
- [ ] Translation pipeline / maintainability tooling details

---

## 10. Voice Localization

| Field | Value |
|-------|-------|
| Status | Exists as a **future system** |
| Voice languages | **Not defined** |

---

## 11. Dependencies

| Dependency | Note |
|------------|------|
| P001 | Gulf Identity / audience context (no explicit language statement) |
| P004 Main Menu | UI text scope |
| P026 / P027 Challenges | Localized challenge text |
| P028 Achievements | Localized achievement text |
| P029 Battle Pass | Localized Battle Pass text |
| P031 Live Events | Localized event text |
| P032 Notifications | Localized notification text; Localization Rules previously flagged not defined there — resolved partially here |
| P033 Inbox (Mail) | Localized system message text |
| P034 Settings | Language category; Change Language / Preview Language controls |
| Chapter 29 | Accessibility & Localization — synced to this document |

---

## 12. Future Specifications

| Topic | Status |
|-------|--------|
| Future languages | Future (list TBD) |
| Voice Localization | Future system (languages not defined) |
| Additional Languages | Not defined |
| Regional Dialects | Not defined |
| Voice Languages | Not defined |
| Localized Images | Not defined |
| Localized Audio | Not defined |
| Localized Videos | Not defined |
| Machine Translation | Not defined |
| Community Translation | Not defined |

---

## 13. Explicitly Not Defined (P037)

- Additional Languages
- Regional Dialects
- Voice Languages
- Localized Images
- Localized Audio
- Localized Videos
- Machine Translation
- Community Translation

---

## 14. Open Questions

| ID | Question |
|----|----------|
| Q-P037-001 | Which future languages and timeline? |
| Q-P037-002 | Regional dialect handling (e.g., Arabic variants)? |
| Q-P037-003 | "Player Messages" scope — chat, mail, or other? |
| Q-P037-004 | RTL behavior for mixed Arabic/English/numeric content? |
| Q-P037-005 | Specific font families / fallback chains? |
| Q-P037-006 | Preview Language exact behavior (temporary vs commit)? |
| Q-P037-007 | Is Change Language account-linked or device-local? |
| Q-P037-008 | Fallback chain if English key itself is missing? |
| Q-P037-009 | Voice Localization languages and timeline? |
| Q-P037-010 | Localized Images / Audio / Videos, Machine Translation, Community Translation — future or never? |

---

## 15. Acceptance Criteria

P037 v1.0 is satisfied when all of the following are true:

1. Multiple languages supported; consistent experience across platforms; every user-facing text localizable.
2. Official Launch Languages: Arabic, English; future languages added later.
3. Localized content scope: UI, Menus, Buttons, Dialogs, Notifications, Challenges, Achievements, Battle Pass, Events, Settings, Tutorial, Store, Player Messages, System Messages.
4. Text rules: no hardcoded text; all text from localization resources; every entry has a localization key.
5. Arabic fully supports RTL; layouts adapt; text alignment adjusts automatically.
6. Fonts support Arabic, English, and future languages.
7. Voice localization exists as a future system; voice languages not defined.
8. Player controls: Change Language, Preview Language; changes apply immediately.
9. Rules: localization must not affect gameplay; missing translations fall back to English; localization data maintainable and scalable.
10. Additional Languages, Regional Dialects, Voice Languages, Localized Images, Localized Audio, Localized Videos, Machine Translation, and Community Translation are not invented.
11. Document version is **P037 v1.0**.

---

## 16. Document Queue

| Order | Document ID | Title | Status |
|-------|-------------|-------|--------|
| 1–36 | P001–P036 | (prior specs) | Approved as previously recorded |
| 37 | P037 | Localization System Specification | **v1.0 Approved** |
| 38 | P038 | Tutorial System Specification | **v1.0 Approved** — [P038](P038-TUTORIAL-SYSTEM-v1.0.md) |
| 39 | P039 | Backend Architecture Specification (engineering doc — docs/02-architecture/) | **v1.0 Approved** |
| 40 | P040 | Database Architecture Specification (engineering doc — docs/02-architecture/) | **v1.0 Approved** |
| 41 | P041 | Authentication System Specification (engineering doc — docs/02-architecture/) | **v1.0 Approved** |
| 42 | P042 | Player Profile System Specification [CONFLICT with P020] | **v1.0 Approved-per-brief** |
| 43 | P043 | Anti-Cheat System Specification (engineering doc — docs/05-security/) | **v1.0 Approved** |
| 44 | P044 | Analytics System Specification (engineering doc — docs/02-architecture/) | **v1.0 Approved** |
| 45 | P045 | Monetization System Specification | **v1.0 Approved** |
| 46 | P046 | Performance Optimization Specification | **v1.0 Approved** |
| 47 | P047 | UI / UX Design System Specification | **v1.0 Approved** |
| 48 | P048 | Art Direction & Visual Style Specification | **v1.0 Approved** |
| 49 | P049 | Technical Architecture Specification | **v1.0 Approved** |
| 50 | P050 | Master Design Bible Specification | **v1.0 Approved** |
| — | Sprint 1 | _(await instructions)_ | Not started |

---

## 17. Change Log

| Version | Date | Summary | Author |
|---------|------|---------|--------|
| **1.0** | 2026-07-31 | Initial Localization System Specification | Documentation Engineer (from Design Owner brief) |

---

*End of document.*
