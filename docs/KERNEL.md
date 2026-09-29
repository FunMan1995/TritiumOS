# KERNEL — Minimal dict / find flesh

**Status:** Shipper-ready stub spec (wave7 item **5**)  
**Canonical brief:** `TritiumOS.txt` Phase 2; Dusk `fs/doc/kernel.txt` + `fs/mem/dict.fs` (see `docs/FORTH-BASE-REFERENCES.md`)  
**Sources of truth (code):** `forth/tritium/kernel.fs`; Linux host mirrors for demos  
**Companions:** `docs/GROUPS-NESTED.md` (ENTRY-GIDS), `docs/ARCHITECTURE.md`, `docs/NEURON.md`, `docs/COLON.md` (wave9 **4**), `docs/CONTROL.md` (wave10 **2**)
**See also:** `docs/INTERPRET.md` (wave8 **1** — interpret loop deepen); `docs/COLON.md` (wave9 **4** — colon body/marker stub).

## 1. Purpose

`kernel.fs` already has a flat name table (`entry-create` / `entry-find` / `group-entry-*` / `cold-boot` / soft `abort`). This tip **fleshes** the Dusk-aligned surface so Lab can smoke a dedicated `kernel-demo` without breaking the suite:

- Documented **find** aliases (`findentry` / `find`)
- Tiny **interpret stub** (lookup-only; no full control-flow compiler)
- Explicit **demo** + markers
- Demoes stay green (groups nested, rekia, license, …)

Not a full VM, linked dict, or xcomp.

## 2. Contract (keep + add)

| Word | Stack | Notes |
|------|-------|-------|
| `entry-create` | `( "name" -- )` | Existing; global gid=-1 |
| `entry-find` | `( c-addr u -- i )` | Existing; `-1` miss |
| `findentry` | `( c-addr u -- i )` | **Alias** of `entry-find` (Dusk name) |
| `find` | `( c-addr u -- i )` | Same alias for host smoke |
| `group-entry-find` / `group-entry-create` | (existing) | Used by GROUPS / nested |
| `words` / `.words` | `( -- )` | List table |
| `dict-reset` | `( -- )` | Empty table |
| `cold-boot` | `( -- )` | sysvars + dict-reset + edition default + markers |
| `abort` / `(abort")` | soft | Existing; no hard exit |
| `interpret-token` | `( c-addr u -- flag )` | **New stub:** `entry-find` ≥0 → true + print hit; else false + soft miss line |
| `kernel-demo` | `( -- )` | See §4 |

Caps: keep `MAX-ENTRIES` ≥ **32** (bump only if nested+demo pressure; document if changed).

## 3. Markers

```
[kernel] cold-boot OK (64-bit edition)   \ or 32-bit
[kernel] interpret-ready
[kernel] created #N
[kernel] find hit #N name=<word>
[kernel] find miss
[kernel-demo] OK
[kernel-demo] FAIL
```

`cold-boot` must remain safe for poly/AppImage load (no infinite abort).

## 4. `kernel-demo`

1. `dict-reset` (or rely on fresh cold-boot path)
2. Create two names via `entry-create` / host equivalent
3. `find` / `findentry` hits both; miss on unknown → miss marker
4. `words` lists them
5. `interpret-token` on a known name → true
6. Print `[kernel-demo] OK`

Linux host SoT preferred (mirror table already used for group entries); Forth words in `kernel.fs` remain the poly contract.

## 5. Non-goals (this tip)

- Full colon compiler / real branch XT (minimal `interpret` loop → `docs/INTERPRET.md`; `IF`/`THEN`/`ELSE` stubs → `docs/CONTROL.md`)
- Linked-list ENTRYSZ / forget / units objects (Dusk full)
- Replacing host C#/Kotlin VMs
- Breaking `group-find-nested` / vocab persist (share ENTRY-GIDS)

## 6. Acceptance (Test Lab)

1. `docs/KERNEL.md` present (Research byte-copy OK).
2. `kernel-demo` → OK; `find`/`findentry` miss path greppable.
3. Full regression green (esp. group-nested / group-vocab-persist / rekia / s3-reserved / cold path).
4. No merge.

## 7. Cite

- `TritiumOS.txt` Phase 2
- `docs/FORTH-BASE-REFERENCES.md` (Dusk kernel.txt / mem/dict.fs)
- `forth/tritium/kernel.fs`
- `docs/GROUPS-NESTED.md`, `docs/ARCHITECTURE.md`
