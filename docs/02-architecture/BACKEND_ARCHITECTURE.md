# Backend Architecture Specification

| Field | Value |
|-------|--------|
| Document ID | P039 |
| Title | Backend Architecture Specification |
| Version | **1.0** |
| Status | Approved (Backend architecture scope only) |
| Project | Project GulfRun |
| Location rationale | Backend architecture is an **engineering** concern → lives under `docs/` per [DOCUMENTATION_STRUCTURE.md](../00-governance/DOCUMENTATION_STRUCTURE.md) §2, not `Design/GDD/`. Numbered **P039** for continuity with the ongoing specification brief sequence. |
| Authority | Official source of truth for the **backend responsibilities list**, **architecture design principles**, **client vs. backend responsibility split**, **data synchronization rules**, and **security principles** stated herein |
| Relates to (engineering) | [MULTIPLAYER_ARCHITECTURE.md](MULTIPLAYER_ARCHITECTURE.md), [TECHNICAL_STACK.md](TECHNICAL_STACK.md), [SCALABILITY_PLAN.md](SCALABILITY_PLAN.md), [SECURITY_STRATEGY.md](../05-security/SECURITY_STRATEGY.md), [ANTI_CHEAT.md](../05-security/ANTI_CHEAT.md), [DATABASE_ARCHITECTURE.md](DATABASE_ARCHITECTURE.md) (P040), [AUTHENTICATION_SYSTEM.md](AUTHENTICATION_SYSTEM.md) (P041) |
| Relates to (gameplay systems named as backend responsibilities) | [P012](../../Design/GDD/P012-ECONOMY-SYSTEM-v1.0.md) Economy/Currencies, [P014](../../Design/GDD/P014-FRIENDS-SYSTEM-v1.0.md) Friends, [P015](../../Design/GDD/P015-CLAN-SYSTEM-v1.0.md) Clans, [P017](../../Design/GDD/P017-MATCHMAKING-SYSTEM-v1.0.md) Matchmaking, [P019](../../Design/GDD/P019-LEADERBOARD-SYSTEM-v1.0.md) Leaderboards, [P020](../../Design/GDD/P020-PLAYER-PROFILE-SYSTEM-v1.0.md) Player Profiles, [P021](../../Design/GDD/P021-INVENTORY-SYSTEM-v1.0.md) Inventory, [P025](../../Design/GDD/P025-RANK-SYSTEM-v1.0.md) Rank, [P026](../../Design/GDD/P026-DAILY-CHALLENGES-SYSTEM-v1.0.md)/[P027](../../Design/GDD/P027-WEEKLY-CHALLENGES-SYSTEM-v1.0.md) Challenges, [P028](../../Design/GDD/P028-ACHIEVEMENT-SYSTEM-v1.0.md) Achievements, [P029](../../Design/GDD/P029-BATTLE-PASS-SYSTEM-v1.0.md) Battle Pass, [P031](../../Design/GDD/P031-LIVE-EVENTS-SYSTEM-v1.0.md) Live Events, [P032](../../Design/GDD/P032-NOTIFICATION-SYSTEM-v1.0.md) Notifications, [P033](../../Design/GDD/P033-INBOX-MAIL-SYSTEM-v1.0.md) Inbox |
| Last updated | 2026-07-31 |

**Rules:** Documentation only. No implementation. Do not invent gameplay. Do not add implementation details beyond this brief. Missing detail → **TODO**.

**Next milestone:** Sprint 1 — do not start until explicitly instructed.

---

## 1. Purpose

Define the high-level Backend Architecture for Project GulfRun: the backend's role as single source of truth, the full list of backend responsibilities, server-authoritative design principles, the client/backend responsibility split, data synchronization rules, and security principles — without specifying cloud provider, database type, programming language, hosting region, microservices topology, caching, message queue, monitoring, or disaster recovery choices.

---

## 2. System Overview

| Field | Value |
|-------|--------|
| Backend | Project GulfRun **uses an online backend** |
| Authority | The backend is **the single source of truth for all persistent player data** |
| Trust model | **Clients must never be trusted for authoritative gameplay decisions** |

### Alignment with existing engineering docs

- This document is the **requirements-level specification** (backend responsibilities + principles, as briefed). [TECHNICAL_STACK.md](TECHNICAL_STACK.md), [MULTIPLAYER_ARCHITECTURE.md](MULTIPLAYER_ARCHITECTURE.md), and [SCALABILITY_PLAN.md](SCALABILITY_PLAN.md) contain **recommendations** (pending ADR) for how those requirements are implemented — no conflict; this document does not choose implementation.
- The server-authoritative principle stated here matches the authority model already recorded in [MULTIPLAYER_ARCHITECTURE.md](MULTIPLAYER_ARCHITECTURE.md) §2.

---

## 3. Backend Responsibilities

The backend is responsible for:

