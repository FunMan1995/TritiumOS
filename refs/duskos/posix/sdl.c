#include <SDL.h>
#include <stdbool.h>
#ifdef __MINGW32__
#include <windows.h>
#else
#include <pthread.h>
#endif
#include "mem.h"
#include "duskconst.h"

#define KEY_UNICODE_OFFSET 0x400
// we use b30, reserved on the NKC, to indicate whether the presence in
// "keybuf" means "pressed" or "released". When b30 is set, key is "released".
#define KEY_ISRELEASED 0x40000000

static int width, height;
static SDL_Renderer *renderer;
static SDL_Texture *texture;
static SDL_Thread *duskThread;
void (*sigINThandler)();
static dword keybuf[128];
static SDL_mutex *inputMutex;
static int nextbufRead, nextbufWrite;
static dword mouseX, mouseY, mouseButtons;

/***** Screen Stuff *****/
/* Dusk writes to a framebuffer as well as to a "damage struct" to indicate
 * which portions of the screen we need to redraw. Because SDL runs on a
 * different thread, we need to think about synchronisation. Our solution is...
 * Don't bother.
 * Here's the idea. We organize our code to avoid collisions as much as
 * possible, that is, we set damage to 0 immediately after having read
 * it, and then once in a while, we do a full screen refresh.
 * So, although synchronization problems can cause us to miss damage reports,
 * the consequences of that missed report is minimized by periodical full
 * refreshes.
 * It's simpler that way.
 */

static dword *damage = NULL;
// Number of damage checks after which we unconditionally refresh the whole
// screen. With a 10ms delay between cycles, that gives us a full refresh per
// second
#define DAMAGETIMEOUT 100
static dword damagecnt = DAMAGETIMEOUT;
static void* framebuffer;

void _screenmode() { // ( -- width height )
	ppush(width);
	ppush(height);
}

void _screen() { // ( buffer damage -- )
	damage = (dword*)absaddr(ppop());
	framebuffer = absaddr(ppop());
}

static void checkScreenDamage() {
	dword dmg, scale;
	SDL_Rect rect;

	if (!damage) return;
	if (--damagecnt) {
		dmg = *damage;
		if (!dmg) return;
		*damage = 0;
		scale = dmg >> 28;
		rect.x = ((dmg >> 21) & 0x7f) << scale;
		rect.y = ((dmg >> 14) & 0x7f) << scale;
		rect.w = ((((dmg >> 7) & 0x7f) + 1) << scale) - rect.x - 1;
		rect.h = (((dmg & 0x7f) + 1) << scale) - rect.y - 1;
	} else {
		*damage = 0;
		damagecnt = DAMAGETIMEOUT;
		rect.x = rect.y = 0;
		rect.w = width;
		rect.h = height;
	}
	SDL_UpdateTexture(
		texture, &rect,
		framebuffer + rect.x * 2 + rect.y * width * 2, width * 2);
	SDL_RenderClear(renderer);
	SDL_RenderCopyEx(renderer, texture, NULL, NULL, 0, NULL, SDL_FLIP_VERTICAL);
	SDL_RenderPresent(renderer);
}
/***** End of Screen Stuff *****/

static void SDL_KillThread(SDL_Thread *thread) {
#ifdef __MINGW32__
		HANDLE hThread = OpenThread(THREAD_TERMINATE, FALSE, SDL_GetThreadID(thread));
		TerminateThread(hThread, 0);
		CloseHandle(hThread);
#else
		pthread_cancel((pthread_t)SDL_GetThreadID(thread));
#endif
}

static void appendInput(dword code) {
	SDL_LockMutex(inputMutex);
	if ((nextbufWrite + 1) % 128 != nextbufRead) {
		keybuf[nextbufWrite] = code;
		nextbufWrite = (nextbufWrite + 1) % 128;
	}
	SDL_UnlockMutex(inputMutex);
}

int SDLCALL thread_cont (void *data) {
	SDL_SetThreadPriority(SDL_THREAD_PRIORITY_LOW);
	// TODO: Thread may have a new stack, we need to change Dusk's stack accordingly!
	if (sigINThandler) sigINThandler();
	return 0;
}

static int shouldstop = 0;
void c_exit() {
	shouldstop = 1;
}

