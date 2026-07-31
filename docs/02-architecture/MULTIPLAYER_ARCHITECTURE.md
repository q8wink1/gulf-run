# Multiplayer Architecture Recommendations

**Last updated:** 2026-07-31  
**Owner:** Principal Architect + Server Lead  
**Audience:** Netcode, gameplay, backend engineers

---

## 1. Goals

- Support competitive and cooperative online play at **millions of MAU** with bursty peaks.
- Preserve **competitive integrity** (server authority).
- Keep mobile clients thin: bandwidth, battery, thermals.
- Allow Solo / async modes without a second economy.

## 2. Authority model (non-negotiable)

| Data / outcome | Authority |
|----------------|-----------|
| Match results, scores, placements | Server |
| Rewards, XP, currency, inventory | Server |
| Purchases | Server + store validators |
| Cosmetics equip (owned check) | Server validates ownership |
| Presentation, VFX, camera | Client |
| Predictive movement (if any) | Client predicts; server reconciles |

**Never** trust the client for grant amounts, RNG of value, or match outcomes.

## 3. Topology

```
Mobile Client
    │ HTTPS (meta, auth, liveops, purchase)
    ▼
API Gateway / BFF
    │
    ├── Identity · Inventory · Economy · LiveOps · Purchase
    │
    └── Matchmaking Service
            │
            ▼
      Session Allocator
            │
            ▼
   Authoritative Session Servers (shards / rooms)
            │
            ▼
      Result → Economy (signed settlement)
```

### Connection types

| Channel | Transport | Use |
|---------|-----------|-----|
| Meta | HTTPS/gRPC | Login, inventory, shop, config |
| Realtime | Reliable UDP / WebRTC data / platform-appropriate | Session simulation |
| Push | FCM/APNs | Re-engagement, not gameplay authority |

## 4. Session design

- **Short-lived authoritative rooms** sized for mode (e.g., 1v1, small lobby, race field).
- Tick rate chosen for mode; mobile-first (often 10–30 Hz sim, not 128 Hz FPS defaults).
- Interest management / relevancy to cut bandwidth.
- Deterministic **settlement** packet: session server → economy service with HMAC/service auth.
- Reconnect: resume within grace window; otherwise forfeit rules from Design.

## 5. Matchmaking

Phases:

1. Soft Launch: single-region, skill + latency buckets, party support optional  
2. Global: regional queues, cross-region only with latency gates  
3. Scale: priority queues, event cups, unfair-match detection  

Backfill and AI fillers only if Design + integrity approve (must be labeled for analytics).

## 6. Time and RNG

- Server time is canonical; clients display offsets.
- Valuable RNG (loot, crits of economic value) is **server-side**.
- Cosmetic RNG may be server-generated and revealed to client.

## 7. Data synchronization (meta)

- Inventory version vectors / etags to avoid lost updates.
- Idempotent command APIs (`command_id`) for spend/grant.
- Client caches are hints; 409/replay rules documented in API guidelines (future `docs/api`).

## 8. Scalability sketch

| Component | Scale strategy |
|-----------|----------------|
| Gateway | Horizontal + rate limits |
| Matchmaking | Sharded queues by region/mode |
| Session servers | Autoscale on concurrent rooms; bin-pack players |
| Economy | Partition by `player_id`; queue writes |
| Redis | Cluster; separate concerns (MM vs rate limit) |

See [SCALABILITY_PLAN.md](SCALABILITY_PLAN.md).

## 9. Netcode approach options (choose via ADR)

| Option | Fit |
|--------|-----|
| Custom lightweight authoritative protocol | Max control; more eng cost |
| Unity Netcode for GameObjects / Entities | Only if it fits authority + mobile budget |
| Third-party (Photon, Mirror, etc.) | Allowed only with **server authority** story and exit plan |

**Default recommendation:** Custom or carefully wrapped session service with thin Unity client; avoid host-authoritative P2P for ranked/competitive.

## 10. Anti-cheat touchpoints

- Movement/action sanity on session server
- Settlement validation before rewards
- Replay sampling for review
- Device/attestation signals at login (platform permitting)

Details: [ANTI_CHEAT.md](../05-security/ANTI_CHEAT.md).

## 11. Offline / airplane mode

- Purely cosmetic or practice modes may run offline.
- Any reward path requires online settlement.
- Single-player campaign rewards still server-claimed when online.

## 12. Deliverables before Soft Launch

- [ ] Protocol versioning & compatibility matrix
- [ ] Lag compensation policy documented
- [ ] Disconnect / forfeit rules implemented
- [ ] Load test: concurrent rooms target
- [ ] Kill switch: disable mode remotely
