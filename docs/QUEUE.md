# QUEUE — Collective cue (local stub)

**Status:** Shipper-ready spec (wave6 item **1**; matches PR #21 queue stub)  
**Canonical brief:** `TritiumOS.txt` §5b.1  
**Sources of truth (code):** `forth/tritium/queue.fs`; Linux host queue path in `install/hosts/linux/tritiumos.c`; `queue/README.txt`  
**Companions:** `docs/ASSUMPTIONS.md` (queue stub), `docs/ASSIMILATE.md` (wave6 **2** — prove → merge), `docs/LICENSE.md` (worker-key later)

## 1. Purpose

When a job cannot run on this host now (`queue-local?` false), TritiumOS **must not drop the work** — it appends to the **collective cue**. This wave is a **local cue stub**: in-memory Forth table + host persist under `evolve/queue/`. No fleet network, no real crypto / proof verification.

Later: licensed fleet workers `queue-pull` / `queue-prove!` and feed Assimilate (§5b.2).

## 2. Words

| Word | Stack | Notes |
|------|-------|-------|
| `queue-local?` | `( job -- flag )` | Can this host run it now? |
| `queue-make` | `( submitter payload why local-ok -- job-id )` | Mint pending job (stub helper) |
| `queue-enqueue!` | `( job -- )` | Ensure pending; or mint from payload id (non-local) |
| `queue-pull` | `( -- job\|0 )` | Next pending → active; 0 if empty |
| `queue-prove!` | `( job proof -- score )` | Mark proved; stub score = fold(proof, id); no crypto |
| `queue-demo` | `( -- )` | Smoke: non-local → pull → prove |

### 2.1 Status / why codes (stub)

| Status | Constant |
|--------|----------|
| pending | `Q-PENDING` |
| active | `Q-ACTIVE` |
| proved | `Q-PROVED` |
| rewarded | `Q-REWARDED` |

| Why (local-failed) | Constant |
|--------------------|----------|
| OOM | `Q-WHY-OOM` |
| timeout | `Q-WHY-TIMEOUT` |
| edition mismatch | `Q-WHY-ED32` |
| opt-in / forced non-local | `Q-WHY-OPTIN` |

Cap: `MAX-QUEUE` = 16 (Forth in-memory).

## 3. Persist

Path: `evolve/queue/jobs.jsonl` (JSON lines; runtime under host evolve dir).

Fields (v1): job-id, submitter, payload, local-failed-why, proof-hash, status (± local-ok).

Host is Test Lab SoT for Linux smoke; Forth mirrors the word contract.

## 4. Smoke

```
queue-demo
\ expect: [queue-demo] OK — local cue (evolve/queue/; no fleet crypto)
```

Forced path: `queue-local?` false → pull → prove with stub score.

## 5. Out of scope (this stub)

- Fleet network / remote pull
- Real proof crypto / master settlement
- Automatic `assimilate-merge!` after prove (see ASSIMILATE; demos may stand alone)
- Worker-key gating (LICENSE / MASTER later)

## 6. Acceptance (Test Lab — docs tip)

1. `docs/QUEUE.md` present; ASSUMPTIONS may keep §5b.1 cite.
2. `queue-demo` still OK; poly + suite green.
3. Docs-only tip: no Forth behavior change required for item **1** alone.

## 7. Cite

- `TritiumOS.txt` §5b.1
- `docs/ASSUMPTIONS.md`; `queue/README.txt`
- `forth/tritium/queue.fs`; `install/hosts/linux/tritiumos.c`
