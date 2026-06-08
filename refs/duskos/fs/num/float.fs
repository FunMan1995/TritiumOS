needs lib/psrs lib/str hal/float
unit num/float

: ?err ?abort"can't parse float" ;
: parsedec# parsedec not ?err ;
:~
  [rcompile] " c@+ 2dup '.' rot> cidx if ( a u idx )
    oover over parsedec# n>f >r ( a u idx V1=intpart )
    1+ consume[] ?dup if ( a u )
      tuck parsedec# n>f ( u float )
      swap neg fscale10 r> f+
      else r> then
    else ( a u ) parsedec# n>f then ;
:> ~ litn ; ' ~ compiling float"

: f. ( float digits -- )
  swap dup f>n dup . n>f f- ( digits rest )
  swap fscale10 ( rest )
  ."." f>n . ;
