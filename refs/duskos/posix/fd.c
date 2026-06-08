#include <stdio.h>
#include <unistd.h>
#include <string.h>
#include "opts.h"
#include "mem.h"

static char _zstrbuf[0x100];
static char* strtoz(char *s) {
	int len = *s++;
	memcpy(_zstrbuf, s, len);
	_zstrbuf[len] = 0;
	return _zstrbuf;
}

#define MAXFILECNT 0x20
#define STARTAT 0x10

static FILE *files[MAXFILECNT] = {NULL};

static FILE* getfile(dword id) {
	if ((id - STARTAT) < MAXFILECNT) return files[id-STARTAT];
	else return NULL;
}

static dword newid(FILE *f) {
	for (int i=0; i<MAXFILECNT; i++) {
		if (!files[i]) {
			files[i] = f;
			return i+STARTAT;
		}
	}
	_printf("Out of files!\n");
	return 0;
}

// we avoid fcntl.h for portability
// ( a u fd -- n )
void FDWRITE() {
	dword fd = ppop();
	dword u = ppop();
	dword a = ppop();
	if (fd < STARTAT) // hardcoded FD
			ppush(write(fd, absaddr(a), u));
	else
			ppush(fwrite(absaddr(a), 1, u, getfile(fd)));
}
// ( a u fd -- n )
void FDREAD() {
	dword fd = ppop();
	dword u = ppop();
	dword a = ppop();
	if (fd < STARTAT) // hardcoded FD
		ppush(read(fd, absaddr(a), u));
	else
		ppush(fread(absaddr(a), 1, u, getfile(fd)));
}

// ( strpath write? -- ?size fd-or-0 )
void FDOPEN() {
	dword dowrite = ppop();
	void *path = absaddr(ppop());
	FILE *f = fopen(strtoz((char*)path), dowrite ? "rb+" : "rb");
	if (!f && dowrite) // create it if it doesn't exist
		f = fopen(strtoz((char*)path), "wb");
	if (f) {
		fseek(f, 0, SEEK_END);
		ppush(ftell(f));
		fseek(f, 0, SEEK_SET);
		ppush(newid(f));
	} else ppush(0);
}
// ( fd -- )
void FDCLOSE() {
	dword idx = ppop() - STARTAT;
	fclose(files[idx]);
	files[idx] = NULL;
}
// ( off fd -- )
void FDSEEK() {
	dword fd = ppop();
	fseek(getfile(fd), ppop(), SEEK_SET);
}
