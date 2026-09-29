# MASTER — Root trust & key issuance (scaffold)

**Status:** Shipper-ready stub spec (wave7 item **1**)  
**Canonical brief:** `TritiumOS.txt` §5a.5, Phase 6  
**Sources of truth (code):** `master/mint-license.ps1` (existing scaffold); new `tools/tritium-master` and/or `forth/tritium/master.fs` + Linux host mirrors  
**Companions:** `docs/LICENSE.md` (slots), `docs/QUEUE.md` / `docs/ASSIMILATE.md` (worker-key consumers)

## 1. Purpose

The **master system** is the root trust and issuance layer. Only master may create keys. This wave lands a **scaffold stub**: GUID-style mint + format check as `master-verify` — **no** real crypto, no master-root HSM, no network sync.

Never commit minted keys, worker keys, or populated registries.

## 2. Key types (§5a.5)

| Key type | Issuer | Purpose |
|----------|--------|---------|
| master-root | factory | Signs other keys — **out of scope** this stub (RESERVED) |
| license-key | master | User-facing shared key (≤10 devices — `LICENSE.md`) |
| worker-key | master | Binds device to collective queue + Assimilate |
| assimilate-id | master | Links wallet to contribution receipts — **thin stub OK** (print / marker only) |

User installers accept **license-key**; queue / Assimilate prefer **worker-key** from the same family (or issued alongside).

## 3. Words / CLI

| Surface | Stack / args | Stub behavior |
|---------|--------------|---------------|
| `master-mint-license` | `( slots -- key$ )` default slots=10 | Emit `TRIT-<16hex>-DRACO` (or CLI print); optional write under `evolve/master/` **gitignored** |
| `master-mint-worker` | `( device-id -- key$ )` | Emit `TRIT-W-<idhash>-DRACO` scaffold; refuse empty id |
| `master-verify` | `( key$ -- flag )` | **Format** check only (prefix/shape); not cryptographic |
| `master-demo` | `( -- )` | Mint license → mint worker → verify both → `[master-demo] OK` |

Existing `master/mint-license.ps1` may remain as a Windows helper; prefer one SoT CLI `tools/tritium-master` (mint-license / mint-worker / verify / demo) aligned with Linux REPL + Forth.

## 4. Markers

```
[master] mint-license slots=10 key=TRIT-…-DRACO
[master] mint-worker device=<id> key=TRIT-W-…-DRACO
[master] verify OK|FAIL format-only (§5a.5 scaffold)
[master-demo] OK
[master-demo] FAIL
```

## 5. Non-goals (this tip)

- Real signatures / `master-root` materialization
- Binding verify to `license/validator.ps1` crypto
- Fleet sync (wave7 **2**)
- Production key escrow

## 6. Acceptance (Test Lab)

1. `docs/MASTER.md` present (Research byte-copy OK); `LICENSE.md` may one-line cite MASTER.
2. `master-demo` → OK (CLI and/or Forth/Linux).
3. `master-verify` rejects clearly malformed strings; accepts scaffold mint shape.
4. Existing suite green (license refuse-11, queue, assimilate, integrate, lineos, …).
5. No secrets committed; no merge.

## 7. Cite

- `TritiumOS.txt` §5a.5, Phase 6
- `docs/LICENSE.md`, `docs/QUEUE.md`, `docs/ASSIMILATE.md`
- `master/mint-license.ps1`
