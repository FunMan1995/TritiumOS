# ASSIMILATE — Collective puzzle + ASIM/simti

**Status:** Shipper-ready spec (wave6 item **2**; matches PR #22 assimilate stub)  
**Canonical brief:** `TritiumOS.txt` §5b.2–5b.4  
**Sources of truth (code):** `forth/tritium/assimilate.fs`; Linux host assimilate path; `assimilate/README.txt`  
**Companions:** `docs/QUEUE.md` (prove → merge handoff), `docs/ASSUMPTIONS.md`, `docs/LICENSE.md` (worker-key later)

## 1. Purpose

**Assimilate** is the collective neural puzzle + contribution currency. Partial merges that reduce global error ε earn **simti**; display uses **ASIM** (1 ASIM = 10⁸ simti, Bitcoin:satoshi scale).

This wave is a **local stub**: in-memory Forth wallet/epoch + host persist under `evolve/assimilate/`. No real crypto, no fleet settlement, no master covenant.

## 2. Currency (§5b.3)

| Unit | Symbol | Value |
|------|--------|-------|
| Assimilate | ASIM | 1 ASIM = 100,000,000 simti |
| simti | simti | Indivisible subunit |

- All balances stored as **integer simti**.
- Stub epoch pool (dev): **10⁶ simti** (`ASIM-STUB-POOL`).
- Solve threshold: ε ≤ `ASIM-SOLVE-EPS` (stub **10**) → new epoch.

Rewards are for **Δε contribution**, not mere queue presence.

## 3. Words

| Word | Stack | Notes |
|------|-------|-------|
| `assimilate-epoch` | `( -- id )` | Current epoch id |
| `assimilate-fragment` | `( group links -- frag )` | Stub fold group⊕links → frag id |
| `assimilate-merge!` | `( frag proof -- delta-epsilon )` | Apply Δε; credit wallet; remember proof |
| `assimilate-solved?` | `( -- flag )` | ε low → bump epoch, reset pool/proofs |
| `assimilate-balance` | `( -- )` | Print ASIM + remainder simti |
| `assimilate-demo` | `( -- )` | Smoke: fragment → merge → credit; dup proof → 0 |

### 3.1 Credit (stub)

```
delta     = (frag ⊕ proof) mod 64 + 1
credit    = floor( pool × delta / (delta + ε_before) )  capped by pool
ε_after   = max(0, ε_before − delta)
```

Duplicate **proof-hash** → Δε=0, credit=0 (anti-gaming stub).

## 4. Persist

Under `evolve/assimilate/` (Linux SoT):

| Path | Role |
|------|------|
| `puzzle.state` | Epoch / ε snapshot (stub) |
| `wallet/` / `wallet.json` / `wallet/local.trit` | Local simti balance |

Runtime under host evolve dir; do not commit populated wallets.

## 5. Handoff from queue

Conceptual flow (`TritiumOS.txt` §5b.4):

1. `queue-local?` false → `queue-enqueue!`
2. Worker `queue-pull` → `queue-prove!`
3. `assimilate-merge!` with proof
4. Later: master settles epoch (out of scope)

`assimilate-demo` **stands alone** (does not require queue-demo).

## 6. Smoke

```
assimilate-demo
\ expect: [assimilate-demo] OK — simti credited (evolve/assimilate/; no crypto)
```

Also: second merge with same proof → zero credit; `assimilate-balance` prints epoch + ASIM/simti.

## 7. Out of scope

- Real crypto / master settlement / fleet wallets
- Full contribution_score (link novelty, forth_validity, …)
- Worker-key registration path (LICENSE / MASTER)

## 8. Acceptance (Test Lab — docs tip)

1. `docs/ASSIMILATE.md` present; ASSUMPTIONS / QUEUE may keep one-line cites.
2. `assimilate-demo` (+ optional `queue-demo`) still OK; suite green.
3. Docs-only tip: no Forth behavior change required for item **2** alone.

## 9. Cite

- `TritiumOS.txt` §§5b.2–5b.4
- `docs/QUEUE.md`, `docs/ASSUMPTIONS.md`
- `forth/tritium/assimilate.fs`; `assimilate/README.txt`; Linux host bridge
