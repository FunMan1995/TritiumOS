needs lib/str lib/fmt
unit comp/tok

$3f const MAXTOKSZ
create curtok MAXTOKSZ 1+ allot

\ Many tokens can be used at once, but they are not supposed to be kept in the
\ long term. When the name needs to be kept around, it must be copied elsewhere.
\ we therefore use lib/str's pool
variable curline
create curfile STR_MAXSZ allot

: curfile! curfile strmove ;
: tokdbg ."File: " curfile stype nl>
         ."Line no: " curline @ . ." token: " curtok stype nl> ;

: err" compile tokdbg [compile] abort" ; immediate
: ?err" [compile] if [compile] err" [compile] then ; immediate

: ?line+ ( c -- c ) dup LF = curline +! ;
: tonws< ( -- lastws c-or-0 ) LF begin ( lastws )
    in< dup 0>= while ?line+ dup ws? while nip repeat
      ( lastws c ) else ( lastws EOF ) drop 0 then ;

: newtok ( -- ) 0 curtok ! ;
: tokacc ( c -- )
  curtok c@+ dup MAXTOKSZ = if abort"token overflow" then ( c a u )
  + c! curtok c@ 1+ curtok c! ;
: tokdel ( -- ) curtok c@ 1- curtok c! ;
: curtokcopy curtok str>pool ;

variable nexttok \ when nonzero, it will be the next token
: tokstepback curtokcopy nexttok ! ;

: deftok<
  tonws< nip dup if ( c )
    newtok begin tokacc in< dup 1+ SPC 1+ <= until
    drop stepback curtokcopy then ;
alias deftok< (?tok<) ( -- tok-or-0 )
: ?tok< 0 nexttok @! ?dup not if (?tok<) then ;
: tok< ?tok< dup not if abort"token expected" then ;
: peektok< tok< tokstepback ;

: tok$ 0 nexttok ! 1 curline !
       "(none)" curfile strmove
       ['] deftok< ['] (?tok<) realias ;

: n>tok ( n -- tok ) "%d" sprintf str>pool dup curtok strmove ;

: isChar? ( tok c -- f ) swap A! 1+ c@ = @Ac@ 1 = and ;
