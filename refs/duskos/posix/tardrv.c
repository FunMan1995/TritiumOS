#include <string.h>
#include "opts.h"
#include "mem.h"
#include "fstar.h"

#define SECSZ 512
// ( sec dst -- )
void BOOTDRVRD() {
	dword dst = ppop();
	dword sec = ppop();
	if (SECSZ*(sec+1) < sizeof(fstar)) {
		memcpy(absaddr(dst), &fstar[SECSZ*sec], SECSZ);
	} else {
		_printf("Out of range fs.tar read %d\n", sec);
	}
}
