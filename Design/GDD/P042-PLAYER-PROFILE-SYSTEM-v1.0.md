# P042 — Player Profile System Specification

| Field | Value |
|-------|--------|
| Document ID | P042 |
| Title | Player Profile System Specification |
| Version | **1.0** |
| Status | **[CONFLICT]** — content documented per brief; **conflicts with [P020](P020-PLAYER-PROFILE-SYSTEM-v1.0.md) (same system title, already Approved v1.0)**; escalated, unresolved — see §0 |
| Project | Project GulfRun |
| Authority | Documents the P042 brief only. **Not yet the sole source of truth** for Player Profile pending conflict resolution with P020 (see §0). |
| Depends on | [P001](P001-GAME-VISION-v1.0.md), [P004](P004-MAIN-MENU-v1.0.md), [P005](P005-CHARACTER-SYSTEM-v1.0.md), [P014](P014-FRIENDS-SYSTEM-v1.0.md), [P015](P015-CLAN-SYSTEM-v1.0.md), [P019](P019-LEADERBOARD-SYSTEM-v1.0.md), [P020](P020-PLAYER-PROFILE-SYSTEM-v1.0.md), [P028](P028-ACHIEVEMENT-SYSTEM-v1.0.md) |
| Last updated | 2026-07-31 |

**Rules:** Documentation only. No implementation. Do not invent gameplay. Do not add implementation details beyond this brief. Missing detail → **TODO**.

**Next milestone:** Sprint 1 — do not start until explicitly instructed.

---

## 0. ⚠ Conflict Notice — P042 vs. P020

Both **P020** and **P042** are titled "Player Profile System Specification" and describe the same system. They were issued as two separate, numbered briefs. Per documentation-engineer policy, conflicting statements are **escalated, not silently resolved**. Neither document is deprecated by this notice; a **Design Owner decision is required**.

