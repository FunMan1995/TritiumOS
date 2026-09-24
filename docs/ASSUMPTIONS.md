# Assumptions — TritiumOS

Living list of packing and policy choices. Change only with an explicit product decision (via Chief of Staff / user).

## S3 mode `11` (RESERVED)

- Encoding: low 2 bits of S3 nibble = `3` (`11b`).
- **Leave alone:** `drena-rewire` must **not** auto-advance into or out of RESERVED.
- `set-s3-mode` refuses to write mode `3` as a progression target; spawning with variation `3` is allowed for research, but rewire is a no-op that logs `rewire skipped (S3 RESERVED)`.
- **`drena-grow` / `drena-step` never advance RESERVED:** grow allocates a child that inherits the parent's S3 mode (RESERVED parent → RESERVED child) and never calls rewire; step on a RESERVED `last-grown` logs `[DRENA] step skipped (S3 RESERVED)` and exits.
- No invented RESERVED behavior until Research + user dictate it (TritiumOS.txt §3.2).

## Smoke (Test Lab)

```
groups-demo
\ expect: join members=1 then 2; prefix GROUP-demo/; graph has group/member lines

groups-persist-demo
\ expect: [persist] OK — groups + members present after restart … GROUP-demo/

s3-reserved-demo
\ expect: before=3, rewire skipped, after=3

grow-step-demo
\ expect: spawn0→grow→step; RESERVED grow child-mode=3; step skipped; after=3

qwantum-atoms-demo
\ expect: OK — refined written; dump not vocab

s0-assist-demo
\ expect: [S0] assist: …; [REKIA] wrote+include …/refined-1.fs; OK — refined written; word live
\ free-text (e.g. hello tritium) → same S0 path; no scaffold / "Example response"
```

## S0 assistant path (free-text)

- Host default chat/REPL free-text routes to **drena-step + rekiA-refine** (Linux `s0_assist`), not scaffold reply.
- Emits `evolve/forth/refined/refined-*.fs` + `[REKIA] wrote+include` / live vocab (same artifact contract as `rekia-demo`).

## Trit nibble packing

Default remains mod-3 split (`trit-pair@` / `encode-trit`). Dense 2+2 packing is out of scope unless recorded here later.

## Persist files

- `evolve/user-graph.trit` — neuron headers + typed links snapshot after refine; also `group <gid> <label>` / `member <gid> <nid>` (v1-compatible; legacy `group gid=` still loads).
- `evolve/assistant-state.trit` — touched after `rekiA-refine` (last-refine label/path).
- Reload on host start; refined `.fs` under `evolve/forth/refined/` stay live vocab.
- **Groups across AppImage restart:** `load_user_graph` on boot restores host labels + member lists and reprints `GROUP-<label>/` prefixes (Linux host is source of truth for demos).

## Qwantum K-atoms (extract scope only)

- Loader word `qwantum-atoms-load` hashes dump text under `evolve/qwantum-dump/<id>/` into `qwantum-k-influence` for `rekiA-extract` mixing only.
- Dump `.fs` (e.g. `qwantum-sample.fs`) is **never** included as live vocab — refine owns emission (`rekiA-to-forth` → `evolve/forth/refined/refined-*.fs`).
- Atoms-load must not touch S3=`11` RESERVED.
- See `docs/QWANTUM-REKIA.md` §5.

Smoke:

```
qwantum-atoms-demo
\ expect: [qwantum-atoms-demo] OK — refined written; dump not vocab
\ expect: [QWANTUM] atoms-load → extract scope (no vocab)
```

