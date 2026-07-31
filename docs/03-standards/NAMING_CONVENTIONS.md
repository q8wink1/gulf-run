# Naming Conventions

**Last updated:** 2026-07-31  
**Owner:** Engineering Manager  
**Audience:** Engineering, Tech Art, LiveOps

---

## 1. Purpose

Consistent naming across code, assets, branches, configs, and telemetry so a multi-year team can navigate without tribal knowledge.

## 2. Code (C#)

| Kind | Convention | Example |
|------|------------|---------|
| Namespace | `GulfRun.<Layer>.<Feature>` | `GulfRun.Features.Shop` |
| Class / struct | PascalCase | `PurchaseService` |
| Interface | `I` + PascalCase | `IPurchaseService` |
| Method | PascalCase | `ValidateReceiptAsync` |
| Local / param | camelCase | `receiptPayload` |
| Private field | `_camelCase` | `_httpClient` |
| Const / static readonly | PascalCase | `MaxRetryCount` |
| Enum | PascalCase type; PascalCase values | `RewardReason.SeasonCap` |
| Event / message | Past tense or noun phrase | `PurchaseCompleted` |
| Asmdef | `GulfRun.<Area>` | `GulfRun.Domain` |

### Feature folders

`Features/<FeatureName>/` where FeatureName is PascalCase matching assembly suffix.

## 3. Unity assets

| Kind | Convention | Example |
|------|------------|---------|
| Scene | `Scn_<Area>_<Name>` | `Scn_Meta_Hub` |
| Prefab | `Pf_<Type>_<Name>` | `Pf_Veh_DesertRunner` |
| Material | `M_<Name>` | `M_Sand_01` |
| Texture | `T_<Name>_<Suffix>` | `T_Sand_01_Albedo` |
| ScriptableObject | `SO_<Domain>_<Name>` | `SO_Event_RamadanCup` |
| Animator Controller | `AC_<Name>` | `AC_Player` |
| Addressable group | `AG_<Pack>` | `AG_Season_01` |
| UI Document | `UI_<Screen>` | `UI_Shop` |

Texture suffixes: `_Albedo`, `_Nrm`, `_Mask`, `_Emissive`, `_AO` (team may extend via Art guide).

## 4. Server

| Kind | Convention | Example |
|------|------------|---------|
| Service repo folder | kebab-case | `matchmaking` |
| gRPC package | `gulfrun.<service>.v1` | `gulfrun.inventory.v1` |
| DB tables | `snake_case` plural | `player_inventories` |
| Columns | `snake_case` | `updated_at` |
| Kafka/queue topics | `gulfrun.<domain>.<event>` | `gulfrun.economy.grant` |
| Config keys | `dot.or.snake` consistent | `liveops.event.ramadan_cup` |

## 5. LiveOps & content IDs

- Content IDs are **stable strings**: `evt_2028_s01_weekend_cup`
- Prefixes: `evt_`, `offer_`, `item_`, `bundle_`, `season_`, `quest_`
- Never reuse an ID for a different meaning; create a new ID.

## 6. Git

See [GIT_BRANCHING_STRATEGY.md](GIT_BRANCHING_STRATEGY.md).

| Kind | Pattern | Example |
|------|---------|---------|
| Feature branch | `feature/<ticket>-short-desc` | `feature/GR-123-shop-flags` |
| Fix | `fix/<ticket>-short-desc` | `fix/GR-450-login-npe` |
| Hotfix | `hotfix/<ticket>-short-desc` | `hotfix/GR-900-iap-dup` |
| Release | `release/x.y.z` | `release/1.2.0` |

## 7. Telemetry events

- `snake_case` or `dot.case` — pick one at M1 ADR; recommendation: `domain.object_action`
- Examples: `session.match_joined`, `economy.purchase_validated`, `ui.shop_opened`
- Properties: `snake_case`

## 8. Files & folders

- Docs: `SCREAMING_SNAKE` for major policy docs (existing), or kebab for runbooks later.
- Tools scripts: kebab-case filenames.
- No spaces in paths.

## 9. Platforms & defines

- Unity scripting defines: `GULFRUN_<AREA>` e.g. `GULFRUN_DEBUG_MENU`

## 10. Anti-patterns

- `Manager2`, `Temp`, `NewNewShop`
- Encoding version in type names instead of API versioning (`InventoryServiceV2` only if parallel temporary)
- Player-facing strings as code identifiers (use loc keys)
