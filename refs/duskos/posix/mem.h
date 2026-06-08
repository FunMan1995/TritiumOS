#include <inttypes.h>

typedef uint8_t byte;
typedef uint16_t word;
typedef uint32_t dword;

dword ppop();
dword ppeek();
void ppush(dword n);
void pset(dword n);
void* absaddr(dword a);