| Responsibility | Status | Related GDD spec |
|-----------------|--------|-------------------|
| **Authentication** | Defined | Detail SoT: [AUTHENTICATION_SYSTEM.md](AUTHENTICATION_SYSTEM.md) (P041) |
| **Player Profiles** | Defined | [P020](../../Design/GDD/P020-PLAYER-PROFILE-SYSTEM-v1.0.md) |
| **Cloud Save** | Defined | — |
| **Inventory** | Defined | [P021](../../Design/GDD/P021-INVENTORY-SYSTEM-v1.0.md) |
| **Currencies** | Defined | [P012](../../Design/GDD/P012-ECONOMY-SYSTEM-v1.0.md) |
| **Matchmaking** | Defined | [P017](../../Design/GDD/P017-MATCHMAKING-SYSTEM-v1.0.md) |
| **Leaderboards** | Defined | [P019](../../Design/GDD/P019-LEADERBOARD-SYSTEM-v1.0.md) |
| **Friends** | Defined | [P014](../../Design/GDD/P014-FRIENDS-SYSTEM-v1.0.md) |
| **Clans** | Defined | [P015](../../Design/GDD/P015-CLAN-SYSTEM-v1.0.md) |
| **Rank System** | Defined | [P025](../../Design/GDD/P025-RANK-SYSTEM-v1.0.md) |
| **Battle Pass** | Defined | [P029](../../Design/GDD/P029-BATTLE-PASS-SYSTEM-v1.0.md) |
| **Challenges** | Defined | [P026](../../Design/GDD/P026-DAILY-CHALLENGES-SYSTEM-v1.0.md) / [P027](../../Design/GDD/P027-WEEKLY-CHALLENGES-SYSTEM-v1.0.md) |
| **Achievements** | Defined | [P028](../../Design/GDD/P028-ACHIEVEMENT-SYSTEM-v1.0.md) |
| **Live Events** | Defined | [P031](../../Design/GDD/P031-LIVE-EVENTS-SYSTEM-v1.0.md) |
| **Inbox** | Defined | [P033](../../Design/GDD/P033-INBOX-MAIL-SYSTEM-v1.0.md) |
| **Notifications** | Defined | [P032](../../Design/GDD/P032-NOTIFICATION-SYSTEM-v1.0.md) |
| **Analytics** | Defined | Detail SoT: [ANALYTICS_SYSTEM.md](ANALYTICS_SYSTEM.md) (P044) |
| **Future Systems** | Future | — |

This list confirms **which systems the backend is responsible for**; it does not redefine or alter the gameplay rules of the linked GDD specs.

### TODO — Responsibilities (not provided)

- [ ] Cloud Save scope/format (separate from Inventory/Profile persistence?)
- [ ] Analytics event scope / ownership boundary with product analytics stack ([TECHNICAL_STACK.md](TECHNICAL_STACK.md) §6)

---

## 4. Architecture Principles

The backend architecture must be:

| Principle | Status |
|-----------|--------|
| **Server Authoritative** | Defined |
| **Scalable** | Defined |
| **Modular** | Defined |
| **Secure** | Defined |
| **Fault Tolerant** | Defined |
| **Cloud Hosted** | Defined |
| **Cross Platform** | Defined |

### TODO — Architecture Principles (not provided)

- [ ] Concrete scalability targets (see [SCALABILITY_PLAN.md](SCALABILITY_PLAN.md) for engineering planning envelopes — not part of this brief)
- [ ] Fault tolerance targets (uptime SLO, failover behavior)

---

## 5. Client vs. Backend Responsibilities

### 5.1 Client Responsibilities

| Responsibility | Status |
|-----------------|--------|
| **Rendering** | Defined |
| **Input** | Defined |
| **Audio** | Defined |
| **Animations** | Defined |
| **Visual Effects** | Defined |
| **UI** | Defined |
| **Prediction where applicable** | Defined |

### 5.2 Backend Responsibilities

See §3 above — full backend responsibility list.

### 5.3 Split rule

| Rule | Statement |
|------|-----------|
| BE-CLI-001 | Clients handle presentation and input; the backend holds authoritative persistent data and gameplay-decision authority. |
| BE-CLI-002 | Client-side prediction is allowed **where applicable**; scope not enumerated here. |

### TODO — Client vs. Backend (not provided)

- [ ] Which specific gameplay actions use client prediction (race movement, item use, etc.)
- [ ] Reconciliation behavior when prediction diverges from server authority

---

## 6. Data Synchronization Rules

| Rule ID | Rule |
|---------|------|
| SYNC-001 | Player data is **synchronized between client and backend**. |
| SYNC-002 | Synchronization **must support reconnects**. |
| SYNC-003 | Conflict resolution **is handled by the backend**. |

### TODO — Synchronization (not provided)

- [ ] Reconnect grace window / timeout values
- [ ] Conflict resolution algorithm/strategy (last-write-wins, vector clock, etc.)
- [ ] Offline/airplane-mode behavior (see [MULTIPLAYER_ARCHITECTURE.md](MULTIPLAYER_ARCHITECTURE.md) §11 for existing engineering position — not part of this brief)

---

## 7. Security Principles

