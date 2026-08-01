# GulfRun Documentation Index

**Source of truth split**

- **Gameplay design:** [`Design/GDD/`](../Design/GDD/README.md) (Design Owner fills; never invent)
- **Engineering / architecture / ops:** this `docs/` tree

All future client, server, tools, and LiveOps *implementation* must conform to engineering docs and to **Approved** GDD sections. Missing GDD content → ask questions; do not assume.

---

## How to use this library

| Role | Start here |
|------|------------|
| New engineer | [Coding Standards](03-standards/CODING_STANDARDS.md) → [Folder Architecture](02-architecture/FOLDER_ARCHITECTURE.md) → [Technical Stack](02-architecture/TECHNICAL_STACK.md) |
| Tech lead | [Roadmap](01-planning/ROADMAP.md) → [Phases](01-planning/DEVELOPMENT_PHASES.md) → [Risk](00-governance/RISK_ASSESSMENT.md) |
| Producer / TD | [Milestones](01-planning/MILESTONES.md) → [LiveOps](06-operations/LIVE_OPERATIONS.md) |
| Security | [Security](05-security/SECURITY_STRATEGY.md) → [Anti-Cheat](05-security/ANTI_CHEAT.md) |
| DevOps | [CI/CD](04-engineering/CI_CD.md) → [Scalability](02-architecture/SCALABILITY_PLAN.md) → [External Services](06-operations/EXTERNAL_SERVICES.md) |

---

## Document catalog

### 00 — Governance

| Document | Purpose |
|----------|---------|
| [DOCUMENTATION_STRUCTURE.md](00-governance/DOCUMENTATION_STRUCTURE.md) | Doc taxonomy, ownership, review cadence |
| [RISK_ASSESSMENT.md](00-governance/RISK_ASSESSMENT.md) | Technical, product, ops, and compliance risks |

### 01 — Planning

| Document | Purpose |
|----------|---------|
| [ROADMAP.md](01-planning/ROADMAP.md) | Multi-year product and technology roadmap |
| [MILESTONES.md](01-planning/MILESTONES.md) | Gateable milestones with exit criteria |
| [DEVELOPMENT_PHASES.md](01-planning/DEVELOPMENT_PHASES.md) | Phase definitions from foundation to LiveOps scale |

### 02 — Architecture

| Document | Purpose |
|----------|---------|
| [FOLDER_ARCHITECTURE.md](02-architecture/FOLDER_ARCHITECTURE.md) | Monorepo / multi-repo layout and ownership |
| [TECHNICAL_STACK.md](02-architecture/TECHNICAL_STACK.md) | Client, server, data, cloud stack |
| [MULTIPLAYER_ARCHITECTURE.md](02-architecture/MULTIPLAYER_ARCHITECTURE.md) | Netcode, authority, matchmaking, scale |
| [SCALABILITY_PLAN.md](02-architecture/SCALABILITY_PLAN.md) | Path from thousands to millions of players |
| [BACKEND_ARCHITECTURE.md](02-architecture/BACKEND_ARCHITECTURE.md) | Backend responsibilities, authority, client/backend split, sync & security principles (P039 v1.0) |
| [DATABASE_ARCHITECTURE.md](02-architecture/DATABASE_ARCHITECTURE.md) | Data categories, principles, ownership, sync & security statements, backup requirement (P040 v1.0) |
| [AUTHENTICATION_SYSTEM.md](02-architecture/AUTHENTICATION_SYSTEM.md) | Account types, auth flow, linking, session management, error handling (P041 v1.0) |
| [ANALYTICS_SYSTEM.md](02-architecture/ANALYTICS_SYSTEM.md) | Analytics categories, tracked data, technical analytics, privacy rules (P044 v1.0) |
| [PERFORMANCE_OPTIMIZATION_SPECIFICATION.md](04-engineering/PERFORMANCE_OPTIMIZATION_SPECIFICATION.md) | Target/minimum frame rate, optimization principles, graphics/memory/network/loading rules (P046 v1.0) |
| [TECHNICAL_ARCHITECTURE.md](02-architecture/TECHNICAL_ARCHITECTURE.md) | Architecture principles, project layers, core system managers, dependency rules, code quality (P049 v1.0) |

### 03 — Standards

