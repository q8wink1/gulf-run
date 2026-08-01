# Sprint 16 — Character Selection, Customization & Locker — Sprint Report

**Role:** Lead Character / Customization Systems Engineer  
**Scope:** Complete the Character Selection / Locker / Customization experience on top of Sprint 8–11 foundations: 12 unlocked launch characters; locked Country display (Flag/Name/Country Ranking); default national outfit after character select; full Locker categories (Outfits, Headwear, Glasses, Victory Poses, Emotes, Footstep Effects, Running Effects, Profile Frames, Titles); Permanent + Temporary (2/3/7-day) outfits with countdown and auto-expire; Gem permanent purchases via existing unlock path; temporary grants via existing `ICosmeticGrantService` (Daily Missions / Login Rewards / Battle Pass / Events / Championships); Majlis showroom with cinematic rotate/zoom/auto-focus camera; Idle/Run/DoubleJump/Victory/Defeat/Celebrate preview animations with breathing/blink/transitions; rarity borders/glow/reward flash; Owned/Not Owned/Temporary/Permanent/Country + Newest/Rarity filters; Name/Category/Country search; instant equip/unequip with automatic PlayerPrefs persistence; Gulf sand/gold UI theme local to Features.Character; null-safe dressing-room audio; responsive layout scaling; debug panel fields; SOLID/ScriptableObject configs.  
**Status:** Complete, under the same environment constraint documented in [SPRINT-01-PROJECT-FOUNDATION.md](SPRINT-01-PROJECT-FOUNDATION.md) §0 (no licensed Unity Editor on this machine — Unity Hub is installed but no Editor version is downloaded).

---

## 0. Continuation check

Sprints 1–14 are complete and were **not** rewritten. Sprint 8 already shipped 12 characters, Account Creation, Traditional Outfits, Gem unlocks, and networking; Sprint 11 already shipped temporary cosmetics + `ICosmeticGrantService` grant seams. This sprint **extends** those systems into a full Locker/Showroom experience. In-flight Matchmaking / Race HUD working-tree changes on `main` were left untouched (only additive shim helpers needed for offline compile coexistence: `Mathf` int `Max`/`Min`, `Sqrt`, `PlayerPrefs` string APIs).

---

## 1. Architecture

| Layer | Type | Responsibility |
|---|---|---|
| Domain | `CosmeticRarity` | Common → Mythic (visual-only ordering). |
| Domain | `LockerCategory` / `LockerCategoryMapping` | UI categories mapped onto `CosmeticSlot` without breaking Sprint 8 asset ordinals (`Hat` = Headwear). |
| Domain | `LockerOwnershipFilter`, `LockerSortMode` | Owned / NotOwned / Temporary / Permanent / Country + Newest / Rarity. |
| Domain | `CosmeticSlot` (extended) | Appended `FootstepEffect`, `RunningEffect`, `ProfileFrame`, `Title`. |
| Domain | `LoadoutSaveData` | Pure encode/decode snapshot for persistence. |
| Core.Save | `ILoadoutRepository` | Implemented by `SaveManager` via PlayerPrefs. |
| Core.Services | `MenuScreen.Locker` | Routes Main Menu "Customize" to the Locker outfits tab. |
| Features.Character | `LockerUiConfig` | Camera / animation / layout / rarity tunables — no magic numbers in views. |
| Features.Character | `CharacterTheme` | Gulf sand/gold palette + rarity colors — Features never references `MainMenuTheme`. |
| Features.Character | `LockerView` + showroom/filter/animator helpers | Full OnGUI Locker experience. |
| Features.Character | `PlayerLoadoutManager` (extended) | Unequip, traditional-outfit re-equip, persist/restore, preview animation state. |
| Configuration | `CosmeticCatalogConfig` (extended) | Rarity, catalogIndex, countryTagged, allowTemporaryGrant + new category sample items. |

---

## 2. Characters & Country

- **12 characters**, all unlocked by default (unchanged Sprint 8 rule — `SelectCharacter` has no ownership gate).
- Selecting a character updates the showroom preview instantly and re-applies the official national Traditional Outfit if Outfit is empty.
- **Country** remains account-locked: Locker shows Flag (placeholder color), Display Name, and Country Ranking via `ILocalProfileProvider` (no Features.Online reference). No country change UI exists.

---

## 3. Locker categories, rarity, filters, search

Categories: Characters, Outfits, Headwear, Glasses, Victory Poses, Emotes, Footstep Effects, Running Effects, Profile Frames, Titles.

Each cosmetic card draws a rarity-colored border + pulsed glow + brief reward flash on equip/purchase. Filters: All / Owned / Not Owned / Temporary / Permanent / Country. Sort: Newest (`catalogIndex`) / Rarity. Search matches Display Name, Collection Tag (category), and Country name.

---

## 4. Permanent vs Temporary ownership

