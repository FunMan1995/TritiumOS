/* Common to both POSIX dusk and usermode dusk-raw */
#include <stdio.h>
#include <unistd.h>
#include <string.h>
#include <stdlib.h>

char *bootcmd = NULL;
int stderremit = 0;
int forceprompt = 0;

static void usage() {
	printf("Usage: ./dusk [-ep] [-n needs] [-c bootcmd] [-f bootfile]\n");
}

void inlinecmd(char *cmd) {
	// why +4? some callers such as bootneeds() need leeway to insert \n.
	size_t appendlen = strlen(cmd) + 4;
	if (bootcmd) {
		bootcmd = realloc(bootcmd, strlen(bootcmd) + appendlen);
		strcat(bootcmd, " ");
		strcat(bootcmd, cmd);
	} else {
		bootcmd = malloc(appendlen);
		strcpy(bootcmd, cmd);
	}
}

static void bootneeds() {
	inlinecmd("needs");
	inlinecmd(optarg);
	strcat(bootcmd, "\n");
}

static int bootfile() {
	int sz;
	char *p;
	FILE *fp = fopen(optarg, "r");
	if (!fp) {
		printf("Couldn't open %s\n", optarg);
		return 3;
	}
	fseek(fp, 0, SEEK_END);
	sz = ftell(fp);
	fseek(fp, 0, SEEK_SET);
	if (bootcmd) {
		char *s = malloc(strlen(bootcmd) + sz + 2);
		p = stpcpy(s, bootcmd);
		*p++ = ' ';
		bootcmd = s;
	} else {
		bootcmd = malloc(sz + 1);
		p = bootcmd;
	}
	fread(p, 1, sz, fp);
	p[sz] = 0;
	fclose(fp);
	return 0;
}

void setupprompt() {
	inlinecmd("needs lib/diag app/prompt\n prompt$ .\"Dusk OS\n\" .free quit");
}

int duskopts(int argc, char **argv) {
	int c;

	opterr = 0;
	while ((c = getopt(argc, argv, "epn:c:f:")) != -1) {
		switch (c) {
			case 'e': stderremit = 1; break;
			case 'p': forceprompt = 1; break;
			case 'n': bootneeds(); break;
			case 'c': inlinecmd(optarg); break;
			case 'f': if (bootfile()) return 3; break;
			default:
				usage();
				return 1;
		}
	}
	if (optind < argc) {
		usage();
		return 2;
	}
	if ((!bootcmd) || forceprompt) setupprompt();
	inlinecmd("bye");
	return 0;
}
