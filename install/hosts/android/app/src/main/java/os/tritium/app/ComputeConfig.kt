package os.tritium.app

import android.content.Context
import org.json.JSONObject
import java.io.File

data class ComputeBackend(
    val label: String,
    val testProvider: String,
)

data class ComputeRoot(
    val active: String,
    val allowQpu: Boolean,
    val maxShots: Int,
    val ibmEnabled: Boolean,
    val backends: Map<String, ComputeBackend>,
)

object ComputeConfig {
    fun load(context: Context): ComputeRoot {
        val file = configFile(context)
        if (!file.isFile) return default()
        val root = JSONObject(file.readText())
        if (!root.has("backends")) return default()
        val backends = mutableMapOf<String, ComputeBackend>()
        val bObj = root.getJSONObject("backends")
        bObj.keys().forEach { id ->
            val b = bObj.getJSONObject(id)
            backends[id] = ComputeBackend(
                label = b.optString("label", id),
                testProvider = b.optString("test_provider", id),
            )
        }
        return ComputeRoot(
            active = root.optString("active", "aer_local"),
            allowQpu = root.optBoolean("allow_qpu", false),
            maxShots = root.optInt("max_shots", 500),
            ibmEnabled = root.optBoolean("ibm_enabled", false),
            backends = backends,
        )
    }

    fun activeTestProvider(cfg: ComputeRoot): String =
        cfg.backends[cfg.active]?.testProvider ?: "aer"

    fun save(context: Context, cfg: ComputeRoot, active: String) {
        val file = configFile(context)
        file.parentFile?.mkdirs()
        val root = if (file.isFile) JSONObject(file.readText()) else seedFromAssets(context)
        root.put("active", active)
        root.put("allow_qpu", cfg.allowQpu)
        root.put("max_shots", cfg.maxShots)
        root.put("ibm_enabled", cfg.ibmEnabled)
        file.writeText(root.toString(2))
        syncPolyStub(context, root)
    }

    private fun configFile(context: Context): File =
        File(File(context.filesDir, "qd"), "compute.json")

    private fun seedFromAssets(context: Context): JSONObject {
        context.assets.open("qd/compute.json").use { stream ->
            return JSONObject(stream.bufferedReader().readText())
        }
    }

    private fun syncPolyStub(context: Context, root: JSONObject) {
        val stub = JSONObject()
            .put("version", root.optInt("version", 1))
            .put("active", root.optString("active"))
            .put("allow_qpu", root.optBoolean("allow_qpu"))
            .put("max_shots", root.optInt("max_shots"))
            .put("ibm_enabled", root.optBoolean("ibm_enabled"))
            .put("doc", "See qd/compute.json")
        File(File(context.filesDir, "tritium.poly"), "compute.json").apply {
            parentFile?.mkdirs()
            writeText(stub.toString(2))
        }
    }

    private fun default() = ComputeRoot(
        active = "aer_local",
        allowQpu = false,
        maxShots = 500,
        ibmEnabled = false,
        backends = mapOf(
            "aer_local" to ComputeBackend("Qiskit Aer (local)", "aer"),
            "braket_local" to ComputeBackend("Amazon Braket local simulator", "braket"),
            "braket_cloud" to ComputeBackend("Amazon Braket cloud (AWS)", "braket-cloud"),
            "ibm_open" to ComputeBackend("IBM Quantum Open", "ibm"),
        ),
    )

    fun ensureInstalled(context: Context) {
        val file = configFile(context)
        if (file.isFile) return
        file.parentFile?.mkdirs()
        file.writeText(seedFromAssets(context).toString(2))
    }
}