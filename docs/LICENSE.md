# LICENSE — Shared key & device slots

**Status:** Shipper-ready spec (wave5 item **1**; matches PR #19 license stub)  
**Canonical brief:** `TritiumOS.txt` §5a.4–5a.5  
**Sources of truth (code):** `tools/tritium-license`; Linux `license_status` / `license_register` in `install/hosts/linux/tritiumos.c`; `license/validator.ps1`  
**Companions:** `docs/ASSUMPTIONS.md` (slot policy), `docs/ASSISTANT.md` (first-run license step), `docs/MASTER.md` (mint/verify scaffold), `docs/QUEUE.md` / `docs/ASSIMILATE.md`

## 1. Purpose

Technical enforcement for the **shared license key**: one user-facing key unlocks TritiumOS on up to **10** devices. This doc covers slot registry, refuse-11, and status surfaces. It does **not** cover legal text, real crypto signing, or master-root issuance (those stay scaffold until a later wave).

## 2. Rules (§5a.4)

| Field | Rule |
|-------|------|
| Max slots | **10** active registrations per shared license key |
| Slot 11 | **Always refused** (and any register when already 10/10) |
| Registry | `evolve/license-slots.json` (runtime; **not** committed) |
| De-register | Frees a slot; local `evolve/` blob may remain on device |
| Sync | Optional fleet sync only among slots sharing the same key (not implemented yet) |
| Crypto | Scaffold only — `master-verify` is **format-only** (§5a.5); no signed payload yet |

Never commit secrets, license keys, or populated `license-slots.json`.

## 3. Registry file

Path: `evolve/license-slots.json` (override with `TRITIUM_LICENSE_SLOTS`).

Shape (v1 stub):

```json
{
  "maxSlots": 10,
  "devices": [
    { "id": "dev1", "slot": 1 }
  ]
}
```

Created on first status/register if missing. `clear` (CLI test helper) wipes the file.

## 4. Surfaces

### 4.1 CLI — `tools/tritium-license`

| Command | Behavior |
|---------|----------|
| `status` | Print `tritium-license status: N/10` (+ per-slot ids) |
| `register <device-id>` | Claim next free slot 1..10; refuse when full / slot 11 |
| `clear` | Wipe registry (Test Lab / local smoke only) |

### 4.2 Linux REPL

| Command | Behavior |
|---------|----------|
| `license-status` | Same N/10 contract |
| `license-register <id>` | Same refuse-11 contract |

### 4.3 Windows

`license/validator.ps1` mirrors the slot gate for installer/scaffold paths.

## 5. Smoke (Test Lab)

```
tools/tritium-license clear
tools/tritium-license register dev1   # … through dev10
tools/tritium-license register dev11  # expect refuse slot 11
tools/tritium-license status          # expect 10/10
```

Markers: `[license] refuse slot 11` (or equivalent) and `tritium-license status: 10/10`. Cite §5a.4 in failure/refuse lines.

## 6. Master system (§5a.5 scaffold)

Mint / format-verify scaffold lives in `docs/MASTER.md` (wave7 item **1**): `master-mint-license` / `master-mint-worker` / `master-verify` / `master-demo` via `tools/tritium-master` + Linux host + Forth. **Format-only** — no real crypto / master-root yet.

| Word | Stack | Notes |
|------|-------|-------|
| `master-mint-license` | `( slots -- key$ )` | default slots=10 → `TRIT-<16hex>-DRACO` |
| `master-mint-worker` | `( device-id -- key$ )` | `TRIT-W-<idhash>-DRACO`; refuse empty |
| `master-verify` | `( key$ -- flag )` | format check only (not cryptographic) |

User installers accept **license-key**; queue / Assimilate prefer **worker-key**.

## 7. Acceptance (Test Lab — docs tip)

1. `docs/LICENSE.md` present; ASSUMPTIONS / ASSISTANT may keep one-line cites.
2. Slot smoke still green: refuse 11 + status 10/10 (CLI and/or Linux REPL).
3. Demo suite unchanged green (rekia / s3-reserved / groups / persist / group-vocab / group-link / qwantum / s0 / edition / grow).
4. Docs-only tip: no behavior change required for item **1** alone.

## 8. Cite

- `TritiumOS.txt` §§5a.4–5a.5
- `docs/ASSUMPTIONS.md` (Shared license slots)
- `tools/tritium-license`; `install/hosts/linux/tritiumos.c`; `license/validator.ps1`
