# ARCHITECTURE — TritiumOS map

**Status:** Shipper-ready overview (wave23 item **5** — cite refresh; supersedes wave22 **5** map)
**Canonical brief:** `TritiumOS.txt` §§1a, 5–8
**Companions:** per-subsystem docs listed in §4 (do not duplicate their contracts here)

## 1. Intent

TritiumOS ships as a **personal assistant** that co-evolves with the user and may **graduate** into **L.I.N.E.O.S.** Intelligence is refined into **executable Forth** (R.E.K.I.A.), not opaque weights. Topology lives in **D.R.E.N.A.**; delivery is **tritium.poly** + Priority-1 host bridges.

## 2. Layer map

```
┌─────────────────────────────────────────────────────────────┐
│  Hosts (Priority-1)                                         │
│  Win TritiumOS.exe │ Android .apk │ Linux AppImage          │
│  first-run: license → name → edition │ REPL / S0 assist     │
│  parity markers (Win/Android CONTRACT OK)                   │
└────────────┬───────────────────────────┬────────────────────┘
             │ platform-* hooks          │ evolve/ I/O
             ▼                           ▼
┌────────────────────────────┐   ┌────────────────────────────┐
│  TritiumForth core         │   │  evolve/ (runtime)         │
│  kernel · interpret · colon│   │  user-graph · assistant-*  │
│  control · loops · do-loop │   │  edition · license-slots   │
│  var · value · 2var · allot│   │  forth/refined cold-load │
│  leave · unloop · case     │   │  queue/ · assimilate/      │
│  create · string · throw   │   │  integrate/ · master/      │
│  cell · pick · fill · imm  │   │  fleet/ · lineos/ · userland│
│  defer · marker · buf · exit│   │  forth/refined/*.fs        │
│  synonym · parse · eval ·  │   │  graduation.json           │
│  recurse · tick · find-xt  │   │  assistant-state.trit v2 │
│  word · state · char ·     │   │                            │
│  body · env · source ·     │   │                            │
│  true · within · count ·   │   │                            │
│  exec · accept · base ·    │   │                            │
│  compare · bit · shift ·   │   │                            │
│  zero · number · search ·  │   │                            │
│  char+ · hold · uless ·    │   │                            │
│  abs · comment · trit ·    │   │                            │
│  drena · rekia · queue ·   │   │                            │
│  assimilate · lineos ·     │   │                            │
│  integrate · master ·      │   │                            │
│  fleet · userland          │   │                            │
│  D.R.E.N.A. = structure    │   │                            │
│  R.E.K.I.A. = refine→Forth │   │                            │
└────────────┬───────────────┘   └────────────────────────────┘
             │ bundled by
             ▼
┌────────────────────────────┐
│  tritium.poly + manifest   │  product_id: tritium → lineos
└────────────────────────────┘
```

## 3. Data / control flows (thin)