- **Permanent:** Gem purchase via existing `PlayerLoadoutManager.TryUnlockCosmetic` (Store/Battle Pass permanent grants continue through `ICosmeticGrantService.GrantCosmetic`).
- **Temporary:** 2/3/7-day grants from Progression/Events continue through `GrantTemporaryCosmetic`; Locker shows live countdown; `RemoveExpired` + unequip + persist already run on a throttled tick.
- Catalog flags `allowTemporaryGrant` so limited/mythic items can refuse temporary grants at data level.

---

## 5. Showroom, camera, animations

OnGUI Majlis environment: luxury carpet, palms, lantern soft lighting, coffee set, modern Gulf wall, large window overlooking the player's Gulf city skyline (placeholder boxes). Camera: 360° rotate, zoom in/out, auto-rotate toggle, auto-focus lerp. Preview animations cycle Idle → Run → DoubleJump → Win → Lose → Celebrate with breathing, blink, smile, idle sway, and transition blending.

---

## 6. Equip & Save

Equip/unequip is instant (no loading). Every select/equip/unequip/unlock/grant/expiry path calls `SaveManager.SaveLoadout` (`ILoadoutRepository`), encoding Character + equipped slots + permanent/temporary ownership into `PlayerPrefs` key `GulfRun.Loadout.v1`. This is the project's **second** deliberate PlayerPrefs exception (after Sprint 14's `HasSeenIntro`) — documented honestly on `SaveManager`. Account progress fields remain in-memory.

---

## 7. Audio, responsive layout, performance

- Soft dressing-room music + button/equip/reward one-shots — all `AudioClip` fields null-safe via `AudioManager`.
- `LockerUiConfig.ResolveUiScale()` clamps phone/tablet scale from a 1080×1920 reference.
- Filter scratch lists are reused; temporary expiry checks stay throttled; OnGUI redraw is limited to the open Locker panel.

---

## 8. Main Menu wiring & debug

- `LockerView` registers `MenuScreen.Characters` and `MenuScreen.Locker`.
- Right Menu: Characters → Characters tab; Customize → Locker (Outfits tab).
- `CharacterMenuView` kept on Boot for GUID stability; UI now lives in `LockerView`.
- `CharacterDebugView` (panelX **10**, established Character slot): Character ID, Outfit ID, Animation State, Country ID, Temporary Timers. Next free panel slot after MainMenu's 4060 remains **4510** for future non-Character panels.

---

## 9. Scene & asset wiring

- `Boot.unity` `CharacterSystems` gained `LockerView` (country catalog + `LockerUiConfig` refs; audio clips unassigned).
- New `Settings/LockerUiConfig.asset`.
- Expanded `Settings/CosmeticCatalogConfig.asset` (rarity/index fields + Headwear/Glasses/Effects/Frames/Titles samples).

---

## 10. Remaining TODOs

1. **No final art/audio/3D showroom** — OnGUI placeholders only; dressing-room music/SFX clips unassigned.
2. **No real 3D character camera** — rotate/zoom approximated with flat silhouette scaling/facing.
3. **Store Visual Effects ledger vs new CosmeticSlots** — ProfileFrame/Title now exist as equip slots; migrating Store's generic `OwnedStoreItem` ledger onto these slots is a low-risk follow-up.
4. **Account/progress still mostly in-memory** — only Intro flag + Loadout snapshot use PlayerPrefs today.
5. Carries forward unresolved Sprint 1–14 items (no licensed Editor, no networked avatar art, etc.).

---

## 11. Build Verification / Compiler Status

- **Offline compile:** all **394** project `.cs` files recompiled via `dotnet build .compile_check/CompileCheck.csproj` against `.compile_check/Shims/UnityEngineShim.cs`. Shim extensions this sprint: `PlayerPrefs.GetString`/`SetString`, `Time.unscaledTime`, `Mathf.Deg2Rad`/`Cos`/`Exp`/`Sqrt`/`FloorToInt`/`RoundToInt`, int `Min`/`Max`, `Rect.xMax`/`yMax`, `GUIStyle.none`. **Result: Build succeeded, 0 errors, 0 warnings.**
- **YAML validation:** `Boot.unity`, `LockerUiConfig.asset`, `CosmeticCatalogConfig.asset` — OK. Scene guid false-positives for Unity built-in RenderSettings refs are the same expected residuals documented since Sprint 4.

---

## 12. Git Workflow

| Item | Value |
|---|---|
| Commit hash | `18c9477` (feature) / `cf458af` (report hash confirmation) |
| Commit message | `Sprint 16 - Character Selection, Locker & Customization` |
| Branch | `main` |
| Push status | Pushed to `origin/main` (`1e67310..cf458af`); `git rev-parse HEAD` matches `origin/main` at `cf458af` |

Sprint 16 is complete within the constraints above.
