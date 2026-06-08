needs fs/sh fs/fat

: cp ( drv "name" -- )
  newfatfs bootfs!
  word ensurefile open ( file )
  dup stdio spit dup truncate close ;
