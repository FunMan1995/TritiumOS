# INTERPRET — Token-stream interpret stub

**Status:** Shipper-ready stub spec (wave8 item **1**)  
**Canonical brief:** Dusk `fs/doc/kernel.txt` interpret loop; `docs/KERNEL.md` (wave7 **5**)  
**Sources of truth (code):** `forth/tritium/kernel.fs` (+ optional `interpret.fs`); Linux host mirrors  
**Companions:** `docs/KERNEL.md`, `docs/GROUPS-NESTED.md`, `docs/FORTH-BASE-REFERENCES.md`

## 1. Purpose

Wave7 landed lookup-only `interpret-token`. This tip deepens a **minimal interpret loop**: walk a whitespace-separated token stream, `find` each name, execute or print a stub action. Optional `:` **create-only** (name into dict, no compile body). Not a full colon compiler or control-flow VM.

## 2. Words

| Word | Stack | Notes |
|------|-------|-------|
| `interpret-token` | `( c-addr u -- flag )` | Existing (KERNEL.md) |
| `interpret` | `( c-addr u -- )` | Split on bl; each token → find → hit: print/exec stub; miss: miss marker (continue or soft-abort — prefer **continue** + count misses) |
| `:` | `( "name" -- )` | **Create-only stub:** `entry-create` next word; no `]` body compile |
| `interpret-demo` | `( -- )` | See §4 |

Execute stub on hit: print `[interpret] exec #<i> <name>` (host may call a bound id later). No nested interpret required this tip.

## 3. Markers

```
[interpret] exec #N name=<word>
[interpret] miss name=<word>
[interpret] : created <name>
[interpret-demo] OK
[interpret-demo] FAIL
```

## 4. `interpret-demo`

1. `dict-reset` (or cold path)
2. `entry-create` (or `:`) two names
3. `interpret` a string containing both + one unknown → hits + one miss marker
4. Assert miss counted / greppable; hits ≥2
5. `[interpret-demo] OK`

Prior `kernel-demo` must stay green.

## 5. Non-goals

- Immediate / compile state, control flow (`if`/`then`/`do`)
- Full Dusk `findentry` linked units
- Host C#/Kotlin full VM rewrite (wave8 **4** is parity markers only)

## 6. Acceptance (Test Lab)

1. `docs/INTERPRET.md` present (Research byte-copy OK); KERNEL may one-line cite.
2. `interpret-demo` → OK; `kernel-demo` still OK.
3. Regression green (nested / vocab / rekia / s3 / …).
4. No merge.

## 7. Cite

- `docs/KERNEL.md`, `docs/FORTH-BASE-REFERENCES.md`
- `forth/tritium/kernel.fs`
- Dusk `fs/doc/kernel.txt`
