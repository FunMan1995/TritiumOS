# Forth Base References: Dusk OS & Collapse OS for TritiumForth

**Recommendation from user:** DuskOS / CollapseOS are excellent reference material for building the Forth base of TritiumOS (TritiumForth kernel + D.R.E.N.A. + R.E.K.I.A.).

**Why they are perfect for this project:**

- **Forth-first OS design**: Everything (or almost) is Forth. Aligns exactly with spec: "Implementation language: Forth (primary). ... OS soul remains TritiumForth + D.R.E.N.A. + R.E.K.I.A."
- **Minimal bootstrap / self-hosting**: Tiny kernel (<4KB in Dusk) that builds itself up from source. Directly relevant to `tritium.poly` bundle (unpack + bootstrap on host), `tritium-integrate` to new platforms, and R.E.K.I.A. "refining" more Forth words at runtime.
- **Simplicity at all costs** (with documented tradeoffs): Dusk aggressively prioritizes simplicity (see design/simple.txt, limits.txt). Great model for "pure math at R.E.K.I.A. core" — avoid complexity, sidestep it (Forth style).
- **HAL / polyglot / multi-arch**: Clear separation of ARCH, HAL (hardware abstraction layer), arch-specific code. Perfect analogy for Tritium's polyglot hosts (Windows C#, Android Kotlin, future Linux/_template) + 32-bit vs 64-bit editions (address width).
- **Dictionary, units, memory model**: Sophisticated but simple dict (entries, units for modularity/namespaces). `GROUP-<label>/` vocabs and labeled neural groups map beautifully to Dusk "units" + sub-dicts. Neurons/links can be implemented as structs + linked lists (see mem/ll.fs, lib/struct.fs).
- **Code emission / compilers in Forth**: Dusk has full "almost C" compiler (comp/c.fs), Oberon, Lisp — all written in Forth that *generate* code. This is the closest real-world analog to R.E.K.I.A.: on-demand refinement that emits loadable/runnable Forth (or higher) from "knowledge".
- **Storage & evolution**: FAT fs, block I/O, self-modification. Good for `evolve/` persistence, user-graph, refined .fs fragments, assimilate puzzle state.
- **CollapseOS (predecessor)**: Even more minimal, classic block-based Forth, multi-arch assemblers, designed for "scavenged/improvised hardware" and bootstrapping post-collapse. Smaller core.fs for seed ideas. Dusk now contains CollapseOS compatibility layer.
- **Bootstrapping techniques**: Cold boot, interpret loop, xcomp (cross-compilation), building up from primitives. Matches spec Phase 2+ (trit primitives → full engines) and "pure math algorithm refines intelligence into Forth".
- **Documentation culture**: Almost as much .txt doc in fs/doc/ as code. We should emulate (many required docs like NEURON.md, DRENA.md are missing).
- **Power density + self-host**: Full system (kernel + C compiler + editor + drivers + more) in ~6000 LOC for i386. "Lasting Intelligent Near Endless" (L.I.N.E.O.S.) vibe.

