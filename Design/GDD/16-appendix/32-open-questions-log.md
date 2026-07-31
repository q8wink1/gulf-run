# 32 — Open Questions Log

**GDD chapter:** 32  
**Status:** Living log  
**Maintained by:** Documentation engineer + Design Owner  
**Last updated:** 2026-07-31

---

## 32.1 Active questions

| ID | Chapter | Question | Asked on | Owner | Blocking? |
|----|---------|----------|----------|-------|-----------|
| Q-02-006 | 02 / P001 | Numeric match-length target within “a few minutes”? | 2026-07-31 | Design Owner | Fast Gameplay |
| Q-P002-001 | P002 | What appears on the Loading Screen? | 2026-07-31 | Design Owner | UI |
| Q-P002-002 | Race start countdown / start sequence? | 2026-07-31 | Design Owner | **Partial:** countdown exists (P010); length TODO |
| Q-P002-004 | P002 | Disconnect / DNF / forfeit behavior? | 2026-07-31 | Design Owner | **Partial:** Disconnect/AFK systems exist (P010); rules TBD |
| Q-P002-005 | Do Invite Friend / Private Room alter Matchmaking vs Quick Match? | 2026-07-31 | Design Owner | **Partial:** P017 types; Private Room details **P018**; Friend Party TODO |
| Q-P002-006 | P002 | Detailed touch controls for Run / Jump / items? | 2026-07-31 | Design Owner | Controls |
| Q-03-002 | 03 | Detailed touch control scheme for side-scrolling racing? | 2026-07-31 | Design Owner | Controls |
| Q-03-003 | 03 | Tablet support at launch? | 2026-07-31 | Design Owner | No |
| Q-03-004 | 03 | Store age rating target? | 2026-07-31 | Design Owner | Before Soft Launch |
| Q-00-001 | 00 | Player-facing title? | 2026-07-31 | Design Owner | No |
| Q-P003-001 | P003 | Exact Double Jump rules? | 2026-07-31 | Design Owner | Controls |
| Q-P003-002 | P003 | Touch layout for Jump / Double Jump / Use item? | 2026-07-31 | Design Owner | Controls |
| Q-P003-003 | P003 | Holding an item + another item box — replace/block/other? | 2026-07-31 | Design Owner | Items |
| Q-P003-004 | Finish-line tie-break? | 2026-07-31 | Design Owner | **Open — also Q-P010-003** |
| Q-P003-006 | P003 | Which document defines item types? | 2026-07-31 | Design Owner | Items |
| Q-P003-007 | P003 | Run speed / lane or vertical movement? | 2026-07-31 | Design Owner | Movement |
| Q-P004-001 | P004 | Login screen specification? | 2026-07-31 | Design Owner | Before full FTUE |
| Q-P004-002 | P004 | Main Menu button layout arrangement? | 2026-07-31 | Design Owner | UI |
| Q-P004-003 | P004 | Back from Play screen to Main Menu? | 2026-07-31 | Design Owner | Navigation |
| Q-P004-004 | P004 | Private Room code format / max players? | 2026-07-31 | Design Owner | **Partial:** max **4** (P018); code format TODO |
| Q-P004-005 | P004 | Doc IDs for Friends/Clans/Shop/Challenges/Settings? | 2026-07-31 | Design Owner | **Partial:** Shop=P013; Friends=P014; Clans=P015; Daily=P026; Weekly=P027; Settings open |
| Q-P004-006 | P004 | Are Profile fields read-only on Profile screen? | 2026-07-31 | Design Owner | **Partial (P020):** stats not editable; cosmetic customize; name edit TODO |
| Q-P005-001 | P005 | Official names for Character 01 / 02? | 2026-07-31 | Design Owner | Content |
| Q-P005-002 | P005 | Are Character 01 / 02 owned at account creation? | 2026-07-31 | Design Owner | **Partial (P022):** default cosmetics granted |
| Q-P005-003 | P005 | Where in P002 flow is character select? | 2026-07-31 | Design Owner | Loop |
| Q-P005-004 | P005 | Cosmetics per-character or account-wide? | 2026-07-31 | Design Owner | Still open — equip slots P021 |
| Q-P005-005 | P005 | Doc ID for unlock/acquisition spec? | 2026-07-31 | Design Owner | Queue |
| Q-P005-006 | P005 | Profile Selected Character vs active race character? | 2026-07-31 | Design Owner | P004 sync |
| Q-P006-001 | P006 | Numeric shared race distance? | 2026-07-31 | Design Owner | Maps |
| Q-P006-002 | P006 | All six maps at Soft Launch / Global? | 2026-07-31 | Design Owner | Content |
| Q-P006-003 | P006 | Random: uniform/weighted? Repeat protection? | 2026-07-31 | Design Owner | Rotation |
| Q-P006-004 | P006 | When is selected map shown to players? | 2026-07-31 | Design Owner | UX |
| Q-P006-005 | P006 | Which modes override random map select? | 2026-07-31 | Design Owner | Modes |
| Q-P007-001 | P007 | Which doc defines collision effects / damage / recovery / respawn? | 2026-07-31 | Design Owner | Combat/recovery |
| Q-P007-002 | P007 | Which items may interact with obstacles? | 2026-07-31 | Design Owner | Items |
| Q-P007-003 | P007 | Obstacle density / count per map? | 2026-07-31 | Design Owner | Maps |
| Q-P007-004 | P007 | Which modes change obstacle positions? | 2026-07-31 | Design Owner | Modes |
| Q-P008-001 | P008 | Collect box while already holding an item? | 2026-07-31 | Design Owner | Items |
| Q-P008-003 | P008 | Item activation method / touch mapping? | 2026-07-31 | Design Owner | Controls |
| Q-P008-004 | P008 | Do boxes respawn after collection in a race? | 2026-07-31 | Design Owner | Items |
| Q-P008-005 | P008 | Same box layout for all players in a match? | 2026-07-31 | Design Owner | Fairness |
| Q-P009-001 | P009 | Which doc lists specific Items and Weapons? | 2026-07-31 | Design Owner | Catalog |
| Q-P009-002 | P009 | Item activation input method? | 2026-07-31 | Design Owner | Controls |
| Q-P009-003 | P009 | Non-weapon Item categories besides Weapons? | 2026-07-31 | Design Owner | Items |
| Q-P009-004 | P009 | Collect-while-holding (P008) vs P009 one-item rules? | 2026-07-31 | Design Owner | Items |
| Q-P010-001 | P010 | Countdown length / display? | 2026-07-31 | Design Owner | Race start |
| Q-P010-002 | P010 | Race end if not all finish (until future rule)? | 2026-07-31 | Design Owner | Race end |
| Q-P010-003 | P010 | Finish-line tie-break? | 2026-07-31 | Design Owner | Ranking |
| Q-P010-004 | P010 | Doc IDs for Disconnection / AFK rules? | 2026-07-31 | Design Owner | Queue |
| Q-P010-005 | P010 | Equal starting position definition? | 2026-07-31 | Design Owner | Fairness |
| Q-P011-001 | P011 | What does Continue do on Results Screen? | 2026-07-31 | Design Owner | Navigation |
| Q-P011-002 | P011 | Race Time format? | 2026-07-31 | Design Owner | Results |
| Q-P011-003 | P011 | Doc ID for Reward system spec? | 2026-07-31 | Design Owner | Queue |
| Q-P011-004 | P011 | Results Screen layout? | 2026-07-31 | Design Owner | UI |
| Q-P011-005 | P011 | Rewards placeholder on Results vs separate P002 Stage 11? | 2026-07-31 | Design Owner | Loop |
| Q-P012-001 | P012 | Which gameplay grants Coins (amounts where)? | 2026-07-31 | Design Owner | Economy |
| Q-P012-003 | P012 | Where are wallets shown in UI? | 2026-07-31 | Design Owner | UI |
| Q-P012-004 | P012 | Non-purchase Gem acquisition later? | 2026-07-31 | Design Owner | Economy |
| Q-P012-005 | P012 | Refund rules document? | 2026-07-31 | Design Owner | Store/IAP |
| Q-P013-001 | P013 | Prices and Coins vs Gems per item? | 2026-07-31 | Design Owner | Shop |
| Q-P013-002 | P013 | Duplicate ownership on repurchase? | 2026-07-31 | Design Owner | Still open — also P021 |
| Q-P013-003 | P013 | Coin/Gem packs = real-money IAP only? | 2026-07-31 | Design Owner | IAP |
| Q-P013-004 | P013 | Shop UI layout / tabs? | 2026-07-31 | Design Owner | UI |
| Q-P013-005 | P013 | Events doc for Shop availability? | 2026-07-31 | Design Owner | LiveOps |
| Q-P013-006 | P013 | Refund rules document? | 2026-07-31 | Design Owner | Store |
| Q-P014-001 | P014 | Maximum friends (Friend Limit)? | 2026-07-31 | Design Owner | TODO |
| Q-P014-002 | P014 | Player ID format? | 2026-07-31 | Design Owner | Identity |
| Q-P014-003 | P014 | Doc IDs for Search/QR/Contact Sync/Report/Block? | 2026-07-31 | Design Owner | Queue |
| Q-P014-004 | P014 | Invite when Offline or In Match? | 2026-07-31 | Design Owner | Invites |
| Q-P014-005 | P014 | View Profile vs P004 Profile? | 2026-07-31 | Design Owner | **Partial:** SoT **P020**; hub vs full TODO |
| Q-P014-006 | P014 | Cross-platform friendship rules? | 2026-07-31 | Design Owner | Platform |
| Q-P015-001 | P015 | Maximum clan member count? | 2026-07-31 | Design Owner | TODO |
| Q-P015-002 | P015 | Full Leader vs Co-Leader permissions? | 2026-07-31 | Design Owner | Security |
| Q-P015-003 | P015 | Decline vs Ignore invitations? | 2026-07-31 | Design Owner | Invites |
| Q-P015-004 | P015 | Must clan invitees be Friends? | 2026-07-31 | Design Owner | P014 |
| Q-P015-005 | P015 | Clan Name / Tag validation? | 2026-07-31 | Design Owner | Content |
| Q-P015-007 | P015 | Clan Text Chat moderation? | 2026-07-31 | Design Owner | Chat |
| Q-P016-001 | P016 | Exact session types enabling Voice Chat? | 2026-07-31 | Design Owner | Channels |
| Q-P016-002 | P016 | Party definition for Party Voice Chat? | 2026-07-31 | Design Owner | Party |
| Q-P016-003 | P016 | Voice Settings host screen (Settings spec)? | 2026-07-31 | Design Owner | P004 |
| Q-P016-004 | P016 | Connection Status values? | 2026-07-31 | Design Owner | UI |
| Q-P016-005 | P016 | Report Voice Abuse / moderation doc ID? | 2026-07-31 | Design Owner | Safety |
| Q-P016-006 | P016 | Age restrictions for voice? | 2026-07-31 | Design Owner | Compliance |
| Q-P017-001 | P017 | Friend Party = P004 Invite Friend? | 2026-07-31 | Design Owner | Naming |
| Q-P017-002 | P017 | What is match “confirmed”? | 2026-07-31 | Design Owner | Cancel |
| Q-P017-003 | P017 | Cancel after confirmation rules doc? | 2026-07-31 | Design Owner | Queue |
| Q-P017-004 | P017 | Search state transition details? | 2026-07-31 | Design Owner | UX |
| Q-P017-005 | P017 | Ranked / Event Match doc IDs? | 2026-07-31 | Design Owner | Queue |
| Q-P017-006 | P017 | Cross-platform matching? | 2026-07-31 | Design Owner | Platform |
| Q-P018-001 | P018 | Min players to start Private Room? | 2026-07-31 | Design Owner | Start rules |
| Q-P018-002 | P018 | Room Code format / length / expiry? | 2026-07-31 | Design Owner | UX |
| Q-P018-003 | P018 | All Ready required before Start Match? | 2026-07-31 | Design Owner | Ready |
| Q-P018-004 | P018 | Post-race: Waiting For Players or Closed? | 2026-07-31 | Design Owner | Loop |
| Q-P018-005 | P018 | Host leave without Transfer Host? | 2026-07-31 | Design Owner | Host |
| Q-P018-006 | P018 | Friend Party vs Private Room vs Invite Friend? | 2026-07-31 | Design Owner | Naming |
| Q-P018-007 | P018 | Voice Chat Behavior for Private Rooms doc? | 2026-07-31 | Design Owner | Voice |
| Q-P019-001 | P019 | Leaderboard entry UI (Main Menu)? | 2026-07-31 | Design Owner | UX |
| Q-P019-002 | P019 | Regional scope without region filters? | 2026-07-31 | Design Owner | Regions |
| Q-P019-003 | P019 | Ranking formula / system doc ID? | 2026-07-31 | Design Owner | Ranking |
| Q-P019-004 | P019 | Season reset / Historical Seasons spec? | 2026-07-31 | Design Owner | Seasons |
| Q-P019-005 | P019 | Leaderboard reward distribution? | 2026-07-31 | Design Owner | Rewards |
| Q-P019-006 | P019 | Refresh frequency policy? | 2026-07-31 | Design Owner | Sync |
| Q-P019-007 | P019 | Search Player future UX? | 2026-07-31 | Design Owner | Future |
| Q-P020-001 | P020 | Main Menu Profile = full P020 screen? | 2026-07-31 | Design Owner | UX |
| Q-P020-002 | P020 | Edit Profile: name vs cosmetics only? | 2026-07-31 | Design Owner | Edit |
| Q-P020-003 | P020 | Win/Loss/Win Rate for 4p races? | 2026-07-31 | Design Owner | Stats |
| Q-P020-004 | P020 | Player ID format? | 2026-07-31 | Design Owner | ID |
| Q-P020-005 | P020 | Avatar source (Shop / upload)? | 2026-07-31 | Design Owner | Cosmetics |
| Q-P020-006 | P020 | Privacy settings future doc ID? | 2026-07-31 | Design Owner | Privacy |
| Q-P020-007 | P020 | Online Status = P014 enum? | 2026-07-31 | Design Owner | Presence |
| Q-P021-001 | P021 | Inventory UI entry point(s)? | 2026-07-31 | Design Owner | UX |
| Q-P021-002 | P021 | Supported equip slots list? | 2026-07-31 | Design Owner | Equip |
| Q-P021-003 | P021 | Duplicate ownership rules? | 2026-07-31 | Design Owner | Ownership |
| Q-P021-004 | P021 | P005 Animations vs Inventory categories? | 2026-07-31 | Design Owner | Categories |
| Q-P021-005 | P021 | Profile Cosmetics ↔ P020 mapping? | 2026-07-31 | Design Owner | Profile |
| Q-P021-006 | P021 | Character equip vs Selected Character? | 2026-07-31 | Design Owner | Character |
| Q-P021-007 | P021 | Capacity / Collection Progress doc? | 2026-07-31 | Design Owner | Future |
| Q-P022-001 | P022 | Supported cosmetic slot list? | 2026-07-31 | Design Owner | Equip |
| Q-P022-002 | P022 | Active race change lockout definition? | 2026-07-31 | Design Owner | Timing |
| Q-P022-003 | P022 | Rarity levels / system doc ID? | 2026-07-31 | Design Owner | Rarity |
| Q-P022-004 | P022 | P005 Animations vs P022 categories? | 2026-07-31 | Design Owner | Categories |
| Q-P022-005 | P022 | Profile Avatar/Frame vs P021/P020? | 2026-07-31 | Design Owner | Profile |
| Q-P022-006 | P022 | Default Profile Avatar/Frame? | 2026-07-31 | Design Owner | Defaults |
| Q-P022-007 | P022 | Cosmetics UI vs Inventory UI? | 2026-07-31 | Design Owner | UX |
| Q-P023-001 | P023 | XP/Level/Rank formula doc IDs? | 2026-07-31 | Design Owner | **Partial:** Level/XP **P024**; formulas TODO |
| Q-P023-002 | P023 | Which gameplay grants which progress? | 2026-07-31 | Design Owner | Sources |
| Q-P023-003 | P023 | Profile Rank vs competitive ranking? | 2026-07-31 | Design Owner | **Partial:** = Competitive Rank **P025**; LB integration TBD |
| Q-P023-004 | P023 | Season Progress vs P019 / resets? | 2026-07-31 | Design Owner | **Partial:** Battle Pass **P029**; relationship TODO |
| Q-P023-005 | P023 | Achievements catalog / display? | 2026-07-31 | Design Owner | **Partial:** system **P028**; list/Profile TBD |
| Q-P023-006 | P023 | Show Current XP on Profile? | 2026-07-31 | Design Owner | **Partial:** XP display field **P024**; Profile placement TODO |
| Q-P023-007 | P023 | Permanent progress vs seasons? | 2026-07-31 | Design Owner | Permanence |
| Q-P024-001 | P024 | Maximum Level value / soft cap? | 2026-07-31 | Design Owner | Cap |
| Q-P024-002 | P024 | XP Formula / per-level thresholds? | 2026-07-31 | Design Owner | Formula |
| Q-P024-003 | P024 | XP Sources (gameplay actions)? | 2026-07-31 | Design Owner | Sources |
| Q-P024-004 | P024 | Level Rewards document ID? | 2026-07-31 | Design Owner | Rewards |
| Q-P024-005 | P024 | Level Display screen placement? | 2026-07-31 | Design Owner | UX |
| Q-P024-006 | P024 | Level Up notification timing? | 2026-07-31 | Design Owner | UX |
| Q-P024-007 | P024 | Prestige / milestones future? | 2026-07-31 | Design Owner | Future |
| Q-P025-001 | P025 | Official rank names / Maximum Rank? | 2026-07-31 | Design Owner | Structure |
| Q-P025-002 | P025 | Which modes are competitive races? | 2026-07-31 | Design Owner | Modes |
| Q-P025-003 | P025 | Rank Formula / MMR / promo-demo rules? | 2026-07-31 | Design Owner | Formula |
| Q-P025-004 | P025 | Season Reset rules? | 2026-07-31 | Design Owner | Seasons |
| Q-P025-005 | P025 | Leaderboard Integration? | 2026-07-31 | Design Owner | P019 |
| Q-P025-006 | P025 | Placement Matches? | 2026-07-31 | Design Owner | Placement |
| Q-P025-007 | P025 | Rank Rewards document ID? | 2026-07-31 | Design Owner | Rewards |
| Q-P025-008 | P025 | Current Rank vs Current Season Rank? | 2026-07-31 | Design Owner | Display |
| Q-P026-001 | P026 | Challenge objectives / Challenge List? | 2026-07-31 | Design Owner | Objectives |
| Q-P026-002 | P026 | Reward Types? | 2026-07-31 | Design Owner | Rewards |
| Q-P026-003 | P026 | Unclaimed rewards at reset? | 2026-07-31 | Design Owner | Reset |
| Q-P026-004 | P026 | Reset timezone / clock? | 2026-07-31 | Design Owner | Sync |
| Q-P026-005 | P026 | Challenges button = Daily only? | 2026-07-31 | Design Owner | **Partial:** Weekly **P027**; hub TODO |
| Q-P026-006 | P026 | Premium / Bonus / Skip future? | 2026-07-31 | Design Owner | Future |
| Q-P026-007 | P026 | How many Daily Challenges per day? | 2026-07-31 | Design Owner | Count |
| Q-P027-001 | P027 | Weekly objectives / Challenge List? | 2026-07-31 | Design Owner | Objectives |
| Q-P027-002 | P027 | Reward Types? | 2026-07-31 | Design Owner | Rewards |
| Q-P027-003 | P027 | Unclaimed rewards at weekly reset? | 2026-07-31 | Design Owner | Reset |
| Q-P027-004 | P027 | Exact weekly reset day/time? | 2026-07-31 | Design Owner | Sync |
| Q-P027-005 | P027 | Challenges hub Daily+Weekly layout? | 2026-07-31 | Design Owner | UX |
| Q-P027-006 | P027 | Premium / Bonus / Chains future? | 2026-07-31 | Design Owner | Future |
| Q-P027-007 | P027 | How many Weekly Challenges per week? | 2026-07-31 | Design Owner | Count |
| Q-P027-008 | P027 | Named Challenge Categories list? | 2026-07-31 | Design Owner | Categories |
| Q-P028-001 | P028 | Achievement List / objectives? | 2026-07-31 | Design Owner | List |
| Q-P028-002 | P028 | Named Achievement Categories? | 2026-07-31 | Design Owner | Categories |
| Q-P028-003 | P028 | Reward Types? | 2026-07-31 | Design Owner | Rewards |
| Q-P028-004 | P028 | View Achievements entry (Profile)? | 2026-07-31 | Design Owner | UX |
| Q-P028-005 | P028 | Hidden / Secret Achievements? | 2026-07-31 | Design Owner | Future |
| Q-P028-006 | P028 | Achievement Points / Rarity? | 2026-07-31 | Design Owner | Future |
| Q-P028-007 | P028 | Completion Date display when future? | 2026-07-31 | Design Owner | Display |
| Q-P029-001 | P029 | Tier Count and Reward List? | 2026-07-31 | Design Owner | Structure |
| Q-P029-002 | P029 | Progress Formula / BP Progress sources? | 2026-07-31 | Design Owner | Formula |
| Q-P029-003 | P029 | Premium Price / purchase currency? | 2026-07-31 | Design Owner | Monetization |
| Q-P029-004 | P029 | Season Duration / remaining time clock? | 2026-07-31 | Design Owner | Seasons |
| Q-P029-005 | P029 | Previous Season / Expired Rewards? | 2026-07-31 | Design Owner | Seasons |
| Q-P029-006 | P029 | BP Progress vs P023 Season Progress? | 2026-07-31 | Design Owner | Progress |
| Q-P029-007 | P029 | Premium Plus / Instant Tier Unlocks? | 2026-07-31 | Design Owner | Future |
| Q-P029-008 | P029 | Battle Pass entry UI location? | 2026-07-31 | Design Owner | UX |
| Q-P030-001 | P030 | Season Duration / remaining-time clock? | 2026-07-31 | Design Owner | Duration |
| Q-P030-002 | P030 | Season Names / Themes / identity? | 2026-07-31 | Design Owner | Identity |
| Q-P030-003 | P030 | Season Progress calculation / sources? | 2026-07-31 | Design Owner | Progress |
| Q-P030-004 | P030 | Previous Season / Archive / Access? | 2026-07-31 | Design Owner | Archive |
| Q-P030-005 | P030 | Season Challenges vs Daily/Weekly? | 2026-07-31 | Design Owner | Challenges |
| Q-P030-006 | P030 | Season Rewards vs BP / Rank rewards? | 2026-07-31 | Design Owner | Rewards |
| Q-P030-007 | P030 | Season Reset vs P025 Rank reset? | 2026-07-31 | Design Owner | Reset |
| Q-P030-008 | P030 | Season Intro — future or never? | 2026-07-31 | Design Owner | Future |
| Q-P031-001 | P031 | Minimum requirements for gated events? | 2026-07-31 | Design Owner | Eligibility |
| Q-P031-002 | P031 | Event Rewards / Limited Rewards catalog? | 2026-07-31 | Design Owner | Rewards |
| Q-P031-003 | P031 | Missions vs Challenges vs Special Missions? | 2026-07-31 | Design Owner | Content |
| Q-P031-004 | P031 | Event Shop / Currency — future or never? | 2026-07-31 | Design Owner | Economy |
| Q-P031-005 | P031 | Event Leaderboards vs P019 future type? | 2026-07-31 | Design Owner | Leaderboards |
| Q-P031-006 | P031 | Event Status enum values? | 2026-07-31 | Design Owner | Status |
| Q-P031-007 | P031 | View Event UI entry point? | 2026-07-31 | Design Owner | UX |
| Q-P031-008 | P031 | Season Events vs P030 calendar? | 2026-07-31 | Design Owner | Seasons |
| Q-P032-001 | P032 | Notification inbox UI entry? | 2026-07-31 | Design Owner | UX |
| Q-P032-002 | P032 | View auto-marks Read vs Mark as Read? | 2026-07-31 | Design Owner | Read |
| Q-P032-003 | P032 | Push categories vs notification types? | 2026-07-31 | Design Owner | Push |
| Q-P032-004 | P032 | Deep Linking Behavior? | 2026-07-31 | Design Owner | Navigation |
| Q-P032-005 | P032 | Expiration / Priority future? | 2026-07-31 | Design Owner | Future |
| Q-P032-006 | P032 | Settings doc for push prefs? | 2026-07-31 | Design Owner | Settings |
| Q-P032-007 | P032 | Which system events emit notifications? | 2026-07-31 | Design Owner | Sources |
| Q-P033-001 | P033 | Attachment Types catalog? | 2026-07-31 | Design Owner | Attachments |
| Q-P033-002 | P033 | Expiration Duration / TTL policy? | 2026-07-31 | Design Owner | Expiry |
| Q-P033-003 | P033 | Unclaimed attachments when mail expires? | 2026-07-31 | Design Owner | Expiry |
| Q-P033-004 | P033 | Inbox UI entry point? | 2026-07-31 | Design Owner | UX |
| Q-P033-005 | P033 | Relationship to P032 for new mail? | 2026-07-31 | Design Owner | Notifications |
| Q-P033-006 | P033 | Claim All scope? | 2026-07-31 | Design Owner | Claim |
| Q-P033-007 | P033 | Search / Filters / Archive — future or never? | 2026-07-31 | Design Owner | Future |
| Q-P033-008 | P033 | Gift Mail — future or never? | 2026-07-31 | Design Owner | Future |
| Q-P034-001 | P034 | General category specific options? | 2026-07-31 | Design Owner | General |
| Q-P034-002 | P034 | Supported Languages list? | 2026-07-31 | Design Owner | Language |
| Q-P034-003 | P034 | About category contents? | 2026-07-31 | Design Owner | About |
| Q-P034-004 | P034 | Which settings are account-scoped vs device-scoped? | 2026-07-31 | Design Owner | Sync |
| Q-P034-005 | P034 | Privacy Options contents? | 2026-07-31 | Design Owner | Privacy |
| Q-P034-006 | P034 | Block List vs P014 future Block Player relationship? | 2026-07-31 | Design Owner | Privacy |
| Q-P034-007 | P034 | Voice Chat Settings (P016) wiring into Settings? | 2026-07-31 | Design Owner | Audio |
| Q-P034-008 | P034 | Notification prefs (P032) wiring into Settings? | 2026-07-31 | Design Owner | Notifications |
| Q-P034-009 | P034 | Graphics Presets / Accessibility / Parental Controls / Developer Options — future or never? | 2026-07-31 | Design Owner | Future |
| Q-P035-001 | P035 | Specific sound design per Gameplay moment? | 2026-07-31 | Design Owner | Gameplay |
| Q-P035-002 | P035 | Per-map Environment sound lists? | 2026-07-31 | Design Owner | Environment |
| Q-P035-003 | P035 | Weapon audio details — which future specification? | 2026-07-31 | Design Owner | Weapons |
| Q-P035-004 | P035 | Music track list and playback contexts? | 2026-07-31 | Design Owner | Music |
| Q-P035-005 | P035 | Ambience sound list and contexts? | 2026-07-31 | Design Owner | Ambience |
| Q-P035-006 | P035 | "Optimized for mobile" concrete targets? | 2026-07-31 | Design Owner | Performance |
| Q-P035-007 | P035 | "Must not interrupt gameplay" concurrency rules? | 2026-07-31 | Design Owner | Rules |
| Q-P035-008 | P035 | Audio settings sync scope — account vs device? | 2026-07-31 | Design Owner | Sync |
| Q-P035-009 | P035 | P016 voice volume vs P035 Voice Chat Volume relationship? | 2026-07-31 | Design Owner | Voice |
| Q-P035-010 | P035 | Compression / 3D Audio / Priorities / Streaming / Localization Voices / Dynamic Music / Accessibility Audio — future or never? | 2026-07-31 | Design Owner | Future |
| Q-P036-001 | P036 | Actual music tracks per category? | 2026-07-31 | Design Owner | Tracks |
| Q-P036-002 | P036 | Lobby / Loading Screen / Shop music behavior details? | 2026-07-31 | Design Owner | Categories |
| Q-P036-003 | P036 | Transition behavior between music categories? | 2026-07-31 | Design Owner | Flow |
| Q-P036-004 | P036 | Map-to-music-theme mapping (P006 six maps)? | 2026-07-31 | Design Owner | Race |
| Q-P036-005 | P036 | Is Mute Music distinct from Music Volume = 0? | 2026-07-31 | Design Owner | Controls |
| Q-P036-006 | P036 | "Loop seamlessly" technical requirements? | 2026-07-31 | Design Owner | Rules |
| Q-P036-007 | P036 | "Optimized for mobile" concrete targets? | 2026-07-31 | Design Owner | Performance |
| Q-P036-008 | P036 | Music Volume vs Master Volume precedence? | 2026-07-31 | Design Owner | Audio |
| Q-P036-009 | P036 | Adaptive Music / Dynamic Layering / Regional Variations / Licensed Music / Streaming Rules — future or never? | 2026-07-31 | Design Owner | Future |
| Q-P037-001 | P037 | Which future languages and timeline? | 2026-07-31 | Design Owner | Languages |
| Q-P037-002 | P037 | Regional dialect handling? | 2026-07-31 | Design Owner | Languages |
| Q-P037-003 | P037 | "Player Messages" scope — chat, mail, or other? | 2026-07-31 | Design Owner | Content |
| Q-P037-004 | P037 | RTL behavior for mixed Arabic/English/numeric content? | 2026-07-31 | Design Owner | RTL |
| Q-P037-005 | P037 | Specific font families / fallback chains? | 2026-07-31 | Design Owner | Fonts |
| Q-P037-006 | P037 | Preview Language exact behavior? | 2026-07-31 | Design Owner | Controls |
| Q-P037-007 | P037 | Is Change Language account-linked or device-local? | 2026-07-31 | Design Owner | Sync |
| Q-P037-008 | P037 | Fallback chain if English key itself is missing? | 2026-07-31 | Design Owner | Rules |
| Q-P037-009 | P037 | Voice Localization languages and timeline? | 2026-07-31 | Design Owner | Voice |
| Q-P037-010 | P037 | Localized Images/Audio/Videos, Machine/Community Translation — future or never? | 2026-07-31 | Design Owner | Future |
| Q-P038-001 | P038 | Exact tutorial map / environment? | 2026-07-31 | Design Owner | Flow |
| Q-P038-002 | P038 | Step-level pass/fail or retry conditions? | 2026-07-31 | Design Owner | Flow |
| Q-P038-003 | P038 | Skip Tutorial granularity — whole vs per-step? | 2026-07-31 | Design Owner | Actions |
| Q-P038-004 | P038 | Difference between Skip Tutorial and Exit Tutorial? | 2026-07-31 | Design Owner | Actions |
| Q-P038-005 | P038 | "New players" definition? | 2026-07-31 | Design Owner | Rules |
| Q-P038-006 | P038 | Tutorial progress save — account-linked or device-local? | 2026-07-31 | Design Owner | Sync |
| Q-P038-007 | P038 | Concrete "fast" duration / mobile performance targets? | 2026-07-31 | Design Owner | Performance |
| Q-P038-008 | P038 | Rewards / Advanced Tutorial / Character Voices / Interactive Tips / Practice Mode / Performance Evaluation / Adaptive Tutorial — future or never? | 2026-07-31 | Design Owner | Future |
| Q-P039-001 | P039 | Cloud Save scope vs. Inventory/Profile persistence? (see docs/02-architecture/BACKEND_ARCHITECTURE.md) | 2026-07-31 | Tech Director | Backend |
| Q-P039-002 | P039 | Analytics event ownership boundary? | 2026-07-31 | Tech Director | Backend |
| Q-P039-003 | P039 | Concrete scalability / fault-tolerance SLOs? | 2026-07-31 | Tech Director | Backend |
| Q-P039-004 | P039 | Which gameplay actions use client-side prediction? | 2026-07-31 | Tech Director | Backend |
| Q-P039-005 | P039 | Reconnect grace window / timeout values? | 2026-07-31 | Tech Director | Backend |
| Q-P039-006 | P039 | Conflict resolution algorithm/strategy? | 2026-07-31 | Tech Director | Backend |
| Q-P039-007 | P039 | Cloud Provider / DB / Language / Region / Microservices / Caching / Queue / Monitoring / DR — ADR timeline? | 2026-07-31 | Tech Director | Backend |
| Q-P040-001 | P040 | Schema-level fields per data category? (see docs/02-architecture/DATABASE_ARCHITECTURE.md) | 2026-07-31 | Tech Director | Database |
| Q-P040-002 | P040 | Are Season / Live Events data distinct categories? | 2026-07-31 | Tech Director | Database |
| Q-P040-003 | P040 | Concrete consistency model per data category? | 2026-07-31 | Tech Director | Database |
| Q-P040-004 | P040 | Ownership boundary for shared systems (e.g., Clan data)? | 2026-07-31 | Tech Director | Database |
| Q-P040-005 | P040 | Offline synchronization behavior? | 2026-07-31 | Tech Director | Database |
| Q-P040-006 | P040 | "Sensitive information" scope definition? | 2026-07-31 | Tech Director | Database |
| Q-P040-007 | P040 | Database Engine / Sharding / Replication / Backup / Retention / Encryption / Indexes / Migration — ADR timeline? | 2026-07-31 | Tech Director | Database |
| Q-P041-001 | P041 | Login screen UI/UX? (see docs/02-architecture/AUTHENTICATION_SYSTEM.md) | 2026-07-31 | Tech Director | Auth |
| Q-P041-002 | P041 | Behavior on Load Player Profile / Load Cloud Data failure? | 2026-07-31 | Tech Director | Auth |
| Q-P041-003 | P041 | Guest-to-permanent-account upgrade path? | 2026-07-31 | Tech Director | Auth |
| Q-P041-004 | P041 | Linking / unlinking flow and conflict handling? | 2026-07-31 | Tech Director | Auth |
| Q-P041-005 | P041 | Behavior on concurrent second login? | 2026-07-31 | Tech Director | Auth |
| Q-P041-006 | P041 | Authentication Provider / Token Lifetime / Session Recovery / Multi-device / Account Recovery / 2FA / Parental Accounts — ADR timeline? | 2026-07-31 | Tech Director | Auth |
| Q-P042-001 | P042 | **[CONFLICT]** Which document (P020, P042, or a merged successor) is authoritative for Player Profile? | 2026-07-31 | Design Owner | **Escalated — blocking** |
| Q-P042-002 | P042 | Is "Current Character" the same field as P020's "Selected Character"? | 2026-07-31 | Design Owner | Profile |
| Q-P042-003 | P042 | Is "Experience" here the same value as P023 Progression XP? | 2026-07-31 | Design Owner | Profile |
| Q-P042-004 | P042 | Is "Profile Background" the same element as P020's "Profile Banner"? | 2026-07-31 | Design Owner | Profile |
| Q-P042-005 | P042 | Should Display Name be editable given P020's cosmetic-only customization rule? | 2026-07-31 | Design Owner | **Escalated — blocking** |
| Q-P042-006 | P042 | Do Losses / First Place Finishes (P020) also apply here? | 2026-07-31 | Design Owner | Profile |
| Q-P043-001 | P043 | Concrete false-positive rate target? (see docs/05-security/ANTI_CHEAT_SPECIFICATION.md) | 2026-07-31 | Security Lead | Anti-Cheat |
| Q-P043-002 | P043 | Report workflow and entry point? | 2026-07-31 | Security Lead | Anti-Cheat |
| Q-P043-003 | P043 | Relationship between Player Reports and Automatic Moderation? | 2026-07-31 | Security Lead | Anti-Cheat |
| Q-P043-004 | P043 | Detection Algorithms / Penalty Types / Ban System / Appeal Process / Hardware Detection / ML Detection / Replay Review / Automatic Moderation — timeline? | 2026-07-31 | Security Lead | Anti-Cheat |
| Q-P044-001 | P044 | Session/Store/Battle Pass/Challenge Analytics specific tracked fields? (see docs/02-architecture/ANALYTICS_SYSTEM.md) | 2026-07-31 | Tech Director | Analytics |
| Q-P044-002 | P044 | Retention definition (D1/D7/D30 or other)? | 2026-07-31 | Tech Director | Analytics |
| Q-P044-003 | P044 | Are Gameplay Wins/Losses same counters as P020/P042 profile stats? | 2026-07-31 | Tech Director | Analytics |
| Q-P044-004 | P044 | "Sensitive personal information" scope for analytics? | 2026-07-31 | Tech Director | Analytics |
| Q-P044-005 | P044 | Analytics Provider / Retention / Sampling / Heatmaps / Dashboards / A-B Testing / Funnels / Predictive Analytics — ADR timeline? | 2026-07-31 | Tech Director | Analytics |
| Q-P045-001 | P045 | Limited Time Cosmetic Bundles — contents, cadence, pricing? | 2026-07-31 | Design Owner | Monetization |
| Q-P045-002 | P045 | Limited Offer rules (trigger, duration, catalog)? | 2026-07-31 | Design Owner | Monetization |
| Q-P045-003 | P045 | Purchase restoration platform-specific implementation? | 2026-07-31 | Tech Director | Monetization |
| Q-P045-004 | P045 | Prices/Bundles/Discounts/Subscription/Starter Packs/Welcome Offers/Regional Pricing/Taxes/Refund Policy — timeline? | 2026-07-31 | Design Owner | Monetization |
| Q-P045-005 | P045 | Future Monetization Features scope? | 2026-07-31 | Design Owner | Monetization |
| Q-26-002 | 26 | Are ads allowed in any surface? | 2026-07-31 | Design Owner | Monetization |
| Q-P046-001 | P046 | Exact loading time targets? | 2026-07-31 | Tech Director | Performance |
| Q-P046-002 | P046 | Graphic quality level definitions (names / count)? | 2026-07-31 | Tech Director | Performance |
| Q-P046-003 | P046 | Memory budget numbers per device tier? | 2026-07-31 | Tech Director | Performance |
| Q-P046-004 | P046 | Network compression / bandwidth optimization strategy? | 2026-07-31 | Tech Director | Performance |
| Q-P046-005 | P046 | Asset streaming strategy? | 2026-07-31 | Tech Director | Performance |
| Q-P046-006 | P046 | Device Support Matrix / Texture Compression / LOD Strategy / Shader Variants — timeline? | 2026-07-31 | Tech Director | Performance |
| Q-P046-007 | P046 | Ownership boundary between P046 (requirements) and MOBILE_OPTIMIZATION.md (engineering)? | 2026-07-31 | Tech Director | Performance |
| Q-P047-001 | P047 | Color Palette/Typography/Icon Library/Spacing Rules/Design Tokens — document ID/timeline? | 2026-07-31 | Art Director | UI/UX |
| Q-P047-002 | P047 | Animation Timing values? | 2026-07-31 | Art Director | UI/UX |
| Q-P047-003 | P047 | UI Grid definition? | 2026-07-31 | Art Director | UI/UX |
| Q-P047-004 | P047 | Dark Mode — planned or out of scope? | 2026-07-31 | Design Owner | UI/UX |
| Q-P047-005 | P047 | UI scaling strategy for screen sizes / tablets? | 2026-07-31 | Tech Director | UI/UX |
| Q-P047-006 | P047 | HUD element list — which future specification? | 2026-07-31 | Design Owner | UI/UX |
| Q-P047-007 | P047 | Additional accessibility options beyond scalable text / color-friendly design? | 2026-07-31 | Design Owner | UI/UX |
| Q-P048-001 | P048 | Character Concepts — document ID / timeline? | 2026-07-31 | Art Director | Art Direction |
| Q-P048-002 | P048 | Map Concepts — document ID / timeline? | 2026-07-31 | Art Director | Art Direction |
| Q-P048-003 | P048 | Exact Color Palette (Future Color Palette Specification)? | 2026-07-31 | Art Director | Art Direction |
| Q-P048-004 | P048 | Material Library / Shader Library / Lighting Rules — timeline? | 2026-07-31 | Art Director | Art Direction |
| Q-P048-005 | P048 | Animation Library / Visual Effect Library — document ID? | 2026-07-31 | Art Director | Art Direction |
| Q-P049-001 | P049 | Formal mapping of Project Layers to Client/Server/Shared/Tools folders? | 2026-07-31 | Principal Architect | Technical Architecture |
| Q-P049-002 | P049 | Core Manager responsibilities/interfaces/lifecycle, relationship to GulfRun.Features.* asmdefs? | 2026-07-31 | Principal Architect | Technical Architecture |
| Q-P049-003 | P049 | Configuration implementation approach? | 2026-07-31 | Principal Architect | Technical Architecture |
| Q-P049-004 | P049 | Testing strategy / framework choice? | 2026-07-31 | Engineering Manager | Technical Architecture |
| Q-P049-005 | P049 | DI Framework / Code Generation / Build Pipeline / CI / CD / Plugin Strategy — ADR timeline? | 2026-07-31 | Tech Director | Technical Architecture |
| Q-31-001 | 31 | Spec priority — Sprint 1 scope? | 2026-07-31 | Design Owner | Yes — next milestone |

