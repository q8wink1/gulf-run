# Sprint 21 — Complete Quick Play Lobby Flow

**Scope:** End-to-end Quick Play → Lobby → Ready/Host/Kick → Map Vote → Loading → Gameplay. Networking stays mock (`LocalLoopbackTransport` + `MockPublicRoomDirectory`).

## Flow

`PlayMenu` → `QuickPlay` (search) → `Lobby` → Host **Play** → `MapVoting` → `Loading` → `Gameplay`

## Mock matchmaking priority

`MockPublicRoomDirectory` seeds public rooms. On Quick Play search:

1. Prefer joinable rooms with highest occupancy under capacity: **3/4 → 2/4 → 1/4**
2. If none: create a new public room (local is Host)
3. Empty seats fill gradually (placeholder joiners); remotes Ready on a stagger
4. Host Kick → delayed refill search for the empty seat

## Lobby rules

- Max **4** players (`NetworkSyncConfig`)
- Quick Play **Play** enabled only when **4/4 all Ready**; Host-only
- Kick Host-only; Host Migration via existing `HostMigrationController`
- Leave returns to Play Menu

## Map Voting

- Three random maps from `MapCatalogConfig`
- Live vote counts; countdown; tie → random among tied
- Winner applied via `MapEnvironmentManager.ApplyForcedMap`

## Key types

| Piece | Location |
|-------|----------|
| Public room mock | `Features/Multiplayer/Matchmaking/MockPublicRoomDirectory.cs` |
| Session / Quick Play | `SessionManager` |
| Map vote state | `MapVotingSession` + `IMapVotingProvider` |
| Map vote UI | `Features/Matchmaking/MapVoting/MapVotingView.cs` |
| Loading handoff | `LoadingTransitionController` |
