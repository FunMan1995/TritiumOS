\ Cross-arch labels and flow
unit asm/label

0 value org
0 value binstart
\ Short-lived labels that we need from time to time
0 value L1
0 value L2
0 value L3
0 value L4

: resetasm 0 to org 0 to binstart ;
: addr>pc org - binstart + ;
: pc here addr>pc ;
: pc>addr ( pc -- a ) org + binstart - ;

: abs>rel ( pc -- rel32 ) pc - ;

: forward here $20000 ;
: forward16 here $200 ;
: forward8 here $20 ;
