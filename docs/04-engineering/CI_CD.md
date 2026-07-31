# CI/CD Recommendations

**Last updated:** 2026-07-31  
**Owner:** DevOps Lead  
**Audience:** Engineering, QA, LiveOps

---

## 1. Goals

- Every PR validated before merge.
- Reproducible mobile binaries from git tags.
- Safe promotion: `dev → staging → softlaunch/prod`.
- Content and config deployable independently from binary when possible.
- Fast feedback; expensive Unity jobs path-filtered.

## 2. Pipeline stages

```
PR
 ├─ docs lint / link check
 ├─ secret scan
 ├─ Shared codegen + contract tests
 ├─ Server unit/integration (path)
 └─ Client compile + EditMode tests (path)

merge → develop/main
 ├─ fuller test matrix
 ├─ Addressables content build (dev)
 └─ publish artifacts

release tag
 ├─ IL2CPP player builds (iOS/Android)
 ├─ signing
 ├─ symbol upload (crash)
 ├─ TestFlight / Play internal
 └─ smoke on device farm (subset)

prod promote
 ├─ manual approval
 ├─ store submission automation (where allowed)
 └─ LiveOps config remains separately gated
```

## 3. Environments

| Env | Binary source | Backend | Purpose |
|-----|---------------|---------|---------|
| `dev` | CI latest | ephemeral/shared dev | Engineers |
| `staging` | release candidates | prod-like | QA |
| `softlaunch` | tagged | prod-like capacity | Soft Launch markets |
| `prod` | tagged | multi-region path | Global |

Secrets per env in cloud secret manager; CI assumes roles via OIDC.

## 4. Unity build agents

- Dedicated runners with licensed Unity, iOS Mac fleet, Android Linux/Win as needed.
- Cache Library carefully; invalidate on package changes.
- Build logs retained; binary artifacts retention policy defined (cost).

## 5. Quality gates

| Gate | PR | Release |
|------|----|---------|
| Compile | Yes | Yes |
| Unit/EditMode | Yes (affected) | Full |
| PlayMode smoke | Nightly / release | Yes |
| Server tests | Yes (affected) | Full |
| App size budget | Warn PR / fail release | Fail if over |
| Static analysis | Server yes | Yes |
| Penetration | — | Pre Soft Launch |

## 6. Mobile signing & distribution

- iOS: certs/profiles in secure storage; match App Store Connect API keys rotation.
- Android: upload key in vault; Play App Signing.
- Internal distribution: TestFlight + Play internal/closed tracks before production track.

## 7. Infrastructure CD

- Terraform (or chosen IaC): `plan` on PR, `apply` on merge to infra main with **manual approve** for prod.
- DB migrations: expand/contract jobs separate from app deploy; never auto-destructive.

## 8. Content / LiveOps CD

- Config schemas validated in CI against `Shared/json-schemas`.
- Staged rollout percentages; automatic rollback on KPI/error triggers where instrumented.
- Addressables catalog publish is a first-class pipeline with version pinning.

## 9. Observability of pipelines

- Track: mean PR duration, flake rate, build failure taxonomy.
- Flaky tests quarantined with owner expiry — not silently skipped forever.

## 10. Security in CI

- Secret scanning, dependency scanning, SBOMs for server images.
- No privileged secrets on fork PRs.
- Signing jobs isolated.

## 11. Branch → deploy mapping

| Branch/tag | Auto-deploy |
|------------|-------------|
| `feature/*` | No |
| `develop` | dev |
| `release/*` | staging |
| `v*` tags | softlaunch/prod via approval |

## 12. Definition of done for CI at M1

- [ ] PR checks for docs + secret scan live
- [ ] Client compile job exists (even if empty scenes)
- [ ] Server skeleton test job exists
- [ ] OIDC to cloud secrets working in `dev`
- [ ] Artifact storage bucket created
