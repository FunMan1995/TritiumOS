needs io/stream fs/core fs/sh lib/psrs lib/str lib/fmt
unit xcomp/deploy

: spitfile ( dstio name -- ) openpath dipdup spitclose SPC swap putc ;
: spitunit ( dstio name -- ) ".fs" strcat spitfile ;

stringlist fsUnits lib/str lib/type lib/struct lib/ival io/stream fs/core io/blk
stringlist tarUnits ar/tar fs/tar
stringlist fatUnits lib/time io/part fs/fat

: spitunits ( dstio stringlist -- )
  begin dup c@ while 2dup spitunit s) repeat 2drop ;

: spitboot ( dstio archname -- )
  >r >r \ V1=arch V2=io
  V2 "xcomp/lo.fs" spitfile
  V1 "i386" s= V1 "amd64" s= or if
    V2 V1 "xcomp/%s/hal1.fs" sprintf spitfile
    V2 "xcomp/x86/hallo.fs" spitfile
    V2 "xcomp/hallo.fs" spitfile
    V2 "xcomp/x86/hal2.fs" spitfile
    V2 V1 "xcomp/%s/hal3.fs" sprintf spitfile
    V2 "xcomp/x86/hal4.fs" spitfile
    V2 V1 "xcomp/%s/hal5.fs" sprintf spitfile
  else
    V2 V1 "xcomp/%s/hallo.fs" sprintf spitfile
    V2 "xcomp/hallo.fs" spitfile
    V2 V1 "xcomp/%s/hal.fs" sprintf spitfile then
  V2 "xcomp/boot.fs" spitfile
  2rdrop ;
