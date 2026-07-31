# Future Scalability Plan

**Last updated:** 2026-07-31  
**Owner:** Principal Architect + DevOps Lead  
**Audience:** Server, DevOps, Leadership

---

## 1. Scale targets (planning envelopes)

Exact numbers are refined with Soft Launch data. Engineering must not assume “small forever.”

| Stage | Concurrent users (order) | MAU (order) | Notes |
|-------|--------------------------|-------------|-------|
| Vertical slice | tens–hundreds | n/a | Functional proof |
| Soft Launch | thousands–tens of thousands CCU | hundreds of thousands | One region |
| Global Year 1 | tens–hundreds of thousands CCU peaks | millions MAU | Multi-market |
| Mature LiveOps | design for **1M+ CCU class events** | multi-million MAU | Spiky seasonality |

Plan capacity for **events**, not averages.

## 2. Scalability principles

1. **Stateless edge, stateful cores carefully** — session servers are stateful but swappable; meta APIs are stateless.
2. **Partition by player_id / region / mode** early in schemas.
3. **Queues absorb spikes** — never let unchecked write storms hit primary DB.
4. **Shed load gracefully** — queue, degrade non-critical features, never corrupt ledgers.
5. **Measure cost per DAU** as a first-class metric.
6. **Design for replay of messages** — idempotency everywhere money/progress touches.

## 3. Growth stages

### Stage A — Single region (Soft Launch)

- One primary region (latency-appropriate to Soft Launch markets)
- PostgreSQL primary + replica
- Redis for ephemeral
- Session fleet autoscaled
- CDN global for assets even if APIs are regional

### Stage B — Regional expansion (Global)

- Active regions closer to players (e.g., ME, EU, NA, Asia as markets demand)
- Player home region + routing policy
- Cross-region play restricted by RTT budgets
- Replicated reference data (configs); **player ledger stays home-region** unless migrating

### Stage C — Mature multi-region

- Active-active for stateless; carefully designed for stateful
- Global identity directory with regional data planes
- Event-driven sync for social/clan light data
- DR: RPO/RTO targets defined and drilled

## 4. Service-specific strategies

| Service | Bottleneck risk | Strategy |
|---------|-----------------|----------|
| Identity | Login storms | Cache sessions; queue account creation; CDN for static | 
| Matchmaking | Hot queues | Shard by mode/skill; separate hot events |
| Session | CPU/tick | Bin-pack rooms; autoscale; mode-specific builds |
| Inventory/Economy | Write contention | Per-player partitioning; ledger+projection; command queue |
| LiveOps config | Read storm | Edge cache + versioned immutable configs |
| Purchase | Store APIs | Idempotent validate; async grant with retry |
| Analytics ingest | Volume | Client batching; sampling; async pipeline |

## 5. Data scalability

- **PostgreSQL:** migrate to partitioned tables by hash of `player_id` before Global if Soft Launch trends require.
- **Read replicas** for CRM/support and non-critical reads.
- **Redis:** separate clusters by purpose (do not share MM and cache eviction policies).
- **Object storage** for large payloads (replays), lifecycle policies.
- **Warehouse** out of hot path entirely.

## 6. Client scalability (device + store)

- Addressables + incremental content catalogs
- Asset quality tiers (Low/Mid/High)
- Feature flags to disable expensive modes on Low tier during thermal events
- App size budgets enforced in CI

## 7. Load testing program

| Gate | Requirement |
|------|-------------|
| M2 | Slice soak on target devices |
| M5 | Soft Launch peak × 10 critical paths |
| M7 | Global peak model × 3 with failure injection |
| Post-launch | Quarterly event rehearsal |

Scenarios: login stampede, matchmake storm, purchase validate, config fetch, session connect.

## 8. Degradation playbook (summary)

| Signal | Action |
|--------|--------|
| Matchmaking wait ↑ | Expand fleet; loosen skill band temporarily (flagged) |
| API error budget burn | Disable non-critical LiveOps surfaces |
| DB CPU ↑ | Shed leaderboards/social; protect auth+economy |
| Session CPU ↑ | Cap concurrent rooms; queue entry |
| CDN origin pressure | Raise TTLs; pre-warm catalogs |

## 9. Cost controls

- Autoscale with max caps + alerts
- Right-size session tick rates per mode
- Compress protocols; avoid chatty meta polling (push/config versions)
- Lifecycle delete replays/logs
- Review top expensive queries monthly

## 10. Organizational scalability

- On-call rotations by service ownership
- Clear SEV definitions (LiveOps doc)
- Platform team owns gateway, observability, CI runners
- Feature teams own services end-to-end (code + dashboards + runbooks)

## 11. Technology escape hatches

Documented in ADRs when triggered:

- Split monorepo art/LFS  
- Move session sim to specialized fleet / different language  
- Introduce Kafka (or equivalent) when queue semantics outgrow simple queues  
- Shard identity if regional law requires strict residency  

## 12. Success metrics

- p95 login < target; p95 match join < target  
- Error budget compliance  
- Crash-free users ≥ target  
- Cost / DAU within band  
- Zero unresolved Sev-1 caused by known single points of failure after Stage B  
