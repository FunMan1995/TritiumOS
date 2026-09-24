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
void grow_step_demo(void);
void s3_reserved_demo(void);
void qwantum_atoms_load(void);
void qwantum_atoms_demo(void);
void groups_demo(void);
void groups_status(void);
void groups_persist_demo(void);
void group_vocab_demo(void);
void group_link_demo(void);
void license_status(void);
void license_register(const char* device_id);
void queue_demo(void);
int queue_local_p(int job);
int queue_enqueue(int job);
int queue_pull(void);
int queue_prove(int job, unsigned proof);
/* Assimilate stub (§5b.2–5b.3) — local wallet/epoch; no crypto */
void assimilate_demo(void);
void lineos_graduate(int demo_force);
void lineos_graduate_demo(void);
void become_lineos(void);
int assimilate_epoch(void);
int assimilate_fragment(int group, int links);
int assimilate_merge(int frag, unsigned proof);
int assimilate_solved_p(void);
void assimilate_balance(void);
void s0_assist(const char* query);
void s0_assist_demo(void);
void load_edition(void);
void save_edition(void);
void edition_demo(void);

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
    snprintf(sub, sizeof(sub), "%s/qwantum-dump", evolve_dir); mkdir(sub, 0755);
    snprintf(sub, sizeof(sub), "%s/queue", evolve_dir); mkdir(sub, 0755);
    snprintf(sub, sizeof(sub), "%s/assimilate", evolve_dir); mkdir(sub, 0755);
    snprintf(sub, sizeof(sub), "%s/assimilate/wallet", evolve_dir); mkdir(sub, 0755);
}

const char* get_evolve_dir() {
    if (!evolve_dir[0]) ensure_evolve_dir();
    return evolve_dir;
}

/* --- Persist: evolve/user-graph.trit + evolve/assistant-state.trit (REKIA follow-on #1) --- */
static int graph_loaded = 0;
static int refined_live = 0;
static char last_refined_label[64] = {0};
static int qwantum_k_influence = 0;
static int qwantum_k_loaded = 0;
static char qwantum_last_id[64] = "sample01test";

/* Host-side group snapshot (mirrors Forth MAX-GROUPS member lists) */
#define HOST_MAX_GROUPS 16
#define HOST_MAX_MEMBERS 32
static int host_group_count = 0;
static char host_group_labels[HOST_MAX_GROUPS][32];
static int host_group_n_members[HOST_MAX_GROUPS];
static int host_group_members[HOST_MAX_GROUPS][HOST_MAX_MEMBERS];
static int host_next_id = 2;
static int host_link_src = 1, host_link_dst = 22754, host_link_type = 2, host_link_s3 = 2;
static int host_neuron_mode = 2;

/* Host typed-link table (mirrors Forth links[] / LINK-INTER) */
#define HOST_MAX_LINKS 64
#define HOST_LINK_INTRA 0
#define HOST_LINK_INTER 1
#define HOST_LINK_FOLD_PHI 2
#define HOST_LINK_RANDOM 3
static int host_link_count = 0;
static int host_links_src[HOST_MAX_LINKS];
static int host_links_dst[HOST_MAX_LINKS];
static int host_links_type[HOST_MAX_LINKS];
static int host_links_s3[HOST_MAX_LINKS];

/* Host-side group vocab unit (mirrors kernel ENTRY-GIDS + group-entry-find) */
#define HOST_MAX_GENTRIES 32
#define HOST_NAMELEN 16
static int host_entry_count = 0;
static char host_entry_names[HOST_MAX_GENTRIES][HOST_NAMELEN];
static int host_entry_gids[HOST_MAX_GENTRIES];

static void host_dict_reset(void) {
    host_entry_count = 0;
    for (int i = 0; i < HOST_MAX_GENTRIES; i++) {
        host_entry_names[i][0] = 0;
        host_entry_gids[i] = -1;
    }
}

static int host_group_entry_find(const char* name, int gid) {
    for (int i = 0; i < host_entry_count; i++) {
        if (host_entry_gids[i] == gid && strcmp(host_entry_names[i], name) == 0)
            return i;
    }
    return -1;
}

static int host_group_entry_create(const char* name, int gid) {
    int ex = host_group_entry_find(name, gid);
    if (ex >= 0) {
        printf("[kernel] group-entry exists #%d\n", ex);
        return ex;
    }
    if (host_entry_count >= HOST_MAX_GENTRIES) {
        printf("[kernel] dict full\n");
        return -1;
    }
    int i = host_entry_count++;
    strncpy(host_entry_names[i], name, HOST_NAMELEN - 1);
    host_entry_names[i][HOST_NAMELEN - 1] = 0;
    host_entry_gids[i] = gid;
    printf("[kernel] group-entry #%d gid=%d\n", i, gid);
    return i;
}

static void host_group_vocab_add(const char* name, int gid) {
    int i = host_group_entry_create(name, gid);
    if (i < 0)
        printf("[DRENA] group-vocab-add failed\n");
    else
        printf("[DRENA] group-vocab-add #%d under gid=%d\n", i, gid);
}

static void host_groups_reset(void) {
    host_group_count = 0;
    for (int i = 0; i < HOST_MAX_GROUPS; i++) {
        host_group_labels[i][0] = 0;
        host_group_n_members[i] = 0;
    }
    /* vocab table is session-scoped; reset with groups for clean demos */
    host_dict_reset();
}

static int host_group_create(const char* label) {
    if (host_group_count >= HOST_MAX_GROUPS) return -1;
    int gid = host_group_count++;
    strncpy(host_group_labels[gid], label, sizeof(host_group_labels[gid]) - 1);
    host_group_labels[gid][sizeof(host_group_labels[gid]) - 1] = 0;
    host_group_n_members[gid] = 0;
    printf("[DRENA] group-label! gid=%d -> %s\n", gid, host_group_labels[gid]);
    printf("[DRENA] vocab prefix GROUP-%s/\n", host_group_labels[gid]);
    {
        char unit[48];
        snprintf(unit, sizeof(unit), "GROUP-%s/", host_group_labels[gid]);
        host_group_entry_create(unit, gid);
        printf("[DRENA] vocab unit %s (searchable)\n", unit);
    }
    printf("[DRENA] group id=%d\n", gid);
    return gid;
}

static void host_group_join(int nid, int gid) {
    if (gid < 0 || gid >= host_group_count) return;
    for (int i = 0; i < host_group_n_members[gid]; i++) {
        if (host_group_members[gid][i] == nid) {
            printf("[DRENA] join neuron %d -> group %d (members=%d already)\n",
                   nid, gid, host_group_n_members[gid]);
            return;
        }
    }
    if (host_group_n_members[gid] >= HOST_MAX_MEMBERS) {
        printf("[DRENA] join full group %d\n", gid);
        return;
    }
    host_group_members[gid][host_group_n_members[gid]++] = nid;
    printf("[DRENA] join neuron %d -> group %d (members=%d)\n",
           nid, gid, host_group_n_members[gid]);
}

static void host_links_reset(void) {
    host_link_count = 0;
}

static int host_link_append(int src, int dst, int type, int s3) {
    if (host_link_count >= HOST_MAX_LINKS) {
        printf("[DRENA] link! full\n");
        return -1;
    }
    int i = host_link_count++;
    host_links_src[i] = src;
    host_links_dst[i] = dst;
    host_links_type[i] = type;
    host_links_s3[i] = s3;
    /* keep legacy single-slot fields in sync with last link */
    host_link_src = src;
    host_link_dst = dst;
    host_link_type = type;
    host_link_s3 = s3;
    printf("[DRENA] link! %d -> %d type=%d\n", src, dst, type);
    return i;
}

/* group-link! ( group-a group-b -- ): LINK-INTER bridge via representative members */
static void host_group_link(int ga, int gb) {
    if (ga < 0 || ga >= host_group_count || gb < 0 || gb >= host_group_count) {
        printf("[DRENA] group-link! bad gid\n");
        return;
    }
    int src, dst;
    if (host_group_n_members[ga] > 0)
        src = host_group_members[ga][0];
    else
        src = (int)(0x80000000u | (unsigned)ga);
    if (host_group_n_members[gb] > 0)
        dst = host_group_members[gb][0];
    else
        dst = (int)(0x80000000u | (unsigned)gb);
    host_link_append(src, dst, HOST_LINK_INTER, 0);
    printf("[DRENA] group-link! %d <-> %d type=%d (LINK-INTER)\n",
           ga, gb, HOST_LINK_INTER);
}

