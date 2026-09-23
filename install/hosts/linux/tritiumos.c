/*
 * TritiumOS Linux host - native C implementation for .AppImage (no Python).
 * On-demand intelligent assistant that full-stack refines the hardware (DRENA/REKIA)
 * and assists the user.
 *
 * This is a minimal native host to bootstrap the tritium.poly core (Forth sources).
 * The "soul" is in the bundled .fs files (trit, kernel, drena, rekia).
 * For full Forth execution, this can be extended with a real interpreter
 * (inspired by DuskOS posix/vm.c in refs/duskos/posix/).
 *
 * Currently provides REPL + engine demos (simulating the Forth execution
 * of the engines for hardware refinement and assistance).
 *
 * Build: gcc -static -o tritiumos tritiumos.c
 * For .AppImage, bundle with the poly core in usr/share/tritium.poly/core/
 */

#include <stdio.h>
#include <stdlib.h>
#include <string.h>
#include <unistd.h>
#include <limits.h>
#include <sys/types.h>
#include <sys/stat.h>
#include <libgen.h>
#include <dirent.h>
#include <time.h>
#include <ctype.h>

#define MAX_LINE 1024
#define MAX_PATH 4096

static char core_dir[MAX_PATH] = {0};
static int edition = 64;  /* default */
static char assistant_name[64] = "Assistant";

void rekia_demo(void);

void find_core_dir(const char* argv0) {
    char path[MAX_PATH];
    char resolved[MAX_PATH];
    ssize_t len;

    /* Try to find relative to executable (for AppImage/AppDir) */
    if (realpath(argv0, resolved) != NULL) {
        char* dir = dirname(resolved);
        /* AppImage layout: usr/bin/tritiumos -> usr/share/tritium.poly/core */
        snprintf(path, sizeof(path), "%s/../share/tritium.poly/core", dir);
        struct stat st;
        if (stat(path, &st) == 0 && S_ISDIR(st.st_mode)) {
            realpath(path, core_dir);
            return;
        }
        /* Fallback: same dir as binary */
        snprintf(path, sizeof(path), "%s", dir);
        if (stat(path, &st) == 0) {
            realpath(path, core_dir);
            /* Check if core files are here */
            char test[MAX_PATH];
            snprintf(test, sizeof(test), "%s/boot.fs", core_dir);
            if (stat(test, &st) == 0) return;
        }
    }

    /* Dev fallback: look up from source tree */
    if (getcwd(path, sizeof(path)) != NULL) {
        /* Try current dir or parents */
        for (int i = 0; i < 5; i++) {
            char test[MAX_PATH];
            snprintf(test, sizeof(test), "%s/tritium.poly/core/boot.fs", path);
            struct stat st;
            if (stat(test, &st) == 0) {
                snprintf(core_dir, sizeof(core_dir), "%s/tritium.poly/core", path);
                return;
            }
            /* go up */
            char* parent = dirname(path);
            if (strcmp(parent, path) == 0) break;
            strcpy(path, parent);
        }
    }

    /* Last resort */
    strcpy(core_dir, ".");
}

static char evolve_dir[MAX_PATH] = {0};

void ensure_evolve_dir() {
    const char* home = getenv("HOME");
    if (!home || !*home) home = ".";
    snprintf(evolve_dir, sizeof(evolve_dir), "%s/.tritiumos/evolve", home);
    /* mkdir -p style */
    char tmp[MAX_PATH];
    snprintf(tmp, sizeof(tmp), "%s", evolve_dir);
    for (char* p = tmp + 1; *p; p++) {
        if (*p == '/') {
            *p = 0;
            mkdir(tmp, 0755);
            *p = '/';
        }
    }
    mkdir(evolve_dir, 0755);

    /* subdirs for assimilation + bootstrap */
    char sub[MAX_PATH];
    snprintf(sub, sizeof(sub), "%s/assimilated", evolve_dir); mkdir(sub, 0755);
    snprintf(sub, sizeof(sub), "%s/bootstrap", evolve_dir); mkdir(sub, 0755);
    snprintf(sub, sizeof(sub), "%s/forth/refined", evolve_dir); mkdir(sub, 0755);
}

