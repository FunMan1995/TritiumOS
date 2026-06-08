# Quantum providers for TritiumOS testing (reasonable cost)

**Note:** “Qwantum Compute” in this project is your refinement/orchestration workflow.
For **real quantum hardware or cloud APIs**, use one of the providers below.

## Recommended stack (cheapest → real QPU)

| Tier | Provider | Cost | Best for |
|------|----------|------|----------|
| **1 — $0** | **Qiskit Aer** (local) | Free | TritiumOS dev, Bell demos, R.E.K.I.A. stubs |
| **2 — $0** | **IBM Quantum Open Plan** | Free QPU time (see limits) | First real hardware jobs |
| **3 — $0–low** | **Amazon Braket simulators** | 1 hr sim/month free (12 mo); then ~$0.075/min | Larger circuits in cloud |
| **4 — pay per shot** | **Amazon Braket QPU** | ~$0.30/task + shots (Rigetti cheapest) | Occasional hardware tests |
| **5** | **Azure Quantum** | Per-shot, no permanent free QPU | If you already use Azure |

---

## 1. IBM Quantum Platform (best free real QPU)

- **Sign up:** https://quantum.cloud.ibm.com/registration  
- **Plan:** **Open** — free QPU execution (**10 minutes per 28-day window** on real hardware).  
- **Promo (Mar 2026):** active Open users can opt in to **+180 minutes over 12 months**  
  ([blog](https://www.ibm.com/quantum/blog/open-plan-updates)).  
- **API:** Qiskit + `qiskit-ibm-runtime`  
- **Simulators:** local sims are free; cloud simulators billed on paid plans.  
- **Paid:** Pay-as-you-go / Flex / Premium when you outgrow Open.

**Why pick this:** Easiest free account, good docs, fits TritiumOS `qd` + Qiskit path in spec.

```python
# pip install qiskit qiskit-ibm-runtime
from qiskit_ibm_runtime import QiskitRuntimeService
QiskitRuntimeService.save_account(channel="ibm_quantum_platform", token="YOUR_API_KEY")
```

---

## 2. Amazon Braket (best $0 sim + cheap shots)

- **Console:** https://console.aws.amazon.com/braket/  
- **Local simulator:** **Free** in Braket SDK (no AWS charge).  
- **Managed sim (SV1):** **1 hour/month free** for first 12 months; then **$0.075/min** (3 s minimum).  
- **Cheapest QPU shots (examples, US pricing):**  
  - Rigetti Cepheus: **$0.000425/shot** + **$0.30/task**  
  - IQM Garnet: **$0.00145/shot** + **$0.30/task**  
  - IonQ: much higher ($0.08/shot) — avoid for budget tests  

**Example:** 1,000 shots on Rigetti ≈ $0.30 task + $0.43 shots ≈ **$0.73** (not including hybrid job VM).

**API:** `amazon-braket-sdk`, optional Qiskit-Braket provider.

---

## 3. Azure Quantum (if you use Microsoft stack)

- **Pricing:** per-shot / provider-specific; **no standing free QPU tier** like IBM Open.  
- **Simulators:** cloud simulators via workspace; cost tied to Azure Quantum workspace usage.  
- **Credits:** occasional research/education programs — not guaranteed.  
- **Docs:** https://learn.microsoft.com/en-us/azure/quantum/pricing  

Use when you already have Azure credits; otherwise IBM Open + Braket sim is simpler.

---

## 4. Quantum Inspire (EU, education-friendly)

- **Site:** https://www.quantum-inspire.com/  
- **Backends:** simulators + smaller QPU access (check current backends page).  
- Good secondary option; IBM/Braket have more straightforward API pricing for automation.

---

## Practical test plan for TritiumOS

1. **Week 1 — $0:** Local `qiskit-aer` for Bell state + `queue-local?` logic.  
2. **Week 2 — $0:** IBM Open account → one hardware job under 10 min quota.  
3. **Week 3 — &lt;$1:** Braket local sim → optional Rigetti 500–1000 shots.  
4. Set one backend in **`qd/compute.json`** (see `docs/QD-COMPUTE.md`):

```powershell
.\tools\compute-config.ps1 -Action set -Backend aer_local
.\tools\run-compute.ps1
```

`tritium.poly/manifest.json` references the same config; Windows/Android apps use `compute` / `compute-set` commands.

---

## Cost guardrails

- Set **max_shots** (e.g. 500) and **max_tasks/day** in config.  
- Default to **simulator**; require explicit flag for real QPU.  
- Log every job id + estimated cost in `evolve/qwantum-jobs.log`.  
- Use IBM Open dashboard + Braket cost explorer alerts.

## Quick test (in repo)

```powershell
# Install deps once
.\tools\test-ibm-quantum.ps1 -InstallDeps

# Copy template and paste IBM key (file is gitignored)
copy apikey.json.example apikey.json

# Auto: uses apikey.json if present, else free Aer
.\tools\test-ibm-quantum.ps1

# Force local simulator only
.\tools\test-ibm-quantum.ps1 -Mode aer

# Force IBM hardware (uses Open plan minutes)
.\tools\test-ibm-quantum.ps1 -Mode ibm
```

**apikey.json** format (from IBM Quantum dashboard export):

```json
{ "name": "...", "description": "IBM Quantum API key", "apikey": "..." }
```

Never commit `apikey.json` — it is listed in `.gitignore`.

### 422 but no instance visible (ghost instance)

UI shows **no instance**, create fails with **422** — IBM account is stuck. See **`docs/IBM-INSTANCE.md`**.

```powershell
.\tools\ibm-support-report.ps1   # paste dist\ibm-support-report.txt to IBM support
.\tools\test-ibm-quantum.ps1 -Mode aer   # keep developing for $0
```

Logs append to `evolve/qwantum-jobs.log`.

## Links

| Provider | Pricing |
|----------|---------|
| IBM | https://www.ibm.com/quantum/pricing |
| IBM plans | https://quantum.cloud.ibm.com/docs/guides/plans-overview |
| AWS Braket | https://aws.amazon.com/braket/pricing/ |
| Azure | https://azure.microsoft.com/pricing/details/azure-quantum/ |