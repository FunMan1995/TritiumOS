# APPIMAGE-REFINED — Host `refined-boot-demo` matches tools/ (portable seed)

**Status:** Shipper-ready stub spec (wave11 item **4**)
**Canonical brief:** Wave10 tip4 (#45 @ `9f634c6`) landed REFINED-BOOT + tools/ green; AppImage hang non-blocking then — **this tip closes it**
**Sources of truth (code):** Linux `refined_boot_demo` / `refined_boot_resolve_dir` / `load_refined_modules` in `install/hosts/linux/tritiumos.c`; `tools/refined-boot-demo`; `dist/TritiumOS.AppImage`
**Companions:** `docs/REFINED-BOOT.md` (thin amend this tip), `docs/HOST-BOOT.md`, `docs/HOST-PARITY.md`

## 1. Purpose

`tools/refined-boot-demo` already seeds fixture + qwantum skip when missing and PASSes. The **AppImage host path does not**: from a non-repo cwd (e.g. `/tmp`), live `~/.tritiumos/evolve/forth/refined/` may hold only `refined-1.fs` → `find_repo_refined_dir` fails → `[refined-boot] dir=MISSING` + `[refined-boot-demo] FAIL`. Without `APPIMAGE_EXTRACT_AND_RUN=1`, FUSE/`libfuse.so.2` fails (Lab hang/timeout class). This tip makes **host AppImage path match tools/ SoT**: portable seed into live evolve, scan safety, and a non-REPL Lab contract. Linux SoT. Tip **5** is docs cites after.

## 2. Surfaces

| Surface | Behavior |
|---------|----------|
| `refined_boot_resolve_dir` / `refined_boot_demo` | **Portable seed** into live evolve when fixture and/or qwantum sample missing (see §3) — never require repo cwd for PASS |
| `refined_boot_scan_dir` / `load_refined_modules` | Skip non-regular files (`S_ISREG`) before `fopen`/`fgets` |
| Host CLI `--oneshot <cmd>` **or** `TRITIUM_ONESHOT=<cmd>` | Prefer: run one REPL command then exit 0/1 from OK/FAIL (no REPL wait). Fallback Lab contract: pipe + EXTRACT (see §5) |
| `tools/refined-boot-demo` | Unchanged SoT; must stay green (regression) |

## 3. Portable seed (Shipper — concrete)

When live `$EVOLVE/forth/refined/` lacks required files, **write them into live evolve** (same spirit as `tools/refined-boot-demo`):

1. **`refined-boot-fixture.fs`** — if missing, copy from repo/AppDir share if available; else write minimal colon stub:

```forth
\ wave10 tip4 fixture — cold-load smoke
: refined-boot-fixture-ok ." [refined] fixture ok" cr ;
```

2. **`qwantum-sample.fs`** — if missing, copy from repo/AppDir if available; else write minimal stub whose name/`qwantum-` prefix guarantees skip:

```forth
\ TritiumOS fragment dumped from qwantum field
: qwantum-ok ." field dump ok" cr ;
```

3. Prefer seeding inside `refined_boot_resolve_dir` (so report + demo + cold-load share the path); `refined_boot_demo` must PASS even when `find_repo_refined_dir` returns 0.
4. Optional nice-to-have: ship fixture + qwantum under AppDir share as copy source — **not required** if live seed works.
5. Never depend on repo cwd for AppImage gate PASS.

## 4. Scan safety

In `refined_boot_scan_dir` (and any twin that `fgets`s `*.fs`):

- `stat` each candidate; **skip** unless `S_ISREG(st.st_mode)`.
- A fifo/socket named `*.fs` must not block `fgets` (hang class).
- tools/ already uses `find -type f`; host must match.

## 5. AppImage Lab invocation

**Required env for gate:** `APPIMAGE_EXTRACT_AND_RUN=1` (no FUSE). Document in this tip + BUILD note if touched.

**Prefer oneshot** (Shipper adds if small):

```
APPIMAGE_EXTRACT_AND_RUN=1 ./dist/TritiumOS.AppImage --oneshot refined-boot-demo
# or: TRITIUM_ONESHOT=refined-boot-demo APPIMAGE_EXTRACT_AND_RUN=1 ./dist/TritiumOS.AppImage
```

Exit **0** on `[refined-boot-demo] OK`, **1** on FAIL. Lab never waits on REPL.

**Fallback Lab contract** (if oneshot too big this tip):

```
printf 'refined-boot-demo\nquit\n' | APPIMAGE_EXTRACT_AND_RUN=1 ./dist/TritiumOS.AppImage
```

Demo must complete within **~30s**. Lab greps `[refined-boot-demo] OK` plus `include=` ≥1 and `skip=` ≥1.

**Without EXTRACT_AND_RUN:** fail fast with a greppable FUSE/`libfuse` message (not hang). **Lab only gates with EXTRACT set** — do not require bare AppImage PASS this tip.

## 6. Markers

```
[refined-boot] dir=<path>          # live evolve path OK; not MISSING after seed
[refined-boot] include=<name>
[refined-boot] skip=<name> reason=qwantum-dump
[refined-boot] loaded=<n> skipped=<n>
[refined-boot-demo] OK
[refined-boot-demo] FAIL
```

Lab greps `[refined-boot-demo] OK` with `include=` ≥1 and `skip=` ≥1 (fixture + qwantum).

## 7. Thin amend — `docs/REFINED-BOOT.md`

- Status / cite: wave11 **4** (AppImage host path = tools/ SoT).
- Companions: add `APPIMAGE-REFINED.md`.
- §Acceptance: AppImage host path must PASS (not tools-only); prior tools-only AppImage gate **superseded**.
- Note portable seed + EXTRACT Lab contract (point here).

## 8. Non-goals

- Full Win/Android Forth VM (stay CONTRACT)
- Crypto / network fleet
- Docs cites refresh (wave11 **5**)
- Merge; Mango
- Requiring bare AppImage without `APPIMAGE_EXTRACT_AND_RUN=1`

## 9. Acceptance (Test Lab)

1. `docs/APPIMAGE-REFINED.md` present (Research byte-copy OK) + `REFINED-BOOT.md` thin amend.
2. `./tools/refined-boot-demo` → OK (regression).
3. Rebuild AppImage; from a **non-repo cwd** (e.g. `/tmp`): `APPIMAGE_EXTRACT_AND_RUN=1` oneshot **or** pipe `refined-boot-demo` → `[refined-boot-demo] OK` with include≥1 and skip≥1 within ~30s.
4. Without EXTRACT: fail fast (greppable) **or** Lab only gates with EXTRACT set — document which (this tip: Lab gates with EXTRACT).
5. Prior wave11 **1–3** + wave10 demos still green.
6. Win/Android stay CONTRACT; no merge.

## 10. Cite

- `install/hosts/linux/tritiumos.c` (`refined_boot_demo`, `refined_boot_resolve_dir`, `load_refined_modules`)
- `tools/refined-boot-demo`, `tools/build-linux.sh` (`APPIMAGE_EXTRACT_AND_RUN=1`)
- `docs/REFINED-BOOT.md`, `docs/HOST-BOOT.md`
- Base tip SHA: `e81aca7` (PR #49 tip / wave11 tip3)
