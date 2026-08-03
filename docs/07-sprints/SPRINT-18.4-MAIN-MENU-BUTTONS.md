# Sprint 18.4 — Import Final Main Menu Buttons — Sprint Report

**Role:** Lead UI Systems Engineer  
**Scope:** Replace MainMenuCanvas button placeholders with production Btn_*.png artwork. Uniform size/spacing. No gameplay/script changes. No other UI modifications beyond button Images and parent container sizes.  
**Status:** Complete.

## 1. Artwork

| Button | Imported path | GUID |
|---|---|---|
| Play | `Assets/_Project/UI/MainMenu/Buttons/Btn_Play.png` | `a46e582c6490e5345a315a5f0ce7e9b1` |
| Lobby | `Assets/_Project/UI/MainMenu/Buttons/Btn_Lobby.png` | `e6d13a57442645e4f88e08b241b0dc60` |
| Friends | `Assets/_Project/UI/MainMenu/Buttons/Btn_Friends.png` | `a5052f77ff7897b43919cbebc130a4e0` |
| Clan | `Assets/_Project/UI/MainMenu/Buttons/Btn_Clan.png` | `6510d24417ce5e949b1ef2c04c364dff` |
| Missions | `Assets/_Project/UI/MainMenu/Buttons/Btn_Missions.png` | `abcbae0cc3394bb4098858fce5f9a9ec` |
| Store | `Assets/_Project/UI/MainMenu/Buttons/Btn_Store.png` | `30686e85f0e945c4ab360cb122535c9c` |
| Settings | `Assets/_Project/UI/MainMenu/Buttons/Btn_Settings.png` | `62974cc4a6c50674989d60cdb727b723` |
| Rankings | `Assets/_Project/UI/MainMenu/Buttons/Btn_Rankings.png` | `c711f3dd9a59a084abff1c61e2b19cc1` |

- Source: untracked `Client/Assets/Btn_*.png` (1672×941 each)
- Texture Type: Sprite (2D and UI), Single
- Root `Assets/Btn_*.png` duplicates removed after move

## 2. Scene mapping (`MainMenu.unity`)

| Hierarchy | Sprite |
|---|---|
| `TopLeft/PlayButtonImage` | Btn_Play |
| `LeftMenu/LobbyButtonImage` | Btn_Lobby |
| `LeftMenu/FriendsButtonImage` | Btn_Friends |
| `LeftMenu/ClanButtonImage` | Btn_Clan |
| `RightMenu/MissionsButtonImage` | Btn_Missions |
| `RightMenu/StoreButtonImage` | Btn_Store |
| `RightMenu/SettingsButtonImage` | Btn_Settings |
| `RightMenu/RankingsButtonImage` | Btn_Rankings |

- Color alpha → **1** (was 0 placeholder)
- `preserveAspect` = **true**
- `raycastTarget` remains **true**
- Parent anchors unchanged (TopLeft top-left, LeftMenu left-middle, RightMenu right-middle)

## 3. Uniform size & spacing

| Item | Value |
|---|---|
| Button size | **220×124** (native ~1.78 aspect) |
| Vertical center spacing | **148** |
| LeftMenu order | Lobby, Friends, Clan (y: 148 / 0 / −148) |
| RightMenu order | Missions, Store, Settings, Rankings (y: 222 / 74 / −74 / −222) |
| Play | TopLeft, same **220×124** size |
| LeftMenu SizeDelta | 240×420 |
| RightMenu SizeDelta | 240×568 |
| TopLeft SizeDelta | 240×140 |

## 4. Constraints honored

- No gameplay `.cs` changes
- No Background / Logo / Character / PlayerCard modifications
- `DefaultNetworkPrefabs.asset` left uncommitted
- No duplicate root `Assets/Btn_*.png` left behind

## 5. Verification

- Unity batchmode: all 8 button sprites assigned, preserveAspect, uniform 220×124, spacing 148, anchors preserved — PASS, 0 failures
- StandaloneWindows64 build: Succeeded, 0 errors, 11 pre-existing warnings

## 6. Git

| Item | Value |
|---|---|
| Branch | `main` |
| Commit | `(pending)` |
| Push | `origin/main` |
