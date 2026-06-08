#include <SDL.h>
#include "common.h"
#include "../posix/opts.h"

extern void (*sigINThandler)();

// from sdl.c
void _screenmode();
void _screen();
void _getnkc();
void _mouse();
void c_exit();
void c_clipboard();
int runSDL(SDL_ThreadFunction fn);

static void _sethdlr() {
	sigINThandler = absaddr(ppeek());
	common_sethdlr();
}

int SDLCALL thread_main (void *data) {
	if (common_init_readfiles(0, NULL)) return 1;
	cbfuncs[2] = _getnkc;
	cbfuncs[4] = c_exit;
	cbfuncs[9] = _screenmode;
	cbfuncs[10] = _screen;
	cbfuncs[11] = _mouse;
	cbfuncs[13] = _sethdlr;
	cbfuncs[14] = c_clipboard;
	SDL_SetThreadPriority(SDL_THREAD_PRIORITY_LOW);
	common_exec();
	return 0;
}

int main (int argc, char *argv[]) {
	inlinecmd("quit");
	return runSDL(thread_main);
}