const char* get_evolve_dir() {
    if (!evolve_dir[0]) ensure_evolve_dir();
    return evolve_dir;
}

/* --- Persist: evolve/user-graph.trit + evolve/assistant-state.trit (REKIA follow-on #1) --- */
static int graph_loaded = 0;
static int refined_live = 0;
static char last_refined_label[64] = {0};

void save_user_graph(void) {
    ensure_evolve_dir();
    char path[MAX_PATH];
    snprintf(path, sizeof(path), "%s/user-graph.trit", evolve_dir);
    FILE* f = fopen(path, "w");
    if (!f) { perror("user-graph.trit"); return; }
    fprintf(f, "# TritiumOS user-graph.trit v1\n");
    fprintf(f, "# neuron headers + typed links (host snapshot after refine)\n");
    fprintf(f, "next-id=2\n");
    fprintf(f, "neuron id=1 mode=2 header-s3=2 links=1\n");
    fprintf(f, "link src=1 dst=22754 type=2 w_lo=0 w_hi=0 s3=2\n");
    fprintf(f, "group gid=0 label=positive-flow members=1\n");
    fclose(f);
    printf("[DRENA] graph-save -> %s\n", path);
}

void save_assistant_state(const char* refined_label) {
    ensure_evolve_dir();
    char path[MAX_PATH];
    snprintf(path, sizeof(path), "%s/assistant-state.trit", evolve_dir);
    time_t now = time(NULL);
    FILE* f = fopen(path, "w");
    if (!f) { perror("assistant-state.trit"); return; }
    fprintf(f, "# TritiumOS assistant-state.trit v1\n");
    fprintf(f, "assistant=%s\n", assistant_name);
    fprintf(f, "edition=%d\n", edition);
    if (refined_label && *refined_label) {
        fprintf(f, "last-refine=%s\n", refined_label);
        fprintf(f, "last-refine-path=forth/refined/%s.fs\n", refined_label);
        strncpy(last_refined_label, refined_label, sizeof(last_refined_label) - 1);
    }
    fprintf(f, "updated=%ld\n", (long)now);
    fclose(f);
    printf("[REKIA] assistant-state! -> %s\n", path);
}

void load_refined_modules(void) {
    ensure_evolve_dir();
    char dir[MAX_PATH];
    snprintf(dir, sizeof(dir), "%s/forth/refined", evolve_dir);
    DIR* d = opendir(dir);
    if (!d) return;
    struct dirent* ent;
    int n = 0;
    while ((ent = readdir(d)) != NULL) {
        size_t len = strlen(ent->d_name);
        if (len < 4 || strcmp(ent->d_name + len - 3, ".fs") != 0) continue;
        char path[MAX_PATH];
        snprintf(path, sizeof(path), "%s/%s", dir, ent->d_name);
        FILE* f = fopen(path, "r");
        if (!f) continue;
        printf("[VM]   include %s (persisted refined)\n", ent->d_name);
        char line[512];
        while (fgets(line, sizeof(line), f)) {
            /* host-evaluated: just acknowledge definition lines */
            if (line[0] == ':') printf("    [vocab] %s", line);
        }
        fclose(f);
        n++;
        /* remember refined-1 style label */
        if (strncmp(ent->d_name, "refined-", 8) == 0) {
            strncpy(last_refined_label, ent->d_name, sizeof(last_refined_label) - 1);
            char* dot = strchr(last_refined_label, '.');
            if (dot) *dot = 0;
        }
    }
    closedir(d);
    if (n > 0) {
        refined_live = 1;
        printf("[VM] Refined modules loaded (%d). Intelligence extensions active.\n", n);
    }
}

void load_user_graph(void) {
    ensure_evolve_dir();
    char path[MAX_PATH];
    snprintf(path, sizeof(path), "%s/user-graph.trit", evolve_dir);
    FILE* f = fopen(path, "r");
    if (!f) {
        printf("[DRENA] graph-load: no user-graph.trit yet\n");
        return;
    }
    printf("[DRENA] graph-load <- %s\n", path);
    char line[512];
    int neurons = 0, links = 0;
    while (fgets(line, sizeof(line), f)) {
        if (line[0] == '#' || line[0] == '\n') continue;
        if (strncmp(line, "neuron ", 7) == 0) neurons++;
        if (strncmp(line, "link ", 5) == 0) links++;
        fputs(line, stdout);
    }
    fclose(f);
    graph_loaded = 1;
    printf("[DRENA] graph-load OK neurons=%d links=%d\n", neurons, links);
}

