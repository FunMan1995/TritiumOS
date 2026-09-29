# REFINED-BOOT — Cold-load `evolve/forth/refined/*.fs`

**Status:** Shipper-ready stub spec (wave10 item **4**; thin amend wave11 **4** AppImage host path)
**Canonical brief:** R.E.K.I.A. → runnable Forth; `TritiumOS.txt` persist / intelligence loop
**Sources of truth (code):** Linux `load_refined_modules()` in `install/hosts/linux/tritiumos.c`; `evolve/forth/refined/`; Win/Android parity later
**Companions:** `docs/HOST-BOOT.md` (wave10 **1**), `docs/REKIA.md`, `docs/ASSIMILATE.md`, `docs/ASSISTANT-STATE.md`, `docs/APPIMAGE-REFINED.md` (wave11 **4**)

## 1. Purpose

Core boot (wave10 **1**) lists poly/core only. This tip makes **persisted refined modules** part of cold boot: scan `evolve/forth/refined/*.fs`, **skip** `qwantum-*` dump atoms, load/acknowledge the rest, and smoke via **`refined-boot-demo`**. Linux SoT. Not a full Forth `INCLUDE` VM on Win/Android.

Wave11 **4** (`APPIMAGE-REFINED.md`): host AppImage path must **match tools/ SoT** — portable seed into live evolve when fixture/qwantum missing; prior tools-only AppImage gate **superseded**.

## 2. Policy

| Rule | Detail |
|------|--------|
| Dir | `$EVOLVE/forth/refined/` (Linux: under evolve_dir; typically `~/.tritiumos/evolve/…` or repo `evolve/`) |
| Include | `*.fs` only; **regular files** (`S_ISREG`) — skip fifo/socket (wave11 **4**) |
| Skip | names starting `qwantum-` — dump atoms, never live vocab (`[VM] skip …`) |
| Load | print `[VM] include <name> (persisted refined)`; stub-ack `:` lines OK |
| Empty OK | zero non-qwantum files → demo still PASS with `loaded=0` **if** skip path proven |
| Fixture | Shipper seeds **`refined-boot-fixture.fs`** under repo `evolve/forth/refined/`; host must **also** portable-seed into live evolve when missing (see `APPIMAGE-REFINED.md`) |

## 3. Surfaces

| Surface | Behavior |
|---------|----------|
| `load-refined` / `load_refined_modules` | Existing host path; call on persist load / cold boot |
| `refined-boot` / `refined-boot-report` | Print dir, per-file include\|skip, `loaded=<n> skipped=<n>` |
| `refined-boot-demo` | See §5; AppImage Lab contract in `APPIMAGE-REFINED.md` |

## 4. Markers

```
[refined-boot] dir=<path>
[refined-boot] include=<name>
[refined-boot] skip=<name> reason=qwantum-dump
[refined-boot] loaded=<n> skipped=<n>
[VM]   include <name> (persisted refined)     # existing OK
[VM]   skip <name> (dump atom — not vocab; refine only)
[refined-boot-demo] OK
[refined-boot-demo] FAIL
```

Lab greps `[refined-boot-demo] OK` plus either `include=` ≥1 **or** explicit `loaded=0` with a greppable `skip=qwantum-sample.fs` (or fixture include). AppImage gate (wave11 **4**) requires include≥1 **and** skip≥1 after portable seed.

## 5. `refined-boot-demo`

1. Ensure evolve refined dir exists (mkdir OK).
2. Ensure `qwantum-sample.fs` remains skippable — expect skip marker (seed into live if missing; do not require repo cwd).
3. Ensure **one** non-qwantum fixture `.fs` present (commit `evolve/forth/refined/refined-boot-fixture.fs` if missing; host portable-seeds live copy), e.g.:

```forth
\ wave10 tip4 fixture — cold-load smoke
: refined-boot-fixture-ok ." [refined] fixture ok" cr ;
```

4. Run `load_refined_modules` / `refined-boot` → `include=refined-boot-fixture.fs`, `skip=qwantum-sample.fs`, `loaded≥1`, `skipped≥1`.
5. Prior `host-boot-demo` + `fold-demo` + `control-demo` still OK.
6. `[refined-boot-demo] OK`.

Win/Android: CONTRACT acceptable (parity line `refined-boot-demo CONTRACT`) — no full VM required.

## 6. Fixture file (Shipper lands in tip)

Path: `evolve/forth/refined/refined-boot-fixture.fs`
Contents: minimal colon stub as §5 (Research may also drop a copy under `/workspace/tritium-research-docs/fixtures/` for byte reference).

## 7. Thin amend — `docs/HOST-BOOT.md`

- §1 / non-goals: refined cold-load is wave10 **4** — cite `REFINED-BOOT.md` (done this tip).
- Companions: add `REFINED-BOOT.md`.

## 8. Non-goals

- Executing refined bodies on Win/Android VM
- Auto-running qwantum dumps as vocab
- Docs cites refresh (wave10 **5** / wave11 **5**)
- Changing `boot.fs` poly/core order
- Merge; Mango

## 9. Acceptance (Test Lab)

1. `docs/REFINED-BOOT.md` present (Research byte-copy OK); HOST-BOOT thin amend; fixture `.fs` present.
2. Linux: `refined-boot-demo` → OK (include + skip markers) via **tools/** and **host AppImage** (wave11 **4** / `APPIMAGE-REFINED.md`). Prior tools-only AppImage exemption **superseded**.
3. Win/Android CONTRACT OK if used.
4. Regression green (wave10 **1–3**; wave11 **1–3** when on tip).
5. No merge.

## 10. Cite

- `install/hosts/linux/tritiumos.c` (`load_refined_modules`, `refined_boot_demo`, `refined_boot_resolve_dir`)
- `docs/HOST-BOOT.md`, `docs/REKIA.md`, `docs/ASSIMILATE.md`, `docs/APPIMAGE-REFINED.md`
- `evolve/forth/refined/`, `tools/refined-boot-demo`