| Document | Purpose |
|----------|---------|
| [CODING_STANDARDS.md](03-standards/CODING_STANDARDS.md) | Language rules, patterns, quality bars |
| [NAMING_CONVENTIONS.md](03-standards/NAMING_CONVENTIONS.md) | Code, assets, branches, IDs |
| [ASSET_ORGANIZATION.md](03-standards/ASSET_ORGANIZATION.md) | Art/audio/UI/content pipelines |
| [GIT_BRANCHING_STRATEGY.md](03-standards/GIT_BRANCHING_STRATEGY.md) | Branch model, reviews, releases |

### 04 — Engineering

| Document | Purpose |
|----------|---------|
| [UNITY_PACKAGES.md](04-engineering/UNITY_PACKAGES.md) | Approved and deferred Unity packages |
| [CI_CD.md](04-engineering/CI_CD.md) | Pipelines, environments, release trains |
| [MOBILE_OPTIMIZATION.md](04-engineering/MOBILE_OPTIMIZATION.md) | Device tiers, budgets, profiling |

### 05 — Security

| Document | Purpose |
|----------|---------|
| [SECURITY_STRATEGY.md](05-security/SECURITY_STRATEGY.md) | App, API, data, account security |
| [ANTI_CHEAT.md](05-security/ANTI_CHEAT.md) | Detection, prevention, response |
| [ANTI_CHEAT_SPECIFICATION.md](05-security/ANTI_CHEAT_SPECIFICATION.md) | Requirements-level spec: protected systems, validation scope, principles (P043 v1.0) |

### 06 — Operations

| Document | Purpose |
|----------|---------|
| [LIVE_OPERATIONS.md](06-operations/LIVE_OPERATIONS.md) | Content cadence, economy, incident ops |
| [EXTERNAL_SERVICES.md](06-operations/EXTERNAL_SERVICES.md) | Third-party integrations roadmap |

### 07 — Sprints