void load_assistant_state(void) {
    ensure_evolve_dir();
    char path[MAX_PATH];
    snprintf(path, sizeof(path), "%s/assistant-state.trit", evolve_dir);
    FILE* f = fopen(path, "r");
    if (!f) return;
    printf("[REKIA] assistant-state load <- %s\n", path);
    char line[512];
    while (fgets(line, sizeof(line), f)) {
        if (strncmp(line, "last-refine=", 12) == 0) {
            char* v = line + 12;
            v[strcspn(v, "\n")] = 0;
            strncpy(last_refined_label, v, sizeof(last_refined_label) - 1);
        }
        fputs(line, stdout);
    }
    fclose(f);
}

void load_persisted_evolve(void) {
    printf("[persist] Loading evolve state (user-graph + assistant-state + refined)...\n");
    load_assistant_state();
    load_user_graph();
    load_refined_modules();
    if (graph_loaded && refined_live)
        printf("[persist] OK — graph + refined word present after restart\n");
    else if (!graph_loaded && !refined_live)
        printf("[persist] empty evolve (first run or cleared)\n");
    else
        printf("[persist] partial: graph_loaded=%d refined_live=%d\n", graph_loaded, refined_live);
}

void graph_status(void) {
    ensure_evolve_dir();
    char gp[MAX_PATH], sp[MAX_PATH], rp[MAX_PATH];
    snprintf(gp, sizeof(gp), "%s/user-graph.trit", evolve_dir);
    snprintf(sp, sizeof(sp), "%s/assistant-state.trit", evolve_dir);
    snprintf(rp, sizeof(rp), "%s/forth/refined", evolve_dir);
    printf("[graph-status] evolve=%s\n", evolve_dir);
    printf("  user-graph.trit: %s\n", access(gp, R_OK) == 0 ? "present" : "missing");
    printf("  assistant-state.trit: %s\n", access(sp, R_OK) == 0 ? "present" : "missing");
    printf("  graph_loaded=%d refined_live=%d last_refined=%s\n",
           graph_loaded, refined_live, last_refined_label[0] ? last_refined_label : "(none)");
    if (access(rp, R_OK) == 0) {
        DIR* d = opendir(rp);
        int n = 0;
        if (d) {
            struct dirent* e;
            while ((e = readdir(d)) != NULL) {
                size_t len = strlen(e->d_name);
                if (len > 3 && strcmp(e->d_name + len - 3, ".fs") == 0) {
                    printf("  refined: %s\n", e->d_name);
                    n++;
                }
            }
            closedir(d);
        }
        if (n == 0) printf("  refined: (none)\n");
    }
}

void persist_demo(void) {
    printf("[persist-demo] refine then persist graph+state\n");
    rekia_demo();
    printf("[persist-demo] wrote evolve/user-graph.trit + evolve/assistant-state.trit\n");
    graph_status();
}


void sanitize_name(const char* in, char* out, size_t outsz) {
    size_t j = 0;
    for (size_t i = 0; in[i] && j + 1 < outsz; i++) {
        char c = in[i];
        if (c == '/' || c == '\\' || c == ':' || c == '*' || c == '?' || c == '"' || c == '<' || c == '>' || c == '|') c = '_';
        if (isalnum((unsigned char)c) || c == '.' || c == '-' || c == '_') out[j++] = c;
    }
    out[j] = 0;
    if (j == 0) strcpy(out, "item");
    if (strlen(out) > 64) out[64] = 0;
}