| Flow | Path |
|------|------|
| S0 assist | Free-text → `drena-step` + `rekiA-refine` → `evolve/forth/refined/` + live word + `neurons=<n>` (`ASSISTANT.md`, `ASSISTANT-S0.md`) |
| Assistant state | v2 persist task/reminder/note hooks (`ASSISTANT-STATE.md`) |
| Topology | spawn / link / grow / step / groups / nested find (`DRENA.md`, `GROUPS.md`, `GROUPS-NESTED.md`) |
| Trit math | `trit+` / `trit*` / `pack-neuron-header` (`TRIT-MATH.md`, `NEURON.md`) |
| Interpret / colon | token stream → `interpret` / `:` body/marker → `;` (`KERNEL.md`, `INTERPRET.md`, `COLON.md`) |
| Words / vocab list | `.words` / `WORDS` flat dict list + `words-demo` (`WORDS-VOCAB.md`) |
| Persist | `graph-save` / assistant-state / edition / license-slots / vocab graph lines |
| Economy | enqueue → pull → `queue-prove!` → `assimilate-merge!` (`QUEUE.md`, `ASSIMILATE.md`, `ECONOMY-WIRE.md`) |
| Trust | format mint/verify scaffold (`MASTER.md`); slot gate (`LICENSE.md`) |
| Fleet | opt-in local export/import same-key (`FLEET.md`) — no network yet |
| Graduation | thresholds → `lineos-graduate` → confirm UX → brand markers (`LINEOS.md`, `LINEOS-CONFIRM.md`, `LINEOS-BRAND.md`) |
| Userland | scaffold tree + `userland-demo` (`USERLAND.md`) |
| Host parity | Linux SoT demos; Win/Android `parity/HOST-PARITY.txt` (`HOST-PARITY.md`) |
| Host boot | poly/core load-order markers + `host-boot-demo` (`HOST-BOOT.md`) |
| Control stubs | `IF`/`THEN`/`ELSE` cs balance + `control-demo` (`CONTROL.md`) |
| Loop stubs | `BEGIN`/`UNTIL`/`WHILE`/`REPEAT` + `loop-demo` (`BEGIN-UNTIL.md`) |
| Counted-loop stubs | `DO`/`LOOP`/`+LOOP`/`I` + `do-loop-demo` (`DO-LOOP.md`) |
| Variable / const stubs | `VARIABLE`/`CONSTANT` + `var-demo` (`VARIABLE-CONST.md`) |
| Comment-parse | `\` / `(` skip + `comment-demo` (`COMMENT-PARSE.md`) |
| Leave / again stubs | `LEAVE`/`AGAIN` + `leave-demo` (`LEAVE-AGAIN.md`) |
| Value / TO stubs | `VALUE`/`TO` + `value-demo` (`VALUE-TO.md`) |
| Case stubs | `CASE`/`OF`/`ENDOF`/`ENDCASE` + `case-demo` (`CASE-OF.md`) |
| Create / DOES> stubs | `CREATE`/`DOES>` + `create-demo` (`CREATE-DOES.md`) |
| String-lit stubs | `S"`/`."` + `string-demo` (`STRING-LIT.md`) |
| Allot / HERE stubs | `HERE`/`ALLOT` + `allot-demo` (`ALLOT-HERE.md`) |
| Unloop / J stubs | `UNLOOP`/`J` + `unloop-demo` (`UNLOOP-J.md`) |
| 2VARIABLE stubs | `2VARIABLE`/`2CONSTANT` + `2var-demo` (`2VARIABLE.md`) |
| Throw / catch stubs | `CATCH`/`THROW` + `throw-demo` (`THROW-CATCH.md`) |
| Cell / align stubs | `CELL`/`CELLS`/`ALIGN`/`ALIGNED` + `cell-demo` (`CELL-CELLS.md`) |
| Pick / roll stubs | `PICK`/`ROLL`/`DEPTH`/`?DUP` + `pick-demo` (`PICK-ROLL.md`) |
| Fill / move stubs | `FILL`/`ERASE`/`MOVE`/`CMOVE` + `fill-demo` (`FILL-MOVE.md`) |
| Immediate / postpone stubs | `IMMEDIATE`/`POSTPONE` + `imm-demo` (`IMMEDIATE-POSTPONE.md`) |
| Defer / IS stubs | `DEFER`/`IS`/`ACTION-OF` + `defer-demo` (`DEFER-IS.md`) |
| Marker restore stubs | `MARKER` restore-mark + `marker-demo` (`MARKER.md`) |
| Buffer: stubs | `BUFFER:` named buffer + `buffer-demo` (`BUFFER-COLON.md`) |
| Exit / quit stubs | `EXIT`/`QUIT` thin markers + `exit-demo` (`EXIT-QUIT.md`) |
| Synonym / alias stubs | `SYNONYM`/`ALIAS` name-map + `synonym-demo` (`SYNONYM-ALIAS.md`) |
| Parse-name stubs | `PARSE`/`PARSE-NAME` markers + `parse-demo` (`PARSE-NAME.md`) |
| Evaluate / include stubs | `EVALUATE`/`INCLUDE` mark-only echo + `eval-demo` (`EVALUATE-INCLUDE.md`) |
| Recurse stubs | `RECURSE` mark-only + `recurse-demo` (`RECURSE.md`) |
| Tick stubs | `'`/`[']` name→xt-id mark + `tick-demo` (`TICK.md`) |
| Find deepen stubs | `FIND`/`find-xt` mark + `find-demo` (`FIND.md`) |
| Word / BL stubs | `WORD`/`BL` token/pad mark + `word-demo` (`WORD-BL.md`) |
| State / compile stubs | `STATE`/`COMPILE,` query mark + `state-demo` (`STATE-COMPILE.md`) |
| Char-unit stubs | `CHAR`/`CHARS`/`[CHAR]` + `char-demo` (`CHAR-CHARS.md`) |
| >BODY address stubs | `>BODY` address mark + `body-demo` (`TO-BODY.md`) |
| Environment-query stubs | `ENVIRONMENT?` query mark + `env-demo` (`ENVIRONMENT-QUERY.md`) |
| Source / PAD stubs | `SOURCE`/`PAD` thin marks + `source-demo` (`SOURCE-PAD.md`) |
| True / false constant stubs | `TRUE`/`FALSE` constant marks + `true-demo` (`TRUE-FALSE.md`) |
| Within range-check stubs | `WITHIN` range-check mark + `within-demo` (`WITHIN.md`) |
| Count (ANS) stubs | ANS `COUNT` c-addr picture + `count-demo` (`COUNT.md`) |
| Execute xt-id invoke stubs | `EXECUTE` xt-id invoke mark only + `exec-demo` (`EXECUTE.md`) |
| Accept / refill stubs | `ACCEPT`/`REFILL` thin marks + `accept-demo` (`ACCEPT-REFILL.md`) |
| Base / hex / decimal stubs | `BASE`/`HEX`/`DECIMAL` base marks + `base-demo` (`BASE-HEX.md`) |
| Compare (ANS) stubs | ANS `COMPARE` string mark + `compare-demo` (`COMPARE.md`) |
| Bitwise stubs | `AND`/`OR`/`XOR`/`INVERT` bitwise marks + `bit-demo` (`BITWISE.md`) |
| LSHIFT / RSHIFT stubs | `LSHIFT`/`RSHIFT` shift marks + `shift-demo` (`LSHIFT-RSHIFT.md`) |
| Zero-equals stubs | `0=` (optional `0<>`) flag marks + `zero-demo` (`ZERO-EQUALS.md`) |
| >NUMBER stubs | `>NUMBER` thin number-parse mark + `number-demo` (`TO-NUMBER.md`) |
| SEARCH-WORDLIST stubs | `SEARCH-WORDLIST` vocab-search mark + `search-demo` (`SEARCH-WORDLIST.md`) |
| Char-plus stubs | `CHAR+` thin char-unit advance + `charplus-demo` (`CHAR-PLUS.md`) |
| Hold stubs | `HOLD` thin pictured-numeric start + `hold-demo` (`HOLD.md`) |
| U-less stubs | `U<` unsigned compare flag + `uless-demo` (`U-LESS.md`) |
| Abs / negate stubs | `ABS`/`NEGATE` signed magnitude / negate marks + `abs-demo` (`ABS-NEGATE.md`) |
| Address fold | `phi-fold` / `fold-target` goldens + `fold-demo` (`ADDRESS-FOLD.md`) |
| Refined cold-load | `evolve/forth/refined/*.fs` skip `qwantum-*` + `refined-boot-demo` (`REFINED-BOOT.md`) |
| AppImage refined | portable seed + `S_ISREG` + oneshot/EXTRACT; host matches tools/ SoT (`APPIMAGE-REFINED.md`) |
| Qwantum | dumps = K atoms for extract only — never live vocab (`QWANTUM-REKIA.md`) |

## 4. Doc index (contracts live here)

| Area | Doc |
|------|-----|
| Neuron / trit / S3 | `NEURON.md`, `ASSUMPTIONS.md`; `TRIT-MATH.md` (wave9 **1**) |
| Topology | `DRENA.md`, `GROUPS.md`, `GROUPS-NESTED.md` (wave7 **4**); `ADDRESS-FOLD.md` (wave10 **3**) |
| Kernel / interpret / colon / control / loops / cells | `KERNEL.md` (wave7 **5**); `INTERPRET.md` (wave8 **1**); `COLON.md` (wave9 **4**); `CONTROL.md` (wave10 **2**); `BEGIN-UNTIL.md` (wave11 **1**); `WORDS-VOCAB.md` (wave11 **3**); `DO-LOOP.md` (wave12 **1**); `VARIABLE-CONST.md` (wave12 **2**); `COMMENT-PARSE.md` (wave12 **3**); `LEAVE-AGAIN.md` (wave12 **4**); `VALUE-TO.md` (wave13 **1**); `CASE-OF.md` (wave13 **2**); `CREATE-DOES.md` (wave13 **3**); `STRING-LIT.md` (wave13 **4**); `ALLOT-HERE.md` (wave14 **1**); `UNLOOP-J.md` (wave14 **2**); `2VARIABLE.md` (wave14 **3**); `THROW-CATCH.md` (wave14 **4**); `CELL-CELLS.md` (wave15 **1**); `PICK-ROLL.md` (wave15 **2**); `FILL-MOVE.md` (wave15 **3**); `IMMEDIATE-POSTPONE.md` (wave15 **4**); `DEFER-IS.md` (wave16 **1**); `MARKER.md` (wave16 **2**); `BUFFER-COLON.md` (wave16 **3**); `EXIT-QUIT.md` (wave16 **4**); `SYNONYM-ALIAS.md` (wave17 **1**); `PARSE-NAME.md` (wave17 **2**); `EVALUATE-INCLUDE.md` (wave17 **3**); `RECURSE.md` (wave17 **4**); `TICK.md` (wave18 **1**); `FIND.md` (wave18 **2**); `WORD-BL.md` (wave18 **3**); `STATE-COMPILE.md` (wave18 **4**); `CHAR-CHARS.md` (wave19 **1**); `TO-BODY.md` (wave19 **2**); `ENVIRONMENT-QUERY.md` (wave19 **3**); `SOURCE-PAD.md` (wave19 **4**); `TRUE-FALSE.md` (wave20 **1**); `WITHIN.md` (wave20 **2**); `COUNT.md` (wave20 **3**); `EXECUTE.md` (wave20 **4**); `ACCEPT-REFILL.md` (wave21 **1**); `BASE-HEX.md` (wave21 **2**); `COMPARE.md` (wave21 **3**); `BITWISE.md` (wave21 **4**); `LSHIFT-RSHIFT.md` (wave22 **1**); `ZERO-EQUALS.md` (wave22 **2**); `TO-NUMBER.md` (wave22 **3**); `SEARCH-WORDLIST.md` (wave22 **4**); `CHAR-PLUS.md` (wave23 **1**); `HOLD.md` (wave23 **2**); `U-LESS.md` (wave23 **3**); `ABS-NEGATE.md` (wave23 **4**) |
| Refine / refined boot / AppImage | `REKIA.md`, `QWANTUM-REKIA.md`; `REFINED-BOOT.md` (wave10 **4**); `APPIMAGE-REFINED.md` (wave11 **4**) |
| Assistant S0 / state | `ASSISTANT.md`; `ASSISTANT-STATE.md` (wave9 **3**); `ASSISTANT-S0.md` (wave11 **2**) |
| License / master | `LICENSE.md`; `MASTER.md` (wave7 **1**) |
| Queue / Assimilate / wire | `QUEUE.md`, `ASSIMILATE.md` (wave6); `ECONOMY-WIRE.md` (wave9 **2**) |
| Fleet sync | `FLEET.md` (wave7 **2**) |
| Graduation / brand | `LINEOS.md`; `LINEOS-BRAND.md` (wave7 **3**); `LINEOS-CONFIRM.md` (wave8 **2**) |
| Install / Integrate | `INSTALL.md`, `INTEGRATE.md` (wave6 **4+5**) |
| Userland scaffold | `USERLAND.md` (wave8 **3**) |
| Host parity | `HOST-PARITY.md` (wave8 **4**) |
| Host boot | `HOST-BOOT.md` (wave10 **1**) |
| Build / platforms | `BUILD.md`, `SYSTEM-DESIGN-INITIAL-PLATFORMS.md` |
| Gaps (living) | `IMPLEMENTATION-GAPS.md` (this tip refreshes cites) |

**Wave cite summary (closed on tip, not merged):** wave6–9 as prior; **wave10** HOST-BOOT / CONTROL / ADDRESS-FOLD / REFINED-BOOT / prior cite; **wave11** BEGIN-UNTIL / ASSISTANT-S0 / WORDS-VOCAB / APPIMAGE-REFINED / prior cite; **wave12** DO-LOOP / VARIABLE-CONST / COMMENT-PARSE / LEAVE-AGAIN / prior cite; **wave13** VALUE-TO / CASE-OF / CREATE-DOES / STRING-LIT / prior cite; **wave14** ALLOT-HERE / UNLOOP-J / 2VARIABLE / THROW-CATCH / prior cite; **wave15** CELL-CELLS / PICK-ROLL / FILL-MOVE / IMMEDIATE-POSTPONE / prior cite; **wave16** DEFER-IS / MARKER / BUFFER-COLON / EXIT-QUIT / prior cite; **wave17** SYNONYM-ALIAS / PARSE-NAME / EVALUATE-INCLUDE / RECURSE / prior cite; **wave18** TICK / FIND / WORD-BL / STATE-COMPILE / prior cite; **wave19** CHAR-CHARS / TO-BODY / ENVIRONMENT-QUERY / SOURCE-PAD / prior cite; **wave20** TRUE-FALSE / WITHIN / COUNT / EXECUTE / prior cite; **wave21** ACCEPT-REFILL / BASE-HEX / COMPARE / BITWISE / prior cite; **wave22** LSHIFT-RSHIFT / ZERO-EQUALS / TO-NUMBER / SEARCH-WORDLIST / prior cite; **wave23** CHAR-PLUS / HOLD / U-LESS / ABS-NEGATE / **this cite refresh**.

## 5. Repo layout (as-built vs target)

| Path | Role |
|------|------|
| `forth/tritium/` | kernel, interpret, colon, drena, rekia, queue, assimilate, lineos, integrate, master, fleet |
| `forth/trit.fs` | trit+/trit*/pack-neuron-header (`TRIT-MATH.md`) |
| `tritium.poly/` | Bundle core + manifest |
| `install/hosts/{windows,android,linux,_template}` | Host bridges + `parity/HOST-PARITY.txt` |
| `tools/` | build-*, tritium-license, tritium-master, tritium-fleet, tritium-lineos, demos |
| `evolve/` | Runtime state (mostly gitignored); `assistant-state.trit` v2 |
| `queue/`, `assimilate/`, `license/`, `lineos/`, `master/`, `fleet/`, `userland/` | Subsystem notes / validators / scaffolds |
| `refs/` | DuskOS / CollapseOS / GrapheneOS references |
| `docs/` | Specs (this file + §4) |

