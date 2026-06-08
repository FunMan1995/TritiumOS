#include <stdint.h>
#include <stdio.h>
#include "../posix/mem.h"
#include "../posix/duskconst.h"

/* Kernels aren't expected to be bigger than this minus the size of the
 * InteropZone which is placed right at the end of it. When they are, it's an
 * indication that we're running a frozen kernel.
 */
#define BOOTZONESZ (8*1024) // In sync with Dusk kernels
#define MAXKERNELSZ (BOOTZONESZ-sizeof(struct interopzone))

typedef void (*CALLBACKFUN)();

int common_init(int argc, char *argv[], size_t memsz);
#ifdef PAYLOADNAME
int common_init_readfiles(int argc, char *argv[]);
#endif
void common_copykernel(void *kernel, size_t sz);
void common_copypayload(void *payload, size_t sz, char* bootcmd);
// Normally never returns
void common_exec();

void common_sethdlr();

#define APIFUNCCNT 0x100
// When extending the Usermode API, one shoudn't use IDs under this count.
#define APIRESERVEDCNT 0x20
extern CALLBACKFUN cbfuncs[];
