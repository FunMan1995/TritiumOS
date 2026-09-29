# HOST-PARITY — Win / Android smoke markers (Linux SoT)

**Status:** Shipper-ready stub spec (wave8 item **4**)  
**Canonical brief:** Priority-1 hosts; Linux demos are Lab source of truth  
**Companions:** `docs/BUILD.md`, `docs/INSTALL.md`, `docs/KERNEL.md`, `docs/FLEET.md`, `docs/MASTER.md`, `docs/LINEOS-BRAND.md`, `docs/GROUPS-NESTED.md`, `docs/INTERPRET.md`, `docs/USERLAND.md`, `docs/HOST-BOOT.md`

## 1. Purpose

Wave7–8 demos (`kernel-demo`, `fleet-demo`, `master-demo`, `lineos-brand-demo`, `group-nested-demo` / vocab-persist, `interpret-demo`, `userland-demo`, …) are green on **Linux**. This tip adds **contract markers** (and light .NET smoke where available) so Win / Android report the same greppable OK / SKIP / CONTRACT lines — not a full VM rewrite.

## 2. Policy

| Host | Expectation this tip |
|------|----------------------|
| Linux | SoT — full demos already gate |
| Windows | Prefer real smoke if `TritiumOS.exe` / host REPL can print markers; else **contract** file + stub command |
| Android | **Contract-only OK** if SDK/emulator missing (Lab precedent) |

Never fail the tip solely for missing Android SDK.

## 3. Contract files

```
install/hosts/windows/parity/
  HOST-PARITY.txt      # lists demo → OK|CONTRACT|SKIP
install/hosts/android/parity/
  HOST-PARITY.txt      # same schema
docs/HOST-PARITY.md    # this doc
```

`HOST-PARITY.txt` schema (one demo per line):

```
kernel-demo OK|CONTRACT|SKIP
fleet-demo …
master-demo …
lineos-brand-demo …
group-nested-demo …
group-vocab-persist-demo …
interpret-demo …
userland-demo …
lineos-confirm-demo …
host-boot-demo …   # wave10 item 1; see docs/HOST-BOOT.md
```

Minimum for PASS: file present on Win + Android paths; every listed demo has a status; Linux suite still green.

## 4. Surfaces

| Surface | Behavior |
|---------|----------|
| `host-parity-demo` | Linux: print SoT note + verify both parity files exist in tree → OK |
| Win stub | Optional: print `[host-parity] windows CONTRACT` reading the txt |
| Android stub | Optional: same with `android` |

## 5. Markers

```
[host-parity] linux SoT
[host-parity] windows OK|CONTRACT|SKIP
[host-parity] android OK|CONTRACT|SKIP
[host-parity-demo] OK
[host-parity-demo] FAIL
```

## 6. Non-goals

- Porting every Forth demo into C#/Kotlin this tip
- Requiring emulator/SDK in CI
- Changing Linux SoT behavior

## 7. Acceptance (Test Lab)

1. `docs/HOST-PARITY.md` present (Research byte-copy OK).
2. Win + Android `parity/HOST-PARITY.txt` present with schema.
3. `host-parity-demo` → OK; Linux regression green.
4. Android CONTRACT-only acceptable without SDK.
5. No merge.

## 8. Cite

- `docs/BUILD.md`, `docs/INSTALL.md`, `docs/SYSTEM-DESIGN-INITIAL-PLATFORMS.md`
- Wave7–8 demo docs listed in §1 companions