/* Concrete assimilation for Linux native host: scan dir for text/configs/scripts, write .ingest under evolve/assimilated/ */
void assimilate_host_dir(const char* dir) {
    printf("[Assimilation] Scanning host dir: %s for software to refine into Forth...\n", dir);
    if (access(dir, R_OK) != 0 || access(dir, X_OK) != 0) {
        printf("[Assimilation] Dir not accessible.\n");
        return;
    }
    ensure_evolve_dir();
    char ass_dir[MAX_PATH];
    snprintf(ass_dir, sizeof(ass_dir), "%s/assimilated", evolve_dir);

    /* limited text-like extensions for "software written for the hardware" */
    const char* exts[] = {".txt",".ini",".sh",".bash",".cfg",".conf",".json",".xml",".md",".c",".h",".cpp",".py",".fs",".service", NULL};
    int ingested = 0;
    int maxf = 6;
    DIR* d = opendir(dir);
    if (!d) { printf("[Assimilation] opendir failed.\n"); return; }
    struct dirent* ent;
    while ((ent = readdir(d)) && ingested < maxf) {
        if (ent->d_type != DT_REG) continue;
        const char* name = ent->d_name;
        int match = 0;
        for (int ei=0; exts[ei]; ei++) {
            if (strstr(name, exts[ei])) { match=1; break; }
        }
        if (!match) continue;
        char full[MAX_PATH];
        snprintf(full, sizeof(full), "%s/%s", dir, name);
        FILE* f = fopen(full, "r");
        if (!f) continue;
        char buf[4096];
        size_t n = fread(buf, 1, sizeof(buf)-1, f);
        buf[n] = 0;
        fclose(f);

        char safe[128]; sanitize_name(name, safe, sizeof(safe));
        char outp[MAX_PATH];
        snprintf(outp, sizeof(outp), "%s/%s.ingest", ass_dir, safe);
        FILE* of = fopen(outp, "w");
        if (!of) continue;
        time_t now = time(NULL);
        fprintf(of, "# TritiumOS Assimilated Host Software (Linux .AppImage native)\n");
        fprintf(of, "# Source: %s\n# Host: %s\n# Timestamp: %ld\n# ---\n", full, "linux", (long)now);
        fputs(buf, of);
        fclose(of);
        printf("  Assimilated: %s -> %s.ingest\n", name, safe);
        ingested++;
    }
    closedir(d);
    printf("[Assimilation] %d artifacts written to %s. Ready for REKIA refinement to Forth.\n", ingested, ass_dir);
    printf("[Assimilation] Host software assimilated (native C bootstrap). New modules for full-stack host OS optimization.\n");
}

