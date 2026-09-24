# ASSISTANT — On-demand S0 path

**Status:** Shipper-ready spec (wave4 items **1** + **3**; Linux SoT + Win/Android host parity)  
**Canonical brief:** `TritiumOS.txt` §§1, 5a (bootstrap), evolution ladder  
**Sources of truth (code):** Linux `s0_assist` / `s0_assist_demo` in `install/hosts/linux/tritiumos.c`; Win `TritiumForthVM.S0Assist` / Android `s0Assist`; Forth pipeline `drena-step` + `rekiA-refine`  
**Companions:** `docs/REKIA.md` (emit contract), `docs/DRENA.md` (step/grow), `docs/GROUPS.md` (context clusters), `docs/ASSUMPTIONS.md` (S0 + persist)

## 1. Purpose

The assistant is the **user-facing on-demand path** into TritiumOS: free-text (chat/REPL) becomes topology motion + pure-math refine, then a **live Forth word** — never opaque weights, never a scaffold “example response.”

Division of labor:

| Layer | Owns |
|-------|------|
| Assistant / host S0 | Route free-text → engines; persist name/state; print assist markers |
| D.R.E.N.A. | `drena-step` (spawn/rewire/grow per S3) |
| R.E.K.I.A. | `rekiA-refine` → `evolve/forth/refined/*.fs` + include |
| Groups | Optional context cluster (`GROUP-<label>/`); see `GROUPS.md` |

## 2. First-run bootstrap (hosts)

Priority-1 hosts (Win / Android / Linux AppImage) share the same wizard shape:

1. License key (scaffold OK until wave4 **5** slot-11 enforce)
2. Name assistant → `evolve/assistant-name.trit`
3. Edition 32/64 → `evolve/edition.trit` (DRENA id/cell width; see wave3 **5**)

Slogan / about: “TritiumOS by Draco” — product chrome, not refine logic.

## 3. S0 free-text contract

**Rule:** Default chat/REPL free-text routes to **`drena-step` then `rekiA-refine`** (host may mirror both). Forbidden in the reply path: scaffold strings, “Example response”, or any path that skips write+include of refined Forth.

### 3.1 Pipeline (logical)

```
free-text query
  → [S0] assist: <query>
  → drena-step          \ topology tick (skip if S3=RESERVED)
  → rekiA-refine        \ extract → contract → to-forth
  → wrote+include evolve/forth/refined/refined-<id>.fs
  → live vocab word
  → assistant-state! + graph-save
  → [S0] assist done — refined word live
```

Query bytes may fold into the contracted trit signature / emitted constant (Linux host stand-in); semantics stay pure-math / deterministic given the same query + graph seed policy.

### 3.2 Smoke

| Demo | Expect |
|------|--------|
| `s0-assist-demo` | Fixed query `hello tritium` → `[S0] assist` → wrote+include `refined-1.fs` → word live; no scaffold |
| Free-text REPL | Same markers as demo for one-input → refined live word |

Gate platforms: **Linux** (AppImage SoT) + **Windows** / **Android** host twins (wave4 **3**).
Win `TritiumForthVM.S0Assist` / Android `s0Assist` match Linux markers; smoke via `s0-assist-demo`
(`[s0-assist-demo] OK — refined written; word live`) or headless `install/hosts/windows/smoke-s0`.

## 4. REPL / demo commands (Linux reference)

Non-exhaustive; keep demos green every tip:

| Command | Role |
|---------|------|
| free-text | S0 assist path (§3) |
| `s0-assist-demo` | Fixed free-text smoke |
| `rekia-demo` / `rekiA-demo` | Engine refine without chat wrapper |
| `groups-demo` / `groups-persist-demo` / `groups-status` / `group-vocab-demo` | Labeled groups + searchable vocab unit |
| `grow-step-demo` / `s3-reserved-demo` | DRENA S3 policy |
| `qwantum-atoms-demo` | K-atoms extract-only |
| `edition-demo` | 32/64 → DRENA width |
| `help` / `status` / `about` | Host chrome |

## 5. Evolve state files

| File | Role |
|------|------|
| `evolve/assistant-name.trit` | User-chosen name (first-run) |
| `evolve/assistant-state.trit` | Post-refine snapshot (assistant=, last refined label/path) |
| `evolve/user-graph.trit` | DRENA graph + `group`/`member` lines |
| `evolve/edition.trit` | `edition=32\|64` |
| `evolve/forth/refined/*.fs` | Live refined words (R.E.K.I.A. emit only) |

Boot loads assistant-state + user-graph + refined includes so live words and groups survive restart (Linux AppImage is gate SoT for demos).

## 6. What the assistant does *not* do

- Does not treat Qwantum dump `.fs` as live vocab (`QWANTUM-REKIA.md`)
- Does not auto-advance S3=`11` RESERVED (`ASSUMPTIONS.md`)
- Does not mint license slots or Assimilate currency (later waves)
- Wave4 **2**: `GROUP-<label>/` is a searchable vocab unit (`group-vocab-demo`); see `GROUPS.md`

## 7. Acceptance (Test Lab — docs tip)

1. `docs/ASSISTANT.md` present; one-line cite from `docs/REKIA.md` (S0) and/or README OK.
2. Linux suite unchanged green: `s0-assist-demo`, `rekia-demo`, `s3-reserved-demo`, `groups-demo` / persist, `grow-step-demo`, `qwantum-atoms-demo`, `edition-demo`.
3. Win and/or Android: free-text → real S0 (no parity-print stub); `s0-assist-demo` OK line greppable; prefer headless Win smoke when SDK absent.

## 8. Cite

- `TritiumOS.txt` §§1, 5a
- `docs/REKIA.md`, `docs/DRENA.md`, `docs/GROUPS.md`, `docs/ASSUMPTIONS.md`
- `install/hosts/linux/tritiumos.c` (`s0_assist`, `s0_assist_demo`); Win `TritiumForthVM.S0Assist` + `install/hosts/windows/smoke-s0`; Android `s0Assist`; `forth/tritium/rekia.fs` / `drena.fs`