| Document | Purpose |
|----------|---------|
| [SPRINT-01-PROJECT-FOUNDATION.md](07-sprints/SPRINT-01-PROJECT-FOUNDATION.md) | Sprint 1 report: folders, managers, scenes, packages, settings, and open items |
| [SPRINT-02-PLAYER-CONTROLLER-FOUNDATION.md](07-sprints/SPRINT-02-PLAYER-CONTROLLER-FOUNDATION.md) | Sprint 2 report: player prefab, movement/input/camera/animator scripts, physics config, and open items |
| [SPRINT-03-ENDLESS-RUNNER-CORE.md](07-sprints/SPRINT-03-ENDLESS-RUNNER-CORE.md) | Sprint 3 report: world generation, object spawning/pooling, game speed, distance, scoring, game loop, save interfaces, debug tools, and open items |
| [SPRINT-04-MULTIPLAYER-FOUNDATION.md](07-sprints/SPRINT-04-MULTIPLAYER-FOUNDATION.md) | Sprint 4 report: transport-agnostic multiplayer architecture, Match Flow, Lobby/Ready System, shared countdown, player sync/interpolation, network managers, and open items |
| [SPRINT-05-WEAPONS-ITEM-BOXES-COMBAT.md](07-sprints/SPRINT-05-WEAPONS-ITEM-BOXES-COMBAT.md) | Sprint 5 report: pooled Item Boxes, 2-slot weapon inventory, 9 Standard + 1 Legendary weapons, targeting types, host-authoritative pickup/use/hit networking, status effects, debug tools, and open items |
| [SPRINT-06-DYNAMIC-TRAP-SYSTEM.md](07-sprints/SPRINT-06-DYNAMIC-TRAP-SYSTEM.md) | Sprint 6 report: 15 map-owned traps, host-authoritative randomized spawn/trigger/expiration networking, pooling, difficulty-scaled randomization, shared status-effect vocabulary, debug tools, and open items |
| [SPRINT-07-RACE-FINISH-RANKING-VICTORY-CEREMONY.md](07-sprints/SPRINT-07-RACE-FINISH-RANKING-VICTORY-CEREMONY.md) | Sprint 7 report: configurable race length, finish/elimination detection, host-authoritative final ranking, Podium Ceremony with camera movement and victory music, private per-player animated Reward Screen, automatic lobby return, full networking, debug tools, and open items — **§14 addendum:** national flags, Golden Trophy/Gulf Bisht/golden confetti/special victory music for the champion, celebration animations for 2nd/3rd, and a corrected individual (non-interrupting) ceremony skip |
| [SPRINT-08-CHARACTERS-COUNTRIES-CUSTOMIZATION.md](07-sprints/SPRINT-08-CHARACTERS-COUNTRIES-CUSTOMIZATION.md) | Sprint 8 report: 12 unlocked-from-launch playable characters, one-time permanent Account Creation (Display Name + Country) with auto-applied free Traditional Outfits for all 8 launch countries, Gem-funded Premium Cosmetics across 10 future-proofed slots, extended Win/Lose/Celebrate animation vocabulary, full Character/Country/Outfit/Cosmetics networking, Character Menu, debug tools, and open items |
| [SPRINT-09-ONLINE-ECOSYSTEM-RANKINGS-FRIENDS-CHAMPIONSHIPS.md](07-sprints/SPRINT-09-ONLINE-ECOSYSTEM-RANKINGS-FRIENDS-CHAMPIONSHIPS.md) | Sprint 9 report: World/Gulf/Country/Weekly/Monthly/Seasonal leaderboards for all 8 launch countries, public Player Profiles with clickable ranks, 5-state Online Status, a full Friend System addable from every brief entry point, Nickname/Player-ID/Country search, a permanent Hall of Fame, 8 Leagues with season promotion/relegation, 5 Championships, 13 Country/seasonal Events, a 10-category Reward catalog, full Player Statistics, Notifications, a mock-but-swappable cloud-ready backend abstraction, leaderboard caching, debug tools, and open items |
| [SPRINT-10-STORE-ECONOMY-BATTLE-PASS.md](07-sprints/SPRINT-10-STORE-ECONOMY-BATTLE-PASS.md) | Sprint 10 report: a 10-tab modern Store (Special Offers/Gems/Coins/Battle Pass/Characters/Outfits/Emotes/Victory Poses/Visual Effects/Profile Frames), 6 configurable Gem Packages, 5 Coin Packs, an 18-item Store catalog, 7 Limited/Special Offer bundles, a 10-tier Paid-only Premium Monthly Battle Pass, a full Purchase System (Confirmation/History/Restore/Validation/Refund Protection), Player Wallet, Inventory, Store/Economy Notifications, a mock-but-swappable cloud-ready backend abstraction, debug tools, and open items |
| [SPRINT-11-DAILY-MISSIONS-LOGIN-REWARDS.md](07-sprints/SPRINT-11-DAILY-MISSIONS-LOGIN-REWARDS.md) | Sprint 11 report: 3 randomly-assigned Daily Missions from a 25-entry configurable pool (Easy/Medium/Hard, automatic reward scaling), a 7-day Login Streak with a standard calendar plus 5 Special Login Event calendars (Ramadan/Eid/National Days/Summer/Winter), 2/3/7-day Temporary Cosmetics with countdown timers, automatic expiry removal, and a Store "Unlock Permanently" upsell, a duplicate-avoidance fallback reward rule, Battle Pass XP as a reward type, Mission/Login/Temporary-Item notifications, a mock-but-swappable cloud-ready Progression backend abstraction, debug tools, and open items |
| [SPRINT-12-GULF-MAPS-LEVEL-DESIGN.md](07-sprints/SPRINT-12-GULF-MAPS-LEVEL-DESIGN.md) | Sprint 12 report: six Gulf-inspired launch maps (Kuwait City/Riyadh/Dubai/Doha/Manama/Muscat) sharing one fair, reusable 11-section-type chunk library (Flat/Small Hill/Slope/Bridge/Wood Platform/Stone Platform/Jump Platform/Short Tunnel/Open Area/Small Drop/Small Climb) with a continuous flat ground collider under every platform, a data-driven per-match Map/Weather/Time-of-Day resolver with fresh Trap/Item-Box random seeds every match, background-only per-city landmarks, animated-background-element data flags driving a reusable parallax component, per-city ambient day/night audio, debug tools, and open items |
| [SPRINT-13-MAIN-MENU-LOBBY.md](07-sprints/SPRINT-13-MAIN-MENU-LOBBY.md) | Sprint 13 report: the complete Main Menu/Lobby — an animated per-session background (random launch map + random Morning/Sunset/Night, moving clouds/birds/palm trees) reusing Sprint 12's real map resolver, a breathing centered player preview with outfit/country/flag, a Top Bar (Name/Level/League/Ranks/Coins/Gems/Settings/Notifications), Left Menu (Friends/Clan/Leaderboard/Missions/Battle Pass/Mail) and Right Menu (Store/Characters/Customize/Inventory/Events/Championships), a glowing golden PLAY button driving matchmaking end-to-end into the first-ever `Boot → MainMenu` scene transition, a Daily Missions widget, a Login Reward popup, a many-producer rotating Event Banner, a Social panel (Friends/Clan Online, Invite, Room Code), a Voice Chat widget, a Settings panel with live audio category volumes, floating gold particles, a shared "Modern Gulf Identity" theme/button-press-animation kit, 11 new `Core.Services` seams keeping the new UI composition root's only Feature-boundary exception clean, debug tools, and open items |
| [SPRINT-14-GULFRUN-BRAND-INTRO.md](07-sprints/SPRINT-14-GULFRUN-BRAND-INTRO.md) | Sprint 14 report: the official GulfRun animated Brand Intro — a 2.65s `Boot → Intro → MainMenu` sequence (moving desert sand dunes, soft wind particles, a falcon flying across the screen then circling above the dunes, a slowly-fading-in palm tree silhouette, the GulfRun logo fading in with a premium golden shine sweep), skippable from the second launch onward via the project's first genuine cross-restart persistence (`SaveManager.HasSeenIntro`, PlayerPrefs-backed), sound cues (startup/desert wind/falcon wing/logo shimmer) crossfading into a real Lobby music fade-in (`AudioManager.FadeMusicTo`), a smooth fade-to-black transition, and one new shared `Core.Branding.GulfRunBrandMark` logo drawing routine (Dunes+Falcon+Palm Tree+Forward Motion) now placed everywhere this client can reach — Loading screen, Main Menu Top Bar, Store header, Battle Pass header — plus debug/build verification and open items |
| [SPRINT-14-MATCHMAKING-ROOM-PRE-RACE-LOBBY.md](07-sprints/SPRINT-14-MATCHMAKING-ROOM-PRE-RACE-LOBBY.md) | Sprint 14 report (Matchmaking track): Quick Play search → Match Found → Pre-Race Lobby scene; Private Room create/join/code copy-share; Ready System + Bot Fill; Auto Start 5-4-3-2-1-GO into Gameplay; player cards; owner Kick/Invite/Start; Quick Chat + Voice widget; host migration + connection quality; `GulfRun.Features.Matchmaking`; offline compile/YAML verification and open items |
| [SPRINT-15-RACE-HUD-GAMEPLAY-UI.md](07-sprints/SPRINT-15-RACE-HUD-GAMEPLAY-UI.md) | Sprint 15 report: Race HUD / Gameplay UI & In-Race Experience — countdown presentation, player chrome (position/lap/speed/shield/weapon/coins/gems/timer), trap warning, effect duration bars, quick emotes, race-progress minimap, finish banner with fireworks/confetti, camera look-ahead/bob/impact shake, dust VFX, audio director, Gulf theme + ScriptableObject config, accessibility/responsive scaling, `RaceHudDebugView` at panelX 4510, Core.Services seams, offline compile/YAML verification, and open items |
| [SPRINT-16-CHARACTER-SELECTION-LOCKER.md](07-sprints/SPRINT-16-CHARACTER-SELECTION-LOCKER.md) | Sprint 16 report: Character Selection / Locker / Customization — Majlis showroom with cinematic camera, 12 unlocked characters, locked Country display, full Locker categories with rarity/filters/search, permanent Gem + temporary mission/reward outfits, instant equip with PlayerPrefs loadout persistence, Gulf CharacterTheme UI, debug tools, and open items |

### Architecture Decision Records

| Path | Purpose |
|------|---------|
| [adr/](adr/README.md) | Numbered ADRs; template for all major decisions |

---

## Change control

1. Editorial fixes (typos, clarity): PR by any engineer, one reviewer.
2. Normative changes (standards, architecture): PR + **Tech Director** approval + ADR if scope warrants.
3. Security / anti-cheat / economy authority changes: PR + Security + Tech Director.

Versioning: documents use `Last updated` dates. Breaking policy changes require an ADR linking to the amended section.