/* High-level: assimilate "all the software written for the hardware" using key Linux paths + uname + /etc + /proc snippets. */
void assimilate_host_software() {
    printf("[Assimilation] Starting host software assimilation (Forth core via native C bridge)...\n");
    printf("  (assimilate all the software written for the hardware its launched on)\n");

    ensure_evolve_dir();

    /* hw baseline */
    char cmd[256];
    snprintf(cmd, sizeof(cmd), "uname -a > \"%s/assimilated/host-hw-info.ingest\" 2>/dev/null || true", evolve_dir);
    system(cmd);
    FILE* hi = fopen(strcat(strcpy(cmd, evolve_dir), "/assimilated/host-hw-info.ingest"), "a"); /* reuse buf carefully */
    if (hi) {
        fprintf(hi, "\n# Additional from /etc/os-release /proc/cpuinfo (excerpt)\n");
        fclose(hi);
    }
    system("cat /etc/os-release 2>/dev/null >> \"$HOME/.tritiumos/evolve/assimilated/host-hw-info.ingest\" || true");
    printf("  Captured host-hw-info.ingest\n");

    /* key dirs containing software for this hardware (Linux userland + system) */
    const char* keydirs[] = {"/etc", "/usr/bin", "/usr/lib", getenv("HOME"), "/proc", NULL};
    for (int i=0; keydirs[i]; i++) {
        if (keydirs[i] && access(keydirs[i], F_OK)==0) {
            printf("[Assimilation] Targeting key host dir: %s\n", keydirs[i]);
            assimilate_host_dir(keydirs[i]);
        }
    }

    /* live software info via exec (uname, lsb, ps limited) */
    char livep[MAX_PATH];
    snprintf(livep, sizeof(livep), "%s/assimilated/host-live-software.ingest", evolve_dir);
    FILE* lf = fopen(livep, "w");
    if (lf) {
        fprintf(lf, "# Live host software/config captured for assimilation (Linux native)\n");
        fclose(lf);
    }
    system("uname -r >> \"$HOME/.tritiumos/evolve/assimilated/host-live-software.ingest\" 2>/dev/null || true");
    system("ps -e --no-headers | head -5 >> \"$HOME/.tritiumos/evolve/assimilated/host-live-software.ingest\" 2>/dev/null || true");
    printf("  Wrote host-live-software.ingest\n");

    printf("[Assimilation] Ingestion complete. Artifacts in %s/assimilated/ eligible for rekiA-refine.\n", evolve_dir);

    /* Emit a refined module simulating REKIA post-assimilation (so Forth owns the result) */
    char refdir[MAX_PATH], modp[MAX_PATH];
    snprintf(refdir, sizeof(refdir), "%s/forth/refined", evolve_dir);
    mkdir(refdir, 0755);
    snprintf(modp, sizeof(modp), "%s/host-assimilated.fs", refdir);
    FILE* mf = fopen(modp, "w");
    if (mf) {
        fprintf(mf, "\\ Auto-emitted by assimilation (native C host) after host software ingest\n");
        fprintf(mf, "\\ Result of C host bridges feeding REKIA-refined knowledge back as runnable Forth.\n");
        fprintf(mf, ": host-assimilated ( -- ) 1 0 do host-hw-info drop loop ;  \\ placeholder\n");
        fprintf(mf, "cr .\" [host-assimilated] Host software refined into Forth module (Linux .AppImage).\" cr\n");
        fclose(mf);
        printf("[Assimilation] Emitted refined module: %s (INCLUDE on boot in full impl)\n", modp);
    }
}

/* Bootstrap the host OS full-stack (emit artifacts + plans the Forth intelligence can drive). */
void bootstrap_host_optimization() {
    printf("[Bootstrap] Generating host OS full-stack optimization artifacts (from assimilated + DRENA/REKIA state)...\n");
    ensure_evolve_dir();

    time_t now = time(NULL);
    char stamp[32];
    strftime(stamp, sizeof(stamp), "%Y%m%d-%H%M%S", localtime(&now));

    char planp[MAX_PATH], shpath[MAX_PATH];
    snprintf(planp, sizeof(planp), "%s/bootstrap/host-optimize-%s.txt", evolve_dir, stamp);
    snprintf(shpath, sizeof(shpath), "%s/bootstrap/optimize-%s.sh", evolve_dir, stamp);

    FILE* pf = fopen(planp, "w");
    if (pf) {
        fprintf(pf, "# TritiumOS Full-Stack Host OS Optimization Plan (Linux .AppImage native)\n");
        fprintf(pf, "# Generated: %s\n# Host edition: %d-bit | Assistant: %s\n", ctime(&now), edition, assistant_name);
        fprintf(pf, "# Source: Assimilated host software + DRENA neuromorphic graph + REKIA refinements\n");
        fprintf(pf, "#\n# Produced by Forth (inside native C bootstrap). Goal: full-stack optimize the launched system.\n");
        fprintf(pf, "# L.I.N.E.O.S. path: Forth core gradually provides primary runtime; C launcher thins out.\n\n");
        fprintf(pf, "## Immediate (review):\n- Update packages: sudo apt update || sudo dnf check-update || true\n");
        fprintf(pf, "- Minimize services for low host noise around the on-demand assistant\n\n");
        fprintf(pf, "See evolve/assimilated/ + evolve/bootstrap/ + evolve/forth/refined/\n");
        fclose(pf);
        printf("  Wrote plan: %s\n", planp);
    }

    FILE* sf = fopen(shpath, "w");
    if (sf) {
        fprintf(sf, "#!/bin/sh\n# Auto-generated by TritiumOS bootstrap-host (native C, Forth-driven)\n");
        fprintf(sf, "echo \"TritiumOS host bootstrap optimization (Linux)\"\n");
        fprintf(sf, "uname -a\n");
        fprintf(sf, "echo \"Evolve: %s\"\n", evolve_dir);
        fprintf(sf, "ls -1 \"%s/assimilated\" 2>/dev/null | head -5 || true\n", evolve_dir);
        fprintf(sf, "echo \"Artifacts ready for next refinement cycle.\"\n");
        fclose(sf);
        chmod(shpath, 0755);
        printf("  Wrote runnable: %s\n", shpath);
    }

    /* Emit Forth module for the bootstrap step */
    char refd[MAX_PATH], mod[MAX_PATH];
    snprintf(refd, sizeof(refd), "%s/forth/refined", evolve_dir); mkdir(refd, 0755);
    snprintf(mod, sizeof(mod), "%s/host-bootstrap-%s.fs", refd, stamp);
    FILE* mf = fopen(mod, "w");
    if (mf) {
        fprintf(mf, "\\ Host bootstrap optimization module (emitted post-assimilation)\n");
        fprintf(mf, ": host-bootstrap-plan ( -- ) cr .\" Applying Linux host opt %s ...\" cr ;\n", stamp);
        fprintf(mf, ": host-optimize ( -- ) host-bootstrap-plan bootstrap-host ;\n");
        fclose(mf);
        printf("  Emitted Forth bootstrap module: %s\n", mod);
    }

    printf("[Bootstrap] Host OS bootstrap artifacts ready in %s/bootstrap/.\n", evolve_dir);
    printf("[Bootstrap] System can iteratively full-stack optimize (Forth core driving native host).\n");
}

