#!/usr/bin/env python3
"""TritiumOS Braket smoke test - local simulator ($0) or cloud device (AWS creds)."""
from __future__ import annotations

import os
import sys


def bell_circuit_braket():
    from braket.circuits import Circuit

    c = Circuit()
    c.h(0)
    c.cnot(0, 1)
    return c


def run_local(shots: int = 1000) -> dict:
    from braket.devices import LocalSimulator

    device = LocalSimulator()
    task = device.run(bell_circuit_braket(), shots=shots)
    result = task.result()
    counts = result.measurement_counts
    return {"backend": "braket_local_simulator", "shots": shots, "counts": dict(counts)}


def run_cloud(device_arn: str, shots: int = 100) -> dict:
    from braket.aws import AwsDevice

    device = AwsDevice(device_arn)
    task = device.run(bell_circuit_braket(), shots=shots)
    result = task.result()
    return {
        "backend": device_arn,
        "shots": shots,
        "counts": dict(result.measurement_counts),
        "task_arn": str(task.id),
    }


def _shots_default(cloud: bool) -> int:
    env = os.environ.get("TRITIUM_BRAKET_SHOTS")
    if env:
        return int(env)
    try:
        from compute_config import load_config

        cfg = load_config()
        max_s = int(cfg.get("max_shots", 0))
        base = 100 if cloud else 1000
        return min(base, max_s) if max_s > 0 else base
    except (ImportError, FileNotFoundError, ValueError, TypeError):
        return 100 if cloud else 1000


def main() -> int:
    cloud = os.environ.get("TRITIUM_BRAKET_CLOUD", "").lower() in ("1", "true", "yes")
    if os.environ.get("TRITIUM_QUANTUM_MODE", "auto").lower() == "auto":
        try:
            from compute_config import active_test_provider

            tp = active_test_provider()
            if tp == "braket-cloud":
                cloud = True
            elif tp == "braket":
                cloud = False
            elif tp in ("aer", "ibm"):
                print(
                    f"qd/compute.json active backend is {tp}; "
                    "run: .\\tools\\run-compute.ps1",
                    file=sys.stderr,
                )
                return 2
        except ImportError:
            pass
    shots = _shots_default(cloud)
    arn = os.environ.get(
        "TRITIUM_BRAKET_DEVICE",
        "arn:aws:braket:us-east-1::device/qpu/rigetti/Ankaa-3",
    )

    print("TritiumOS Braket smoke test")
    print(f"mode={'cloud' if cloud else 'local'}")
    print()

    try:
        if cloud:
            print("Cloud mode: AWS credentials required; charges may apply.")
            out = run_cloud(arn, shots)
        else:
            print("Local simulator: $0 AWS usage.")
            out = run_local(shots)
    except ImportError:
        print("Install: pip install amazon-braket-sdk", file=sys.stderr)
        return 2
    except Exception as e:
        print("FAIL:", e, file=sys.stderr)
        if cloud:
            print("Hint: configure AWS CLI or env AWS_ACCESS_KEY_ID / AWS_SECRET_ACCESS_KEY", file=sys.stderr)
        return 1

    print("OK")
    print("  backend:", out["backend"])
    print("  shots: ", out["shots"])
    print("  counts:", out["counts"])
    if "task_arn" in out:
        print("  task_arn:", out["task_arn"])

    log = os.path.normpath(os.path.join(os.path.dirname(__file__), "..", "evolve", "qwantum-jobs.log"))
    os.makedirs(os.path.dirname(log), exist_ok=True)
    with open(log, "a", encoding="utf-8") as f:
        f.write(f"{out}\n")
    print("  log:", log)
    return 0


if __name__ == "__main__":
    sys.exit(main())