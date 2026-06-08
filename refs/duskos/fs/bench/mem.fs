needs drv/timer io/kbd gr/buf

$4000 const 16KB
: .t ( ts -- ) elapsedus . ." us\n" ;
: 16kbargs ( -- src dst u ) here# dup 16KB + 16KB ;

."Copying a 16kb block in 1 byte chunks\n"
:~ ticks 16kbargs cmove .t ; ~

."Copying a 16kb block in 4 bytes chunks\n"
:~ ticks 16kbargs 4/ move .t ; ~

."Doing this 10 times on the same addresses\n"
:~ ticks 10 0 do 16kbargs 4/ move loop .t ; ~

."Doing it 10 times, same src, different dst\n"
:~ ticks 10 0 do here# dup 16KB i 1+ * + 16KB 4/ move loop .t ; ~

."Doing it 10 times, different src, different dst\n"
:~ ticks 10 0 do here# dup 16KB i * - swap 16KB i 1+ * + 16KB 4/ move loop .t ; ~

."Copying 16kb to the screen buffer\n"
:~ screen buffer ?dup not if ."no screen, skip\n" else
     ."This will corrupt the screen, press a key to continue\n"
     key drop
     ticks swap here# swap 16KB 4/ move .t then ; ~
