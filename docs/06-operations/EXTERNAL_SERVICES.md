# External Services Integration Roadmap

**Last updated:** 2026-07-31  
**Owner:** Principal Architect + Producer  
**Audience:** Engineering, Legal, LiveOps

---

## 1. Policy

- All vendors sit behind **interfaces** in client/server.
- Legal/DPA review before production data flows.
- Prefer one vendor per category at Soft Launch; expand later via ADR.
- Data export drill annually for lock-in risk (see Risk O-03).

## 2. Integration waves

### Wave A — Bootstrap (M1–M2)

| Service | Purpose | Notes |
|---------|---------|-------|
| Cloud provider | Compute, DB, storage, secrets | ADR picks AWS/GCP |
| CDN | Addressables / static | Required for content |
| Crash reporting | Crashlytics / Sentry | Client stability |
| Log/metrics backend | Cloud or Grafana stack | Ops |
| Identity providers (platform) | Apple/Google/Play Games sign-in | Account linking |

### Wave B — Economy & growth (M3–M5)

| Service | Purpose | Notes |
|---------|---------|-------|
| App stores IAP | Apple / Google billing | Server validate |
| Attribution | Adjust / AppsFlyer / Singular | Privacy manifests |
| Push | FCM + APNs | Abstraction |
| Email / transactional | SendGrid / SES | Support & security mail |
| Feature flags | LaunchDarkly / Unleash / custom | May be first-party |
| Remote config | Custom LiveOps preferred long-term | Vendor OK early |

### Wave C — Soft Launch / Global (M5–M7)

| Service | Purpose | Notes |
|---------|---------|-------|
| Customer support CRM | Zendesk / Salesforce / Intercom | Tooling integration |
| Anti-abuse / bot | Edge bot mgmt (Cloudflare etc.) | With WAF |
| Marketing automation | CRM segments | Careful with privacy |
| Store review tooling | Fastlane / supply chain | CI |
| Status page | Statuspage / custom | Incidents |

### Wave D — Mature LiveOps (M8+)

| Service | Purpose | Notes |
|---------|---------|-------|
| Data warehouse + BI | BigQuery/Snowflake + Looker/etc. | Product analytics |
| Experimentation platform | In-house or vendor | Stats rigor |
| Voice/text moderation | If UGC/social | Safety |
| Customer messaging | In-app + push orchestration | |
| Account defense | Device fingerprint vendors | Legal review |

## 3. Category decisions matrix

| Category | Build | Buy | Default lean |
|----------|-------|-----|--------------|
| LiveOps config | ✓ capable | ✓ early | Build core schemas; buy flag UI if needed |
| Analytics ingest | thin client | warehouse buy | Hybrid |
| Crash | — | ✓ | Buy |
| Matchmaking | ✓ | rare | Build |
| Session sim | ✓ | rare | Build |
| Auth | ✓ + platform | BaaS risky for economy | Build + platform IdPs |
| Chat | later | ✓ possible | ADR |

## 4. Interface requirements

Every integration MUST document:

1. Owner  
2. Data classification (PII? payment?)  
3. Failure mode (gameplay continues?)  
4. Kill switch  
5. Exit/export plan  

## 5. Regional considerations

- Gulf / MENA payment alternatives may require additional processors later (ADR).
- Data residency may force regional warehouses or restricted subprocessors.
- Localization vendors for AR/EN quality review.

## 6. Forbidden until ADR

- Shipping production traffic through personal freemium accounts
- Multiple overlapping attribution SDKs
- Client-side only “analytics” that include sensitive economy without server mirror
- Unvetted Chinese/obscure SDKs without security review (or any SDK without review)

## 7. Checklist before enabling a vendor in prod

- [ ] DPA signed  
- [ ] Privacy manifest / data safety updated  
- [ ] Interface wrapper merged  
- [ ] Staging validation  
- [ ] Kill switch tested  
- [ ] Cost alert configured  
