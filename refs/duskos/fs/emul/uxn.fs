needs hal/opq hal/vreg lib/ival lib/str lib/diag asm/uxntal
unit emul/uxn

\ When we execute uxn ops, we replace PSP with uxn's, use A as RSP and R0 as PC.
\ We can't use our real RSP because of HAL's alignment constraints.
\ stacks pointers go down
\ Note: we intentionally do not implement "wrap" logic, that is, 16b accessed to
\ "$ff memory", where we're supposed to detect that condition and wrap the lo
\ byte to 0. It is costly and I don't think that any ROM takes advantage of it.

0 value uxn \ always aligned to $100 bytes
: uxn# uxn not ?abort"uxn VM not created" ;

addrof uxn ivalmap {
  +$00100 [void,0] rstvec ;
  +$10000 [void,0] devices ;
  +$10200 [void,0] wst ;
  +$10300 [void,0] rst ;
  uint wptr rptr deohandlers ;
}
\ deohandlers: ptr to an array of $100 4b pointer to ( -- ) words, or 0 for noop
$1030c const UXNSZ

: uxn$ wst to wptr rst to rptr
       uxn $40 0 fill devices $40 0 fill ;
: newuxn ( deohandlers -- )
  $100 alignheren here to uxn UXNSZ 4- allot0 , uxn$ ;

create ops $100 4* allot0
code nextop R0) 8b) @+, 2 i) <<, W) ops +) br,
: opcode ( n -- ) 4* ops + code> swap ! ;
: op# ( str -- opcode ) ?op not ?abort"invalid opcode" ;
: opcode" [rcompile] " op# opcode ;

PSP) value st)
A) value other)
: regstack PSP) to st) A) to other) ;
: revstack A) to st) PSP) to other) ;
: uxn+, addrof uxn m) +, ; \ Preserves A
: uxn-, addrof uxn m) -, ; \ Preserves A
: dev+, uxn+, offsetof devices i) +, ;
:~ dup, addrof uxn m) @, W) n< offsetof devices + +) ;
\ for use in DEO handlers.
: [dev@] ~ 8b) @, ; immediate
: [dev2@] ~ 16b) be@, ; immediate
:~ addrof uxn m) S>) @, S) n< offsetof devices + +) ;
: [dev!] ~ 8b) !, drop, ; immediate
: [dev2!] ~ 16b) be!, drop, ; immediate
: [devk!] ~ 8b) !, ; immediate
: [dev2k!] ~ 16b) be!, ; immediate
: stswap, \ preserves A but *not* W
  offsetof wptr i) @, uxn+, \ W=wptr
  PSP) &) S>) @, W) S>) @!, PSP) &) S>) !,
  4 i) +, \ W=rptr
  W) A>) @!, ;
code uxnon ( pc -- pc ) pushlr, R0) &) !, stswap, ' nextop bbr,
opcode"BRK" ( -- pc ) stswap, R0) &) @, popexit,

\ Ops signature: ( -- ) \ A=PC
: nget8, ( n -- ) st) swap +) 8b) @, ;
: peek8, 0 nget8, ;
: pop8, st) 8b) @+, ;
: set8, st) 8b) !, ;
: push8, st) 8b) -!, ;
: push8r, other) 8b) -!, ;
: nget16, ( n -- ) st) swap +) 16b) le@, ;
: peek16, 0 nget16, ;
: pop16, peek16, 2 st) &) +n, ;
: set16, st) 16b) le!, ;
: push16, -2 st) &) +n, set16, ;
: push16r, -2 other) &) +n, other) 16b) le!, ;
: sex8, 24 i) <<, 24 i) signed) >>, ;
: sex16, 16 i) <<, 16 i) signed) >>, ;
: pc8+, sex8, R0) &) +, ;
: W>S, S) &) !, ; : S>W, S) &) @, ; : W<>S, S) &) @!, ;