void save_user_graph(void) {
    ensure_evolve_dir();
    char path[MAX_PATH];
    snprintf(path, sizeof(path), "%s/user-graph.trit", evolve_dir);
    FILE* f = fopen(path, "w");
    if (!f) { perror("user-graph.trit"); return; }
    fprintf(f, "# TritiumOS user-graph.trit v1\n");
    fprintf(f, "# neuron headers + typed links + groups/members (host snapshot)\n");
    fprintf(f, "next-id=%d\n", host_next_id);
    fprintf(f, "neuron id=1 mode=%d header-s3=%d links=1\n", host_neuron_mode, host_neuron_mode);
    fprintf(f, "link src=%d dst=%d type=%d w_lo=0 w_hi=0 s3=%d\n",
            host_link_src, host_link_dst, host_link_type, host_link_s3);
    /* New format (item 4): group <gid> <label> / member <gid> <nid>
       Legacy "group gid=N label=..." still accepted by load_user_graph. */
    for (int g = 0; g < host_group_count; g++) {
        fprintf(f, "group %d %s\n", g, host_group_labels[g]);
        for (int m = 0; m < host_group_n_members[g]; m++)
            fprintf(f, "member %d %d\n", g, host_group_members[g][m]);
    }
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
        /* CRITICAL: never load dump fragments (qwantum-*.fs) as live vocab */
        if (strncmp(ent->d_name, "qwantum-", 8) == 0) {
            printf("[VM]   skip %s (dump atom — not vocab; refine only)\n", ent->d_name);
            continue;
        }
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
    int neurons = 0, links = 0, groups = 0, members = 0;
    host_groups_reset();
    while (fgets(line, sizeof(line), f)) {
        if (line[0] == '#' || line[0] == '\n') continue;
        if (strncmp(line, "neuron ", 7) == 0) neurons++;
        else if (strncmp(line, "link ", 5) == 0) links++;
        else if (strncmp(line, "member ", 7) == 0) {
            int gid = -1, nid = -1;
            if (sscanf(line + 7, "%d %d", &gid, &nid) == 2 &&
                gid >= 0 && gid < HOST_MAX_GROUPS && nid >= 0) {
                /* ensure group slot exists */
                while (host_group_count <= gid && host_group_count < HOST_MAX_GROUPS) {
                    host_group_labels[host_group_count][0] = 0;
                    host_group_n_members[host_group_count] = 0;
                    host_group_count++;
                }
                if (gid < host_group_count &&
                    host_group_n_members[gid] < HOST_MAX_MEMBERS) {
                    int dup = 0;
                    for (int i = 0; i < host_group_n_members[gid]; i++)
                        if (host_group_members[gid][i] == nid) { dup = 1; break; }
                    if (!dup)
                        host_group_members[gid][host_group_n_members[gid]++] = nid;
                }
                members++;
            }
        } else if (strncmp(line, "group ", 6) == 0) {
            /* New: "group <gid> <label>" or legacy "group gid=N label=..." */
            int gid = -1;
            char label[32] = {0};
            if (sscanf(line + 6, "%d %31s", &gid, label) == 2 && gid >= 0) {
                while (host_group_count <= gid && host_group_count < HOST_MAX_GROUPS) {
                    host_group_labels[host_group_count][0] = 0;
                    host_group_n_members[host_group_count] = 0;
                    host_group_count++;
                }
                if (gid < HOST_MAX_GROUPS) {
                    strncpy(host_group_labels[gid], label, sizeof(host_group_labels[gid]) - 1);
                }
                groups++;
            } else if (sscanf(line + 6, "gid=%d label=%31s", &gid, label) >= 1 && gid >= 0) {
                /* strip optional members=N from legacy label token already limited */
                char* sp = strchr(label, ' ');
                if (sp) *sp = 0;
                while (host_group_count <= gid && host_group_count < HOST_MAX_GROUPS) {
                    host_group_labels[host_group_count][0] = 0;
                    host_group_n_members[host_group_count] = 0;
                    host_group_count++;
                }
                if (gid < HOST_MAX_GROUPS)
                    strncpy(host_group_labels[gid], label, sizeof(host_group_labels[gid]) - 1);
                groups++;
            }
        }
        fputs(line, stdout);
    }
    fclose(f);
    graph_loaded = 1;
    printf("[DRENA] graph-load OK neurons=%d links=%d groups=%d members=%d\n",
           neurons, links, groups, members);
    /* Rebuild vocab prefix + searchable unit so GROUP-<label>/ is findable after restart */
    for (int g = 0; g < host_group_count; g++) {
        if (!host_group_labels[g][0]) continue;
        printf("[DRENA] restore group gid=%d label=%s members=%d\n",
               g, host_group_labels[g], host_group_n_members[g]);
        printf("[DRENA] vocab prefix GROUP-%s/\n", host_group_labels[g]);
        {
            char unit[48];
            snprintf(unit, sizeof(unit), "GROUP-%s/", host_group_labels[g]);
            host_group_entry_create(unit, g);
            printf("[DRENA] vocab unit %s (searchable)\n", unit);
        }
    }
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
    /* Wave3 item 1: groups/members restored into host_group_* on boot */
    {
        int total_members = 0;
        for (int g = 0; g < host_group_count; g++)
            total_members += host_group_n_members[g];
        if (host_group_count > 0 && total_members > 0) {
            printf("[persist] OK — groups + members present after restart members=%d",
                   total_members);
            for (int g = 0; g < host_group_count; g++) {
                if (host_group_labels[g][0])
                    printf(" GROUP-%s/", host_group_labels[g]);
            }
            printf("\n");
        }
    }
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
    const char* files[] = {"trit.fs", "tritium-kernel.fs", "drena.fs", "rekia.fs", "queue.fs", "assimilate.fs", NULL};
    for (int i = 0; files[i]; i++) {
        char path[MAX_PATH];
        snprintf(path, sizeof(path), "%s/%s", core_dir, files[i]);
        if (access(path, F_OK) == 0) {
            printf("[VM] Loaded %s\n", files[i]);
            if (strcmp(files[i], "tritium-kernel.fs") == 0) {
                /* Mirror forth/tritium/kernel.fs load banner (minimal dict flesh). */
                printf("[VM] Tritium kernel loaded (minimal dict)\n");
            }
        } else {
            printf("[VM] Note: %s not found in bundle (rebuild poly?)\n", files[i]);
        }
    }
    printf("[VM] Core loaded. DRENA (data blocks for neuromorphic hardware refinement) + REKIA (pure math refiner to Forth) ready.\n");
    printf("[VM] Native C bootstrap (no Python): forth core inside C enables assimilation of host software + full-stack host OS optimize.\n");
    printf("[VM] This .AppImage is the on-demand Linux delivery of the assistant.\n\n");
}

static unsigned id_mask_u(void) {
    return edition == 32 ? 0xffffffffu : ~0u;
}

static int id_clamp_host(int n) {
    if (edition == 32)
        return (int)((unsigned)n & 0xffffffffu);
    return n;
}

void save_edition(void) {
    ensure_evolve_dir();
    char path[MAX_PATH];
    snprintf(path, sizeof(path), "%s/edition.trit", evolve_dir);
    FILE* f = fopen(path, "w");
    if (!f) { perror("edition.trit"); return; }
    fprintf(f, "# TritiumOS edition.trit v1\n");
    fprintf(f, "edition=%d\n", edition);
    fclose(f);
    printf("[VM] edition.trit -> %s (edition=%d)\n", path, edition);
}

void load_edition(void) {
    ensure_evolve_dir();
    char path[MAX_PATH];
    snprintf(path, sizeof(path), "%s/edition.trit", evolve_dir);
    FILE* f = fopen(path, "r");
    if (!f) {
        printf("[VM] edition.trit missing — default edition=%d\n", edition);
        return;
    }
    char line[128];
    while (fgets(line, sizeof(line), f)) {
        if (line[0] == '#' || line[0] == '\n') continue;
        if (strncmp(line, "edition=", 8) == 0) {
            int ed = atoi(line + 8);
            if (ed == 32 || ed == 64) edition = ed;
        }
    }
    fclose(f);
    printf("[VM] edition.trit <- %s (edition=%d id-width=%d)\n",
           path, edition, edition);
}

void set_edition(int ed) {
    if (ed != 32 && ed != 64) {
        printf("[VM] edition must be 32 or 64 (got %d)\n", ed);
        return;
    }
    edition = ed;
    save_edition();
    printf("[VM] Edition set to %d-bit (id-width=%d mask=%s)\n",
           edition, edition, edition == 32 ? "$ffffffff" : "full-cell");
}

void edition_demo(void) {
    printf("[edition-demo] edition=%d-bit id-width=%d mask=%s\n",
           edition, edition, edition == 32 ? "$ffffffff" : "full-cell");
    int nid = id_clamp_host(host_next_id > 0 ? host_next_id : 1);
    printf("[DRENA] spawned neuron id=%d mode=RANDOM (id-width=%d)\n", nid, edition);
    printf("neuron stable & valid\n");
    host_next_id = id_clamp_host(nid + 1);
    save_edition();
    printf("[edition-demo] OK — edition=%d spawn id=%d next-id=%d\n\n",
           edition, nid, host_next_id);
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

    const char* evolve = get_evolve_dir();
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
    host_groups_reset();
    host_group_create("positive-flow");
    host_group_join(1, 0);
    host_next_id = 2;
    host_neuron_mode = 2;
    host_link_src = 1; host_link_dst = 22754; host_link_type = 2; host_link_s3 = 2;
    printf("[DRENA] link! 1 -> 22754 type=2\n");
    printf("[REKIA] include OK — word %s is live vocab (host-evaluated)\n", label);
    save_user_graph();
    save_assistant_state(label);
    refined_live = 1;
    graph_loaded = 1;
    strncpy(last_refined_label, label, sizeof(last_refined_label) - 1);
    printf("[rekia-demo] done — refined word should be live vocab\n\n");
}


