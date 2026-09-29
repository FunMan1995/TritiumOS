# HOST-BOOT — Core load-order markers (toward Forth VM)

**Status:** Shipper-ready stub spec (wave10 item **1**; thin amend wave17 **3** EVALUATE-INCLUDE)
**Canonical brief:** `TritiumOS.txt` Phase 1 / Priority-1 hosts; `tritium.poly/core/boot.fs`
**Sources of truth (code):** `tritium.poly/core/boot.fs` (+ mirrored `forth/`); Linux `install/hosts/linux/tritiumos.c`; Win/Android parity contracts
**Companions:** `docs/HOST-PARITY.md`, `docs/BUILD.md`, `docs/INSTALL.md`, `docs/KERNEL.md`, `docs/REFINED-BOOT.md` (wave10 **4**), `docs/EVALUATE-INCLUDE.md` (wave17 **3**)

## 1. Purpose

Hosts still lack a full embedded Forth VM. This tip lands **greppable boot markers**: declare the poly/core **load order**, report **sources present** + a stub **word/file count**, and smoke via **`host-boot-demo`**. Linux is SoT. Win/Android may be **CONTRACT** (parity file) — not a C#/Kotlin VM rewrite this tip.

Wave17 **3** (`EVALUATE-INCLUDE.md`): Forth-surface `INCLUDE` / `include-mark` is an **echo marker only** — it must **not** redefine poly/core `boot.fs` host `include` load-order or semantics.

## 2. Canonical load order (`boot.fs`)

```
boot.fs
  → trit.fs
  → tritium-kernel.fs
  → drena.fs
  → rekia.fs
  → queue.fs
  → assimilate.fs
  → lineos.fs
  → integrate.fs
  → master.fs
  → fleet.fs
```

Shipper must keep `tritium.poly/core/boot.fs` includes aligned with this list (additions only via later tips). Cold-load of `evolve/forth/refined/*.fs` is **`docs/REFINED-BOOT.md`** (wave10 **4**). Forth `EVALUATE` / `INCLUDE` mark-only echo is **`docs/EVALUATE-INCLUDE.md`** (wave17 **3**) — does not change this include list.

## 3. Policy

| Host | Expectation |
|------|-------------|
| Linux | SoT — walk/list core files; print markers; `host-boot-demo` → OK |
| Windows | Prefer real list if host can see poly/core; else **CONTRACT** line in parity |
| Android | **CONTRACT-only OK** without SDK |

Never fail solely for missing Android SDK.

## 4. Surfaces

| Surface | Behavior |
|---------|----------|
| `host-boot` / `host-boot-report` | Print load order + per-file present/missing + `files=N` (+ optional stub `words≈M`) |
| `host-boot-demo` | Assert all §2 files present under resolved core dir → OK |
| Win/Android | Optional stub cmd; or `parity/HOST-BOOT.txt` / extend `HOST-PARITY.txt` with `host-boot-demo OK\|CONTRACT\|SKIP` |

## 5. Markers

```
[host-boot] core-dir=<path>
[host-boot] order: trit → kernel → drena → rekia → queue → assimilate → lineos → integrate → master → fleet
[host-boot] file=<name> OK|MISSING
[host-boot] sources-loaded files=<n>
[host-boot] words=<n>          # stub count OK (e.g. grep ^: across core) — optional but preferred
[host-boot-demo] OK
[host-boot-demo] FAIL
```

Minimum PASS: every §2 file `OK`; `sources-loaded files=` matches list length; `[host-boot-demo] OK`.

## 6. `host-boot-demo`

1. Resolve core dir (same search as Linux host for `boot.fs` / `tritium.poly/core`).
2. For each file in §2 (after `boot.fs` itself present): print `file=… OK|MISSING`.
3. Fail if any MISSING.
4. Print `sources-loaded` (+ optional `words=`).
5. `[host-boot-demo] OK`.

Prior demos stay green. No requirement to *execute* Forth in Win/Android this tip.

## 7. Non-goals

- Embedding pForth / full C# or Kotlin token VM (later wave)
- Auto-including refined modules — see `docs/REFINED-BOOT.md` (wave10 **4**)
- Forth `EVALUATE` / `INCLUDE` mark-only echo — see `docs/EVALUATE-INCLUDE.md` (wave17 **3**); must **not** redefine host boot `include` / this load order
- Control-flow / address-fold (wave10 **2–3**)
- Changing `boot.fs` semantics beyond include-list alignment

## 8. Acceptance (Test Lab)

1. `docs/HOST-BOOT.md` present (Research byte-copy OK); wave17 **3**: `EVALUATE-INCLUDE.md` + thin amend (host boot include list retained).
2. Linux: `host-boot-demo` → OK (markers §5); all §2 core files present. Wave17 **3**: `eval-demo` → OK (mark-only echo; does not redefine host include).
3. Win/Android: CONTRACT acceptable; if parity file used, `host-boot-demo` listed.
4. Regression green (wave9 demos).
5. No merge.

## 9. Cite

- `tritium.poly/core/boot.fs`
- `docs/HOST-PARITY.md`, `docs/INSTALL.md`, `docs/BUILD.md`
- `docs/EVALUATE-INCLUDE.md` (wave17 **3**)
- `docs/REFINED-BOOT.md` (wave10 **4**)
- `install/hosts/linux/tritiumos.c` (core path resolve)
