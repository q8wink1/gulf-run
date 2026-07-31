# Technical Stack Recommendations

**Last updated:** 2026-07-31  
**Owner:** Principal Architect  
**Status:** Recommended baseline — pin versions via ADR at M1

---

## 1. Guiding principles

1. Prefer **LTS / boring technology** for systems that hold money and trust.
2. Optimize for **mobile constraints** and **operability**, not desktop convenience.
3. Keep **vendor-replaceable** edges (analytics, crash, push) behind interfaces.
4. Generate **shared contracts** once; never duplicate wire formats by hand.

## 2. Client

| Layer | Recommendation | Notes |
|-------|----------------|-------|
| Engine | **Unity 6 LTS** (or current Unity LTS at M1 pin) | ADR locks exact version |
| Language | **C#** (nullable enabled) | Uniform across client |
| UI | **UI Toolkit** for meta; UGUI only if justified by ADR | Consistency over mix-and-match |
| Assets | **Addressables** + CDN | No giant `Resources` |
| Netcode | Thin client + **server-authoritative** sessions | See Multiplayer doc |
| DI / composition | Explicit composition root; optional VContainer/Zenject via ADR | Avoid service-locator sprawl |
| Local DB | Encrypted preferences + optional SQLite for cache only | Never source of truth for economy |
| Push | Platform native via abstraction | FCM / APNs |
| Ads (if any) | Mediation behind interface | Non-competitive surfaces only |

### Target OS

| Platform | Minimum (planning) |
|----------|--------------------|
| iOS | Version set at M1 from market data (typically last 2–3 major) |
| Android | API level set at M1; 64-bit required |

## 3. Server

| Layer | Recommendation | Notes |
|-------|----------------|-------|
| Primary language | **Go** or **C# (.NET)** | Pick one org-wide via ADR at M1; avoid polyglot sprawl early |
| Gateway | Envoy / API Gateway / cloud LB + BFF | Auth, rate limit, routing |
| APIs | gRPC internal; HTTPS JSON/gRPC-Web at edge as needed | Versioned |
| Realtime | Dedicated session servers (UDP/WebRTC or reliable UDP stack) | Authoritative |
| Async | Managed queues (SQS/PubSub/Kafka) | Ledger, fan-out, retries |
| Auth | JWT / opaque tokens + refresh; platform identity linking | Short-lived access tokens |

**Recommendation bias:** .NET if shared C# domain logic with Unity is highly valued; Go if ops simplicity and dense networking services are prioritized. **Decide in ADR-0001 at M1.**

## 4. Data

| Store | Use |
|-------|-----|
| PostgreSQL | System of record (accounts, inventory, purchases) |
| Redis | Sessions, rate limits, matchmaking ephemeral, locks |
| Object storage (S3-compatible) | Replays, dumps, Addressables catalogs (as applicable) |
| Analytics warehouse | BigQuery / Snowflake / ClickHouse (ADR) |
| Time-series | Metrics (Prometheus / cloud metrics) |

**Rules**

- No unbounded Mongo-as-primary without ADR.
- Inventory and currency mutations are **append-only ledger + current projection**.
- Migrations are expand/contract; never break Soft Launch schemas in-place.

## 5. Cloud & edge

| Concern | Recommendation |
|---------|----------------|
| Cloud | AWS or GCP (single primary until multi-cloud ADR) |
| Kubernetes | For stateless services when team ready; managed containers OK early |
| CDN | CloudFront / Fastly / Cloudflare for Addressables & static |
| Edge protection | WAF + DDoS (cloud native or Cloudflare) |
| Secrets | Cloud secret manager + CI OIDC; never long-lived keys in repo |

## 6. Observability

| Signal | Stack |
|--------|-------|
| Logs | Structured JSON → central logging |
| Metrics | Prometheus-compatible + dashboards |
| Traces | OpenTelemetry |
| Crashes (client) | Firebase Crashlytics or Sentry (interface) |
| Product analytics | Adjustable vendor behind events schema |

SLOs defined before Soft Launch for: auth success, match join, purchase validate, p95 session tick latency.

## 7. CI/CD tooling

| Piece | Recommendation |
|-------|----------------|
| SCM | GitHub or GitLab |
| CI | GitHub Actions / GitLab CI / Buildkite (Unity builders) |
| Unity build agents | Dedicated hardware or GameCI-class runners |
| Mobile distribute | TestFlight + Play internal/closed testing |
| IaC | Terraform |

See [CI_CD.md](../04-engineering/CI_CD.md).

## 8. Security tooling

- Dependency scanning (client NuGet/npm tools, server modules)
- Secret scanning on push
- SAST on server
- Mobile hardening (IL2CPP, obfuscation policy — see Security)
- Pentest before Soft Launch

## 9. Explicitly deferred / discouraged early

| Item | Why |
|------|-----|
| Custom engine | Cost vs. benefit |
| Blockchain / NFT inventory | Trust & store risk |
| Multiple competing DI/UI frameworks | Fragmentation |
| Peer-authoritative competitive mode | Cheat surface |
| Unmanaged dedicated bare metal first | Ops load |

## 10. Version pinning policy

- Engine, major SDKs, and IDL tools are pinned in repo.
- Upgrades require ADR + soak on `develop` + performance regression check.
- Security patches may fast-track with Security Lead approval.

## 11. ADR required before M1 close

- ADR: Unity exact version  
- ADR: Server language  
- ADR: Cloud provider  
- ADR: IDL (protobuf vs alternatives)  
- ADR: Analytics / crash vendors  
