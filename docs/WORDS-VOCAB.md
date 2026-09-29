# WORDS-VOCAB — Thin `WORDS` / dict-list + `words-demo`

**Status:** Shipper-ready stub spec (wave11 item **3**)
**Canonical brief:** Dusk `words` / dict list; `docs/KERNEL.md` (wave7 **5**)
**Sources of truth (code):** `forth/tritium/kernel.fs` (`.words`); Linux `host_words()` in `tritiumos.c`
**Companions:** `docs/KERNEL.md` (thin amend this tip), `docs/INTERPRET.md`, `docs/COLON.md`

## 1. Purpose

Flat dict already supports `.words` / `host_words`. This tip **locks the Lab surface**: document **`WORDS`** (alias), greppable list markers, and **`words-demo`** that creates ≥2 entries then lists them. Not SEARCH-WORDLIST / linked units / full vocab hierarchy.

## 2. Words

| Word | Stack | Notes |
|------|-------|-------|
| `.words` | `( -- )` | Existing; print `[kernel] words (N):` + names |
| `WORDS` / `words` | `( -- )` | **Alias** of `.words` (ANS/Dusk-facing name for demos) |
| `words-count` | `( -- n )` | Optional helper = `ENTRY-COUNT @` / host_entry_count |
| `words-demo` | `( -- )` | See §5 |

Host: bind `WORDS` / `words` → `host_words` (or print identical markers).

## 3. Markers

```
[kernel] words (N): <name> <name> …
[words-demo] OK
[words-demo] FAIL
```

Lab greps `[words-demo] OK` and `words (` with N ≥ 2 after create.

## 4. Stub semantics

- List **flat** table only (gid ignored for global `.words`; group-scoped list is out of scope).
- Empty dict → `words (0):` still OK (demo creates first).
- Truncate/pad names stay NAMELEN policy from KERNEL.
- No requirement to dump body markers or XT.

## 5. `words-demo`

1. `dict-reset` (or host equivalent).
2. `entry-create` / `:` two distinct names (e.g. `alpha` `beta`).
3. Run `WORDS` (or `.words`) → marker with count ≥2; both names greppable in the line.
4. Optional: `words-count` equals listed N.
5. Prior `kernel-demo` / `interpret-demo` / `colon-demo` / `assistant-s0-demo` still OK.
6. `[words-demo] OK`.

## 6. Thin amend — `docs/KERNEL.md`

- Companions: add `WORDS-VOCAB.md`.
- Words table: add `WORDS` / `words` alias + `words-demo`.
- Non-goals: note SEARCH-WORDLIST still later; this tip is list-only.
- Acceptance: Lab smokes `words-demo`.

## 7. Non-goals

- Full SEARCH-WORDLIST / wordlist stack / linked dict
- Group-scoped `WORDS` filter (use `group-vocab-*` later)
- AppImage refined hang (wave11 **4**)
- Docs cites (wave11 **5**)

## 8. Acceptance (Test Lab)

1. `docs/WORDS-VOCAB.md` present (Research byte-copy OK); `KERNEL.md` thin amend present.
2. `words-demo` → OK (markers §3); `kernel-demo` still OK.
3. Regression green (wave11 **1–2** + wave10).
4. No merge.

## 9. Cite

- `docs/KERNEL.md`, `docs/INTERPRET.md`
- `forth/tritium/kernel.fs` (`.words`)
- Dusk `fs/mem/dict.fs` words (stub list only)