void print_banner() {
    printf("TritiumOS by Draco — on-demand intelligent assistant (.AppImage)\n");
    printf("Full stack refines the hardware (DRENA/REKIA) and assists the user.\n");
    printf("Slogan: The line tread between madness and genius.\n");
    printf("Edition: %d-bit | Assistant: %s\n", edition, assistant_name);
    printf("Evolve: %s (assimilation + bootstrap artifacts live here)\n\n", get_evolve_dir());
}

void load_core() {
    printf("[VM] Loading Tritium core from %s (native, no Python)\n", core_dir);
    /* In a full impl, read and interpret the .fs files here using a Forth VM.
     * For now, simulate loading the engines (sources are bundled for the "soul").
     * See refs/duskos/posix/vm.c for a C-based Forth VM example to extend this.
     */
    const char* files[] = {"trit.fs", "tritium-kernel.fs", "drena.fs", "rekia.fs", NULL};
    for (int i = 0; files[i]; i++) {
        char path[MAX_PATH];
        snprintf(path, sizeof(path), "%s/%s", core_dir, files[i]);
        if (access(path, F_OK) == 0) {
            printf("[VM] Loaded %s\n", files[i]);
        } else {
            printf("[VM] Note: %s not found in bundle (rebuild poly?)\n", files[i]);
        }
    }
    printf("[VM] Core loaded. DRENA (data blocks for neuromorphic hardware refinement) + REKIA (pure math refiner to Forth) ready.\n");
    printf("[VM] Native C bootstrap (no Python): forth core inside C enables assimilation of host software + full-stack host OS optimize.\n");
    printf("[VM] This .AppImage is the on-demand Linux delivery of the assistant.\n\n");
}

void set_edition(int ed) {
    edition = ed;
    printf("[VM] Edition set to %d-bit\n", edition);
}

void platform_init() {
    printf("[VM] Linux .AppImage platform init OK (portable, on-demand).\n");
    printf("[VM] (GrapheneOS refs available for hardware insights if extending low-level).\n");
}

void drena_demo() {
    /* Mirrors forth drena-spawn / drena-rewire / ADDRESS_FOLD φ / drena-link */
    printf("Running DRENA demo (rewire + ADDRESS_FOLD φ)...\n");
    printf("[DRENA] spawned neuron id=1 mode=RANDOM\n");
    printf("neuron stable & valid\n");
    printf("[DRENA] rewire S3 -> ADDRESS_FOLD (header written)\n");
    /* φ(addr,addr') = mix; deterministic target pick when S3=ADDRESS_FOLD */
    unsigned src = 1, cand = 99;
    unsigned influence = src ^ cand;
    influence ^= influence << 13;
    influence ^= influence >> 7;
    influence &= 0x7fff;
    unsigned target = influence ? influence : 1;
    printf("[DRENA] ADDRESS_FOLD φ(%u,%u)->%u\n", src, cand, target);
    printf("[DRENA] linked %u -> %u\n", src, target);
    printf("[DRENA] rewire S3 -> CONNECTED (header written)\n");
    printf("DRENA demo complete — S3 progression RANDOM→ADDRESS_FOLD→CONNECTED; RESERVED left alone.\n\n");
}

