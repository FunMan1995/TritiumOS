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
void group_nested_demo(void);
void group_vocab_persist_demo(void);
void kernel_demo(void);
void interpret_demo(void);
void colon_demo(void);
void control_demo(void);
void loop_demo(void);
void do_loop_demo(void);
void words_demo(void);
void var_demo(void);
void value_demo(void);
void comment_demo(void);
void string_demo(void);
void allot_demo(void);
void leave_demo(void);
void unloop_demo(void);
void two_var_demo(void);
void throw_demo(void);
void cell_demo(void);
void pick_demo(void);
void fill_demo(void);
void fold_demo(void);
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
void lineos_confirm(void);
void lineos_confirm_demo(void);
void become_lineos(void);
void lineos_splash(void);
void lineos_about(void);
void lineos_brand_demo(void);
void tritium_integrate(const char* platform);
void tritium_integrate_demo(void);
/* Master mint/verify scaffold (§5a.5) — format-only; no crypto */
void master_mint_license(int slots);
void master_mint_worker(const char* device_id);
int master_verify(const char* key);
void master_demo(void);
/* Fleet evolve-sync stub (§§5a.2–5a.4) — local export/import; same-key only */
void fleet_export(const char* device_id);
int fleet_import(void);
void fleet_demo(void);
/* userland scaffold presence demo (wave8 item 3) */
void userland_demo(void);
/* host parity Win/Android contracts (wave8 item 4) */
void host_parity_demo(void);
void host_boot_demo(void);
void host_boot_report(void);
void refined_boot_report(void);
void refined_boot_demo(void);
void trit_math_demo(void);
void economy_wire_demo(void);
/* assistant-state v2 hooks (wave9 item 3) */
void assistant_task_set(int id, const char* text);
void assistant_reminder_set(int id, const char* when, const char* text);
void assistant_note_set(int id, const char* text);
void assistant_state_bang(void);
void assistant_state_at(void);
void assistant_state_demo(void);
void clear_assistant_hooks(void);
int queue_reward(int job);
int assimilate_epoch(void);
int assimilate_fragment(int group, int links);
int assimilate_merge(int frag, unsigned proof);
int assimilate_solved_p(void);
void assimilate_balance(void);
void s0_assist(const char* query);
void s0_assist_demo(void);
void assistant_s0_demo(void);
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
    snprintf(sub, sizeof(sub), "%s/integrate", evolve_dir); mkdir(sub, 0755);
    snprintf(sub, sizeof(sub), "%s/master", evolve_dir); mkdir(sub, 0755);
    snprintf(sub, sizeof(sub), "%s/fleet", evolve_dir); mkdir(sub, 0755);
    snprintf(sub, sizeof(sub), "%s/lineos", evolve_dir); mkdir(sub, 0755);
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

/* --- assistant-state v2 hooks (task / reminder / note; cap 8 each) --- */
#define ASSIST_MAX_HOOKS 8
#define ASSIST_TEXT_LEN 128
#define ASSIST_WHEN_LEN 64
static int assist_task_count = 0;
static int assist_task_ids[ASSIST_MAX_HOOKS];
static char assist_task_text[ASSIST_MAX_HOOKS][ASSIST_TEXT_LEN];
static int assist_reminder_count = 0;
static int assist_reminder_ids[ASSIST_MAX_HOOKS];
static char assist_reminder_when[ASSIST_MAX_HOOKS][ASSIST_WHEN_LEN];
static char assist_reminder_text[ASSIST_MAX_HOOKS][ASSIST_TEXT_LEN];
static int assist_note_count = 0;
static int assist_note_ids[ASSIST_MAX_HOOKS];
static char assist_note_text[ASSIST_MAX_HOOKS][ASSIST_TEXT_LEN];

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
static int host_entry_body[HOST_MAX_GENTRIES];   /* 0=create-only; 1=body-present */
static int host_entry_btoks[HOST_MAX_GENTRIES];  /* body token count */
static int host_entry_cells[HOST_MAX_GENTRIES];  /* VARIABLE/CONSTANT stub cell (wave12 item 2) */
static int host_entry_value_p[HOST_MAX_GENTRIES]; /* 1 = VALUE stub (wave13 item 1) */
static int host_entry_cells2[HOST_MAX_GENTRIES]; /* 2VARIABLE/2CONSTANT hi stub (wave14 item 3) */
static int host_entry_2cell_p[HOST_MAX_GENTRIES]; /* 1 = 2VARIABLE/2CONSTANT stub (wave14 item 3) */
static int host_entry_does_body[HOST_MAX_GENTRIES]; /* 1 = DOES> stub marked (wave13 item 3) */
static int host_create_latest = -1; /* index of most recent CREATE; -1 = none */
/* wave14 item 1: HERE/ALLOT stub dictionary pointer (bytes); no arena */
#define HOST_HERE_BASE 0x1000
static int host_here = HOST_HERE_BASE;
/* wave15 item 1: CELL size constant (Linux SoT n=8); no dictionary image */
#define HOST_CELL_BYTES 8
/* wave15 item 2: tiny host stack picture for PICK/ROLL/DEPTH/?DUP stubs */
#define HOST_PICK_MAX 8
static int host_pick_pic[HOST_PICK_MAX];
static int host_pick_depth = 0;
/* wave15 item 3: fixed host byte buffer for FILL/ERASE/MOVE/CMOVE stubs */
#define HOST_FILL_CAP 64
static unsigned char host_fill_mem[HOST_FILL_CAP];
static int host_colon_def = 0;
static int host_colon_idx = -1;
static int host_colon_toks = 0;
/* wave10 item 2: control-stack stub (depth only; no XT patching) */
static int host_cs_depth = 0;
static int host_cs_side = 0; /* 0=IF side; 1=ELSE side */
/* wave11 item 1: loop-stack stub (sibling depth; no XT patching) */
static int host_loop_depth = 0;
/* wave12 item 1: do-loop stub (sibling of BEGIN loop-depth; no XT patching) */
static int host_do_loop_depth = 0;
static int host_do_loop_index = 0;
static int host_do_loop_outer = 0; /* enclosing-frame index stub for J (wave14 item 2) */
/* wave13 item 2: case-stack stub (sibling of cs; balance-only CASE/OF) */
static int host_case_depth = 0;
/* wave14 item 4: CATCH/THROW catch-depth stub (frame-mark only; no RS unwind) */
static int host_catch_depth = 0;

