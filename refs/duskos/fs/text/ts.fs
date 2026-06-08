needs lib/str io/stream mem/range
unit text/ts

0 value tspos
variable _oldrtype

: _ts$ 0 to tspos ;
: _rtype ( a u -- )
  2dup begin 2dup LF rot> cidx while _ts$ 1+ ltrim[] repeat ( a u a u )
  doto tspos + | drop _oldrtype @ execute ;
: nspcs console 32SPCS fillstream spitn ;
: tsgo ( pos -- ) tspos - dup 0>= if nspcs else drop then ;
: ts[ ( -- oldrtype ) _ts$ ['] _rtype RTYPE @! dup _oldrtype ! ;
: ]ts ( oldemit -- ) RTYPE ! ;
: ts< ( xt -- ) ts[ >r execute r> ]ts ;