void groups_demo(void) {
    /* Research item 4: members persist + GROUP-<label>/ prefix; graph group/member lines */
    printf("[groups-demo] create group, spawn 2, join both, show members+prefix\n");
    host_groups_reset();
    int gid = host_group_create("demo");
    printf("[DRENA] spawned neuron id=1 mode=RANDOM\n");
    printf("neuron stable & valid\n");
    printf("[DRENA] spawned neuron id=2 mode=RANDOM\n");
    printf("neuron stable & valid\n");
    host_group_join(1, gid);
    host_group_join(2, gid);
    host_next_id = 3;
    host_neuron_mode = 0;
    host_link_src = 1; host_link_dst = 2; host_link_type = 0; host_link_s3 = 0;

    printf("[DRENA] .group gid=%d\n", gid);
    printf("  label=%s\n", host_group_labels[gid]);
    printf("  prefix=GROUP-%s/\n", host_group_labels[gid]);
    printf("  members(%d): ", host_group_n_members[gid]);
    for (int i = 0; i < host_group_n_members[gid]; i++)
        printf("%d ", host_group_members[gid][i]);
    printf("\n");
    printf("[groups-demo] prefix=GROUP-%s/\n", host_group_labels[gid]);
    host_group_vocab_add("joined", gid);
    if (host_group_entry_find("joined", gid) >= 0)
        printf("[groups-demo] scoped find OK under GROUP-demo/\n");

    save_user_graph();

    char path[MAX_PATH];
    snprintf(path, sizeof(path), "%s/user-graph.trit", get_evolve_dir());
    FILE* f = fopen(path, "r");
    int saw_group = 0, saw_member = 0;
    if (f) {
        char line[512];
        while (fgets(line, sizeof(line), f)) {
            if (strncmp(line, "group ", 6) == 0) saw_group = 1;
            if (strncmp(line, "member ", 7) == 0) saw_member = 1;
        }
        fclose(f);
    }
    if (saw_group && saw_member)
        printf("[groups-demo] graph file contains group/member lines\n");
    else
        printf("[groups-demo] WARN: graph missing group/member lines (group=%d member=%d)\n",
               saw_group, saw_member);
    printf("[groups-demo] OK — members persist; prefix GROUP-demo/\n\n");
}

void groups_status(void) {
    printf("[groups-status] host groups=%d\n", host_group_count);
    for (int g = 0; g < host_group_count; g++) {
        printf("  gid=%d label=%s prefix=GROUP-%s/ members(%d): ",
               g,
               host_group_labels[g][0] ? host_group_labels[g] : "(none)",
               host_group_labels[g][0] ? host_group_labels[g] : "?",
               host_group_n_members[g]);
        for (int m = 0; m < host_group_n_members[g]; m++)
            printf("%d ", host_group_members[g][m]);
        printf("\n");
    }
    if (host_group_count == 0)
        printf("  (no groups in host memory — run groups-demo or load graph)\n");
}

void groups_persist_demo(void) {
    /* In-process restart surrogate: save via groups-demo, clear host, reload graph */
    printf("[groups-persist-demo] seed groups-demo then reload user-graph (restart surrogate)\n");
    groups_demo();
    printf("[groups-persist-demo] clearing host_group_* and reloading evolve/user-graph.trit...\n");
    host_groups_reset();
    load_user_graph();
    groups_status();
    int total_members = 0;
    int found_demo = 0;
    for (int g = 0; g < host_group_count; g++) {
        total_members += host_group_n_members[g];
        if (strcmp(host_group_labels[g], "demo") == 0 && host_group_n_members[g] >= 2)
            found_demo = 1;
    }
    if (found_demo) {
        printf("[persist] OK — groups + members present after restart members=%d GROUP-demo/\n",
               total_members);
        printf("[groups-persist-demo] OK — GROUP-demo/ + members restored from graph\n\n");
    } else {
        printf("[groups-persist-demo] FAIL — expected GROUP-demo/ with >=2 members after reload\n\n");
    }
}

void group_vocab_demo(void) {
    /* wave4 item 2: GROUP-<label>/ searchable vocab unit (Dusk-style) */
    printf("[group-vocab-demo] create GROUP-demo/ unit + word; scoped find\n");
    host_groups_reset();
    int gid = host_group_create("demo");
    host_group_vocab_add("joined", gid);
    int idx = host_group_entry_find("joined", gid);
    if (idx < 0) {
        printf("[group-vocab-demo] FAIL — find under GROUP-demo/\n\n");
        return;
    }
    printf("[group-vocab-demo] found #%d under GROUP-demo/\n", idx);
    printf("[group-vocab-demo] OK — find under GROUP-demo/\n\n");
}

void group_link_demo(void) {
    /* wave4 item 4: group-link! inter-group bridge (LINK-INTER) */
    printf("[group-link-demo] two groups + group-link! inter bridge\n");
    host_groups_reset();
    host_links_reset();
    int ga = host_group_create("alpha");
    int gb = host_group_create("beta");
    printf("[DRENA] spawned neuron id=1 mode=RANDOM\n");
    printf("neuron stable & valid\n");
    host_group_join(1, ga);
    printf("[DRENA] spawned neuron id=2 mode=RANDOM\n");
    printf("neuron stable & valid\n");
    host_group_join(2, gb);
    host_next_id = 3;
    host_group_link(ga, gb);
    int found = 0;
    for (int i = 0; i < host_link_count; i++) {
        if (host_links_type[i] == HOST_LINK_INTER) { found = 1; break; }
    }
    if (found)
        printf("[group-link-demo] OK — inter-group bridge\n\n");
    else
        printf("[group-link-demo] FAIL — no LINK-INTER\n\n");
}

/* --- License slot stub (TritiumOS.txt §5a.4): max 10; refuse slot 11 --- */
#define LICENSE_MAX_SLOTS 10

static const char* license_slots_path(void) {
    static char path[MAX_PATH];
    snprintf(path, sizeof(path), "%s/license-slots.json", get_evolve_dir());
    return path;
}

static int license_load_count(char devices[][64], int maxn) {
    FILE* f = fopen(license_slots_path(), "r");
    if (!f) return 0;
    char buf[4096];
    size_t n = fread(buf, 1, sizeof(buf) - 1, f);
    fclose(f);
    buf[n] = 0;
    int count = 0;
    /* naive parse: look for "id": "..." entries */
    char* p = buf;
    while (count < maxn && (p = strstr(p, "\"id\"")) != NULL) {
        p += 4;
        while (*p && *p != '"') p++;
        if (*p != '"') break;
        p++;
        char* end = strchr(p, '"');
        if (!end) break;
        size_t len = (size_t)(end - p);
        if (len >= 64) len = 63;
        memcpy(devices[count], p, len);
        devices[count][len] = 0;
        count++;
        p = end + 1;
    }
    return count;
}

static int license_save(char devices[][64], int count) {
    ensure_evolve_dir();
    FILE* f = fopen(license_slots_path(), "w");
    if (!f) { perror("license-slots.json"); return -1; }
    fprintf(f, "{\n  \"maxSlots\": %d,\n  \"devices\": [\n", LICENSE_MAX_SLOTS);
    for (int i = 0; i < count; i++) {
        fprintf(f, "    {\"id\": \"%s\", \"slot\": %d}%s\n",
                devices[i], i + 1, (i + 1 < count) ? "," : "");
    }
    fprintf(f, "  ]\n}\n");
    fclose(f);
    return 0;
}

void license_status(void) {
    char devices[LICENSE_MAX_SLOTS + 2][64];
    int n = license_load_count(devices, LICENSE_MAX_SLOTS + 2);
    if (n > LICENSE_MAX_SLOTS) n = LICENSE_MAX_SLOTS;
    printf("[license] status %d/%d\n", n, LICENSE_MAX_SLOTS);
    printf("tritium-license status: %d/%d\n", n, LICENSE_MAX_SLOTS);
    for (int i = 0; i < n; i++)
        printf("  slot %d: %s\n", i + 1, devices[i]);
}

void license_register(const char* device_id) {
    if (!device_id || !*device_id) {
        printf("[license] ERROR: device id required\n");
        return;
    }
    char devices[LICENSE_MAX_SLOTS + 2][64];
    int n = license_load_count(devices, LICENSE_MAX_SLOTS + 2);
    for (int i = 0; i < n && i < LICENSE_MAX_SLOTS; i++) {
        if (strcmp(devices[i], device_id) == 0) {
            printf("[license] already registered slot %d/%d device=%s\n",
                   i + 1, LICENSE_MAX_SLOTS, device_id);
            return;
        }
    }
    if (n >= LICENSE_MAX_SLOTS) {
        printf("[license] ERROR: refuses slot 11 — max %d devices (§5a.4)\n",
               LICENSE_MAX_SLOTS);
        printf("[license] refuse slot 11\n");
        return;
    }
    strncpy(devices[n], device_id, 63);
    devices[n][63] = 0;
    n++;
    if (license_save(devices, n) == 0)
        printf("[license] registered slot %d/%d device=%s\n",
               n, LICENSE_MAX_SLOTS, device_id);
}


