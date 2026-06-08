needs tests/harness lib/struct lib/ival
testbegin

\test ivalue
create data 12 , 42 , 54 ,
create ptr data ,
ptr uint ivalue myval
myval 12 #eq
:~ myval ; ~ 12 #eq
15 to myval \ replaces "12" in data with "15"
data @ 15 #eq
4 ptr +!
myval 42 #eq
~ 42 #eq

\test ivalmap
: 42+ 42 + ;
create data 42 , $1234 wle, $5678 wbe, ' 42+ ,
create ptr data ,
ptr ivalmap {
  uint mydword ;
  leshort mywle ;
  beshort mywbe ;
  xt myxt ;
  [void,0] myarray ;
}
mydword 42 #eq
:~ 54 to mydword ; ~
data @ 54 #eq
mywle $1234 #eq
mywbe $5678 #eq
2 myxt 44 #eq
:> 54 + ; to myxt
2 myxt 56 #eq
:~ 3 myxt 1+ ; ~ 58 #eq
myarray data 12 + #eq

create newdata 123 ,
newdata ptr !
mydword 123 #eq

\test absvalmap
data absvalmap { uint absdword ; }
absdword 54 #eq

\test addrof on IVAL
addrof mywle newdata 4+ #eq
:~ addrof mywbe ; ~ newdata 6 + #eq
data ptr ! ~ data 6 + #eq
addrof absdword data #eq

\test addrof on VALU
42 value myval
:~ addrof myval ;
~ @ 42 #eq
54 ~ ! myval 54 #eq

\test addrof on LVAR
:~ 42 >r 3 addrof V1 +! r> ; ~ 45 #eq

\test doto
:~ doto absdword 1+ | ;
~ absdword 55 #eq
~ absdword 56 #eq

\test doto on fields
struct MyStruct { uint foo bar ; }
create data 42 , 54 ,
:~ data doto bar 8+ | ; ~
data bar 62 #eq
testend
