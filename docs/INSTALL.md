# INSTALL — Bootstrap & host install checklist

**Status:** Shipper-ready overview (wave6 item **5**)  
**Canonical brief:** `TritiumOS.txt` §§5a.1–5a.2, §11  
**Companions:** `docs/BUILD.md` (compile recipes), `docs/LICENSE.md`, `docs/ASSISTANT.md`, `docs/INTEGRATE.md`, `docs/ARCHITECTURE.md`  
**Non-goal:** Duplicate full build command tables — keep those in `BUILD.md`.

## 1. Intent

Users install a **personal assistant** on a Priority-1 host, complete **bootstrap** (license → name → edition), then may **develop** on-device and later **integrate** additional platforms (`INTEGRATE.md`). Ship targets:

| Artifact | Host |
|----------|------|
| `dist/TritiumOS.exe` | Windows 11 |
| `dist/TritiumOS.apk` | Android (Pixel / emulator) |
| `dist/TritiumOS.AppImage` | Linux (on-demand portable) |

Build steps: `docs/BUILD.md`. This doc is the **install / first-run / integrate** map.

## 2. Prerequisites (run-time)

| Host | Need |
|------|------|
| Windows | Built `.exe` (or CI artifact); .NET runtime as required by host project |
| Android | `.apk` sideload or emulator; JDK/SDK only for *building* |
| Linux | `.AppImage` (or `install/hosts/linux` binary from `build-linux.sh`) |

Dev license mint (scaffold): `master/mint-license.ps1` — never commit real keys or populated `evolve/license-slots.json`.

## 3. Phase A — Bootstrap (any Priority-1 host)

Order (§5a.2):

1. Install / run the host artifact.
2. Enter **license-key**; register device slot **1..10** (refuse **11** / full — `LICENSE.md`).
3. Extract / load `tritium.poly` core (Forth + D.R.E.N.A. + R.E.K.I.A.).
4. First-run wizard: **name your assistant**; edition **32** or **64**.
5. REPL / S0 assist available (`ASSISTANT.md`).

Checklist (smoke):

- [ ] License accepted; `license-status` (or CLI) shows `N/10`
- [ ] Assistant name persisted; about / banner shows user name + Tritium branding
- [ ] Edition set; spawn / id-width clamped (`edition-demo` OK)
- [ ] Core engines respond (`rekia-demo` / `s0-assist-demo` sample OK)

## 4. Phase B — Develop (on-device)

After bootstrap, user + R.E.K.I.A. may refine words into `evolve/forth/refined/`. Graph / assistant state under `evolve/`. No extra installer step.

## 5. Phase C — Integrate (additional platforms)

See **`docs/INTEGRATE.md`**. Short path:

1. Free license slot (or capacity on same key).
2. `tritium-integrate <platform>` scaffolds from `install/hosts/_template/`.
3. Smoke: `tritium-integrate-demo` → `[tritium-integrate-demo] OK`.

Fleet evolve sync stays optional / later.

## 6. Host tree (as-built)

```
install/hosts/
  windows/     — TritiumOS.exe project
  android/     — APK (komodo / Pixel notes)
  linux/       — tritiumos.c + install.sh → AppImage path
  _template/   — post-bootstrap integrate scaffold
```

Bundle: `tritium.poly` + `manifest.json` (`product_id: tritium` until graduation — `LINEOS.md`).

## 7. Success criteria slice (§11)

| Criterion | Doc / gate |
|-----------|------------|
| First-run names assistant; about shows branding | §3, `ASSISTANT.md` |
| `.exe` / `.apk` install+run | `BUILD.md` + this §3 |
| License slot refuse-11 | `LICENSE.md` |
| `tritium-integrate` + neuron demos on one host | `INTEGRATE.md` + suite |
| Docs present | this file + index in `ARCHITECTURE.md` |

## 8. Acceptance (Test Lab — docs tip)

1. `docs/INSTALL.md` present; README / ARCHITECTURE may one-line cite.
2. Docs-only portion: light suite still green.
3. When tipped with item **4**: also require `tritium-integrate-demo` OK per `INTEGRATE.md`.
4. No merge without explicit ask.

## 9. Cite

- `TritiumOS.txt` §§5a.1–5a.2, Phase 8, §11
- `docs/BUILD.md`, `docs/LICENSE.md`, `docs/ASSISTANT.md`, `docs/INTEGRATE.md`, `docs/ARCHITECTURE.md`
- `install/hosts/_template/README.txt`
