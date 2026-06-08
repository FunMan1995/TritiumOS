needs tests/harness mem/stream io/stream emul/6502 emul/virtio \
      asm/6502 asm/label
testbegin
0 to binstart here# to org
$200 allot \ pages 0 and 1
\ A routine that loops over its serial input, add 1 to each character it reads
\ then spits it back, then BRK.
pc \ mainloop
  pc $fc <0+> LDA, ( pc ) abs>rel BEQ, \ wait for contents
  $fd <0+> LDX, 0 # LDA, $fc <0+> STA,
  INX, $f9 <0+> STX, 1 # LDA, $f8 <0+> STA,
  BRK, JMP,

\ VirtIO cmd area is ZP+f8
$100 newmemstreambuf const myout
org new6502 const cpu
0 myout org $f8 + cpu newvirtio const vio
resetasm

: check ( refstr inputstr -- )
  vio puts
  myout writtenrange s[]= #true
  myout rewind ;
"Ifmmp!Xpsme\"" "Hello World!" check
\ Let's try another run to see if the BRK/unhalt combo works
"gpp" "foo" check
testend
