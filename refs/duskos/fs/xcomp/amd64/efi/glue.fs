' efirtype console!
AllocatedPages $1000 * HEREMAX !
bootblkidx newefiblk newfatfs bootfs!
:~ f<< quit ; ~ init.fs