dword sdl2nkc(dword sdlkey) {
	switch (sdlkey) {
		// See doc/sys/kbd for key code reference
		case SDLK_RETURN: return '\r';
		case SDLK_BACKSPACE: return '\b';
		case SDLK_TAB: return '\t';
		case SDLK_ESCAPE: return 0x1b;
		case SDLK_INSERT: return 0xa3;
		case SDLK_DELETE: return 0xa4;
		case SDLK_HOME: return 0xa5;
		case SDLK_END: return 0xa6;
		case SDLK_PAGEUP: return 0xa7;
		case SDLK_PAGEDOWN: return 0xa8;
		case SDLK_UP: return 0xab;
		case SDLK_DOWN: return 0xac;
		case SDLK_LEFT: return 0xa9;
		case SDLK_RIGHT: return 0xaa;
		case SDLK_F1: return 0x81;
		case SDLK_F2: return 0x82;
		case SDLK_F3: return 0x83;
		case SDLK_F4: return 0x84;
		case SDLK_F5: return 0x85;
		case SDLK_F6: return 0x86;
		case SDLK_F7: return 0x87;
		case SDLK_F8: return 0x88;
		case SDLK_F9: return 0x89;
		case SDLK_F10: return 0x8a;
		case SDLK_LSHIFT: return KBD_LSHIFT;
		case SDLK_RSHIFT: return KBD_RSHIFT;
		case SDLK_LCTRL: return KBD_LCONTROL;
		case SDLK_RCTRL: return KBD_RCONTROL;
		case SDLK_LALT: return KBD_LALT;
		case SDLK_RALT: return KBD_RALT;
		case SDLK_LGUI: return KBD_LGUI;
		case SDLK_RGUI: return KBD_RGUI;
		default: return 0;
	}
}

/* Normally, we send "textual" keystrokes through the SDL_TEXTINPUT event so
 * that we can ignore the host OS' keyboard layout. This works fine, except for
 * CTRL+<anychar> keystrokes. The "<anychar>" part of the sequence generates no
 * SDL_TEXTINPUT event, so it never goes down to Dusk. To work around that, we
 * remember whether the Control mod key is down. When it's down, we send any
 * unrecognized KEYUP/KEYDOWN that is in the ASCII range down to Dusk.
 */
static int lctrldown = 0;
static void handleEvents() {
    SDL_Event event;
    while (SDL_PollEvent(&event)) {
		switch (event.type) {
			case SDL_QUIT: {
				c_exit();
				break;
			}

			case SDL_MOUSEMOTION: {
				SDL_LockMutex(inputMutex);
				mouseX = event.motion.x;
				mouseY = event.motion.y;
				SDL_UnlockMutex(inputMutex);
				break;
			}

			case SDL_MOUSEBUTTONDOWN:
			case SDL_MOUSEBUTTONUP: {
				SDL_LockMutex(inputMutex);
				dword btnmask = 0;
				if (event.button.button == SDL_BUTTON_LEFT)
					btnmask = 1;
				else if (event.button.button == SDL_BUTTON_RIGHT)
					btnmask = 2;
				else if (event.button.button == SDL_BUTTON_MIDDLE)
					btnmask = 4;
				if (event.button.state == SDL_PRESSED)
					mouseButtons |= btnmask;
				else
					mouseButtons &= ~btnmask;
				SDL_UnlockMutex(inputMutex);
				break;
			}

			case SDL_KEYDOWN: {
				if (event.key.keysym.sym == SDLK_F12) {
					if (sigINThandler && SDL_GetModState() & KMOD_CTRL) {
						SDL_KillThread(duskThread);
						duskThread = SDL_CreateThread(thread_cont, "DuskOS", NULL);
					}
				}
				dword nkc = sdl2nkc(event.key.keysym.sym);
				if (nkc) {
					if (nkc == KBD_LCONTROL) lctrldown = 1;
					appendInput(nkc);
				} else if (lctrldown && (event.key.keysym.sym < 0x80)) {
					appendInput(toupper(event.key.keysym.sym));
				}
				break;
			}
			case SDL_KEYUP: {
				dword nkc = sdl2nkc(event.key.keysym.sym);
				if (nkc) {
					if (nkc == KBD_LCONTROL) lctrldown = 0;
					appendInput(nkc|KEY_ISRELEASED);
				} else if (lctrldown && (event.key.keysym.sym < 0x80)) {
					appendInput(toupper(event.key.keysym.sym)|KEY_ISRELEASED);
				}
				break;
			}
			case SDL_TEXTINPUT: {
				// Even though the logic below sends down unicode code points
				// when it encounters non-ASCII characters, Dusk has no support
				// for it, input is going to end up garbled.
				char* txt = event.text.text;
				while(*txt) {
					dword val = *txt & 0xff;
					dword nextval = txt[1] & 0xff;
					dword nextnextval = nextval == 0 ? 0 : txt[2] & 0xff;
					if (val < 0x80) {
						// ASCII
						appendInput(val|KBD_PASSTHROUGH);
						txt++;
					} else if (val >= 0xc0 && val < 0xe0 && nextval >= 0x80 && nextval < 0xc0) {
						// UTF-8 2-byte
						appendInput((((val & 0x1f) << 6) | ((nextval & 0x3f) + KEY_UNICODE_OFFSET)) | KBD_PASSTHROUGH);
						txt += 2;
					} else if (val >= 0xe0 && val < 0xf0 && nextval >= 0x80 && nextval < 0xc0 && nextnextval >= 0x80 && nextnextval < 0xc0) {
						appendInput((((val & 0x0f) << 12) | ((nextval & 0x3f) << 6) | ((nextnextval & 0x3f) + KEY_UNICODE_OFFSET)) | KBD_PASSTHROUGH);
						txt += 3;
						// UTF-8 3-byte
					} else {
						// outside BMP or invalid, skip
						txt++;
					}
				}
				break;
			}
		}
    }
}

