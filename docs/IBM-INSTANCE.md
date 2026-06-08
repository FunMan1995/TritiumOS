# IBM Quantum: ghost instance (422 + no instance visible)

## Your situation

| What you see | What IBM thinks |
|--------------|-----------------|
| **Instances page empty** (no rows) | An Open Plan instance **already exists** for your account |
| **Create instance** → 422 | "only one open plan instance is allowed per account" |
| **TritiumOS / API** | `list_instances` returns **0** instances for your API key |

This is a **stuck / ghost instance** on IBM’s side. It is **not** something you can fix by creating another instance or changing TritiumOS code.

**Stop clicking Create instance** — it will always fail with 422 until IBM repairs the account.

---

## What you can do now

### A. Keep building TritiumOS ($0, no IBM instance)

```powershell
.\tools\test-ibm-quantum.ps1 -Mode aer
```

Local simulator — same Bell-circuit test, no cloud instance required.

### B. Open an IBM support ticket (required for real QPU)

1. Generate a report:

```powershell
.\tools\ibm-support-report.ps1
```

2. Paste **`dist\IBM-Support-Ticket-4000.txt`** into the support form (~2000 chars, no personal info).  
   Longer draft: `IBM-Support-Ticket.txt` (do not use if avoiding PII).

3. Open IBM Quantum support (from https://quantum.cloud.ibm.com/ — Help / support, or IBM Cloud support linked to the same account).

4. **Subject:** `Open Plan ghost instance — 422 but zero instances visible`

5. **Paste this message:**

```
Problem:
- On https://quantum.cloud.ibm.com/instances (region us-east) I see NO instances.
- When I click "Create instance" (Open plan), I get:
  [422] only one open plan instance is allowed per account
  Broker: Qiskit Runtime

Trace IDs:
- 626c-1114ce38fcbbb253
- 626c-ec1ce2f9a4678e1a

API behavior:
- qiskit_ibm_runtime Account.list_instances() returns 0 for my API key
  (channel ibm_quantum_platform), while create-instance still returns 422.

Request:
Please delete the orphaned Open instance OR make my existing Open instance
visible and usable (CRN exposed) so I can run Qiskit Runtime jobs.

Account email / IBM Cloud account: [fill in]
API key created: 2026-06-02 (IBM Quantum Platform)
```

6. Wait for IBM to fix the account or give you a **CRN** to paste into `apikey.json`:

```json
"instance": "crn:..."
```

---

## Quick checks (5 minutes, before ticket)

Do these once — if still empty + 422, it is definitely IBM-side.

1. **Region:** header → **us-east** (Open Plan is not eu-de).

2. **Instances page:** https://quantum.cloud.ibm.com/instances  
   - Open tab  
   - Other plan tabs  
   - **Archive** section at bottom  

3. **Account switcher:** top header — try **every** IBM Cloud account you have. Repeat Instances page.

4. **Dashboard:** https://quantum.cloud.ibm.com/ — any banner about incomplete setup or instance?

5. **New API key:** Home → API key → revoke old, create new, update `apikey.json` only (does not fix ghost instance often, but worth one try).

6. **Diagnostic:**

```powershell
python tools\ibm-list-instances.py
```

If output says `API list_instances (raw): 0 instance(s)` and UI still 422 on create → **support ticket**.

---

## After IBM fixes it

```powershell
# Add "instance": "crn:..." to apikey.json first
.\tools\test-ibm-quantum.ps1 -Mode ibm
```

---

## What does NOT work

- Creating another Open instance (422).
- A second IBM Quantum registration on the same email (usually blocked).
- Waiting indefinitely without a ticket (ghost instances often need backend cleanup).

---

## Optional: second account (only if support is slow)

If you need hardware **before** IBM fixes this account:

- Use a **different email** / IBM Cloud account for a fresh Open registration.
- New `apikey.json` for that account only.
- Keep TritiumOS on **Aer** on the broken account until fixed.

---

## Traces to cite

| Trace | When |
|-------|------|
| `626c-1114ce38fcbbb253` | Earlier create attempt |
| `626c-ec1ce2f9a4678e1a` | Recent create attempt |

Include **both** in the ticket.