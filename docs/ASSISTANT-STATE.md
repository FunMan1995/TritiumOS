# ASSISTANT-STATE — Persist stub + hooks (S0)

**Status:** Shipper-ready stub spec (wave9 item **3**)  
**Canonical brief:** `TritiumOS.txt` §1a.2 (task / reminder / note hooks)  
**Sources of truth (code):** Linux `save_assistant_state` / `load_assistant_state` in `install/hosts/linux/tritiumos.c`; optional Forth mirrors under `forth/tritium/`  
**Companions:** `docs/ASSISTANT.md` (thin amend this tip), `docs/REKIA.md`

## 1. Purpose

Hosts already write a thin `evolve/assistant-state.trit` (assistant=, edition=, last-refine*). Spec §1a.2 also wants **task / reminder / note hooks**. This tip locks a **v2 schema** + greppable **`assistant-state-demo`**: write → load → assert keys survive. Not a full GUI planner; not Win/Android VM rewrite.

## 2. File

Path: `evolve/assistant-state.trit` (host evolve dir; gitignored runtime).

### 2.1 Schema (v2)

```
# TritiumOS assistant-state.trit v2
assistant=<name>
edition=32|64
last-refine=<label>                 # optional
last-refine-path=forth/refined/<label>.fs   # optional
updated=<unix-epoch>
neuron-count=<n>                    # stub OK (0 if unknown)
task=<id>|<text>                    # 0..N lines
reminder=<id>|<when>|<text>         # 0..N; when = unix or ISO stub string
note=<id>|<text>                    # 0..N lines
```

Rules:

- Header comment must say `v2` after this tip (v1 files: load best-effort; rewrite as v2 on next save).
- Cap stub lists at **8** each (task / reminder / note).
- Never commit populated state with secrets.

## 3. Words / host surfaces

| Surface | Stack / args | Notes |
|---------|--------------|-------|
| `assistant-state!` | `( -- )` or host void | Write v2 file (current name/edition/last-refine + hooks) |
| `assistant-state@` | `( -- )` | Load; print greppable lines; restore in-memory hooks |
| `assistant-task!` | `( id c-addr u -- )` / host | Upsert task line |
| `assistant-reminder!` | `( id when c-addr u -- )` / host | Upsert reminder |
| `assistant-note!` | `( id c-addr u -- )` / host | Upsert note |
| `assistant-state-demo` | `( -- )` | See §5 |

Existing `save_assistant_state` / `load_assistant_state` may grow in place; Forth mirrors optional if Linux host is Lab SoT.

## 4. Markers

```
[assistant-state] save v2 path=...
[assistant-state] load ok tasks=N reminders=N notes=N
[assistant-state-demo] OK
[assistant-state-demo] FAIL
```

## 5. `assistant-state-demo`

1. Reset / clear evolve assistant-state (or write over).
2. Set name (or keep), add ≥1 task, ≥1 reminder, ≥1 note via hooks.
3. `assistant-state!` → file present; header `v2`.
4. Clear in-memory hooks (or restart process / reload path).
5. `assistant-state@` → counts match; greppable `load ok`.
6. Print `[assistant-state-demo] OK`.

Prior `s0-assist-demo` / `persist-demo` stay green (last-refine path unchanged).

## 6. Thin amend — `docs/ASSISTANT.md`

- Status: cite wave9 **3**.
- Companions: add `ASSISTANT-STATE.md`.
- §5: point schema details to this doc; keep one-line role summary.
- §4 commands: add `assistant-state-demo`.
- Acceptance: Lab smokes `assistant-state-demo`.

## 7. Non-goals

- Calendar sync / push notifications
- Encrypted state
- Changing S0 refine pipeline markers
- economy-wire / colon (wave9 **2** done; **4** next)

## 8. Acceptance (Test Lab)

1. `docs/ASSISTANT-STATE.md` present (Research byte-copy OK); `ASSISTANT.md` thin amend present.
2. `assistant-state-demo` → OK (markers §4); file shows `v2` + task/reminder/note lines.
3. `s0-assist-demo` still OK; regression green (`economy-wire-demo`, `trit-math-demo`, …).
4. No merge.

## 9. Cite

- `TritiumOS.txt` §1a.2
- `docs/ASSISTANT.md`
- `install/hosts/linux/tritiumos.c` (`save_assistant_state`, `load_assistant_state`)
