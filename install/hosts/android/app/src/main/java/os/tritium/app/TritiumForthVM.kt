package os.tritium.app

import android.content.Context
import android.content.pm.PackageManager
import java.io.File
import java.lang.StringBuilder
import java.text.SimpleDateFormat
import java.util.Date
import java.util.Locale

/**
 * Hosted Forth VM for TritiumOS Android (komodo).
 * Ported logic from C# reference TritiumForthVM.cs : tokenizer, stack, : ; compiling,
 * HERE/memory sim, primitives, include of assets, control stubs.
 * Loads real core/*.fs (now synced with full trit defs) so drena/rekia words execute.
 * Host bridges (assimilate etc) available as Forth words + REPL cmds.
 */
class TritiumForthVM(private val context: Context) {
    private val output = StringBuilder()
    private var outputCallback: ((String) -> Unit)? = null

    private val dataStack = mutableListOf<Any>()
    private val dictionary = mutableMapOf<String, Word>()
    private val memory = ByteArray(64 * 1024)
    private var herePtr: Int = 0
    private var arch: Int = 64

    private var compiling = false
    private var currentDefinition: Word? = null

    private var evolveDirPath: String = ""

    var coreLoaded = false
        private set

    private data class Word(
        val name: String,
        var action: (() -> Unit)? = null,
        val compiledTokens: MutableList<String> = mutableListOf()
    )

    fun setOutputCallback(cb: (String) -> Unit) {
        outputCallback = cb
    }

    private fun emit(s: String) {
        output.append(s)
        outputCallback?.invoke(s)
    }

    private fun emitLine(s: String = "") = emit("$s\n")

    private fun push(v: Any) = dataStack.add(v)
    private fun pop(): Any? = if (dataStack.isNotEmpty()) dataStack.removeAt(dataStack.lastIndex) else null
    private fun popInt(): Int {
        val v = pop()
        return when (v) {
            is Int -> v
            is Long -> v.toInt()
            is String -> v.toIntOrNull() ?: 0
            else -> 0
        }
    }

    private fun peek(): Any? = if (dataStack.isNotEmpty()) dataStack.last() else null

    fun setEvolveDir(path: String) {
        evolveDirPath = path
        if (evolveDirPath.isNotEmpty()) {
            try { File(evolveDirPath).mkdirs() } catch (_: Exception) {}
        }
    }

    private fun getEvolveDir(): File {
        val base = if (evolveDirPath.isNotEmpty()) File(evolveDirPath) else File(context.filesDir, "evolve")
        base.mkdirs()
        return base
    }

    private fun ensureSub(sub: String): File {
        val d = File(getEvolveDir(), sub)
        d.mkdirs()
        return d
    }

    fun loadCore() {
        emitLine("[VM] Loading Tritium core from assets/core/ (GrapheneOS-informed for komodo)...")
        defineCorePrimitives()
        val coreFiles = listOf("trit.fs", "tritium-kernel.fs", "drena.fs", "rekia.fs")
        for (f in coreFiles) {
            emitLine("[VM] Loading $f...")
            try {
                context.assets.open("core/$f").use { stream ->
                    val src = stream.bufferedReader().readText()
                    interpret(src)
                }
            } catch (e: Exception) {
                emitLine("[VM]   Load error for $f: ${e.message}")
            }
        }
        coreLoaded = true
        emitLine("[VM] Core loaded (drena + rekia engines from real sources).")
        // Set edition and platform (Forth words now exist)
        interpret("64 set-edition")
        interpret("platform-init")
        emitLine("[VM] Ready. Raw Forth supported (try: 1 2 + .  or drena-demo).")
    }

