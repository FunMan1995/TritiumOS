#include <unistd.h>
#include "opts.h"
#include "mem.h"
#include "duskconst.h"

void MAYBEKEY() { // ( -- ?c f )
	char c;
	if (read(STDIN_FILENO, &c, 1) == 1) {
		ppush((uint32_t)c);
		ppush(1);
	} else {
		ppush(0);
	}
}

void RTYPE() {
	dword u = ppop();
	void *a = absaddr(ppop());
	FILE *f = stderremit ? stderr : stdout;
	fwrite(a, 1, u, f);
	fflush(f);
}