| Principle | Statement |
|-----------|-----------|
| SEC-001 | **Never trust client values.** |
| SEC-002 | **Validate every backend request.** |
| SEC-003 | **Protect all player progression.** |
| SEC-004 | **Prevent cheating whenever possible.** |

These principles align with the existing [SECURITY_STRATEGY.md](../05-security/SECURITY_STRATEGY.md) and [ANTI_CHEAT.md](../05-security/ANTI_CHEAT.md) engineering documents; this document does not add new security implementation detail beyond restating the brief's principles.

### TODO — Security (not provided)

- [ ] Specific validation rules per endpoint (deferred to engineering implementation, not this specification)

---

## 8. Dependencies

| Dependency | Note |
|------------|------|
| All GDD systems listed in §3 | Backend responsibility confirmed; gameplay rules remain owned by respective GDD spec |
| [MULTIPLAYER_ARCHITECTURE.md](MULTIPLAYER_ARCHITECTURE.md) | Server authority model, topology (engineering recommendation) |
| [TECHNICAL_STACK.md](TECHNICAL_STACK.md) | Stack choices deferred to ADR (Cloud Provider, DB Type, Language) |
| [SCALABILITY_PLAN.md](SCALABILITY_PLAN.md) | Scalability engineering planning |
| [SECURITY_STRATEGY.md](../05-security/SECURITY_STRATEGY.md) / [ANTI_CHEAT.md](../05-security/ANTI_CHEAT.md) | Security implementation detail |

---

## 9. Future Specifications

| Topic | Status |
|-------|--------|
| Future Systems (backend responsibility) | Future |
| Cloud Provider | Not defined (ADR — see [TECHNICAL_STACK.md](TECHNICAL_STACK.md) §11) |
| Database Type | Not defined (ADR) |
| Programming Language | Not defined (ADR) |
| Hosting Region | Not defined |
| Microservices | Not defined |
| Caching | Not defined |
| Message Queue | Not defined |
| Monitoring | Not defined |
| Disaster Recovery | Not defined |

---

## 10. Explicitly Not Defined (P039)

- Cloud Provider
- Database Type
- Programming Language
- Hosting Region
- Microservices
- Caching
- Message Queue
- Monitoring
- Disaster Recovery

---

## 11. Open Questions

| ID | Question |
|----|----------|
| Q-P039-001 | Cloud Save scope vs. Inventory/Profile persistence? |
| Q-P039-002 | Analytics event ownership boundary? |
| Q-P039-003 | Concrete scalability / fault-tolerance targets (SLOs)? |
| Q-P039-004 | Which gameplay actions use client-side prediction? |
| Q-P039-005 | Reconciliation behavior on prediction divergence? |
| Q-P039-006 | Reconnect grace window / timeout values? |
| Q-P039-007 | Conflict resolution algorithm/strategy? |
| Q-P039-008 | Cloud Provider, Database Type, Programming Language, Hosting Region, Microservices, Caching, Message Queue, Monitoring, Disaster Recovery — when will these be decided (ADR)? |

---

## 12. Acceptance Criteria

P039 v1.0 is satisfied when all of the following are true:

1. Online backend confirmed as single source of truth for persistent player data; clients never authoritative for gameplay decisions.
2. Backend responsibilities: Authentication, Player Profiles, Cloud Save, Inventory, Currencies, Matchmaking, Leaderboards, Friends, Clans, Rank System, Battle Pass, Challenges, Achievements, Live Events, Inbox, Notifications, Analytics; Future Systems future.
3. Architecture principles: Server Authoritative, Scalable, Modular, Secure, Fault Tolerant, Cloud Hosted, Cross Platform.
4. Client responsibilities: Rendering, Input, Audio, Animations, Visual Effects, UI, Prediction where applicable.
5. Synchronization rules: client/backend sync; reconnect support; backend-handled conflict resolution.
6. Security principles: never trust client values; validate every backend request; protect player progression; prevent cheating whenever possible.
7. Cloud Provider, Database Type, Programming Language, Hosting Region, Microservices, Caching, Message Queue, Monitoring, and Disaster Recovery are not invented.
8. No gameplay mechanics invented; all backend-responsibility systems reference their existing GDD spec rather than redefining rules.
9. Document version is **P039 v1.0**.

---

## 13. Document Queue (cross-reference to GDD specification sequence)

| Order | Document ID | Title | Status |
|-------|-------------|-------|--------|
| 1–38 | P001–P038 | (prior gameplay specs, `Design/GDD/`) | Approved as previously recorded |
| 39 | P039 | Backend Architecture Specification (`docs/02-architecture/`) | v1.0 Approved |
| 40 | P040 | Database Architecture Specification (`docs/02-architecture/`) | v1.0 Approved — [P040](DATABASE_ARCHITECTURE.md) |
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

## 14. Change Log

| Version | Date | Summary | Author |
|---------|------|---------|--------|
| **1.0** | 2026-07-31 | Initial Backend Architecture Specification | Documentation Engineer (from brief) |

---

*End of document.*
