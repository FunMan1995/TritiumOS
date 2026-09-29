# USERLAND — Shell / init / demos scaffold

**Status:** Shipper-ready stub spec (wave8 item **3**)  
**Canonical brief:** `TritiumOS.txt` §7 layout (`/userland` — shell, init, demos, `tritium-integrate`)  
**Sources of truth (code):** new top-level `userland/`; demos may wrap existing Linux REPL markers  
**Companions:** `docs/INTEGRATE.md`, `docs/INSTALL.md`, `docs/ARCHITECTURE.md`, `docs/INTERPRET.md`

## 1. Purpose

Spec layout calls for `/userland` (shell, init, demos, integrate). Integrate stub already lives under `tools/` + Forth + host (wave6). This tip **scaffolds** the directory tree + thin init/shell stubs + a presence demo — not a full user-facing shell or bare-metal `/boot`.

## 2. Layout (create if missing)

```
userland/
  README.txt           # one-paragraph purpose + cite this doc
  init/
    README.txt         # cold path notes; may call cold-boot conceptually
    init.txt           # stub checklist (license → edition → REPL)
  shell/
    README.txt         # REPL entry notes (host SoT for now)
    shell.txt          # stub command list pointer (help mirror)
  demos/
    README.txt
    userland-demo.txt  # expected markers / steps for Lab
```

Optional: `userland/integrate.md` one-line pointer to `docs/INTEGRATE.md` / `tools/tritium-integrate` (do not duplicate the stub).

Do **not** move working integrate CLI this tip unless Shipper prefers a symlink note in README.

## 3. Surfaces

| Surface | Behavior |
|---------|----------|
| Tree presence | `userland/{init,shell,demos}/` + READMEs exist in tip |
| `userland-demo` | Host and/or script: assert dirs + key files; print OK |

Linux SoT: add REPL command or `tools/userland-demo` that checks paths relative to repo root / AppDir.

## 4. Markers

```
[userland] init/ OK
[userland] shell/ OK
[userland] demos/ OK
[userland-demo] OK
[userland-demo] FAIL
```

## 5. Non-goals

- Real interactive shell rewriting Linux REPL
- `/boot` bare-metal
- Relocating all demos out of `tritiumos.c`
- Wave8 **4** host parity (separate tip)

## 6. Acceptance (Test Lab)

1. `docs/USERLAND.md` present (Research byte-copy OK); ARCHITECTURE may one-line cite.
2. `userland/` tree present per §2; `userland-demo` → OK.
3. Existing suite green (interpret / confirm / integrate / …).
4. No merge.

## 7. Cite

- `TritiumOS.txt` §7
- `docs/INTEGRATE.md`, `docs/INSTALL.md`, `docs/ARCHITECTURE.md`