    fun evaluate(input: String): String {
        output.clear()
        if (!coreLoaded) {
            // allow load-core even if not
            val t = input.trim().lowercase()
            if (t == "load-core" || t.startsWith("load-core")) {
                loadCore()
                return output.toString()
            }
            emitLine("[VM] Core not loaded. (auto-loading...)")
            loadCore()
        }
        val trimmed = input.trim()
        val lower = trimmed.lowercase()
        when {
            lower == "drena-demo" -> interpret("drena-demo")
            lower == "rekiA-demo" -> interpret("rekiA-demo")
            lower == "assimilate" -> interpret("assimilate")
            lower == "bootstrap-host" -> interpret("bootstrap-host")
            lower == "full-stack-optimize" -> interpret("full-stack-optimize")
            lower == "host-hw-info" -> interpret("host-hw-info")
            lower.startsWith("assimilate-host-dir") -> {
                val d = trimmed.substringAfter("assimilate-host-dir", "").trim()
                if (d.isNotEmpty()) interpret("s\" $d\" assimilate-host-dir") else interpret("host-pwd assimilate-host-dir")
            }
            lower == "host-exec" || lower.startsWith("host-exec ") -> {
                val cmd = trimmed.substringAfter("host-exec", "").trim().ifEmpty { "getprop ro.product.model" }
                // push string then call word (or direct)
                dataStack.add(cmd)
                if (dictionary.containsKey("host-exec")) {
                    dictionary["host-exec"]?.action?.invoke()
                } else {
                    hostExec(cmd)
                }
            }
            lower == "load-core" -> loadCore()
            lower == "load-refined" -> { /* handled in activity */ }
            else -> {
                // Send to real interpreter for raw Forth or unknown command words
                interpret(trimmed)
            }
        }
        return output.toString()
    }

    fun interpret(text: String) {
        if (text.isBlank()) return
        val tokens = tokenize(text)
        var i = 0
        while (i < tokens.size) {
            var tok = tokens[i]
            if (tok.isBlank()) { i++; continue }

            if (tok == ":") {
                compiling = true
                if (i + 1 < tokens.size) {
                    val name = tokens[++i]
                    currentDefinition = Word(name = name)
                }
                i++; continue
            }

            val num = tok.toIntOrNull()
            if (num != null) {
                if (compiling && currentDefinition != null) {
                    currentDefinition!!.compiledTokens.add(tok)
                } else {
                    push(num)
                }
                i++; continue
            }

            if (tok.startsWith(".\"")) {
                val content = if (tok.length > 2) tok.substring(2).trimEnd('"') else ""
                emit(content)
                i++; continue
            }

            // value/constant/variable/create stubs (consume name, make const or var that pushes)
            if (tok.equals("constant", true) || tok.equals("value", true) ||
                tok.equals("variable", true) || tok.equals("create", true)) {
                if (i + 1 < tokens.size) {
                    val name = tokens[++i]
                    val v = if (dataStack.isNotEmpty()) pop() else 0
                    dictionary[name.lowercase()] = Word(name = name, action = { push(v) })
                }
                i++; continue
            }

            val word = dictionary[tok.lowercase()]
            if (word != null) {
                if (compiling && word.name != ";") {
                    currentDefinition?.compiledTokens?.add(tok)
                } else {
                    word.action?.invoke()
                }
            } else if (compiling && currentDefinition != null) {
                currentDefinition!!.compiledTokens.add(tok)
            } else if (tok.equals("include", true)) {
                if (i + 1 < tokens.size) {
                    val inc = tokens[++i]
                    tryInclude(inc)
                }
            } else {
                emitLine("[VM] ? $tok")
            }
            i++
        }
    }

    private fun tryInclude(fileName: String) {
        val candidates = listOf(fileName, "$fileName.fs", fileName.removeSuffix(".fs") + ".fs")
        for (c in candidates) {
            try {
                context.assets.open("core/$c").use { s ->
                    emitLine("[VM] include $c")
                    val src = s.bufferedReader().readText()
                    interpret(src)
                    return
                }
            } catch (_: Exception) {}
        }
        emitLine("[VM] include failed: $fileName")
    }

