# Sprint 17.1 — Final Main Menu Layout — Sprint Report

**Role:** Lead UI Systems Engineer  
**Scope:** Replace temporary Main Menu placeholders with production layout hierarchy on `MainMenu.unity`. Visual hierarchy only.  
**Status:** Superseded by Sprint 17.2 (same commit ships final 17.2 naming).

## Hierarchy intent (layout containers)

```
MainMenuCanvas
├── Background
├── Logo (→ GulfRunLogo in 17.2)
├── Character (→ CharacterImage in 17.2)
├── TopLeft / PlayButton
├── LeftMenu / Lobby, Friends, Clan
├── TopRight / PlayerCard
├── RightMenu / Missions, Store, Settings, Rankings
├── PopupRoot
└── SafeArea
```

Canvas: Overlay, Scale With Screen Size 1920×1080 match 0.5.