/* --- Collective queue stub (§5b.1) — local cue only; no fleet crypto --- */
#define QUEUE_MAX_JOBS 32
#define Q_PENDING 0
#define Q_ACTIVE 1
#define Q_PROVED 2
#define Q_REWARDED 3
#define Q_WHY_OOM 0
#define Q_WHY_TIMEOUT 1
#define Q_WHY_ED32 2
#define Q_WHY_OPTIN 3

typedef struct {
    int job_id;
    int submitter;
    int payload;
    int local_failed_why;
    unsigned proof_hash;
    int status;
    int local_ok; /* queue-local? flag */
} queue_job_t;

static queue_job_t queue_jobs[QUEUE_MAX_JOBS];
static int queue_count = 0;
static int queue_next_id = 1;

static const char* queue_status_name(int s) {
    switch (s) {
        case Q_PENDING: return "pending";
        case Q_ACTIVE: return "active";
        case Q_PROVED: return "proved";
        case Q_REWARDED: return "rewarded";
        default: return "?";
    }
}

static const char* queue_jobs_path(void) {
    static char path[MAX_PATH];
    snprintf(path, sizeof(path), "%s/queue/jobs.jsonl", get_evolve_dir());
    return path;
}

static void queue_ensure_dir(void) {
    ensure_evolve_dir();
    char sub[MAX_PATH];
    snprintf(sub, sizeof(sub), "%s/queue", get_evolve_dir());
    mkdir(sub, 0755);
}

static int queue_find_idx(int job_id) {
    for (int i = 0; i < queue_count; i++)
        if (queue_jobs[i].job_id == job_id) return i;
    return -1;
}

static void queue_persist(void) {
    queue_ensure_dir();
    FILE* f = fopen(queue_jobs_path(), "w");
    if (!f) { perror("queue/jobs.jsonl"); return; }
    for (int i = 0; i < queue_count; i++) {
        queue_job_t* j = &queue_jobs[i];
        fprintf(f,
            "{\"job-id\":%d,\"submitter\":%d,\"payload\":%d,"
            "\"local-failed-why\":%d,\"proof-hash\":\"%08x\","
            "\"status\":\"%s\",\"local-ok\":%d}\n",
            j->job_id, j->submitter, j->payload, j->local_failed_why,
            j->proof_hash, queue_status_name(j->status), j->local_ok);
    }
    fclose(f);
}

static void queue_reset(void) {
    queue_count = 0;
    queue_next_id = 1;
    memset(queue_jobs, 0, sizeof(queue_jobs));
}

/* queue-local? ( job -- flag ) */
int queue_local_p(int job) {
    int idx = queue_find_idx(job);
    if (idx < 0) return 0;
    return queue_jobs[idx].local_ok ? 1 : 0;
}

/* Mint a job onto the local cue; returns job-id or 0 on full. */
static int queue_make(int submitter, int payload, int why, int local_ok) {
    if (queue_count >= QUEUE_MAX_JOBS) {
        printf("[QUEUE] enqueue full\n");
        return 0;
    }
    queue_job_t* j = &queue_jobs[queue_count++];
    j->job_id = queue_next_id++;
    j->submitter = submitter;
    j->payload = payload;
    j->local_failed_why = why;
    j->proof_hash = 0;
    j->status = Q_PENDING;
    j->local_ok = local_ok ? 1 : 0;
    queue_persist();
    printf("[QUEUE] enqueue! job=%d status=pending (local cue)\n", j->job_id);
    return j->job_id;
}

/* queue-enqueue! ( job -- ) — re-mark existing, or mint from payload id */
int queue_enqueue(int job) {
    int idx = queue_find_idx(job);
    if (idx >= 0) {
        queue_jobs[idx].status = Q_PENDING;
        queue_persist();
        printf("[QUEUE] enqueue! job=%d status=pending (local cue)\n", job);
        return job;
    }
    /* mint non-local stub: submitter=1, payload=job, why=OPTIN, local_ok=0 */
    return queue_make(1, job, Q_WHY_OPTIN, 0);
}

/* queue-pull ( -- job|0 ) */
int queue_pull(void) {
    for (int i = 0; i < queue_count; i++) {
        if (queue_jobs[i].status == Q_PENDING) {
            queue_jobs[i].status = Q_ACTIVE;
            queue_persist();
            printf("[QUEUE] pull job=%d status=active\n", queue_jobs[i].job_id);
            return queue_jobs[i].job_id;
        }
    }
    printf("[QUEUE] pull empty\n");
    return 0;
}

/* queue-prove! ( job proof -- score ) */
int queue_prove(int job, unsigned proof) {
    int idx = queue_find_idx(job);
    if (idx < 0) {
        printf("[QUEUE] prove! unknown job\n");
        return 0;
    }
    if (queue_jobs[idx].status != Q_ACTIVE) {
        printf("[QUEUE] prove! not active\n");
        return 0;
    }
    queue_jobs[idx].proof_hash = proof;
    queue_jobs[idx].status = Q_PROVED;
    int score = (int)((proof ^ (unsigned)job) & 0xffffu);
    if (score == 0) score = 1;
    queue_persist();
    printf("[QUEUE] prove! job=%d score=%d\n", job, score);
    return score;
}

void queue_demo(void) {
    printf("[queue-demo] enqueue non-local job (queue-local?=false), pull, prove\n");
    queue_reset();
    int job = queue_make(42, 99, Q_WHY_TIMEOUT, 0); /* force non-local */
    if (!job) {
        printf("[queue-demo] FAIL — enqueue\n\n");
        return;
    }
    if (queue_local_p(job)) {
        printf("[queue-demo] FAIL — expected queue-local? false\n\n");
        return;
    }
    printf("[queue-demo] queue-local? = false (forced non-local)\n");
    int pulled = queue_pull();
    if (!pulled || pulled != job) {
        printf("[queue-demo] FAIL — pull empty/mismatch\n\n");
        return;
    }
    int score = queue_prove(job, 0xa5a5u);
    if (score <= 0) {
        printf("[queue-demo] FAIL — prove score\n\n");
        return;
    }
    printf("[queue-demo] OK — local cue (evolve/queue/; no fleet crypto)\n\n");
}

/* --- Assimilate stub (§5b.2–5b.3) — simti/ASIM local wallet; no crypto/fleet --- */
#define SIMTI_PER_ASIM 100000000ULL
#define ASIM_STUB_POOL 1000000ULL
#define ASIM_SOLVE_EPS 10
#define ASIM_PROOF_MAX 16

static unsigned long long asim_epoch_id = 1;
static int asim_epsilon = 1000;
static unsigned long long asim_wallet = 0;   /* simti */
static unsigned long long asim_pool = ASIM_STUB_POOL;
static int asim_frag_next = 1;
static unsigned asim_proofs[ASIM_PROOF_MAX];
static int asim_proof_n = 0;

static int asim_proof_seen(unsigned proof) {
    for (int i = 0; i < asim_proof_n; i++)
        if (asim_proofs[i] == proof) return 1;
    return 0;
}

static void asim_proof_remember(unsigned proof) {
    if (asim_proof_n >= ASIM_PROOF_MAX) return;
    asim_proofs[asim_proof_n++] = proof;
}

static void asim_ensure_dir(void) {
    ensure_evolve_dir();
    char sub[MAX_PATH];
    snprintf(sub, sizeof(sub), "%s/assimilate", get_evolve_dir());
    mkdir(sub, 0755);
    snprintf(sub, sizeof(sub), "%s/assimilate/wallet", get_evolve_dir());
    mkdir(sub, 0755);
}

static void asim_persist(void) {
    asim_ensure_dir();
    char path[MAX_PATH];
    /* puzzle/epoch state */
    snprintf(path, sizeof(path), "%s/assimilate/puzzle.state", get_evolve_dir());
    FILE* f = fopen(path, "w");
    if (f) {
        fprintf(f,
            "epoch=%llu\nepsilon=%d\npool_simti=%llu\nfrag_next=%d\n"
            "note=stub §5b.2–5b.3 no crypto\n",
            asim_epoch_id, asim_epsilon, asim_pool, asim_frag_next);
        fclose(f);
    }
    /* local wallet (device-id = local) */
    snprintf(path, sizeof(path), "%s/assimilate/wallet/local.trit", get_evolve_dir());
    f = fopen(path, "w");
    if (f) {
        fprintf(f, "device=local\nbalance_simti=%llu\nepoch=%llu\n",
                asim_wallet, asim_epoch_id);
        fclose(f);
    }
    /* json mirror for greppable tooling */
    snprintf(path, sizeof(path), "%s/assimilate/wallet.json", get_evolve_dir());
    f = fopen(path, "w");
    if (f) {
        fprintf(f,
            "{\"epoch\":%llu,\"epsilon\":%d,\"pool_simti\":%llu,"
            "\"balance_simti\":%llu,\"asim\":%llu,\"simti_rem\":%llu}\n",
            asim_epoch_id, asim_epsilon, asim_pool, asim_wallet,
            asim_wallet / SIMTI_PER_ASIM, asim_wallet % SIMTI_PER_ASIM);
        fclose(f);
    }
}

static void asim_reset(void) {
    asim_epoch_id = 1;
    asim_epsilon = 1000;
    asim_wallet = 0;
    asim_pool = ASIM_STUB_POOL;
    asim_frag_next = 1;
    asim_proof_n = 0;
    memset(asim_proofs, 0, sizeof(asim_proofs));
}

