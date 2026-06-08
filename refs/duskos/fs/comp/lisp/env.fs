needs lib/str
unit comp/lisp/env
\ The "environment" is a string list that represents a mapping to runtime PS.
\ For example, in an environment "foo bar baz", "baz" maps to W, "bar" to PSP+0
\ and "foo" to PSP+4. Nested functions will remember environment position before
\ they add their own local variables to it, allowing it to unwind afterwards.
\ Strings are stored "backwards" from the top of the buffer so that lookup
\ happen in the correct order.
$200 const MAXENVSZ
create env MAXENVSZ allot 0 c,
here 1- const envtop
envtop value envtail
: envadd ( s -- )
  dup c@ 1+ doto envtail swap- dup | env < ?abort"env overflow" ( s )
  envtail strmove ;
: ?local ( s -- psidx-or-s f ) dup envtail sfind if nip 1 else 0 then ;
: env$ envtop to envtail ;
: .env envtail begin dup c@ while dup stype spc> s) repeat drop ;