## 32.2 Resolved questions

| ID | Resolution summary | Resolved on | Approved by |
|----|--------------------|-------------|-------------|
| Q-02-001 | Primary audience: ages 12–35; competitive MP, party racing, social mobile | 2026-07-31 | Design Owner |
| Q-02-002 | Secondary: casual, families, Gulf culture enthusiasts | 2026-07-31 | Design Owner |
| Q-02-003 | Primary age range 12–35 (additional bands TODO) | 2026-07-31 | Design Owner |
| Q-02-004 | Player types listed from primary/secondary statements | 2026-07-31 | Design Owner |
| Q-02-005 | Regions: GCC → Middle East → Global | 2026-07-31 | Design Owner |
| Q-01-001 | All 7 pillar definitions supplied in P001 v1.1 | 2026-07-31 | Design Owner |
| Q-03-001 | Screen orientation: Landscape only | 2026-07-31 | Design Owner |
| Q-31-001 (partial) | Next specification is P002 — Core Gameplay Loop | 2026-07-31 | Design Owner |
| Q-P002-008 | Item boxes → **P008**; item **types/effects** still future | 2026-07-31 | Design Owner (P008) |
| Q-P003-005 | CFL-003: Obstacle system → **P007** | 2026-07-31 | Design Owner (P003A + P007) |
| Q-P008-002 | Item/weapon system rules → **P009**; lists/effects still future | 2026-07-31 | Design Owner (P009) |
| Q-P015-006 | Voice Chat specification → **P016** | 2026-07-31 | Design Owner (P016) |

