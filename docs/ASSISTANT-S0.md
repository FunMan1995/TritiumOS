# ASSISTANT-S0 — Deepen free-text S0 (neuron-count + no scaffold)

**Status:** Shipper-ready stub spec (wave11 item **2**)
**Canonical brief:** `TritiumOS.txt` §§1, 5a; `docs/ASSISTANT.md` (wave4 S0)
**Sources of truth (code):** Linux `s0_assist` / `s0_assist_demo` in `install/hosts/linux/tritiumos.c`; Win/Android twins
**Companions:** `docs/ASSISTANT.md` (thin amend this tip), `docs/ASSISTANT-STATE.md`, `docs/REKIA.md`, `docs/DRENA.md`

## 1. Purpose

Linux already routes free-text through `s0_assist` → drena-step stand-in → refine write+include. This tip **deepens** that path: print a greppable **neuron-count**, keep **scaffold strings forbidden**, and add **`assistant-s0-demo`** (alias/wrapper OK) that Lab greps. Not a chat LLM; not full Win/Android Forth VM.

## 2. Pipeline (unchanged contract)

```
free-text
  → [S0] assist: <query>
  → drena-step (host stand-in OK)
  → rekiA-refine / write+include evolve/forth/refined/<label>.fs
  → assistant-state! + graph-save
  → [S0] assist done — refined word live neurons=<n>
```

`<n>` = stub neuron count after the step (host counter / graph size; **≥1** after a successful assist that spawned).

## 3. Words / surfaces

| Surface | Notes |
|---------|-------|
| `s0_assist` / free-text REPL | Existing; amend done-line to include `neurons=<n>` |
| `s0-assist-demo` | Keep green; may share body with `assistant-s0-demo` |
| `assistant-s0-demo` | **This tip’s Lab primary** — see §5 |

## 4. Markers

```
[S0] assist: <query>
[REKIA] wrote+include …
[S0] assist done — refined word live neurons=<n>
[assistant-s0-demo] OK
[assistant-s0-demo] FAIL
```

Optional alias line: `[assistant-s0] neurons=<n>` (Lab greps either `neurons=` on done-line **or** this alias).

**Forbidden** anywhere in the assist reply path (demo FAIL if present):

- `scaffold reply`
- `connect R.E.K.I.A. next`
- `Example response`

## 5. `assistant-s0-demo`

1. Run fixed query `hello tritium` through S0 (same as `s0-assist-demo`).
2. Assert `refined-1.fs` (or labeled refined) written + live flag.
3. Assert greppable `neurons=` with integer ≥1.
4. Assert output has **no** forbidden scaffold strings (§4).
5. Prior `s0-assist-demo` / `assistant-state-demo` / `loop-demo` still OK.
6. Print `[assistant-s0-demo] OK`.

Win/Android: prefer real S0 twin; **CONTRACT** only if host still cannot run assist — but then parity file must **not** claim scaffold is OK; document CONTRACT = markers deferred, not scaffold allowed.

## 6. Thin amend — `docs/ASSISTANT.md`

- Status: cite wave11 **2**.
- Companions: add `ASSISTANT-S0.md`.
- §3.1 done-line: add `neurons=<n>`.
- §3.2 / §4: add `assistant-s0-demo`.
- Acceptance: Lab smokes `assistant-s0-demo`.

## 7. Non-goals

- Real LLM / opaque-weight replies
- Full GUI chat chrome
- `WORDS` vocab list (wave11 **3**)
- AppImage refined hang (wave11 **4**)
- Crypto / fleet network

## 8. Acceptance (Test Lab)

1. `docs/ASSISTANT-S0.md` present (Research byte-copy OK); `ASSISTANT.md` thin amend present.
2. Linux: `assistant-s0-demo` → OK (`neurons=` + no scaffold); `s0-assist-demo` still OK.
3. Regression green (wave11 **1** + wave10 demos).
4. No merge.

## 9. Cite

- `docs/ASSISTANT.md`, `docs/ASSISTANT-STATE.md`, `docs/REKIA.md`
- `install/hosts/linux/tritiumos.c` (`s0_assist`)
- `TritiumOS.txt` §1 S0