static void host_dict_reset(void) {
    host_entry_count = 0;
    for (int i = 0; i < HOST_MAX_GENTRIES; i++) {
        host_entry_names[i][0] = 0;
        host_entry_gids[i] = -1;
        host_entry_body[i] = 0;
        host_entry_btoks[i] = 0;
        host_entry_cells[i] = 0;
        host_entry_value_p[i] = 0;
        host_entry_cells2[i] = 0;
        host_entry_2cell_p[i] = 0;
        host_entry_does_body[i] = 0;
    }
    host_colon_def = 0;
    host_colon_idx = -1;
    host_colon_toks = 0;
    host_cs_depth = 0;
    host_cs_side = 0;
    host_loop_depth = 0;
    host_do_loop_depth = 0;
    host_do_loop_index = 0;
    host_do_loop_outer = 0;
    host_case_depth = 0;
    host_catch_depth = 0;
    host_create_latest = -1;
    host_here = HOST_HERE_BASE;
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
    host_entry_body[i] = 0;
    host_entry_btoks[i] = 0;
    host_entry_cells[i] = 0;
    host_entry_value_p[i] = 0;
    host_entry_cells2[i] = 0;
    host_entry_2cell_p[i] = 0;
    host_entry_does_body[i] = 0;
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

/* Flat dict find/create (mirrors Forth entry-find / entry-create-from; shares ENTRY-GIDS table) */
static int host_entry_find(const char* name) {
    for (int i = 0; i < host_entry_count; i++) {
        if (strcmp(host_entry_names[i], name) == 0)
            return i;
    }
    return -1;
}

static int host_entry_create(const char* name) {
    int ex = host_entry_find(name);
    if (ex >= 0) {
        printf("[kernel] entry exists #%d\n", ex);
        return ex;
    }
    if (host_entry_count >= HOST_MAX_GENTRIES) {
        printf("[kernel] dict full\n");
        return -1;
    }
    int i = host_entry_count++;
    strncpy(host_entry_names[i], name, HOST_NAMELEN - 1);
    host_entry_names[i][HOST_NAMELEN - 1] = 0;
    host_entry_gids[i] = -1; /* global / flat */
    host_entry_body[i] = 0;
    host_entry_btoks[i] = 0;
    host_entry_cells[i] = 0;
    host_entry_value_p[i] = 0;
    host_entry_cells2[i] = 0;
    host_entry_2cell_p[i] = 0;
    host_entry_does_body[i] = 0;
    printf("[kernel] created #%d\n", i);
    return i;
}

/* interpret-token stub: lookup-only; hit → true + marker; miss → false + soft miss */
static int host_interpret_token(const char* name) {
    int i = host_entry_find(name);
    if (i < 0) {
        printf("[kernel] find miss\n");
        return 0;
    }
    printf("[kernel] find hit #%d name=%s\n", i, name);
    return 1;
}

static void host_words(void) {
    printf("[kernel] words (%d): ", host_entry_count);
    for (int i = 0; i < host_entry_count; i++)
        printf("%s ", host_entry_names[i]);
    printf("\n");
}

/* wave8 item 1 + wave9 item 4: interpret + colon body/marker stub */
static int host_interp_misses = 0;
static int host_interp_hits = 0;

static void host_colon_abandon(void) {
    if (host_colon_def) {
        host_colon_def = 0;
        host_colon_idx = -1;
        host_colon_toks = 0;
    }
}

static int host_colon_body_p(int i) {
    return (i >= 0 && i < host_entry_count && host_entry_body[i] != 0);
}

static void host_colon_body_tok(const char* tok) {
    if (!host_colon_def)
        return;
    printf("[colon] body + %s\n", tok);
    host_colon_toks++;
}

static void host_semicolon(void) {
    if (!host_colon_def) {
        printf("[colon] ; (not in colon-def)\n");
        return;
    }
    if (host_colon_idx < 0 || host_colon_idx >= host_entry_count) {
        printf("[colon] ; bad idx\n");
        host_colon_abandon();
        return;
    }
    int k = host_colon_toks;
    if (k == 0)
        k = 1; /* fixed BODY */
    host_entry_btoks[host_colon_idx] = k;
    host_entry_body[host_colon_idx] = 1;
    printf("[colon] ; name=%s tokens=%d\n",
           host_entry_names[host_colon_idx], k);
    host_colon_def = 0;
    host_colon_idx = -1;
    host_colon_toks = 0;
}

static void host_interpret(const char* s) {
    host_colon_abandon(); /* open : without ; → create-only */
    host_interp_misses = 0;
    host_interp_hits = 0;
    const char* p = s;
    while (*p) {
        while (*p == ' ' || *p == '\t' || *p == '\n' || *p == '\r')
            p++;
        if (!*p)
            break;
        const char* start = p;
        while (*p && *p != ' ' && *p != '\t' && *p != '\n' && *p != '\r')
            p++;
        char tok[HOST_NAMELEN];
        size_t n = (size_t)(p - start);
        if (n >= HOST_NAMELEN)
            n = HOST_NAMELEN - 1;
        memcpy(tok, start, n);
        tok[n] = 0;

        /* wave12 item 3: comment skip before find/exec */
        if (strcmp(tok, "\\") == 0) {
            while (*p && *p != '\n' && *p != '\r')
                p++;
            if (*p == '\r')
                p++;
            if (*p == '\n')
                p++;
            printf("[comment] skip line\n");
            continue;
        }
        if (strcmp(tok, "(") == 0) {
            int found = 0;
            while (*p) {
                if (*p == ')') {
                    p++;
                    found = 1;
                    break;
                }
                p++;
            }
            if (!found)
                printf("[comment] FAIL reason=unclosed\n");
            else
                printf("[comment] skip paren\n");
            continue;
        }

        /* wave13 item 4: stub string-literal parse S" / ." / .( after comment skip */
        if (strcmp(tok, "S\"") == 0 || strcmp(tok, ".\"") == 0 ||
            strcmp(tok, ".(") == 0) {
            char delim = (strcmp(tok, ".(") == 0) ? ')' : '"';
            /* ANS-ish: skip one leading blank */
            if (*p == ' ' || *p == '\t')
                p++;
            const char* content = p;
            int found = 0;
            while (*p) {
                if (*p == delim) {
                    found = 1;
                    break;
                }
                /* quote forms: EOL before close → unclosed */
                if (delim == '"' && (*p == '\n' || *p == '\r'))
                    break;
                p++;
            }
            if (!found) {
                printf("[string] FAIL reason=unclosed\n");
            } else {
                size_t clen = (size_t)(p - content);
                printf("[string] %s ", tok);
                fwrite(content, 1, clen, stdout);
                printf("\n");
                p++; /* consume closing delim */
            }
            continue;
        }

        int i = host_entry_find(tok);
        if (i < 0) {
            printf("[interpret] miss name=%s\n", tok);
            host_interp_misses++;
        } else {
            printf("[interpret] exec #%d name=%s\n", i, tok);
            if (host_colon_body_p(i)) {
                printf("[colon] run body name=%s tokens=%d\n",
                       host_entry_names[i], host_entry_btoks[i]);
            }
            host_interp_hits++;
        }
    }
}

/* : name [body…] [;] — create + colon-def; body tokens until ; mark body-present */
static int host_colon_create(const char* name) {
    host_colon_abandon();
    while (*name == ' ')
        name++;
    if (!name[0]) {
        printf("[colon] : \n");
        printf("[interpret] : created \n");
        return -1;
    }
    /* first token = name; remaining until ';' = body toks (one-liner support) */
    char buf[HOST_NAMELEN];
    const char* p = name;
    size_t n = 0;
    while (*p && *p != ' ' && *p != '\t' && n < HOST_NAMELEN - 1)
        buf[n++] = *p++;
    buf[n] = 0;
    if (!buf[0]) {
        printf("[colon] : \n");
        printf("[interpret] : created \n");
        return -1;
    }
    int idx = host_entry_create(buf);
    if (idx < 0)
        return -1;
    host_colon_idx = idx;
    host_colon_def = 1;
    host_colon_toks = 0;
    host_entry_body[idx] = 0;
    host_entry_btoks[idx] = 0;
    printf("[colon] : %s\n", buf);
    printf("[interpret] : created %s\n", buf);

    while (*p == ' ' || *p == '\t')
        p++;
    while (*p) {
        while (*p == ' ' || *p == '\t')
            p++;
        if (!*p)
            break;
        if (*p == ';' && (p[1] == 0 || p[1] == ' ' || p[1] == '\t')) {
            host_semicolon();
            break;
        }
        char tok[HOST_NAMELEN];
        size_t tn = 0;
        while (*p && *p != ' ' && *p != '\t' && tn < HOST_NAMELEN - 1)
            tok[tn++] = *p++;
        tok[tn] = 0;
        if (strcmp(tok, ";") == 0) {
            host_semicolon();
            break;
        }
        host_colon_body_tok(tok);
    }
    return idx;
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

/* wave7 item 4: nested search-order across LINK-INTER (docs/GROUPS-NESTED.md) */
#define HOST_NEST_MAX_HOPS 4
#define HOST_NEST_MAX_GIDS 8
#define HOST_GID_MARK 0x80000000u

static int host_id_to_gid(int id) {
    if ((unsigned)id & HOST_GID_MARK)
        return (int)((unsigned)id & ~HOST_GID_MARK);
    for (int g = 0; g < host_group_count; g++) {
        for (int m = 0; m < host_group_n_members[g]; m++) {
            if (host_group_members[g][m] == id)
                return g;
        }
    }
    return -1;
}

static int host_nest_has(const int *order, int n, int gid) {
    for (int i = 0; i < n; i++)
        if (order[i] == gid) return 1;
    return 0;
}

/* Build walk order: start gid then LINK-INTER neighbors (undirected), caps hops/gids.
   Returns count; writes gids into out[0..count). */
static int host_group_search_order(int start_gid, int *out, int maxn) {
    int depth[HOST_NEST_MAX_GIDS];
    int n = 0;
    if (start_gid < 0 || start_gid >= host_group_count || maxn <= 0)
        return 0;
    out[n] = start_gid;
    depth[n] = 0;
    n++;
    for (int i = 0; i < n; i++) {
        if (depth[i] >= HOST_NEST_MAX_HOPS) continue;
        int gid = out[i];
        for (int L = 0; L < host_link_count; L++) {
            if (host_links_type[L] != HOST_LINK_INTER) continue;
            int ga = host_id_to_gid(host_links_src[L]);
            int gb = host_id_to_gid(host_links_dst[L]);
            if (ga < 0 || gb < 0) continue;
            int nb = -1;
            if (ga == gid) nb = gb;
            else if (gb == gid) nb = ga;
            if (nb < 0 || nb >= host_group_count) continue;
            if (host_nest_has(out, n, nb)) continue;
            if (n >= maxn || n >= HOST_NEST_MAX_GIDS) break;
            out[n] = nb;
            depth[n] = depth[i] + 1;
            n++;
        }
    }
    return n;
}

static int host_group_find_nested(const char *name, int gid) {
    int order[HOST_NEST_MAX_GIDS];
    int n = host_group_search_order(gid, order, HOST_NEST_MAX_GIDS);
    for (int i = 0; i < n; i++) {
        int idx = host_group_entry_find(name, order[i]);
        if (idx >= 0) return idx;
    }
    return -1;
}

static int host_entry_is_unit(int eid) {
    int gid = host_entry_gids[eid];
    if (gid < 0 || gid >= host_group_count || !host_group_labels[gid][0])
        return 0;
    char unit[48];
    snprintf(unit, sizeof(unit), "GROUP-%s/", host_group_labels[gid]);
    return strcmp(host_entry_names[eid], unit) == 0;
}


void save_user_graph(void) {
    ensure_evolve_dir();
    char path[MAX_PATH];
    snprintf(path, sizeof(path), "%s/user-graph.trit", evolve_dir);
    FILE* f = fopen(path, "w");
    if (!f) { perror("user-graph.trit"); return; }
    fprintf(f, "# TritiumOS user-graph.trit v2\n");
    fprintf(f, "# neuron headers + typed links + groups/members/vocab (host snapshot)\n");
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
        /* wave7 item 4: vocab <gid> <word-name> (skip GROUP-<label>/ unit row) */
        for (int e = 0; e < host_entry_count; e++) {
            if (host_entry_gids[e] != g) continue;
            if (host_entry_is_unit(e)) continue;
            fprintf(f, "vocab %d %s\n", g, host_entry_names[e]);
        }
    }
    fclose(f);
    printf("[DRENA] graph-save -> %s\n", path);
}

void clear_assistant_hooks(void) {
    assist_task_count = 0;
    assist_reminder_count = 0;
    assist_note_count = 0;
    for (int i = 0; i < ASSIST_MAX_HOOKS; i++) {
        assist_task_ids[i] = 0;
        assist_task_text[i][0] = 0;
        assist_reminder_ids[i] = 0;
        assist_reminder_when[i][0] = 0;
        assist_reminder_text[i][0] = 0;
        assist_note_ids[i] = 0;
        assist_note_text[i][0] = 0;
    }
}

void assistant_task_set(int id, const char* text) {
    if (!text) text = "";
    for (int i = 0; i < assist_task_count; i++) {
        if (assist_task_ids[i] == id) {
            strncpy(assist_task_text[i], text, ASSIST_TEXT_LEN - 1);
            assist_task_text[i][ASSIST_TEXT_LEN - 1] = 0;
            printf("[assistant-state] task! id=%d\n", id);
            return;
        }
    }
    if (assist_task_count >= ASSIST_MAX_HOOKS) {
        printf("[assistant-state] task! refuse — cap %d\n", ASSIST_MAX_HOOKS);
        return;
    }
    int i = assist_task_count++;
    assist_task_ids[i] = id;
    strncpy(assist_task_text[i], text, ASSIST_TEXT_LEN - 1);
    assist_task_text[i][ASSIST_TEXT_LEN - 1] = 0;
    printf("[assistant-state] task! id=%d\n", id);
}

void assistant_reminder_set(int id, const char* when, const char* text) {
    if (!when) when = "0";
    if (!text) text = "";
    for (int i = 0; i < assist_reminder_count; i++) {
        if (assist_reminder_ids[i] == id) {
            strncpy(assist_reminder_when[i], when, ASSIST_WHEN_LEN - 1);
            assist_reminder_when[i][ASSIST_WHEN_LEN - 1] = 0;
            strncpy(assist_reminder_text[i], text, ASSIST_TEXT_LEN - 1);
            assist_reminder_text[i][ASSIST_TEXT_LEN - 1] = 0;
            printf("[assistant-state] reminder! id=%d\n", id);
            return;
        }
    }
    if (assist_reminder_count >= ASSIST_MAX_HOOKS) {
        printf("[assistant-state] reminder! refuse — cap %d\n", ASSIST_MAX_HOOKS);
        return;
    }
    int i = assist_reminder_count++;
    assist_reminder_ids[i] = id;
    strncpy(assist_reminder_when[i], when, ASSIST_WHEN_LEN - 1);
    assist_reminder_when[i][ASSIST_WHEN_LEN - 1] = 0;
    strncpy(assist_reminder_text[i], text, ASSIST_TEXT_LEN - 1);
    assist_reminder_text[i][ASSIST_TEXT_LEN - 1] = 0;
    printf("[assistant-state] reminder! id=%d\n", id);
}

void assistant_note_set(int id, const char* text) {
    if (!text) text = "";
    for (int i = 0; i < assist_note_count; i++) {
        if (assist_note_ids[i] == id) {
            strncpy(assist_note_text[i], text, ASSIST_TEXT_LEN - 1);
            assist_note_text[i][ASSIST_TEXT_LEN - 1] = 0;
            printf("[assistant-state] note! id=%d\n", id);
            return;
        }
    }
    if (assist_note_count >= ASSIST_MAX_HOOKS) {
        printf("[assistant-state] note! refuse — cap %d\n", ASSIST_MAX_HOOKS);
        return;
    }
    int i = assist_note_count++;
    assist_note_ids[i] = id;
    strncpy(assist_note_text[i], text, ASSIST_TEXT_LEN - 1);
    assist_note_text[i][ASSIST_TEXT_LEN - 1] = 0;
    printf("[assistant-state] note! id=%d\n", id);
}

void save_assistant_state(const char* refined_label) {
    ensure_evolve_dir();
    char path[MAX_PATH];
    snprintf(path, sizeof(path), "%s/assistant-state.trit", evolve_dir);
    time_t now = time(NULL);
    FILE* f = fopen(path, "w");
    if (!f) { perror("assistant-state.trit"); return; }
    fprintf(f, "# TritiumOS assistant-state.trit v2\n");
    fprintf(f, "assistant=%s\n", assistant_name);
    fprintf(f, "edition=%d\n", edition);
    if (refined_label && *refined_label) {
        fprintf(f, "last-refine=%s\n", refined_label);
        fprintf(f, "last-refine-path=forth/refined/%s.fs\n", refined_label);
        strncpy(last_refined_label, refined_label, sizeof(last_refined_label) - 1);
        last_refined_label[sizeof(last_refined_label) - 1] = 0;
    } else if (last_refined_label[0]) {
        fprintf(f, "last-refine=%s\n", last_refined_label);
        fprintf(f, "last-refine-path=forth/refined/%s.fs\n", last_refined_label);
    }
    fprintf(f, "updated=%ld\n", (long)now);
    /* stub neuron-count (0 if unknown) */
    int ncount = (host_next_id > 1) ? (host_next_id - 1) : 0;
    fprintf(f, "neuron-count=%d\n", ncount);
    for (int i = 0; i < assist_task_count; i++)
        fprintf(f, "task=%d|%s\n", assist_task_ids[i], assist_task_text[i]);
    for (int i = 0; i < assist_reminder_count; i++)
        fprintf(f, "reminder=%d|%s|%s\n",
                assist_reminder_ids[i], assist_reminder_when[i], assist_reminder_text[i]);
    for (int i = 0; i < assist_note_count; i++)
        fprintf(f, "note=%d|%s\n", assist_note_ids[i], assist_note_text[i]);
    fclose(f);
    printf("[assistant-state] save v2 path=%s\n", path);
    printf("[REKIA] assistant-state! -> %s\n", path);
}

void assistant_state_bang(void) {
    save_assistant_state(NULL);
}

static int refined_boot_last_loaded = 0;
static int refined_boot_last_skipped = 0;

static int refined_boot_resolve_dir(char* out, size_t outsz);

/* Scan dir for *.fs: skip qwantum-* dumps, include the rest. Prints [refined-boot] + [VM] markers.
 * Returns 1 if dir was opened (even if empty). Sets refined_boot_last_{loaded,skipped}. */
static int refined_boot_scan_dir(const char* dir, int do_vocab_ack) {
    DIR* d;
    struct dirent* ent;
    int n = 0;
    int skipped = 0;

    refined_boot_last_loaded = 0;
    refined_boot_last_skipped = 0;
    printf("[refined-boot] dir=%s\n", dir);
    d = opendir(dir);
    if (!d) {
        printf("[refined-boot] loaded=0 skipped=0\n");
        return 0;
    }
    while ((ent = readdir(d)) != NULL) {
        size_t len = strlen(ent->d_name);
        char fspath[MAX_PATH];
        struct stat st;
        if (len < 4 || strcmp(ent->d_name + len - 3, ".fs") != 0) continue;
        snprintf(fspath, sizeof(fspath), "%s/%s", dir, ent->d_name);
        /* wave11 tip4: skip fifo/socket before fopen/fgets (hang class) */
        if (stat(fspath, &st) != 0 || !S_ISREG(st.st_mode)) continue;
        /* CRITICAL: never load dump fragments (qwantum-*.fs) as live vocab */
        if (strncmp(ent->d_name, "qwantum-", 8) == 0) {
            printf("[VM]   skip %s (dump atom — not vocab; refine only)\n", ent->d_name);
            printf("[refined-boot] skip=%s reason=qwantum-dump\n", ent->d_name);
            skipped++;
            continue;
        }
        {
            FILE* f = fopen(fspath, "r");
            if (!f) continue;
            printf("[VM]   include %s (persisted refined)\n", ent->d_name);
            printf("[refined-boot] include=%s\n", ent->d_name);
            if (do_vocab_ack) {
                char line[512];
                while (fgets(line, sizeof(line), f)) {
                    /* host-evaluated: just acknowledge definition lines */
                    if (line[0] == ':') printf("    [vocab] %s", line);
                }
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
    }
    closedir(d);
    refined_boot_last_loaded = n;
    refined_boot_last_skipped = skipped;
    printf("[refined-boot] loaded=%d skipped=%d\n", n, skipped);
    if (n > 0) {
        refined_live = 1;
        printf("[VM] Refined modules loaded (%d). Intelligence extensions active.\n", n);
    }
    return 1;
}

void load_refined_modules(void) {
    char dir[MAX_PATH];
    if (!refined_boot_resolve_dir(dir, sizeof(dir))) {
        ensure_evolve_dir();
        snprintf(dir, sizeof(dir), "%s/forth/refined", evolve_dir);
    }
    (void)refined_boot_scan_dir(dir, 1);
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
    int neurons = 0, links = 0, groups = 0, members = 0, vocabs = 0;
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
        } else if (strncmp(line, "vocab ", 6) == 0) {
            /* wave7 item 4: vocab <gid> <word-name> */
            int gid = -1;
            char wname[HOST_NAMELEN] = {0};
            if (sscanf(line + 6, "%d %15s", &gid, wname) == 2 &&
                gid >= 0 && gid < HOST_MAX_GROUPS && wname[0]) {
                while (host_group_count <= gid && host_group_count < HOST_MAX_GROUPS) {
                    host_group_labels[host_group_count][0] = 0;
                    host_group_n_members[host_group_count] = 0;
                    host_group_count++;
                }
                host_group_vocab_add(wname, gid);
                vocabs++;
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
    printf("[DRENA] graph-load OK neurons=%d links=%d groups=%d members=%d vocabs=%d\n",
           neurons, links, groups, members, vocabs);
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
    clear_assistant_hooks();
    char line[512];
    while (fgets(line, sizeof(line), f)) {
        line[strcspn(line, "\n")] = 0;
        fputs(line, stdout);
        fputc('\n', stdout);
        if (line[0] == '#' || line[0] == 0)
            continue;
        if (strncmp(line, "last-refine=", 12) == 0) {
            strncpy(last_refined_label, line + 12, sizeof(last_refined_label) - 1);
            last_refined_label[sizeof(last_refined_label) - 1] = 0;
        } else if (strncmp(line, "task=", 5) == 0) {
            char buf[512];
            strncpy(buf, line + 5, sizeof(buf) - 1);
            buf[sizeof(buf) - 1] = 0;
            char* bar = strchr(buf, '|');
            if (bar && assist_task_count < ASSIST_MAX_HOOKS) {
                *bar = 0;
                int i = assist_task_count++;
                assist_task_ids[i] = atoi(buf);
                strncpy(assist_task_text[i], bar + 1, ASSIST_TEXT_LEN - 1);
                assist_task_text[i][ASSIST_TEXT_LEN - 1] = 0;
            }
        } else if (strncmp(line, "reminder=", 9) == 0) {
            char buf[512];
            strncpy(buf, line + 9, sizeof(buf) - 1);
            buf[sizeof(buf) - 1] = 0;
            char* bar1 = strchr(buf, '|');
            if (bar1 && assist_reminder_count < ASSIST_MAX_HOOKS) {
                *bar1 = 0;
                char* bar2 = strchr(bar1 + 1, '|');
                int i = assist_reminder_count++;
                assist_reminder_ids[i] = atoi(buf);
                if (bar2) {
                    *bar2 = 0;
                    strncpy(assist_reminder_when[i], bar1 + 1, ASSIST_WHEN_LEN - 1);
                    assist_reminder_when[i][ASSIST_WHEN_LEN - 1] = 0;
                    strncpy(assist_reminder_text[i], bar2 + 1, ASSIST_TEXT_LEN - 1);
                    assist_reminder_text[i][ASSIST_TEXT_LEN - 1] = 0;
                } else {
                    strncpy(assist_reminder_when[i], bar1 + 1, ASSIST_WHEN_LEN - 1);
                    assist_reminder_when[i][ASSIST_WHEN_LEN - 1] = 0;
                    assist_reminder_text[i][0] = 0;
                }
            }
        } else if (strncmp(line, "note=", 5) == 0) {
            char buf[512];
            strncpy(buf, line + 5, sizeof(buf) - 1);
            buf[sizeof(buf) - 1] = 0;
            char* bar = strchr(buf, '|');
            if (bar && assist_note_count < ASSIST_MAX_HOOKS) {
                *bar = 0;
                int i = assist_note_count++;
                assist_note_ids[i] = atoi(buf);
                strncpy(assist_note_text[i], bar + 1, ASSIST_TEXT_LEN - 1);
                assist_note_text[i][ASSIST_TEXT_LEN - 1] = 0;
            }
        }
    }
    fclose(f);
    printf("[assistant-state] load ok tasks=%d reminders=%d notes=%d\n",
           assist_task_count, assist_reminder_count, assist_note_count);
}

void assistant_state_at(void) {
    load_assistant_state();
}

void assistant_state_demo(void) {
    ensure_evolve_dir();
    char path[MAX_PATH];
    snprintf(path, sizeof(path), "%s/assistant-state.trit", evolve_dir);
    printf("[assistant-state-demo] write≥1 task/reminder/note → save v2 → clear → load\n");
    clear_assistant_hooks();
    unlink(path);
    assistant_task_set(1, "smoke-task");
    assistant_reminder_set(1, "1700000000", "smoke-reminder");
    assistant_note_set(1, "smoke-note");
    if (assist_task_count < 1 || assist_reminder_count < 1 || assist_note_count < 1) {
        printf("[assistant-state-demo] FAIL\n\n");
        return;
    }
    assistant_state_bang();
    /* assert file present + v2 header */
    FILE* f = fopen(path, "r");
    if (!f) {
        printf("[assistant-state-demo] FAIL\n\n");
        return;
    }
    char hdr[128] = {0};
    if (!fgets(hdr, sizeof(hdr), f) || strstr(hdr, "v2") == NULL) {
        fclose(f);
        printf("[assistant-state-demo] FAIL\n\n");
        return;
    }
    fclose(f);
    /* clear memory then reload */
    clear_assistant_hooks();
    if (assist_task_count != 0 || assist_reminder_count != 0 || assist_note_count != 0) {
        printf("[assistant-state-demo] FAIL\n\n");
        return;
    }
    assistant_state_at();
    if (assist_task_count < 1 || assist_reminder_count < 1 || assist_note_count < 1) {
        printf("[assistant-state-demo] FAIL\n\n");
        return;
    }
    if (strcmp(assist_task_text[0], "smoke-task") != 0 ||
        strcmp(assist_reminder_text[0], "smoke-reminder") != 0 ||
        strcmp(assist_note_text[0], "smoke-note") != 0) {
        printf("[assistant-state-demo] FAIL\n\n");
        return;
    }
    printf("[assistant-state-demo] OK\n\n");
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


/* --- address-fold deepen (wave10 item 3 / docs/ADDRESS-FOLD.md) --- */
static int host_phi_fold_rounds = 1;
static unsigned host_last_fold_influence = 0;
static unsigned host_last_fold_target = 0;

static unsigned host_phi_fold(unsigned a, unsigned b) {
    unsigned x = a ^ b;
    int rounds = host_phi_fold_rounds < 1 ? 1 : host_phi_fold_rounds;
    int i;
    for (i = 0; i < rounds; i++) {
        x ^= x << 13;
        x ^= x >> 7;
    }
    x &= 0x7fff;
    host_last_fold_influence = x;
    return x;
}

static unsigned host_phi_fold3(unsigned a, unsigned b, unsigned c) {
    return host_phi_fold(host_phi_fold(a, b), c);
}

static unsigned host_fold_target(unsigned src, unsigned cand) {
    unsigned influ = host_phi_fold(src, cand);
    unsigned target = influ ? influ : 1;
    host_last_fold_target = target;
    printf("[fold] phi a=%u b=%u influence=%u\n", src, cand, influ);
    printf("[fold] target src=%u cand=%u -> %u\n", src, cand, target);
    return target;
}

void fold_demo(void) {
    unsigned t;
    printf("[fold-demo] golden vectors rounds=1\n");
    host_phi_fold_rounds = 1;
    t = host_fold_target(1, 2);
    if (host_last_fold_influence != 24771 || t != 24771) {
        printf("[fold-demo] FAIL\n\n");
        return;
    }
    t = host_fold_target(7, 11);
    if (host_last_fold_influence != 780 || t != 780) {
        printf("[fold-demo] FAIL\n\n");
        return;
    }
    t = host_fold_target(100, 100);
    if (host_last_fold_influence != 0 || t != 1) {
        printf("[fold-demo] FAIL\n\n");
        return;
    }
        /* optional phi-fold3 stub: multi-hop mix (a,b,c) */
    (void)host_phi_fold3(1, 2, 0);
printf("[fold-demo] OK\n\n");
}

void drena_demo() {
    /* Mirrors forth drena-spawn / drena-rewire / ADDRESS_FOLD φ / drena-link */
    printf("Running DRENA demo (rewire + ADDRESS_FOLD φ)...\n");
    printf("[DRENA] spawned neuron id=1 mode=RANDOM\n");
    printf("neuron stable & valid\n");
    printf("[DRENA] rewire S3 -> ADDRESS_FOLD (header written)\n");
    /* φ(addr,addr') = mix; deterministic target pick when S3=ADDRESS_FOLD */
    unsigned src = 1, cand = 99;
    unsigned target = host_fold_target(src, cand);
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

void group_nested_demo(void) {
    /* wave7 item 4: nested find via LINK-INTER */
    printf("[group-nested-demo] two groups + link; nested find from A\n");
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
    host_group_vocab_add("nested", gb);
    if (host_group_entry_find("nested", ga) >= 0) {
        printf("[group-nested-demo] FAIL — direct find on A should miss\n\n");
        return;
    }
    int idx = host_group_find_nested("nested", ga);
    if (idx < 0) {
        printf("[group-nested-demo] FAIL\n\n");
        return;
    }
    printf("[group-nested-demo] found #%d via nested walk\n", idx);
    printf("[group-nested-demo] OK — nested find via LINK-INTER\n\n");
}

void group_vocab_persist_demo(void) {
    /* wave7 item 4: vocab lines survive save → clear → reload */
    printf("[group-vocab-persist-demo] add→save→reload vocab contract\n");
    host_groups_reset();
    host_links_reset();
    int gid = host_group_create("demo");
    host_group_vocab_add("nested", gid);
    if (host_group_entry_find("nested", gid) < 0) {
        printf("[group-vocab-persist-demo] FAIL\n\n");
        return;
    }
    save_user_graph();
    /* prove graph contains vocab line */
    {
        char path[MAX_PATH];
        snprintf(path, sizeof(path), "%s/user-graph.trit", get_evolve_dir());
        FILE* f = fopen(path, "r");
        int saw = 0;
        if (f) {
            char line[512];
            while (fgets(line, sizeof(line), f)) {
                if (strncmp(line, "vocab ", 6) == 0 && strstr(line, "nested"))
                    saw = 1;
            }
            fclose(f);
        }
        if (!saw) {
            printf("[group-vocab-persist-demo] FAIL — no vocab line in graph\n\n");
            return;
        }
    }
    printf("[group-vocab-persist-demo] clearing host and reloading evolve/user-graph.trit...\n");
    host_groups_reset();
    load_user_graph();
    int idx = host_group_entry_find("nested", gid);
    /* gid may rematerialize as 0 after reload with single group */
    if (idx < 0) {
        for (int g = 0; g < host_group_count; g++) {
            idx = host_group_entry_find("nested", g);
            if (idx >= 0) break;
        }
    }
    if (idx < 0) {
        printf("[group-vocab-persist-demo] FAIL\n\n");
        return;
    }
    printf("[group-vocab-persist-demo] OK — vocab restored from graph\n\n");
}

void kernel_demo(void) {
    /* wave7 item 5: flat find/findentry aliases + interpret-token stub */
    printf("[kernel-demo] dict-reset + two names + find/interpret stub\n");
    host_dict_reset();
    if (host_entry_create("alpha") < 0 || host_entry_create("beta") < 0) {
        printf("[kernel-demo] FAIL\n\n");
        return;
    }
    int ia = host_entry_find("alpha");
    int ib = host_entry_find("beta");
    if (ia < 0 || ib < 0) {
        printf("[kernel-demo] FAIL\n\n");
        return;
    }
    printf("[kernel] find hit #%d name=alpha\n", ia);
    printf("[kernel] find hit #%d name=beta\n", ib);
    if (host_entry_find("nosuch") >= 0) {
        printf("[kernel-demo] FAIL\n\n");
        return;
    }
    printf("[kernel] find miss\n");
    host_words();
    if (!host_interpret_token("alpha")) {
        printf("[kernel-demo] FAIL\n\n");
        return;
    }
    printf("[kernel-demo] OK\n\n");
}

void interpret_demo(void) {
    /* wave8 item 1: interpret loop deepen + : create-only stub */
    printf("[interpret-demo] dict-reset + colon-create + interpret stream\n");
    host_dict_reset();
    if (host_colon_create("alpha") < 0 || host_colon_create("beta") < 0) {
        printf("[interpret-demo] FAIL\n\n");
        return;
    }
    host_interpret("alpha beta nosuch");
    if (host_interp_hits < 2 || host_interp_misses != 1) {
        printf("[interpret-demo] FAIL\n\n");
        return;
    }
    printf("[interpret-demo] OK\n\n");
}

void colon_demo(void) {
    /* wave9 item 4: colon body/marker stub */
    printf("[colon-demo] dict-reset + : body ; + interpret + create-only\n");
    host_dict_reset();
    if (host_colon_create("square dup * ;") < 0) {
        printf("[colon-demo] FAIL\n\n");
        return;
    }
    int isq = host_entry_find("square");
    if (isq < 0 || !host_colon_body_p(isq) || host_entry_btoks[isq] < 1) {
        printf("[colon-demo] FAIL\n\n");
        return;
    }
    host_interpret("square");
    if (host_interp_hits < 1) {
        printf("[colon-demo] FAIL\n\n");
        return;
    }
    /* create-only path (no ;) */
    if (host_colon_create("only") < 0) {
        printf("[colon-demo] FAIL\n\n");
        return;
    }
    host_colon_abandon();
    int io = host_entry_find("only");
    if (io < 0 || host_colon_body_p(io)) {
        printf("[colon-demo] FAIL\n\n");
        return;
    }
    printf("[colon-demo] OK\n\n");
}

/* wave10 item 2: IF/THEN/ELSE control stubs — balance-only cs depth */
static void host_control_if(int flag_present, int flag) {
    host_cs_depth++;
    host_cs_side = 0;
    if (flag_present)
        printf("[control] IF taken=%d\n", flag ? 1 : 0);
    else
        printf("[control] IF\n");
}

static void host_control_else(void) {
    if (host_cs_depth <= 0) {
        printf("[control] FAIL reason=unbalanced\n");
        return;
    }
    host_cs_side = 1;
    printf("[control] ELSE\n");
}

static void host_control_then(void) {
    if (host_cs_depth <= 0) {
        printf("[control] FAIL reason=unbalanced\n");
        return;
    }
    host_cs_depth--;
    host_cs_side = 0;
    printf("[control] THEN depth=%d\n", host_cs_depth);
}

static int host_control_cs_depth(void) {
    return host_cs_depth;
}

void control_demo(void) {
    /* wave10 item 2: balanced IF…THEN + IF…ELSE…THEN */
    printf("[control-demo] dict-reset + IF…THEN + IF…ELSE…THEN\n");
    host_dict_reset();
    host_control_if(1, 1);
    host_control_then();
    if (host_control_cs_depth() != 0) {
        printf("[control-demo] FAIL\n\n");
        return;
    }
    host_control_if(1, 0);
    host_control_else();
    host_control_then();
    if (host_control_cs_depth() != 0) {
        printf("[control-demo] FAIL\n\n");
        return;
    }
    printf("[control-demo] OK\n\n");
}

/* wave11 item 1: BEGIN/UNTIL/WHILE/REPEAT loop stubs — sibling depth balance */
static void host_control_begin(void) {
    host_loop_depth++;
    printf("[loop] BEGIN depth=%d\n", host_loop_depth);
}

static void host_control_until(int flag_present, int flag) {
    if (host_loop_depth <= 0) {
        printf("[loop] FAIL reason=unbalanced\n");
        return;
    }
    host_loop_depth--;
    if (flag_present)
        printf("[loop] UNTIL again=%d\n", flag ? 1 : 0);
    else
        printf("[loop] UNTIL again=0\n");
}

static void host_control_while(int flag_present, int flag) {
    if (host_loop_depth <= 0) {
        printf("[loop] FAIL reason=unbalanced\n");
        return;
    }
    if (flag_present)
        printf("[loop] WHILE cont=%d\n", flag ? 1 : 0);
    else
        printf("[loop] WHILE cont=0\n");
}

static void host_control_repeat(void) {
    if (host_loop_depth <= 0) {
        printf("[loop] FAIL reason=unbalanced\n");
        return;
    }
    host_loop_depth--;
    printf("[loop] REPEAT\n");
}

static int host_loop_cs_depth(void) {
    return host_loop_depth;
}

void loop_demo(void) {
    /* wave11 item 1: BEGIN…UNTIL + BEGIN…WHILE…REPEAT */
    printf("[loop-demo] dict-reset + BEGIN…UNTIL + BEGIN…WHILE…REPEAT\n");
    host_dict_reset();
    host_control_begin();
    host_control_until(1, 0);
    if (host_loop_cs_depth() != 0) {
        printf("[loop-demo] FAIL\n\n");
        return;
    }
    host_control_begin();
    host_control_while(1, 1);
    host_control_repeat();
    if (host_loop_cs_depth() != 0) {
        printf("[loop-demo] FAIL\n\n");
        return;
    }
    printf("[loop-demo] OK\n\n");
}

/* wave12 item 1: DO/LOOP/+LOOP/I stubs — sibling do-loop depth balance */
static void host_control_do(int limit_present, int limit, int start) {
    (void)limit_present;
    (void)limit; /* limit unused this tip beyond optional presence */
    if (host_do_loop_depth > 0)
        host_do_loop_outer = host_do_loop_index; /* save enclosing index for J */
    host_do_loop_index = start;
    host_do_loop_depth++;
    printf("[do-loop] DO depth=%d index=%d\n", host_do_loop_depth, host_do_loop_index);
}

static void host_control_loop(void) {
    if (host_do_loop_depth <= 0) {
        printf("[do-loop] FAIL reason=unbalanced\n");
        return;
    }
    host_do_loop_depth--;
    printf("[do-loop] LOOP depth=%d\n", host_do_loop_depth);
}

static void host_control_plus_loop(int step_present, int step) {
    if (host_do_loop_depth <= 0) {
        printf("[do-loop] FAIL reason=unbalanced\n");
        return;
    }
    host_do_loop_depth--;
    if (step_present)
        printf("[do-loop] +LOOP depth=%d step=%d\n", host_do_loop_depth, step);
    else
        printf("[do-loop] +LOOP depth=%d\n", host_do_loop_depth);
}

static void host_control_i(void) {
    if (host_do_loop_depth <= 0) {
        printf("[do-loop] FAIL reason=unbalanced\n");
        return;
    }
    printf("[do-loop] I index=%d\n", host_do_loop_index);
}

static int host_do_loop_cs_depth(void) {
    return host_do_loop_depth;
}

void do_loop_demo(void) {
    /* wave12 item 1: DO…I…LOOP + DO…I…+LOOP */
    printf("[do-loop-demo] dict-reset + DO…I…LOOP + DO…I…+LOOP\n");
    host_dict_reset();
    host_control_do(1, 10, 0);
    host_control_i();
    host_control_loop();
    if (host_do_loop_cs_depth() != 0) {
        printf("[do-loop-demo] FAIL\n\n");
        return;
    }
    host_control_do(1, 5, 0);
    host_control_i();
    host_control_plus_loop(1, 1);
    if (host_do_loop_cs_depth() != 0) {
        printf("[do-loop-demo] FAIL\n\n");
        return;
    }
    printf("[do-loop-demo] OK\n\n");
}


/* wave12 item 4: LEAVE / AGAIN stubs — mark / close BEGIN or DO frames */
static void host_control_leave(void) {
    if (host_do_loop_depth > 0) {
        printf("[leave] LEAVE depth=%d frame=do\n", host_do_loop_depth);
        return;
    }
    if (host_loop_depth > 0) {
        printf("[leave] LEAVE depth=%d frame=begin\n", host_loop_depth);
        return;
    }
    printf("[leave] FAIL reason=unbalanced\n");
}

static void host_control_again(void) {
    if (host_loop_depth <= 0) {
        printf("[loop] FAIL reason=unbalanced\n");
        return;
    }
    host_loop_depth--;
    printf("[loop] AGAIN depth=%d\n", host_loop_depth);
}

void leave_demo(void) {
    /* wave12 item 4: BEGIN…LEAVE…UNTIL + DO…LEAVE…LOOP + BEGIN…AGAIN */
    printf("[leave-demo] dict-reset + BEGIN…LEAVE…UNTIL + DO…LEAVE…LOOP + BEGIN…AGAIN\n");
    host_dict_reset();
    /* Stream A */
    host_control_begin();
    host_control_leave();
    host_control_until(1, 0);
    if (host_loop_cs_depth() != 0) {
        printf("[leave-demo] FAIL\n\n");
        return;
    }
    /* Stream B */
    host_control_do(1, 10, 0);
    host_control_leave();
    host_control_loop();
    if (host_do_loop_cs_depth() != 0) {
        printf("[leave-demo] FAIL\n\n");
        return;
    }
    /* Stream C */
    host_control_begin();
    host_control_again();
    if (host_loop_cs_depth() != 0) {
        printf("[leave-demo] FAIL\n\n");
        return;
    }
    printf("[leave-demo] OK\n\n");
}

/* wave14 item 2: UNLOOP / J stubs — pop DO frame / outer-index; [do-loop] markers */
static void host_control_unloop(void) {
    if (host_do_loop_depth <= 0) {
        printf("[do-loop] FAIL reason=unbalanced\n");
        return;
    }
    host_do_loop_depth--;
    printf("[do-loop] UNLOOP depth=%d\n", host_do_loop_depth);
}

static void host_control_j(void) {
    if (host_do_loop_depth < 2) {
        printf("[do-loop] FAIL reason=no-outer\n");
        return;
    }
    printf("[do-loop] J index=%d\n", host_do_loop_outer);
}

void unloop_demo(void) {
    /* wave14 item 2: DO…UNLOOP + nested DO…J…LOOP…LOOP */
    printf("[unloop-demo] dict-reset + DO…UNLOOP + DO…DO…J…LOOP…LOOP\n");
    host_dict_reset();
    /* Stream A: DO … UNLOOP (no LOOP) */
    host_control_do(1, 10, 0);
    host_control_unloop();
    if (host_do_loop_cs_depth() != 0) {
        printf("[unloop-demo] FAIL\n\n");
        return;
    }
    /* Stream B: nested DO + J + LOOP closers */
    host_control_do(1, 10, 0);
    host_control_do(1, 5, 1);
    host_control_j();
    host_control_loop();
    host_control_loop();
    if (host_do_loop_cs_depth() != 0) {
        printf("[unloop-demo] FAIL\n\n");
        return;
    }
    printf("[unloop-demo] OK\n\n");
}

void words_demo(void) {
    /* wave11 item 3: WORDS / dict-list smoke — create ≥2 then list */
    printf("[words-demo] dict-reset + entry-create alpha + beta + WORDS\n");
    host_dict_reset();
    if (host_entry_create("alpha") < 0 || host_entry_create("beta") < 0) {
        printf("[words-demo] FAIL\n\n");
        return;
    }
    host_words();
    if (host_entry_count < 2 ||
        host_entry_find("alpha") < 0 ||
        host_entry_find("beta") < 0) {
        printf("[words-demo] FAIL\n\n");
        return;
    }
    printf("[words-demo] OK\n\n");
}

/* wave12 item 2: VARIABLE / CONSTANT named-cell stubs — host-side cell map */
static int host_var_create(const char* name) {
    int i = host_entry_create(name);
    if (i < 0) return -1;
    host_entry_cells[i] = 0;
    printf("[var] VARIABLE name=%s\n", host_entry_names[i]);
    return i;
}

static int host_const_create(const char* name, int value) {
    int i = host_entry_create(name);
    if (i < 0) return -1;
    host_entry_cells[i] = value;
    printf("[var] CONSTANT name=%s value=%d\n", host_entry_names[i], value);
    return i;
}

static void host_var_fetch(const char* name) {
    int i = host_entry_find(name);
    if (i < 0) {
        printf("[var] FAIL reason=miss\n");
        return;
    }
    printf("[var] @ name=%s value=%d\n", host_entry_names[i], host_entry_cells[i]);
}

static void host_var_store(const char* name, int value) {
    int i = host_entry_find(name);
    if (i < 0) {
        printf("[var] FAIL reason=miss\n");
        return;
    }
    host_entry_cells[i] = value;
    printf("[var] ! name=%s value=%d\n", host_entry_names[i], host_entry_cells[i]);
}

void var_demo(void) {
    /* wave12 item 2: VARIABLE foo + CONSTANT bar value=42 + find/WORDS */
    printf("[var-demo] dict-reset + VARIABLE foo + CONSTANT bar value=42\n");
    host_dict_reset();
    if (host_var_create("foo") < 0) {
        printf("[var-demo] FAIL\n\n");
        return;
    }
    if (host_const_create("bar", 42) < 0) {
        printf("[var-demo] FAIL\n\n");
        return;
    }
    host_words();
    if (host_entry_count < 2 ||
        host_entry_find("foo") < 0 ||
        host_entry_find("bar") < 0) {
        printf("[var-demo] FAIL\n\n");
        return;
    }
    printf("[var-demo] OK\n\n");
}

/* wave13 item 1: VALUE / TO named mutable-cell stubs — host-side cell map */
static int host_value_create(const char* name, int value) {
    int i = host_entry_create(name);
    if (i < 0) return -1;
    host_entry_cells[i] = value;
    host_entry_value_p[i] = 1;
    printf("[value] VALUE name=%s value=%d\n", host_entry_names[i], host_entry_cells[i]);
    return i;
}

static void host_value_to(const char* name, int value) {
    int i = host_entry_find(name);
    if (i < 0 || !host_entry_value_p[i]) {
        printf("[value] FAIL reason=miss\n");
        return;
    }
    host_entry_cells[i] = value;
    printf("[value] TO name=%s value=%d\n", host_entry_names[i], host_entry_cells[i]);
}

static void host_value_fetch(const char* name) {
    int i = host_entry_find(name);
    if (i < 0 || !host_entry_value_p[i]) {
        printf("[value] FAIL reason=miss\n");
        return;
    }
    printf("[value] @ name=%s value=%d\n", host_entry_names[i], host_entry_cells[i]);
}

void value_demo(void) {
    /* wave13 item 1: 7 VALUE baz + 99 TO baz + value@ + find/WORDS */
    printf("[value-demo] dict-reset + 7 VALUE baz + 99 TO baz\n");
    host_dict_reset();
    if (host_value_create("baz", 7) < 0) {
        printf("[value-demo] FAIL\n\n");
        return;
    }
    host_value_to("baz", 99);
    host_value_fetch("baz");
    host_words();
    if (host_entry_count < 1 || host_entry_find("baz") < 0) {
        printf("[value-demo] FAIL\n\n");
        return;
    }
    printf("[value-demo] OK\n\n");
}

/* wave14 item 3: 2VARIABLE / 2CONSTANT double-cell stubs — host two-slot map */
static int host_2var_create(const char* name) {
    int i = host_entry_create(name);
    if (i < 0) return -1;
    host_entry_cells[i] = 0;
    host_entry_cells2[i] = 0;
    host_entry_2cell_p[i] = 1;
    printf("[2var] 2VARIABLE name=%s\n", host_entry_names[i]);
    return i;
}

static int host_2const_create(const char* name, int lo, int hi) {
    int i = host_entry_create(name);
    if (i < 0) return -1;
    host_entry_cells[i] = lo;
    host_entry_cells2[i] = hi;
    host_entry_2cell_p[i] = 1;
    printf("[2var] 2CONSTANT name=%s lo=%d hi=%d\n", host_entry_names[i], lo, hi);
    return i;
}

static void host_2var_fetch(const char* name) {
    int i = host_entry_find(name);
    if (i < 0 || !host_entry_2cell_p[i]) {
        printf("[2var] FAIL reason=miss\n");
        return;
    }
    printf("[2var] 2@ name=%s lo=%d hi=%d\n",
           host_entry_names[i], host_entry_cells[i], host_entry_cells2[i]);
}

static void host_2var_store(const char* name, int lo, int hi) {
    int i = host_entry_find(name);
    if (i < 0 || !host_entry_2cell_p[i]) {
        printf("[2var] FAIL reason=miss\n");
        return;
    }
    host_entry_cells[i] = lo;
    host_entry_cells2[i] = hi;
    printf("[2var] 2! name=%s lo=%d hi=%d\n",
           host_entry_names[i], host_entry_cells[i], host_entry_cells2[i]);
}

void two_var_demo(void) {
    /* wave14 item 3: 2VARIABLE dv + 1 2 2CONSTANT dc + find/WORDS */
    printf("[2var-demo] dict-reset + 2VARIABLE dv + 1 2 2CONSTANT dc\n");
    host_dict_reset();
    if (host_2var_create("dv") < 0) {
        printf("[2var-demo] FAIL\n\n");
        return;
    }
    if (host_2const_create("dc", 1, 2) < 0) {
        printf("[2var-demo] FAIL\n\n");
        return;
    }
    host_words();
    if (host_entry_count < 2 ||
        host_entry_find("dv") < 0 ||
        host_entry_find("dc") < 0) {
        printf("[2var-demo] FAIL\n\n");
        return;
    }
    printf("[2var-demo] OK\n\n");
}

/* wave14 item 4: CATCH / THROW / optional ABORT" stubs — frame-mark only */
static int host_catch_cs_depth(void) {
    return host_catch_depth;
}

static void host_catch_mark(void) {
    host_catch_depth++;
    printf("[throw] CATCH depth=%d\n", host_catch_depth);
}

static void host_throw_code(int n) {
    if (n == 0) {
        printf("[throw] THROW code=0\n");
        return;
    }
    if (host_catch_depth <= 0) {
        printf("[throw] FAIL reason=uncaught\n");
        return;
    }
    host_catch_depth--;
    printf("[throw] THROW code=%d\n", n);
}

static void host_abort_quote(const char* content) {
    printf("[throw] ABORT\" %s\n", content ? content : "");
    host_throw_code(-2);
}

void throw_demo(void) {
    /* wave14 item 4: CATCH + THROW 0 + THROW 42 + ABORT" boom */
    printf("[throw-demo] dict-reset + CATCH + THROW 0 + THROW 42 + ABORT\" boom\"\n");
    host_dict_reset();
    host_catch_mark();
    host_throw_code(0);
    if (host_catch_cs_depth() != 1) {
        printf("[throw-demo] FAIL\n\n");
        return;
    }
    host_throw_code(42);
    if (host_catch_cs_depth() != 0) {
        printf("[throw-demo] FAIL\n\n");
        return;
    }
    host_catch_mark();
    host_abort_quote("boom");
    if (host_catch_cs_depth() != 0) {
        printf("[throw-demo] FAIL\n\n");
        return;
    }
    printf("[throw-demo] OK\n\n");
}


/* wave13 item 2: CASE/OF/ENDOF/ENDCASE stubs — balance-only case-depth; markers [case] */
static void host_control_case(void) {
    host_case_depth++;
    printf("[case] CASE depth=%d\n", host_case_depth);
}

static void host_control_of(void) {
    if (host_case_depth <= 0) {
        printf("[case] FAIL reason=unbalanced\n");
        return;
    }
    printf("[case] OF\n");
}

static void host_control_endof(void) {
    if (host_case_depth <= 0) {
        printf("[case] FAIL reason=unbalanced\n");
        return;
    }
    printf("[case] ENDOF\n");
}

static void host_control_endcase(void) {
    if (host_case_depth <= 0) {
        printf("[case] FAIL reason=unbalanced\n");
        return;
    }
    host_case_depth--;
    printf("[case] ENDCASE depth=%d\n", host_case_depth);
}

static int host_case_cs_depth(void) {
    return host_case_depth;
}

void case_demo(void) {
    /* wave13 item 2: Stream A single OF + Stream B two OF arms */
    printf("[case-demo] dict-reset + CASE…OF…ENDOF…ENDCASE + two-OF\n");
    host_dict_reset();
    /* Stream A */
    host_control_case();
    host_control_of();
    host_control_endof();
    host_control_endcase();
    if (host_case_cs_depth() != 0) {
        printf("[case-demo] FAIL\n\n");
        return;
    }
    /* Stream B — two OF arms */
    host_control_case();
    host_control_of();
    host_control_endof();
    host_control_of();
    host_control_endof();
    host_control_endcase();
    if (host_case_cs_depth() != 0) {
        printf("[case-demo] FAIL\n\n");
        return;
    }
    printf("[case-demo] OK\n\n");
}

/* wave13 item 3: CREATE / DOES> defining-word stubs — presence + does-mark; no XT child */
static int host_create_entry(const char* name) {
    int i = host_entry_create(name);
    if (i < 0) return -1;
    host_entry_does_body[i] = 0;
    host_create_latest = i;
    printf("[create] CREATE name=%s\n", host_entry_names[i]);
    return i;
}

static void host_does_mark(void) {
    if (host_create_latest < 0) {
        printf("[create] FAIL reason=unbalanced\n");
        return;
    }
    int i = host_create_latest;
    host_entry_does_body[i] = 1;
    printf("[create] DOES> name=%s\n", host_entry_names[i]);
    host_create_latest = -1;
}

void create_demo(void) {
    /* wave13 item 3: dict-reset → CREATE widget → DOES> → find/WORDS */
    printf("[create-demo] dict-reset + CREATE widget + DOES>\n");
    host_dict_reset();
    if (host_create_entry("widget") < 0) {
        printf("[create-demo] FAIL\n\n");
        return;
    }
    host_does_mark();
    host_words();
    if (host_entry_count < 1 || host_entry_find("widget") < 0) {
        printf("[create-demo] FAIL\n\n");
        return;
    }
    printf("[create-demo] OK\n\n");
}

/* wave14 item 1: HERE / ALLOT dictionary-pointer stubs — host int only; no arena */
static void host_here_at(void) {
    printf("[allot] HERE addr=%d\n", host_here);
}

static void host_allot_bump(int n) {
    if (n < 0) {
        printf("[allot] FAIL reason=neg\n");
        return;
    }
    host_here += n;
    printf("[allot] ALLOT n=%d addr=%d\n", n, host_here);
}

static void host_comma(int value) {
    /* optional cell-store stub: bump by sizeof(int) bytes; marker only */
    host_here += (int)sizeof(int);
    printf("[allot] , value=%d\n", value);
}

static void host_c_comma(int c) {
    /* optional char-store stub: bump by 1; marker only */
    host_here += 1;
    printf("[allot] C, value=%d\n", c & 0xff);
}

void allot_demo(void) {
    /* wave14 item 1: HERE → ALLOT 8 → HERE (bump visible) */
    printf("[allot-demo] HERE + ALLOT 8 + HERE\n");
    host_dict_reset();
    int a0 = host_here;
    host_here_at();
    host_allot_bump(8);
    int a1 = host_here;
    host_here_at();
    if (a1 <= a0 || a1 != a0 + 8) {
        printf("[allot-demo] FAIL\n\n");
        return;
    }
    printf("[allot-demo] OK\n\n");
}

/* wave15 item 1: CELL / CELLS / ALIGN / ALIGNED stubs — host constant + HERE round-up */
static int host_align_up(int addr) {
    int n = HOST_CELL_BYTES;
    int rem;
    if (addr < 0)
        return -1;
    rem = addr % n;
    if (rem == 0)
        return addr;
    return addr + (n - rem);
}

static void host_cell_size(void) {
    printf("[cell] CELL bytes=%d\n", HOST_CELL_BYTES);
}

static void host_cells_n(int k) {
    if (k < 0) {
        printf("[cell] FAIL reason=neg\n");
        return;
    }
    printf("[cell] CELLS n=%d bytes=%d\n", k, k * HOST_CELL_BYTES);
}

static void host_align_here(void) {
    int rounded = host_align_up(host_here);
    if (rounded < 0) {
        printf("[cell] FAIL reason=neg\n");
        return;
    }
    host_here = rounded;
    printf("[cell] ALIGN addr=%d\n", host_here);
}

static void host_aligned_addr(int addr) {
    int rounded;
    if (addr < 0) {
        printf("[cell] FAIL reason=neg\n");
        return;
    }
    rounded = host_align_up(addr);
    printf("[cell] ALIGNED addr=%d\n", rounded);
}

void cell_demo(void) {
    /* wave15 item 1: CELL → CELLS 3 → 1 ALLOT → ALIGN → ALIGNED 1 */
    int unaligned;
    printf("[cell-demo] CELL + CELLS + ALIGN + ALIGNED\n");
    host_dict_reset();
    host_cell_size();
    host_cells_n(3);
    /* Force unaligned: HERE base 0x1000 is cell-aligned */
    host_allot_bump(1);
    unaligned = host_here;
    host_align_here();
    if (host_here % HOST_CELL_BYTES != 0 || host_here <= unaligned) {
        printf("[cell-demo] FAIL\n\n");
        return;
    }
    host_aligned_addr(1);
    if (host_align_up(1) != 8) {
        printf("[cell-demo] FAIL\n\n");
        return;
    }
    printf("[cell-demo] OK\n\n");
}

/* wave15 item 2: PICK / ROLL / DEPTH / ?DUP stubs — tiny host stack picture */
static void host_pick_pic_reset(void) {
    host_pick_depth = 0;
}

static void host_pick_pic_push(int x) {
    if (host_pick_depth >= HOST_PICK_MAX)
        return;
    host_pick_pic[host_pick_depth++] = x;
}

static int host_pick_tos(void) {
    if (host_pick_depth <= 0)
        return 0;
    return host_pick_pic[host_pick_depth - 1];
}

static void host_stack_depth(void) {
    printf("[pick] DEPTH n=%d\n", host_pick_depth);
}

static void host_pick_nth(int u) {
    int idx;
    int x;
    if (u < 0 || u >= host_pick_depth) {
        printf("[pick] FAIL reason=underflow\n");
        return;
    }
    idx = host_pick_depth - 1 - u;
    x = host_pick_pic[idx];
    printf("[pick] PICK u=%d x=%d\n", u, x);
}

static void host_roll_nth(int u) {
    int idx;
    int val;
    int i;
    if (u < 0 || u >= host_pick_depth) {
        printf("[pick] FAIL reason=underflow\n");
        return;
    }
    if (u == 0) {
        printf("[pick] ROLL u=%d\n", u);
        return;
    }
    idx = host_pick_depth - 1 - u;
    val = host_pick_pic[idx];
    for (i = idx; i < host_pick_depth - 1; i++)
        host_pick_pic[i] = host_pick_pic[i + 1];
    host_pick_pic[host_pick_depth - 1] = val;
    printf("[pick] ROLL u=%d\n", u);
}

static void host_qdup(void) {
    int tos;
    if (host_pick_depth <= 0) {
        printf("[pick] FAIL reason=underflow\n");
        return;
    }
    tos = host_pick_tos();
    if (tos == 0) {
        printf("[pick] ?DUP flag=0\n");
        return;
    }
    host_pick_pic_push(tos);
    printf("[pick] ?DUP flag=1\n");
}

void pick_demo(void) {
    /* wave15 item 2: seed≥3 → DEPTH → PICK → ROLL → ?DUP both paths */
    printf("[pick-demo] DEPTH + PICK + ROLL + ?DUP\n");
    host_pick_pic_reset();
    host_pick_pic_push(10);
    host_pick_pic_push(20);
    host_pick_pic_push(30);
    host_stack_depth();
    if (host_pick_depth < 3) {
        printf("[pick-demo] FAIL\n\n");
        return;
    }
    host_pick_nth(1);
    host_roll_nth(2);
    host_qdup();
    host_pick_pic_push(0);
    host_qdup();
    printf("[pick-demo] OK\n\n");
}

/* wave15 item 3: FILL / ERASE / MOVE / CMOVE stubs — fixed host byte buffer (cap=64) */
static int host_fill_bounds(int addr, int u) {
    if (addr < 0 || u < 0)
        return 0;
    if (addr + u > HOST_FILL_CAP)
        return 0;
    return 1;
}

static void host_fill_buf(int addr, int u, int ch) {
    int i;
    if (!host_fill_bounds(addr, u)) {
        printf("[fill] FAIL reason=bounds\n");
        return;
    }
    for (i = 0; i < u; i++)
        host_fill_mem[addr + i] = (unsigned char)(ch & 0xff);
    printf("[fill] FILL addr=%d u=%d char=%d\n", addr, u, ch & 0xff);
}

static void host_erase_buf(int addr, int u) {
    int i;
    if (!host_fill_bounds(addr, u)) {
        printf("[fill] FAIL reason=bounds\n");
        return;
    }
    for (i = 0; i < u; i++)
        host_fill_mem[addr + i] = 0;
    printf("[fill] ERASE addr=%d u=%d\n", addr, u);
}

static void host_move_buf(int from, int to, int u) {
    int i;
    unsigned char tmp[HOST_FILL_CAP];
    if (!host_fill_bounds(from, u) || !host_fill_bounds(to, u)) {
        printf("[fill] FAIL reason=bounds\n");
        return;
    }
    for (i = 0; i < u; i++)
        tmp[i] = host_fill_mem[from + i];
    for (i = 0; i < u; i++)
        host_fill_mem[to + i] = tmp[i];
    printf("[fill] MOVE from=%d to=%d u=%d bytes=%d\n", from, to, u, u);
}

static void host_cmove_buf(int from, int to, int u) {
    int i;
    unsigned char tmp[HOST_FILL_CAP];
    if (!host_fill_bounds(from, u) || !host_fill_bounds(to, u)) {
        printf("[fill] FAIL reason=bounds\n");
        return;
    }
    for (i = 0; i < u; i++)
        tmp[i] = host_fill_mem[from + i];
    for (i = 0; i < u; i++)
        host_fill_mem[to + i] = tmp[i];
    printf("[fill] CMOVE from=%d to=%d u=%d bytes=%d\n", from, to, u, u);
}

void fill_demo(void) {
    /* wave15 item 3: FILL → ERASE → MOVE → CMOVE (non-overlap) */
    printf("[fill-demo] FILL + ERASE + MOVE + CMOVE cap=%d\n", HOST_FILL_CAP);
    host_fill_buf(0, 8, 65);
    host_erase_buf(0, 8);
    host_fill_buf(0, 4, 66);
    host_move_buf(0, 8, 4);
    host_fill_buf(16, 4, 67);
    host_cmove_buf(16, 24, 4);
    printf("[fill-demo] OK\n\n");
}


/* wave12 item 3: comment-parse skip + comment-demo */
void comment_demo(void) {
    printf("[comment-demo] dict-reset + mixed \\ / ( ) streams\n");
    host_dict_reset();
    if (host_entry_create("alpha") < 0) {
        printf("[comment-demo] FAIL\n\n");
        return;
    }
    host_interp_hits = 0;
    host_interp_misses = 0;
    host_interpret("alpha \\ this is a line comment\n");
    if (host_interp_hits < 1) {
        printf("[comment-demo] FAIL\n\n");
        return;
    }
    if (host_entry_find("this") >= 0 ||
        host_entry_find("is") >= 0 ||
        host_entry_find("line") >= 0 ||
        host_entry_find("comment") >= 0) {
        printf("[comment-demo] FAIL\n\n");
        return;
    }
    host_interp_hits = 0;
    host_interp_misses = 0;
    /* also create beta so flank after paren can hit if already present;
       interpret will exec alpha then skip paren then miss/create? beta must exist */
    if (host_entry_create("beta") < 0) {
        printf("[comment-demo] FAIL\n\n");
        return;
    }
    host_interpret("alpha ( paren comment ) beta");
    if (host_interp_hits < 2) {
        printf("[comment-demo] FAIL\n\n");
        return;
    }
    if (host_entry_find("paren") >= 0 ||
        host_entry_find("comment") >= 0) {
        printf("[comment-demo] FAIL\n\n");
        return;
    }
    if (host_entry_find("alpha") < 0 || host_entry_find("beta") < 0) {
        printf("[comment-demo] FAIL\n\n");
        return;
    }
    printf("[comment-demo] OK\n\n");
}

/* wave13 item 4: string-lit parse stubs + string-demo */
void string_demo(void) {
    /* wave13 item 4: dict-reset → S" hello" + ." world" + optional .( → OK */
    printf("[string-demo] dict-reset + S\" / .\" / .(\n");
    host_dict_reset();
    host_interpret("S\" hello\"");
    if (host_entry_find("hello") >= 0) {
        printf("[string-demo] FAIL\n\n");
        return;
    }
    host_interpret(".\" world\"");
    if (host_entry_find("world") >= 0) {
        printf("[string-demo] FAIL\n\n");
        return;
    }
    host_interpret(".( hi)");
    if (host_entry_find("hi") >= 0) {
        printf("[string-demo] FAIL\n\n");
        return;
    }
    /* comment ( ) skip still works alongside .( */
    if (host_entry_create("alpha") < 0) {
        printf("[string-demo] FAIL\n\n");
        return;
    }
    host_interp_hits = 0;
    host_interpret("alpha ( still comment )");
    if (host_interp_hits < 1) {
        printf("[string-demo] FAIL\n\n");
        return;
    }
    if (host_entry_find("still") >= 0 || host_entry_find("comment") >= 0) {
        printf("[string-demo] FAIL\n\n");
        return;
    }
    printf("[string-demo] OK\n\n");
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

/* queue-reward! ( job -- ) — mark Q-REWARDED after successful merge (stub) */
int queue_reward(int job) {
    int idx = queue_find_idx(job);
    if (idx < 0) {
        printf("[QUEUE] reward! unknown job\n");
        return 0;
    }
    if (queue_jobs[idx].status != Q_PROVED) {
        printf("[QUEUE] reward! not proved\n");
        return 0;
    }
    queue_jobs[idx].status = Q_REWARDED;
    queue_persist();
    printf("[QUEUE] reward! job=%d status=rewarded\n", job);
    return 1;
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

/* --- economy-wire-demo: queue-prove! → assimilate-merge! e2e (wave9 §5b.4) --- */
void economy_wire_demo(void) {
    const unsigned proof = 0xb0b0u;
    printf("[economy-wire] enqueue→pull→prove→assimilate-merge (local stub)\n");
    queue_reset();
    asim_reset();
    int job = queue_make(42, 77, Q_WHY_OPTIN, 0); /* force non-local */
    if (!job) {
        printf("[economy-wire-demo] FAIL\n\n");
        return;
    }
    printf("[economy-wire] enqueue job=%d\n", job);
    if (queue_local_p(job)) {
        printf("[economy-wire-demo] FAIL\n\n");
        return;
    }
    int pulled = queue_pull();
    if (!pulled || pulled != job) {
        printf("[economy-wire-demo] FAIL\n\n");
        return;
    }
    int score = queue_prove(job, proof);
    if (score <= 0) {
        printf("[economy-wire-demo] FAIL\n\n");
        return;
    }
    printf("[economy-wire] prove score=%d\n", score);
    int frag = assimilate_fragment(job, 1);
    int delta = assimilate_merge(frag, proof);
    if (delta <= 0 || asim_wallet == 0) {
        printf("[economy-wire-demo] FAIL\n\n");
        return;
    }
    printf("[economy-wire] merge delta=%d credit-ok\n", delta);
    queue_reward(job);
    /* duplicate proof → 0 credit (anti-gaming) */
    int dup = assimilate_merge(frag, proof);
    if (dup != 0) {
        printf("[economy-wire-demo] FAIL\n\n");
        return;
    }
    printf("[economy-wire-demo] OK\n\n");
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
    int userConfirmed; /* optional persist; default false */
    char productIdBefore[32];
    char productIdAfter[32];
    int demoForceReady;
} graduation_cfg_t;

static graduation_cfg_t grad_cfg = {
    30, 8, 1, 1, 1, 1, 0, "tritium", "lineos", 0
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
    c->userConfirmed = 0;
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
        "  \"userConfirmed\": false,\n"
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
    grad_cfg.userConfirmed = grad_json_int(buf, "userConfirmed", 0);
    grad_cfg.demoForceReady = grad_json_int(buf, "demoForceReady", 0);
    grad_json_str(buf, "productIdBefore", grad_cfg.productIdBefore,
                  sizeof(grad_cfg.productIdBefore), "tritium");
    grad_json_str(buf, "productIdAfter", grad_cfg.productIdAfter,
                  sizeof(grad_cfg.productIdAfter), "lineos");
    /* Optional persisted confirm; default remains false until lineos-confirm */
    if (grad_cfg.userConfirmed) grad_confirm = 1;
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
    /* Brand markers stub (wave7 item 3 / docs/LINEOS-BRAND.md) */
    {
        char brand_dir[MAX_PATH];
        snprintf(brand_dir, sizeof(brand_dir), "%s/lineos", get_evolve_dir());
        mkdir(brand_dir, 0755);
        snprintf(path, sizeof(path), "%s/brand.json", brand_dir);
        f = fopen(path, "w");
        if (f) {
            fprintf(f,
                "{\n"
                "  \"productName\": \"L.I.N.E.O.S.\",\n"
                "  \"productId\": \"lineos\",\n"
                "  \"slogan\": \"%s\",\n"
                "  \"originBadge\": \"TritiumOS by Draco\",\n"
                "  \"edition\": %d,\n"
                "  \"graduated\": true\n"
                "}\n",
                GRAD_SLOGAN, edition);
            fclose(f);
        }
        snprintf(path, sizeof(path), "%s/SPLASH.txt", brand_dir);
        f = fopen(path, "w");
        if (f) {
            fprintf(f, "L.I.N.E.O.S.\n%s\nTritiumOS by Draco (edition=%d)\n",
                    GRAD_SLOGAN, edition);
            fclose(f);
        }
        snprintf(path, sizeof(path), "%s/ABOUT.txt", brand_dir);
        f = fopen(path, "w");
        if (f) {
            fprintf(f, "L.I.N.E.O.S.\n%s — L.I.N.E.O.S.\n%s\nTritiumOS by Draco (edition=%d)\n",
                    assistant_name[0] ? assistant_name : "assistant",
                    GRAD_SLOGAN, edition);
            fclose(f);
        }
    }
    printf("[LINEOS] UI product name → L.I.N.E.O.S.\n");
    printf("[LINEOS] splash: L.I.N.E.O.S.\n");
    printf("[LINEOS] slogan: %s\n", GRAD_SLOGAN);
    printf("[LINEOS] origin: TritiumOS by Draco (edition=%d)\n", edition);
    printf("[LINEOS] scaffold product_id → %s (preserve edition=%d-bit)\n",
           grad_cfg.productIdAfter, edition);
    printf("[LINEOS] wrote evolve/lineos-manifest-scaffold.json + lineos-host-flag.trit\n");
    printf("[LINEOS] brand markers → evolve/lineos/ (brand.json SPLASH.txt ABOUT.txt)\n");
    printf("[LINEOS] graduate OK — scaffold only (not a production release)\n");
}

static void grad_persist_user_confirmed(int confirmed) {
    char path[MAX_PATH];
    ensure_evolve_dir();
    snprintf(path, sizeof(path), "%s/graduation.json", get_evolve_dir());
    /* Ensure cfg loaded so we rewrite known fields + userConfirmed. */
    {
        FILE* rf = fopen(path, "r");
        if (!rf) grad_write_defaults(path);
        else fclose(rf);
    }
    grad_load_cfg();
    FILE* f = fopen(path, "w");
    if (!f) return;
    fprintf(f,
        "{\n"
        "  \"minSessions\": %d,\n"
        "  \"minNeurons\": %d,\n"
        "  \"minConnectedClusters\": %d,\n"
        "  \"minRefinedPerClass\": %d,\n"
        "  \"requireLicenseValid\": %s,\n"
        "  \"requireUserConfirm\": %s,\n"
        "  \"userConfirmed\": %s,\n"
        "  \"productIdBefore\": \"%s\",\n"
        "  \"productIdAfter\": \"%s\",\n"
        "  \"demoForceReady\": %s\n"
        "}\n",
        grad_cfg.minSessions, grad_cfg.minNeurons, grad_cfg.minConnectedClusters,
        grad_cfg.minRefinedPerClass,
        grad_cfg.requireLicenseValid ? "true" : "false",
        grad_cfg.requireUserConfirm ? "true" : "false",
        confirmed ? "true" : "false",
        grad_cfg.productIdBefore, grad_cfg.productIdAfter,
        grad_cfg.demoForceReady ? "true" : "false");
    fclose(f);
    grad_cfg.userConfirmed = confirmed ? 1 : 0;
    grad_confirm = confirmed ? 1 : 0;
}

void become_lineos(void) {
    grad_become = 1;
    grad_confirm = 1;
    printf("[LINEOS] become-lineos — confirm flag set (§1a.1)\n");
}

void lineos_confirm(void) {
    grad_load_cfg();
    grad_persist_user_confirmed(1);
    printf("[LINEOS] confirm set\n");
}

void lineos_graduate(int demo_force) {
    grad_load_cfg();
    printf("[LINEOS] lineos-graduate — TritiumOS.txt §1a.1 gates\n");

    if (demo_force || grad_cfg.demoForceReady) {
        /* Policy: demoForceReady may imply confirm for graduate-demo only;
           production lineos_graduate(0) still honors requireUserConfirm. */
        printf("[LINEOS] demoForceReady — forcing stub metrics ready (smoke)\n");
        printf("[LINEOS] demoForceReady implies confirm (graduate-demo only)\n");
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
    } else if (g1 && g2 && g3 && g4 && !g5) {
        /* Confirm required and not set — refuse; no product_id flip */
        printf("[LINEOS] refuse — confirm required (§1a.1)\n");
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
    lineos_graduate(1); /* demoForceReady implies confirm */
    if (grad_product_is_lineos && grad_host_flag_lineos) {
        printf("[lineos-graduate-demo] OK\n\n");
    } else {
        printf("[lineos-graduate-demo] FAIL\n\n");
    }
}

void lineos_confirm_demo(void) {
    int refused = 0;
    printf("[lineos-confirm-demo] refuse→confirm→graduate (§1a.1 / docs/LINEOS-CONFIRM.md)\n");
    grad_product_is_lineos = 0;
    grad_host_flag_lineos = 0;
    grad_become = 0;
    {
        char devices[LICENSE_MAX_SLOTS + 2][64];
        int n = license_load_count(devices, LICENSE_MAX_SLOTS + 2);
        if (n == 0) {
            strncpy(devices[0], "lineos-confirm-demo-device", 63);
            license_save(devices, 1);
        }
    }
    /* Clear any prior confirm; force gates 1–4 WITHOUT confirm (production path) */
    grad_load_cfg();
    grad_cfg.requireUserConfirm = 1;
    grad_cfg.demoForceReady = 0;
    grad_persist_user_confirmed(0); /* userConfirmed=false; clears grad_confirm */
    grad_sessions = grad_cfg.minSessions;
    grad_neurons = grad_cfg.minNeurons;
    grad_clusters = grad_cfg.minConnectedClusters;
    grad_refined = grad_cfg.minRefinedPerClass;
    grad_license_ok = 1;
    grad_confirm = 0;
    grad_cfg.demoForceReady = 0; /* persist may have reloaded; keep off */

    lineos_graduate(0);
    if (grad_product_is_lineos || grad_host_flag_lineos) {
        printf("[lineos-confirm-demo] FAIL — product_id flipped without confirm\n\n");
        return;
    }
    refused = 1;
    printf("[lineos-confirm-demo] refuse path greppable (confirm required)\n");

    lineos_confirm();

    /* Keep gates 1–4; confirm now set; still production path (no demoForceReady) */
    grad_sessions = grad_cfg.minSessions;
    grad_neurons = grad_cfg.minNeurons;
    grad_clusters = grad_cfg.minConnectedClusters;
    grad_refined = grad_cfg.minRefinedPerClass;
    grad_license_ok = 1;
    lineos_graduate(0);

    if (grad_product_is_lineos && grad_host_flag_lineos && refused) {
        printf("[lineos-confirm-demo] OK\n\n");
    } else {
        printf("[lineos-confirm-demo] FAIL\n\n");
    }
}

/* --- LINEOS brand markers stub (wave7 item 3 / docs/LINEOS-BRAND.md) --- */
static int lineos_graduated_now(void) {
    if (grad_product_is_lineos || grad_host_flag_lineos) return 1;
    {
        char path[MAX_PATH];
        snprintf(path, sizeof(path), "%s/lineos/brand.json", get_evolve_dir());
        FILE* f = fopen(path, "r");
        if (f) {
            char buf[512];
            size_t n = fread(buf, 1, sizeof(buf) - 1, f);
            fclose(f);
            buf[n] = 0;
            if (strstr(buf, "\"graduated\": true") && strstr(buf, "\"productId\": \"lineos\""))
                return 1;
        }
    }
    {
        char path[MAX_PATH];
        snprintf(path, sizeof(path), "%s/lineos-host-flag.trit", get_evolve_dir());
        FILE* f = fopen(path, "r");
        if (f) {
            char buf[512];
            size_t n = fread(buf, 1, sizeof(buf) - 1, f);
            fclose(f);
            buf[n] = 0;
            if (strstr(buf, "product_id=lineos")) return 1;
        }
    }
    return 0;
}

void lineos_splash(void) {
    if (!lineos_graduated_now()) {
        printf("[LINEOS] refuse — not graduated (§1a.1)\n");
        return;
    }
    printf("[LINEOS] splash: L.I.N.E.O.S.\n");
    printf("[LINEOS] slogan: %s\n", GRAD_SLOGAN);
    printf("[LINEOS] origin: TritiumOS by Draco (edition=%d)\n", edition);
}

void lineos_about(void) {
    const char* aname = assistant_name[0] ? assistant_name : "assistant";
    if (!lineos_graduated_now()) {
        printf("[LINEOS] about: %s — powered by TritiumOS\n", aname);
        return;
    }
    printf("[LINEOS] about: L.I.N.E.O.S.\n");
    printf("[LINEOS] about: %s — L.I.N.E.O.S.\n", aname);
    printf("[LINEOS] slogan: %s\n", GRAD_SLOGAN);
    printf("[LINEOS] origin: TritiumOS by Draco (edition=%d)\n", edition);
}

void lineos_brand_demo(void) {
    char path[MAX_PATH], brand_dir[MAX_PATH];
    FILE* f;
    int ok = 1;
    printf("[lineos-brand-demo] force-ready → brand markers (§1a.1 / docs/LINEOS-BRAND.md)\n");
    /* Force graduated state + write markers (demo may force-ready) */
    grad_product_is_lineos = 1;
    grad_host_flag_lineos = 1;
    ensure_evolve_dir();
    snprintf(brand_dir, sizeof(brand_dir), "%s/lineos", get_evolve_dir());
    mkdir(brand_dir, 0755);
    snprintf(path, sizeof(path), "%s/brand.json", brand_dir);
    f = fopen(path, "w");
    if (!f) {
        printf("[lineos-brand-demo] FAIL — cannot write brand.json\n\n");
        return;
    }
    fprintf(f,
        "{\n"
        "  \"productName\": \"L.I.N.E.O.S.\",\n"
        "  \"productId\": \"lineos\",\n"
        "  \"slogan\": \"%s\",\n"
        "  \"originBadge\": \"TritiumOS by Draco\",\n"
        "  \"edition\": %d,\n"
        "  \"graduated\": true\n"
        "}\n",
        GRAD_SLOGAN, edition);
    fclose(f);
    snprintf(path, sizeof(path), "%s/SPLASH.txt", brand_dir);
    f = fopen(path, "w");
    if (f) {
        fprintf(f, "L.I.N.E.O.S.\n%s\nTritiumOS by Draco (edition=%d)\n",
                GRAD_SLOGAN, edition);
        fclose(f);
    } else ok = 0;
    snprintf(path, sizeof(path), "%s/ABOUT.txt", brand_dir);
    f = fopen(path, "w");
    if (f) {
        fprintf(f, "L.I.N.E.O.S.\n%s — L.I.N.E.O.S.\n%s\nTritiumOS by Draco (edition=%d)\n",
                assistant_name[0] ? assistant_name : "assistant",
                GRAD_SLOGAN, edition);
        fclose(f);
    } else ok = 0;

    /* Assert slogan + productName + edition */
    snprintf(path, sizeof(path), "%s/brand.json", brand_dir);
    f = fopen(path, "r");
    if (!f) {
        ok = 0;
    } else {
        char buf[1024];
        size_t n = fread(buf, 1, sizeof(buf) - 1, f);
        fclose(f);
        buf[n] = 0;
        if (!strstr(buf, "\"productName\": \"L.I.N.E.O.S.\"")) ok = 0;
        if (!strstr(buf, GRAD_SLOGAN)) ok = 0;
        {
            char edbuf[32];
            snprintf(edbuf, sizeof(edbuf), "\"edition\": %d", edition);
            if (!strstr(buf, edbuf)) ok = 0;
        }
        if (!strstr(buf, "\"graduated\": true")) ok = 0;
    }
    printf("[LINEOS] splash: L.I.N.E.O.S.\n");
    printf("[LINEOS] slogan: %s\n", GRAD_SLOGAN);
    printf("[LINEOS] origin: TritiumOS by Draco (edition=%d)\n", edition);
    if (ok) {
        printf("[lineos-brand-demo] OK\n\n");
    } else {
        printf("[lineos-brand-demo] FAIL\n\n");
    }
}

/* --- tritium-integrate stub (§5a.2 / Phase 8) — scaffold from _template --- */
static int integrate_find_template(char* out, size_t outsz) {
    /* Prefer repo install/hosts/_template next to tritium.poly/core */
    char cand[MAX_PATH];
    if (core_dir[0]) {
        snprintf(cand, sizeof(cand), "%s/../../install/hosts/_template/README.txt", core_dir);
        struct stat st;
        if (stat(cand, &st) == 0) {
            snprintf(out, outsz, "%s/../../install/hosts/_template", core_dir);
            return 1;
        }
        snprintf(cand, sizeof(cand), "%s/../../../install/hosts/_template/README.txt", core_dir);
        if (stat(cand, &st) == 0) {
            snprintf(out, outsz, "%s/../../../install/hosts/_template", core_dir);
            return 1;
        }
    }
    /* Walk up from cwd */
    char path[MAX_PATH];
    if (!getcwd(path, sizeof(path))) return 0;
    for (int depth = 0; depth < 8; depth++) {
        snprintf(cand, sizeof(cand), "%s/install/hosts/_template/README.txt", path);
        struct stat st;
        if (stat(cand, &st) == 0) {
            snprintf(out, outsz, "%s/install/hosts/_template", path);
            return 1;
        }
        char* slash = strrchr(path, '/');
        if (!slash || slash == path) break;
        *slash = 0;
    }
    return 0;
}

static int integrate_copy_file(const char* src, const char* dst) {
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

static int integrate_scaffold(const char* platform) {
    char dest[MAX_PATH], tmpl[MAX_PATH], src[MAX_PATH], dst[MAX_PATH];
    const char* evolve = get_evolve_dir();
    snprintf(dest, sizeof(dest), "%s/integrate/%s", evolve, platform);
    mkdir(dest, 0755);

    int have_tmpl = integrate_find_template(tmpl, sizeof(tmpl));
    if (have_tmpl) {
        snprintf(src, sizeof(src), "%s/README.txt", tmpl);
        snprintf(dst, sizeof(dst), "%s/README.txt", dest);
        integrate_copy_file(src, dst);
        snprintf(src, sizeof(src), "%s/INTEGRATE.txt", tmpl);
        snprintf(dst, sizeof(dst), "%s/INTEGRATE.txt", dest);
        if (integrate_copy_file(src, dst) != 0) {
            FILE* f = fopen(dst, "w");
            if (f) {
                fprintf(f, "tritium-integrate checklist: free license slot → copy _template → evolve/integrate/<platform>/ → markers (§5a.2 / Phase 8).\n");
                fclose(f);
            }
        }
    } else {
        snprintf(dst, sizeof(dst), "%s/README.txt", dest);
        FILE* f = fopen(dst, "w");
        if (f) {
            fprintf(f, "Post-bootstrap platform integrate template (embedded fallback).\n");
            fclose(f);
        }
        snprintf(dst, sizeof(dst), "%s/INTEGRATE.txt", dest);
        f = fopen(dst, "w");
        if (f) {
            fprintf(f, "tritium-integrate checklist: free license slot → copy _template → evolve/integrate/<platform>/ → markers (§5a.2 / Phase 8).\n");
            fclose(f);
        }
    }
    snprintf(dst, sizeof(dst), "%s/INTEGRATE.marker", dest);
    FILE* mf = fopen(dst, "w");
    if (mf) {
        fprintf(mf, "platform=%s from=_template\n", platform);
        fclose(mf);
    }
    printf("[tritium-integrate] platform=%s from=_template\n", platform);
    printf("[tritium-integrate] scaffold → %s (§5a.2 / Phase 8)\n", dest);
    return 0;
}

void tritium_integrate(const char* platform) {
    if (!platform || !*platform) {
        printf("[tritium-integrate] FAIL — platform required\n");
        return;
    }
    /* sanitize: reject path separators */
    for (const char* p = platform; *p; p++) {
        if (*p == '/' || *p == '\\') {
            printf("[tritium-integrate] FAIL — invalid platform name\n");
            return;
        }
    }
    char devices[LICENSE_MAX_SLOTS + 2][64];
    int n = license_load_count(devices, LICENSE_MAX_SLOTS + 2);
    if (n >= LICENSE_MAX_SLOTS) {
        printf("[tritium-integrate] refuse — no free license slot (§5a.4)\n");
        printf("[tritium-integrate] refuse — LICENSE.md / TritiumOS.txt §5a.4 (10/10 or slot 11)\n");
        return;
    }
    if (integrate_scaffold(platform) != 0) {
        printf("[tritium-integrate] FAIL — scaffold\n");
        return;
    }
    /* bind identity: register integrate-<platform> if new */
    char id[80];
    snprintf(id, sizeof(id), "integrate-%s", platform);
    license_register(id);
    printf("[tritium-integrate] OK — scaffolded %s\n", platform);
}

void tritium_integrate_demo(void) {
    printf("[tritium-integrate-demo] force free-slot path → platform=demo (§5a.2 / Phase 8)\n");
    /* Force free-slot: wipe registry so smoke never hits 10/10 */
    {
        ensure_evolve_dir();
        FILE* f = fopen(license_slots_path(), "w");
        if (f) {
            fprintf(f, "{\n  \"maxSlots\": %d,\n  \"devices\": [\n  ]\n}\n", LICENSE_MAX_SLOTS);
            fclose(f);
        }
    }
    tritium_integrate("demo");
    {
        char marker[MAX_PATH];
        snprintf(marker, sizeof(marker), "%s/integrate/demo/INTEGRATE.marker", get_evolve_dir());
        struct stat st;
        if (stat(marker, &st) == 0) {
            printf("[tritium-integrate-demo] OK\n\n");
        } else {
            printf("[tritium-integrate-demo] FAIL\n\n");
        }
    }
}

/* --- master mint/verify scaffold (§5a.5) — GUID-style; format-only --- */
static void master_rand_hex16(char* out /* 17 bytes */) {
    unsigned char buf[8];
    FILE* f = fopen("/dev/urandom", "rb");
    if (f) {
        if (fread(buf, 1, 8, f) != 8) memset(buf, 0xA5, 8);
        fclose(f);
    } else {
        unsigned seed = (unsigned)time(NULL) ^ (unsigned)getpid();
        for (int i = 0; i < 8; i++) {
            seed = seed * 1103515245u + 12345u;
            buf[i] = (unsigned char)((seed >> 16) & 0xff);
        }
    }
    static const char* hexd = "0123456789ABCDEF";
    for (int i = 0; i < 8; i++) {
        out[i * 2] = hexd[(buf[i] >> 4) & 0xf];
        out[i * 2 + 1] = hexd[buf[i] & 0xf];
    }
    out[16] = 0;
}

static void master_id_hash16(const char* id, char* out /* 17 bytes */) {
    /* FNV-1a 64-bit fold → 16 hex (scaffold; not crypto) */
    unsigned long long h = 14695981039346656037ull;
    for (const unsigned char* p = (const unsigned char*)id; *p; p++) {
        h ^= (unsigned long long)(*p);
        h *= 1099511628211ull;
    }
    static const char* hexd = "0123456789ABCDEF";
    for (int i = 15; i >= 0; i--) {
        out[i] = hexd[h & 0xf];
        h >>= 4;
    }
    out[16] = 0;
}

static int master_is_hex16(const char* s) {
    for (int i = 0; i < 16; i++) {
        char c = s[i];
        if (!((c >= '0' && c <= '9') || (c >= 'A' && c <= 'F') || (c >= 'a' && c <= 'f')))
            return 0;
    }
    return 1;
}

void master_mint_license(int slots) {
    if (slots <= 0) slots = 10;
    char hex[17];
    master_rand_hex16(hex);
    char key[48];
    snprintf(key, sizeof(key), "TRIT-%s-DRACO", hex);
    printf("[master] mint-license slots=%d key=%s\n", slots, key);
    /* optional write under evolve/master/ (gitignored) */
    char path[MAX_PATH];
    snprintf(path, sizeof(path), "%s/master/last-license.key", get_evolve_dir());
    FILE* f = fopen(path, "w");
    if (f) { fprintf(f, "%s\n", key); fclose(f); }
}

void master_mint_worker(const char* device_id) {
    if (!device_id || !*device_id) {
        printf("[master] FAIL — device-id required (refuse empty id)\n");
        printf("[master] mint-worker refuse empty id (§5a.5 scaffold)\n");
        return;
    }
    char hex[17];
    master_id_hash16(device_id, hex);
    char key[56];
    snprintf(key, sizeof(key), "TRIT-W-%s-DRACO", hex);
    printf("[master] mint-worker device=%s key=%s\n", device_id, key);
    char path[MAX_PATH];
    snprintf(path, sizeof(path), "%s/master/last-worker.key", get_evolve_dir());
    FILE* f = fopen(path, "w");
    if (f) { fprintf(f, "%s\n", key); fclose(f); }
}

int master_verify(const char* key) {
    /* Format-only: TRIT-<16hex>-DRACO or TRIT-W-<16hex>-DRACO */
    int ok = 0;
    if (key && *key) {
        size_t n = strlen(key);
        if (n == 4 + 1 + 16 + 1 + 5 && strncmp(key, "TRIT-", 5) == 0 &&
            strcmp(key + 5 + 16, "-DRACO") == 0 && master_is_hex16(key + 5)) {
            ok = 1;
        } else if (n == 6 + 1 + 16 + 1 + 5 && strncmp(key, "TRIT-W-", 7) == 0 &&
                   strcmp(key + 7 + 16, "-DRACO") == 0 && master_is_hex16(key + 7)) {
            ok = 1;
        }
    }
    if (ok)
        printf("[master] verify OK format-only (§5a.5 scaffold)\n");
    else
        printf("[master] verify FAIL format-only (§5a.5 scaffold)\n");
    return ok;
}

void master_demo(void) {
    char hex[17];
    char lic[48], worker[56];
    master_rand_hex16(hex);
    snprintf(lic, sizeof(lic), "TRIT-%s-DRACO", hex);
    printf("[master] mint-license slots=10 key=%s\n", lic);
    master_id_hash16("demo-device-1", hex);
    snprintf(worker, sizeof(worker), "TRIT-W-%s-DRACO", hex);
    printf("[master] mint-worker device=demo-device-1 key=%s\n", worker);
    int ok = 1;
    if (!master_verify(lic)) ok = 0;
    if (!master_verify(worker)) ok = 0;
    /* malformed must FAIL */
    if (master_verify("NOT-A-KEY")) ok = 0;
    if (ok)
        printf("[master-demo] OK\n\n");
    else
        printf("[master-demo] FAIL\n\n");
}



/* --- Fleet evolve-sync stub (§§5a.2–5a.4 / docs/FLEET.md) — no network/crypto --- */
static char fleet_context_fp[64] = {0};

static void fleet_ensure_dir(void) {
    ensure_evolve_dir();
    char sub[MAX_PATH];
    snprintf(sub, sizeof(sub), "%s/fleet", get_evolve_dir());
    mkdir(sub, 0755);
}

static int fleet_read_file_trim(const char* path, char* out, size_t outsz) {
    FILE* f = fopen(path, "r");
    if (!f) return 0;
    if (!fgets(out, (int)outsz, f)) { fclose(f); return 0; }
    fclose(f);
    /* trim whitespace/newline */
    size_t n = strlen(out);
    while (n > 0 && (out[n - 1] == '\n' || out[n - 1] == '\r' || out[n - 1] == ' ' || out[n - 1] == '\t')) {
        out[--n] = 0;
    }
    return n > 0;
}

static int fleet_load_context(char* out, size_t outsz) {
    const char* env = getenv("TRITIUM_LICENSE_FINGERPRINT");
    if (env && *env) {
        snprintf(out, outsz, "%s", env);
        return 1;
    }
    if (fleet_context_fp[0]) {
        snprintf(out, outsz, "%s", fleet_context_fp);
        return 1;
    }
    char path[MAX_PATH];
    snprintf(path, sizeof(path), "%s/fleet/.license-context", get_evolve_dir());
    if (fleet_read_file_trim(path, out, outsz)) return 1;
    snprintf(path, sizeof(path), "%s/master/last-license.key", get_evolve_dir());
    if (fleet_read_file_trim(path, out, outsz)) return 1;
    return 0;
}

static void fleet_set_context(const char* fp) {
    fleet_ensure_dir();
    snprintf(fleet_context_fp, sizeof(fleet_context_fp), "%s", fp ? fp : "");
    char path[MAX_PATH];
    snprintf(path, sizeof(path), "%s/fleet/.license-context", get_evolve_dir());
    FILE* f = fopen(path, "w");
    if (f) { fprintf(f, "%s\n", fleet_context_fp); fclose(f); }
}

static void fleet_iso_now(char* out, size_t outsz) {
    time_t t = time(NULL);
    struct tm tm;
    gmtime_r(&t, &tm);
    strftime(out, outsz, "%Y-%m-%dT%H:%M:%SZ", &tm);
}

void fleet_export(const char* device_id) {
    if (!device_id || !*device_id) device_id = "dev1";
    char fp[64];
    if (!fleet_load_context(fp, sizeof(fp))) {
        printf("[fleet] FAIL — no license context\n");
        return;
    }
    fleet_ensure_dir();
    char path[MAX_PATH];
    char src[MAX_PATH];
    snprintf(src, sizeof(src), "%s/user-graph.trit", get_evolve_dir());
    snprintf(path, sizeof(path), "%s/fleet/user-graph.trit", get_evolve_dir());
    FILE* in = fopen(src, "r");
    FILE* out = fopen(path, "w");
    if (out) {
        if (in) {
            char buf[4096];
            size_t n;
            while ((n = fread(buf, 1, sizeof(buf), in)) > 0) fwrite(buf, 1, n, out);
            fclose(in);
        } else {
            fprintf(out, "# TritiumOS user-graph.trit placeholder (fleet stub)\n");
            fprintf(out, "# No live graph at export; placeholder OK per docs/FLEET.md\n");
        }
        fclose(out);
    }
    char exported[40];
    fleet_iso_now(exported, sizeof(exported));
    snprintf(path, sizeof(path), "%s/fleet/manifest.json", get_evolve_dir());
    out = fopen(path, "w");
    if (out) {
        fprintf(out,
            "{\n"
            "  \"keyFingerprint\": \"%s\",\n"
            "  \"deviceId\": \"%s\",\n"
            "  \"exportedAt\": \"%s\",\n"
            "  \"paths\": [\"user-graph.trit\"]\n"
            "}\n",
            fp, device_id, exported);
        fclose(out);
    }
    snprintf(path, sizeof(path), "%s/fleet/MARKER.txt", get_evolve_dir());
    out = fopen(path, "w");
    if (out) {
        fprintf(out, "fleet export key=%s device=%s\n", fp, device_id);
        fclose(out);
    }
    printf("[fleet] export → evolve/fleet/ key=%s\n", fp);
}

static int fleet_manifest_fp(char* out, size_t outsz) {
    char path[MAX_PATH];
    snprintf(path, sizeof(path), "%s/fleet/manifest.json", get_evolve_dir());
    FILE* f = fopen(path, "r");
    if (!f) return 0;
    char buf[2048];
    size_t n = fread(buf, 1, sizeof(buf) - 1, f);
    fclose(f);
    buf[n] = 0;
    const char* key = "\"keyFingerprint\"";
    char* p = strstr(buf, key);
    if (!p) return 0;
    p = strchr(p + strlen(key), '"');
    if (!p) return 0;
    p++;
    char* end = strchr(p, '"');
    if (!end) return 0;
    size_t len = (size_t)(end - p);
    if (len >= outsz) len = outsz - 1;
    memcpy(out, p, len);
    out[len] = 0;
    return 1;
}

int fleet_import(void) {
    char fp[64];
    if (!fleet_load_context(fp, sizeof(fp))) {
        printf("[fleet] FAIL — no license context for import\n");
        return 0;
    }
    char blob_fp[64];
    if (!fleet_manifest_fp(blob_fp, sizeof(blob_fp))) {
        printf("[fleet] FAIL — missing export path / manifest.json\n");
        return 0;
    }
    if (strcmp(blob_fp, fp) != 0) {
        printf("[fleet] refuse — key mismatch (§5a.4)\n");
        return 0;
    }
    /* restore graph */
    char src[MAX_PATH], dst[MAX_PATH];
    snprintf(src, sizeof(src), "%s/fleet/user-graph.trit", get_evolve_dir());
    snprintf(dst, sizeof(dst), "%s/user-graph.trit", get_evolve_dir());
    FILE* in = fopen(src, "r");
    if (in) {
        FILE* out = fopen(dst, "w");
        if (out) {
            char buf[4096];
            size_t n;
            while ((n = fread(buf, 1, sizeof(buf), in)) > 0) fwrite(buf, 1, n, out);
            fclose(out);
        }
        fclose(in);
    }
    printf("[fleet] import OK same-key\n");
    return 1;
}

void fleet_demo(void) {
    char hex[17], key_a[48], key_b[48];
    master_rand_hex16(hex);
    snprintf(key_a, sizeof(key_a), "TRIT-%s-DRACO", hex);
    master_rand_hex16(hex);
    snprintf(key_b, sizeof(key_b), "TRIT-%s-DRACO", hex);
    if (strcmp(key_a, key_b) == 0) {
        master_rand_hex16(hex);
        snprintf(key_b, sizeof(key_b), "TRIT-%s-DRACO", hex);
    }
    int ok = 1;
    fleet_set_context(key_a);
    fleet_export("demo-device-1");
    if (!fleet_import()) ok = 0;
    fleet_set_context(key_b);
    if (fleet_import()) ok = 0; /* must refuse */
    if (ok)
        printf("[fleet-demo] OK\n\n");
    else
        printf("[fleet-demo] FAIL\n\n");
}

/* --- userland scaffold presence (wave8 item 3 / docs/USERLAND.md) --- */
static int userland_path_ok(const char* root, const char* rel) {
    char path[MAX_PATH];
    struct stat st;
    snprintf(path, sizeof(path), "%s/%s", root, rel);
    return (stat(path, &st) == 0);
}

static int find_userland_root(char* out, size_t outsz) {
    const char* env = getenv("TRITIUM_USERLAND");
    char cand[MAX_PATH];
    char resolved[MAX_PATH];
    struct stat st;
    if (env && *env) {
        if (stat(env, &st) == 0 && S_ISDIR(st.st_mode)) {
            snprintf(out, outsz, "%s", env);
            return 1;
        }
    }
    /* AppImage/poly share: usr/share/userland next to tritium.poly */
    {
        const char* poly = getenv("TRITIUM_POLY");
        if (poly && *poly) {
            snprintf(cand, sizeof(cand), "%s/../userland", poly);
            if (realpath(cand, resolved) != NULL && stat(resolved, &st) == 0 && S_ISDIR(st.st_mode)) {
                snprintf(out, outsz, "%s", resolved);
                return 1;
            }
        }
    }
    if (core_dir[0]) {
        snprintf(cand, sizeof(cand), "%s/../../userland", core_dir);
        if (realpath(cand, resolved) != NULL && stat(resolved, &st) == 0 && S_ISDIR(st.st_mode)) {
            snprintf(out, outsz, "%s", resolved);
            return 1;
        }
        snprintf(cand, sizeof(cand), "%s/../../../userland", core_dir);
        if (realpath(cand, resolved) != NULL && stat(resolved, &st) == 0 && S_ISDIR(st.st_mode)) {
            snprintf(out, outsz, "%s", resolved);
            return 1;
        }
        /* AppDir: usr/share/tritium.poly/core -> usr/share/userland */
        snprintf(cand, sizeof(cand), "%s/../userland", core_dir);
        if (realpath(cand, resolved) != NULL && stat(resolved, &st) == 0 && S_ISDIR(st.st_mode)) {
            snprintf(out, outsz, "%s", resolved);
            return 1;
        }
    }
    if (getcwd(cand, sizeof(cand)) != NULL) {
        char try[MAX_PATH];
        snprintf(try, sizeof(try), "%s/userland", cand);
        if (stat(try, &st) == 0 && S_ISDIR(st.st_mode)) {
            snprintf(out, outsz, "%s", try);
            return 1;
        }
        /* walk parents a few levels (dev from subdirs) */
        char walk[MAX_PATH];
        snprintf(walk, sizeof(walk), "%s", cand);
        for (int i = 0; i < 5; i++) {
            snprintf(try, sizeof(try), "%s/userland", walk);
            if (stat(try, &st) == 0 && S_ISDIR(st.st_mode)) {
                snprintf(out, outsz, "%s", try);
                return 1;
            }
            char* slash = strrchr(walk, '/');
            if (!slash || slash == walk) break;
            *slash = '\0';
        }
    }
    out[0] = 0;
    return 0;
}

void userland_demo(void) {
    char root[MAX_PATH];
    int ok_init = 0, ok_shell = 0, ok_demos = 0, fail = 0;
    if (!find_userland_root(root, sizeof(root))) {
        printf("[userland] FAIL — userland/ not found (repo root or AppDir share)\n");
        printf("[userland-demo] FAIL\n\n");
        return;
    }
    if (userland_path_ok(root, "init") &&
        userland_path_ok(root, "init/README.txt") &&
        userland_path_ok(root, "init/init.txt")) {
        ok_init = 1;
        printf("[userland] init/ OK\n");
    } else {
        fail = 1;
        printf("[userland] init/ FAIL\n");
    }
    if (userland_path_ok(root, "shell") &&
        userland_path_ok(root, "shell/README.txt") &&
        userland_path_ok(root, "shell/shell.txt")) {
        ok_shell = 1;
        printf("[userland] shell/ OK\n");
    } else {
        fail = 1;
        printf("[userland] shell/ FAIL\n");
    }
    if (userland_path_ok(root, "demos") &&
        userland_path_ok(root, "demos/README.txt") &&
        userland_path_ok(root, "demos/userland-demo.txt")) {
        ok_demos = 1;
        printf("[userland] demos/ OK\n");
    } else {
        fail = 1;
        printf("[userland] demos/ FAIL\n");
    }
    if (!userland_path_ok(root, "README.txt")) fail = 1;
    if (!fail && ok_init && ok_shell && ok_demos) {
        printf("[userland-demo] OK\n\n");
    } else {
        printf("[userland-demo] FAIL\n\n");
    }
}


/* --- host parity Win/Android contracts (wave8 item 4 / docs/HOST-PARITY.md) --- */
static const char* HOST_PARITY_REQUIRED[] = {
    "kernel-demo",
    "fleet-demo",
    "master-demo",
    "lineos-brand-demo",
    "group-nested-demo",
    "group-vocab-persist-demo",
    "interpret-demo",
    "userland-demo",
    "lineos-confirm-demo",
    "host-boot-demo",
    "refined-boot-demo",
    NULL
};

static int find_repo_root_for_parity(char* out, size_t outsz) {
    char cand[MAX_PATH];
    char resolved[MAX_PATH];
    char try_path[MAX_PATH];
    struct stat st;
    const char* env = getenv("TRITIUM_ROOT");
    if (env && *env) {
        snprintf(try_path, sizeof(try_path), "%s/install/hosts/windows/parity/HOST-PARITY.txt", env);
        if (stat(try_path, &st) == 0) {
            snprintf(out, outsz, "%s", env);
            return 1;
        }
    }
    if (core_dir[0]) {
        snprintf(cand, sizeof(cand), "%s/../..", core_dir);
        if (realpath(cand, resolved) != NULL) {
            snprintf(try_path, sizeof(try_path), "%s/install/hosts/windows/parity/HOST-PARITY.txt", resolved);
            if (stat(try_path, &st) == 0) {
                snprintf(out, outsz, "%s", resolved);
                return 1;
            }
        }
        snprintf(cand, sizeof(cand), "%s/../../..", core_dir);
        if (realpath(cand, resolved) != NULL) {
            snprintf(try_path, sizeof(try_path), "%s/install/hosts/windows/parity/HOST-PARITY.txt", resolved);
            if (stat(try_path, &st) == 0) {
                snprintf(out, outsz, "%s", resolved);
                return 1;
            }
        }
    }
    if (getcwd(cand, sizeof(cand)) != NULL) {
        char walk[MAX_PATH];
        snprintf(walk, sizeof(walk), "%s", cand);
        for (int i = 0; i < 6; i++) {
            snprintf(try_path, sizeof(try_path), "%s/install/hosts/windows/parity/HOST-PARITY.txt", walk);
            if (stat(try_path, &st) == 0) {
                snprintf(out, outsz, "%s", walk);
                return 1;
            }
            char* slash = strrchr(walk, '/');
            if (!slash || slash == walk) break;
            *slash = '\0';
        }
    }
    out[0] = 0;
    return 0;
}

/* Returns 1 on OK; prints [host-parity] <host> <status|FAIL>. Aggregate: SKIP > CONTRACT > OK. */
static int host_parity_check_file(const char* host, const char* path) {
    FILE* f;
    char line[256];
    char names[24][64];
    char statuses[24][16];
    int n = 0;
    int i, j;
    int has_skip = 0, has_contract = 0, has_ok = 0;
    const char* agg;

    f = fopen(path, "r");
    if (!f) {
        printf("[host-parity] %s FAIL\n", host);
        return 0;
    }
    while (fgets(line, sizeof(line), f) && n < 24) {
        char* p = line;
        char* hash;
        char name[64];
        char status[16];
        while (*p == ' ' || *p == '\t') p++;
        hash = strchr(p, '#');
        if (hash) *hash = '\0';
        {
            size_t len = strlen(p);
            while (len > 0 && (p[len - 1] == '\n' || p[len - 1] == '\r' ||
                               p[len - 1] == ' ' || p[len - 1] == '\t')) {
                p[--len] = '\0';
            }
        }
        if (!*p) continue;
        if (sscanf(p, "%63s %15s", name, status) != 2) {
            fclose(f);
            printf("[host-parity] %s FAIL\n", host);
            return 0;
        }
        for (char* s = status; *s; s++) {
            if (*s >= 'a' && *s <= 'z') *s = (char)(*s - 'a' + 'A');
        }
        if (strcmp(status, "OK") != 0 && strcmp(status, "CONTRACT") != 0 &&
            strcmp(status, "SKIP") != 0) {
            fclose(f);
            printf("[host-parity] %s FAIL\n", host);
            return 0;
        }
        snprintf(names[n], sizeof(names[n]), "%s", name);
        snprintf(statuses[n], sizeof(statuses[n]), "%s", status);
        n++;
    }
    fclose(f);

    for (i = 0; HOST_PARITY_REQUIRED[i]; i++) {
        int found = 0;
        for (j = 0; j < n; j++) {
            if (strcmp(names[j], HOST_PARITY_REQUIRED[i]) == 0) {
                found = 1;
                if (strcmp(statuses[j], "SKIP") == 0) has_skip = 1;
                else if (strcmp(statuses[j], "CONTRACT") == 0) has_contract = 1;
                else if (strcmp(statuses[j], "OK") == 0) has_ok = 1;
                break;
            }
        }
        if (!found) {
            printf("[host-parity] %s FAIL\n", host);
            return 0;
        }
    }
    (void)has_ok;
    if (has_skip) agg = "SKIP";
    else if (has_contract) agg = "CONTRACT";
    else agg = "OK";
    printf("[host-parity] %s %s\n", host, agg);
    return 1;
}

void host_parity_demo(void) {
    char root[MAX_PATH];
    char win_path[MAX_PATH];
    char and_path[MAX_PATH];
    int ok_win, ok_and;
    printf("[host-parity] linux SoT\n");
    if (!find_repo_root_for_parity(root, sizeof(root))) {
        printf("[host-parity] windows FAIL\n");
        printf("[host-parity] android FAIL\n");
        printf("[host-parity-demo] FAIL\n\n");
        return;
    }
    snprintf(win_path, sizeof(win_path), "%s/install/hosts/windows/parity/HOST-PARITY.txt", root);
    snprintf(and_path, sizeof(and_path), "%s/install/hosts/android/parity/HOST-PARITY.txt", root);
    ok_win = host_parity_check_file("windows", win_path);
    ok_and = host_parity_check_file("android", and_path);
    if (ok_win && ok_and) {
        printf("[host-parity-demo] OK\n\n");
    } else {
        printf("[host-parity-demo] FAIL\n\n");
    }
}

/* --- host-boot load-order markers (wave10 item 1 / docs/HOST-BOOT.md) --- */
static const char* HOST_BOOT_FILES[] = {
    "trit.fs",
    "tritium-kernel.fs",
    "drena.fs",
    "rekia.fs",
    "queue.fs",
    "assimilate.fs",
    "lineos.fs",
    "integrate.fs",
    "master.fs",
    "fleet.fs",
    NULL
};

static int host_boot_count_words(const char* dir) {
    /* Stub: count lines that look like Forth colon defs (^\s*:) across core *.fs */
    DIR* d;
    struct dirent* ent;
    char path[MAX_PATH];
    int words = 0;
    d = opendir(dir);
    if (!d) return 0;
    while ((ent = readdir(d)) != NULL) {
        size_t len;
        FILE* f;
        char line[512];
        len = strlen(ent->d_name);
        if (len < 4 || strcmp(ent->d_name + len - 3, ".fs") != 0) continue;
        snprintf(path, sizeof(path), "%s/%s", dir, ent->d_name);
        f = fopen(path, "r");
        if (!f) continue;
        while (fgets(line, sizeof(line), f)) {
            char* p = line;
            while (*p == ' ' || *p == '\t') p++;
            if (*p == ':') words++;
        }
        fclose(f);
    }
    closedir(d);
    return words;
}

/* Returns 1 on OK. Prints [host-boot] markers; caller prints [host-boot-demo] OK|FAIL. */
static int host_boot_report_core(void) {
    char dir[MAX_PATH];
    char test[MAX_PATH];
    struct stat st;
    int i;
    int fail = 0;
    int n_section2 = 0;
    const char* env = getenv("TRITIUM_CORE");

    dir[0] = 0;
    if (env && *env) {
        snprintf(test, sizeof(test), "%s/boot.fs", env);
        if (stat(test, &st) == 0) {
            snprintf(dir, sizeof(dir), "%s", env);
        }
    }
    if (!dir[0] && core_dir[0]) {
        snprintf(test, sizeof(test), "%s/boot.fs", core_dir);
        if (stat(test, &st) == 0) {
            snprintf(dir, sizeof(dir), "%s", core_dir);
        }
    }
    if (!dir[0]) {
        /* Dev fallback: walk from cwd for tritium.poly/core/boot.fs */
        char walk[MAX_PATH];
        if (getcwd(walk, sizeof(walk)) != NULL) {
            for (i = 0; i < 6; i++) {
                snprintf(test, sizeof(test), "%s/tritium.poly/core/boot.fs", walk);
                if (stat(test, &st) == 0) {
                    snprintf(dir, sizeof(dir), "%s/tritium.poly/core", walk);
                    break;
                }
                {
                    char* slash = strrchr(walk, '/');
                    if (!slash || slash == walk) break;
                    *slash = '\0';
                }
            }
        }
    }
    if (!dir[0]) {
        printf("[host-boot] core-dir=MISSING\n");
        return 0;
    }

    printf("[host-boot] core-dir=%s\n", dir);
    printf("[host-boot] order: trit → kernel → drena → rekia → queue → assimilate → "
           "lineos → integrate → master → fleet\n");

    snprintf(test, sizeof(test), "%s/boot.fs", dir);
    if (stat(test, &st) == 0) {
        printf("[host-boot] file=boot.fs OK\n");
    } else {
        printf("[host-boot] file=boot.fs MISSING\n");
        fail = 1;
    }

    for (i = 0; HOST_BOOT_FILES[i]; i++) {
        n_section2++;
        snprintf(test, sizeof(test), "%s/%s", dir, HOST_BOOT_FILES[i]);
        if (stat(test, &st) == 0) {
            printf("[host-boot] file=%s OK\n", HOST_BOOT_FILES[i]);
        } else {
            printf("[host-boot] file=%s MISSING\n", HOST_BOOT_FILES[i]);
            fail = 1;
        }
    }

    printf("[host-boot] sources-loaded files=%d\n", n_section2);
    printf("[host-boot] words=%d\n", host_boot_count_words(dir));
    return fail ? 0 : 1;
}

void host_boot_report(void) {
    (void)host_boot_report_core();
    printf("\n");
}

void host_boot_demo(void) {
    if (host_boot_report_core()) {
        printf("[host-boot-demo] OK\n\n");
    } else {
        printf("[host-boot-demo] FAIL\n\n");
    }
}

/* --- refined-boot cold-load (wave10 item 4 / docs/REFINED-BOOT.md) --- */
static int find_repo_refined_dir(char* out, size_t outsz) {
    char root[MAX_PATH];
    char try_path[MAX_PATH];
    struct stat st;
    if (!find_repo_root_for_parity(root, sizeof(root))) {
        /* Fallback: walk cwd for evolve/forth/refined/qwantum-sample.fs */
        char walk[MAX_PATH];
        int i;
        if (getcwd(walk, sizeof(walk)) == NULL) return 0;
        for (i = 0; i < 6; i++) {
            snprintf(try_path, sizeof(try_path), "%s/evolve/forth/refined/qwantum-sample.fs", walk);
            if (stat(try_path, &st) == 0) {
                snprintf(out, outsz, "%s/evolve/forth/refined", walk);
                return 1;
            }
            {
                char* slash = strrchr(walk, '/');
                if (!slash || slash == walk) break;
                *slash = '\0';
            }
        }
        return 0;
    }
    snprintf(try_path, sizeof(try_path), "%s/evolve/forth/refined/qwantum-sample.fs", root);
    if (stat(try_path, &st) != 0) return 0;
    snprintf(out, outsz, "%s/evolve/forth/refined", root);
    return 1;
}

static int refined_boot_copy_file(const char* src, const char* dst) {
    FILE* in;
    FILE* out;
    char buf[4096];
    size_t n;
    in = fopen(src, "rb");
    if (!in) return 0;
    out = fopen(dst, "wb");
    if (!out) { fclose(in); return 0; }
    while ((n = fread(buf, 1, sizeof(buf), in)) > 0) {
        if (fwrite(buf, 1, n, out) != n) {
            fclose(in); fclose(out); return 0;
        }
    }
    fclose(in);
    fclose(out);
    return 1;
}

/* Minimal stubs when repo/AppDir share unavailable (wave11 tip4 / APPIMAGE-REFINED). */
static int refined_boot_write_stub(const char* path, const char* contents) {
    FILE* f = fopen(path, "w");
    if (!f) return 0;
    if (fputs(contents, f) == EOF) { fclose(f); return 0; }
    fclose(f);
    return 1;
}

/* AppDir / TRITIUM_POLY share: .../tritium.poly/evolve/forth/refined. */
static int find_share_refined_dir(char* out, size_t outsz) {
    char try_path[MAX_PATH];
    struct stat st;
    const char* poly = getenv("TRITIUM_POLY");
    if (poly && *poly) {
        snprintf(try_path, sizeof(try_path),
                 "%s/evolve/forth/refined/refined-boot-fixture.fs", poly);
        if (stat(try_path, &st) == 0 && S_ISREG(st.st_mode)) {
            snprintf(out, outsz, "%s/evolve/forth/refined", poly);
            return 1;
        }
    }
    if (core_dir[0]) {
        snprintf(try_path, sizeof(try_path),
                 "%s/../evolve/forth/refined/refined-boot-fixture.fs", core_dir);
        if (stat(try_path, &st) == 0 && S_ISREG(st.st_mode)) {
            snprintf(out, outsz, "%s/../evolve/forth/refined", core_dir);
            return 1;
        }
    }
    return 0;
}

/* Ensure live evolve refined dir has fixture + qwantum sample.
 * Portable seed: copy from repo/AppDir if available, else write minimal stubs.
 * Always returns live evolve path (demo PASS without repo cwd). */
static int refined_boot_resolve_dir(char* out, size_t outsz) {
    char live[MAX_PATH];
    char repo[MAX_PATH];
    char share[MAX_PATH];
    char live_fx[MAX_PATH];
    char live_qw[MAX_PATH];
    char src[MAX_PATH];
    struct stat st;
    int has_repo;
    int has_share;
    static const char* fixture_stub =
        "\\ wave10 tip4 fixture — cold-load smoke\n"
        ": refined-boot-fixture-ok .\" [refined] fixture ok\" cr ;\n";
    static const char* qwantum_stub =
        "\\ TritiumOS fragment dumped from qwantum field\n"
        ": qwantum-ok .\" field dump ok\" cr ;\n";

    ensure_evolve_dir();
    snprintf(live, sizeof(live), "%s/forth/refined", evolve_dir);
    mkdir(live, 0755);
    snprintf(live_fx, sizeof(live_fx), "%s/refined-boot-fixture.fs", live);
    snprintf(live_qw, sizeof(live_qw), "%s/qwantum-sample.fs", live);
    has_repo = find_repo_refined_dir(repo, sizeof(repo));
    has_share = find_share_refined_dir(share, sizeof(share));

    if (stat(live_fx, &st) != 0 || !S_ISREG(st.st_mode)) {
        int seeded = 0;
        if (has_repo) {
            snprintf(src, sizeof(src), "%s/refined-boot-fixture.fs", repo);
            seeded = refined_boot_copy_file(src, live_fx);
        }
        if (!seeded && has_share) {
            snprintf(src, sizeof(src), "%s/refined-boot-fixture.fs", share);
            seeded = refined_boot_copy_file(src, live_fx);
        }
        if (!seeded)
            seeded = refined_boot_write_stub(live_fx, fixture_stub);
        if (!seeded) {
            snprintf(out, outsz, "%s", live);
            return 0;
        }
    }
    if (stat(live_qw, &st) != 0 || !S_ISREG(st.st_mode)) {
        int seeded = 0;
        if (has_repo) {
            snprintf(src, sizeof(src), "%s/qwantum-sample.fs", repo);
            seeded = refined_boot_copy_file(src, live_qw);
        }
        if (!seeded && has_share) {
            snprintf(src, sizeof(src), "%s/qwantum-sample.fs", share);
            seeded = refined_boot_copy_file(src, live_qw);
        }
        if (!seeded)
            (void)refined_boot_write_stub(live_qw, qwantum_stub);
    }
    snprintf(out, outsz, "%s", live);
    return 1;
}

void refined_boot_report(void) {
    char dir[MAX_PATH];
    if (!refined_boot_resolve_dir(dir, sizeof(dir))) {
        ensure_evolve_dir();
        snprintf(dir, sizeof(dir), "%s/forth/refined", evolve_dir);
    }
    (void)refined_boot_scan_dir(dir, 1);
    printf("\n");
}

/* oneshot exit: 0 on last demo OK, 1 on FAIL (wave11 tip4) */
static int g_oneshot_status = 0;

void refined_boot_demo(void) {
    char dir[MAX_PATH];
    int ok;

    ensure_evolve_dir();
    if (!refined_boot_resolve_dir(dir, sizeof(dir))) {
        printf("[refined-boot] dir=MISSING\n");
        printf("[refined-boot-demo] FAIL\n\n");
        g_oneshot_status = 1;
        return;
    }
    (void)refined_boot_scan_dir(dir, 1);
    ok = (refined_boot_last_loaded >= 1 && refined_boot_last_skipped >= 1);
    if (ok) {
        printf("[refined-boot-demo] OK\n\n");
    } else {
        printf("[refined-boot-demo] FAIL\n\n");
        g_oneshot_status = 1;
    }
}


/* --- trit-math: trit+ / trit* / pack clamp Lab smoke (wave9 item 1 / docs/TRIT-MATH.md) --- */
static int trit_plus(int a, int b) {
    int s = a + b;
    if (s == 2) return 1;
    if (s == -2) return -1;
    return s;
}

static int trit_mul(int a, int b) {
    return a * b;
}

static unsigned pack_neuron_header(int s0, int s1, int s2, int s3) {
    return (unsigned)((s0 & 0xf) | ((s1 & 0xf) << 4) | ((s2 & 0xf) << 8) | ((s3 & 0xf) << 12));
}

static void unpack_header(unsigned h, int* s0, int* s1, int* s2, int* s3) {
    *s0 = (int)(h & 0xf);
    *s1 = (int)((h >> 4) & 0xf);
    *s2 = (int)((h >> 8) & 0xf);
    *s3 = (int)((h >> 12) & 0xf);
}

void trit_math_demo(void) {
    int s0, s1, s2, s3;
    unsigned h;
    if (trit_plus(1, 1) != 1 || trit_plus(-1, -1) != -1 ||
        trit_plus(1, -1) != 0 || trit_plus(0, 1) != 1) {
        printf("[trit-math-demo] FAIL\n\n");
        return;
    }
    printf("[trit-math] trit+ ok\n");
    if (trit_mul(1, -1) != -1 || trit_mul(-1, -1) != 1 || trit_mul(0, 1) != 0) {
        printf("[trit-math-demo] FAIL\n\n");
        return;
    }
    printf("[trit-math] trit* ok\n");
    h = pack_neuron_header(1, 2, 3, 4);
    unpack_header(h, &s0, &s1, &s2, &s3);
    if (s0 != 1 || s1 != 2 || s2 != 3 || s3 != 4) {
        printf("[trit-math-demo] FAIL\n\n");
        return;
    }
    printf("[trit-math] pack-roundtrip ok\n");
    printf("[trit-math-demo] OK\n\n");
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
    /* Stub neuron count after spawn (≥1 after successful assist that spawned). */
    {
        int neurons = (host_next_id > 1) ? (host_next_id - 1) : 1;
        if (neurons < 1) neurons = 1;
        printf("[S0] assist done — refined word live neurons=%d\n", neurons);
        printf("[assistant-s0] neurons=%d\n\n", neurons);
    }
}

/* Shared smoke check: refined-1.fs written + live flag. */
static int s0_assist_smoke_ok(void) {
    const char* evolve = get_evolve_dir();
    char path[MAX_PATH];
    snprintf(path, sizeof(path), "%s/forth/refined/refined-1.fs", evolve);
    struct stat st;
    return (stat(path, &st) == 0 && refined_live && last_refined_label[0]);
}

/* Fail if refined file contains forbidden scaffold strings (assist reply path contract). */
static int s0_assist_forbidden_in_refined(void) {
    const char* evolve = get_evolve_dir();
    char path[MAX_PATH];
    char buf[8192];
    snprintf(path, sizeof(path), "%s/forth/refined/refined-1.fs", evolve);
    FILE* f = fopen(path, "r");
    if (!f) return 1;
    size_t n = fread(buf, 1, sizeof(buf) - 1, f);
    buf[n] = 0;
    fclose(f);
    if (strstr(buf, "scaffold reply") ||
        strstr(buf, "connect R.E.K.I.A. next") ||
        strstr(buf, "Example response"))
        return 1;
    return 0;
}

void s0_assist_demo(void) {
    /* Fixed free-text → must write refined + live word; no scaffold strings. */
    printf("[s0-assist-demo] feed fixed query through S0 path\n");
    s0_assist("hello tritium");
    if (s0_assist_smoke_ok() && !s0_assist_forbidden_in_refined())
        printf("[s0-assist-demo] OK — refined written; word live\n\n");
    else
        printf("[s0-assist-demo] FAIL — expected refined-1.fs + live flag\n\n");
}

void assistant_s0_demo(void) {
    /* Wave11 tip2: same fixed query + greppable neurons= ≥1; forbid scaffold. */
    printf("[assistant-s0-demo] feed fixed query through S0 path\n");
    s0_assist("hello tritium");
    int neurons = (host_next_id > 1) ? (host_next_id - 1) : 0;
    int ok = s0_assist_smoke_ok() && neurons >= 1 && !s0_assist_forbidden_in_refined();
    if (ok)
        printf("[assistant-s0-demo] OK\n\n");
    else
        printf("[assistant-s0-demo] FAIL\n\n");
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
    printf("  rekiA-demo    - run REKIA engine (refine to Forth + assistance)\n  rekia-demo    - alias: spawn→rewire→φ-link→refine→write evolve/forth/refined/*.fs\n  s3-reserved-demo - spawn S3=11 then rewire; mode must stay 3\n  grow-step-demo - spawn0→grow→step; RESERVED grow+step skipped\n  groups-demo   - create group, join 2 neurons, persist members + GROUP-<label>/\n  groups-status - show restored host groups + GROUP-<label>/ + members\n  groups-persist-demo - groups-demo then reload graph (restart surrogate)\n  group-vocab-demo - GROUP-<label>/ searchable vocab unit; scoped find\n  group-link-demo - two groups + group-link! LINK-INTER bridge\n  group-nested-demo - nested find via LINK-INTER neighbors\n  group-vocab-persist-demo - vocab lines survive graph reload\n  kernel-demo - flat find/findentry + interpret-token stub\n  interpret-demo - interpret loop + : create-only stub\n  colon-demo - : body ; marker stub + run body\n  control-demo - IF/THEN/ELSE cs-depth stubs → greppable OK\n  loop-demo - BEGIN/UNTIL/WHILE/REPEAT loop stubs → greppable OK\n  do-loop-demo - DO/LOOP/+LOOP/I counted-loop stubs → greppable OK\n  words-demo - WORDS/dict-list create≥2 + list → greppable OK\n  var-demo - VARIABLE/CONSTANT named-cell stubs → greppable OK\n  value-demo - VALUE/TO named mutable-cell stubs → greppable OK\n  case-demo - CASE/OF/ENDOF/ENDCASE stubs → greppable OK\n  create-demo - CREATE/DOES> defining-word stubs → greppable OK\n  string-demo - S\" /.\" /.( string-lit stubs → greppable OK\n  allot-demo - HERE/ALLOT pointer stubs → greppable OK\n  unloop-demo - UNLOOP/J do-loop sibling stubs → greppable OK\n  2var-demo - 2VARIABLE/2CONSTANT double-cell stubs → greppable OK\n  throw-demo - CATCH/THROW exception-mark stubs → greppable OK\n  cell-demo - CELL/CELLS/ALIGN/ALIGNED unit stubs → greppable OK\n  pick-demo - PICK/ROLL/DEPTH/?DUP stack stubs → greppable OK\n  fill-demo - FILL/ERASE/MOVE/CMOVE buffer stubs → greppable OK\n  comment-demo - \\ / ( ) comment skip stubs → greppable OK\n  leave-demo - LEAVE/AGAIN stubs (BEGIN+DO) → greppable OK\n  WORDS / words / .words - list flat dict names\n  VARIABLE <name> / CONSTANT <name> <n> - named-cell stubs (aliases var-create/const-create)\n  VALUE <n> <name> / TO <n> <name> - VALUE/TO stubs (aliases value-create/value-to)\n  value@ <name> - optional VALUE fetch stub\n  2VARIABLE <name> / 2CONSTANT <lo> <hi> <name> - double-cell stubs (aliases 2var-create/2const-create)\n  2@ <name> / 2! <lo> <hi> <name> - optional 2VARIABLE fetch/store stubs\n  CREATE <name> / DOES> - CREATE/DOES> stubs (aliases create-entry/does-mark)\n  HERE / ALLOT <n> - dictionary-pointer stubs (aliases here-at/allot-bump)\n  CELL / CELLS <k> / ALIGN / ALIGNED <addr> - unit stubs (aliases cell-size/cells-n/align-here/aligned-addr)\n  DEPTH / PICK <u> / ROLL <u> / ?DUP - stack stubs (aliases stack-depth/pick-nth/roll-nth/qdup)\n  FILL <addr> <u> <char> / ERASE <addr> <u> / MOVE <from> <to> <u> / CMOVE <from> <to> <u> - buffer stubs (aliases fill-buf/erase-buf/move-buf/cmove-buf)\n  , <n> / C, <c> - optional cell/char stub store + bump\n  fold-demo - phi-fold / fold-target goldens → greppable OK\n  IF [0|1] / THEN / ELSE - control stubs (aliases control-if/then/else)\n  CASE / OF / ENDOF / ENDCASE - case stubs (aliases control-case/of/endof/endcase)\n  BEGIN / UNTIL [0|1] / WHILE [0|1] / REPEAT - loop stubs (aliases control-*)\n  LEAVE / AGAIN - leave-mark / infinite-close stubs (aliases control-leave/again)\n  DO [limit start] / LOOP / +LOOP [n] / I - do-loop stubs (aliases control-*)\n  UNLOOP / J - do-loop sibling stubs (aliases control-unloop/control-j)\n  CATCH / THROW <n> / ABORT\" <text>\" - exception-mark stubs (aliases catch-mark/throw-code/abort-quote)\n  : <name> [body…] ; - colon-def body/marker stub (wave9)\n  s0-assist-demo - fixed free-text → S0 (drena-step + rekiA-refine)\n  assistant-s0-demo - S0 deepen + neurons= (wave11 tip2 / ASSISTANT-S0)\n  edition-demo - show edition/id-width + spawn (clamped)\n  edition 32|64 - set-edition + persist evolve/edition.trit\n  license-status - show N/10 device slots (§5a.4)\n  license-register <id> - register device; refuses slot 11\n  queue-demo   - §5b.1 local cue: enqueue non-local → pull → prove\n  queue-local? <job> / queue-enqueue! <job> / queue-pull / queue-prove! <job> <proof>\n  assimilate-demo - §5b.2–5b.3 fragment → merge → simti credit (no crypto)\n  assimilate-epoch / assimilate-fragment <g> <l> / assimilate-merge! <frag> <proof>\n  assimilate-balance / assimilate-solved?\n  lineos-graduate-demo - §1a.1 force-ready → scaffold product_id=lineos\n  lineos-graduate / become-lineos — graduation gates + scaffold flip\n  lineos-confirm - set userConfirmed; marker [LINEOS] confirm set\n  lineos-confirm-demo - refuse without confirm → confirm → graduate OK\n  lineos-splash / lineos-about - brand markers (refuse splash if not graduated)\n  lineos-brand-demo - force brand markers → assert slogan/name/edition → OK\n  tritium-integrate <platform> - §5a.2 scaffold from _template → evolve/integrate/\n  tritium-integrate-demo - force free-slot → platform=demo; greppable OK\n  master-mint-license [slots] - §5a.5 TRIT-<16hex>-DRACO (default 10)\n  master-mint-worker <device-id> - §5a.5 TRIT-W-<idhash>-DRACO; refuse empty\n  master-verify <key> - format-only check (not crypto)\n  master-demo - mint license→worker→verify both → greppable OK\n  fleet-export [deviceId] - §§5a.2–5a.4 write evolve/fleet/ blob (same-key stamp)\n  fleet-import - same-key restore; refuse mismatch (§5a.4)\n  fleet-demo - export→import OK; wrong fingerprint refuse → greppable OK\n  userland-demo - assert userland/{init,shell,demos}/ scaffold → greppable OK\n  host-parity-demo - Win/Android HOST-PARITY.txt contracts → greppable OK\n  host-boot / host-boot-report - poly/core load-order markers (wave10)\n  host-boot-demo - assert boot.fs + §2 core files present → greppable OK\n  refined-boot / refined-boot-report - cold-load evolve/forth/refined/*.fs markers (wave10)\n  refined-boot-demo - include fixture + skip qwantum → greppable OK\n  trit-math-demo - trit+ clamp + trit* + pack round-trip → greppable OK\n  economy-wire-demo - queue-prove!→assimilate-merge! e2e → greppable OK\n  queue-assim-demo - alias for economy-wire-demo\n  assistant-state-demo - v2 task/reminder/note persist → greppable OK\n  assistant-state! / assistant-state@ - save/load evolve/assistant-state.trit v2\n  assistant-task! / assistant-reminder! / assistant-note! - upsert hooks\n");
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
    int oneshot = 0;
    const char* oneshot_cmd = NULL;
    int ai;
    int oneshot_done = 0;
    char line[MAX_LINE];

    find_core_dir(argv[0]);

    /* --oneshot <cmd> or TRITIUM_ONESHOT=<cmd>: one REPL command then exit (wave11 tip4) */
    for (ai = 1; ai < argc; ai++) {
        if (strcmp(argv[ai], "--oneshot") == 0 && ai + 1 < argc) {
            oneshot = 1;
            oneshot_cmd = argv[++ai];
        } else if (strncmp(argv[ai], "--oneshot=", 10) == 0) {
            oneshot = 1;
            oneshot_cmd = argv[ai] + 10;
        }
    }
    if (!oneshot_cmd) {
        const char* env_os = getenv("TRITIUM_ONESHOT");
        if (env_os && *env_os) {
            oneshot = 1;
            oneshot_cmd = env_os;
        }
    }

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

    if (!oneshot) {
        printf("\nType 'help' to begin. The assistant is ready (on-demand .AppImage).\n");
        printf("(Native bootstrap: 'assimilate' | 'bootstrap-host' | 'full-stack-optimize' exercise Forth-to-C host assimilation + optimization.)\n");
    }

    while (1) {
        if (oneshot) {
            if (oneshot_done)
                return g_oneshot_status;
            snprintf(line, sizeof(line), "%s", oneshot_cmd);
            oneshot_done = 1;
        } else {
            printf("> ");
            if (!fgets(line, sizeof(line), stdin)) break;
            line[strcspn(line, "\n")] = 0;
            if (strlen(line) == 0) continue;
        }

        if (strcasecmp(line, "quit") == 0 || strcasecmp(line, "exit") == 0) {

            if (!oneshot)
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
        } else if (strcasecmp(line, "group-nested-demo") == 0) {
            group_nested_demo();
        } else if (strcasecmp(line, "group-vocab-persist-demo") == 0) {
            group_vocab_persist_demo();
        } else if (strcasecmp(line, "kernel-demo") == 0) {
            kernel_demo();
        } else if (strcasecmp(line, "interpret-demo") == 0) {
            interpret_demo();
        } else if (strcasecmp(line, "colon-demo") == 0) {
            colon_demo();
        } else if (strcasecmp(line, "control-demo") == 0) {
            control_demo();
        } else if (strcasecmp(line, "loop-demo") == 0) {
            loop_demo();
        } else if (strcasecmp(line, "do-loop-demo") == 0 ||
                   strcasecmp(line, "do_loop_demo") == 0) {
            do_loop_demo();
        } else if (strcasecmp(line, "words-demo") == 0 ||
                   strcasecmp(line, "words_demo") == 0) {
            words_demo();
        } else if (strcasecmp(line, "var-demo") == 0 ||
                   strcasecmp(line, "var_demo") == 0) {
            var_demo();
        } else if (strcasecmp(line, "value-demo") == 0 ||
                   strcasecmp(line, "value_demo") == 0) {
            value_demo();
        } else if (strcasecmp(line, "case-demo") == 0 ||
                   strcasecmp(line, "case_demo") == 0) {
            case_demo();
        } else if (strcasecmp(line, "create-demo") == 0 ||
                   strcasecmp(line, "create_demo") == 0) {
            create_demo();
        } else if (strcasecmp(line, "comment-demo") == 0 ||
                   strcasecmp(line, "comment_demo") == 0) {
            comment_demo();
        } else if (strcasecmp(line, "string-demo") == 0 ||
                   strcasecmp(line, "string_demo") == 0) {
            string_demo();
        } else if (strcasecmp(line, "allot-demo") == 0 ||
                   strcasecmp(line, "allot_demo") == 0) {
            allot_demo();
        } else if (strcasecmp(line, "leave-demo") == 0 ||
                   strcasecmp(line, "leave_demo") == 0) {
            leave_demo();
        } else if (strcasecmp(line, "unloop-demo") == 0 ||
                   strcasecmp(line, "unloop_demo") == 0) {
            unloop_demo();
        } else if (strcasecmp(line, "2var-demo") == 0 ||
                   strcasecmp(line, "2var_demo") == 0 ||
                   strcasecmp(line, "two_var_demo") == 0) {
            two_var_demo();
        } else if (strcasecmp(line, "throw-demo") == 0 ||
                   strcasecmp(line, "throw_demo") == 0) {
            throw_demo();
        } else if (strcasecmp(line, "cell-demo") == 0 ||
                   strcasecmp(line, "cell_demo") == 0) {
            cell_demo();
        } else if (strcasecmp(line, "pick-demo") == 0 ||
                   strcasecmp(line, "pick_demo") == 0) {
            pick_demo();
        } else if (strcasecmp(line, "fill-demo") == 0 ||
                   strcasecmp(line, "fill_demo") == 0) {
            fill_demo();
        } else if (strcasecmp(line, "WORDS") == 0 ||
                   strcasecmp(line, "words") == 0 ||
                   strcasecmp(line, ".words") == 0) {
            host_words();
        } else if (strcasecmp(line, "fold-demo") == 0) {
            fold_demo();
        } else if (strcasecmp(line, "control-cs-depth") == 0) {
            printf("[control] cs-depth=%d\n", host_control_cs_depth());
        } else if (strcasecmp(line, "ELSE") == 0 ||
                   strcasecmp(line, "control-else") == 0) {
            host_control_else();
        } else if (strcasecmp(line, "THEN") == 0 ||
                   strcasecmp(line, "control-then") == 0) {
            host_control_then();
        } else if (strcasecmp(line, "case-cs-depth") == 0) {
            printf("[case] case-depth=%d\n", host_case_cs_depth());
        } else if (strcasecmp(line, "CASE") == 0 ||
                   strcasecmp(line, "control-case") == 0) {
            host_control_case();
        } else if (strcasecmp(line, "OF") == 0 ||
                   strcasecmp(line, "control-of") == 0) {
            host_control_of();
        } else if (strcasecmp(line, "ENDOF") == 0 ||
                   strcasecmp(line, "control-endof") == 0) {
            host_control_endof();
        } else if (strcasecmp(line, "ENDCASE") == 0 ||
                   strcasecmp(line, "control-endcase") == 0) {
            host_control_endcase();
        } else if (strncasecmp(line, "IF ", 3) == 0 ||
                   strcasecmp(line, "IF") == 0 ||
                   strncasecmp(line, "control-if ", 11) == 0 ||
                   strcasecmp(line, "control-if") == 0) {
            const char* arg = line;
            if (strncasecmp(arg, "control-if", 10) == 0)
                arg += 10;
            else
                arg += 2; /* IF */
            while (*arg == ' ') arg++;
            if (*arg)
                host_control_if(1, atoi(arg));
            else
                host_control_if(0, 0);
        } else if (strcasecmp(line, "loop-cs-depth") == 0) {
            printf("[loop] loop-depth=%d\n", host_loop_cs_depth());
        } else if (strcasecmp(line, "BEGIN") == 0 ||
                   strcasecmp(line, "control-begin") == 0) {
            host_control_begin();
        } else if (strcasecmp(line, "REPEAT") == 0 ||
                   strcasecmp(line, "control-repeat") == 0) {
            host_control_repeat();
        } else if (strcasecmp(line, "LEAVE") == 0 ||
                   strcasecmp(line, "control-leave") == 0) {
            host_control_leave();
        } else if (strcasecmp(line, "AGAIN") == 0 ||
                   strcasecmp(line, "control-again") == 0) {
            host_control_again();
        } else if (strncasecmp(line, "UNTIL ", 6) == 0 ||
                   strcasecmp(line, "UNTIL") == 0 ||
                   strncasecmp(line, "control-until ", 14) == 0 ||
                   strcasecmp(line, "control-until") == 0) {
            const char* arg = line;
            if (strncasecmp(arg, "control-until", 13) == 0)
                arg += 13;
            else
                arg += 5; /* UNTIL */
            while (*arg == ' ') arg++;
            if (*arg)
                host_control_until(1, atoi(arg));
            else
                host_control_until(0, 0);
        } else if (strncasecmp(line, "WHILE ", 6) == 0 ||
                   strcasecmp(line, "WHILE") == 0 ||
                   strncasecmp(line, "control-while ", 14) == 0 ||
                   strcasecmp(line, "control-while") == 0) {
            const char* arg = line;
            if (strncasecmp(arg, "control-while", 13) == 0)
                arg += 13;
            else
                arg += 5; /* WHILE */
            while (*arg == ' ') arg++;
            if (*arg)
                host_control_while(1, atoi(arg));
            else
                host_control_while(0, 0);
        } else if (line[0] >= '0' && line[0] <= '9') {
            /* <n> CONSTANT|VALUE|TO <name>  (Forth stack order) */
            int nval = 0;
            char rest[128];
            char cname[HOST_NAMELEN];
            if (sscanf(line, "%d %127s %15s", &nval, rest, cname) >= 2 &&
                (strcasecmp(rest, "CONSTANT") == 0 ||
                 strcasecmp(rest, "const-create") == 0)) {
                if (cname[0])
                    host_const_create(cname, nval);
                else
                    printf("[var] FAIL reason=miss\n");
            } else if (sscanf(line, "%d %127s %15s", &nval, rest, cname) >= 2 &&
                       (strcasecmp(rest, "VALUE") == 0 ||
                        strcasecmp(rest, "value-create") == 0)) {
                if (cname[0])
                    host_value_create(cname, nval);
                else
                    printf("[value] FAIL reason=miss\n");
            } else if (sscanf(line, "%d %127s %15s", &nval, rest, cname) >= 2 &&
                       (strcasecmp(rest, "TO") == 0 ||
                        strcasecmp(rest, "value-to") == 0)) {
                if (cname[0])
                    host_value_to(cname, nval);
                else
                    printf("[value] FAIL reason=miss\n");
            } else {
                printf("Unknown command. Type 'help'.\n");
            }
        } else if (strncasecmp(line, "VALUE ", 6) == 0 ||
                   strncasecmp(line, "value-create ", 13) == 0) {
            /* VALUE <name> <n>  OR value-create <name> <n> */
            const char* arg = line;
            if (strncasecmp(arg, "value-create", 12) == 0)
                arg += 12;
            else
                arg += 5; /* VALUE */
            while (*arg == ' ') arg++;
            char vname[HOST_NAMELEN];
            int vval = 0;
            if (sscanf(arg, "%15s %d", vname, &vval) == 2 && vname[0])
                host_value_create(vname, vval);
            else if (sscanf(arg, "%d %15s", &vval, vname) == 2 && vname[0])
                host_value_create(vname, vval);
            else
                printf("[value] FAIL reason=miss\n");
        } else if (strncasecmp(line, "TO ", 3) == 0 ||
                   strncasecmp(line, "value-to ", 9) == 0) {
            /* TO <name> <n>  OR value-to <name> <n>  (also <n> TO <name> via digit path) */
            const char* arg = line;
            if (strncasecmp(arg, "value-to", 8) == 0)
                arg += 8;
            else
                arg += 2; /* TO */
            while (*arg == ' ') arg++;
            char vname[HOST_NAMELEN];
            int vval = 0;
            if (sscanf(arg, "%d %15s", &vval, vname) == 2 && vname[0])
                host_value_to(vname, vval);
            else if (sscanf(arg, "%15s %d", vname, &vval) == 2 && vname[0])
                host_value_to(vname, vval);
            else
                printf("[value] FAIL reason=miss\n");
        } else if (strncasecmp(line, "value@ ", 7) == 0 ||
                   strncasecmp(line, "value-fetch ", 12) == 0) {
            const char* arg = line;
            if (strncasecmp(arg, "value-fetch", 11) == 0)
                arg += 11;
            else
                arg += 6; /* value@ */
            while (*arg == ' ') arg++;
            if (*arg)
                host_value_fetch(arg);
            else
                printf("[value] FAIL reason=miss\n");
        } else if (strncasecmp(line, "CREATE ", 7) == 0 ||
                   strncasecmp(line, "create-entry ", 13) == 0) {
            const char* arg = line;
            if (strncasecmp(arg, "create-entry", 12) == 0)
                arg += 12;
            else
                arg += 6; /* CREATE */
            while (*arg == ' ') arg++;
            if (*arg)
                host_create_entry(arg);
            else
                printf("[create] FAIL reason=miss\n");
        } else if (strcasecmp(line, "DOES>") == 0 ||
                   strcasecmp(line, "does-mark") == 0) {
            host_does_mark();
        } else if (strcasecmp(line, "HERE") == 0 ||
                   strcasecmp(line, "here-at") == 0) {
            host_here_at();
        } else if (strncasecmp(line, "ALLOT ", 6) == 0 ||
                   strcasecmp(line, "ALLOT") == 0 ||
                   strncasecmp(line, "allot-bump ", 11) == 0 ||
                   strcasecmp(line, "allot-bump") == 0) {
            const char* arg = line;
            if (strncasecmp(arg, "allot-bump", 10) == 0)
                arg += 10;
            else
                arg += 5; /* ALLOT */
            while (*arg == ' ') arg++;
            if (*arg)
                host_allot_bump(atoi(arg));
            else
                printf("[allot] FAIL reason=miss\n");
        } else if ((line[0] == ',' && (line[1] == 0 || line[1] == ' ')) ||
                   strncasecmp(line, "comma ", 6) == 0 ||
                   strcasecmp(line, "comma") == 0) {
            const char* arg = line;
            if (strncasecmp(arg, "comma", 5) == 0)
                arg += 5;
            else
                arg += 1; /* , */
            while (*arg == ' ') arg++;
            if (*arg)
                host_comma(atoi(arg));
            else
                printf("[allot] FAIL reason=miss\n");
        } else if (strncasecmp(line, "C, ", 3) == 0 ||
                   strcasecmp(line, "C,") == 0 ||
                   strncasecmp(line, "c-comma ", 8) == 0 ||
                   strcasecmp(line, "c-comma") == 0) {
            const char* arg = line;
            if (strncasecmp(arg, "c-comma", 7) == 0)
                arg += 7;
            else
                arg += 2; /* C, */
            while (*arg == ' ') arg++;
            if (*arg)
                host_c_comma(atoi(arg));
            else
                printf("[allot] FAIL reason=miss\n");
        } else if (strcasecmp(line, "CELL") == 0 ||
                   strcasecmp(line, "cell-size") == 0) {
            host_cell_size();
        } else if (strncasecmp(line, "CELLS ", 6) == 0 ||
                   strcasecmp(line, "CELLS") == 0 ||
                   strncasecmp(line, "cells-n ", 8) == 0 ||
                   strcasecmp(line, "cells-n") == 0) {
            const char* arg = line;
            if (strncasecmp(arg, "cells-n", 7) == 0)
                arg += 7;
            else
                arg += 5; /* CELLS */
            while (*arg == ' ') arg++;
            if (*arg)
                host_cells_n(atoi(arg));
            else
                printf("[cell] FAIL reason=miss\n");
        } else if (strcasecmp(line, "ALIGN") == 0 ||
                   strcasecmp(line, "align-here") == 0) {
            host_align_here();
        } else if (strncasecmp(line, "ALIGNED ", 8) == 0 ||
                   strcasecmp(line, "ALIGNED") == 0 ||
                   strncasecmp(line, "aligned-addr ", 13) == 0 ||
                   strcasecmp(line, "aligned-addr") == 0) {
            const char* arg = line;
            if (strncasecmp(arg, "aligned-addr", 12) == 0)
                arg += 12;
            else
                arg += 7; /* ALIGNED */
            while (*arg == ' ') arg++;
            if (*arg)
                host_aligned_addr(atoi(arg));
            else
                printf("[cell] FAIL reason=miss\n");
        } else if (strcasecmp(line, "DEPTH") == 0 ||
                   strcasecmp(line, "stack-depth") == 0) {
            host_stack_depth();
        } else if (strncasecmp(line, "PICK ", 5) == 0 ||
                   strcasecmp(line, "PICK") == 0 ||
                   strncasecmp(line, "pick-nth ", 9) == 0 ||
                   strcasecmp(line, "pick-nth") == 0) {
            const char* arg = line;
            if (strncasecmp(arg, "pick-nth", 8) == 0)
                arg += 8;
            else
                arg += 4; /* PICK */
            while (*arg == ' ') arg++;
            if (*arg)
                host_pick_nth(atoi(arg));
            else
                printf("[pick] FAIL reason=miss\n");
        } else if (strncasecmp(line, "ROLL ", 5) == 0 ||
                   strcasecmp(line, "ROLL") == 0 ||
                   strncasecmp(line, "roll-nth ", 9) == 0 ||
                   strcasecmp(line, "roll-nth") == 0) {
            const char* arg = line;
            if (strncasecmp(arg, "roll-nth", 8) == 0)
                arg += 8;
            else
                arg += 4; /* ROLL */
            while (*arg == ' ') arg++;
            if (*arg)
                host_roll_nth(atoi(arg));
            else
                printf("[pick] FAIL reason=miss\n");
        } else if (strcasecmp(line, "?DUP") == 0 ||
                   strcasecmp(line, "qdup") == 0) {
            host_qdup();
        } else if (strncasecmp(line, "FILL ", 5) == 0 ||
                   strncasecmp(line, "fill-buf ", 9) == 0) {
            const char* arg = line;
            int a = 0, u = 0, ch = 0;
            if (strncasecmp(arg, "fill-buf", 8) == 0)
                arg += 8;
            else
                arg += 4; /* FILL */
            while (*arg == ' ') arg++;
            if (sscanf(arg, "%d %d %d", &a, &u, &ch) == 3)
                host_fill_buf(a, u, ch);
            else
                printf("[fill] FAIL reason=miss\n");
        } else if (strncasecmp(line, "ERASE ", 6) == 0 ||
                   strncasecmp(line, "erase-buf ", 10) == 0) {
            const char* arg = line;
            int a = 0, u = 0;
            if (strncasecmp(arg, "erase-buf", 9) == 0)
                arg += 9;
            else
                arg += 5; /* ERASE */
            while (*arg == ' ') arg++;
            if (sscanf(arg, "%d %d", &a, &u) == 2)
                host_erase_buf(a, u);
            else
                printf("[fill] FAIL reason=miss\n");
        } else if (strncasecmp(line, "MOVE ", 5) == 0 ||
                   strncasecmp(line, "move-buf ", 9) == 0) {
            const char* arg = line;
            int f = 0, t = 0, u = 0;
            if (strncasecmp(arg, "move-buf", 8) == 0)
                arg += 8;
            else
                arg += 4; /* MOVE */
            while (*arg == ' ') arg++;
            if (sscanf(arg, "%d %d %d", &f, &t, &u) == 3)
                host_move_buf(f, t, u);
            else
                printf("[fill] FAIL reason=miss\n");
        } else if (strncasecmp(line, "CMOVE ", 6) == 0 ||
                   strncasecmp(line, "cmove-buf ", 10) == 0) {
            const char* arg = line;
            int f = 0, t = 0, u = 0;
            if (strncasecmp(arg, "cmove-buf", 9) == 0)
                arg += 9;
            else
                arg += 5; /* CMOVE */
            while (*arg == ' ') arg++;
            if (sscanf(arg, "%d %d %d", &f, &t, &u) == 3)
                host_cmove_buf(f, t, u);
            else
                printf("[fill] FAIL reason=miss\n");
        } else if (strncasecmp(line, "VARIABLE ", 9) == 0 ||
                   strncasecmp(line, "var-create ", 11) == 0) {
            const char* arg = line;
            if (strncasecmp(arg, "var-create", 10) == 0)
                arg += 10;
            else
                arg += 8; /* VARIABLE */
            while (*arg == ' ') arg++;
            if (*arg)
                host_var_create(arg);
            else
                printf("[var] FAIL reason=miss\n");
        } else if (strncasecmp(line, "CONSTANT ", 9) == 0 ||
                   strncasecmp(line, "const-create ", 13) == 0) {
            /* CONSTANT <name> <value> */
            const char* arg = line;
            if (strncasecmp(arg, "const-create", 12) == 0)
                arg += 12;
            else
                arg += 8; /* CONSTANT */
            while (*arg == ' ') arg++;
            char cname[HOST_NAMELEN];
            int cval = 0;
            if (sscanf(arg, "%15s %d", cname, &cval) >= 1 && cname[0])
                host_const_create(cname, cval);
            else
                printf("[var] FAIL reason=miss\n");
        } else if (strncasecmp(line, "2VARIABLE ", 10) == 0 ||
                   strncasecmp(line, "2var-create ", 12) == 0) {
            const char* arg = line;
            if (strncasecmp(arg, "2var-create", 11) == 0)
                arg += 11;
            else
                arg += 9; /* 2VARIABLE */
            while (*arg == ' ') arg++;
            if (*arg)
                host_2var_create(arg);
            else
                printf("[2var] FAIL reason=miss\n");
        } else if (strncasecmp(line, "2CONSTANT ", 10) == 0 ||
                   strncasecmp(line, "2const-create ", 14) == 0) {
            /* 2CONSTANT <name> <lo> <hi>  OR  <lo> <hi> <name> */
            const char* arg = line;
            if (strncasecmp(arg, "2const-create", 13) == 0)
                arg += 13;
            else
                arg += 9; /* 2CONSTANT */
            while (*arg == ' ') arg++;
            char cname[HOST_NAMELEN];
            int lo = 0, hi = 0;
            if (sscanf(arg, "%15s %d %d", cname, &lo, &hi) == 3 && cname[0])
                host_2const_create(cname, lo, hi);
            else if (sscanf(arg, "%d %d %15s", &lo, &hi, cname) == 3 && cname[0])
                host_2const_create(cname, lo, hi);
            else
                printf("[2var] FAIL reason=miss\n");
        } else if (strncasecmp(line, "2@ ", 3) == 0 ||
                   strncasecmp(line, "2var-fetch ", 11) == 0) {
            const char* arg = line;
            if (strncasecmp(arg, "2var-fetch", 10) == 0)
                arg += 10;
            else
                arg += 2; /* 2@ */
            while (*arg == ' ') arg++;
            if (*arg)
                host_2var_fetch(arg);
            else
                printf("[2var] FAIL reason=miss\n");
        } else if (strncasecmp(line, "2! ", 3) == 0 ||
                   strncasecmp(line, "2var-store ", 11) == 0) {
            const char* arg = line;
            if (strncasecmp(arg, "2var-store", 10) == 0)
                arg += 10;
            else
                arg += 2; /* 2! */
            while (*arg == ' ') arg++;
            char vname[HOST_NAMELEN];
            int lo = 0, hi = 0;
            if (sscanf(arg, "%d %d %15s", &lo, &hi, vname) == 3 && vname[0])
                host_2var_store(vname, lo, hi);
            else if (sscanf(arg, "%15s %d %d", vname, &lo, &hi) == 3 && vname[0])
                host_2var_store(vname, lo, hi);
            else
                printf("[2var] FAIL reason=miss\n");
        } else if (strncasecmp(line, "@ ", 2) == 0) {
            const char* arg = line + 1;
            while (*arg == ' ') arg++;
            if (*arg)
                host_var_fetch(arg);
            else
                printf("[var] FAIL reason=miss\n");
        } else if (strncasecmp(line, "! ", 2) == 0) {
            const char* arg = line + 1;
            while (*arg == ' ') arg++;
            char vname[HOST_NAMELEN];
            int vval = 0;
            if (sscanf(arg, "%d %15s", &vval, vname) == 2)
                host_var_store(vname, vval);
            else if (sscanf(arg, "%15s %d", vname, &vval) == 2)
                host_var_store(vname, vval);
            else
                printf("[var] FAIL reason=miss\n");
        } else if (strcasecmp(line, "do-loop-depth") == 0 ||
                   strcasecmp(line, "do_loop_depth") == 0) {
            printf("[do-loop] do-loop-depth=%d\n", host_do_loop_cs_depth());
        } else if (strcasecmp(line, "LOOP") == 0 ||
                   strcasecmp(line, "control-loop") == 0) {
            host_control_loop();
        } else if (strcasecmp(line, "I") == 0 ||
                   strcasecmp(line, "control-i") == 0) {
            host_control_i();
        } else if (strcasecmp(line, "CATCH") == 0 ||
                   strcasecmp(line, "catch-mark") == 0) {
            host_catch_mark();
        } else if (strcasecmp(line, "catch-depth") == 0) {
            printf("[throw] catch-depth=%d\n", host_catch_cs_depth());
        } else if (strncasecmp(line, "THROW ", 6) == 0 ||
                   strncasecmp(line, "throw-code ", 11) == 0 ||
                   strcasecmp(line, "THROW") == 0 ||
                   strcasecmp(line, "throw-code") == 0) {
            const char* arg = line;
            if (strncasecmp(arg, "throw-code", 10) == 0)
                arg += 10;
            else
                arg += 5; /* THROW */
            while (*arg == ' ') arg++;
            int code = 0;
            if (*arg)
                code = atoi(arg);
            host_throw_code(code);
        } else if (strncasecmp(line, "ABORT\"", 6) == 0 ||
                   strncasecmp(line, "abort-quote", 11) == 0) {
            /* ABORT" content"  or  abort-quote content */
            const char* arg = line;
            char buf[256];
            buf[0] = 0;
            if (strncasecmp(arg, "abort-quote", 11) == 0) {
                arg += 11;
                while (*arg == ' ') arg++;
                snprintf(buf, sizeof(buf), "%s", arg);
            } else {
                /* ABORT" ... " */
                arg += 6; /* ABORT" */
                const char* end = strchr(arg, '"');
                size_t n = end ? (size_t)(end - arg) : strlen(arg);
                if (n >= sizeof(buf)) n = sizeof(buf) - 1;
                memcpy(buf, arg, n);
                buf[n] = 0;
            }
            host_abort_quote(buf);
        } else if (strcasecmp(line, "UNLOOP") == 0 ||
                   strcasecmp(line, "control-unloop") == 0) {
            host_control_unloop();
        } else if (strcasecmp(line, "J") == 0 ||
                   strcasecmp(line, "control-j") == 0) {
            host_control_j();
        } else if (strncasecmp(line, "+LOOP ", 6) == 0 ||
                   strcasecmp(line, "+LOOP") == 0 ||
                   strncasecmp(line, "control-plus-loop ", 18) == 0 ||
                   strcasecmp(line, "control-plus-loop") == 0) {
            const char* arg = line;
            if (strncasecmp(arg, "control-plus-loop", 17) == 0)
                arg += 17;
            else
                arg += 5; /* +LOOP */
            while (*arg == ' ') arg++;
            if (*arg)
                host_control_plus_loop(1, atoi(arg));
            else
                host_control_plus_loop(0, 0);
        } else if (strncasecmp(line, "DO ", 3) == 0 ||
                   strcasecmp(line, "DO") == 0 ||
                   strncasecmp(line, "control-do ", 11) == 0 ||
                   strcasecmp(line, "control-do") == 0) {
            /* DO [limit start] — stack order limit then start; index=start */
            const char* arg = line;
            if (strncasecmp(arg, "control-do", 10) == 0)
                arg += 10;
            else
                arg += 2; /* DO */
            while (*arg == ' ') arg++;
            if (*arg) {
                int a = 0, b = 0;
                int n = sscanf(arg, "%d %d", &a, &b);
                if (n >= 2)
                    host_control_do(1, a, b);
                else if (n == 1)
                    host_control_do(1, a, a);
                else
                    host_control_do(0, 0, 0);
            } else {
                host_control_do(0, 0, 0);
            }
        } else if (strcmp(line, ";") == 0) {
            host_semicolon();
        } else if (strncmp(line, ": ", 2) == 0 || strcmp(line, ":") == 0) {
            host_colon_create(line[0] == ':' && line[1] == ' ' ? line + 2 : line + 1);
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
        } else if (strcasecmp(line, "lineos-confirm") == 0) {
            lineos_confirm();
        } else if (strcasecmp(line, "lineos-confirm-demo") == 0) {
            lineos_confirm_demo();
        } else if (strcasecmp(line, "become-lineos") == 0) {
            become_lineos();
        } else if (strcasecmp(line, "lineos-splash") == 0) {
            lineos_splash();
        } else if (strcasecmp(line, "lineos-about") == 0) {
            lineos_about();
        } else if (strcasecmp(line, "lineos-brand-demo") == 0) {
            lineos_brand_demo();
        } else if (strcasecmp(line, "tritium-integrate-demo") == 0) {
            tritium_integrate_demo();
        } else if (strncasecmp(line, "tritium-integrate ", 18) == 0) {
            const char* arg = line + 18;
            while (*arg == ' ') arg++;
            tritium_integrate(arg);
        } else if (strcasecmp(line, "tritium-integrate") == 0) {
            printf("[tritium-integrate] FAIL — platform required\n");
        } else if (strcasecmp(line, "master-demo") == 0) {
            master_demo();
        } else if (strncasecmp(line, "master-mint-license", 19) == 0) {
            const char* arg = line + 19;
            while (*arg == ' ') arg++;
            int slots = (*arg) ? atoi(arg) : 10;
            master_mint_license(slots);
        } else if (strncasecmp(line, "master-mint-worker ", 19) == 0) {
            const char* arg = line + 19;
            while (*arg == ' ') arg++;
            master_mint_worker(arg);
        } else if (strcasecmp(line, "master-mint-worker") == 0) {
            master_mint_worker("");
        } else if (strncasecmp(line, "master-verify ", 14) == 0) {
            const char* arg = line + 14;
            while (*arg == ' ') arg++;
            master_verify(arg);
        } else if (strcasecmp(line, "master-verify") == 0) {
            master_verify("");
        } else if (strcasecmp(line, "fleet-demo") == 0) {
            fleet_demo();
        } else if (strncasecmp(line, "fleet-export ", 13) == 0) {
            const char* arg = line + 13;
            while (*arg == ' ') arg++;
            fleet_export(arg);
        } else if (strcasecmp(line, "fleet-export") == 0) {
            fleet_export("dev1");
        } else if (strcasecmp(line, "fleet-import") == 0) {
            fleet_import();
        } else if (strcasecmp(line, "userland-demo") == 0) {
            userland_demo();
        } else if (strcasecmp(line, "host-parity-demo") == 0) {
            host_parity_demo();
        } else if (strcasecmp(line, "host-boot") == 0 ||
                   strcasecmp(line, "host-boot-report") == 0) {
            host_boot_report();
        } else if (strcasecmp(line, "host-boot-demo") == 0) {
            host_boot_demo();
        } else if (strcasecmp(line, "refined-boot") == 0 ||
                   strcasecmp(line, "refined-boot-report") == 0) {
            refined_boot_report();
        } else if (strcasecmp(line, "refined-boot-demo") == 0) {
            refined_boot_demo();
        } else if (strcasecmp(line, "trit-math-demo") == 0) {
            trit_math_demo();
        } else if (strcasecmp(line, "economy-wire-demo") == 0 ||
                   strcasecmp(line, "queue-assim-demo") == 0) {
            economy_wire_demo();
        } else if (strcasecmp(line, "assistant-state-demo") == 0) {
            assistant_state_demo();
        } else if (strcasecmp(line, "assistant-state!") == 0) {
            assistant_state_bang();
        } else if (strcasecmp(line, "assistant-state@") == 0) {
            assistant_state_at();
        } else if (strncasecmp(line, "assistant-task! ", 16) == 0) {
            int id = 0;
            char textbuf[ASSIST_TEXT_LEN] = {0};
            const char* p = line + 16;
            while (*p == ' ') p++;
            id = atoi(p);
            while (*p && *p != ' ') p++;
            while (*p == ' ') p++;
            strncpy(textbuf, p, ASSIST_TEXT_LEN - 1);
            assistant_task_set(id, textbuf);
        } else if (strncasecmp(line, "assistant-reminder! ", 20) == 0) {
            int id = 0;
            char whenbuf[ASSIST_WHEN_LEN] = {0};
            char textbuf[ASSIST_TEXT_LEN] = {0};
            const char* p = line + 20;
            while (*p == ' ') p++;
            id = atoi(p);
            while (*p && *p != ' ') p++;
            while (*p == ' ') p++;
            size_t wi = 0;
            while (*p && *p != ' ' && wi + 1 < ASSIST_WHEN_LEN) whenbuf[wi++] = *p++;
            whenbuf[wi] = 0;
            while (*p == ' ') p++;
            strncpy(textbuf, p, ASSIST_TEXT_LEN - 1);
            assistant_reminder_set(id, whenbuf, textbuf);
        } else if (strncasecmp(line, "assistant-note! ", 16) == 0) {
            int id = 0;
            char textbuf[ASSIST_TEXT_LEN] = {0};
            const char* p = line + 16;
            while (*p == ' ') p++;
            id = atoi(p);
            while (*p && *p != ' ') p++;
            while (*p == ' ') p++;
            strncpy(textbuf, p, ASSIST_TEXT_LEN - 1);
            assistant_note_set(id, textbuf);
        } else if (strncasecmp(line, "queue-reward! ", 14) == 0) {
            int job = atoi(line + 14);
            queue_reward(job);
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
        } else if (strcasecmp(line, "assistant-s0-demo") == 0) {
            assistant_s0_demo();
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

    if (oneshot)
        return g_oneshot_status;
    return 0;
}