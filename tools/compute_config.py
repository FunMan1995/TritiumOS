"""Load TritiumOS qd/compute.json — shared by test scripts."""
from __future__ import annotations

import json
import os
from pathlib import Path


def repo_root() -> Path:
    return Path(__file__).resolve().parent.parent


def config_paths() -> list[Path]:
    root = repo_root()
    env = os.environ.get("TRITIUM_COMPUTE_CONFIG")
    paths = []
    if env:
        paths.append(Path(env))
    paths.extend([
        root / "qd" / "compute.json",
        root / "tritium.poly" / "compute.json",
        root / "evolve" / "compute.json",
    ])
    return paths


def load_config() -> dict:
    for p in config_paths():
        if p.is_file():
            data = json.loads(p.read_text(encoding="utf-8-sig"))
            if p.name == "compute.json" and "active" not in data and "doc" in data:
                continue
            data["_config_path"] = str(p)
            return data
    raise FileNotFoundError(
        "compute.json not found. Create qd/compute.json from qd/compute.json.example"
    )


def active_backend(cfg: dict | None = None) -> str:
    cfg = cfg or load_config()
    return cfg.get("active", "aer_local")


def active_test_provider(cfg: dict | None = None) -> str:
    cfg = cfg or load_config()
    bid = active_backend(cfg)
    backends = cfg.get("backends", {})
    if bid in backends:
        return backends[bid].get("test_provider", bid)
    return bid.replace("_local", "").replace("_cloud", "-cloud")


def resolve_provider(mode: str | None = None) -> str:
    if mode and mode != "auto":
        return mode
    return active_test_provider()


def max_shots(cfg: dict | None = None) -> int:
    cfg = cfg or load_config()
    return int(cfg.get("max_shots", 500))