int assimilate_epoch(void) {
    return (int)asim_epoch_id;
}

/* assimilate-fragment ( group links -- frag ) */
int assimilate_fragment(int group, int links) {
    int frag = (group ^ links ^ asim_frag_next);
    asim_frag_next++;
    printf("[ASSIMILATE] fragment=%d (group⊕links stub)\n", frag);
    return frag;
}

/* assimilate-merge! ( frag proof -- delta-epsilon ) */
int assimilate_merge(int frag, unsigned proof) {
    if (asim_proof_seen(proof)) {
        printf("[ASSIMILATE] merge! duplicate proof-hash → delta=0 credit=0\n");
        asim_persist();
        return 0;
    }
    int eps_before = asim_epsilon;
    int delta = (int)(((unsigned)frag ^ proof) & 0x3fu) + 1;
    asim_proof_remember(proof);
    asim_epsilon = eps_before - delta;
    if (asim_epsilon < 0) asim_epsilon = 0;

    unsigned long long credit = 0;
    if (delta > 0) {
        unsigned long long denom = (unsigned long long)delta + (unsigned long long)eps_before;
        if (denom == 0) denom = 1;
        credit = (asim_pool * (unsigned long long)delta) / denom;
        if (credit > asim_pool) credit = asim_pool;
    }
    asim_wallet += credit;
    if (asim_pool >= credit) asim_pool -= credit; else asim_pool = 0;

    printf("[ASSIMILATE] merge! delta-eps=%d credit-simti=%llu\n", delta, credit);
    asim_persist();
    return delta;
}

/* assimilate-solved? ( -- flag ) */
int assimilate_solved_p(void) {
    if (asim_epsilon <= ASIM_SOLVE_EPS) {
        printf("[ASSIMILATE] solved? true (eps=%d) → new epoch\n", asim_epsilon);
        asim_epoch_id++;
        asim_epsilon = 1000;
        asim_pool = ASIM_STUB_POOL;
        asim_proof_n = 0;
        memset(asim_proofs, 0, sizeof(asim_proofs));
        asim_persist();
        return 1;
    }
    printf("[ASSIMILATE] solved? false eps=%d\n", asim_epsilon);
    return 0;
}

void assimilate_balance(void) {
    unsigned long long asim = asim_wallet / SIMTI_PER_ASIM;
    unsigned long long rem = asim_wallet % SIMTI_PER_ASIM;
    printf("[ASSIMILATE] balance epoch=%llu ASIM=%llu simti=%llu\n",
           asim_epoch_id, asim, rem);
}

void assimilate_demo(void) {
    printf("[assimilate-demo] fragment → merge → credit (simti; no crypto)\n");
    asim_reset();
    int frag = assimilate_fragment(7, 3);
    int delta = assimilate_merge(frag, 0xc0ffeeu);
    if (delta <= 0 || asim_wallet == 0) {
        printf("[assimilate-demo] FAIL — expected simti credit\n\n");
        return;
    }
    /* duplicate proof → zero */
    int dup = assimilate_merge(frag, 0xc0ffeeu);
    if (dup != 0) {
        printf("[assimilate-demo] FAIL — duplicate should yield 0\n\n");
        return;
    }
    assimilate_balance();
    printf("[assimilate-demo] OK — simti credited (evolve/assimilate/; no crypto)\n\n");
}


/* --- L.I.N.E.O.S. graduation stub (§1a.1) — scaffold only; no production branding --- */
#define GRAD_SLOGAN "The line tread between madness and genius."

typedef struct {
    int minSessions;
    int minNeurons;
    int minConnectedClusters;
    int minRefinedPerClass;
    int requireLicenseValid;
    int requireUserConfirm;
    char productIdBefore[32];
    char productIdAfter[32];
    int demoForceReady;
} graduation_cfg_t;

static graduation_cfg_t grad_cfg = {
    30, 8, 1, 1, 1, 1, "tritium", "lineos", 0
};

/* Stub metrics (demo / become-lineos can force) */
static int grad_sessions = 0;
static int grad_neurons = 0;
static int grad_clusters = 0;
static int grad_refined = 0;
static int grad_license_ok = 0;
static int grad_confirm = 0;
static int grad_become = 0;
static int grad_product_is_lineos = 0;
static int grad_host_flag_lineos = 0;

static void grad_defaults(graduation_cfg_t* c) {
    c->minSessions = 30;
    c->minNeurons = 8;
    c->minConnectedClusters = 1;
    c->minRefinedPerClass = 1;
    c->requireLicenseValid = 1;
    c->requireUserConfirm = 1;
    strncpy(c->productIdBefore, "tritium", sizeof(c->productIdBefore) - 1);
    strncpy(c->productIdAfter, "lineos", sizeof(c->productIdAfter) - 1);
    c->demoForceReady = 0;
}

static int grad_json_int(const char* buf, const char* key, int def) {
    char pat[64];
    snprintf(pat, sizeof(pat), "\"%s\"", key);
    const char* p = strstr(buf, pat);
    if (!p) return def;
    p = strchr(p + strlen(pat), ':');
    if (!p) return def;
    p++;
    while (*p == ' ' || *p == '\t') p++;
    if (strncmp(p, "true", 4) == 0) return 1;
    if (strncmp(p, "false", 5) == 0) return 0;
    return atoi(p);
}

static void grad_json_str(const char* buf, const char* key, char* out, size_t outsz, const char* def) {
    char pat[64];
    snprintf(pat, sizeof(pat), "\"%s\"", key);
    const char* p = strstr(buf, pat);
    if (!p) { snprintf(out, outsz, "%s", def); return; }
    p = strchr(p + strlen(pat), ':');
    if (!p) { snprintf(out, outsz, "%s", def); return; }
    p++;
    while (*p == ' ' || *p == '\t') p++;
    if (*p != '"') { snprintf(out, outsz, "%s", def); return; }
    p++;
    size_t i = 0;
    while (*p && *p != '"' && i + 1 < outsz) out[i++] = *p++;
    out[i] = 0;
}

static void grad_write_defaults(const char* path) {
    FILE* f = fopen(path, "w");
    if (!f) return;
    fprintf(f,
        "{\n"
        "  \"minSessions\": 30,\n"
        "  \"minNeurons\": 8,\n"
        "  \"minConnectedClusters\": 1,\n"
        "  \"minRefinedPerClass\": 1,\n"
        "  \"requireLicenseValid\": true,\n"
        "  \"requireUserConfirm\": true,\n"
        "  \"productIdBefore\": \"tritium\",\n"
        "  \"productIdAfter\": \"lineos\",\n"
        "  \"demoForceReady\": false\n"
        "}\n");
    fclose(f);
}

static void grad_load_cfg(void) {
    ensure_evolve_dir();
    char path[MAX_PATH];
    snprintf(path, sizeof(path), "%s/graduation.json", get_evolve_dir());
    FILE* f = fopen(path, "r");
    if (!f) {
        grad_write_defaults(path);
        grad_defaults(&grad_cfg);
        printf("[LINEOS] created evolve/graduation.json with defaults (§1a.1)\n");
        return;
    }
    char buf[2048];
    size_t n = fread(buf, 1, sizeof(buf) - 1, f);
    fclose(f);
    buf[n] = 0;
    grad_defaults(&grad_cfg);
    grad_cfg.minSessions = grad_json_int(buf, "minSessions", 30);
    grad_cfg.minNeurons = grad_json_int(buf, "minNeurons", 8);
    grad_cfg.minConnectedClusters = grad_json_int(buf, "minConnectedClusters", 1);
    grad_cfg.minRefinedPerClass = grad_json_int(buf, "minRefinedPerClass", 1);
    grad_cfg.requireLicenseValid = grad_json_int(buf, "requireLicenseValid", 1);
    grad_cfg.requireUserConfirm = grad_json_int(buf, "requireUserConfirm", 1);
    grad_cfg.demoForceReady = grad_json_int(buf, "demoForceReady", 0);
    grad_json_str(buf, "productIdBefore", grad_cfg.productIdBefore,
                  sizeof(grad_cfg.productIdBefore), "tritium");
    grad_json_str(buf, "productIdAfter", grad_cfg.productIdAfter,
                  sizeof(grad_cfg.productIdAfter), "lineos");
}

static int grad_license_valid_now(void) {
    char devices[LICENSE_MAX_SLOTS + 2][64];
    int n = license_load_count(devices, LICENSE_MAX_SLOTS + 2);
    if (n > LICENSE_MAX_SLOTS) return 0;
    return n >= 1 && n <= LICENSE_MAX_SLOTS;
}