Target layout in `TritiumOS.txt` §7 still lists top-level `/drena` `/rekia` `/boot` `/userland` — engines today live under `forth/tritium/` (+ stubs). Landed scaffolds: integrate (wave6 **4**), userland (wave8 **3**), host-parity (wave8 **4**), assistant-state v2 (wave9 **3**), host-boot markers (wave10 **1**), refined cold-load (wave10 **4**), loop stubs (wave11 **1**), S0 deepen (wave11 **2**), WORDS list (wave11 **3**), AppImage refined host path (wave11 **4**), counted-loop stubs (wave12 **1**), VARIABLE/CONSTANT stubs (wave12 **2**), comment-parse (wave12 **3**), LEAVE/AGAIN stubs (wave12 **4**), VALUE/TO stubs (wave13 **1**), CASE/OF stubs (wave13 **2**), CREATE/DOES> stubs (wave13 **3**), string-lit stubs (wave13 **4**), HERE/ALLOT stubs (wave14 **1**), UNLOOP/J stubs (wave14 **2**), 2VARIABLE/2CONSTANT stubs (wave14 **3**), CATCH/THROW stubs (wave14 **4**), CELL/CELLS/ALIGN/ALIGNED stubs (wave15 **1**), PICK/ROLL/DEPTH/?DUP stubs (wave15 **2**), FILL/ERASE/MOVE/CMOVE stubs (wave15 **3**), IMMEDIATE/POSTPONE stubs (wave15 **4**), DEFER/IS/ACTION-OF stubs (wave16 **1**), MARKER restore-mark stubs (wave16 **2**), BUFFER: named buffer stubs (wave16 **3**), EXIT/QUIT thin markers (wave16 **4**), SYNONYM/ALIAS name-map stubs (wave17 **1**), PARSE/PARSE-NAME markers (wave17 **2**), EVALUATE/INCLUDE mark-only echo (wave17 **3**), RECURSE mark-only (wave17 **4**), `'`/`[']` tick mark (wave18 **1**), FIND/find-xt mark (wave18 **2**), WORD/BL stub markers (wave18 **3**), STATE/COMPILE, query mark (wave18 **4**), CHAR/CHARS/[CHAR] char-unit mark (wave19 **1**), >BODY address mark (wave19 **2**), ENVIRONMENT? query mark (wave19 **3**), SOURCE/PAD thin marks (wave19 **4**), TRUE/FALSE constant marks (wave20 **1**), WITHIN range-check mark (wave20 **2**), ANS COUNT c-addr picture (wave20 **3**), EXECUTE xt-id invoke mark only (wave20 **4**), ACCEPT/REFILL thin marks (wave21 **1**), BASE/HEX/DECIMAL base marks (wave21 **2**), ANS COMPARE string mark (wave21 **3**), AND/OR/XOR/INVERT bitwise marks (wave21 **4**), LSHIFT/RSHIFT shift marks (wave22 **1**), `0=` (optional `0<>`) flag marks (wave22 **2**), `>NUMBER` thin number-parse mark (wave22 **3**), SEARCH-WORDLIST vocab-search mark (wave22 **4**), CHAR+ thin char-unit advance mark (wave23 **1**), HOLD thin pictured-numeric start (wave23 **2**), U< unsigned compare flag mark (wave23 **3**), ABS/NEGATE signed magnitude / negate marks (wave23 **4**). Bare-metal `/boot` later.

