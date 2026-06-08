qd/ — Quantum Daemon configuration

compute.json  — single switch for Aer vs Braket vs IBM (active backend)

PowerShell:
  .\tools\compute-config.ps1 -Action list
  .\tools\compute-config.ps1 -Action set -Backend braket_local
  .\tools\run-compute.ps1

Windows app: compute | compute-set braket_local | compute-test