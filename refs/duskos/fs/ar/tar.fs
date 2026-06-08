needs lib/struct
unit ar/tar

: parseoctal ( a u -- n )
  swap >A 0 swap 0 do ( res ) 8* '0' - @Ac@ 1 A+ + loop ;

struct Record {
  [uchar,100] zname ;
  [uchar,8] omode ouid ogid ; \ o prefix means "octal", ASCII octal.
  [uchar,12] ofilesz omtime ;
  [uchar,8] checksum ;
  uchar type ;
  [uchar,100] zlinkname ;
  [uchar,6] signature ;
  +$200 [void,0] endmarker ;
}

: empty? ( rec -- f ) @ not ;
: dir? type '5' = ;
: recordsize ( rec -- sz ) ofilesz 11 parseoctal ;
