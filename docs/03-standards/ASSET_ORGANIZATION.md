# Asset Organization

**Last updated:** 2026-07-31  
**Owner:** Art Director + Tech Art + Client Lead  
**Audience:** Art, Audio, UI, Engineering, LiveOps

---

## 1. Dual-store model

| Store | Path | Purpose |
|-------|------|---------|
| Source | `Art/` | DCC masters, layered PSDs, high-poly, project files |
| Runtime | `Client/Assets/_Project/...` | Budgeted, imported Unity assets |

**Rule:** Artists work in Source; Tech Art / pipeline publishes to Runtime.

## 2. Source tree (`Art/`)

```
Art/
├── README.md
├── characters/
├── vehicles/              # or mode-specific props
├── environments/
├── props/
├── ui/
│   ├── icons/
│   ├── marketing/
│   └── fonts-source/
├── audio/
│   ├── music/
│   ├── sfx/
│   └── voice/
├── vfx/
├── animation/
└── _reference/            # mood, legal-cleared refs only
```

Use Git LFS or an art vault (ADR at M1). `.gitignore` excludes caches (`*.blend1`, Maya `workspace`, Substance temp).

## 3. Runtime tree (Unity)

Aligns with [FOLDER_ARCHITECTURE](../02-architecture/FOLDER_ARCHITECTURE.md):

```
Client/Assets/_Project/
├── Art/
│   ├── Characters/
│   ├── Environments/
│   ├── Props/
│   ├── Vehicles/
│   └── VFX/
├── Audio/
│   ├── Music/
│   ├── SFX/
│   └── Voice/
├── UI/
│   ├── Icons/
│   ├── Fonts/
│   ├── Toolkit/
│   └── Sprites/
├── Prefabs/
├── Scenes/
├── Settings/
└── Addressables/
```

## 4. Addressables & CDN

- Default delivery for non-boot content: **Addressables remote catalog** via CDN.
- Groups by: `Boot` (local), `Meta`, `Season_XX`, `Mode_YY`, `Locale_ZZ`.
- Catalog versioning MUST be monotonic; broken catalogs have kill switch.
- Soft Launch+: seasonal content SHOULD be remote-updatable without full binary when store policy allows.

## 5. Budgets (initial — refine at M1)

| Budget | Soft target |
|--------|-------------|
| Install size (first download) | Set per market; track in CI |
| Peak RAM Low tier | Device matrix |
| Texture: character hero | Max resolution by tier |
| Audio music | Compressed format standards (e.g., Vorbis/AAC policy) |
| Draw calls / scene | Mode-specific caps |

Exact numbers live in [MOBILE_OPTIMIZATION.md](../04-engineering/MOBILE_OPTIMIZATION.md) and `QA/device-matrix/`.

## 6. Import settings governance

- Presets committed for textures, audio, models.
- PRs that change presets require Tech Art review.
- Mipmaps, compression (ASTC/ETC2), and Read/Write MUST be intentional.

## 7. Naming

Follow [NAMING_CONVENTIONS.md](NAMING_CONVENTIONS.md). Loc keys: `UI.SHOP.TITLE`, not hardcoded English in prefabs where avoidable.

## 8. Localization assets

- String tables under versioned loc pipeline (`Tools/localization/`).
- Fonts: licensed files only; document license in `ThirdParty` notices.
- Images with baked text avoided; prefer runtime text.

## 9. Audio

- Loudness targets documented by Audio Lead.
- Voice files partitioned by locale Addressables groups.
- Music stems vs. loops naming consistent (`Mus_`, `Sfx_`, `Vo_`).

## 10. UI

- Prefer UI Toolkit assets under `UI/Toolkit/`.
- Icon grid sizes standardized (e.g., 128/256 masters → runtime atlases).
- No orphan sprites in random folders.

## 11. Legal & safety

- No unlicensed scrapes in `_reference` that ship.
- UGC (if ever) requires moderation pipeline before runtime publish.
- Store screenshots/marketing binaries may live outside game Addressables.

## 12. Pipeline ownership

| Step | Owner |
|------|-------|
| Create source | Art |
| Validate budgets | Tech Art |
| Publish to Unity | Tech Art / content pipeline |
| Wire Addressables | Client eng + Tech Art |
| Live content enable | LiveOps |

## 13. Forbidden

- Committing raw photos multi-hundred-MB without need
- Duplicating the same texture in many folders
- Putting seasonal content only in binary without remote plan (post M4)
- Editing `generated` atlases by hand without source update
