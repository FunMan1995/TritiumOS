' efirtype console!
AllocatedPages $1000 * SYSVARS + HEREMAX !
bootblkidx newefiblk newfatfs bootfs!
:~ f<< quit ; ~ init.fs