## 32.3 Conflict register

| ID | Conflict description | Chapters / docs involved | Status |
|----|----------------------|--------------------------|--------|
| CFL-001 | P001 v1.0 Non Goals listed Voice Chat as “not defined,” while Social Multiplayer pillar (v1.1) makes voice communication core. **Resolution:** Intent affirmed in pillar; Voice Chat *system specification* remains deferred. | P001 §3.3 vs §7 | **Resolved (clarified)** |
| CFL-002 | P001 v1.0 Non Goals listed Progression as undefined while Long-Term Progression pillar (v1.1) names levels, ranks, cosmetics, seasonal content. **Resolution:** Goal types affirmed in pillar; Progression system specified in **P023** (formulas still TODO). | P001 §3.5 vs §7 | **Resolved (P023)** |
| CFL-003 | P002 “Avoid obstacles” vs P003 “Obstacles not defined.” **P003A + P007:** system exists; **P007** is Obstacle System Spec. Types/damage/collision effects still undefined in P007. Placeholder **P008** for obstacles **superseded by P007**. | P002, P003, P007 | **Resolved** |
| CFL-005 | P003A queued obstacles as **P008**; obstacles authored as **P007**. **P008** later assigned to **Item Box System** (v1.0). | P003A queue vs P007 / P008 | **Resolved** |
| CFL-004 | P003 adds Double Jump; supersedes P002 control list for in-race actions. | P002 vs P003 controls | **Resolved** |