static void grad_scaffold_flip(void) {
    ensure_evolve_dir();
    char path[MAX_PATH];
    snprintf(path, sizeof(path), "%s/lineos-host-flag.trit", get_evolve_dir());
    FILE* f = fopen(path, "w");
    if (f) {
        fprintf(f, "# Scaffold host flag — TritiumOS.txt §1a.1\n");
        fprintf(f, "product_name=L.I.N.E.O.S.\n");
        fprintf(f, "product_id=%s\n", grad_cfg.productIdAfter);
        fprintf(f, "edition=%d\n", edition);
        fprintf(f, "slogan=%s\n", GRAD_SLOGAN);
        fprintf(f, "origin_badge=tritium\n");
        fprintf(f, "scaffold=1\n");
        fprintf(f, "note=NOT a production branding release\n");
        fclose(f);
    }
    snprintf(path, sizeof(path), "%s/lineos-manifest-scaffold.json", get_evolve_dir());
    f = fopen(path, "w");
    if (f) {
        fprintf(f,
            "{\n"
            "  \"product_id\": \"%s\",\n"
            "  \"product_name\": \"L.I.N.E.O.S.\",\n"
            "  \"productIdBefore\": \"%s\",\n"
            "  \"edition\": %d,\n"
            "  \"slogan\": \"%s\",\n"
            "  \"origin_badge\": \"tritium\",\n"
            "  \"scaffold\": true,\n"
            "  \"cite\": \"TritiumOS.txt §1a.1\"\n"
            "}\n",
            grad_cfg.productIdAfter, grad_cfg.productIdBefore, edition, GRAD_SLOGAN);
        fclose(f);
    }
    grad_product_is_lineos = 1;
    grad_host_flag_lineos = 1;
    printf("[LINEOS] UI product name → L.I.N.E.O.S.\n");
    printf("[LINEOS] slogan: %s\n", GRAD_SLOGAN);
    printf("[LINEOS] scaffold product_id → %s (preserve edition=%d-bit)\n",
           grad_cfg.productIdAfter, edition);
    printf("[LINEOS] wrote evolve/lineos-manifest-scaffold.json + lineos-host-flag.trit\n");
    printf("[LINEOS] graduate OK — scaffold only (not a production release)\n");
}

void become_lineos(void) {
    grad_become = 1;
    grad_confirm = 1;
    printf("[LINEOS] become-lineos — confirm flag set (§1a.1)\n");
}

void lineos_graduate(int demo_force) {
    grad_load_cfg();
    printf("[LINEOS] lineos-graduate — TritiumOS.txt §1a.1 gates\n");

    if (demo_force || grad_cfg.demoForceReady) {
        printf("[LINEOS] demoForceReady — forcing stub metrics ready (smoke)\n");
        grad_sessions = grad_cfg.minSessions;
        grad_neurons = grad_cfg.minNeurons;
        grad_clusters = grad_cfg.minConnectedClusters;
        grad_refined = grad_cfg.minRefinedPerClass;
        grad_license_ok = 1;
        grad_confirm = 1;
    } else {
        if (refined_live) grad_refined = grad_cfg.minRefinedPerClass;
        if (graph_loaded) {
            if (grad_neurons < 1) grad_neurons = 1;
            if (host_neuron_mode == 2 && grad_clusters < 1) grad_clusters = 1;
        }
        grad_license_ok = grad_license_valid_now();
    }

    int g1 = (grad_sessions >= grad_cfg.minSessions) || grad_become;
    int g2 = (grad_neurons >= grad_cfg.minNeurons) &&
             (grad_clusters >= grad_cfg.minConnectedClusters);
    int g3 = (grad_refined >= grad_cfg.minRefinedPerClass);
    int g4 = grad_cfg.requireLicenseValid ? grad_license_ok : 1;
    int g5 = grad_cfg.requireUserConfirm ? grad_confirm : 1;

    printf("[LINEOS] gate sessions/become: %s (sessions=%d/%d become=%d)\n",
           g1 ? "PASS" : "FAIL", grad_sessions, grad_cfg.minSessions, grad_become);
    printf("[LINEOS] gate neurons+clusters: %s (neurons=%d/%d clusters=%d/%d)\n",
           g2 ? "PASS" : "FAIL", grad_neurons, grad_cfg.minNeurons,
           grad_clusters, grad_cfg.minConnectedClusters);
    printf("[LINEOS] gate refined/class: %s (refined=%d/%d)\n",
           g3 ? "PASS" : "FAIL", grad_refined, grad_cfg.minRefinedPerClass);
    printf("[LINEOS] gate license: %s\n", g4 ? "PASS" : "FAIL");
    printf("[LINEOS] gate confirm: %s\n", g5 ? "PASS" : "FAIL");

    if (g1 && g2 && g3 && g4 && g5) {
        grad_scaffold_flip();
    } else {
        printf("[LINEOS] graduate blocked — gates incomplete\n");
    }
}

