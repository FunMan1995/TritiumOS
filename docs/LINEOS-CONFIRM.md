# LINEOS-CONFIRM — Irreversible confirm UX (scaffold)

**Status:** Shipper-ready stub spec (wave8 item **2**; amends `docs/LINEOS.md` §3 gate 5)  
**Canonical brief:** `TritiumOS.txt` §1a.1 (user confirms “Promote to L.I.N.E.O.S.”)  
**Sources of truth (code):** extend `forth/tritium/lineos.fs` + Linux host; `evolve/graduation.json` / `evolve/lineos/`  
**Companions:** `docs/LINEOS.md`, `docs/LINEOS-BRAND.md`

## 1. Purpose

Graduation gate 5 requires explicit user confirm before scaffold `product_id` flip. Today `lineos-graduate-demo` force-ready skips confirm. This tip adds a **confirm flag** + refuse path, with a demo force that still PASSes Lab without interactive UI.

## 2. Rules

| Rule | Stub behavior |
|------|----------------|
| Default | `confirmed` / `userConfirmed` = **false** |
| `lineos-confirm` | Set confirm flag true; print marker |
| `lineos-graduate` | If gates 1–4 OK but confirm false → **refuse**; no product_id flip |
| After confirm | `lineos-graduate` may proceed (other gates still apply) |
| Demo | `lineos-confirm-demo` force-confirms then graduates **or** shows refuse then confirm→OK |
| Irreversible | Stub only: set `graduated` + brand markers; no real wipe/export-reset pack |

Extend `graduation.json` (optional field):

```json
"requireUserConfirm": true,
"userConfirmed": false
```

## 3. Surfaces

| Surface | Behavior |
|---------|----------|
| `lineos-confirm` | `( -- )` set userConfirmed / host flag |
| `lineos-graduate` | Refuse with marker if confirm required and not set |
| `lineos-confirm-demo` | 1) graduate without confirm → refuse marker; 2) `lineos-confirm`; 3) graduate → OK + brand markers |

Amend `docs/LINEOS.md` with one-line cite to this doc (Shipper may land both files same tip).

## 4. Markers

```
[LINEOS] confirm set
[LINEOS] refuse — confirm required (§1a.1)
[lineos-confirm-demo] OK
[lineos-confirm-demo] FAIL
```

Prior `lineos-graduate-demo` / `lineos-brand-demo` stay green (demo force may set confirm internally or bypass via `demoForceReady` — document which).

**Policy:** If `demoForceReady` remains, it may imply confirm for graduate-demo only; production `lineos-graduate` must still honor confirm when `requireUserConfirm` is true.

## 5. Non-goals

- Real GUI modal / irreversible export-reset
- Production store rebrand
- Merge to main

## 6. Acceptance (Test Lab)

1. `docs/LINEOS-CONFIRM.md` present (Research byte-copy OK); `LINEOS.md` cites it.
2. `lineos-confirm-demo` → OK (refuse then confirm path greppable).
3. `lineos-brand-demo` / `lineos-graduate-demo` still green.
4. No merge.

## 7. Cite

- `TritiumOS.txt` §1a.1
- `docs/LINEOS.md`, `docs/LINEOS-BRAND.md`
- `forth/tritium/lineos.fs`
