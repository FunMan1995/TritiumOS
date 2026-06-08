\ Print statistics about the size of Dusk OS
needs io/stream fs/core fs/sh lib/str text/ts xcomp/deploy

: bc walksize ;
: bcn ( ... n -- bytecount )
  0 swap 0 do swap ".fs" strcat lookup# bc + loop ;
: dirbc ( -- bytecount )
  0 enterdir begin gotonext while
    walkdir? if walk>r dirbc r>walk + else bc + then repeat ;
: listbc ( stringlist -- bytecount )
  0 begin over c@ while
    over ".fs" strcat lookup# bc + dip s) | repeat nip ;

stringlist ignore doc data tests bench
: allcode ( -- bytecount )
  0 bootfs walk begin gotonext while
    walkname lowstr ignore sfind if drop else
      walkdir? if walk>r dirbc r>walk + else bc + then then repeat ;

: .k $400 /+ . ;
: spit" ( n -- ) ts[ [rcompile] ." 40 tsgo swap .k ]ts nl> ;
."Kilobytes of code in Dusk OS\n"
allcode spit"Everything but /doc /data /tests /bench"
p"doc" dirbc spit"Documentation"
p"tests" dirbc p"bench" dirbc + spit"Automated tests and benches"
strings< xcomp/boot xcomp/lo
bcn fsUnits listbc + fatUnits listbc + spit"Boot payload minus HAL"
strings< com/link com/ether com/slip com/arp com/ip4 com/udp com/net
bcn spit"TCP/IP stack (in progress)"
strings< comp/tok comp/sym comp/sig comp/w
bcn const compcommon
p"comp/c.fs" bc p"comp/c" dirbc + compcommon + spit"C compiler"
p"comp/oberon.fs" bc p"comp/oberon" dirbc + compcommon +
spit"Oberon compiler"
p"oberon" dirbc spit"Oberon system"
p"comp/lisp" dirbc strings< comp/lisp mem/cons
bcn + spit"Lisp"
strings< text/ed app/ed
bcn spit"Text Editor"
p"drv" dirbc spit"All drivers"
p"bench/codesz.fs" bc spit"This script"
nl>
: _ does> tsgo ; map< _ 16 TS1 22 TS2 28 TS3 34 TS4 40 TS5
."CPU-specific... i386  amd64 arm   riscv m68k \n" ts[
."Assembler"
TS1 p"asm/x86.fs" bc .k
TS2 ."same"
TS3 p"asm/arm.fs" bc .k
TS4 p"asm/riscv.fs" bc .k
TS5 p"asm/m68k.fs" bc .k
."\nDisassembler"
TS1 p"asm/x86d.fs" bc .k
TS2 ."same"
TS3 p"asm/armd.fs" bc .k
TS4 p"asm/riscvd.fs" bc .k
."\nKernel"
strings< xcomp/x86/kernel xcomp/x86/kernelm
bcn const x86kernel
TS1 p"xcomp/i386/kernel.fs" bc x86kernel + .k
TS2 p"xcomp/amd64/kernel.fs" bc x86kernel + .k
TS3 p"xcomp/arm/kernel.fs" bc .k
TS4 p"xcomp/riscv/kernel.fs" bc .k
TS5 p"xcomp/m68k/kernel.fs" bc .k
."\nHAL"
p"xcomp/hallo.fs" bc const hallo
strings< xcomp/x86/hallo xcomp/x86/hal2 xcomp/x86/hal4
bcn const x86hal
strings< xcomp/i386/hal1 xcomp/i386/hal3 xcomp/i386/hal5
TS1 bcn x86hal + .k
strings< xcomp/amd64/hal1 xcomp/amd64/hal3 xcomp/amd64/hal5
TS2 bcn x86hal + .k
strings< xcomp/arm/hallo xcomp/arm/hal
TS3 bcn .k
strings< xcomp/riscv/hallo xcomp/riscv/hal
TS4 bcn .k
strings< xcomp/m68k/hallo xcomp/m68k/hal
TS5 bcn .k
."\nEFI interface"
TS1 p"xcomp/i386/efi/kernel.fs" bc .k
TS2 p"xcomp/amd64/efi/kernel.fs" bc .k
]ts nl>