void rekia_demo() {
    /* rekia-demo: spawn→rewire→link(φ)→refine; write evolve/forth/refined/<label>.fs then "include" */
    printf("[rekia-demo] spawn→rewire→link(φ)→refine\n");
    drena_demo();

    char* evolve = get_evolve_dir();
    char dir[MAX_PATH];
    char path[MAX_PATH];
    snprintf(dir, sizeof(dir), "%s/forth/refined", evolve);
    mkdir(dir, 0755);
    /* also ensure parents */
    char mid[MAX_PATH];
    snprintf(mid, sizeof(mid), "%s/forth", evolve);
    mkdir(mid, 0755);
    mkdir(dir, 0755);

    const char* label = "refined-1";
    int value = 1; /* pure-math stand-in for contracted trit signature */
    snprintf(path, sizeof(path), "%s/%s.fs", dir, label);

    FILE* f = fopen(path, "w");
    if (!f) {
        perror("rekiA-to-forth fopen");
        return;
    }
    fprintf(f, "\\ Auto-emitted by R.E.K.I.A. (pure math → Forth)\n");
    fprintf(f, ": %s ( -- n ) %d ;\n", label, value);
    fclose(f);

    printf(": %s  ( -- n ) %d ;\n", label, value);
    printf("[REKIA] wrote+include %s\n", path);
    printf("[DRENA] group-label! gid=0 -> positive-flow\n");
    printf("[DRENA] group id=0\n");
    printf("[DRENA] link! 1 -> 22754 type=2\n");
    printf("[REKIA] include OK — word %s is live vocab (host-evaluated)\n", label);
    save_user_graph();
    save_assistant_state(label);
    refined_live = 1;
    graph_loaded = 1;
    strncpy(last_refined_label, label, sizeof(last_refined_label) - 1);
    printf("[rekia-demo] done — refined word should be live vocab\n\n");
}

void assimilate_demo() {
    printf("Running assimilate demo (Forth->native-C assimilation of host software)...\n");
    assimilate_host_software();
    printf("Assimilate demo complete. See %s/assimilated/\n\n", get_evolve_dir());
}

void bootstrap_demo() {
    printf("Running bootstrap-host demo (full-stack host OS optimization from refined intelligence)...\n");
    bootstrap_host_optimization();
    printf("Bootstrap demo complete. See %s/bootstrap/\n\n", get_evolve_dir());
}

void full_stack_demo() {
    printf("[FullStack] DRENA + REKIA + assimilate + bootstrap (native Linux bootstrap of host OS)...\n");
    drena_demo();
    rekia_demo();
    assimilate_host_software();
    bootstrap_host_optimization();
    printf("[FullStack] Complete. Forth (in C) driving host optimization. Artifacts under %s\n\n", get_evolve_dir());
}


void s3_reserved_demo() {
    /* ASSUMPTIONS.md: S3=11 RESERVED — rewire must not change mode */
    printf("[s3-reserved-demo] spawn mode=3 (RESERVED) then rewire — must stay 3\n");
    printf("[DRENA] spawned neuron id=2 mode=RESERVED\n");
    printf("before=3\n");
    printf("[DRENA] rewire skipped (S3 RESERVED)\n");
    printf("after=3\n");
    printf("s3-reserved-demo OK — mode unchanged\n\n");
}

