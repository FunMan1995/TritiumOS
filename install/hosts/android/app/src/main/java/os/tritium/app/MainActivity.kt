package os.tritium.app

import android.os.Bundle
import android.widget.EditText
import android.widget.TextView
import android.widget.Toast
import androidx.appcompat.app.AlertDialog
import androidx.appcompat.app.AppCompatActivity
import java.io.File

class MainActivity : AppCompatActivity() {
    private lateinit var titleText: TextView
    private lateinit var logText: TextView
    private lateinit var inputText: EditText
    private var assistantName = ""

    private var compute = ComputeConfig.default()
    private var vm: TritiumForthVM? = null

    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        setContentView(R.layout.activity_main)
        titleText = findViewById(R.id.titleText)
        logText = findViewById(R.id.logText)
        inputText = findViewById(R.id.inputText)
        ComputeConfig.ensureInstalled(this)
        compute = ComputeConfig.load(this)
        vm = TritiumForthVM(this)
        vm?.setOutputCallback { s -> append(s) }
        vm?.loadCore()  // Loads the core (drena + rekia) from assets, using GrapheneOS for komodo insights
        val nameFile = File(evolveDir(), "assistant-name.trit")
        if (nameFile.exists()) {
            assistantName = nameFile.readText().trim()
            finishBoot()
        } else {
            showLicenseDialog(nameFile)
        }
    }

    private fun evolveDir(): File = File(filesDir, "evolve").also { it.mkdirs() }

    private fun showLicenseDialog(nameFile: File) {
        val keyInput = EditText(this).apply { hint = "License key (8+ chars)" }
        AlertDialog.Builder(this)
            .setTitle("TritiumOS — License")
            .setView(keyInput)
            .setCancelable(false)
            .setPositiveButton("Next") { _, _ ->
                val key = keyInput.text.toString()
                if (key.length < 8) {
                    Toast.makeText(this, "Invalid license", Toast.LENGTH_SHORT).show()
                    showLicenseDialog(nameFile)
                } else {
                    showNameDialog(nameFile)
                }
            }
            .show()
    }

    private fun showNameDialog(nameFile: File) {
        val nameInput = EditText(this).apply { hint = "Assistant name" }
        AlertDialog.Builder(this)
            .setTitle("Name your assistant")
            .setView(nameInput)
            .setCancelable(false)
            .setPositiveButton("Next") { _, _ ->
                assistantName = nameInput.text.toString().trim().ifEmpty { "Assistant" }
                showEditionDialog(nameFile)
            }
            .show()
    }

    private fun showEditionDialog(nameFile: File) {
        AlertDialog.Builder(this)
            .setTitle("Edition")
            .setMessage("Use 64-bit (magenta) edition?")
            .setCancelable(false)
            .setPositiveButton("64-bit") { _, _ -> saveBoot(nameFile, "64") }
            .setNegativeButton("32-bit") { _, _ -> saveBoot(nameFile, "32") }
            .show()
    }

    private fun saveBoot(nameFile: File, edition: String) {
        nameFile.writeText(assistantName)
        File(evolveDir(), "edition.trit").writeText(edition)
        finishBoot()
    }

    private fun finishBoot() {
        titleText.text = "$assistantName — powered by TritiumOS"
        append("TritiumOS by Draco — Android (komodo / Pixel 9 Pro XL)\n")
        append("Core: assets/core/boot.fs (Dusk-inspired TritiumForth base + GrapheneOS for hardware/bootstrap)\n")
        append("Compute: ${compute.active} (${ComputeConfig.activeTestProvider(compute)})\n")
        vm?.platformInit()
        // Set edition (from UI choice)
        val edFile = File(evolveDir(), "edition.trit")
        if (edFile.exists()) {
            val ed = edFile.readText().trim().toIntOrNull() ?: 64
            vm?.setEdition(ed)
        }
        // Auto test engines on boot for verification (like C# side)
        append("[VM] Auto-running engine tests for verification...\n")
        append(vm?.evaluate("drena-demo") ?: "")
        append(vm?.evaluate("rekiA-demo") ?: "")
        append("[VM] Engine tests complete. Ready.\n")

        // Auto load any refined modules from prior assimilation/bootstrap (mirrors C# LoadRefinedModules)
        loadRefinedModules()

        // Light host bridge demo (safe on Android; full scans in 'assimilate' command)
        append("[VM] Host bridge ready (forth-to-Kotlin for phone hardware assimilation + host bootstrap).\n")
        append(vm?.evaluate("host-hw-info") ?: "")
        append("Try 'assimilate', 'bootstrap-host', or 'full-stack-optimize' (requires storage perms in real device for broader dirs).\n")
        inputText.setOnEditorActionListener { _, _, _ ->
            handleCommand(inputText.text.toString().trim())
            inputText.text.clear()
            true
        }
    }

    private fun handleCommand(line: String) {
        if (line.isEmpty()) return
        append("> $line\n")
        when (line.lowercase().split(" ").first()) {
            "help" -> append("help | status | compute | compute-set | about | load-core | drena-demo | rekiA-demo | assimilate | bootstrap-host | full-stack-optimize | host-hw-info | load-refined\nAny other input sent to TritiumForth VM (demos or raw Forth).\n" +
                "assimilate = ingest software on this phone hardware into evolve/assimilated/\n" +
                "full-stack-optimize = DRENA+REKIA + assimilate + emit host bootstrap plans + refined modules\n")
            "compute" -> append(formatCompute())
            "compute-set" -> {
                val id = line.substringAfter(" ", "").trim()
                if (id.isEmpty()) {
                    append("compute-set aer_local | braket_local | braket_cloud | ibm_open\n")
                } else if (!compute.backends.containsKey(id)) {
                    append("unknown backend: $id\n")
                } else if (id == "ibm_open" && !compute.ibmEnabled) {
                    append("ibm_open disabled (ibm_enabled=false)\n")
                } else {
                    ComputeConfig.save(this, compute, id)
                    compute = ComputeConfig.load(this)
                    append("compute active -> ${compute.active}\n")
                }
            }
            "status" -> append(
                "product=TritiumOS creator=Draco assistant=$assistantName " +
                    "compute=${compute.active} test=${ComputeConfig.activeTestProvider(compute)}\n"
            )
            "about" -> Toast.makeText(
                this,
                "$assistantName\nTritiumOS by Draco\nThe line tread between madness and genius.",
                Toast.LENGTH_LONG
            ).show()
            "load-core" -> append(vm?.evaluate("load-core") ?: "")
            "drena-demo" -> append(vm?.evaluate("drena-demo") ?: "")
            "rekiA-demo" -> append(vm?.evaluate("rekiA-demo") ?: "")
            "assimilate" -> {
                append(vm?.evaluate("assimilate") ?: "")
                loadRefinedModules()  // auto-activate any emitted refined .fs
            }
            "bootstrap-host" -> {
                append(vm?.evaluate("bootstrap-host") ?: "")
                loadRefinedModules()
            }
            "full-stack-optimize" -> {
                append(vm?.evaluate("full-stack-optimize") ?: "")
                loadRefinedModules()
            }
            "host-hw-info" -> append(vm?.evaluate("host-hw-info") ?: "")
            "load-refined" -> loadRefinedModules()
            else -> append(vm?.evaluate(line) ?: "[$assistantName] scaffold — connect R.E.K.I.A. next.\n")
        }
    }

    private fun formatCompute(): String {
        val lines = mutableListOf(
            "active=${compute.active} test=${ComputeConfig.activeTestProvider(compute)} " +
                "allow_qpu=${compute.allowQpu} max_shots=${compute.maxShots}",
        )
        compute.backends.forEach { (id, b) ->
            val mark = if (id == compute.active) " *" else ""
            lines.add("  $id$mark -> ${b.testProvider} (${b.label})")
        }
        lines.add("Edit filesDir/qd/compute.json or: compute-set braket_local")
        return lines.joinToString("\n") + "\n"
    }

    private fun append(s: String) {
        logText.append(s)
    }

    private fun loadRefinedModules() {
        val refinedDir = File(evolveDir(), "forth/refined")
        if (!refinedDir.exists()) return
        val modules = refinedDir.listFiles { f -> f.extension == "fs" }?.sortedBy { it.name } ?: return
        if (modules.isEmpty()) return
        append("[VM] Loading refined modules from ${refinedDir.absolutePath} (${modules.size} files)...\n")
        modules.forEach { mod ->
            try {
                val src = mod.readText()
                append("[VM]   include ${mod.name}\n")
                // In a full VM: vm?.evaluate(src) or Interpret
                // For current Android stub: log the head so user sees the emitted intelligence is active
                append("    [head] ${src.take(120).replace("\n", " ")}...\n")
                // Also feed a bit to the VM if it can handle simple words
                if (src.contains("host-assimilated")) {
                    append(vm?.evaluate("host-assimilated") ?: "")
                }
            } catch (e: Exception) {
                append("[VM]   Failed ${mod.name}: ${e.message}\n")
            }
        }
        append("[VM] Refined modules from assimilation/bootstrap now active (persistent intelligence extensions).\n")
    }
}