\ keep stuff
0 value keep?
0 value keepidx
: keep! 0 to keepidx 1 to keep? ;
: unkeep! 0 to keepidx 0 to keep? ;
: ?pop8, keep? if keepidx nget8, doto keepidx 1+ | else pop8, then ;
: ?pop16, keep? if keepidx nget16, doto keepidx 2+ | else pop16, then ;
: ?push8, keep? if push8, else set8, then ;
: ?push16, keep? if push16, else set16, then ;
: ?st+n, keep? if drop else st) &) +n, then ;
: keepst) st) keepidx +) ;
: ?@+, keep? if dup (sz doto keepidx + | @, else @+, then ;

\ DEO stuff
sysvar savedpc) SAVEDPC
sysvar curport) CURPORT
sysvar curhandlers) CURHANDLERS
: ?calldeo, \ W holds the port
  $ff i) &, 2 i) <<, curhandlers) +, 0 W) +n, ifnz,
    W) @, savedpc) !,
    stswap, savedpc) R0>) @!, R0) &) brr, savedpc) R0>) @, stswap,
    [compile] then ;
: popport, ?pop8, curport) !, ;

: nextop, ['] nextop bbr, ;
:~ execute nextop, ;
: _pre ( -- xt opcode )
  [rcompile] " op# :> ( opcode xt ) over opcode tuck ~ ( xt opcode ) ;
: :op" ( -- )
  _pre revstack $40 or ( "r" variant ) opcode ~ regstack ;
: :opk" ( -- )
  _pre tuck $80 or ( "k" variant ) keep! opcode tuck ~ unkeep!
  revstack
  tuck $40 or ( "r" variant ) opcode tuck ~
  $c0 or ( "kr" variant ) keep! opcode ~ unkeep!
  regstack ;

:op"INC" 1 st) 8b) +n, ;
:op"INCk" peek8, 1 i) +, push8, ;
:opk"INC2" peek16, 1 i) +, ?push16, ;
:opk"POP" 1 ?st+n, ;
:opk"POP2" 2 ?st+n, ;
:opk"NIP" ?pop8, ?push8, ;
:opk"NIP2" ?pop16, ?push16, ;
:opk"SWP" st) 16b) be@, ?push16, ;
:op"SWP2" st) 16b) S>) le@, 2 nget16, set16, st) 2 +) 16b) S>) le!, ;
:op"SWP2k" peek16, push16, 4 nget16, push16, ;
:op"ROT" peek8, st) 1 +) 8b) @!, st) 2 +) 8b) @!, set8, ;
:op"ROTk" peek16, push16, 4 nget8, push8, ;
:op"ROT2"
  peek8, st) 2 +) 8b) @!, st) 4 +) 8b) @!, set8,
  1 nget8, st) 3 +) 8b) @!, st) 5 +) 8b) @!, st) 1 +) 8b) !, ;
:op"ROT2k" 2 nget16, push16, 2 nget16, push16, 12 nget16, push16, ;
:op"DUP" peek8, push8, ;
:op"DUPk" peek8, push8, push8, ;
:op"DUP2" peek16, push16, ;
:op"DUP2k" peek16, push16, push16, ;
:op"OVR" 1 nget8, push8, ;
:op"OVRk" peek16, push16, 8 i) >>, push8, ;
:op"OVR2" 2 nget16, push16, ;
:op"OVR2k" 2 nget16, S) &) !, push16, 2 nget16, push16, S) &) @, push16, ;
:opk"JMP" ?pop8, sex8, W) &) R0>) +, ;
:opk"JMP2" ?pop16, uxn+, W) &) R0>) @, ;
:opk"JCN" 1 nget8, 0 i) <>) if,
    peek8, sex8, W) &) R0>) +, [compile] then 2 ?st+n, ;
:opk"JCN2" 2 nget8, 0 i) <>) if,
    peek16, uxn+, W) &) R0>) @, [compile] then 3 ?st+n, ;
:opk"JSR" ?pop8, pc8+, R0) &) @!, uxn-, push16r, ;
:opk"JSR2" ?pop16, uxn+, R0) &) @!, uxn-, push16r, ;
:opk"STH" ?pop8, push8r, ;
:opk"STH2" ?pop16, push16r, ;
: ld, W) 8b) @, ;
:opk"LDZ" peek8, uxn+, ld, ?push8, ;
:opk"LDA" peek16, uxn+, ld, 1 ?st+n, ?push8, ;
:opk"LDR" peek8, pc8+, ld, ?push8, ;
:opk"DEI" peek8, dev+, ld, ?push8, ;
: ld, W) 16b) be@, ;
:opk"LDZ2" peek8, uxn+, ld, -1 ?st+n, ?push16, ;
:opk"LDA2" peek16, uxn+, ld, ?push16, ;
:opk"LDR2" peek8, pc8+, ld, -1 ?st+n, ?push16, ;
:opk"DEI2" peek8, dev+, ld, -1 ?st+n, ?push16, ;
: st, keepst) 8b) S>) ?@+, W) 8b) S>) !, ;
:opk"STZ" ?pop8, uxn+, st, ;
:opk"STA" ?pop16, uxn+, st, ;
:opk"STR" ?pop8, pc8+, st, ;
:opk"DEO" popport, dev+, st, ?calldeo, ;
: st, keepst) 16b) S>) le@, W) 16b) S>) be!, 2 ?st+n, ;
:opk"STZ2" ?pop8, uxn+, st, ;
:opk"STA2" ?pop16, uxn+, st, ;
:opk"STR2" ?pop8, pc8+, st, ;
:opk"STR2" ?pop8, pc8+, st, ;
:opk"DEO2" popport, dev+, st, 1 i) +, ?calldeo, ;

