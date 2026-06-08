#!/usr/bin/env python3
"""TritiumOS quantum smoke test — Aer (local) or IBM Quantum Open if token set."""
from __future__ import annotations

import json
import os
import sys
from pathlib import Path


def repo_root() -> Path:
    return Path(__file__).resolve().parent.parent


def load_ibm_token() -> str | None:
    token = os.environ.get("IBM_QUANTUM_TOKEN") or os.environ.get("QISKIT_IBM_TOKEN")
    if token:
        return token.strip()
    key_file = os.environ.get("TRITIUM_APIKEY_FILE")
    path = Path(key_file) if key_file else repo_root() / "apikey.json"
    if not path.is_file():
        return None
    try:
        data = json.loads(path.read_text(encoding="utf-8"))
    except (OSError, json.JSONDecodeError) as e:
        print(f"WARN: could not read {path}: {e}", file=sys.stderr)
        return None
    for field in ("apikey", "api_key", "token", "IBM_QUANTUM_TOKEN"):
        val = data.get(field)
        if isinstance(val, str) and val.strip():
            return val.strip()
    return None


def load_ibm_instance() -> str | None:
    inst = os.environ.get("TRITIUM_IBM_INSTANCE") or os.environ.get("QISKIT_IBM_INSTANCE")
    if inst:
        return inst.strip()
    key_file = os.environ.get("TRITIUM_APIKEY_FILE")
    path = Path(key_file) if key_file else repo_root() / "apikey.json"
    if not path.is_file():
        return None
    try:
        data = json.loads(path.read_text(encoding="utf-8"))
    except (OSError, json.JSONDecodeError):
        return None
    for field in ("instance", "instance_id", "crn", "IBM_INSTANCE"):
        val = data.get(field)
        if isinstance(val, str) and val.strip():
            return val.strip()
    return None


def resolve_ibm_service():
    from qiskit_ibm_runtime import QiskitRuntimeService

    token = load_ibm_token()
    if not token:
        raise RuntimeError("IBM token missing (apikey.json or IBM_QUANTUM_TOKEN)")

    instance = load_ibm_instance()
    if instance:
        label = f"{instance[:48]}..." if len(instance) > 48 else instance
        print(f"Using IBM instance from config: {label}")
        return QiskitRuntimeService(
            channel="ibm_quantum_platform",
            token=token,
            instance=instance,
        )

    from qiskit_ibm_runtime.accounts import Account

    acc = Account.create_account(channel="ibm_quantum_platform", token=token)
    raw = acc.list_instances()
    if not raw:
        raise RuntimeError(
            "IBM API lists 0 instances for this API key. You cannot create a second Open "
            "instance (422). Open https://quantum.cloud.ibm.com/instances (region us-east), "
            "copy your existing instance CRN into apikey.json as \"instance\", or see "
            "docs/IBM-INSTANCE.md. Use -Mode aer for free local tests meanwhile."
        )

    print("No instance in config — auto-selecting (region us-east, plan open).")
    return QiskitRuntimeService(
        channel="ibm_quantum_platform",
        token=token,
        region="us-east",
        plans_preference=["open"],
    )


def bell_circuit():
    from qiskit import QuantumCircuit

    qc = QuantumCircuit(2, 2)
    qc.h(0)
    qc.cx(0, 1)
    qc.measure([0, 1], [0, 1])
    return qc


def run_aer(shots: int = 1024) -> dict:
    from qiskit_aer import AerSimulator

    qc = bell_circuit()
    sim = AerSimulator()
    job = sim.run(qc, shots=shots)
    counts = job.result().get_counts()
    return {"backend": "qiskit_aer (local)", "shots": shots, "counts": counts}


def run_ibm(shots: int = 128) -> dict:
    from qiskit.transpiler.preset_passmanagers import generate_preset_pass_manager

    service = resolve_ibm_service()
    backend = service.least_busy(operational=True, simulator=False)
    qc = bell_circuit()
    pm = generate_preset_pass_manager(backend=backend, optimization_level=1)
    isa = pm.run(qc)
    job = backend.run(isa, shots=shots)
    result = job.result()
    counts = result.get_counts()
    return {
        "backend": str(backend),
        "shots": shots,
        "counts": counts,
        "job_id": getattr(job, "job_id", str(job)),
    }


def _mode_from_qd() -> str | None:
    try:
        from compute_config import active_test_provider
    except ImportError:
        return None
    tp = active_test_provider()
    if tp in ("aer", "ibm"):
        return tp
    if tp in ("braket", "braket-cloud"):
        print(
            f"qd/compute.json active backend uses {tp}; "
            "run: .\\tools\\run-compute.ps1 or .\\tools\\test-braket.ps1",
            file=sys.stderr,
        )
        return "__braket__"
    return None


def main() -> int:
    mode = (os.environ.get("TRITIUM_QUANTUM_MODE") or "auto").lower()
    if mode == "auto":
        qd = _mode_from_qd()
        if qd == "__braket__":
            return 2
        if qd:
            mode = qd
    shots_aer = int(os.environ.get("TRITIUM_AER_SHOTS", "1024"))
    shots_ibm = int(os.environ.get("TRITIUM_IBM_SHOTS", "128"))
    try:
        from compute_config import load_config

        max_s = int(load_config().get("max_shots", 0))
        if max_s > 0:
            shots_aer = min(shots_aer, max_s)
            shots_ibm = min(shots_ibm, max_s)
    except (ImportError, FileNotFoundError, ValueError, TypeError):
        pass

    print("TritiumOS quantum smoke test — TritiumOS by Draco")
    print(f"mode={mode}")
    print()

    try:
        if mode in ("aer", "local", "sim"):
            out = run_aer(shots_aer)
        elif mode in ("ibm", "hardware", "qpu"):
            out = run_ibm(shots_ibm)
        else:
            if load_ibm_token():
                print("Token found (env or apikey.json) — using IBM Quantum (Open plan usage applies).")
                out = run_ibm(shots_ibm)
            else:
                print("No IBM_QUANTUM_TOKEN — using local Aer simulator ($0).")
                out = run_aer(shots_aer)
    except ImportError as e:
        print("Missing package:", e, file=sys.stderr)
        print("Install: pip install qiskit qiskit-aer", file=sys.stderr)
        print("For IBM: pip install qiskit-ibm-runtime", file=sys.stderr)
        return 2
    except Exception as e:
        err = str(e)
        print("FAIL:", err, file=sys.stderr)
        if "only one open plan" in err.lower() or "422" in err:
            print("Hint: docs/IBM-INSTANCE.md — use existing instance, do not create another.", file=sys.stderr)
        elif "No matching instances" in err or "0 instances" in err:
            print("Hint: python tools/ibm-list-instances.py — add CRN to apikey.json", file=sys.stderr)
        return 1

    print("OK")
    print("  backend:", out["backend"])
    print("  shots: ", out["shots"])
    print("  counts:", out["counts"])
    if "job_id" in out:
        print("  job_id: ", out["job_id"])

    log_dir = os.path.join(os.path.dirname(__file__), "..", "evolve")
    os.makedirs(log_dir, exist_ok=True)
    log_path = os.path.join(log_dir, "qwantum-jobs.log")
    with open(log_path, "a", encoding="utf-8") as f:
        f.write(f"{out}\n")
    print("  log:   ", os.path.normpath(log_path))
    return 0


if __name__ == "__main__":
    sys.exit(main())