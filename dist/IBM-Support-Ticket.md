# IBM Quantum Support Ticket (copy into web form)

---

## Detailed description

### 1. Describe in detail the issue you're having.

I am developing **TritiumOS** and registered for the **IBM Quantum Platform Open Plan**. My account is stuck:

- **UI:** https://quantum.cloud.ibm.com/instances (region **us-east**) shows **no instances** (Open tab, other tabs, and Archive checked).
- **Create instance (Open plan)** always fails with:

  > The broker for 'Qiskit Runtime' service returned error, [422, Unprocessable Entity] only one open plan instance is allowed per account.

  **Trace IDs:**
  - `626c-1114ce38fcbbb253`
  - `626c-ec1ce2f9a4678e1a`

- **API (same API key as dashboard):** `Account.list_instances()` returns **0** instances (`ibm_quantum_platform` channel).
- **QiskitRuntimeService** with `region=us-east`, `plans_preference=['open']` fails: no matching instances.

The platform blocks a second Open instance (422) but exposes **no** usable instance in the UI or API.

---

### 2. What did you expect to happen? What happened instead? What would you like to see changed?

| | |
|---|---|
| **Expected** | One visible Open instance with a CRN after registration, **or** successful Create instance once. |
| **Actual** | Empty Instances page; Create → 422; API → 0 instances. |
| **Requested fix** | **Delete** the orphaned Open instance **or** **expose** it (show CRN on Instances page + API) so Qiskit Runtime jobs can run. Confirm which Cloud account owns the hidden instance if mismatched. |

---

### 3. What browser are you working in?

**Microsoft Edge** (Chromium) on **Windows**.

*(Issue also reproduces via Python API only; browser not required for API failure.)*

---

## Steps to reproduce the issue

1. Log in to https://quantum.cloud.ibm.com/ (affected account).
2. Set header region to **us-east**.
3. Open **Instances** → confirm **no** instances listed.
4. Click **Create instance** → select **Open** plan → submit.
5. Observe **422** — only one open plan instance allowed (trace e.g. `626c-ec1ce2f9a4678e1a`).
6. Run Python: `Account.create_account(channel="ibm_quantum_platform", token=API_KEY).list_instances()` → `[]`.
7. Run `QiskitRuntimeService(..., region="us-east", plans_preference=["open"])` → no matching instances.

---

## Additional information

### Code

```python
from qiskit_ibm_runtime.accounts import Account

API_KEY = "<from quantum.cloud.ibm.com dashboard>"

acc = Account.create_account(channel="ibm_quantum_platform", token=API_KEY)
print(acc.list_instances())  # []
```

### Notes

- API key created **2026-06-02**; name on export: **Draco Borg**.
- Not sharing API key in this ticket.
- Local sim (**Qiskit Aer**) works; blocked only on IBM instance / hardware path.
- Stop creating instances after 422 — same error repeats.
- Attach **`ibm-support-report.txt`** from `.\tools\ibm-support-report.ps1` if the form allows files.

---

*End of ticket*