: mulsave, R2) ?saveR0, *, R2) ?restoreR0, ;
: divsave, R2) ?saveR0, /mod, R2) ?restoreR0, ;

: ari8, ( op -- )
  ?pop8, keep? if
  W>S, 1 nget8, S) &) swap execute push8,
  else st) 8b) dir) swap execute then ;
:opk"ADD" ['] +, ari8, ;
:opk"SUB" ['] -, ari8, ;
:opk"MUL" ['] mulsave, ari8, ;
:opk"AND" ['] &, ari8, ;
:opk"ORA" ['] |, ari8, ;
:opk"EOR" ['] ^, ari8, ;
:opk"DIV"
  keep? if peek16, push16, then
  pop8, 0 i) <>) if, st) 8b) dir) divsave, [compile] else
  set8, [compile] then ;

: ari16, ( op -- ) ?pop16, keepst) 16b) S>) le@, S) &) swap execute ?push16, ;
:opk"ADD2" ['] +, ari16, ;
:opk"SUB2" ['] swap-, ari16, ;
:opk"MUL2" ['] *, ari16, ;
:opk"AND2" ['] &, ari16, ;
:opk"ORA2" ['] |, ari16, ;
:opk"EOR2" ['] ^, ari16, ;
:opk"DIV2"
  ?pop16, 0 i) <>) if,
    W>S, keepidx nget16, S) &) divsave, [compile] then
  ?push16, ;

: sft,
  st) 8b) S>) @, $f i) S>) &, S) &) >>,
  st) 8b) S>) ?@+, 4 i) S>) >>, S) &) <<, ;
:opk"SFT" 1 nget8, sft, ?push8, ;
:opk"SFT2" 1 nget16, sft, ?push16, ;

: cmp8, ( cond -- ) ?pop8, keepst) 8b) swap bool, ?push8, ;
:opk"EQU" =) cmp8, ;
:opk"NEQ" <>) cmp8, ;
:opk"GTH" <) cmp8, ;
:opk"LTH" >) cmp8, ;

: cmp16, ( cond -- )
  st) 2 +) 16b) S>) le@, peek16, S) &) swap bool,
  keep? if push8, else 3 st) &) +n, set8, then ;
:opk"EQU2" =) cmp16, ;
:opk"NEQ2" <>) cmp16, ;
:opk"GTH2" <) cmp16, ;
:opk"LTH2" >) cmp16, ;

:~ R0) 8b) @+, push8, ;
opcode"LIT" ~ nextop,
opcode"LITr" revstack ~ regstack nextop,
:~ R0) 8b) @, push8,
   R0) 1 +) 8b) @, push8,
   2 i) R0>) +, ;
opcode"LIT2" ~ nextop,
opcode"LIT2r" revstack ~ regstack nextop,
:~ R0) 16b) be@, sex16, ;
opcode"JCI"
  pop8, 0 i) <>) if, ~ W) &) R0>) +, then
  2 i) R0>) +, nextop,
opcode"JMI" ~ 2 i) R0>) +, W) &) R0>) +, nextop,
opcode"JSI" ~ 2 i) R0>) +, R0) &) +, R0) &) @!, uxn-, push16r, nextop,

: runat ( pc -- pc ) uxn# deohandlers CURHANDLERS ! uxnon ;
: poweron ( -- ) rstvec runat drop ;

: wstcnt wst wptr - $100 min ;
: rstcnt rst rptr - $100 min ;
: .uxn ( -- )
  ."W " wptr wstcnt cspit[] nl>
  ."R " rptr rstcnt cspit[] nl> ;