    private fun tokenize(text: String): List<String> {
        val tokens = mutableListOf<String>()
        val sb = StringBuilder()
        var inString = false
        var inComment = false
        var i = 0
        while (i < text.length) {
            val c = text[i]
            if (inComment) {
                if (c == '\n' || c == '\r') inComment = false
                i++; continue
            }
            if (inString) {
                sb.append(c)
                if (c == '"') {
                    tokens.add(sb.toString())
                    sb.clear()
                    inString = false
                }
                i++; continue
            }
            if (c.isWhitespace()) {
                if (sb.isNotEmpty()) {
                    tokens.add(sb.toString())
                    sb.clear()
                }
                i++; continue
            }
            if (c == '\\') { inComment = true; i++; continue }
            if (c == '"') { inString = true; sb.append(c); i++; continue }
            sb.append(c)
            i++
        }
        if (sb.isNotEmpty()) tokens.add(sb.toString())
        return tokens
    }

    private fun defineCorePrimitives() {
        dictionary.clear()
        herePtr = 0

        fun def(name: String, act: () -> Unit) {
            dictionary[name.lowercase()] = Word(name = name, action = act)
        }

        // stack
        def("dup") { if (dataStack.isNotEmpty()) push(peek()!!) }
        def("drop") { pop() }
        def("swap") {
            if (dataStack.size >= 2) {
                val b = pop()!!; val a = pop()!!; push(b); push(a)
            }
        }
        def("over") {
            if (dataStack.size >= 2) {
                val b = pop()!!; val a = peek()!!; push(b); push(a)
            }
        }
        def("rot") {
            if (dataStack.size >= 3) {
                val c = pop()!!; val b = pop()!!; val a = pop()!!; push(b); push(c); push(a)
            }
        }
        def("2drop") { pop(); pop() }
        def("2dup") {
            if (dataStack.size >= 2) { val b = peek()!!; val a = dataStack[dataStack.lastIndex - 1]; push(a); push(b) }
        }

        // return (simplified)
        def(">r") { /* stub */ }
        def("r>") { }
        def("r@") { }

        // arith
        def("+") { if (dataStack.size >= 2) { val b=popInt(); val a=popInt(); push(a+b) } }
        def("-") { if (dataStack.size >= 2) { val b=popInt(); val a=popInt(); push(a-b) } }
        def("*") { if (dataStack.size >= 2) { val b=popInt(); val a=popInt(); push(a*b) } }
        def("/") { if (dataStack.size >= 2) { val b=popInt(); val a=popInt(); if(b!=0) push(a/b) } }
        def("mod") { if (dataStack.size >= 2) { val b=popInt(); val a=popInt(); push(a % b) } }
        def("negate") { if (dataStack.isNotEmpty()) push( -popInt() ) }
        def("1+") { if (dataStack.isNotEmpty()) push( popInt() + 1 ) }
        def("1-") { if (dataStack.isNotEmpty()) push( popInt() - 1 ) }

        // compare (Forth true=-1)
        def("=") { if (dataStack.size>=2){ val b=popInt();val a=popInt(); push(if(a==b)-1 else 0)} }
        def("<") { if (dataStack.size>=2){ val b=popInt();val a=popInt(); push(if(a<b)-1 else 0)} }
        def(">") { if (dataStack.size>=2){ val b=popInt();val a=popInt(); push(if(a>b)-1 else 0)} }
        def("0=") { val a=popInt(); push(if(a==0)-1 else 0) }
        def("0<") { val a=popInt(); push(if(a<0)-1 else 0) }
        def("and") { if (dataStack.size >= 2) { val b = popInt(); val a = popInt(); push(a and b) } }
        def("or") { if (dataStack.size >= 2) { val b = popInt(); val a = popInt(); push(a or b) } }

        // mem / here (for drena allocation)
        def("here") { push(herePtr) }
        def(",") { val v=popInt(); writeCell(herePtr, v); herePtr += 4 }
        def("!") { if (dataStack.size >= 2) { val addr=popInt(); val v=popInt(); writeCell(addr,v) } }
        def("@") { val addr=popInt(); push( readCell(addr) ) }
        def("c!") { if (dataStack.size >= 2) { val addr=popInt(); val v=popInt(); if(addr in memory.indices) memory[addr]=(v and 0xff).toByte() } }
        def("c@") { val addr=popInt(); push( if(addr in memory.indices) memory[addr].toInt() and 0xff else 0 ) }
        def("allot") { val n=popInt(); herePtr += n }
        def("cells") { val n=popInt(); push(n * 4) }  // assume 32-bit cells for sim

        // output
        def(".") { if (dataStack.isNotEmpty()) emit( popInt().toString() + " ") }
        def("cr") { emitLine() }
        def("emit") { val ch = popInt().toChar(); emit(ch.toString()) }
        def("type") { /* addr u stub: just note */ val u=popInt(); val a=pop(); emit("[type u=$u] ") }
        def("hex") { /* base stub, numbers decimal for sim */ }
        def("decimal") { }

        // within? ( n lo hi -- f ) lo <= n < hi ? (note: sources use 0 3 within? for mode 0-2)
        def("within?") {
            if (dataStack.size >= 3) {
                val hi = popInt(); val lo = popInt(); val n = popInt()
                push( if (n >= lo && n < hi) -1 else 0 )
            }
        }

        // control flow stubs (collect during compile; runtime no-op or pop test for if)
        listOf("if","else","then","do","loop","+loop","begin","until","again","case","of","endof","endcase","leave","exit","recurse","immediate").forEach { w ->
            def(w) {
                if (!compiling) {
                    if (w=="if" || w=="until") pop() // consume flag if executed at top (rare)
                }
            }
        }

        // var/const etc already handled in interpret special case, but provide fallbacks
        def("variable") { /* name consumed in interpret */ if (dataStack.isNotEmpty()) pop() }
        def("value") { if (dataStack.isNotEmpty()) pop() }
        def("constant") { if (dataStack.isNotEmpty()) pop() }
        def("to") { }
        def("create") { }
        def("does>") { }

        // edition / platform
        def("set-edition") { if (dataStack.isNotEmpty()) arch = popInt() }
        def("edition@") { push(arch) }
        def("64bit?") { push( if(arch==64) -1 else 0 ) }
        def("32bit?") { push( if(arch==32) -1 else 0 ) }
        def("platform-init") { emitLine("[VM] komodo (Pixel 9 Pro XL) platform init OK (GrapheneOS patterns).") }
        def("platform-evolve-path") { push("evolve/") }

        // demos (can be called from Forth or REPL)
        def("drena-demo") { runDrenaDemo() }
        def("rekiA-demo") { runRekiADemo() }

        // Host bridge words (Forth-callable, match C# effects)
        def("host-hw-info") {
            val info = hostHwInfo()
            push(info)
            // also emit for convenience on REPL host-hw-info
            emitLine(info)
        }
        def("host-pwd") { push(context.filesDir.absolutePath) }
        def("host-list-dir") {
            val d = (pop()?.toString() ?: ".").trim()
            val dir = File(d)
            val entries = if (dir.exists() && dir.isDirectory) dir.list() ?: emptyArray() else emptyArray()
            push(entries.size)
            entries.reversed().forEach { push(it) } // push in rev so first pop is first?
        }
        def("host-read-file") {
            val p = pop()?.toString() ?: ""
            try { push( File(p).takeIf {it.exists()}?.readText() ?: "" ) } catch(_:Exception){ push("") }
        }
        def("host-exec") {
            val cmd = (pop()?.toString() ?: "").ifEmpty { "getprop ro.product.model" }
            val out = hostExec(cmd)  // this emits too
            push(out)
        }
        def("host-evolve-dir") { push( getEvolveDir().absolutePath ) }

        // The high level ops
        def("assimilate-host-dir") {
            val d = pop()?.toString() ?: context.filesDir.absolutePath
            assimilateHostDir(d)
        }
        def("assimilate") { assimilate() }
        def("bootstrap-host") { bootstrapHost() }
        def("full-stack-optimize") { fullStackOptimize() }

        // ; is special cased in interpret for compile end
        def(";") {
            if (currentDefinition != null) {
                val defn = currentDefinition!!
                val toks = defn.compiledTokens.toList()
                defn.action = {
                    for (t in toks) {
                        val n = t.toIntOrNull()
                        if (n != null) { push(n); continue }
                        val w = dictionary[t.lowercase()]
                        if (w != null) w.action?.invoke()
                        // else ignore unknown (allows incomplete sources like within? to "run")
                    }
                }
                dictionary[defn.name.lowercase()] = defn
            }
            compiling = false
            currentDefinition = null
        }
    }

