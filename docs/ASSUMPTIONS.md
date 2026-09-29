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

group-vocab-demo
\ expect: [group-vocab-demo] OK — find under GROUP-demo/

group-link-demo
\ expect: [group-link-demo] OK — inter-group bridge

group-nested-demo
\ expect: [group-nested-demo] OK — nested find via LINK-INTER

group-vocab-persist-demo
\ expect: [group-vocab-persist-demo] OK — vocab restored from graph

kernel-demo
\ expect: [kernel-demo] OK ; greppable [kernel] find miss + find hit

queue-demo
\ expect: [queue-demo] OK — local cue (evolve/queue/; no fleet crypto)

assimilate-demo
\ expect: [assimilate-demo] OK — simti credited (evolve/assimilate/; no crypto)

lineos-graduate-demo
\ expect: [lineos-graduate-demo] OK

tritium-integrate-demo
\ expect: [tritium-integrate-demo] OK

master-demo
\ expect: [master-demo] OK

fleet-demo
\ expect: [fleet-demo] OK

lineos-brand-demo
\ expect: [lineos-brand-demo] OK — slogan + L.I.N.E.O.S. + edition markers

s3-reserved-demo
\ expect: before=3, rewire skipped, after=3

grow-step-demo
\ expect: spawn0→grow→step; RESERVED grow child-mode=3; step skipped; after=3

qwantum-atoms-demo
\ expect: OK — refined written; dump not vocab

edition-demo
\ expect: OK — edition=… spawn works; id clamped on 32

s0-assist-demo
\ expect: [S0] assist: …; [REKIA] wrote+include …/refined-1.fs; OK — refined written; word live
\ free-text (e.g. hello tritium) → same S0 path; no scaffold / "Example response"
```

## S0 assistant path (free-text)

- Host default chat/REPL free-text routes to **drena-step + rekiA-refine** (Linux `s0_assist`; Win `S0Assist`; Android `s0Assist`), not scaffold / "Example response" / parity-print-only stubs.
- Emits `evolve/forth/refined/refined-*.fs` + `[REKIA] wrote+include` / live vocab (same artifact contract as `rekia-demo`).
- Demo grep (all Priority-1 hosts): `[s0-assist-demo] OK — refined written; word live`
- Headless Win twin smoke (no WinForms): `dotnet run --project install/hosts/windows/smoke-s0`

## Trit nibble packing

Default remains mod-3 split (`trit-pair@` / `encode-trit`). Dense 2+2 packing is out of scope unless recorded here later.

## Persist files

- `evolve/user-graph.trit` — neuron headers + typed links snapshot after refine; also `group <gid> <label>` / `member <gid> <nid>` / `vocab <gid> <word-name>` (v2 additive; v1/legacy still load). See `docs/GROUPS-NESTED.md`.
- `evolve/assistant-state.trit` — touched after `rekiA-refine` (last-refine label/path).
- `evolve/edition.trit` — `edition=32|64`; host load on start / save on `set_edition`.
- Reload on host start; refined `.fs` under `evolve/forth/refined/` stay live vocab.
- **Groups across AppImage restart:** `load_user_graph` on boot restores host labels + member lists, reprints `GROUP-<label>/` prefixes, and re-registers each as a searchable vocab unit (Linux host is source of truth for demos).
- **Group vocab unit:** `GROUP-<label>/` is findable via `group-find` / host `host_group_entry_find` under that gid (wave4 item 2; Dusk-style), not prefix-only.

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


## Edition id width (32 / 64)

- Persist: `evolve/edition.trit` holds `edition=32` or `edition=64` (Linux host reads on boot, writes on `set_edition`).
- Kernel: `ARCH` / `edition@` / `32bit?` / `64bit?` / `set-edition` (`forth/tritium/kernel.fs`).
- D.R.E.N.A.: `id-width` / `id-mask` / `id-clamp` — 32-bit edition masks neuron ids and `next-id` with `$ffffffff and`; 64-bit keeps the full cell.
- Default remains **64** when unset (`cold-boot` / host).
- Trit packing and S3 policy are edition-independent (`TritiumOS.txt` § Editions).

Smoke:

```
status
\ expect: edition=64-bit (default)

