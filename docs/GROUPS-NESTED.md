# GROUPS-NESTED — Nested search-order + vocab persist

**Status:** Shipper-ready amend (wave7 item **4**; deepens `docs/GROUPS.md`)  
**Canonical brief:** `TritiumOS.txt` §3.4–3.5; Dusk-style wordlist search order  
**Sources of truth (code):** `forth/tritium/drena.fs` + kernel entry table; Linux `install/hosts/linux/tritiumos.c` graph I/O  
**Companions:** `docs/GROUPS.md` (base), `docs/DRENA.md`, wave7 **5** kernel dict flesh

## 1. Purpose

Wave4 landed scoped `group-find` / `group-vocab-add` under `GROUP-<label>/`. Still open (GAPS): **nested search-order** across linked groups, and **persist of member words** (vocab entries) across restart — today only `group` / `member` lines survive in `evolve/user-graph.trit`.

This tip stubs both without a full Dusk wordlist hierarchy.

## 2. Nested search-order (stub)

When finding a name in group context, walk:

1. **Active gid** vocab unit (`group-find`)
2. **`LINK-INTER` neighbors** of that gid (via `group-link!` bridges) — each neighbor’s vocab, capped
3. Stop at first hit; miss → `-1`

| Word | Stack | Notes |
|------|-------|-------|
| `group-find-nested` | `( c-addr u gid -- i )` | Walk gid then linked neighbors |
| `group-search-order` | `( gid -- addr count )` | Optional: list of gids in walk order (debug) |

Caps: max **4** neighbor hops / **8** gids total walk (stub constants). No global dict fallback required for the demo (keep scoped).

## 3. Vocab persist

Extend graph lines (v2 additive; v1 still loads):

```
group <gid> <label>
member <gid> <nid>
vocab <gid> <word-name>
```

- On `graph-save` / host save: emit one `vocab` line per `group-vocab-add` entry for that gid.
- On load: recreate unit + re-`group-vocab-add` each word; `group-find` must succeed after reload without re-add.
- Legacy graphs without `vocab` lines remain valid.

## 4. Demo

| Surface | Behavior |
|---------|----------|
| `group-nested-demo` | Two groups + `group-link!`; add word only under B; `group-find-nested` from A finds it → OK |
| `group-vocab-persist-demo` | `group-vocab-add` → save → clear/reload → `group-find` still hits → OK |

Markers:

```
[group-nested-demo] OK — nested find via LINK-INTER
[group-nested-demo] FAIL
[group-vocab-persist-demo] OK — vocab restored from graph
[group-vocab-persist-demo] FAIL
```

Existing `group-vocab-demo` / `group-link-demo` / `groups-persist-demo` must stay green.

## 5. Doc landing

Shipper either:

- Lands this file as `docs/GROUPS-NESTED.md` **and** one-line cite from `GROUPS.md`, **or**
- Merges §§2–4 into `docs/GROUPS.md` (byte-cmp then vs this draft’s normative sections).

Prefer separate `docs/GROUPS-NESTED.md` for clean Lab byte-cmp.

## 6. Non-goals (this tip)

- Full Forth `SEARCH-WORDLIST` / nestable wordlist objects
- Kernel `find` rewrite (wave7 **5** may share entry table)
- Cross-host fleet of vocab (see `FLEET.md`)

## 7. Acceptance (Test Lab)

1. `docs/GROUPS-NESTED.md` present (Research byte-copy OK) or GROUPS.md amended with equivalent contract.
2. `group-nested-demo` → OK; `group-vocab-persist-demo` → OK.
3. Prior group demos still green.
4. No merge.

## 8. Cite

- `TritiumOS.txt` §§3.4–3.5
- `docs/GROUPS.md`, `docs/DRENA.md`, `docs/ASSUMPTIONS.md`
- `forth/tritium/drena.fs`; Linux graph save/load
