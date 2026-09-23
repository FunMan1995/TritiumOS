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
s3-reserved-demo
\ expect: before=3, rewire skipped, after=3

grow-step-demo
\ expect: spawn0→grow→step; RESERVED grow child-mode=3; step skipped; after=3
```

## Trit nibble packing

Default remains mod-3 split (`trit-pair@` / `encode-trit`). Dense 2+2 packing is out of scope unless recorded here later.

## Persist files

- `evolve/user-graph.trit` — neuron headers + typed links snapshot after refine.
- `evolve/assistant-state.trit` — touched after `rekiA-refine` (last-refine label/path).
- Reload on host start; refined `.fs` under `evolve/forth/refined/` stay live vocab.