edition-demo
\ expect: [edition-demo] OK — edition=… spawn id=… ; spawning works

\ optional: edition 32 → edition-demo → spawn still OK; edition 64 restores default
```

## Kernel (minimal dict)

- Bundle load / Forth seed prints `Tritium kernel loaded (minimal dict)` (not the old “study Dusk for full bootstrap” stub line).
- `cold-boot` → `[kernel] cold-boot OK` + interpret-ready; soft `abort` → `[kernel] abort` then return (AppImage demos must not hang).
- Dict: `entry-create` / `entry-find` / `words` — in-memory table; `ENTRY-GIDS` + `group-entry-find` / `group-entry-create` scope words under a group (wave4 item 2).

## Shared license slots (§5a.4)

- Max **10** active device registrations per shared license key (`TritiumOS.txt` §5a.4).
- Registry: `evolve/license-slots.json` (created if missing; runtime — not committed).
- **Installer / CLI refuses slot 11** (and any registration when already 10/10).
- CLI: `tools/tritium-license status` → prints `N/10`; `register <device-id>` registers or refuses.
- Scaffold only (no crypto signing yet); `master-verify` is format-only — see `docs/MASTER.md`; `license/validator.ps1` mirrors the slot gate.
- Linux host REPL: `license-status`, `license-register <id>` (optional; same refuse-11 contract).

Smoke:

```
tools/tritium-license clear
tools/tritium-license register dev1   # … through dev10
tools/tritium-license register dev11  # expect refuse slot 11
tools/tritium-license status          # expect 10/10
```

## Collective queue stub (§5b.1)

- Local cue only: `queue-local?` / `queue-enqueue!` / `queue-pull` / `queue-prove!` + `queue-demo`.
- Persist: `evolve/queue/jobs.jsonl` (JSON lines; job-id, submitter, payload, local-failed-why, proof-hash, status).
- No fleet network, no real crypto / proof verification beyond stub score fold.
- Forth: `forth/tritium/queue.fs` (poly + Android asset mirrors); Linux host SoT for Test Lab.
- Cite: `docs/QUEUE.md`, `TritiumOS.txt` §5b.1.

## Assimilate stub (§5b.2–5b.3)

- Collective neural puzzle stub: `assimilate-epoch` / `assimilate-fragment` / `assimilate-merge!` / `assimilate-solved?` / `assimilate-balance` + `assimilate-demo`.
- Currency: balances as integer **simti**; **1 ASIM = 10⁸ simti**; stub epoch pool **10⁶ simti**.
- On merge with Δε improvement → credit local wallet under `evolve/assimilate/` (puzzle.state + wallet/).
- Duplicate proof-hash → zero credit (anti-gaming stub). No real crypto / fleet / master settlement.
- Forth: `forth/tritium/assimilate.fs` (poly + Android asset mirrors); Linux host SoT for Test Lab.
- May call after `queue-prove!` conceptually; `assimilate-demo` stands alone.
- Cite: `docs/ASSIMILATE.md`, `TritiumOS.txt` §5b.2–5b.3.

## L.I.N.E.O.S. graduation stub (§1a.1)

- Thresholds: `evolve/graduation.json` (runtime create with defaults) + committed `evolve/graduation.json.example` / `lineos/graduation.json.example`.
- Words/CLI: `lineos-graduate` / `lineos-graduate-demo` / `become-lineos`.
- On success (scaffold only): host flag → **L.I.N.E.O.S.**; write `evolve/lineos-manifest-scaffold.json` with `product_id=lineos`; preserve edition; slogan printed. **Not** a production branding release.
- Demo: `demoForceReady` / force-ready path so smoke PASSes without 30 sessions; prints each gate PASS/FAIL; greppable `[lineos-graduate-demo] OK`.
- Forth: `forth/tritium/lineos.fs` (poly + Android asset mirrors); Linux host SoT for Test Lab.
- Brand markers (wave7 item **3**): `evolve/lineos/{brand.json,SPLASH.txt,ABOUT.txt}` on graduate success; surfaces `lineos-splash` (refuse if not graduated), `lineos-about`, `lineos-brand-demo` → `[lineos-brand-demo] OK`. Slogan *The line tread between madness and genius.*; product `L.I.N.E.O.S.`; origin `TritiumOS by Draco`; preserve edition 32/64. CLI `tools/tritium-lineos`. See `docs/LINEOS-BRAND.md`. No production assets / store rebrand.
- Cite: `docs/LINEOS.md`, `TritiumOS.txt` §1a.1.

## tritium-integrate stub (§5a.2 / Phase 8)

- Scaffold from `install/hosts/_template/` → prefer demo path `evolve/integrate/<platform>/` (gitignored; do not mutate `install/hosts/` in CI).
- Require free device slot (reuse license helpers); refuse when 10/10 or slot 11 — cite §5a.4 / `LICENSE.md`.
- Surfaces: Linux host C SoT + CLI `tools/tritium-integrate`; Forth `forth/tritium/integrate.fs` (+ poly / Android mirrors).
- Demo: force free-slot path → greppable `[tritium-integrate-demo] OK`.
- Cite: `docs/INTEGRATE.md`, `docs/INSTALL.md`, `TritiumOS.txt` §5a.2 / Phase 8.

## Master mint/verify stub (§5a.5)

- Surfaces: `master-mint-license` (default slots=10) → `TRIT-<16hex>-DRACO`; `master-mint-worker <device-id>` → `TRIT-W-<idhash>-DRACO` (refuse empty); `master-verify` format-only; `master-demo` → `[master-demo] OK`.
- SoT: Linux host C + CLI `tools/tritium-master` + Forth `forth/tritium/master.fs` (poly / Android mirrors). Windows helper: `master/mint-license.ps1`.
- Optional write under `evolve/master/` (gitignored); never commit keys.
- Non-goals: real crypto, master-root, fleet sync, production escrow.
- Cite: `docs/MASTER.md`, `TritiumOS.txt` §5a.5 / Phase 6.

## Fleet evolve-sync stub (§§5a.2–5a.4 / Phase 8 optional)

- Opt-in local export↔import under `evolve/fleet/` (manifest.json + user-graph.trit + MARKER.txt).
- Surfaces: `fleet-export` / `fleet-import` / `fleet-demo` via `tools/tritium-fleet` + Linux host + Forth `forth/tritium/fleet.fs` (poly / Android mirrors).
- Same-key gate: stamp `keyFingerprint` from current license context; refuse import on mismatch → `[fleet] refuse — key mismatch (§5a.4)`.
- Non-goals: network, encryption, auto-sync, CRDT, LINEOS branding.
- Cite: `docs/FLEET.md`, `docs/LICENSE.md`, `docs/MASTER.md`, `TritiumOS.txt` §§5a.2–5a.4 / Phase 8.

## Kernel find/interpret stub (wave7 item 5)

- `findentry` / `find` aliases of `entry-find`; `interpret-token` lookup-only stub; `kernel-demo` → `[kernel-demo] OK`.
- SoT: Linux host C (shares ENTRY-GIDS / host entry table with groups); Forth `forth/tritium/kernel.fs` (+ poly / Android mirrors).
- Caps: `MAX-ENTRIES` ≥ 32; cold-boot / soft abort unchanged.
- Non-goals: full interpret `:`, linked dict, forget/units, host VM replace.
- Cite: `docs/KERNEL.md`, `docs/GROUPS-NESTED.md`, `docs/ARCHITECTURE.md`, `docs/ASSUMPTIONS.md`.

