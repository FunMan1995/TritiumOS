#include <time.h>
#include "mem.h"

void NOW() { // ( -- time )
  struct timespec tp;
  clock_gettime(CLOCK_REALTIME, &tp);
  ppush(tp.tv_sec - 0x386e9500);
}
