#include <unistd.h>
#include <SDL.h>
#include "vm.h"
#include "opts.h"
#include "boot-graphic.h"

static void SNOOZE() { usleep(100); }
// from io.c
void MAYBEKEY();
void RTYPE();
// from tardrv.c
void BOOTDRVRD();
// from sdl.c
void _screenmode();
void _screen();
void _getnkc();
void _mouse();
void c_exit();
void c_clipboard();
int runSDL(SDL_ThreadFunction fn);

int SDLCALL thread_main (void *data) {
	setupvm(bootstring, sizeof(bootstring));
	newsyscall("(snooze)", SNOOZE);
	newsyscall("(rtype)", RTYPE);
	newsyscall("(?key)", MAYBEKEY);
	newsyscall("?getnkc", _getnkc);
	newsyscall("bootdrv@", BOOTDRVRD);
	newsyscall("screeninfo", _screenmode);
	newsyscall("screencb", _screen);
	newsyscall("mousecb", _mouse);
	newsyscall("clipcb", c_clipboard);
	SDL_SetThreadPriority(SDL_THREAD_PRIORITY_LOW);
	int res = runvm();
	c_exit();
	return res;
}

int main (int argc, char *argv[]) {
	inlinecmd("quit");
	if (duskopts(argc, argv)) return 1;
	return runSDL(thread_main);
}
