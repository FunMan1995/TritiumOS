# ECONOMY-WIRE — queue-prove! → assimilate-merge! (e2e stub)

**Status:** Shipper-ready stub spec (wave9 item **2**)  
**Canonical brief:** `TritiumOS.txt` §5b.4  
**Sources of truth (code):** `forth/tritium/queue.fs`, `forth/tritium/assimilate.fs`; Linux host / `tools/` demo mirrors  
**Companions:** `docs/QUEUE.md`, `docs/ASSIMILATE.md` (thin amend this tip)

## 1. Purpose

Wave6 landed **standalone** `queue-demo` and `assimilate-demo`. Spec §5b.4 requires the handoff:

`queue-local?` false → enqueue → pull → `queue-prove!` → `assimilate-merge!` → (later) master settle.

This tip wires that path in one greppable **`economy-wire-demo`**. Still local stub — no fleet network, no real crypto, no master epoch settlement.

## 2. Words

| Word | Stack | Notes |
|------|-------|-------|
| existing queue / assimilate words | — | unchanged contracts (`QUEUE.md`, `ASSIMILATE.md`) |
| `economy-wire-demo` | `( -- )` | See §4; alias `queue-assim-demo` OK if both print same OK marker |

Optional helper (nice-to-have, not required):

| Word | Stack | Notes |
|------|-------|-------|
| `queue-reward!` | `( job -- )` | Mark `Q-REWARDED` after successful merge (stub print OK) |

## 3. Wire contract

1. Mint/enqueue a **non-local** job (`queue-local?` would be false / `Q-WHY-OPTIN`).
2. `queue-pull` → active job id `J`.
3. Choose stub proof `P` (nonzero constant OK).
4. `J P queue-prove!` → score `S` with `S > 0`; job status `Q-PROVED`.
5. Build frag: `assimilate-fragment` from job payload (or fixed group/links derived from `J`).
6. `frag P assimilate-merge!` → `delta > 0` and wallet simti increases (first proof).
7. Optional: `queue-reward!` / status → `Q-REWARDED`.
8. Duplicate `P` merge still → 0 credit (anti-gaming preserved).

Do **not** require network or worker-key gate this tip.

## 4. Markers

```
[economy-wire] enqueue job=<id>
[economy-wire] prove score=<n>
[economy-wire] merge delta=<n> credit-ok
[economy-wire-demo] OK
[economy-wire-demo] FAIL
```

If aliasing `queue-assim-demo`, also print `[economy-wire-demo] OK` (Lab greps that).

## 5. `economy-wire-demo`

1. Reset queue cue + assimilate wallet/epoch/proofs (same hygiene as existing demos).
2. Run §3 steps 1–7.
3. Assert: prove score > 0; wallet simti > 0 after merge; greppable markers.
4. Print `[economy-wire-demo] OK`.

Prior `queue-demo` and `assimilate-demo` must stay green (standalone paths remain).

## 6. Thin doc amends (Shipper)

**`docs/QUEUE.md`**

- Companions: cite `ECONOMY-WIRE.md` (wave9 **2**).
- Remove / soften out-of-scope bullet “Automatic `assimilate-merge!` after prove”.
- Add one line under Smoke: e2e via `economy-wire-demo`.

**`docs/ASSIMILATE.md`**

- §5 Handoff: mark wired by `economy-wire-demo` (wave9 **2**); standalone `assimilate-demo` still OK.
- Acceptance: Lab also smokes `economy-wire-demo`.

Research folder copies of QUEUE/ASSIMILATE may carry the same thin edits for byte-cmp if Shipper prefers Research SoT for those two files this tip; otherwise patch tip copies only.

## 7. Non-goals

- Fleet pull / remote workers
- Real proof crypto / master settlement
- Changing ASIM scale or stub credit formula
- trit-math / assistant-state / colon (wave9 **1** done; **3–4** later)

## 8. Acceptance (Test Lab)

1. `docs/ECONOMY-WIRE.md` present (Research byte-copy OK); QUEUE/ASSIMILATE thin amends present.
2. `economy-wire-demo` → OK (markers §4); `queue-demo` + `assimilate-demo` still OK.
3. Regression green (`trit-math-demo`, interpret, …).
4. No merge.

## 9. Cite

- `TritiumOS.txt` §5b.4
- `docs/QUEUE.md`, `docs/ASSIMILATE.md`
- `forth/tritium/queue.fs`, `forth/tritium/assimilate.fs`