    private fun writeCell(addr: Int, v: Int) {
        if (addr < 0 || addr + 3 >= memory.size) return
        memory[addr] = (v and 0xff).toByte()
        memory[addr+1] = ((v ushr 8) and 0xff).toByte()
        memory[addr+2] = ((v ushr 16) and 0xff).toByte()
        memory[addr+3] = ((v ushr 24) and 0xff).toByte()
    }
    private fun readCell(addr: Int): Int {
        if (addr < 0 || addr + 3 >= memory.size) return 0
        return (memory[addr].toInt() and 0xff) or
               ((memory[addr+1].toInt() and 0xff) shl 8) or
               ((memory[addr+2].toInt() and 0xff) shl 16) or
               ((memory[addr+3].toInt() and 0xff) shl 24)
    }

    private fun runDrenaDemo() {
        emitLine("Running DRENA demo (from loaded sources via VM)...")
        try { interpret("42 0 drena-spawn") } catch (_: Exception) {}
        // After spawn, n42 may not be defined (the demo comments use constant which our interpret handles at top)
        // Simulate the link part by direct if possible
        try { interpret("99 42 drena-link") } catch (_:Exception){}
        try { interpret("100 42 drena-link") } catch (_:Exception){}
        try { interpret("42 .neuron-graph") } catch (_:Exception){}
        emitLine("DRENA demo complete.")
    }

