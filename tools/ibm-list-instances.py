#!/usr/bin/env python3
"""List IBM Quantum instances for this account (Open plan allows only one)."""
from __future__ import annotations

import importlib.util
import sys
from pathlib import Path


def _load_helpers():
    path = Path(__file__).parent / "test-quantum.py"
    spec = importlib.util.spec_from_file_location("test_quantum", path)
    mod = importlib.util.module_from_spec(spec)
    spec.loader.exec_module(mod)
    return mod


def main() -> int:
    tq = _load_helpers()
    token = tq.load_ibm_token()
    if not token:
        print("No token: add apikey.json or set IBM_QUANTUM_TOKEN", file=sys.stderr)
        return 2

    from qiskit_ibm_runtime import QiskitRuntimeService

    print("TritiumOS — IBM Quantum instances")
    print("(Open plan: only ONE instance allowed — use existing, do not create another)")
    print()

    from qiskit_ibm_runtime.accounts import Account

    acc = Account.create_account(channel="ibm_quantum_platform", token=token)
    raw = acc.list_instances()
    print(f"API list_instances (raw): {len(raw)} instance(s)")
    for i, inst in enumerate(raw):
        print(f"  raw[{i}]: {inst}")
    print()

    if not raw:
        print("API sees ZERO instances, but UI may block a second Open instance (422).")
        print("This usually means:")
        print("  - An Open instance exists in IBM records but is not linked to this API key yet")
        print("  - Wrong IBM Cloud account selected in the dashboard header")
        print("  - Instance is Archived (check Archive section on Instances page)")
        print("  - Region is not us-east (Open Plan requires us-east)")
        print()
        print("See docs/IBM-INSTANCE.md — do NOT create another instance.")
        print("If Instances page shows a CRN, add it to apikey.json as \"instance\".")
        return 1

    try:
        service = QiskitRuntimeService(
            channel="ibm_quantum_platform",
            token=token,
            region="us-east",
            plans_preference=["open"],
        )
        instances = service.instances()
    except Exception as e:
        print("QiskitRuntimeService filter warning:", e)
        instances = raw

    for i, inst in enumerate(instances):
        print(f"--- instance {i + 1} ---")
        if isinstance(inst, dict):
            for k, v in inst.items():
                print(f"  {k}: {v}")
        else:
            print(f"  {inst}")
        print()

    try:
        a = service.active_instance()
        if a:
            print("active_instance():", a)
    except Exception as e:
        print("active_instance():", e)

    print()
    print("Fix for 422 error: do NOT click Create instance again.")
    print("Copy CRN or name into apikey.json:")
    print('  "instance": "your-crn-or-name-here"')
    return 0


if __name__ == "__main__":
    try:
        sys.exit(main())
    except Exception as e:
        print("FAIL:", e, file=sys.stderr)
        sys.exit(1)