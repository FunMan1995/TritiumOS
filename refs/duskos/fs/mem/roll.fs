needs lib/struct lib/psrs io/stream
unit mem/roll

extends Stream struct RollBuf {
  uint widx wlim ridx bufsz ;
  *void buffer ;
}

: ridx>wlim ( rbuf -- wlim ) A! ridx dup A> to wlim ;
: peekbuf ( n rbuf -- ?a peek-n )
  r! ridx>wlim ( n wlim ) \ V1=stream
  r@ widx swap r@ buffer over + rot> ( n a wr rd )
  2dup >= if - else nip r@ bufsz swap- then ( n a u )
  rot min ( a u ) dup not if nip then rdrop ;
: :readbuf ( n stream -- ?a read-n )
  r! peekbuf r@ wlim over + r@ bufsz mod r> to ridx ;

: wrmax ( rbuf -- n )
  tri bufsz 1- | ridx>wlim | widx - ?dup if
    dup 0< if neg - else nip 1- then then ;
: widx+ ( n rbuf -- n ) r! widx + r> bufsz mod ;
: writeahead ( a n off rbuf -- written-n )
  r! wrmax over - max0 rot min ( a off maxn V1=rbuf )
  ?dup not if rdrop 2drop 0 exit then
  r! rswap ( V1=written-n V2=rbuf )
  swap r@ widx+ ( a n idx )
  2dup + r@ bufsz - dup 0< if drop else ( a n idx ovfl-n )
    rot over - ( a idx nleft nright )
    3 dig over + ( a idx nleft nright a+ )
    rot r@ buffer swap cmove ( a idx nright )
    swap then ( a n idx )
  r> buffer + swap cmove r> ;

: :writebuf ( a n stream -- written-n )
  r! 0 swap writeahead dup r@ widx+ r> to widx ;

: newroller ( a u -- rbuf )
  ['] :readbuf ['] :writebuf newstream
  0 , 0 , 0 , swap ( bufsz ) , swap ( buffer ) , ;
: newrollingbuffer ( bufsz -- rbuf ) dup allot@ swap newroller ;

: fastputc, ( -- br ) \ A=stream W=c
  A) offsetof widx +) S>) @, A) offsetof buffer +) S>) +, S) 8b) !,
  A) offsetof widx +) @, 1 i) +, A) offsetof bufsz +) /mod, \ S=widx
  A) offsetof wlim +) S>) <>) if, A) offsetof widx +) S>) !, ;

: reset 0 swap 2dup to widx 2dup to wlim to ridx ;

: advancewindow ( n winsz rbuf -- n )
  r! wrmax swap- max0 min dup r@ widx+ r> to widx ;
