Windows host — builds dist/TritiumOS.exe

Initial platform: Windows 11 (x64 primary).

Requires: .NET 8 SDK
  https://dotnet.microsoft.com/download/dotnet/8.0

See README-Win11.md for platform specifics, persistence notes, and VM strategy.

Build:
  ..\..\..\tools\build-windows.ps1

Dev run (no .exe yet):
  ..\..\..\tools\run-windows-dev.ps1