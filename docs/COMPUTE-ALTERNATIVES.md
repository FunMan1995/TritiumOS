# Compute alternatives for TritiumOS (IBM blocked)

IBM Quantum Open Plan is **blocked** on your account (ghost instance: 422, zero visible instances).  
Use this matrix to pick **where Qwantum / R.E.K.I.A. / queue jobs** run until IBM fixes it.

**Legend:** $ = paid · ★ = best fit for TritiumOS right now

---

## Summary table

| Rank | Provider | Real QPU | Simulator | API / SDK | Typical cost | TritiumOS fit |
|------|----------|----------|-----------|-----------|--------------|---------------|
| ★1 | **Qiskit Aer (local)** | No | Yes | Qiskit | **$0** | Default dev; already works |
| ★2 | **Amazon Braket** | Yes | Yes (local + cloud) | `amazon-braket-sdk`, Qiskit bridge | **$0** local; cloud sim 1 hr/mo free¹; QPU ~$0.73/1k shots² | Best IBM replacement |
| 3 | **Google Quantum AI** | Limited / program | Cirq sim | Cirq | Research access; not instant signup | Future `qd` adapter |
| 4 | **Azure Quantum** | Yes | Yes | Q#, Qiskit via workspace | Per-shot; Azure credits | If you already pay Azure |
| 5 | **IonQ Cloud** | Yes | Via partners | IonQ API | Higher $/shot | Via Braket/Azure, not direct first |
| 6 | **Quantum Inspire** | Small QPU | Yes | Web + API | Low / academic | EU; secondary |
| — | **IBM Quantum Open** | Yes | Yes | Qiskit Runtime | **$0** when working | **Blocked for you** — support ticket |

¹ Braket SV1 managed sim: 1 hour/month free first 12 months ([pricing](https://aws.amazon.com/braket/pricing/)).  
² Example: Rigetti on Braket, 1000 shots + task fee.

---

## ★1 Local simulator (use now)

| | |
|--|--|
| **What** | Qiskit Aer on your PC |
| **Cost** | $0 |
| **Setup** | `pip install qiskit qiskit-aer` |
| **Test** | `.\tools\test-ibm-quantum.ps1 -Mode aer` |
| **Pros** | No account, no instance, matches Forth/Qiskit stack |
| **Cons** | Not real hardware; no Assimilate fleet offload |

**Verdict:** Primary compute for TritiumOS until cloud QPU is unlocked.

---

## ★2 Amazon Braket (best cloud alternative)

| | |
|--|--|
| **Sign up** | https://aws.amazon.com/braket/getting-started/ |
| **Console** | https://console.aws.amazon.com/braket/ |
| **Pricing** | https://aws.amazon.com/braket/pricing/ |

| Mode | Cost | Notes |
|------|------|--------|
| **Local simulator** (SDK) | **$0** | No AWS charge; runs on your machine |
| **SV1 cloud sim** | Free tier then ~$0.075/min | Needs AWS account |
| **Rigetti QPU** | ~$0.30/task + $0.000425/shot | Cheapest Braket hardware |
| **IQM** | Low per-shot | Good budget option |
| **IonQ on Braket** | ~$0.08/shot | Avoid for budget tests |

```powershell
pip install amazon-braket-sdk
.\tools\test-braket.ps1              # local sim, $0
.\tools\test-braket.ps1 -Cloud       # needs AWS credentials
```

**Pros:** No “one instance” ghost issue; multi-vendor QPUs; fits hybrid queue story.  
**Cons:** AWS account + IAM; real QPU costs money (set billing alarm).

---

## 3 Google Quantum AI (Cirq)

| | |
|--|--|
| **Site** | https://quantumai.google/ |
| **Hardware** | Often via **application / research** access, not open instant QPU like old IBM |
| **SDK** | Cirq (+ simulators free locally) |

```bash
pip install cirq
```

**Verdict:** Good for research later; not the fastest path to replace IBM **this week**.

---

## 4 Azure Quantum

| | |
|--|--|
| **Pricing** | https://azure.microsoft.com/pricing/details/azure-quantum/ |
| **SDK** | Q# workspace; Qiskit possible via providers |

**Verdict:** Use only if you already have Azure credits/subscription.

---

## 5 IonQ (direct cloud)

| | |
|--|--|
| **Site** | https://www.ionq.com/quantum-cloud |
| **Credits** | Research programs sometimes |

**Verdict:** Usually reach IonQ **through Braket or Azure**, not a separate first integration.

---

## 6 Quantum Inspire

| | |
|--|--|
| **Site** | https://www.quantum-inspire.com/ |

**Verdict:** Backup EU platform; smaller ecosystem than Braket/Qiskit.

---

## Recommended path for TritiumOS

```
Now (IBM blocked):
  TritiumOS dev  →  Aer local ($0)
  Optional       →  Braket local sim ($0, test-braket.ps1)

Next (cloud, still low $):
  AWS account    →  Braket SV1 free hour / Rigetti small shot count
  Config         →  qwantum/providers.json + tritium.poly backends

Later (IBM fixed):
  Add back ibm_quantum:open as second backend in config
```

### Suggested `tritium.poly` backend config (today)

```json
{
  "backends": {
    "sim_default": "aer_local",
    "sim_cloud": "braket_local",
    "qpu_budget": "braket:rigetti_cepheus",
    "qpu_premium": "braket:ionq_forte",
    "ibm_quantum": "disabled_until_instance_visible"
  }
}
```

---

## Cost guardrails (all providers)

- Default **simulator** in assistant and queue.
- Require explicit flag for **QPU** (`TRITIUM_ALLOW_QPU=1`).
- Cap shots (e.g. 500) and log to `evolve/qwantum-jobs.log`.
- AWS: enable billing alerts before first Braket QPU job.

---

## Quick commands

```powershell
# Local Qiskit (works today)
.\tools\test-ibm-quantum.ps1 -Mode aer

# Braket local (no AWS bill)
.\tools\test-braket.ps1

# IBM (only after support fixes instance)
.\tools\test-ibm-quantum.ps1 -Mode ibm
```

See also: `docs/QUANTUM-PROVIDERS.md`, `docs/IBM-INSTANCE.md`.