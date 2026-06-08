#include <unistd.h>
#include "vm.h"
#include "opts.h"
#include "boot-stream.h"

static void SNOOZE() { usleep(100); }
// from io.c
void MAYBEKEY();
void RTYPE();
// from tardrv.c
void BOOTDRVRD();

int main(int argc, char **argv) {
	if (duskopts(argc, argv)) return 1;
	setupvm(bootstring, sizeof(bootstring));
	newsyscall("(snooze)", SNOOZE);
	newsyscall("(rtype)", RTYPE);
	newsyscall("(?key)", MAYBEKEY);
	newsyscall("bootdrv@", BOOTDRVRD);
	return runvm();
}
