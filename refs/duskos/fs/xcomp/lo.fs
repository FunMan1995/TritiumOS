
create HERESTART
: variable create $0 , ;
: SYSDICT SYSVARS $c + ;
: e>xt $4 + ;
: e>wlen $1 - ;
: sysdict SYSDICT @ ;
: current sysdict e>xt ;
: ' word SYSDICT @ findentry $4 + ;
variable HBANKIDX
create HBANK
$0 , $0 , $0 , $0 , $0 , $0 , $0 , $0 ,
$0 , $0 , $0 , $0 , $0 , $0 , $0 , $0 ,
: b3:0 $f and ;
: tuck swap over ;
: hbank' b3:0 $2 lshift HBANK + ;
: hbank@ hbank' @ ;
: hbank! HBANKIDX @ $1 + dup $4 rshift + b3:0 dup HBANKIDX ! tuck hbank' ! ;
: immediate SYSDICT @ $1 - dup c@ $80 or swap c! ;
: here HERE @ ;
: here# alignhere here ;
: halerr ; $0 ,
: = - not ;
: <> - bool ;
