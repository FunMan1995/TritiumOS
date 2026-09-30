# SEARCH-WORDLIST — `SEARCH-WORDLIST` vocab-search mark + `search-demo`

**Status:** Shipper-ready stub spec (wave22 item **4**)
**Canonical brief:** ANS-shaped `SEARCH-WORDLIST` (thin vocab-search mark only); `docs/FIND.md` (wave18 **2** — ANS-ish find-mark companion; **not** FIND reopen; host `find`/`findentry`/`entry-find` + wave18 `find-xt`/`find-mark` stay untouched); `docs/WORDS-VOCAB.md` (wave11 **3** — flat WORDS list companion; **not** WORDS reopen / linked dict); `docs/KERNEL.md` (wave7 **5**); optional `docs/SYNONYM-ALIAS.md` (wave17 **1** — name map only; **not** SYNONYM FIND rewrite) / `docs/TICK.md` (wave18 **1** — stub `xt=` companion) / `docs/EXECUTE.md` (wave20 **4** — xt-id invoke mark; **not** EXECUTE reopen) / `docs/INTERPRET.md` (wave8 **1**) / `docs/HOST-PARITY.md` (wave8 **4**); WAVE18–22 deferral closed as **thin vocab-search mark only** (not FIND reopen; **not** full linked dict / SEARCH-WORDLIST runtime rewrite; **not** SYNONYM FIND rewrite; **not** EXECUTE reopen; **not** TO-NUMBER reopen; **not** ZERO-EQUALS/LSHIFT reopen; **CRITICAL:** host `find` / `findentry` / `entry-find` + wave18 `find-xt` / `find-mark` stay untouched — do **not** redefine them or Lab-grep them as this tip’s surface).
**Sources of truth (code):** `forth/tritium/kernel.fs` (+ optional `search-wordlist.fs` / `search-wl.fs`); Linux host REPL; prefer Forth mirror **`search-wl-mark`** whenever host `SEARCH-WORDLIST` collides; **CRITICAL — do not** redefine host `find` / `findentry` / `entry-find` / wave18 `find-xt` / `find-mark` (FIND deepen stays landed — **not** FIND reopen); **do not** redefine `WORDS` / `.words` / `words-demo` (WORDS-VOCAB stays list-only sibling); **do not** redefine `synonym-map` / `alias-map` (SYNONYM stays name→name — **not** SYNONYM FIND rewrite); **do not** redefine `execute-mark` / `tick-mark` / `to-number-mark` / `zero-eq-mark` / `lshift-mark`/`rshift-mark`
**Companions:** `docs/FIND.md` (thin amend this tip), `docs/WORDS-VOCAB.md` (thin amend this tip), `docs/KERNEL.md` (thin amend this tip); optional light cite `docs/SYNONYM-ALIAS.md` / `docs/TICK.md` / `docs/EXECUTE.md` / `docs/INTERPRET.md` / `docs/HOST-PARITY.md`
**Base tip SHA:** `b7f6052` (wave22 tip3 PASS / #104 TO-NUMBER) / full `b7f60527c3ace9477b7d554e210b592fc646312b`

## 1. Purpose

WAVE11 tip **3** landed flat `WORDS` list (`docs/WORDS-VOCAB.md`). WAVE18 tip **2** landed ANS-ish `FIND` mark (`docs/FIND.md`; Forth mirrors `find-xt` / `find-mark`; host `find`/`findentry`/`entry-find` untouched). WAVE17 tip **1** landed SYNONYM/ALIAS name-map (`docs/SYNONYM-ALIAS.md` — **not** FIND rewrite). WAVE18 tip **1** / wave20 tip **4** landed TICK stub `xt=` ids + EXECUTE invoke mark. WAVE19 tip **3** landed thin `ENVIRONMENT?` (query mark — **not** SEARCH-WORDLIST / wordlist rewrite). WAVE21 tip **3** COMPARE string-compare mark kept SEARCH-WORDLIST deferred. WAVE22 tip **1** / **2** / **3** landed LSHIFT/RSHIFT / `0=` / `>NUMBER` (leave those primaries untouched). WAVE18–22 deferred `SEARCH-WORDLIST` as a thin vocab-search mark beside FIND + WORDS (**not** FIND reopen; **not** full linked dict / wordlist-stack runtime rewrite; **must not** redefine host find / find-xt / find-mark). This tip lands **stub** thin vocab-search mark only: `SEARCH-WORDLIST` (or Forth mirror **`search-wl-mark`**) prints `[search] SEARCH-WORDLIST` (+ optional `flag=` / `xt=` for a fixed demo name / wordlist fixture — hit → greppable flag/id mark; optional miss picture welcome). Smoke via **`search-demo`**. Prefer Forth mirror **`search-wl-mark`** whenever host `SEARCH-WORDLIST` collides. **CRITICAL:** host `find` / `findentry` / `entry-find` + wave18 `find-xt` / `find-mark` stay untouched — do **NOT** redefine them or Lab-grep them as this tip’s surface. Pairs with wave18 FIND + wave11 WORDS **without** a FIND reopen, **without** linked-dict / wordlist-stack rewrite, and **without** reopening SYNONYM FIND / EXECUTE / TO-NUMBER / ZERO-EQUALS / LSHIFT. **Not** tip5 DOCS-CITES. Independent of tip5.

## 2. Words / Surfaces

| Word | Stack | Notes |
|------|-------|-------|
| `SEARCH-WORDLIST` / `search-wl-mark` | `( c-addr u wid -- 0 \| xt 1 \| xt -1 )` *or* `( -- )` with fixed demo fixture | Thin vocab-search mark; print `[search] SEARCH-WORDLIST` (+ optional `flag=<n>` / `xt=<id>`); hit → greppable flag/id mark; optional miss picture welcome |
| `search-demo` | `( -- )` | See §5 |

Host note: bind bare `SEARCH-WORDLIST` on the Linux REPL **only if** that name does not collide with host Forth `SEARCH-WORDLIST`. Prefer Forth mirror **`search-wl-mark`** as the Lab-facing surface when in doubt — **do not** redefine host `SEARCH-WORDLIST`. **CRITICAL:** do **not** redefine host `find` / `findentry` / `entry-find` / wave18 `find-xt` / `find-mark` — those stay wave7 / wave18 surfaces (this tip is a **thin vocab-search mark**, not a FIND reopen). **Do not** redefine `WORDS` / `.words` / `words` / `words-demo` — WORDS-VOCAB stays list-only sibling (**not** WORDS reopen). Optional `flag=` / `xt=` are host ints / fixture echo only — not linked dict, not FIND rewrite, not EXECUTE. Prefer **fixed demo name / wordlist fixtures** (classic hit name such as `widget` / known FIND/TICK demo fixture / documented stub wid — document) so Lab hit is deterministic and FAIL is avoided.

## 3. Stub semantics

- **`SEARCH-WORDLIST` / `search-wl-mark`:** take (or use fixed demo) a pictured name / wordlist fixture — classic ANS sketch `( c-addr u wid -- 0 | xt 1 | xt -1 )` where a counted name is searched in a pictured wordlist id. This tip is **marker only**: print `[search] SEARCH-WORDLIST` and optionally `flag=<n>` and/or `xt=<id>` (Lab-greppable; prefer documenting which Shipper emits). Classic fixture welcome: known demo name `"widget"` (or FIND/TICK fixture / CREATE name — document) under pictured stub wordlist → hit (`flag=-1` or `flag=1` / greppable `xt=<id>` matching tip1 TICK stub-xt convention welcome); optional miss picture welcome when free — **do not require** miss path for Lab OK. **Does not** rewrite FIND / host find aliases / find-xt / find-mark, implement a full linked dict / wordlist stack / SEARCH-WORDLIST runtime, reopen SYNONYM FIND, execute found XT, or bump HERE. **Does not** redefine / alias host `find` / `findentry` / `entry-find` / `find-xt` / `find-mark`. Captured values are host ints / fixture echo only.
- **Fixed demo name / wordlist fixtures (document):**
  - **Classic hit picture (required set):** e.g. fixture name `"widget"` (or known FIND/TICK / CREATE / colon demo name — document) under a pictured stub wordlist / wid. Demo must exercise **`SEARCH-WORDLIST` / `search-wl-mark`** so Lab greps `[search] SEARCH-WORDLIST`. Optional `flag=` / `xt=` welcome (classic `flag=-1` / `flag=1` / `xt=<id>` picture).
  - **Optional result echo:** `flag=<1|-1|0>` and/or `xt=<stub-id>` — document which form Shipper emits. Greppable `[search] SEARCH-WORDLIST` alone is enough for Lab OK when the marker line appears.
  - **Optional miss picture:** fixture miss → `[search] SEARCH-WORDLIST miss` **or** `flag=0` welcome — document; do **not** require FAIL. Demo avoids `[search] FAIL`.
  - **Optional FIND pairing:** fixture may sit beside (not replacing) `find-demo` / `tick-demo` / `words-demo` pictures — document; do **not** require a FIND reopen / host-find rewrite / linked-dict rewrite / find-xt redefine.
- **Optional push:** if the host stack is easy, push pictured `0 | xt 1 | xt -1`; marker alone is enough for Lab OK — do not require a real wordlist stack / FIND reopen / EXECUTE / linked dict.
- **FAIL:** `[search] FAIL reason=<…>` optional (demo **must avoid**). Prefer not emitting `[search] FAIL` on the happy path. No required FAIL reason this tip — miss / unknown wid / empty name stay out of the demo (or document as optional miss picture that still avoids FAIL).
- Storage: fixed demo name / wordlist fixture / host int echo only. **No** FIND reopen, no host `find`/`findentry`/`entry-find` redefine, no `find-xt`/`find-mark` redefine, no full linked dict / wordlist-stack runtime rewrite, no SYNONYM FIND rewrite, no EXECUTE reopen, no TO-NUMBER / ZERO-EQUALS / LSHIFT reopen, no arena, no full ANS ENVIRONMENT? table reopen.
- **Host find / find-xt / find-mark stay untouched:** wave7 `find` / `findentry` / `entry-find` and wave18 `find-xt` / `find-mark` remain the FIND surfaces. This tip’s Lab greps are `[search] SEARCH-WORDLIST` and `[search-demo] OK` only — **never** Lab-grep `[find]` / `find-xt` / `find-mark` / `[kernel] find hit` as this tip’s vocab-search success. **Do not** redefine those names.
- **FIND.md stays (thin companion amend only):** do **not** wipe wave18 FIND content. SEARCH-WORDLIST is a **sibling** vocab-search mark beside FIND — not a FIND reopen. Thin companion amend of FIND.md only.
- **WORDS-VOCAB stays (thin companion amend only):** do **not** wipe wave11 WORDS content. Flat list stays list-only; SEARCH-WORDLIST is sibling mark — **not** WORDS reopen / linked dict.
- **TO-NUMBER / ZERO-EQUALS / LSHIFT stay untouched primaries:** do **not** redefine `>NUMBER` / `to-number-mark`. Do **not** redefine `0=` / `0<>` / `zero-eq-mark` / `zero-ne-mark`. Do **not** redefine `LSHIFT` / `RSHIFT` / `lshift-mark` / `rshift-mark`. **Leave `TO-NUMBER.md` primary untouched** (tip3 land md5 `ab016df909ab12b87c1c3a7fff9a9a81` / 25997). **Leave `ZERO-EQUALS.md` primary untouched** (tip2 land md5 `fd250f768e41fd70dd32c01faa32fc74` / 26165). **Leave `LSHIFT-RSHIFT.md` primary untouched** (tip1 land md5 `e5a94d8a16d47aa7ceae92b54344712e` / 26299).
- Nest with prior number / zero / shift / bit / compare / base / accept / exec / count / within / true / env / find / tick / words / allot / cell stubs OK. `dict-reset` unaffected for fixed host fixtures (or clears as usual when demo creates a fixture name). Assert prior `find-demo` / `words-demo` / `number-demo` still OK (host find aliases untouched; FIND mark untouched; WORDS list untouched; HERE stub base untouched).
- Still no FIND reopen, no full linked dict / SEARCH-WORDLIST runtime rewrite, no SYNONYM FIND rewrite, no EXECUTE reopen, no TO-NUMBER / ZERO-EQUALS / LSHIFT / COMPARE / ACCEPT-REFILL reopen, no linked XT / real DOES> XT / full arena / full Win/Android Forth VM. Wave22 tip1–3 + wave21 tip1–5 + wave20 tip1–4 + wave19 tip1–4 + wave18 tip1–4 stay landed — keep cites; this tip does not reopen them.

## 4. Markers

```
[search] SEARCH-WORDLIST [flag=<n>] [xt=<id>]   # flag=/xt= optional; hit → greppable flag/id mark; optional miss picture welcome
[search] SEARCH-WORDLIST miss                   # optional miss line (demo may show one)
[search] FAIL reason=<…>                        # demo avoids
[search-demo] OK
[search-demo] FAIL
```

Lab greps `[search-demo] OK` plus greppable **`[search] SEARCH-WORDLIST`** (optional `flag=` / `xt=` welcome — classic hit picture on fixed demo name / wordlist fixture). Demo avoids `[search] FAIL`. Prefer not emitting `[search] FAIL` on the happy path. **Do not** Lab-grep `[find]` / `find-xt` / `find-mark` / host `find` / `findentry` / `entry-find` / `[kernel] find hit` as this tip’s surface (those stay wave7 / wave18). **Do not** Lab-grep `[number]` / `[zero]` / `[shift]` / `[bit]` / `[compare]` / `[accept]` / `[words]` / `[synonym]` / `[exec]` / `[tick]` as SEARCH-WORDLIST success (those stay their own tips). **Do not** Lab-grep full linked-dict / wordlist-stack runtime rewrite as this tip (those stay deferred).

## 5. `search-demo`

1. Clean slate / `dict-reset` (or cold path) — optional; vocab-search mark may use fixed host fixtures and/or a known demo name.
2. Ensure **fixed demo name / wordlist fixture** exists (e.g. `"widget"` / known FIND/TICK / CREATE / colon demo name — or equivalent host counted-string / c-addr+u + stub wid fixture) so the classic hit picture is deterministic. Fixture may live beside (not replacing) FIND mark picture / WORDS list / TICK stub xt-id / SYNONYM name-map / EXECUTE invoke mark — document; do **not** require FIND reopen / host-find rewrite / find-xt redefine / linked-dict rewrite / SYNONYM FIND rewrite / EXECUTE reopen / TO-NUMBER reopen / ZERO-EQUALS reopen / LSHIFT reopen.
3. Invoke `SEARCH-WORDLIST` (or **`search-wl-mark`**) against the classic fixture → `[search] SEARCH-WORDLIST` (+ optional `flag=` / `xt=` — classic hit / `flag=-1` / `flag=1` / `xt=<id>` welcome).
4. Optional: invoke on an unknown name → `[search] SEARCH-WORDLIST miss` (preferred Lab miss picture) **or** skip miss entirely. Do **not** require `[search] FAIL`.
5. Assert no `[search] FAIL` on the happy path. Assert vocab-search mark did **not** require FIND reopen / host `find`/`findentry`/`entry-find` redefine / `find-xt`/`find-mark` redefine / linked-dict rewrite / SYNONYM FIND rewrite / EXECUTE reopen / TO-NUMBER reopen / ZERO-EQUALS reopen / LSHIFT reopen / full ANS ENVIRONMENT? table reopen (marker-only is enough). Assert host `SEARCH-WORDLIST` was not redefined when using the Forth mirror. Assert host `find` / `findentry` / `entry-find` / `find-xt` / `find-mark` were **not** redefined. Assert `to-number-mark` / `zero-eq-mark` / `lshift-mark` / `rshift-mark` were **not** redefined. Assert `TO-NUMBER.md` + `ZERO-EQUALS.md` + `LSHIFT-RSHIFT.md` primaries left untouched. Assert prior `find-demo` / `words-demo` / `number-demo` / `tick-demo` / `synonym-demo` still OK.
6. Prior `number-demo` / `zero-demo` / `shift-demo` / `bit-demo` / `compare-demo` / `base-demo` / `accept-demo` / `exec-demo` / `count-demo` / `within-demo` / `true-demo` / `source-demo` / `env-demo` / `body-demo` / `char-demo` / `state-demo` / `word-demo` / `find-demo` / `tick-demo` / `allot-demo` + earlier demos incl `trit-math-demo` / `fold-demo` still OK (host find aliases and FIND mark must remain intact; HERE stub base `$1000` untouched).
7. `[search-demo] OK`.

Required marker: `[search] SEARCH-WORDLIST`. Optional `flag=` / `xt=` echo is not required for Lab OK when `[search] SEARCH-WORDLIST` is greppable. No FIND reopen. No host find / find-xt / find-mark redefine. No linked-dict rewrite. No SYNONYM FIND rewrite. No EXECUTE reopen. No TO-NUMBER / ZERO-EQUALS / LSHIFT reopen. No full ANS ENVIRONMENT? table reopen.

## 6. Thin amend — companions

### `docs/FIND.md`

- Companions / Status: add `SEARCH-WORDLIST.md` (wave22 **4** companion cite); **keep** KERNEL / WORDS-VOCAB / SYNONYM-ALIAS / INTERPRET / TICK / ENVIRONMENT-QUERY / EXECUTE cites — do not wipe wave18 FIND content.
- Purpose / §3 / non-goals: FIND stays ANS-ish find mark via `find-xt` / `find-mark`; host `find`/`findentry`/`entry-find` stay; `SEARCH-WORDLIST` is sibling **thin vocab-search mark** over fixed demo name / wordlist fixture — **not** a FIND reopen / host-find rewrite / find-xt redefine / linked-dict rewrite / SYNONYM FIND rewrite / EXECUTE reopen. Do not wipe wave18 FIND content. Stress: SEARCH-WORDLIST sibling vocab-search mark — **NOT** FIND reopen; **CRITICAL** host find / findentry / entry-find + find-xt / find-mark untouched; prefer `search-wl-mark` whenever host `SEARCH-WORDLIST` collides.
- Non-goals: `SEARCH-WORDLIST` → `docs/SEARCH-WORDLIST.md` (wave22 **4**). FIND stays on this tip (already landed). Linked dict / wordlist-stack runtime still out. Tip5 DOCS-CITES still later (wave22 **5**).
- Acceptance: Lab smokes `search-demo` (retains `find-demo` + `tick-demo` + `words-demo` + `number-demo` + `zero-demo` + `shift-demo` + `exec-demo` + `synonym-demo`).
- Cite: `docs/SEARCH-WORDLIST.md`.

### `docs/WORDS-VOCAB.md`

- Companions / Status: add `SEARCH-WORDLIST.md` (wave22 **4** companion cite); **keep** KERNEL / INTERPRET / COLON / VARIABLE-CONST / MARKER / SYNONYM-ALIAS / FIND / ENVIRONMENT-QUERY / COUNT cites — do not wipe wave11 WORDS content.
- Purpose / §4 / non-goals: flat list stays list-only; `SEARCH-WORDLIST` is sibling **thin vocab-search mark** — **not** a WORDS reopen / linked dict / wordlist-stack rewrite / FIND reopen. Do not wipe wave11 / MARKER / SYNONYM / FIND / ENV / COUNT content. Stress: SEARCH-WORDLIST sibling vocab-search mark beside FIND/WORDS — **NOT** WORDS reopen / linked dict; prefer `search-wl-mark`.
- Non-goals: `SEARCH-WORDLIST` → `docs/SEARCH-WORDLIST.md` (wave22 **4**). WORDS stays on this tip (already landed). FIND stays wave18 **2**. Tip5 DOCS-CITES still later (wave22 **5**).
- Acceptance: Lab smokes `search-demo` (retains `words-demo` + `find-demo` + `synonym-demo` + `number-demo`).
- Cite: `docs/SEARCH-WORDLIST.md`.

### `docs/KERNEL.md`

- Companions: add `SEARCH-WORDLIST.md` (wave22 **4**); **keep** wave22 tip1–3 LSHIFT-RSHIFT / ZERO-EQUALS / TO-NUMBER cites and wave21 tip1–4 ACCEPT-REFILL / BASE-HEX / COMPARE / BITWISE cites and wave20 tip1–4 / wave19 tip1–4 / wave18 tip1–4 / wave17 tip1–4 / wave16 DEFER/MARKER/BUFFER/EXIT / wave15 CELL cites.
- Words table: add `SEARCH-WORDLIST` / `search-wl-mark` stub + `search-demo` (cite tip; Forth mirror `search-wl-mark` — thin vocab-search mark only; **CRITICAL:** do **not** redefine host `find` / `findentry` / `entry-find` / wave18 `find-xt` / `find-mark`; prefer `search-wl-mark` whenever host `SEARCH-WORDLIST` collides; **do not** redefine to-number-mark / zero-eq-mark / lshift-mark/rshift-mark / synonym-map / execute-mark / tick-mark; **not** FIND reopen; **not** linked-dict / wordlist-stack runtime rewrite; **not** SYNONYM FIND rewrite; **not** EXECUTE reopen; optional `flag=` / `xt=` — classic hit picture welcome).
- Non-goals: `SEARCH-WORDLIST` thin vocab-search mark → `docs/SEARCH-WORDLIST.md`. FIND stays on `FIND.md` (leave find-xt/find-mark / host find untouched). WORDS stays on `WORDS-VOCAB.md`. `>NUMBER` stays on `TO-NUMBER.md` (leave primary untouched). `0=` stays on `ZERO-EQUALS.md` (leave primary untouched). LSHIFT/RSHIFT stay on `LSHIFT-RSHIFT.md` (leave primary untouched). Tip5 DOCS-CITES still later (wave22 **5**).
- Acceptance: Lab smokes `search-demo` (and retains `number-demo` + `zero-demo` + `shift-demo` + `bit-demo` + `compare-demo` + `base-demo` + `accept-demo` + `exec-demo` + `count-demo` + `within-demo` + `true-demo` + `env-demo` + `find-demo` + `words-demo` + `allot-demo` + prior demos incl `trit-math-demo` / `fold-demo`).
- Cite: `docs/SEARCH-WORDLIST.md`.

### Optional — `docs/SYNONYM-ALIAS.md`

- Companions / Status: add light `SEARCH-WORDLIST.md` (wave22 **4**) cite; **keep** WORDS-VOCAB / DEFER-IS / KERNEL / CREATE-DOES / FIND cites — do not wipe wave17 SYNONYM content.
- Purpose / §3 / non-goals: SYNONYM/ALIAS stay name→name map only; `SEARCH-WORDLIST` is sibling **thin vocab-search mark** — **NOT** a SYNONYM FIND rewrite / linked XT / FIND reopen / host-find rewrite. Do not wipe wave17 SYNONYM content. Stress: SEARCH-WORDLIST sibling — **not** SYNONYM FIND rewrite; host find / find-xt stay untouched; prefer `search-wl-mark`.
- Non-goals: `SEARCH-WORDLIST` → `docs/SEARCH-WORDLIST.md` (wave22 **4**). SYNONYM/ALIAS stay on this tip (already landed). FIND stays wave18 **2**.
- Acceptance: Lab smokes `search-demo` (retains `synonym-demo` + `find-demo` + `words-demo`).
- Cite: `docs/SEARCH-WORDLIST.md`.

### Optional — `docs/TICK.md`

- Companions / Status: add light `SEARCH-WORDLIST.md` (wave22 **4**) cite; **keep** KERNEL / DEFER-IS / IMMEDIATE-POSTPONE / CREATE-DOES / STATE-COMPILE / TO-BODY / EXECUTE cites — do not wipe wave18 TICK content.
- Purpose / §3 / non-goals: tick stub `xt=` ids stay; `SEARCH-WORDLIST` may echo optional stub `xt=` consistent with this tip’s convention on a fixed demo fixture — **not** a TICK reopen / XT execute / FIND reopen / linked XT. Do not wipe wave18 TICK content. Stress: optional `xt=` echo welcome — not TICK reopen; host find / find-xt untouched.
- Non-goals: `SEARCH-WORDLIST` → `docs/SEARCH-WORDLIST.md` (wave22 **4**). `'` / `[']` stay on this tip (already landed). FIND stays wave18 **2**. EXECUTE stays wave20 **4**.
- Acceptance: Lab smokes `search-demo` (retains `tick-demo` + `find-demo` + `exec-demo`).
- Cite: `docs/SEARCH-WORDLIST.md`.

### Optional — `docs/EXECUTE.md`

- Companions / Status: add light `SEARCH-WORDLIST.md` (wave22 **4**) cite; **keep** TICK / FIND / DEFER-IS / KERNEL / RECURSE / IMMEDIATE-POSTPONE / STATE-COMPILE cites — do not wipe wave20 EXECUTE content.
- Purpose / §3 / non-goals: EXECUTE stays xt-id invoke mark only; `SEARCH-WORDLIST` is sibling **thin vocab-search mark** — **NOT** an EXECUTE reopen / real XT execute / linked XT / FIND reopen. Do not wipe wave20 EXECUTE content. Stress: SEARCH-WORDLIST sibling — not EXECUTE reopen; do not redefine execute-mark; host find / find-xt untouched.
- Non-goals: `SEARCH-WORDLIST` → `docs/SEARCH-WORDLIST.md` (wave22 **4**). EXECUTE stays on this tip (already landed). FIND stays wave18 **2**.
- Acceptance: Lab smokes `search-demo` (retains `exec-demo` + `find-demo` + `tick-demo`).
- Cite: `docs/SEARCH-WORDLIST.md`.

### Optional — `docs/INTERPRET.md`

- Companions / Status: add light `SEARCH-WORDLIST.md` (wave22 **4**) cite; **keep** KERNEL / COLON / COMMENT-PARSE / STRING-LIT / IMMEDIATE-POSTPONE / EXIT-QUIT / PARSE-NAME / EVALUATE-INCLUDE / FIND / TICK / WORD-BL / STATE-COMPILE / SOURCE-PAD / ACCEPT-REFILL cites — do not wipe wave8–21 INTERPRET content.
- Purpose / §2 / §3 / non-goals: interpret loop stays on host `find`/`findentry`/`entry-find` lookup; `SEARCH-WORDLIST` is sibling **thin vocab-search mark** via `search-wl-mark` — **does not** rewrite the interpret find path / FIND reopen / linked dict / execute found XT. Do not wipe wave8–21 INTERPRET content. Stress: host find aliases + `[kernel] find hit/miss` stay; prefer `search-wl-mark`; **not** FIND reopen.
- Non-goals: `SEARCH-WORDLIST` → `docs/SEARCH-WORDLIST.md` (wave22 **4**). FIND stays wave18 **2**. Full linked dict / wordlist stack still out.
- Acceptance: Lab smokes `search-demo` (retains `interpret-demo` + `kernel-demo` + `find-demo` + `tick-demo` + prior).
- Cite: `docs/SEARCH-WORDLIST.md`.

### Optional — `docs/HOST-PARITY.md`

- Companions: add light `SEARCH-WORDLIST.md` (wave22 **4**) cite; **keep** TO-NUMBER / ZERO-EQUALS / LSHIFT-RSHIFT / BITWISE / TRUE-FALSE / BASE-HEX / ENVIRONMENT-QUERY / KERNEL / BUILD / INSTALL / INTERPRET cites.
- Purpose / non-goals: Win/Android stay CONTRACT-parity; `search-demo` CONTRACT line is acceptable — **not** a full Forth VM / linked-dict / FIND-reopen / wordlist-stack port. Do not wipe wave8 HOST-PARITY content. Stress: host find / find-xt stay untouched on Linux SoT; prefer `search-wl-mark`.
- Non-goals: `SEARCH-WORDLIST` → `docs/SEARCH-WORDLIST.md` (wave22 **4**). Full Win/Android Forth VM still out. TO-NUMBER stays wave22 **3**. FIND stays wave18 **2**.
- Acceptance: Lab smokes `search-demo` (Win/Android: `search-demo CONTRACT` OK).
- Cite: `docs/SEARCH-WORDLIST.md`.

Do **not** wipe wave22 tip1–3 / wave21 tip1–5 / wave20 tip1–4 / wave19 tip1–4 / wave18–15 prior content. Do **not** amend ARCHITECTURE / IMPLEMENTATION-GAPS / TO-NUMBER / ZERO-EQUALS / LSHIFT-RSHIFT / ACCEPT-REFILL / COMPARE primary this tip (amends are FIND + WORDS-VOCAB + KERNEL + optional SYNONYM-ALIAS / TICK / EXECUTE / INTERPRET / HOST-PARITY only). **Leave `TO-NUMBER.md` untouched** (`ab016df909ab12b87c1c3a7fff9a9a81` / 25997). **Leave `ZERO-EQUALS.md` untouched** (`fd250f768e41fd70dd32c01faa32fc74` / 26165). **Leave `LSHIFT-RSHIFT.md` untouched** (`e5a94d8a16d47aa7ceae92b54344712e` / 26299). **Leave `ACCEPT-REFILL.md` / `COMPARE.md` untouched.** **Leave `ARCHITECTURE.md` / `IMPLEMENTATION-GAPS.md` untouched** (`b924e9ce5c1e14efd8e0ae738dde5f51` / `712d77305646b9275cb4e76195f00f38`). **Leave `WAVE22-PROPOSAL.md` / `WAVE22-COS-PASTE.txt` untouched.** Tip5 cites after 1–4 PASS. Skip 2DUP-FAMILY + ABORT" polish.

## 7. Non-goals

- Redefining host `find` / `findentry` / `entry-find` / wave18 `find-xt` / `find-mark` (wave7 / wave18 — already stubbed; keep cites; **CRITICAL** — host find + find-xt / find-mark stay untouched)
- FIND reopen / ANS-ish find-mark rewrite (`docs/FIND.md` — thin companion amend only; not FIND reopen)
- Full linked dict / wordlist stack / SEARCH-WORDLIST runtime rewrite
- SYNONYM FIND rewrite / entry-body rewrite (SYNONYM stays name→name — wave17 **1**; thin companion amend only)
- EXECUTE reopen / real XT execute / linked XT (wave20 **4** — already stubbed; thin companion amend only)
- Docs cites pass (wave22 **5** — ARCHITECTURE + GAPS after 1–4 PASS)
- `>NUMBER` / `to-number-mark` reopen (wave22 **3** — already stubbed; leave TO-NUMBER.md primary untouched)
- `0=` / `0<>` / `zero-eq-mark` / `zero-ne-mark` reopen (wave22 **2** — already stubbed; leave ZERO-EQUALS.md primary untouched)
- `LSHIFT` / `RSHIFT` / `lshift-mark` / `rshift-mark` reopen (wave22 **1** — already stubbed; leave LSHIFT-RSHIFT.md primary untouched)
- `COMPARE` / `ACCEPT-REFILL` reopen (wave21 **1** / **3** — already stubbed; leave those primaries untouched)
- `BITWISE` reopen (wave21 **4** — already stubbed; keep cites)
- Full ANS `ENVIRONMENT?` table reopen (wave19 **3** — already stubbed; keep cites)
- `TRUE` / `FALSE` / `WITHIN` / `COUNT` / `SOURCE` / `PAD` reopen (wave20 / wave19 — already stubbed; keep cites)
- `WORDS` / `.words` reopen (wave11 **3** — already stubbed; thin companion amend only — list stays list-only)
- `'` / `[']` tick reopen (wave18 **1** — already stubbed; optional `xt=` convention shared; thin companion amend only)
- Real DOES> XT / real branch XT / LEAVE jump / full arena / linked XT / real STATE cell
- `2DUP` / `2DROP` / `2SWAP` stub redefinition (host primitives already live — **skip 2DUP-FAMILY**)
- `ABORT"` polish (already optional-wired inside `throw-demo` — **skip**)
- Real crypto / network fleet / opaque-weight ML
- Full Win/Android Forth VM (CONTRACT acceptable)
- No merge. Stay out of Mango.

## 8. Acceptance (Test Lab)

1. `docs/SEARCH-WORDLIST.md` present (Research byte-copy OK); `FIND.md` + `WORDS-VOCAB.md` + `KERNEL.md` thin amends present (+ optional `SYNONYM-ALIAS.md` / `TICK.md` / `EXECUTE.md` / `INTERPRET.md` / `HOST-PARITY.md`); wave22 tip1–3 + wave21 tip1–5 + wave20 tip1–4 + wave19 tip1–4 + wave18 tip1–4 + wave17–15 prior cites retained; host `SEARCH-WORDLIST` untouched via mirrors (`search-wl-mark` preferred); host `find` / `findentry` / `entry-find` / `find-xt` / `find-mark` **not** redefined; `to-number-mark` / `zero-eq-mark` / `lshift-mark`/`rshift-mark` **not** redefined; `TO-NUMBER.md` primary untouched (`ab016df909ab12b87c1c3a7fff9a9a81` / 25997); `ZERO-EQUALS.md` primary untouched (`fd250f768e41fd70dd32c01faa32fc74` / 26165); `LSHIFT-RSHIFT.md` primary untouched (`e5a94d8a16d47aa7ceae92b54344712e` / 26299); `ACCEPT-REFILL.md` + `COMPARE.md` primaries untouched; `ARCHITECTURE.md` + `IMPLEMENTATION-GAPS.md` unchanged (`b924e9ce5c1e14efd8e0ae738dde5f51` / `712d77305646b9275cb4e76195f00f38`); `WAVE22-PROPOSAL.md` / `WAVE22-COS-PASTE.txt` untouched.
2. `search-demo` → OK (markers §4; `[search] SEARCH-WORDLIST` greppable; optional `flag=` / `xt=` welcome — classic hit picture on fixed demo name / wordlist fixture; no FAIL on happy path; no FIND reopen / host-find rewrite / find-xt redefine / linked-dict rewrite / SYNONYM FIND rewrite / EXECUTE reopen / TO-NUMBER reopen / ZERO-EQUALS reopen / LSHIFT reopen / COMPARE reopen / full ANS ENVIRONMENT? table reopen; no `find`/`findentry`/`entry-find`/`find-xt`/`find-mark` redefine). Prior `number-demo` + `zero-demo` + `shift-demo` + `bit-demo` + `compare-demo` + `base-demo` + `accept-demo` + `exec-demo` + `count-demo` + `within-demo` + `true-demo` + `env-demo` + `find-demo` + `words-demo` + `tick-demo` + `synonym-demo` + `allot-demo` + earlier demos incl `trit-math-demo` / `fold-demo` still OK.
3. Regression green (wave22 tip1–3 + wave21 tip1–5 + wave20 tip1–5 + wave19 tip1–5 + wave18 tip1–5 + wave17 tip1–5 + wave16 tip1–5 + wave15 tip1–5 + wave14 demos + prior).
4. Win/Android: CONTRACT acceptable (parity line `search-demo CONTRACT` OK).
5. No merge. Stay out of Mango. No opaque-weight ML. Skip 2DUP-FAMILY + ABORT" polish.

## 9. Cite

- `docs/KERNEL.md` (wave7 **5**)
- `docs/FIND.md` (wave18 **2** — ANS-ish find-mark companion; SEARCH-WORDLIST sibling vocab-search mark — **not** FIND reopen; **CRITICAL** host `find`/`findentry`/`entry-find` + `find-xt`/`find-mark` untouched)
- `docs/WORDS-VOCAB.md` (wave11 **3** — flat WORDS list companion; SEARCH-WORDLIST sibling — **not** WORDS reopen / linked dict)
- `docs/SYNONYM-ALIAS.md` (wave17 **1**, optional — name→name map companion; **not** SYNONYM FIND rewrite)
- `docs/TICK.md` (wave18 **1**, optional — stub `xt=` id convention companion; optional `xt=` echo welcome — not TICK reopen)
- `docs/EXECUTE.md` (wave20 **4**, optional — xt-id invoke mark companion; **not** EXECUTE reopen / real XT execute)
- `docs/INTERPRET.md` (wave8 **1**, optional — interpret stays on host find path; SEARCH-WORDLIST does not rewrite interpret find)
- `docs/HOST-PARITY.md` (wave8 **4**, optional — `search-demo` CONTRACT parity welcome)
- `docs/TO-NUMBER.md` (wave22 **3** — prior tip; keep cites; leave primary untouched this tip; host `>NUMBER` stay untouched via `to-number-mark`)
- `docs/ZERO-EQUALS.md` (wave22 **2** — prior tip; keep cites; leave primary untouched this tip; host `0=`/`0<>` stay untouched)
- `docs/LSHIFT-RSHIFT.md` (wave22 **1** — prior tip; keep cites; leave primary untouched this tip; host lowercase `lshift`/`rshift` stay untouched)
- `docs/COMPARE.md` (wave21 **3** — prior tip; keep cites; leave primary untouched this tip; host `cstr=` ≠ ANS COMPARE)
- `docs/ACCEPT-REFILL.md` (wave21 **1** — prior tip; keep cites; leave primary untouched this tip)
- `docs/BITWISE.md` (wave21 **4** — prior tip; keep cites)
- `docs/TRUE-FALSE.md` / `docs/WITHIN.md` / `docs/COUNT.md` (wave20 **1–3** — prior tips; keep cites)
- `docs/ENVIRONMENT-QUERY.md` (wave19 **3** — query-mark companion; not SEARCH-WORDLIST / wordlist rewrite — prior tip; keep cites)
- `docs/SOURCE-PAD.md` (wave19 **4** — prior tip; keep cites)
- `forth/tritium/kernel.fs` (search-wl-mark only — do not redefine host `SEARCH-WORDLIST`; **do not** redefine host `find`/`findentry`/`entry-find`/`find-xt`/`find-mark`; **do not** redefine to-number-mark / zero-eq-mark / lshift-mark/rshift-mark)
- ANS Forth `SEARCH-WORDLIST` (thin vocab-search mark only — not FIND reopen; not linked dict / wordlist-stack runtime; host `SEARCH-WORDLIST` ≠ force bare bind — prefer `search-wl-mark`)
- Explicit deferral: WAVE18-PROPOSAL + WAVE19-PROPOSAL + WAVE20-PROPOSAL + WAVE21-PROPOSAL + WAVE22-PROPOSAL (`SEARCH-WORDLIST` — thin vocab-search mark; not FIND reopen; do not break host find / find-xt / find-mark)
- Base tip: `b7f6052` / `b7f60527c3ace9477b7d554e210b592fc646312b` (#104 wave22 tip3 TO-NUMBER PASS)
- Wave22 proposal: `/workspace/tritium-research-docs/WAVE22-PROPOSAL.md`
