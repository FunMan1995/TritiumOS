# Headless S0 assist smoke (Windows host twin)

Exercises `TritiumForthVM.S0Assist` / `S0AssistDemo` on **net8.0** without WinForms / `net8.0-windows`.

```bash
dotnet run --project install/hosts/windows/smoke-s0
```

Expect:
```
[s0-assist-demo] OK — refined written; word live
[s0-assist-smoke] OK — host twin S0AssistDemo green (net8.0 headless)
```

Forbidden in output: `parity print`, `Example response`, scaffold stubs.
