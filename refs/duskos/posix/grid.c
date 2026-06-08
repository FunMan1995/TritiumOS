#include <stdio.h>
#include <sys/ioctl.h>
#include <unistd.h>
#include "mem.h"

void TERMSZ() { // ( -- columns lines )
	struct winsize w;
    if (ioctl(STDOUT_FILENO, TIOCGWINSZ, &w) == -1) {
		w.ws_row = 24;
		w.ws_col = 80;
    }
	ppush(w.ws_col);
	ppush(w.ws_row);
}
