# Database Architecture Specification

| Field | Value |
|-------|--------|
| Document ID | P040 |
| Title | Database Architecture Specification |
| Version | **1.0** |
| Status | Approved (Database architecture scope only) |
| Project | Project GulfRun |
| Location rationale | Database architecture is an **engineering** concern → lives under `docs/` per [DOCUMENTATION_STRUCTURE.md](../00-governance/DOCUMENTATION_STRUCTURE.md) §2, not `Design/GDD/`. Numbered **P040** for continuity with the ongoing specification brief sequence (see [P039](BACKEND_ARCHITECTURE.md)). |
| Authority | Official source of truth for the **data categories list**, **database design principles**, **data ownership rules**, **synchronization statement**, **security statement**, and **backup statement** stated herein |
| Relates to (engineering) | [BACKEND_ARCHITECTURE.md](BACKEND_ARCHITECTURE.md) (P039), [TECHNICAL_STACK.md](TECHNICAL_STACK.md) §4, [SCALABILITY_PLAN.md](SCALABILITY_PLAN.md), [SECURITY_STRATEGY.md](../05-security/SECURITY_STRATEGY.md), [AUTHENTICATION_SYSTEM.md](AUTHENTICATION_SYSTEM.md) (P041) |
| Relates to (gameplay systems named as data categories) | [P020](../../Design/GDD/P020-PLAYER-PROFILE-SYSTEM-v1.0.md) Player Profiles, [P021](../../Design/GDD/P021-INVENTORY-SYSTEM-v1.0.md) Inventory, [P012](../../Design/GDD/P012-ECONOMY-SYSTEM-v1.0.md) Currencies, [P005](../../Design/GDD/P005-CHARACTER-SYSTEM-v1.0.md) Character Unlocks, [P022](../../Design/GDD/P022-COSMETICS-SYSTEM-v1.0.md) Cosmetics, [P023](../../Design/GDD/P023-PLAYER-PROGRESSION-SYSTEM-v1.0.md) Progression, [P025](../../Design/GDD/P025-RANK-SYSTEM-v1.0.md) Rank Data, [P029](../../Design/GDD/P029-BATTLE-PASS-SYSTEM-v1.0.md) Battle Pass, [P026](../../Design/GDD/P026-DAILY-CHALLENGES-SYSTEM-v1.0.md)/[P027](../../Design/GDD/P027-WEEKLY-CHALLENGES-SYSTEM-v1.0.md) Challenges, [P028](../../Design/GDD/P028-ACHIEVEMENT-SYSTEM-v1.0.md) Achievements, [P014](../../Design/GDD/P014-FRIENDS-SYSTEM-v1.0.md) Friends, [P015](../../Design/GDD/P015-CLAN-SYSTEM-v1.0.md) Clans, [P033](../../Design/GDD/P033-INBOX-MAIL-SYSTEM-v1.0.md) Mail, [P032](../../Design/GDD/P032-NOTIFICATION-SYSTEM-v1.0.md) Notifications |
| Last updated | 2026-07-31 |

**Rules:** Documentation only. No implementation. Do not invent gameplay. Do not add implementation details beyond this brief. Missing detail → **TODO**.

**Next milestone:** Sprint 1 — do not start until explicitly instructed.

---

## 1. Purpose

Define the high-level Database Architecture for Project GulfRun: centralized persistent storage accessed only by the backend, the full list of data categories, database design principles, data ownership rules (per-player vs. shared), synchronization and security statements, and the backup requirement — without specifying database engine, sharding, replication, backup schedule, retention policy, encryption method, indexes, or migration strategy.

---

## 2. System Overview

| Field | Value |
|-------|--------|
| Storage | Project GulfRun stores **persistent player data in a centralized database** |
| Access control | **The backend is responsible for all database access** |
| Client access | **Clients never communicate directly with the database** |

### Alignment with existing engineering docs

