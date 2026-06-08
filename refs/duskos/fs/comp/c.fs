needs lib/str lib/fmt mem/arena \
      comp/c/tok comp/c/pp comp/c/type comp/c/stmt
unit comp/c

: :c cctok$ cparse ;
: cc$ tarena1 reset ; cc$
: _ begin ( ) ?tok< while tokstepback cparse repeat ;
: cc<<
  cc$ cctok$
  word dup curfile! NEXTWORD !
  ['] _ exec<< ;
