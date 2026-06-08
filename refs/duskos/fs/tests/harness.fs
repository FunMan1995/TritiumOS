needs fs/core fs/sh hal/opq lib/str lib/psrs lib/diag lib/coop
unit tests/harness

: #true ( f -- ) not if abort"assertion failed" then ;
alias #true #
: #eq ( n n -- ) 2dup = if 2drop else swap .x ." != " .x abort then ;
: #hal= ( op op -- ) over (bank over (bank #eq 0 slot) swap 0 slot) #eq ;

create _buf $100 allot
variable _ptr
: _rtype _ptr @ swap cmove+ _ptr ! ;

\ exec>str is called with one word to call with exec>str on. It returns the
\ captured string. $ff bytes max.
: exec>str ( -- str )
  ' _buf 1+ _ptr ! ['] _rtype RTYPE @! >r
  execute r> RTYPE ! _ptr @ _buf - 1- ( sz )
  _buf c! _buf ;

: #s= ( s1 s2 -- ) 2dup s= if 2drop else swap stype ." != " stype abort then ;
: #s[]= ( str a u -- )
  >r 2dup r@ s[]= if rdrop 2drop else swap stype ." != " r> rtype abort then ;

\ expectabort: don't use inside exec<, it's not pretty
variable _oldabort
variable _startscnt
variable _startrcnt
: _restore _oldabort @ ABORTPTR ! ;
: _run1 run1 ; \ needs this on ARM to have "_jump" in call stack
: _jump _run1 _restore abort"abort expected" ;
: _fakeabort
  ."\naborted as expected!\n"
  _restore
  \ After an abort, PS is in an unpredictable state. Let's always empty it.
  scnt _startscnt @ - max0 ndrop
  \ It's not easy to unwind the call stack to where we were. Hackish but works
  rcnt begin popret ['] _jump - $20 < until rcnt = if
    \ rcnt hasn't budged? we're on a CPU where popret doesn't pop from RSP!
    \ restore rcnt too
    begin rcnt _startrcnt @ > while rdrop repeat then ;
: expectabort ['] _fakeabort ABORTPTR @! _oldabort ! _jump ;

createapplication Expecter
0 value evcnt
:> doto evcnt 1+ | ; Expecter dispatch!
Expecter newcontext const expecterctx

: expectevent ( xt -- )
  0 to evcnt
  doto rootcontext expecterctx | >r execute r> to rootcontext
  evcnt not ?abort"no event fired"
  evcnt 1 > ?abort"too many events fired" ;

: scntneutral# scnt _startscnt @ <> ?abort"PS imbalance" ;

0 value testcnt
0 value gtestcnt
: \test scntneutral# ."test " doto testcnt 1+ dup | . doto gtestcnt 1+ |
        begin eol? not while word spc> stype repeat nl> ;

: testbegin scnt _startscnt ! rcnt _startrcnt ! 0 to testcnt idle ;
: testend ."tests passed\n" .S nl> .free nl> scntneutral# idle ;

: rundir
  ."Running tests in " .walkpath nl>
  enterdir begin gotonext while
  walkdir? if ."Opening dir " .walkpath nl> walk>r rundir r>walk else
    ".FS" walkname upstr endswith? if
      ."Running test file " .walkpath nl>
      walk>r ['] interpret open exec< r>walk then then repeat ;
: rundir<< word lookup# rundir ;