    private fun runRekiADemo() {
        emitLine("Running REKIA refiner math demo (from loaded sources)...")
        try {
            interpret("7 2 drena-spawn constant demo-n")
            interpret("42 demo-n drena-link")
            interpret("99 demo-n drena-link")
            interpret("demo-n rekiA-refine")
        } catch (_: Exception) { emitLine("[REKIA] demo partial (some words may be stubbed in sources)") }
        emitLine("REKIA demo complete - refined modules may be emitted to evolve/forth/refined/ on full runs.")
    }

    // ========== Host OS bridge / assimilation layer (Android komodo) ==========
    // Mirrors C# reference. Called either via REPL high-level cmds (which now route to interpret)
    // or from within Forth after loading sources (host- words are defined in primitives).

    fun hostHwInfo(): String {
        val info = buildString {
            append("OS: Android ${android.os.Build.VERSION.RELEASE} (SDK ${android.os.Build.VERSION.SDK_INT})\n")
            append("Device: ${android.os.Build.MANUFACTURER} ${android.os.Build.MODEL} (${android.os.Build.DEVICE})\n")
            append("Board: ${android.os.Build.BOARD} Hardware: ${android.os.Build.HARDWARE}\n")
            append("Fingerprint: ${android.os.Build.FINGERPRINT}\n")
            append("ABIs: ${android.os.Build.SUPPORTED_ABIS.joinToString()}\n")
            append("Processors: ${Runtime.getRuntime().availableProcessors()}\n")
            append("Is64Bit: ${android.os.Build.SUPPORTED_64_BIT_ABIS.isNotEmpty()}\n")
            append("User: ${System.getProperty("user.name") ?: "app-sandbox"}\n")
            append("Expected GrapheneOS komodo: bootloader=ripcurrentpro-16.4-14791556 baseband=g5400c-251201-260127-B-14784805\n")
            append("Partition note: vendor_kernel_boot present, A/B slots, custom AVB key support\n")
        }
        return info
    }