| Topic | P020 (existing, Approved) | P042 (this brief) | Conflict type |
|-------|----------------------------|--------------------|----------------|
| Profile Frame | **Future** | **Defined** (currently customizable) | Status conflict |
| Profile Banner / Background | **Future** ("Profile Banner") | **Defined** ("Profile Background" — not confirmed to be the same element as P020's Banner) | Status + naming conflict |
| Display Name editing | **Not resolved** — Q-P020-002 asks whether Edit Profile may change name; rule PP-003 states **only cosmetic elements** may be customized (name is not cosmetic) | **Change Display Name** listed as a defined player action | Rule conflict |
| Experience field | Not listed | **Defined** ("Experience") | Addition, not necessarily conflicting |
| Highest Rank statistic | Not listed | **Defined** | Addition, not necessarily conflicting |
| Achievements statistic | Listed only as "Achievements Display" — **TODO** placement, system is P028 | **Defined** as a profile statistic | Overlap — placement previously TODO in P020, now asserted Defined in P042 |
| Friends Count | Not listed as a profile stat (Friends system is P014) | **Defined** as Social Information | Addition, not necessarily conflicting |
| Clan Information | "Clan (If Joined)" — Profile Information field | "Clan Information" — Social Information field | Naming/grouping difference only |
| Statistics: Losses, First Place Finishes | **Defined** | Not listed | Omission — unclear if intentionally dropped |
| Statistics: Total Races vs Total Matches | "Total Matches" | "Total Races" | Naming difference only |

### Resolution required

- [ ] Design Owner must state which document (P020, P042, or a merged successor) is authoritative for Player Profile.
- [ ] If P042 supersedes P020, P020 should be marked **Deprecated** with a link to P042 (per [DOCUMENTATION_STRUCTURE.md](../../docs/00-governance/DOCUMENTATION_STRUCTURE.md) §9) — **not done here**, since deprecation requires Design Owner / Tech Director approval, not a documentation-engineer decision.
- [ ] If both remain valid (e.g., P020 = full profile screen, P042 = a different profile surface), the relationship must be explicitly named.

Until resolved, **both documents are recorded as Approved-per-brief but mutually conflicting**, and downstream specs (P004, P014, P019) that point to P020 are **not automatically repointed to P042**.

---

## 1. Purpose

Document the P042 brief for the Player Profile System: profile identity/structure fields, player customization options, profile statistics, social information, player actions, and rules — without inventing avatar sources, nickname rules, profile privacy, profile history, favorite statistics, badges, titles, or customization unlock rules. See §0 for the unresolved conflict with P020.

---

## 2. System Overview

| Field | Value |
|-------|--------|
| Ownership | **Every player owns one Player Profile** |
| Content | The Player Profile **stores persistent account information** |
| Sync | Profile data **is synchronized with the backend** |

---

## 3. Profile Structure

| Field | Status |
|-------|--------|
| **Player ID** | Defined |
| **Display Name** | Defined |
| **Avatar** | Defined |
| **Current Character** | Defined |
| **Current Rank** | Defined |
| **Player Level** | Defined |
| **Experience** | Defined |
| **Country** | Future |
| **Online Status** | Defined |
| **Account Creation Date** | Defined |

### TODO — Profile Structure (not provided)

- [ ] Relationship between "Current Character" here and "Selected Character" in P020 — same field, different name, or different concept?
- [ ] Relationship between "Experience" here and P023 Progression XP — same value surfaced, or a separate counter?

---

## 4. Customization

Players may customize:

| Element | Status |
|---------|--------|
| **Display Name** | Defined |
| **Avatar** | Defined |
| **Profile Frame** | Defined |
| **Profile Background** | Defined |
| **Favorite Character** | Defined |
| **Future Customizations** | Future |

**See §0** — Profile Frame/Background status and Display Name editability directly conflict with P020.

### TODO — Customization (not provided)

- [ ] Customization Unlock Rules (explicitly not defined — see §10)
- [ ] Source of Profile Frame / Background assets (Shop, Inventory, Cosmetics — see [P013](P013-SHOP-SYSTEM-v1.0.md) / [P021](P021-INVENTORY-SYSTEM-v1.0.md) / [P022](P022-COSMETICS-SYSTEM-v1.0.md))

---

## 5. Statistics

The profile may display:

| Statistic | Status |
|-----------|--------|
| **Total Races** | Defined |
| **Wins** | Defined |
| **Win Rate** | Defined |
| **Current Rank** | Defined |
| **Highest Rank** | Defined |
| **Achievements** | Defined |
| **Future Statistics** | Future |

### TODO — Statistics (not provided)

- [ ] Win Rate calculation formula
- [ ] Achievements display format (list, count, badges — see [P028](P028-ACHIEVEMENT-SYSTEM-v1.0.md); this document only confirms an "Achievements" stat exists)
- [ ] Whether Losses / First Place Finishes (present in P020) also apply here — not listed in this brief

---

## 6. Social Information

| Field | Status |
|-------|--------|
| **Friends Count** | Defined |
| **Clan Information** | Defined |
| **Online Status** | Defined |
| **Recent Activity** | Future |

---

## 7. Player Actions

| Action | Status |
|--------|--------|
| **View Own Profile** | Defined |
| **View Other Player Profiles** | Defined |
| **Edit Profile** | Defined |
| **Change Avatar** | Defined |
| **Change Display Name** | Defined |

**See §0** — Change Display Name conflicts with P020's cosmetic-only customization rule (PP-003) and open question Q-P020-002.

---

## 8. Rules

| Rule ID | Rule |
|---------|------|
| PP2-001 | Player Profile **is synchronized with the backend**. |
| PP2-002 | **Only editable fields may be modified.** |
| PP2-003 | Profile data **must remain consistent across all devices**. |

### TODO — Rules (not provided)

- [ ] Explicit list of which fields are "editable" (Display Name, Avatar, Profile Frame, Profile Background, Favorite Character per §4 — but see conflict in §0 regarding whether Display Name should be editable per P020's cosmetic-only rule)

---

## 9. Dependencies

| Dependency | Note |
|------------|------|
| [P020](P020-PLAYER-PROFILE-SYSTEM-v1.0.md) | **Conflicting** Player Profile spec — see §0 |
| P001 | Vision context |
| P004 Main Menu | Profile entry point (currently points to P020; not repointed) |
| P005 Characters | Current Character / Favorite Character |
| P014 Friends | Friends Count |
| P015 Clans | Clan Information |
| P019 Leaderboards | Current Rank / Highest Rank context |
| P028 Achievements | Achievements statistic |
| Backend | Profile sync, cross-device consistency |

---

## 10. Future Specifications

| Topic | Status |
|-------|--------|
| Country | Future profile field |
| Future Customizations | Future |
| Future Statistics | Future |
| Recent Activity | Future |
| Avatar Sources | Not defined |
| Nickname Rules | Not defined |
| Profile Privacy | Not defined |
| Profile History | Not defined |
| Favorite Statistics | Not defined |
| Badges | Not defined |
| Titles | Not defined |
| Customization Unlock Rules | Not defined |

---

## 11. Explicitly Not Defined (P042)

- Avatar Sources
- Nickname Rules
- Profile Privacy
- Profile History
- Favorite Statistics
- Badges
- Titles
- Customization Unlock Rules

---

## 12. Open Questions

| ID | Question |
|----|----------|
| Q-P042-001 | **[CONFLICT]** Which document (P020, P042, or a merged successor) is authoritative for Player Profile? |
| Q-P042-002 | Is "Current Character" the same field as P020's "Selected Character"? |
| Q-P042-003 | Is "Experience" here the same value as P023 Progression XP? |
| Q-P042-004 | Is "Profile Background" the same element as P020's "Profile Banner"? |
| Q-P042-005 | Should Display Name be editable given P020's cosmetic-only customization rule? |
| Q-P042-006 | Do Losses / First Place Finishes (P020) also apply to this profile structure? |
| Q-P042-007 | Win Rate calculation formula? |
| Q-P042-008 | Avatar Sources, Nickname Rules, Profile Privacy, Profile History, Favorite Statistics, Badges, Titles, Customization Unlock Rules — future or never? |

---

## 13. Acceptance Criteria

P042 v1.0 (as a documented brief) is satisfied when all of the following are true:

1. Every player owns one Player Profile storing persistent, backend-synchronized account information.
2. Profile structure: Player ID, Display Name, Avatar, Current Character, Current Rank, Player Level, Experience, Country (future), Online Status, Account Creation Date.
3. Customization: Display Name, Avatar, Profile Frame, Profile Background, Favorite Character; Future Customizations future.
4. Statistics: Total Races, Wins, Win Rate, Current Rank, Highest Rank, Achievements; Future Statistics future.
5. Social Information: Friends Count, Clan Information, Online Status; Recent Activity future.
6. Actions: View Own Profile, View Other Player Profiles, Edit Profile, Change Avatar, Change Display Name.
7. Rules: backend-synchronized; only editable fields modifiable; cross-device consistency.
8. Avatar Sources, Nickname Rules, Profile Privacy, Profile History, Favorite Statistics, Badges, Titles, and Customization Unlock Rules are not invented.
9. **Conflict with P020 is recorded and escalated, not silently resolved** (§0).
10. Document version is **P042 v1.0**.

---

## 14. Document Queue

| Order | Document ID | Title | Status |
|-------|-------------|-------|--------|
| 1–41 | P001–P041 | (prior specs) | Approved as previously recorded |
| 42 | P042 | Player Profile System Specification | **v1.0 Approved-per-brief — [CONFLICT] with P020, unresolved** |
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

## 15. Change Log

| Version | Date | Summary | Author |
|---------|------|---------|--------|
| **1.0** | 2026-07-31 | Initial Player Profile System Specification (P042); conflict with P020 identified and escalated | Documentation Engineer (from brief) |

---

*End of document.*