**Cloned references in this workspace (for local study):**
- `refs/duskos/` (primary — https://git.sr.ht/~vdupras/duskos , https://duskos.org/ )
- `refs/collapseos/` ( https://git.sr.ht/~vdupras/collapseos , http://collapseos.org/ )

Run `git pull` in them occasionally for updates. Dusk is the live "big brother".

## Key Files & Lessons to Study (Dusk-first)

### Design Philosophy (study first)
- `fs/doc/design/purpose.txt` — "maximally useful during first stage of civilizational collapse" (maps to Tritium "evolve with user", "integrate", "graduate to L.I.N.E.O.S.", resilience).
- `fs/doc/design/simple.txt` — How Forth sidesteps complexity (e.g. no ELF/reloc bloat like tcc; memory-oriented; 35:1 code size win on C compiler). Apply to neuron/refinement math — keep pure and direct.
- `fs/doc/design/limits.txt` — Explicitly accepts limits (e.g. no concurrency for linear simplicity/global state). For Tritium: single-user co-evolution focus, no need for heavy preemption initially.
- `fs/doc/tour.txt` — Hands-on show-and-tell (run inside Dusk).
- `fs/doc/kernel.txt` — "Anatomy of a kernel": cold boot, interpret loop, entry creation (`entry[] code : ] [ ;`), minimal stack/arith/mem words, SYSVARS, ARCH constant.
- `fs/doc/dict.txt` — Full system dictionary spec (SYSVARS, COMPILING, INPTR, units, findentry, etc.).
- `fs/doc/hal.txt` + `fs/doc/arch/core.txt` — HAL separation (critical for our hosts + editions).
- `fs/doc/mem/dict.txt` etc. — Memory model (HERE, alloc, dict entries ~8 bytes each, units).

### Core Implementation Patterns
- `fs/arch/core.fs` — ARCH detection, family, loading arch-specific.
- `fs/mem/dict.fs` — Dictionary implementation (ENTRYSZ, forget, xt>e, words, units/inunit?).
- `fs/mem/` (alloc.fs, arena.fs, ll.fs linked lists, stack.fs, struct.fs, etc.) — Use for neuron records, neural linking data (typed edges as structs or linked), DRENA graph.
- `fs/fs/core.fs` + `fs/fs/fat.fs` — Filesystem abstraction (inspiration for evolve/ persistence, refined fragments, without full POSIX).
- `fs/lib/struct.fs`, `fs/lib/tagl.fs`, `fs/lib/woordtbl.fs` — For labeled groups, neuron headers (S0-S3 trit pairs as bitfields).
- `fs/xcomp/boot.fs` — The bootstrap/cross layer that builds the full system from kernel seed. Study for how to layer Tritium's boot.fs + drena + rekia.
- `fs/comp/c.fs` (and sub) — Forth-written compiler that emits code. **Direct model for R.E.K.I.A.**: take neuron context/links + "K" (knowledge), refine/contract, emit Forth colon defs / CREATE-DOES> bodies, label groups.
- `fs/hal/` + arch/*/hal/ — Primitives like dup/drop/swap/+/!/@/, , (comma), litn, exit, etc. + machine specifics.

### Bootstrap & Build
- `posix/vm.c` + `posix/*.c` / `posix/*.fs` — C implementation of a Dusk kernel/VM (emulates HAL). **Excellent template for Tritium's Windows (C#) and Android (Kotlin) hosts**: thin "VM" layer providing console I/O, file access to bundled poly/, memory, then interpret the Forth core sources. Dusk's POSIX VM is the "gateway" for building.
- `usermode/` — Native-speed on host OS.
- `mk/`, `GNUmakefile`, xcomp/ tools — Cross compilation for different targets (our "polyglot" + 32/64 editions).
- Root build produces self-hosting images.

### CollapseOS (for minimal seed)
- `core.fs` — Master index + lots of classic Forth (CRC, blocks, context for multiple dicts, assemblers).
- Smaller footprint, block-based (traditional Forth editor/storage). Good for ultra-minimal Tritium seed before layering Dusk-like features.
- `doc/`, `files/`, `mk/` — Multi-arch (Z80 etc.), self-assembly.
- Note: Collapse now largely a compat layer inside Dusk.

### Other Gems
- `fs/doc/usage/*.txt` — How to use (word, to, mem, lit, etc.).
- `fs/lib/macro.fs` — Forth macros (useful for neuron header packing words).
- `fs/tests/kernel.fs` etc. — Test harness ideas.
- Dusk has "grid" text UI, editors (ed, bed), which could inspire the assistant REPL + "neuron peek" for power users.
- Self-hosting fully: a running Dusk can improve itself and produce media for another machine — matches Tritium "evolves with its user", "develop on-device", "integrate".

## Recommended Adoption for TritiumOS

1. **Adopt Dusk-style kernel anatomy** in `tritium.poly/core/` (or new `forth/tritium/kernel.fs`):
   - Cold boot / init (SYSVARS analog for edition, assistant name, graph root).
   - Interpret loop (can start simple in host, move to Forth).
   - Entry/dict words.
   - Minimal primitives + trit-specific (`decode-trit` already exists; add `trit+`, `trit*`, `pack-neuron-header` per spec Phase 2).
   - ARCH-like constant (32/64 edition, host type).

2. **Use Dusk mem/ + struct/ for DRENA**:
   - Neuron as fixed + var record (header 2 bytes, id, links, group ref, REKIA cache).
   - Neural linking data as linked lists or arenas (mem/ll.fs, mem/arena.fs).
   - Labeled groups as "units" or named dicts (drena-group, drena-join).

3. **Model R.E.K.I.A. after Dusk comp/**:
   - `rekiA-refine ( neuron ctx -- )` does extract (scope by links) → contract (pure math fixed point) → to-forth (emit colon def or CREATE) → label-group.
   - Write to `evolve/forth/refined/<label>.fs` then `include`.
   - Start with simple emitters; grow the "C compiler" analogy into noliage refinement.

4. **Host integration modeled on posix/ VM**:
   - In C# `Program.cs` and Kotlin `MainActivity`: implement a minimal Forth VM class/object that:
     - Provides the primitive words (stack, mem, I/O via TextBox / EditText + log).
     - Loads/bundles `poly/core/boot.fs` + `trit.fs` + higher (drena, rekia) from assets/files.
     - Exposes host bridges (compute backends as Forth words?).
   - First, make the existing UI REPL actually feed into a real (even tiny) interpreter.
   - Later replace the toy VM with one that can run full Dusk-inspired TritiumForth (or even embed/adapt Dusk kernel if licenses allow — CC0/public domain friendly).

5. **Bootstrap story**:
   - `tritium.poly` = the "seed" (like Dusk's tar of fs/).
   - Build scripts = cross tools (like xcomp/).
   - On first run: "cold boot" the Forth, run assistant naming (store as counted string), edition.
   - R.E.K.I.A. + DRENA grow the system live (self-hosting evolution).

6. **Docs**:
   - Mirror Dusk's style: put detailed docs alongside code in `forth/doc/` or under `docs/`.
   - Fill the missing ones (NEURON.md etc.) using Dusk dict/kernel as template + Tritium neuron spec §3.
   - Add `docs/FORTH-BOOTSTRAP.md` walking through trit → neuron → drena → rekia layering.

7. **Start small (practical next steps)**:
   - Study `refs/duskos/fs/arch/core.fs` + `fs/mem/dict.fs` + `fs/xcomp/boot.fs`.
   - Enhance `forth/trit.fs` with more (trit+, pack-header, .neuron).
   - Add `forth/tritium/` subdir with a minimal kernel.fs modeled on Dusk kernel anatomy (even if initially just comments + existing stubs).
   - In one host (start with Windows C# since easier REPL), wire a *very* basic threaded Forth interpreter that can at least run the current boot.fs + define some words.
   - Implement `drena-spawn` etc. using Dusk dict patterns + trit words.
   - For REKIA demo: a word that on input emits a simple "refined" Forth fragment to a file and `includes` it.

**Licensing note**: Dusk/Collapse are very open (CC0 for much of Dusk). We can study, adapt patterns, and reimplement in Tritium's own style (creator Draco). Don't copy large chunks verbatim without attribution if needed.

**Further exploration**:
- Run Dusk yourself: `cd refs/duskos; make` (needs C toolchain) then `./dusk` or `make rungrid`.
- Dusk Tour inside a running Dusk.
- Read `fs/doc/why.html` (on site) + Collapse "Why Forth?".
- Related: Tumble Forth blog (vulgarization by same author).

This gives Tritium a battle-tested, collapse-resilient, high power-density Forth foundation while we layer the unique D.R.E.N.A. (neuromorphic graph evolution) + R.E.K.I.A. (pure-math → Forth refinement) + quantum/Assimilate elements on top.

Slogan remains: *The line tread between madness and genius.*

Update: Clone the refs, read the design/kernel/dict docs, then implement the base following the "minimal kernel + build up" pattern.