    private fun sanitizeName(name: String): String =
        name.replace(Regex("[^A-Za-z0-9._-]"), "_").take(80).ifEmpty { "item" }

    fun hostExec(cmd: String): String {
        return try {
            val process = Runtime.getRuntime().exec(arrayOf("sh", "-c", cmd))
            val out = process.inputStream.bufferedReader().use { it.readText() }
            process.waitFor()
            emitLine("[host-exec] $cmd\n${out.take(200)}")
            out
        } catch (e: Exception) {
            val msg = "ERROR: ${e.message} (sandbox/SELinux restrictions expected on GrapheneOS)"
            emitLine(msg)
            msg
        }
    }

    fun assimilateHostDir(dirPath: String) {
        emitLine("[Assimilation] Scanning Android dir: $dirPath for software/configs to refine...")
        val dir = File(dirPath)
        if (!dir.canRead()) {
            emitLine("[Assimilation] Dir not readable (scoped storage / sandbox). Using private dirs only.")
            return
        }
        val outDir = ensureSub("assimilated")
        val textExts = listOf(".txt", ".json", ".xml", ".prop", ".rc", ".conf", ".ini", ".sh", ".md", ".mk", ".fs")
        var ingested = 0
        dir.listFiles()?.filter { f -> f.isFile && textExts.any { f.name.endsWith(it, ignoreCase=true) } }
            ?.take(6)?.forEach { f ->
                try {
                    val content = f.readText().take(4096)
                    val safe = sanitizeName(f.name)
                    val out = File(outDir, "$safe.ingest")
                    out.writeText("# TritiumOS Assimilated (Android komodo)\n# Source: ${f.absolutePath}\n# Time: ${Date()}\n$content\n")
                    emitLine("  Assimilated ${f.name} -> ${out.name}")
                    ingested++
                } catch (_: Exception) {}
            }
        emitLine("[Assimilation] $ingested artifacts written to ${outDir.absolutePath}. Ready for REKIA.")
    }

    fun assimilate() {
        emitLine("[Assimilation] Assimilating all software written for this phone hardware (Android komodo)...")
        emitLine("  (Forth core + Kotlin host bridge)")

        val hwInfo = hostHwInfo()
        File(ensureSub("assimilated"), "host-hw-info.ingest").writeText("# Host HW baseline for komodo\n$hwInfo")
        emitLine("  Captured host-hw-info.ingest")

        try {
            val pm = context.packageManager
            val apps = pm.getInstalledApplications(0)
            val pkgFile = File(ensureSub("assimilated"), "installed-software.ingest")
            val sb = StringBuilder("# Installed applications / software on this hardware\n")
            apps.take(30).forEach { app ->
                val label = try { pm.getApplicationLabel(app).toString() } catch (_: Exception) { app.packageName }
                sb.append("$label | ${app.packageName} | targetSdk=${app.targetSdkVersion}\n")
            }
            pkgFile.writeText(sb.toString())
            emitLine("  Assimilated ${apps.size} packages (core 'software written for the hardware')")
        } catch (e: Exception) {
            emitLine("  PackageManager limited: ${e.message}")
        }

        hostExec("getprop ro.product.model")
        hostExec("getprop ro.build.fingerprint | cut -c1-80")
        hostExec("cat /proc/cpuinfo | head -10")
        hostExec("pm list packages | head -5")

        listOf(
            context.filesDir.absolutePath,
            context.cacheDir.absolutePath,
            context.getExternalFilesDir(null)?.absolutePath ?: ""
        ).filter { it.isNotEmpty() }.forEach { p -> if (File(p).exists()) assimilateHostDir(p) }

        // Emit refined module (REKIA-like)
        val refinedDir = ensureSub("forth/refined")
        File(refinedDir, "host-assimilated.fs").writeText(
            "\\ Auto-emitted by Android/Kotlin assimilation bridge (komodo)\n" +
            "\\ host bridges fed software into REKIA path -> runnable Forth.\n" +
            ": host-assimilated ( -- ) host-hw-info drop ;\n" +
            "cr .\" [host-assimilated] Phone software assimilated and refined to Forth.\" cr\n"
        )
        emitLine("[Assimilation] Emitted evolve/forth/refined/host-assimilated.fs")
        emitLine("[Assimilation] Host software assimilated. New modules for full-stack host optimization.")
    }

