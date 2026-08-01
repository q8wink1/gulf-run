# ADR-0001: Multiplayer Transport Abstraction for the Sprint 4 Foundation

**Status:** Proposed
**Date:** 2026-08-01
**Deciders:** Lead Multiplayer Engineer (Sprint 4)
**Consulted:** Principal Architect, Server Lead (per [MULTIPLAYER_ARCHITECTURE.md](../02-architecture/MULTIPLAYER_ARCHITECTURE.md) ownership)

## Context

[MULTIPLAYER_ARCHITECTURE.md §9](../02-architecture/MULTIPLAYER_ARCHITECTURE.md#9-netcode-approach-options-choose-via-adr) explicitly defers the Netcode approach (custom authoritative protocol vs. Unity Netcode for GameObjects vs. a third party such as Photon/Mirror) to an ADR, and no ADR has been created yet (`docs/adr/README.md` index was empty before this one). [UNITY_PACKAGES.md](../04-engineering/UNITY_PACKAGES.md) marks `com.unity.netcode.gameobjects`/`com.unity.transport` as **Conditional — "Only if ADR selects Unity netcode"**, yet both packages are already present in `Client/Packages/manifest.json` from Sprint 1's "Netcode preparation" instruction. Sprint 1/2/3 reports all carry forward "Netcode ADR" as an open item.

Sprint 4 ("Multiplayer Foundation") requires standing up Lobby/Match/Session/Connection/Spawn managers, player synchronization, and network interpolation **now**, but the brief also says "Do NOT implement final gameplay logic" and "Prepare the project for scalable online multiplayer" — i.e. build the architecture, not commit to a wire protocol that the real Netcode ADR might reject. Committing directly to Unity Netcode for GameObjects (or any concrete transport) inside every gameplay manager would violate the "choose via ADR" governance rule and create vendor lock-in risk before Principal Architect/Server Lead sign-off.

## Options considered

1. **Build directly on Unity Netcode for GameObjects (`NetworkBehaviour`/`NetworkVariable`/RPCs) now.**
   Pros: uses the already-installed package; no abstraction overhead.
   Cons: prejudges the still-open ADR-0001-worthy decision that MULTIPLAYER_ARCHITECTURE.md explicitly reserves for Architect+Server Lead sign-off; the doc's own default recommendation leans toward a custom/wrapped session service, not host-authoritative NGO, for competitive integrity; every gameplay manager would need rewriting if the ratified ADR picks something else.
2. **Wait for the Netcode ADR before writing any multiplayer code.**
   Pros: avoids any wasted work.
   Cons: blocks the entire Sprint 4 brief indefinitely; the Lobby/Ready/Countdown/Spawn/Sync architecture, data model, and manager responsibilities are independent of *which* transport eventually carries the bytes and don't need to wait.
3. **Introduce a transport-agnostic seam (`Core.Networking.IMatchTransport`) that every manager depends on, backed today by an in-process `LocalLoopbackTransport`, with the real Netcode decision deferred to this ADR's ratification.**
   Pros: unblocks the full Sprint 4 architecture immediately; zero gameplay-facing code changes required once a real transport is chosen — only `MatchTransportService.Current`'s assigned implementation changes; keeps faith with MULTIPLAYER_ARCHITECTURE.md's governance; the same interface can be implemented identically for a future dedicated server process (no UnityEngine dependency in the interface or the default implementation).
   Cons: the default `LocalLoopbackTransport` cannot demonstrate real cross-machine networking — it is honestly scoped as an architecture-validation/offline-testing tool only, not a networking solution.

## Decision

Adopt **Option 3**. `Core.Networking.IMatchTransport` is the only type any Multiplayer manager (Connection/Lobby/Match/Session/Spawn/Sync) depends on. `Core.Networking.MatchTransportService.Current` is a self-initializing service-locator slot defaulting to `LocalLoopbackTransport` (pure C#, no UnityEngine dependency, simulates local-only "remote" participants for testing). This ADR does **not** itself select Unity Netcode for GameObjects, a custom protocol, or a third party — that remains a separate, future decision for Principal Architect + Server Lead, recorded either as an amendment to this ADR or a new one, per `docs/adr/README.md` rule 1 (one ADR per decision).

Recommendation for that follow-up decision, consistent with MULTIPLAYER_ARCHITECTURE.md §2/§9's non-negotiable server-authority model: prefer a custom or carefully wrapped authoritative session service over host-authoritative Unity Netcode for GameObjects for ranked/competitive racing; NGO remains acceptable if deployed in a dedicated-server (not host-migration P2P) configuration with an explicit server-authority story.

## Consequences

### Positive

- Every Sprint 4 manager (Connection/Lobby/Match/Session/Spawn/Sync) is fully implemented and testable today (see `MultiplayerDebugView`'s Create/Ready/Simulate-Join/Leave buttons) without waiting on, or prejudging, the Netcode ADR.
- Swapping in a real transport later is a one-line change (`MatchTransportService.Current = new <RealTransport>()`), not a rewrite — no gameplay code references any concrete transport type.
- `LocalLoopbackTransport` has zero UnityEngine dependency, so the identical class is a candidate starting point for logic that later runs on a plain-.NET dedicated server process — a concrete step toward "future dedicated server compatibility," not just a documented intention.

### Negative / risks

- `LocalLoopbackTransport` cannot be used to validate real network conditions (latency, packet loss, bandwidth) — load testing and lag-compensation validation (MULTIPLAYER_ARCHITECTURE.md §12 deliverables) still require a real transport.
- Until a real transport ships, `JoinMatch`/`JoinAsClient` cannot actually reach another machine; multi-client testing is limited to one process simulating extra local participants.
- If the eventual Netcode ADR selects a solution whose idiomatic usage pattern doesn't map cleanly onto `IMatchTransport`'s event/method shape (e.g. a very RPC-heavy or ECS-heavy model), the interface itself may need a follow-up revision — acceptable, since it is an internal seam, not a public contract.

### Follow-ups

- [ ] Ratify (or supersede) this ADR once Principal Architect + Server Lead select the concrete Netcode approach (Unity Netcode for GameObjects in dedicated-server mode, a custom protocol, or a third party) per MULTIPLAYER_ARCHITECTURE.md §9.
- [ ] Implement that transport as a new `IMatchTransport`, register it from `Core.Managers.NetworkManager.OnInitialize()`.
- [ ] Load test, lag-compensation, and disconnect/forfeit policy validation (MULTIPLAYER_ARCHITECTURE.md §12) against the real transport, not `LocalLoopbackTransport`.
- [ ] Review date: before Soft Launch networking hardening milestone.

## References

- [MULTIPLAYER_ARCHITECTURE.md](../02-architecture/MULTIPLAYER_ARCHITECTURE.md) §2 (authority model), §9 (Netcode options)
- [UNITY_PACKAGES.md](../04-engineering/UNITY_PACKAGES.md) (Netcode packages: Conditional)
- [SPRINT-04-MULTIPLAYER-FOUNDATION.md](../07-sprints/SPRINT-04-MULTIPLAYER-FOUNDATION.md)
- `Client/Assets/_Project/Scripts/Core/Networking/IMatchTransport.cs`, `LocalLoopbackTransport.cs`, `MatchTransportService.cs`
