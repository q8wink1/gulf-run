# Git Branching Strategy

**Last updated:** 2026-07-31  
**Owner:** Engineering Manager + DevOps  
**Audience:** All engineers

---

## 1. Model

GulfRun uses **trunk-based development with short-lived branches** and **release trains**, suitable for mobile live-service with store certification constraints.

```
main              always releasable / production-aligned
  ↑
develop           integration (optional early; may collapse to main at maturity)
  ↑
feature/*  fix/*  short-lived
release/*         cut for store submission
hotfix/*          from main for production emergencies
```

**Phase 0–2:** `main` + `develop` allowed.  
**Post Soft Launch maturity goal:** prefer `main` + feature branches + `release/*` if `develop` adds lag.

## 2. Branch definitions

| Branch | Purpose | Protection |
|--------|---------|------------|
| `main` | Production-of-record source | Require reviews, CI green, no force-push |
| `develop` | Integration for unstable epic work | Require CI; squash/rebase policies |
| `feature/*` | Single feature/task | Delete after merge |
| `fix/*` | Non-urgent bugfix | Delete after merge |
| `release/x.y.z` | Stabilization & cert | Only fixes + version bumps |
| `hotfix/*` | Prod Sev-1/2 | Fast review; backport rules |

## 3. Naming

See [NAMING_CONVENTIONS.md](NAMING_CONVENTIONS.md). Always include ticket id when tracker exists.

## 4. Commit hygiene

- Conventional commits: `type(scope): summary`
- Atomic commits preferred; squash on merge OK if history stays readable
- No secrets; pre-commit secret scan in CI

## 5. Pull requests

- Mandatory review: 1 peer minimum; **2** for Server economy, Security, Infra prod
- CI must pass
- Docs updated when normative behavior changes
- Feature flags noted in PR description for LiveOps-impacting work

## 6. Merge strategy

| Path | Strategy |
|------|----------|
| feature → develop/main | Squash or rebase merge (team choice locked at M1) |
| release → main | Merge commit OK for traceability |
| hotfix → main | Merge; then immediately merge main → develop |

**Never** force-push to `main` or `release/*`.

## 7. Release train

1. Cut `release/x.y.z` from `main` (or `develop` if still used)
2. Only bugfixes cherry-picked / PRs into release
3. Tag `vX.Y.Z` on merge to `main`
4. Store builds produced only from tags or release branches
5. Hotfix bumps `x.y.(z+1)`

Versioning: **SemVer** for client (`1.4.2`) + separate **content catalog** version.

## 8. Mobile store specifics

- Binary versionCode / CFBundleVersion monotonic
- Mapping of git tag → store version kept in release notes doc
- Content-only LiveOps changes do **not** require branch cut if remote config/Addressables suffice

## 9. Hotfix protocol

1. Branch `hotfix/*` from `main`
2. Fix + test + expedited review (Security if relevant)
3. CI → tag → deploy/submit
4. Merge back to `main` and `develop`
5. Postmortem within 48h for Sev-1

## 10. Monorepo path filters

CI SHOULD use path filters:

- `Client/**` → Unity jobs
- `Server/**` → server tests
- `docs/**` → doc lint only
- `Infrastructure/**` → plan/apply (manual approve prod)

## 11. Large binaries

- Git LFS patterns in `.gitattributes`
- Art vault ADR may replace LFS for certain types
- Reject accidental `Library/` uploads via `.gitignore`

## 12. Access control

- Admin: Tech Director, DevOps Lead
- Write: engineers via PR
- Break-glass: audited emergency role

## 13. Phase 0 rule

Until M1, only documentation and scaffolding commits land on `main`. Gameplay code PRs are rejected.