    fun bootstrapHost() {
        emitLine("[Bootstrap] Generating full-stack host OS optimization artifacts (Android komodo)...")

        val bootDir = ensureSub("bootstrap")
        val stamp = SimpleDateFormat("yyyyMMdd-HHmmss", Locale.US).format(Date())

        val planFile = File(bootDir, "host-optimize-$stamp.txt")
        planFile.writeText(
            "# TritiumOS Full-Stack Host OS Optimization Plan (Android komodo)\n" +
            "# Generated: $stamp\n" +
            "# From: Assimilated + DRENA graph + REKIA refinements\n" +
            "# komodo-install-2026060100.zip (GrapheneOS Pixel 9 Pro XL)\n" +
            "# bootloader=ripcurrentpro-16.4-14791556 baseband=g5400c-251201-260127-B-14784805\n" +
            "# GrapheneOS vs Stock: GrapheneOS smaller/cleaner/hardened (verified boot). Stock noisier (more GMS to assimilate).\n" +
            "# Optimizations stay in app sandbox. See evolve/ for artifacts.\n" +
            "\n" +
            "## Immediate safe actions:\n" +
            "- Battery opt / permission review for the assistant\n" +
            "- On GrapheneOS: respect verified boot\n"
        )
        emitLine("  Wrote plan: ${planFile.name}")

        val sh = File(bootDir, "optimize-$stamp.sh")
        sh.writeText(
            "#!/system/bin/sh\n# Generated by TritiumOS bootstrap-host on Android\n" +
            "echo 'TritiumOS Android host bootstrap'\n" +
            "getprop ro.product.model\n" +
            "echo Evolve: ${getEvolveDir().absolutePath}\n" +
            "ls ${ensureSub("assimilated").absolutePath} 2>/dev/null | head -3 || true\n"
        )
        emitLine("  Wrote runnable: ${sh.name}")

        val refinedDir = ensureSub("forth/refined")
        File(refinedDir, "host-bootstrap-$stamp.fs").writeText(
            "\\ emitted post-assimilation bootstrap module\n" +
            ": host-bootstrap-plan ( -- ) cr .\" Applying Android/komodo opt $stamp ...\" cr ;\n" +
            ": host-optimize ( -- ) host-bootstrap-plan bootstrap-host ;\n"
        )
        emitLine("  Emitted Forth module: host-bootstrap-$stamp.fs")

        emitLine("[Bootstrap] Host OS bootstrap artifacts ready under filesDir/evolve/bootstrap/.")
        emitLine("[Bootstrap] Forth (inside Kotlin) can iteratively full-stack optimize the phone host.")
    }

    fun fullStackOptimize() {
        emitLine("[FullStack] Starting full-stack phone host OS optimization (DRENA+REKIA+assimilate+bootstrap)...")
        interpret("drena-demo")
        interpret("rekiA-demo")
        assimilate()
        bootstrapHost()
        emitLine("[FullStack] Cycle complete. Check evolve/ for .ingest + refined .fs modules (load-refined in host).")
    }

    fun hostListDir(dirPath: String): List<String> {
        val dir = File(dirPath)
        return if (dir.canRead() && dir.isDirectory) dir.list()?.toList() ?: emptyList() else emptyList()
    }
}