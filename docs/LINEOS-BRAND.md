# LINEOS-BRAND — Splash / about markers (scaffold)

**Status:** Shipper-ready stub spec (wave7 item **3**)  
**Canonical brief:** `TritiumOS.txt` §§1a.1, 1 (slogan), Phase 9  
**Sources of truth (code):** extend `lineos-graduate` path + new `lineos-splash` / `lineos-brand-demo`; markers under `evolve/lineos/`  
**Companions:** `docs/LINEOS.md` (graduation gates), `docs/ASSISTANT.md` (about / first-run)

## 1. Purpose

After graduation (`product_id → lineos`), hosts must surface **L.I.N.E.O.S.** naming + slogan on splash/about — while preserving Tritium **32/64** origin badge. This tip lands **marker files + demo printouts** only: no production asset bake, no store rebrand, no irreversible UX confirm pack.

## 2. Brand constants

| Field | Value |
|-------|-------|
| Product (post-grad) | `L.I.N.E.O.S.` |
| `product_id` | `lineos` |
| Slogan / `LINEOS_SLOGAN` | *The line tread between madness and genius.* |
| Creator credit | *TritiumOS by Draco* (origin) |
| Edition badge | 32 (cyan) \| 64 (magenta) — preserved |
| Pre-grad about | `{assistant-name} — powered by TritiumOS` |

Slogan appears on L.I.N.E.O.S. splash/about **only after** scaffold graduation (or demo force-ready).

## 3. Marker layout

```
evolve/lineos/
  brand.json       # productName, slogan, productId, edition, originBadge
  SPLASH.txt       # human-readable splash lines for smoke
  ABOUT.txt        # about-screen lines for smoke
```

`brand.json` shape (v1 stub):

```json
{
  "productName": "L.I.N.E.O.S.",
  "productId": "lineos",
  "slogan": "The line tread between madness and genius.",
  "originBadge": "TritiumOS by Draco",
  "edition": 64,
  "graduated": true
}
```

Write markers when:

1. `lineos-graduate` succeeds, **or**
2. `lineos-brand-demo` / `lineos-splash-demo` force-writes for smoke.

Do not commit populated `evolve/lineos/` blobs; template under `lineos/` OK.

## 4. Surfaces

| Surface | Behavior |
|---------|----------|
| `lineos-splash` | Print / write splash markers if graduated (or `--demo`); else refuse *not graduated* |
| `lineos-about` | Print / write about markers (assistant name if known + slogan if graduated) |
| `lineos-brand-demo` | Force graduated markers → assert slogan + productName + edition present → `[lineos-brand-demo] OK` |

May live in `forth/tritium/lineos.fs` + Linux host + thin CLI; reuse existing graduate path to set `graduated`.

## 5. Markers (stdout)

```
[LINEOS] splash: L.I.N.E.O.S.
[LINEOS] slogan: The line tread between madness and genius.
[LINEOS] origin: TritiumOS by Draco (edition=32|64)
[LINEOS] refuse — not graduated (§1a.1)
[lineos-brand-demo] OK
[lineos-brand-demo] FAIL
```

## 6. Non-goals (this tip)

- Shipping new JPG/BMP asset packs to dist/
- Changing store listings / package display names in production
- Irreversible confirm UX (still later)
- Merging graduate flips to main

## 7. Acceptance (Test Lab)

1. `docs/LINEOS-BRAND.md` present (Research byte-copy OK); `LINEOS.md` may one-line cite.
2. `lineos-brand-demo` → OK; splash/about files or stdout show slogan + L.I.N.E.O.S. + edition.
3. Pre-grad `lineos-splash` refuses with clear marker (optional but preferred).
4. Existing suite green (`lineos-graduate-demo`, fleet, master, …).
5. No merge; no production branding release.

## 8. Cite

- `TritiumOS.txt` §§1, 1a.1, Phase 9
- `docs/LINEOS.md`, `docs/ASSISTANT.md`
- `forth/tritium/lineos.fs`; `LINEOS_SLOGAN`
