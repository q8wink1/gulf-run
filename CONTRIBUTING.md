# Contributing to GulfRun

This repository currently contains **foundation documentation, an empty GDD template, and scaffolding only**. Gameplay code is not accepted until Phase gates are met **and** the relevant [`Design/GDD/`](Design/GDD/README.md) chapters are **Approved**.

**Never invent gameplay** in PRs, ADRs, or engineering docs. If design is missing, open a `[QUESTION]` for the Design Owner.

## Prerequisites

- Read [docs/README.md](docs/README.md)
- Follow [Git Branching Strategy](docs/03-standards/GIT_BRANCHING_STRATEGY.md)
- Follow [Coding Standards](docs/03-standards/CODING_STANDARDS.md) when code lands
- Follow [Naming Conventions](docs/03-standards/NAMING_CONVENTIONS.md)

## Contribution types (Phase 0)

| Allowed | Not allowed |
|---------|-------------|
| Documentation | Gameplay systems / modes |
| Architecture ADRs | Client hacks that bypass server authority |
| Folder scaffolding / README markers | Secrets, API keys, certs |
| CI skeleton (no shipping builds yet) | Unapproved third-party SDKs |
| Tooling specs | Large binary dumps outside asset policy |

## Pull request checklist

- [ ] Scope matches an open milestone or approved ADR
- [ ] Docs updated if behavior or policy changes
- [ ] No secrets or PII
- [ ] Naming conventions respected
- [ ] Reviewers: at least one peer; normative docs need Tech Director

## Commit messages

```
<type>(<scope>): <imperative summary>

Types: docs, chore, ci, feat, fix, refactor, test, security
Scopes: docs, client, server, shared, tools, infra, art, design, qa
```

Examples:

- `docs(roadmap): add Year-2 LiveOps content pillars`
- `chore(repo): scaffold Client/ and Server/ trees`

## Questions

Escalate architecture questions via ADR draft + Tech Director review. Do not invent parallel conventions in feature branches.