- This document is the **requirements-level specification** (data categories + principles, as briefed). [TECHNICAL_STACK.md](TECHNICAL_STACK.md) §4 already recommends PostgreSQL (system of record), Redis (ephemeral), object storage, and an analytics warehouse **pending ADR** — no conflict; this document does not choose an engine.
- The no-direct-client-access rule matches [BACKEND_ARCHITECTURE.md](BACKEND_ARCHITECTURE.md) (P039) §2 trust model: the backend is the sole authority between clients and persistent data.

---

## 3. Data Categories

| Category | Status | Related GDD spec |
|----------|--------|-------------------|
| **Player Accounts** | Defined | Detail SoT: [AUTHENTICATION_SYSTEM.md](AUTHENTICATION_SYSTEM.md) (P041) |
| **Player Profiles** | Defined | [P020](../../Design/GDD/P020-PLAYER-PROFILE-SYSTEM-v1.0.md) |
| **Inventory** | Defined | [P021](../../Design/GDD/P021-INVENTORY-SYSTEM-v1.0.md) |
| **Currencies** | Defined | [P012](../../Design/GDD/P012-ECONOMY-SYSTEM-v1.0.md) |
| **Character Unlocks** | Defined | [P005](../../Design/GDD/P005-CHARACTER-SYSTEM-v1.0.md) |
| **Cosmetics** | Defined | [P022](../../Design/GDD/P022-COSMETICS-SYSTEM-v1.0.md) |
| **Progression** | Defined | [P023](../../Design/GDD/P023-PLAYER-PROGRESSION-SYSTEM-v1.0.md) |
| **Rank Data** | Defined | [P025](../../Design/GDD/P025-RANK-SYSTEM-v1.0.md) |
| **Battle Pass** | Defined | [P029](../../Design/GDD/P029-BATTLE-PASS-SYSTEM-v1.0.md) |
| **Challenges** | Defined | [P026](../../Design/GDD/P026-DAILY-CHALLENGES-SYSTEM-v1.0.md) / [P027](../../Design/GDD/P027-WEEKLY-CHALLENGES-SYSTEM-v1.0.md) |
| **Achievements** | Defined | [P028](../../Design/GDD/P028-ACHIEVEMENT-SYSTEM-v1.0.md) |
| **Friends** | Defined | [P014](../../Design/GDD/P014-FRIENDS-SYSTEM-v1.0.md) |
| **Clans** | Defined | [P015](../../Design/GDD/P015-CLAN-SYSTEM-v1.0.md) |
| **Mail** | Defined | [P033](../../Design/GDD/P033-INBOX-MAIL-SYSTEM-v1.0.md) |
| **Notifications** | Defined | [P032](../../Design/GDD/P032-NOTIFICATION-SYSTEM-v1.0.md) |
| **Analytics** | Defined | Detail SoT: [ANALYTICS_SYSTEM.md](ANALYTICS_SYSTEM.md) (P044) |
| **Future Data** | Future | — |

This list confirms **which data categories the database stores**; it does not redefine the gameplay rules of the linked GDD specs.

### TODO — Data Categories (not provided)

- [ ] Schema-level fields per category (deferred to implementation)
- [ ] Whether Season data ([P030](../../Design/GDD/P030-SEASON-SYSTEM-v1.0.md)) / Live Events data ([P031](../../Design/GDD/P031-LIVE-EVENTS-SYSTEM-v1.0.md)) are distinct categories or folded into listed ones

---

## 4. Database Design Principles

The database architecture must be:

| Principle | Status |
|-----------|--------|
| **Scalable** | Defined |
| **Reliable** | Defined |
| **Consistent** | Defined |
| **Secure** | Defined |
| **Versioned** | Defined |
| **Cloud Hosted** | Defined |
| **High Availability** | Defined |

### TODO — Design Principles (not provided)

- [ ] Concrete consistency model (strong vs. eventual, per data category)
- [ ] Versioning strategy meaning (schema versioning vs. record versioning)
- [ ] High availability targets (uptime SLO)

