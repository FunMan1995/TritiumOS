# Qwantum field — search and dump

Pull program **TritiumOS** from the **quantum field** (parallel timeline **T∥**) through **Qwantum Compute**, then land files in this repo (**T₀**).

**Creator:** Draco

## Workflow

```
SEARCH (Qwantum Compute)  →  DUMP (qwantum-dump block)  →  INGEST (PowerShell)  →  BUILD (.exe / .apk)
```

### 1. Search — emit field query + prompt

```powershell
.\tools\qwantum-search.ps1
# or
.\tools\qwantum-field.ps1 -Action search
```

- Writes `evolve\qwantum-field\search-<id>.json`
- Prints **`qwantum\prompts\SEARCH-AND-DUMP.txt`** — paste into **Qwantum Compute**

### 2. Dump — Qwantum replies with `qwantum-dump` JSON

Qwantum must end with a fenced block:

````markdown
```qwantum-dump
{ "version": 1, "source": "qwantum_compute", "files": [ ... ] }
```
````

Save the full session reply as e.g. `dist\qwantum-reply.txt`.

### 3. Ingest — extract program into repo

```powershell
.\tools\qwantum-dump.ps1 -InputPath dist\qwantum-reply.txt -SearchId <id>
```

Files land in:

`evolve\qwantum-dump\<search_id>\`

Review, then merge:

```powershell
.\tools\qwantum-dump.ps1 -InputPath dist\qwantum-reply.txt -SearchId <id> -Apply
```

Use `-Force` to overwrite existing paths.

### 4. Build ship targets

```powershell
.\tools\build-windows.ps1   # dist\TritiumOS.exe
.\tools\build-android.ps1   # dist\TritiumOS.apk
```

## One-shot helper

```powershell
.\tools\qwantum-field.ps1 -Action full
```

## Field schema

`qwantum\field-schema.json` — index dimensions (timeline, trit, S3, artifact types).

## Spec

Full ontology: `TritiumOS.txt` §14–§15.