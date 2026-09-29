# GROUPS — Labeled neural groups

**Status:** Shipper-ready spec (wave3 item **3** + wave4 items **2** vocab unit + **4** `group-link!`)  
**Canonical brief:** `TritiumOS.txt` §3.4–3.5, §5  
**Sources of truth (code):** `forth/tritium/drena.fs` (group words); Linux host `install/hosts/linux/tritiumos.c` (graph lines + restart restore)  
**Companions:** `docs/DRENA.md` §6–7 (topology owner), `docs/REKIA.md` (`rekiA-label-group`), `docs/ASSUMPTIONS.md` (persist format), `docs/NEURON.md`, `docs/GROUPS-NESTED.md` (nested find + vocab persist)

## 1. Purpose

A **labeled neural group** clusters neurons under a human/machine label. Groups are D.R.E.N.A. structure: membership, prefix string, and persist. They are **not** refined Forth emitters — R.E.K.I.A. may suggest a label (`rekiA-label-group`) then hand off to `drena-group` / `group-label!`.

Groups are the unit of assistant context (which cluster is active) and a Forth vocabulary namespace: `GROUP-<label>/` is a **searchable vocab unit** (Dusk-style), not prefix-only. The prefix string remains for display/persist; the kernel mounts a findable wordlist keyed by that name under the group id (D.R.E.N.A. + kernel `ENTRY-GIDS`).

## 2. Caps (current)

| Constant | Value | Notes |
|----------|-------|-------|
| `MAX-GROUPS` | 16 | Hard cap; `drena-group` returns `-1` when full |
| `MAX-MEMBERS` | 32 | Per group; join no-ops when full |
| Label storage | 32 bytes / group | Counted string at `group-label-addr` |
| `VOCAB-PFX-SZ` | 48 | Counted `GROUP-<label>/` buffer |

## 3. Words

| Word | Stack | Notes |
|------|-------|-------|
| `drena-group` | `( label-addr -- group-id )` | Counted string → new gid; builds prefix |
| `group-label!` | `( c-addr u gid -- )` | Set/rename label$; rebuilds vocab prefix |
| `drena-join` | `( neuron group -- )` | Append `neuron-id`; skip dups; capacity-checked |
| `group-members` | `( gid -- addr count )` | Member id table slice |
| `group-vocab-prefix` | `( gid -- c-addr u )` | `GROUP-<label>/` counted string |
| `group-register-vocab-unit` | `( gid -- )` | Mount prefix as searchable dict entry |
| `group-vocab-add` | `( c-addr u gid -- )` | Register a word under the group unit |
| `group-find` | `( c-addr u gid -- i )` | Scoped find (`group-entry-find`) |
| `.group` | `( gid -- )` | Debug: label, prefix, member list |
| `group-link!` | `( group-a group-b -- )` | Inter-group bridge (`LINK-INTER` typed record) |

Create path accepts a **counted-string address** (`drena-group`); rename uses explicit `( c-addr u gid )`.

## 4. `GROUP-<label>/` searchable vocab unit

Format: literal `GROUP-` + label bytes + `/`.

- Built by `group-set-vocab-prefix` whenever the label is set (display/persist prefix string kept).
- **Also a searchable vocab unit** (Dusk-style): `group-register-vocab-unit` mounts the name in the kernel dict with `ENTRY-GIDS` = gid.
- Words under the group (`group-vocab-add` / refined labels) are findable via `group-find` / `group-entry-find` — scoped, not only the flat global dict.
- Visible after AppImage restart via host restore (`groups-status` / `groups-persist-demo`); host re-registers the unit name on load.

Smoke expects: label `demo` → prefix `GROUP-demo/`; `group-vocab-demo` → `[group-vocab-demo] OK — find under GROUP-demo/`.


## Inter-group bridge (`group-link!`)

Stack: `( group-a group-b -- )` — cite `TritiumOS.txt` §3.5.

Creates a typed `LINK-INTER` record in the global links table via existing `link!`. Prefer bridge through representative members (first nid of each group). If a group has no members, store a marked group-id edge (`$80000000 | gid`) so the table clearly flags a group–group link.

Smoke: `group-link-demo` → `[group-link-demo] OK — inter-group bridge` (Linux host SoT).

## 5. Persist (`evolve/user-graph.trit`)

Host `platform-graph-save` / `platform-graph-load` (Linux is gate source of truth for demos).

**v1 lines** (preferred):

```
group <gid> <label>
member <gid> <nid>
```

Legacy still loads: `group gid=N label=...`.

`graph-save` / `graph-load` in Forth defer to the host. After load, host reprints `GROUP-<label>/` and restores member lists so demos survive restart without re-join.

See `docs/ASSUMPTIONS.md` (groups-persist-demo expectations).

## 6. R.E.K.I.A. handoff

| Word | Role |
|------|------|
| `rekiA-label-group` | `( s0 s1 s2 -- c-addr u )` suggested label from refined trit state |
| refine pipeline | may `drena-group` the suggested label after contract |

Labeling semantics live in `docs/REKIA.md`; group **storage and topology** stay in D.R.E.N.A. / this doc.

## 7. Edition width (wave3 **5** companion)

`TritiumOS.txt` treats `group-id` / neuron ids as **edition-width** addresses. Today `edition.trit` (32 cyan / 64 magenta) drives UI + `ARCH` / `edition@` but historically did not size DRENA ids.

Wave3 **5** wires `edition.trit` → DRENA id/cell width so group and neuron ids match the chosen edition. GROUPS.md does not change when only width plumbing lands; member/join/persist contracts stay the same.

## 8. Acceptance (Test Lab)

1. `docs/GROUPS.md` present; cite from `docs/DRENA.md` §6 (one-line pointer OK).
2. Linux suite still green: `groups-demo`, `groups-persist-demo`, `group-vocab-demo`, `rekia-demo`, `s3-reserved-demo`, `grow-step-demo`, `qwantum-atoms-demo`, `s0-assist-demo`, `edition-demo`.
3. `group-vocab-demo` greps: `[group-vocab-demo] OK — find under GROUP-demo/`.
4. Wave4 item **2**: `GROUP-<label>/` is a searchable vocab unit (not prefix-only).
5. Wave4 item **4**: `group-link-demo` greps `[group-link-demo] OK — inter-group bridge`.

## 9. Cite

- `TritiumOS.txt` §§3.4–3.5, 5
- `docs/DRENA.md`, `docs/REKIA.md`, `docs/ASSUMPTIONS.md`, `docs/NEURON.md`
- `forth/tritium/drena.fs`; `install/hosts/linux/tritiumos.c` (graph group/member + restore)
