#ifdef OPENPTY
#define _XOPEN_SOURCE 600
#define _DEFAULT_SOURCE
#include <fcntl.h>
#endif
#include <signal.h>
#include <stdio.h>
#include <stdlib.h>
#include <unistd.h>
#include <string.h>
#include <errno.h>
#include <sys/time.h>
#include "mmap.h"
#include "common.h"
#include "../posix/opts.h"

#ifdef ARCH_arm
#ifdef PLAT_bsd
#include <machine/sysarch.h>
#endif
#endif

static size_t duskmemsz;
static void *data;

struct interopzone {
	void* funcs;
	long rW;
	dword *rPSP;
	dword *rRSP; // shouldn't be used on the C side, not all kernels save it
	long argc;
	char **argv;
	char* payload;
	long memsz;
};

static struct interopzone *iz;

#ifdef __MINGW32__
#include <errhandlingapi.h>
static uint32_t segvHandler;
static LONG WINAPI unhandledException(PEXCEPTION_POINTERS info)
{
    info->ContextRecord->Eip = segvHandler;
    return EXCEPTION_CONTINUE_EXECUTION;
}
#endif

dword ppop() { dword n = iz->rW; iz->rW = *(iz->rPSP++); return n; }
dword ppeek() { return iz->rW; }
void ppush(dword n) { *(--iz->rPSP) = iz->rW; iz->rW = n;}
void pset(dword n) { iz->rW = n; }
void* absaddr(dword a) {
#ifdef ARCH_amd64
	return data + a;
#else
	return (void*)a;
#endif
}
static void _snooze() { usleep(100); }
static void _debug() {
	dword n1 = ppop();
	dword n2 = ppop();
	fprintf(stderr, "DEBUG: %x (%d) %x (%d)\n", n1, n1, n2, n2);
}
static void _sysexit() { exit(0); }
static void c_sleep() { usleep(ppop()); }
static void _ticks() {
	struct timeval time;
	gettimeofday(&time, NULL);
	int64_t s1 = (int64_t)(time.tv_sec) * 1000000;
	ppush((int32_t) (s1 + time.tv_usec));
}
static void _clear_icache() {
#ifdef ARCH_arm
#ifdef PLAT_bsd
	arm_sync_icache((uintptr_t)data, duskmemsz);
#endif
#ifdef PLAT_linux
	/* This caching method below might seem weird, but it turns out that
     * clearing the cache for the whole Dusk memory like we do under BSD above
     * is **super freaking slow**!!!.
     * So, we try to be a bit more surgical about cache clearing by getting
     * current "HERE" and only clearing memory up to that address. It's weird,
     * but it makes things much much faster. Maybe that as we get near the end
     * of the mmap, we "spill over" and all hell breaks loose? maybe.
     */
    unsigned int here = *(unsigned int *)(data+0x1c);
	__builtin___clear_cache(data, (void*)here);
#endif
#endif
}

void common_sethdlr() { // handle bad things
	dword arg3 = ppop();
	dword arg2 = ppop();
	dword arg1 = ppop();
#ifdef __MINGW32__
	// SetConsoleCtrlHandler is pointless since in recent Windows versions, the handler
	// is run in a separate thread with 5 seconds timeout, and cannot prevent the process
	// to be terminated. Users might want to use SDL there anyway.
#ifndef NO_TRAP_SEGV
	segvHandler = arg2;
	SetUnhandledExceptionFilter(unhandledException);
#endif
#else
	struct sigaction sa;
	sa.sa_handler = absaddr(arg1);
	sigemptyset (&sa.sa_mask);
	sa.sa_flags = SA_NODEFER;
	sigaction(SIGINT, &sa, NULL);
	sa.sa_handler = absaddr(arg2);
#ifndef NO_TRAP_SEGV
	sigaction(SIGSEGV, &sa, NULL);
#endif
#endif
}

// from io.c
void MAYBEKEY();
void RTYPE();

// from fd.c
void FDWRITE();
void FDREAD();
void FDOPEN();
void FDCLOSE();
void FDSEEK();

// from time.c
void NOW();

CALLBACKFUN cbfuncs[APIFUNCCNT] = {NULL};

int common_init(int argc, char *argv[], size_t memsz) {

#ifdef OPENPTY
	int fd = open("/dev/ptmx", O_RDWR | O_NOCTTY);
	fprintf(stderr, "Allocating terminal %s controlled by file descriptor %d\n", ptsname(fd), fd);
	grantpt(fd);
	unlockpt(fd);
	fcntl(fd, F_SETFL, fcntl(fd, F_GETFL, 0) | O_NONBLOCK);
#endif

	duskmemsz = memsz;
	data = mmap(
		NULL, memsz, PROT_READ|PROT_WRITE|PROT_EXEC,
		MAP_PRIVATE|MAP_ANONYMOUS, -1, 0);
	if (data == MAP_FAILED) {
		printf("mmap failed: %s (%d).\n", strerror(errno),  errno);
		return 1;
	}
	iz = data+MAXKERNELSZ;
	iz->funcs = cbfuncs;
	iz->argc = argc;
	iz->argv = argv;
	iz->payload = NULL;
	iz->memsz = memsz;

	// Common API setup
	cbfuncs[0] = _snooze;
	cbfuncs[1] = RTYPE;
	cbfuncs[2] = MAYBEKEY;
	cbfuncs[3] = _debug;
	cbfuncs[4] = _sysexit;
	cbfuncs[5] = c_sleep;
	cbfuncs[6] = _ticks;
	cbfuncs[12] = _clear_icache;
	cbfuncs[13] = common_sethdlr;
	cbfuncs[16] = FDWRITE;
	cbfuncs[17] = FDREAD;
	cbfuncs[18] = FDOPEN;
	cbfuncs[19] = FDCLOSE;
	cbfuncs[20] = FDSEEK;
	cbfuncs[21] = NOW;
	return 0;
}

static int common_readfile(void *dst, char *path, size_t maxsz) {
	size_t sz;
	FILE* f = fopen(path, "rb");
	if (!f) {
		printf("Couldn't open %s\n", path);
		return 1;
	}
	sz = fread(dst, 1, maxsz, f);
	if (!feof(f)) {
		printf("Incomplete read of file\n");
		return 1;
	}
	fclose(f);
	((char*)dst)[sz] = 0; // null terminate
	return 0;
}

void common_copykernel(void *kernel, size_t sz) {
	memcpy(data, kernel, sz);
}
void common_copypayload(void *payload, size_t payloadsz, char* bootcmd) {
	int bootlen = strlen(bootcmd);
	size_t totsz = bootlen + payloadsz;
	iz->payload = data + iz->memsz - (totsz+1);
	memcpy(iz->payload, payload, payloadsz);
	memcpy(iz->payload+payloadsz, bootcmd, bootlen+1);
}

#ifdef PAYLOADNAME
#define MAXPAYLOADSZ (1024*1024)
int common_init_readfiles(int argc, char *argv[]) {
	if (common_init(argc, argv, 32*1024*1024)) return 1;
	if (common_readfile(data, KERNELNAME, MAXKERNELSZ)) return 1;
	iz->payload = data + iz->memsz - MAXPAYLOADSZ;
	if (common_readfile(iz->payload, PAYLOADNAME, MAXPAYLOADSZ)) return 1;
	setupprompt();
	strcat(iz->payload, bootcmd);
	return 0;
}
#endif

void common_exec() {
	long (*cf)() = (void*)data;
	long res;
	if (iz->payload) {
		long res = cf();
		printf("Returned from root call!? %lx\n", res);
	} else printf("No payload, aborting!\n");
}