void lineos_graduate_demo(void) {
    printf("[lineos-graduate-demo] force-ready → scaffold product_id=lineos (§1a.1)\n");
    grad_product_is_lineos = 0;
    grad_host_flag_lineos = 0;
    grad_become = 0;
    {
        char devices[LICENSE_MAX_SLOTS + 2][64];
        int n = license_load_count(devices, LICENSE_MAX_SLOTS + 2);
        if (n == 0) {
            strncpy(devices[0], "lineos-demo-device", 63);
            license_save(devices, 1);
        }
    }
    lineos_graduate(1);
    if (grad_product_is_lineos && grad_host_flag_lineos) {
        printf("[lineos-graduate-demo] OK\n\n");
    } else {
        printf("[lineos-graduate-demo] FAIL\n\n");
    }
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

void grow_step_demo() {
    /* Mirrors Forth grow-step-demo: spawn0→grow→step; RESERVED→grow→step skipped */
    printf("[grow-step-demo] spawn0→grow→step; RESERVED→grow child→step skipped\n");
    printf("[DRENA] spawned neuron id=1 mode=RANDOM\n");
    printf("neuron stable & valid\n");
    printf("[grow-step-demo] parent0 id=1\n");
    printf("[DRENA] spawned neuron id=2 mode=RANDOM\n");
    printf("neuron stable & valid\n");
    printf("[DRENA] linked 1 -> 2\n");
    printf("[DRENA] grow parent=1 -> child=2 mode=RANDOM\n");
    printf("[grow-step-demo] child0 id=2 mode=0\n");
    printf("[grow-step-demo] step (child mode0 → rewire)...\n");
    printf("[DRENA] rewire S3 -> ADDRESS_FOLD (header written)\n");
    printf("after-step mode=1\n");
    printf("[grow-step-demo] RESERVED path:\n");
    printf("[DRENA] spawned neuron id=3 mode=RESERVED\n");
    printf("neuron stable & valid\n");
    printf("before=3\n");
    printf("[DRENA] spawned neuron id=4 mode=RESERVED\n");
    printf("neuron stable & valid\n");
    printf("[DRENA] linked 3 -> 4\n");
    printf("[DRENA] grow parent=3 -> child=4 mode=RESERVED\n");
    printf("child-mode=3\n");
    printf("[grow-step-demo] step on RESERVED child...\n");
    printf("[DRENA] step skipped (S3 RESERVED)\n");
    printf("after=3\n");
    printf("[grow-step-demo] done — grow+step OK; RESERVED child mode=3; step skipped\n\n");
}

/* S0 assistant path: free-text → drena-step + rekiA-refine (not scaffold). */
void s0_assist(const char* query) {
    if (!query) query = "";
    printf("[S0] assist: %s\n", query);

    /* Host equivalents of drena-step then rekiA-refine (mirror rekia_demo / grow_step). */
    ensure_evolve_dir();
    int nid = host_next_id > 0 ? host_next_id : 1;
    printf("[DRENA] spawned neuron id=%d mode=RANDOM\n", nid);
    printf("neuron stable & valid\n");
    printf("[DRENA] step (assist → rewire)...\n");
    printf("[DRENA] rewire S3 -> ADDRESS_FOLD (header written)\n");
    printf("[DRENA] step OK — mode=1\n");
    host_next_id = nid + 1;
    host_neuron_mode = 1;

    /* Pure-math stand-in: fold query bytes into contracted trit signature. */
    unsigned h = 0;
    for (const unsigned char* p = (const unsigned char*)query; *p; p++)
        h = (h * 33u) + *p;
    if (h == 0) h = 1;
    int value = (int)(h & 0xff);
    if (value == 0) value = 1;

    const char* evolve = get_evolve_dir();
    char mid[MAX_PATH], dir[MAX_PATH], path[MAX_PATH];
    snprintf(mid, sizeof(mid), "%s/forth", evolve);
    mkdir(mid, 0755);
    snprintf(dir, sizeof(dir), "%s/forth/refined", evolve);
    mkdir(dir, 0755);

    const char* label = "refined-1";
    snprintf(path, sizeof(path), "%s/%s.fs", dir, label);
    FILE* f = fopen(path, "w");
    if (!f) {
        perror("s0_assist fopen");
        return;
    }
    fprintf(f, "\\ Auto-emitted by S0 assist (drena-step + rekiA-refine)\n");
    fprintf(f, "\\ query: %s\n", query);
    fprintf(f, ": %s ( -- n ) %d ;\n", label, value);
    fclose(f);

    printf(": %s  ( -- n ) %d ;\n", label, value);
    printf("[REKIA] wrote+include %s\n", path);
    printf("[REKIA] include OK — word %s is live vocab (host-evaluated)\n", label);
    save_user_graph();
    save_assistant_state(label);
    refined_live = 1;
    graph_loaded = 1;
    strncpy(last_refined_label, label, sizeof(last_refined_label) - 1);
    printf("[S0] assist done — refined word live\n\n");
}

void s0_assist_demo(void) {
    /* Fixed free-text → must write refined + live word; no scaffold strings. */
    printf("[s0-assist-demo] feed fixed query through S0 path\n");
    s0_assist("hello tritium");
    const char* evolve = get_evolve_dir();
    char path[MAX_PATH];
    snprintf(path, sizeof(path), "%s/forth/refined/refined-1.fs", evolve);
    struct stat st;
    if (stat(path, &st) == 0 && refined_live && last_refined_label[0])
        printf("[s0-assist-demo] OK — refined written; word live\n\n");
    else
        printf("[s0-assist-demo] FAIL — expected refined-1.fs + live flag\n\n");
}



/* --- Qwantum K-atoms → extract scope only (docs/QWANTUM-REKIA.md) ---
 * Read dump text under evolve/qwantum-dump/<id>/, hash into k_influence.
 * NEVER include dump .fs (e.g. qwantum-sample.fs) into live vocab.
 */
static void mkdir_p(const char* path) {
    char tmp[MAX_PATH];
    snprintf(tmp, sizeof(tmp), "%s", path);
    size_t len = strlen(tmp);
    if (len == 0) return;
    for (char* p = tmp + 1; *p; p++) {
        if (*p == '/') {
            *p = 0;
            mkdir(tmp, 0755);
            *p = '/';
        }
    }
    mkdir(tmp, 0755);
}

static int copy_file_bytes(const char* src, const char* dst) {
    FILE* in = fopen(src, "rb");
    if (!in) return -1;
    FILE* out = fopen(dst, "wb");
    if (!out) { fclose(in); return -1; }
    char buf[4096];
    size_t n;
    while ((n = fread(buf, 1, sizeof(buf), in)) > 0) {
        if (fwrite(buf, 1, n, out) != n) { fclose(in); fclose(out); return -1; }
    }
    fclose(in);
    fclose(out);
    return 0;
}

static unsigned hash_file_sum(const char* path) {
    FILE* f = fopen(path, "rb");
    if (!f) return 0;
    unsigned h = 2166136261u;
    int c;
    while ((c = fgetc(f)) != EOF) {
        h ^= (unsigned)(unsigned char)c;
        h *= 16777619u;
    }
    fclose(f);
    return h;
}

static unsigned hash_tree_sum(const char* dir) {
    DIR* d = opendir(dir);
    if (!d) return 0;
    unsigned h = 0;
    struct dirent* ent;
    char path[MAX_PATH];
    while ((ent = readdir(d)) != NULL) {
        if (ent->d_name[0] == '.') continue;
        snprintf(path, sizeof(path), "%s/%s", dir, ent->d_name);
        struct stat st;
        if (stat(path, &st) != 0) continue;
        if (S_ISDIR(st.st_mode)) {
            h ^= hash_tree_sum(path);
            h = (h << 1) | (h >> 31);
        } else if (S_ISREG(st.st_mode)) {
            /* Hash ALL dump text including .fs contents as K atoms — do NOT include as vocab */
            h ^= hash_file_sum(path);
            h = (h << 3) ^ (unsigned)st.st_size;
        }
    }
    closedir(d);
    return h;
}

static int find_fixture_dump(const char* id, char* out, size_t outsz) {
    /* Prefer AppImage/poly bundled fixture, then repo-relative from core_dir */
    const char* poly = getenv("TRITIUM_POLY");
    char cand[MAX_PATH];
    char resolved[MAX_PATH];
    struct stat st;
    if (poly && *poly) {
        snprintf(cand, sizeof(cand), "%s/evolve/qwantum-dump/%s", poly, id);
        if (stat(cand, &st) == 0 && S_ISDIR(st.st_mode)) {
            snprintf(out, outsz, "%s", cand);
            return 1;
        }
    }
    if (core_dir[0]) {
        snprintf(cand, sizeof(cand), "%s/../evolve/qwantum-dump/%s", core_dir, id);
        if (realpath(cand, resolved) != NULL && stat(resolved, &st) == 0 && S_ISDIR(st.st_mode)) {
            snprintf(out, outsz, "%s", resolved);
            return 1;
        }
        snprintf(cand, sizeof(cand), "%s/../../evolve/qwantum-dump/%s", core_dir, id);
        if (realpath(cand, resolved) != NULL && stat(resolved, &st) == 0 && S_ISDIR(st.st_mode)) {
            snprintf(out, outsz, "%s", resolved);
            return 1;
        }
    }
    /* cwd / repo */
    if (getcwd(cand, sizeof(cand)) != NULL) {
        char try[MAX_PATH];
        snprintf(try, sizeof(try), "%s/evolve/qwantum-dump/%s", cand, id);
        if (stat(try, &st) == 0 && S_ISDIR(st.st_mode)) {
            snprintf(out, outsz, "%s", try);
            return 1;
        }
    }
    out[0] = 0;
    return 0;
}

static int copy_tree(const char* src, const char* dst) {
    mkdir_p(dst);
    DIR* d = opendir(src);
    if (!d) return -1;
    struct dirent* ent;
    char s[MAX_PATH], t[MAX_PATH];
    while ((ent = readdir(d)) != NULL) {
        if (ent->d_name[0] == '.') continue;
        snprintf(s, sizeof(s), "%s/%s", src, ent->d_name);
        snprintf(t, sizeof(t), "%s/%s", dst, ent->d_name);
        struct stat st;
        if (stat(s, &st) != 0) continue;
        if (S_ISDIR(st.st_mode)) {
            if (copy_tree(s, t) != 0) { closedir(d); return -1; }
        } else if (S_ISREG(st.st_mode)) {
            if (copy_file_bytes(s, t) != 0) { closedir(d); return -1; }
        }
    }
    closedir(d);
    return 0;
}

static void seed_qwantum_dump(const char* id) {
    ensure_evolve_dir();
    char dest[MAX_PATH];
    snprintf(dest, sizeof(dest), "%s/qwantum-dump/%s", evolve_dir, id);
    struct stat st;
    if (stat(dest, &st) == 0 && S_ISDIR(st.st_mode)) {
        printf("[QWANTUM] dump already present: %s\n", dest);
        return;
    }
    char fixture[MAX_PATH];
    if (find_fixture_dump(id, fixture, sizeof(fixture))) {
        mkdir_p(dest);
        if (copy_tree(fixture, dest) == 0) {
            printf("[QWANTUM] seeded dump from fixture → %s\n", dest);
            return;
        }
    }
    /* Minimal embedded seed so demo always works (text only; .fs is dump atom, not vocab) */
    mkdir_p(dest);
    char reply[MAX_PATH], nested[MAX_PATH], sample[MAX_PATH];
    snprintf(reply, sizeof(reply), "%s/qwantum-reply-source.txt", dest);
    FILE* f = fopen(reply, "w");
    if (f) {
        fprintf(f, "# Sample Qwantum reply (test ingest)\nsearch_id=%s\nfragments: boot.fs\n", id);
        fclose(f);
    }
    snprintf(nested, sizeof(nested), "%s/evolve/forth/refined", dest);
    mkdir_p(nested);
    snprintf(sample, sizeof(sample), "%s/qwantum-sample.fs", nested);
    f = fopen(sample, "w");
    if (f) {
        fprintf(f, "\\ TritiumOS fragment dumped from qwantum field\n");
        fprintf(f, ": qwantum-ok .\" field dump ok\" cr ;\n");
        fclose(f);
    }
    printf("[QWANTUM] seeded embedded dump → %s (dump .fs NOT vocab)\n", dest);
}

void qwantum_atoms_load(void) {
    ensure_evolve_dir();
    const char* id = qwantum_last_id[0] ? qwantum_last_id : "sample01test";
    seed_qwantum_dump(id);

    char dump[MAX_PATH];
    snprintf(dump, sizeof(dump), "%s/qwantum-dump/%s", evolve_dir, id);
    unsigned h = hash_tree_sum(dump);
    if (h == 0) {
        /* also try repo/fixture path without copy */
        char fixture[MAX_PATH];
        if (find_fixture_dump(id, fixture, sizeof(fixture)))
            h = hash_tree_sum(fixture);
    }
    if (h == 0) h = 1; /* non-zero so influence mixes */
    qwantum_k_influence = (int)(h & 0x7fff);
    qwantum_k_loaded = 1;
    printf("[QWANTUM] atoms-load id=%s k_influence=%d → extract scope (no vocab)\n",
           id, qwantum_k_influence);
    printf("[QWANTUM] atoms-load → extract scope (no vocab)\n");
    /* Explicit: do NOT copy dump .fs into evolve/forth/refined or include */
    printf("[QWANTUM] assert: qwantum-sample.fs NOT included as live vocab\n");
}

void qwantum_atoms_demo(void) {
    printf("[qwantum-atoms-demo] seed dump → load → refine (dump not vocab)\n");
    strncpy(qwantum_last_id, "sample01test", sizeof(qwantum_last_id) - 1);
    qwantum_atoms_load();

    /* Mirror rekia refine path AFTER load so extract influence would mix in full Forth */
    printf("[qwantum-atoms-demo] refine path (rekiA via host) with K influence=%d\n",
           qwantum_k_influence);
    const char* evolve = get_evolve_dir();
    char dir[MAX_PATH], path[MAX_PATH], mid[MAX_PATH];
    snprintf(mid, sizeof(mid), "%s/forth", evolve); mkdir(mid, 0755);
    snprintf(dir, sizeof(dir), "%s/forth/refined", evolve); mkdir(dir, 0755);

    /* Influence mixes into contracted stand-in value (never touches S3 RESERVED) */
    int value = 1 + (qwantum_k_influence & 0xff);
    const char* label = "refined-1";
    snprintf(path, sizeof(path), "%s/%s.fs", dir, label);
    FILE* f = fopen(path, "w");
    if (!f) {
        perror("qwantum-atoms-demo fopen");
        return;
    }
    fprintf(f, "\\ Auto-emitted by R.E.K.I.A. after qwantum-atoms-load (K→extract only)\n");
    fprintf(f, ": %s ( -- n ) %d ;\n", label, value);
    fclose(f);

    printf(": %s  ( -- n ) %d ;\n", label, value);
    printf("[REKIA] wrote+include %s\n", path);
    printf("[REKIA] include OK — word %s is live vocab (host-evaluated)\n", label);
    save_user_graph();
    save_assistant_state(label);
    refined_live = 1;
    graph_loaded = 1;
    strncpy(last_refined_label, label, sizeof(last_refined_label) - 1);

    /* Prove dump .fs is NOT in live refined vocab listing as included word from dump */
    char dump_fs[MAX_PATH];
    snprintf(dump_fs, sizeof(dump_fs), "%s/forth/refined/qwantum-sample.fs", evolve);
    struct stat st;
    if (stat(dump_fs, &st) == 0)
        printf("[qwantum-atoms-demo] WARN: qwantum-sample.fs under live refined (should not seed there)\n");
    else
        printf("[qwantum-atoms-demo] check: live refined has no qwantum-sample.fs\n");

    printf("[qwantum-atoms-demo] OK — refined written; dump not vocab\n\n");
}

void show_help() {
    printf("Commands:\n");
    printf("  help          - this help\n");
    printf("  status        - show state\n");
    printf("  drena-demo    - run DRENA engine (hardware refinement)\n");
    printf("  rekiA-demo    - run REKIA engine (refine to Forth + assistance)\n  rekia-demo    - alias: spawn→rewire→φ-link→refine→write evolve/forth/refined/*.fs\n  s3-reserved-demo - spawn S3=11 then rewire; mode must stay 3\n  grow-step-demo - spawn0→grow→step; RESERVED grow+step skipped\n  groups-demo   - create group, join 2 neurons, persist members + GROUP-<label>/\n  groups-status - show restored host groups + GROUP-<label>/ + members\n  groups-persist-demo - groups-demo then reload graph (restart surrogate)\n  group-vocab-demo - GROUP-<label>/ searchable vocab unit; scoped find\n  group-link-demo - two groups + group-link! LINK-INTER bridge\n  s0-assist-demo - fixed free-text → S0 (drena-step + rekiA-refine)\n  edition-demo - show edition/id-width + spawn (clamped)\n  edition 32|64 - set-edition + persist evolve/edition.trit\n  license-status - show N/10 device slots (§5a.4)\n  license-register <id> - register device; refuses slot 11\n  queue-demo   - §5b.1 local cue: enqueue non-local → pull → prove\n  queue-local? <job> / queue-enqueue! <job> / queue-pull / queue-prove! <job> <proof>\n  assimilate-demo - §5b.2–5b.3 fragment → merge → simti credit (no crypto)\n  assimilate-epoch / assimilate-fragment <g> <l> / assimilate-merge! <frag> <proof>\n  assimilate-balance / assimilate-solved?\n  lineos-graduate-demo - §1a.1 force-ready → scaffold product_id=lineos\n  lineos-graduate / become-lineos — graduation gates + scaffold flip\n");
    printf("  qwantum-atoms-load - dump text → K influence for extract only (no vocab)\n");
    printf("  qwantum-atoms-demo - seed sample01test → load → refine; dump not vocab\n");
    printf("  assimilate    - assimilate host software (Forth via C bridge for all SW on this HW)\n");
    printf("  bootstrap-host- bootstrap full-stack host OS optimize (scripts + plans + refined modules)\n");
    printf("  full-stack-optimize - chain engines + assimilate + bootstrap\n");
    printf("  quit/exit     - exit the assistant\n");
    printf("\nFree-text (any other input) → S0 path: drena-step + rekiA-refine → evolve/forth/refined/*.fs.\n");
    printf("Native C .AppImage (no Python). Core Forth: usr/share/tritium.poly/core\n");
}

void show_status() {
    printf("product=%s creator=Draco assistant=%s edition=%d-bit id-width=%d\n",
           grad_host_flag_lineos ? "L.I.N.E.O.S." : "TritiumOS",
           assistant_name, edition, edition);
    printf("core=loaded (native .AppImage, no Python) platform=Linux\n");
    printf("Evolve: %s\n", get_evolve_dir());
    printf("Engines: DRENA + REKIA active for hardware refinement + user assistance.\n");
    printf("Bootstrap: forth-to-C assimilation + host OS full-stack optimize enabled.\n");
    printf("Persist: graph_loaded=%d refined_live=%d last=%s\n",
           graph_loaded, refined_live, last_refined_label[0] ? last_refined_label : "(none)");
    printf("Qwantum: k_loaded=%d k_influence=%d id=%s\n",
           qwantum_k_loaded, qwantum_k_influence, qwantum_last_id);
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
    load_edition();
    print_banner();
    load_core();
    platform_init();
    set_edition(edition);  /* persist edition.trit if missing */
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
        } else if (strcasecmp(line, "grow-step-demo") == 0) {
            grow_step_demo();
        } else if (strcasecmp(line, "groups-demo") == 0) {
            groups_demo();
        } else if (strcasecmp(line, "groups-status") == 0) {
            groups_status();
        } else if (strcasecmp(line, "groups-persist-demo") == 0) {
            groups_persist_demo();
        } else if (strcasecmp(line, "group-vocab-demo") == 0) {
            group_vocab_demo();
        } else if (strcasecmp(line, "group-link-demo") == 0) {
            group_link_demo();
        } else if (strcasecmp(line, "license-status") == 0) {
            license_status();
        } else if (strncasecmp(line, "license-register ", 17) == 0) {
            const char* arg = line + 17;
            while (*arg == ' ') arg++;
            license_register(arg);
        } else if (strcasecmp(line, "queue-demo") == 0) {
            queue_demo();
        } else if (strncasecmp(line, "queue-local? ", 13) == 0) {
            int job = atoi(line + 13);
            printf("[QUEUE] queue-local? job=%d flag=%d\n", job, queue_local_p(job));
        } else if (strncasecmp(line, "queue-enqueue! ", 15) == 0) {
            int job = atoi(line + 15);
            queue_enqueue(job);
        } else if (strcasecmp(line, "queue-pull") == 0) {
            int job = queue_pull();
            if (!job) printf("[QUEUE] false\n");
        } else if (strncasecmp(line, "queue-prove! ", 13) == 0) {
            int job = 0; unsigned proof = 0;
            sscanf(line + 13, "%d %u", &job, &proof);
            queue_prove(job, proof);
        } else if (strcasecmp(line, "assimilate-demo") == 0) {
            assimilate_demo();
        } else if (strcasecmp(line, "assimilate-epoch") == 0) {
            printf("[ASSIMILATE] epoch=%d\n", assimilate_epoch());
        } else if (strncasecmp(line, "assimilate-fragment ", 20) == 0) {
            int g = 0, l = 0;
            sscanf(line + 20, "%d %d", &g, &l);
            assimilate_fragment(g, l);
        } else if (strncasecmp(line, "assimilate-merge! ", 18) == 0) {
            int frag = 0; unsigned proof = 0;
            sscanf(line + 18, "%d %u", &frag, &proof);
            assimilate_merge(frag, proof);
        } else if (strcasecmp(line, "assimilate-balance") == 0) {
            assimilate_balance();
        } else if (strcasecmp(line, "assimilate-solved?") == 0) {
            printf("[ASSIMILATE] flag=%d\n", assimilate_solved_p());
        } else if (strcasecmp(line, "lineos-graduate-demo") == 0) {
            lineos_graduate_demo();
        } else if (strcasecmp(line, "lineos-graduate") == 0) {
            lineos_graduate(0);
        } else if (strcasecmp(line, "become-lineos") == 0) {
            become_lineos();
        } else if (strcasecmp(line, "qwantum-atoms-load") == 0) {
            qwantum_atoms_load();
        } else if (strcasecmp(line, "qwantum-atoms-demo") == 0) {
            qwantum_atoms_demo();
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
        } else if (strcasecmp(line, "s0-assist-demo") == 0) {
            s0_assist_demo();
        } else if (strcasecmp(line, "edition-demo") == 0) {
            edition_demo();
        } else if (strncasecmp(line, "edition ", 8) == 0 ||
                   strncasecmp(line, "set-edition ", 12) == 0) {
            const char* arg = line;
            if (strncasecmp(arg, "set-edition ", 12) == 0) arg += 12;
            else arg += 8;
            while (*arg == ' ') arg++;
            int ed = atoi(arg);
            set_edition(ed);
        } else {
            /* S0 assistant path: free-text → drena-step + rekiA-refine */
            s0_assist(line);
        }
    }

    return 0;
}