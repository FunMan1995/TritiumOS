needs lib/type lib/struct lib/psrs mem/range \
      comp/tok comp/sig comp/sym comp/c/glob
unit comp/c/type

:~ word newint repeatword addtype ;
' @, 1 4 ~ long
' @, 0 4 ~ flexuint
' @, 1 4 ~ flexint
0 const NULL

: ?newstructure ( name -- )
  ?dup if dup ?placeholder if drop else
    NEXTWORD ! 0 newstructure dup cur ! addtype then
    else 0 newstructure cur ! then ;

alias abort parsetype ( -- ?type f ) \ forward declaration
alias abort decl ( type -- type name ) \ forward declaration
: parsetype# ( -- type ) parsetype not ?err"type expected" ;
: parsetypeorforward ( -- type )
  parsetype not if cur @ curtok ?newstructure cur @! then ;
: readlvl ( type -- type ) '*' readChar? if newpointer readlvl then ;

: topfieldsz ( struct -- n )
  0 swap structfields @ begin ?dup while ( n ll )
    tuck 4+ @ typesz max swap @ repeat ;

: _struct ( union? -- type )
  cur @ >r >r readIdent? not if 0 then \ V1=oldstruct V2=union?
  '{' readChar ?newstructure begin ( )
    parsetypeorforward begin ( type )
      V2 if 0 curoff ! then
      dup decl NEXTWORD ! addfield drop ( type )
      ',' readChar? not until ( type )
    drop read; '}' readChar? until ( )
  r> ( union? ) if cur @ topfieldsz else curoff @ then ( n )
  align4 curoff ! r> cur @! ;

stringlist baseints long int short char
create unsigned uint , uint , ushort , uchar ,
create signed int , int , short , char ,
: baseint< ( -- idx ) tok< baseints sfind not ?err"base int expected" ;

stringlist structkws struct union
:realias parsetype ( -- type? f )
  tok< case ( )
    "unsigned" s= of baseint< 4* unsigned + @ 1 endof
    "signed" s= of baseint< 4* signed + @ 1 endof
    findtype ?dup of 1 endof
    structkws sfind of ( idx ) _struct 1 endof
    drop 0 endcase ;

: _funcargs ( type name addsym? -- type name ) \ opening '(' is read
  >r >r >r 0 begin ( ... n V1=addsym? V2=name V3=sig )
    tok< "..." s= not while tokstepback
    ')' readChar? not while
    parsetypeorforward decl
    V1 if dipdup addargument then drop ( ... n type )
    swap 1+ ( ... n ... n )
    ',' readChar? while repeat
    read) else then 0 else read) 1 then ( ... n vararg? )
  >r r! ps[] swap[] r> r> \ reverse arg order
  r> dup void? if drop 0 swap else 1 rot then ( ... n ... n varargs? )
  256* newsignature r> rdrop ;

:realias decl ( type -- type name )
  readlvl
  '(' readChar? if
    '*' readChar? if tok< expectIdent else NULLSTR then
    read) read( 0 _funcargs exit then
  readIdent? not if NULLSTR then ( type name )
  '[' readChar? if
    parseConstExpr ']' readChar rot newarray swap then ( type name )
  '(' readChar? if symbols$ 1 _funcargs then ( type name ) ;