---

## 5. Data Ownership Rules

| Field | Value |
|-------|-------|
| Per-player data | **Every player owns their own account data** |

Shared systems manage:

| Shared System | Status |
|-----------------|--------|
| **Leaderboards** | Defined |
| **Clans** | Defined |
| **Friends** | Defined |
| **Events** | Defined |
| **Global Statistics** | Defined |

### TODO — Data Ownership (not provided)

- [ ] Ownership boundary details for shared systems (e.g., who owns a Clan's data — the clan entity vs. individual members)
- [ ] Data deletion/portability rules tied to account ownership (see [P034](../../Design/GDD/P034-SETTINGS-SYSTEM-v1.0.md) Delete Account — future)

---

## 6. Synchronization

| Rule ID | Rule |
|---------|------|
| DB-SYNC-001 | Database synchronization **is controlled by the backend**. |

| Field | Value |
|-------|-------|
| Offline synchronization behavior | **Not defined** |

### Alignment

Relates to [BACKEND_ARCHITECTURE.md](BACKEND_ARCHITECTURE.md) (P039) §6 Data Synchronization Rules (client/backend sync, reconnect support, backend-handled conflict resolution) — this document confirms the database layer as the backend-controlled endpoint of that synchronization.

### TODO — Synchronization (not provided)

- [ ] Offline synchronization behavior (explicitly not defined in this brief)

---

## 7. Security

| Rule ID | Rule |
|---------|------|
| DB-SEC-001 | **No direct database access from clients.** |
| DB-SEC-002 | **All writes require backend validation.** |
| DB-SEC-003 | **Sensitive information must be protected.** |

These align with [SECURITY_STRATEGY.md](../05-security/SECURITY_STRATEGY.md) (encryption at rest, backups encrypted) and [BACKEND_ARCHITECTURE.md](BACKEND_ARCHITECTURE.md) (P039) §7 Security Principles; this document does not add new security implementation detail beyond restating the brief.

### TODO — Security (not provided)

- [ ] Definition of "sensitive information" scope (PII, payment data, credentials)
- [ ] Encryption method (deferred — see §10 Not Defined)

---

## 8. Backup

| Field | Value |
|-------|-------|
| Requirement | **Regular backups are required** |
| Restore procedures | **Not defined** |

### TODO — Backup (not provided)

- [ ] Backup schedule (explicitly not defined in this brief)
- [ ] Restore procedures (explicitly not defined in this brief)

---

## 9. Dependencies

| Dependency | Note |
|------------|------|
| All GDD systems listed in §3 | Data category confirmed; gameplay rules remain owned by respective GDD spec |
| [BACKEND_ARCHITECTURE.md](BACKEND_ARCHITECTURE.md) (P039) | Backend as sole database access point; sync/security principle parent |
| [TECHNICAL_STACK.md](TECHNICAL_STACK.md) §4 | Data store recommendations pending ADR (Database Engine) |
| [SCALABILITY_PLAN.md](SCALABILITY_PLAN.md) | Sharding/replication engineering planning |
| [SECURITY_STRATEGY.md](../05-security/SECURITY_STRATEGY.md) | Encryption, backup security implementation |

---

## 10. Future Specifications

| Topic | Status |
|-------|--------|
| Future Data | Future |
| Database Engine | Not defined (ADR — see [TECHNICAL_STACK.md](TECHNICAL_STACK.md) §11) |
| Sharding | Not defined |
| Replication | Not defined |
| Backup Schedule | Not defined |
| Retention Policy | Not defined |
| Encryption | Not defined |
| Indexes | Not defined |
| Migration Strategy | Not defined |

---

## 11. Explicitly Not Defined (P040)

- Database Engine
- Sharding
- Replication
- Backup Schedule
- Retention Policy
- Encryption
- Indexes
- Migration Strategy

---

## 12. Open Questions

| ID | Question |
|----|----------|
| Q-P040-001 | Schema-level fields per data category? |
| Q-P040-002 | Are Season / Live Events data distinct categories or folded into listed ones? |
| Q-P040-003 | Concrete consistency model per data category? |
| Q-P040-004 | Versioning strategy — schema vs. record? |
| Q-P040-005 | High availability uptime target? |
| Q-P040-006 | Ownership boundary for shared systems (e.g., Clan data)? |
| Q-P040-007 | Data deletion/portability rules tied to account ownership? |
| Q-P040-008 | Offline synchronization behavior? |
| Q-P040-009 | "Sensitive information" scope definition? |
| Q-P040-010 | Database Engine, Sharding, Replication, Backup Schedule, Retention Policy, Encryption, Indexes, Migration Strategy — ADR timeline? |

---

## 13. Acceptance Criteria

P040 v1.0 is satisfied when all of the following are true:

1. Centralized database confirmed; backend sole access point; clients never access database directly.
2. Data categories: Player Accounts, Player Profiles, Inventory, Currencies, Character Unlocks, Cosmetics, Progression, Rank Data, Battle Pass, Challenges, Achievements, Friends, Clans, Mail, Notifications, Analytics; Future Data future.
3. Design principles: Scalable, Reliable, Consistent, Secure, Versioned, Cloud Hosted, High Availability.
4. Ownership: every player owns their own account data; Leaderboards, Clans, Friends, Events, Global Statistics managed as shared systems.
5. Synchronization controlled by the backend; offline synchronization behavior not defined.
6. Security: no direct client database access; all writes backend-validated; sensitive information protected.
7. Backup: regular backups required; restore procedures not defined.
8. Database Engine, Sharding, Replication, Backup Schedule, Retention Policy, Encryption, Indexes, and Migration Strategy are not invented.
9. No gameplay mechanics invented; all data-category systems reference their existing GDD spec rather than redefining rules.
10. Document version is **P040 v1.0**.

---

## 14. Document Queue (cross-reference to GDD specification sequence)

| Order | Document ID | Title | Status |
|-------|-------------|-------|--------|
| 1–38 | P001–P038 | (prior gameplay specs, `Design/GDD/`) | Approved as previously recorded |
| 39 | P039 | Backend Architecture Specification (`docs/02-architecture/`) | v1.0 Approved |
| 40 | P040 | Database Architecture Specification (`docs/02-architecture/`) | v1.0 Approved |
| 41 | P041 | Authentication System Specification (`docs/02-architecture/`) | v1.0 Approved — [P041](AUTHENTICATION_SYSTEM.md) |
| 42 | P042 | Player Profile System Specification (`Design/GDD/`) | v1.0 Approved-per-brief — [CONFLICT] with P020, unresolved |
| 43 | P043 | Anti-Cheat System Specification (`docs/05-security/`) | v1.0 Approved |
| 44 | P044 | Analytics System Specification (`docs/02-architecture/`) | v1.0 Approved |
| 45 | P045 | Monetization System Specification (`Design/GDD/`) | v1.0 Approved |
| 46 | P046 | Performance Optimization Specification (`docs/04-engineering/`) | v1.0 Approved |
| 47 | P047 | UI / UX Design System Specification (`Design/GDD/`) | v1.0 Approved |
| 48 | P048 | Art Direction & Visual Style Specification (`Design/GDD/`) | v1.0 Approved |
| 49 | P049 | Technical Architecture Specification (`docs/02-architecture/`) | v1.0 Approved |
| 50 | P050 | Master Design Bible Specification (`Design/GDD/`) | v1.0 Approved |
| — | Sprint 1 | _(await instructions)_ | Not started |

---

## 15. Change Log

| Version | Date | Summary | Author |
|---------|------|---------|--------|
| **1.0** | 2026-07-31 | Initial Database Architecture Specification | Documentation Engineer (from brief) |

---

*End of document.*
