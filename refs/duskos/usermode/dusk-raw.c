#include <stdlib.h>
#include <string.h>
#include "common.h"
#include "kernel.h"
#include "payload_raw.h"
#include "../posix/opts.h"

// from tardrv.c
void BOOTDRVRD();

int main(int argc, char *argv[]) {
	char *s, *p;
	size_t memsz = 32*1024*1024;
	if (duskopts(argc, argv)) return 1;
	if (common_init(argc, argv, memsz)) return 1;
	common_copykernel(kernel, sizeof(kernel));
	common_copypayload(payload, sizeof(payload), bootcmd);
	cbfuncs[10] = BOOTDRVRD;
	common_exec();
	return 0;
}

