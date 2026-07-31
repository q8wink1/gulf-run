# Security Strategy

**Last updated:** 2026-07-31  
**Owner:** Security Lead  
**Audience:** All engineers, DevOps, Legal (privacy)

---

## 1. Objectives

Protect players, revenue, studio reputation, and competitive integrity at million-player scale. Security is designed in from Phase 1 — not bolted on at Soft Launch.

## 2. Threat themes

| Theme | Examples |
|-------|----------|
| Account abuse | Credential stuffing, SIM/social engineering |
| Economic fraud | IAP replay, refund abuse, dupes |
| Cheating | Speed hacks, memory edits, botting |
| Service abuse | DDoS, credential stuffing APIs, scraping |
| Data breach | DB dump, misconfigured buckets, insider |
| Supply chain | Malicious packages, compromised CI |

## 3. Client security

- IL2CPP for release; obfuscation policy via ADR (not a substitute for server authority).
- No embedded service account keys; use player-scoped tokens only.
- Certificate pinning: evaluate carefully (ops risk) — ADR.
- Jailbreak/root signals as **signals**, not sole trust.
- Debug menus stripped from production.
- Secure storage for refresh tokens (Keychain/Keystore).

## 4. Transport & API

- TLS everywhere; HSTS at edge.
- Short-lived access tokens; rotating refresh.
- mTLS or signed service tokens between internal services.
- Strict rate limits + bot management at edge.
- Input validation / schema validation on all writes.
- Idempotent economy APIs.

## 5. Data security

- Encrypt at rest (cloud defaults + KMS CMKs for sensitive).
- Principle of least privilege IAM.
- PII minimization; retention schedules.
- Separate prod data access — no engineer laptops with prod DB by default; break-glass audited.
- Backups encrypted; restore tested.

## 6. Privacy & compliance

- Privacy by design checklist on features handling PII.
- Data Safety / Privacy Manifest maintained.
- Player deletion/export request workflow before Soft Launch.
- Regional residency requirements tracked with Legal (Gulf + global markets).
- Age gates / parental requirements per store policy.

## 7. IAP & payments

- Receipt validation **only** on server.
- Replay caches; account-store binding checks.
- Refund / chargeback reconciliation jobs.
- Grant ledger immutable; compensation via controlled tools.

## 8. Secrets & supply chain

- Secret manager; CI OIDC; rotation runbooks.
- Pre-receive/CI secret scanning.
- Dependency pinning + vulnerability scanning.
- Review new Unity/server packages (see Unity Packages allowlist).

## 9. Secure SDLC

| Phase | Control |
|-------|---------|
| Design | Threat model for economy/auth/social |
| Implement | Coding standards; authz reviews |
| Review | Mandatory reviewers on sensitive paths |
| Test | SAST, DAST sampling, abuse cases in QA |
| Release | Security sign-off Soft Launch / Global |
| Operate | Pentest cadence; vuln SLAs |

## 10. Incident response (security)

- Sev definitions shared with LiveOps; security Sev-1 = active breach/exploit in wild.
- Runbooks in `docs/runbooks/` (future): token revoke, forced patch, economy freeze.
- Comms templates with Legal/PR.

## 11. Soft Launch security gate (M5)

- [ ] Pentest report accepted
- [ ] IAP fraud path tested
- [ ] Secret scan clean history policy
- [ ] Privacy review signed
- [ ] WAF/DDoS baseline enabled
- [ ] Admin tools behind SSO + MFA + audit log

## 12. Non-goals

- Magical client-only security
- Storing PAN/card data (use platform stores)
- Building custom cryptography without review
