# FLEET — Opt-in evolve sync (scaffold)

**Status:** Shipper-ready stub spec (wave7 item **2**)  
**Canonical brief:** `TritiumOS.txt` §§5a.2–5a.4, Phase 8 (optional sync)  
**Sources of truth (code):** stub CLI / Forth / Linux host under `evolve/fleet/`; license same-key gate via `LICENSE.md` helpers  
**Companions:** `docs/LICENSE.md`, `docs/MASTER.md`, `docs/INTEGRATE.md`, `docs/LINEOS.md`

## 1. Purpose

Licensed devices sharing the **same license-key** may **opt-in** to sync evolve state (user-graph + markers). This wave lands a **local stub**: export ↔ import of a fleet blob under `evolve/fleet/`, gated by same-key / slot membership — **no** network, **no** encryption, **no** multi-host daemon.

## 2. Rules

| Rule | Stub behavior |
|------|----------------|
| Same key | Export and import both require a recorded license family / key fingerprint marker |
| Opt-in | Explicit `fleet-export` / `fleet-import` (or demo); never auto-push |
| Scope | Pack `evolve/user-graph.trit` if present + small JSON manifest; placeholders OK if graph missing |
| Refuse | Mismatched key fingerprint → refuse import; empty/missing export path → FAIL markers |
| Secrets | Never pack `license-slots.json` secrets beyond fingerprint; never commit blobs |

## 3. Layout

```
evolve/fleet/
  manifest.json     # keyFingerprint, deviceId, exportedAt, paths[]
  user-graph.trit   # copy or stub placeholder
  MARKER.txt        # human-readable smoke marker
```

Runtime only (gitignored under `evolve/`). Committed example optional: `evolve/fleet/.gitkeep` or `fleet/README.txt`.

Manifest shape (v1 stub):

```json
{
  "keyFingerprint": "TRIT-…-DRACO",
  "deviceId": "dev1",
  "exportedAt": "ISO-8601-or-stub",
  "paths": ["user-graph.trit"]
}
```

## 4. Surfaces

| Surface | Behavior |
|---------|----------|
| `fleet-export` | Write `evolve/fleet/` blob; stamp key fingerprint from current license context |
| `fleet-import` | Read blob; **same-key** check; restore graph/markers into evolve/ |
| `fleet-demo` | Export → import same key → OK; then import with wrong fingerprint → refuse |

Suggested SoT: `tools/tritium-fleet` and/or Forth `forth/tritium/fleet.fs` + Linux REPL mirrors (same markers).

## 5. Markers

```
[fleet] export → evolve/fleet/ key=<fingerprint>
[fleet] import OK same-key
[fleet] refuse — key mismatch (§5a.4)
[fleet-demo] OK
[fleet-demo] FAIL
```

## 6. Non-goals (this tip)

- Network / peer discovery / conflict CRDT
- Encrypted payloads
- Auto-sync on boot
- Cross-product LINEOS branding (wave7 **3**)

## 7. Acceptance (Test Lab)

1. `docs/FLEET.md` present (Research byte-copy OK).
2. `fleet-demo` → OK: same-key export↔import; mismatch refuse marker present.
3. Existing suite green (master-demo, license, integrate, …).
4. No secrets committed; no merge.

## 8. Cite

- `TritiumOS.txt` §§5a.2–5a.4, Phase 8
- `docs/LICENSE.md`, `docs/MASTER.md`, `docs/INTEGRATE.md`