void _getnkc() {
	SDL_LockMutex(inputMutex);
	if (nextbufRead != nextbufWrite) {
		dword nkc = keybuf[nextbufRead];
		ppush(nkc & ~KEY_ISRELEASED);
		nextbufRead = (nextbufRead + 1) % 128;
		ppush(nkc & KEY_ISRELEASED ? KBD_EVENT_RELEASE : KBD_EVENT_PRESS);
	} else {
		ppush(KBD_EVENT_NONE);
	}
	SDL_UnlockMutex(inputMutex);
}

void _mouse() {
	SDL_LockMutex(inputMutex);
	ppush(mouseX);
	ppush(mouseY);
	ppush(mouseButtons);
	SDL_UnlockMutex(inputMutex);
}

void c_clipboard() { // ( f a u -- ?u )
	dword len = ppop();
	char* addr = (char*)absaddr(ppop());
	if (ppeek() == 0) { // get
		char* txt = SDL_GetClipboardText();
		if (len) {
			strncpy(addr, txt, len);
			pset(strlen(txt));
			if (ppeek() > len) pset(len);
		} else {
			pset(strlen(txt));
		}
		SDL_free(txt);
	} else if (ppop() == 1) { // set
		char save = addr[len]; addr[len] = 0;
		SDL_SetClipboardText(addr);
		addr[len] = save;
	}
}

int runSDL(SDL_ThreadFunction fn) {
	width = 1024;
	height = 768;
	if (SDL_Init(SDL_INIT_VIDEO) != 0) {
		fprintf(stderr, "Unable to initialize SDL: %s", SDL_GetError());
		return 1;
	}
	mouseX = mouseY = mouseButtons = 0;
	sigINThandler = 0;
	nextbufRead = nextbufWrite = 0;
	inputMutex = SDL_CreateMutex();
	SDL_EnableScreenSaver();
	SDL_ShowCursor(false);
	SDL_SetHint(SDL_HINT_RENDER_SCALE_QUALITY, "best");
	SDL_Window *window = SDL_CreateWindow("DuskOS",	SDL_WINDOWPOS_UNDEFINED, SDL_WINDOWPOS_UNDEFINED, width, height, SDL_WINDOW_SHOWN);
	if (window == NULL) {
		fprintf(stderr, "Could not create window: %s\n", SDL_GetError());
		SDL_Quit();
		return 1;
	}
	renderer = SDL_CreateRenderer(window, -1, SDL_RENDERER_PRESENTVSYNC);
	if (renderer == NULL) {
		fprintf(stderr, "Could not create renderer: %s\n", SDL_GetError());
		SDL_Quit();
		return 1;
	}
	texture = SDL_CreateTexture(renderer, SDL_PIXELFORMAT_RGB565, SDL_TEXTUREACCESS_STREAMING, width, height);
	if (texture == NULL) {
		fprintf(stderr, "Could not create texture: %s\n", SDL_GetError());
		SDL_Quit();
		exit(1);
	}
	duskThread = SDL_CreateThread(fn, "DuskOS", NULL);
	while (!shouldstop) {
		handleEvents();
		checkScreenDamage();
		SDL_Delay(10);
	}
	SDL_KillThread(duskThread);
	SDL_Quit();
	return 0;
}
