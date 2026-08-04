# Sprint 19.0 — Final Main Menu Layout — Sprint Report

**Role:** Lead UI Systems Engineer  
**Scope:** Arrange imported Main Menu artwork on `MainMenu.unity` to match the approved GulfRun layout. RectTransforms only. No gameplay/script/button functionality changes.  
**Status:** Complete.

## 1. Layout (1920×1080 reference)

| Element | Anchors | Pivot | Position | Size |
|---|---|---|---|---|
| Background | stretch (0,0)–(1,1) | center | (0, 0) | (0, 0) |
| GulfRunLogo | top-center (0.5,1) | top-center | (0, −28) | **460×307** |
| CharacterImage | bottom-center (0.5,0) | bottom-center | (0, 20) | **860×573** |
| TopRight / PlayerCard | top-right (1,1) | top-right | (−40, −40) / (0, 0) | card **360×112** |
| TopLeft (Play) | left-middle (0,0.5) | left-middle | **(56, 222)** | 220×124 |
| LeftMenu | left-middle | left-middle | **(56, 0)** | 240×500 |
| RightMenu | right-middle | right-middle | **(−56, 0)** | 240×568 |

### Button columns (all **220×124**, center spacing **148**)

| Left (X=56) | Center Y | Right (X=−56) | Center Y |
|---|---|---|---|
| Play | 222 | Missions | 222 |
| Lobby | 74 | Store | 74 |
| Friends | −74 | Settings | −74 |
| Clan | −222 | Rankings | −222 |

Play remains under `TopLeft` but shares LeftMenu’s left-middle X and column spacing so both sides form mirrored vertical stacks.

## 2. Constraints honored

- No `.cs` gameplay/script changes
- No sprite GUID / Image source / Button onClick changes
- No new GameObjects
- Sibling draw order unchanged (Background → Character → Logo → menus)
- `DefaultNetworkPrefabs.asset` and root `Assets/Btn_*.png` left uncommitted

## 3. Verification

- Unity batchmode layout validation: PASS, **0 failures** (canvas 1920×1080 match 0.5 Overlay; stretch background; logo top-center; character bottom-center; player card top-right; equal button size/spacing)
- StandaloneWindows64 build: **Succeeded**, 0 errors, 11 pre-existing warnings

## 4. Git

| Item | Value |
|---|---|
| Branch | `main` |
| Commit | `321a99440b4d463e0abd12938f299b41dbd241d5` |
| Push | `origin/main` |
