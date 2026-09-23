# Assumptions — TritiumOS

Living list of packing and policy choices. Change only with an explicit product decision (via Chief of Staff / user).

## S3 mode `11` (RESERVED)

- Encoding: low 2 bits of S3 nibble = `3` (`11b`).
- **Leave alone:** `drena-rewire` must **not** auto-advance into or out of RESERVED.
- `set-s3-mode` refuses to write mode `3` as a progression target; spawning with variation `3` is allowed for research, but rewire is a no-op that logs `rewire skipped (S3 RESERVED)`.
- No invented RESERVED behavior until Research + user dictate it (TritiumOS.txt §3.2).

## Smoke (Test Lab)

```
s3-reserved-demo
\ expect: before=3, rewire skipped, after=3
```

## Trit nibble packing

Default remains mod-3 split (`trit-pair@` / `encode-trit`). Dense 2+2 packing is out of scope unless recorded here later.