## 6. Non-goals (this doc)

- Stack-effect tables (belong in subsystem docs)
- Build command recipes (`BUILD.md`)
- Product decisions (escalate via Chief of Staff)
- Forth behavior change (docs-only tip)

## 7. Acceptance (Test Lab — wave23 **5**)

1. `docs/ARCHITECTURE.md` matches Research draft (byte-cmp OK).
2. §4 index cites wave23 **1–4** docs (`CHAR-PLUS`, `HOLD`, `U-LESS`, `ABS-NEGATE`) plus wave22 **1–4**, wave21 **1–4**, wave20 **1–4**, wave19 **1–4**, wave18 **1–4**, wave17 **1–4**, wave16 **1–4**, wave15 **1–4**, wave14 **1–4**, wave13 **1–4**, wave12 **1–4**, wave11 **1–4**, wave10 **1–4**, and prior wave6–9 index.
3. `docs/IMPLEMENTATION-GAPS.md` refreshed with wave23 **5** cite note + wave23 **1–4** landed markers; prior wave11 AppImage hang class remains closed; wave12–22 cites remain historical.
4. Docs-only: light suite still green (no Forth behavior change required). Prior demos still greppable (`charplus-demo` / `hold-demo` / `uless-demo` / `abs-demo` + prior).
5. No merge.

## 8. Cite

- `TritiumOS.txt` §§1a, 5–8
- `README.md` project layout; subsystem docs in §4
- Wave6–23 tip stack (gate by SHA; unmerged tip stack OK)
