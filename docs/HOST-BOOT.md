# HOST-BOOT — Core load-order markers (toward Forth VM)

**Status:** Shipper-ready stub spec (wave10 item **1**)
**Canonical brief:** `TritiumOS.txt` Phase 1 / Priority-1 hosts; `tritium.poly/core/boot.fs`
**Sources of truth (code):** `tritium.poly/core/boot.fs` (+ mirrored `forth/`); Linux `install/hosts/linux/tritiumos.c`; Win/Android parity contracts
**Companions:** `docs/HOST-PARITY.md`, `docs/BUILD.md`, `docs/INSTALL.md`, `docs/KERNEL.md`

## 1. Purpose

Hosts still lack a full embedded Forth VM. This tip lands **greppable boot markers**: declare the poly/core **load order**, report **sources present** + a stub **word/file count**, and smoke via **`host-boot-demo`**. Linux is SoT. Win/Android may be **CONTRACT** (parity file) — not a C#/Kotlin VM rewrite this tip.

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

Shipper must keep `tritium.poly/core/boot.fs` includes aligned with this list (additions only via later tips). Refining `evolve/forth/refined/*.fs` is **wave10 tip 4** (refined-boot), not this tip.

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
- Auto-including refined modules (wave10 **4**)
- Control-flow / address-fold (wave10 **2–3**)
- Changing `boot.fs` semantics beyond include-list alignment

## 8. Acceptance (Test Lab)

1. `docs/HOST-BOOT.md` present (Research byte-copy OK).
2. Linux: `host-boot-demo` → OK (markers §5); all §2 core files present.
3. Win/Android: CONTRACT acceptable; if parity file used, `host-boot-demo` listed.
4. Regression green (wave9 demos).
5. No merge.

## 9. Cite

- `tritium.poly/core/boot.fs`
- `docs/HOST-PARITY.md`, `docs/INSTALL.md`, `docs/BUILD.md`
- `install/hosts/linux/tritiumos.c` (core path resolve)
