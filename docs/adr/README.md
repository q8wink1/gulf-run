# Architecture Decision Records

ADRs capture **normative** decisions that affect architecture, security, economy authority, or vendor lock-in.

## Rules

1. One ADR per decision; sequential IDs: `0001`, `0002`, …
2. Filename: `NNNN-short-kebab-title.md`
3. Status: `Proposed` → `Accepted` → `Superseded` / `Deprecated`
4. Accepted ADRs override conflicting older prose in foundation docs when explicitly stated
5. Soft Launch+ economy/security ADRs require Security Lead acknowledgment

## Index

| ID | Title | Status |
|----|-------|--------|
| [0001](0001-multiplayer-transport-abstraction.md) | Multiplayer Transport Abstraction for the Sprint 4 Foundation | Proposed |

_(Unity version / server language ADR for M1 is still outstanding.)_

## Creating an ADR

Copy [template.md](template.md) to `NNNN-title.md`, fill sections, open PR, seek Architect + relevant owners approval.
