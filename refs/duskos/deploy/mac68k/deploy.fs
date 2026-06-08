: deploy ( blk -- )
  dup mkfloppyfat dup install
  walkdst newfatfs walk copyfloppy ;