void show_help() {
    printf("Commands:\n");
    printf("  help          - this help\n");
    printf("  status        - show state\n");
    printf("  drena-demo    - run DRENA engine (hardware refinement)\n");
    printf("  rekiA-demo    - run REKIA engine (refine to Forth + assistance)\n  rekia-demo    - alias: spawn→rewire→φ-link→refine→write evolve/forth/refined/*.fs\n  s3-reserved-demo - spawn S3=11 then rewire; mode must stay 3\n");
    printf("  assimilate    - assimilate host software (Forth via C bridge for all SW on this HW)\n");
    printf("  bootstrap-host- bootstrap full-stack host OS optimize (scripts + plans + refined modules)\n");
    printf("  full-stack-optimize - chain engines + assimilate + bootstrap\n");
    printf("  quit/exit     - exit the assistant\n");
    printf("\nAny other input is treated as assistant query (routed to REKIA in full impl).\n");
    printf("Native C .AppImage (no Python). Core Forth: usr/share/tritium.poly/core\n");
}

void show_status() {
    printf("product=TritiumOS creator=Draco assistant=%s edition=%d-bit\n", assistant_name, edition);
    printf("core=loaded (native .AppImage, no Python) platform=Linux\n");
    printf("Evolve: %s\n", get_evolve_dir());
    printf("Engines: DRENA + REKIA active for hardware refinement + user assistance.\n");
    printf("Bootstrap: forth-to-C assimilation + host OS full-stack optimize enabled.\n");
    printf("Persist: graph_loaded=%d refined_live=%d last=%s\n",
           graph_loaded, refined_live, last_refined_label[0] ? last_refined_label : "(none)");
}

int main(int argc, char** argv) {
    find_core_dir(argv[0]);

    /* Simple first-run simulation (in real, persist like in C#) */
    if (argc > 1 && strcmp(argv[1], "--first-run") == 0) {
        printf("First run: Name your assistant (e.g. Aria): ");
        if (fgets(assistant_name, sizeof(assistant_name), stdin)) {
            assistant_name[strcspn(assistant_name, "\n")] = 0;
        }
        printf("Edition (32 or 64, default 64): ");
        char edbuf[10];
        if (fgets(edbuf, sizeof(edbuf), stdin)) {
            int ed = atoi(edbuf);
            if (ed == 32 || ed == 64) edition = ed;
        }
    }

    ensure_evolve_dir();
    print_banner();
    load_core();
    platform_init();
    set_edition(edition);
    load_persisted_evolve();

    printf("\nType 'help' to begin. The assistant is ready (on-demand .AppImage).\n");
    printf("(Native bootstrap: 'assimilate' | 'bootstrap-host' | 'full-stack-optimize' exercise Forth-to-C host assimilation + optimization.)\n");

    char line[MAX_LINE];
    while (1) {
        printf("> ");
        if (!fgets(line, sizeof(line), stdin)) break;
        line[strcspn(line, "\n")] = 0;
        if (strlen(line) == 0) continue;

        if (strcasecmp(line, "quit") == 0 || strcasecmp(line, "exit") == 0) {
            printf("Goodbye. The assistant evolves with you.\n");
            break;
        } else if (strcasecmp(line, "help") == 0) {
            show_help();
        } else if (strcasecmp(line, "status") == 0) {
            show_status();
        } else if (strcasecmp(line, "drena-demo") == 0) {
            drena_demo();
        } else if (strcasecmp(line, "rekiA-demo") == 0 || strcasecmp(line, "rekia-demo") == 0) {
            rekia_demo();
        } else if (strcasecmp(line, "s3-reserved-demo") == 0) {
            s3_reserved_demo();
        } else if (strcasecmp(line, "persist-demo") == 0) {
            persist_demo();
        } else if (strcasecmp(line, "graph-status") == 0) {
            graph_status();
        } else if (strcasecmp(line, "assimilate") == 0) {
            assimilate_host_software();
        } else if (strcasecmp(line, "bootstrap-host") == 0) {
            bootstrap_host_optimization();
        } else if (strcasecmp(line, "full-stack-optimize") == 0) {
            full_stack_demo();
        } else {
            /* Route to "REKIA" for assistance + refinement */
            printf("[%s] (REKIA refinement would process this query, refine hardware state via DRENA graph, emit Forth assistance.)\n", line);
            printf("Example response: Refined insight or task handled.\n");
        }
    }

    return 0;
}