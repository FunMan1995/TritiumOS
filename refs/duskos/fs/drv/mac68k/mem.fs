unit drv/mac68k/mem

consts $2aa ApplZone $904 CurrentA5 $908 CurStackBase $114 HeapEnd \
       $220 MemErr $108 MemTop $31e MinStack $2b2 RAMBase $2ae ROMBase \
       $1f8 SysParam $2a6 SysZone $118 TheZone

: W>A0 $2047 w, ; immediate
: W>D0 $2007 w, ; immediate
: A0>W $2e08 w, ; immediate
: D0>W $2e00 w, ; immediate

: NewPtr ( sz -- a res ) W>D0 [ $a11e w, ] A0>W dup D0>W ;
: DisposePtr ( a -- ) W>A0 [ $a01f w, ] drop ;
: GetPtrSize ( a -- sz ) W>A0 [ $a021 w, ] D0>W ;