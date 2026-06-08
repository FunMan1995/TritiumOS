#ifdef __MINGW32__
	#include <memoryapi.h>
	#include <errhandlingapi.h>
	void* mmap(void* _0, size_t length, int _1, int _2, int _3, off_t _4) {
		return VirtualAlloc(NULL, length, MEM_RESERVE|MEM_COMMIT, PAGE_EXECUTE_READWRITE);
	}
	#define PROT_READ 0
	#define PROT_WRITE 0
	#define PROT_EXEC 0
	#define MAP_PRIVATE 0
	#define MAP_ANONYMOUS 0
	#define MAP_FIXED 0
	#define MAP_FAILED 0
#else
	#include <sys/mman.h>
#endif
