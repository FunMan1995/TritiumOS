1 here# ! here c@ not [if]
  ."num/chacha doesn't work on big endian yet, skipping\n" \s [then]

needs tests/harness num/chacha mem/stream fs/sh
testbegin

400 allot@ const PLAINTEXT
400 allot@ const CIPHERTEXT

\test quarter round on simple
code rowqround ( a -- )
  0 1 2 3 qround@, drop, exit,
create TST_VECTOR $11111111 , $01020304 , $9b8d6f43 , $01234567 ,
create expected   $ea2a92f4 , $cb1cf8ce , $4581472e , $5881c4bb ,

TST_VECTOR rowqround
TST_VECTOR expected 4 []= #true

\test newChaCha20
newChaCha20 const CHACHA_S
CHACHA_S 3 and       0 #eq \ address is aligned to 4
CHACHA_S cntrnonce @ 1 #eq \ counter is set to 1
CHACHA_S rounds     20 #eq \ rounds is set to 20
CHACHA_S remaining   0 #eq \ there's no remaining byte yet

\test setup
create TST_NONCE map< c, $00 $00 $00 $09 $00 $00 $00 $4a $00 $00 $00 $00
create CHK_NONCE map< , $09000000 $4a000000 $00000000
create TST_KEY map< c, $00 $01 $02 $03 $04 $05 $06 $07 \
                       $08 $09 $0a $0b $0c $0d $0e $0f \
                       $10 $11 $12 $13 $14 $15 $16 $17 \
                       $18 $19 $1a $1b $1c $1d $1e $1f
create CHK_KEY map< , $03020100 $07060504 $0b0a0908 $0f0e0d0c \
                      $13121110 $17161514 $1b1a1918 $1f1e1d1c

12 allot@ const COPY_NONCE
TST_NONCE COPY_NONCE 12 cmove
32 allot@ const COPY_KEY
TST_KEY COPY_KEY 32 cmove

TST_NONCE TST_KEY CHACHA_S setup

create zeroes 16 4* allot0
TST_NONCE zeroes 3 []= #true
TST_KEY zeroes 8 []= #true
CHK_NONCE CHACHA_S cntrnonce 4+ 3 []= #true
CHK_KEY CHACHA_S symkey 8 []= #true
COPY_KEY TST_KEY 32 cmove

\test round
\ note: CHACHA_S has been set with the correct keys and nonce before
CHACHA_S _loadstate drop
create PREV_STATE map< , $61707865 $3320646e $79622d32 $6b206574 \
                         $03020100 $07060504 $0b0a0908 $0f0e0d0c \
                         $13121110 $17161514 $1b1a1918 $1f1e1d1c \
                         $00000001 $09000000 $4a000000 $00000000
create THEN_STATE map< , $837778ab $e238d763 $a67ae21e $5950bb2f \
                         $c4f2d0c7 $fc62bb2f $8fa018fc $3f5ec7b7 \
                         $335271c2 $f29489f3 $eabda8fc $82e46ebd \
                         $d19c12b4 $b04e16de $9e83d0cb $4e3c50a2

PREV_STATE CHACHA_S wstate 16 []= #true
CHACHA_S round
THEN_STATE CHACHA_S wstate 16 []= #true

\test ChaCha20 block function
create THEN_STATE map< ,  $e4e7f110 $15593bd1 $1fdd0f50 $c47120a3 \
                          $c7f4d1c7 $0368c033 $9aaa2204 $4e6cd4c3 \
                          $466482d2 $09aa9f07 $05d7c214 $a2028bd9 \
                          $d19c12b5 $b94e16de $e883d0cb $4e3c50a2
CHACHA_S _loadstate blockfn
THEN_STATE CHACHA_S wstate 16 []= #true

\test ChaCha close
CHACHA_S close
\ check the inner state of CHACHA_S
zeroes CHACHA_S wstate 16 []= #true
CHACHA_CST CHACHA_S bstate 4 []= #true
zeroes CHACHA_S bstate 4 4* + 8 []= #true
zeroes CHACHA_S cntrnonce 4+ 3 []= #true
CHACHA_S cntrnonce le@ 1 #eq
CHACHA_S remaining 0 #eq

\test ChaCha20 cipher
: readallclose<< ( dst "name" -- len ) -1 word openpath r! read r> close ;

create TST_NONCE map< c,  $00 $00 $00 $00 $00 $00 $00 $4a $00 $00 $00 $00

PLAINTEXT readallclose<< data/tests/chacha/plain1.txt const TEXT_LEN
CIPHERTEXT readallclose<< data/tests/chacha/cipher1.bin TEXT_LEN #eq

newChaCha20 const CHACHA_S
PLAINTEXT TEXT_LEN newmemstream CHACHA_S to wrappedstream
TST_NONCE TST_KEY CHACHA_S setup
TEXT_LEN CHACHA_S readbuf dup TEXT_LEN #eq ( a u )
CIPHERTEXT swap c[]= #true

create TST_KEY map< c, $00 $00 $00 $00 $00 $00 $00 $00 $00 $00 $00 $00 $00 $00 \
                       $00 $00 $00 $00 $00 $00 $00 $00 $00 $00 $00 $00 $00 $00 \
                       $00 $00 $00 $01
create TST_NONCE map< c, $00 $00 $00 $00 $00 $00 $00 $00 $00 $00 $00 $02

PLAINTEXT readallclose<< data/tests/chacha/plain2.txt const TEXT_LEN
CIPHERTEXT readallclose<< data/tests/chacha/cipher2.bin TEXT_LEN #eq
PLAINTEXT TEXT_LEN newmemstream const STREAM

STREAM CHACHA_S to wrappedstream
CHACHA_S close
TST_NONCE TST_KEY CHACHA_S setup
TEXT_LEN CHACHA_S readbuf
CIPHERTEXT swap c[]= #true
CHACHA_S close

\test ChaCha streaming capabilities
\ NOTE: we must reset key and nonce everytime because they're zeroed out for
\ security purposes
create TST_KEY map< c, $00 $00 $00 $00 $00 $00 $00 $00 $00 $00 $00 $00 $00 $00 \
                       $00 $00 $00 $00 $00 $00 $00 $00 $00 $00 $00 $00 $00 $00 \
                       $00 $00 $00 $01
create TST_NONCE map< c, $00 $00 $00 $00 $00 $00 $00 $00 $00 $00 $00 $02
PLAINTEXT TEXT_LEN newmemstream const STREAM
STREAM CHACHA_S to wrappedstream \ we modify PLAINTEXT
TST_NONCE TST_KEY CHACHA_S setup
308 CHACHA_S readbuf 308 #eq drop
CHACHA_S close

create TST_KEY map< c, $00 $00 $00 $00 $00 $00 $00 $00 $00 $00 $00 $00 $00 $00 \
                       $00 $00 $00 $00 $00 $00 $00 $00 $00 $00 $00 $00 $00 $00 \
                       $00 $00 $00 $01
create TST_NONCE map< c, $00 $00 $00 $00 $00 $00 $00 $00 $00 $00 $00 $02
CIPHERTEXT TEXT_LEN newmemstream const STREAM
STREAM CHACHA_S to wrappedstream
TST_NONCE TST_KEY CHACHA_S setup
70 CHACHA_S readbuf 70 #eq drop
14 CHACHA_S readbuf 14 #eq drop
82 CHACHA_S readbuf 82 #eq drop
23 CHACHA_S readbuf 23 #eq drop
53 CHACHA_S readbuf 53 #eq drop
66 CHACHA_S readbuf 66 #eq drop
\ assert that we've read the same quantity of data
70 14 + 82 + 23 + 53 + 66 + 308 #eq

PLAINTEXT CIPHERTEXT 308 c[]= #true

\test XChaCha construction
create TST_KEY map< c, $00 $01 $02 $03 $04 $05 $06 $07 \
                       $08 $09 $0a $0b $0c $0d $0e $0f \
                       $10 $11 $12 $13 $14 $15 $16 $17 \
                       $18 $19 $1a $1b $1c $1d $1e $1f
create TST_NONCE map< c, $00 $00 $00 $09 $00 $00 $00 $4a \
                         $00 $00 $00 $00 $31 $41 $59 $27 \
                         $40 $41 $42 $43 $44 $45 $46 $47
create EXP_STATE map< , $61707865 $3320646e $79622d32 $6b206574 \
                        $423b4182 $fe7bb227 $50420ed3 $737d878a \
                        $d5e4f9a0 $53a8748a $13c42ec1 $dcecd326 \
                        $00000001 $00000000 $43424140 $47464544
20 XChaCha newChaCha const CHACHA_S
TST_NONCE TST_KEY CHACHA_S setup
EXP_STATE CHACHA_S bstate 16 []= #

\test XChaCha20 cipher
create TST_KEY map< c,
  $80 $81 $82 $83 $84 $85 $86 $87 $88 $89 $8a $8b $8c $8d $8e $8f \
  $90 $91 $92 $93 $94 $95 $96 $97 $98 $99 $9a $9b $9c $9d $9e $9f

create TST_NONCE map< c,
  $40 $41 $42 $43 $44 $45 $46 $47 $48 $49 $4a $4b $4c $4d $4e $4f \
  $50 $51 $52 $53 $54 $55 $56 $58

PLAINTEXT readallclose<< data/tests/chacha/plainx.txt const TEXT_LEN
CIPHERTEXT readallclose<< data/tests/chacha/cipherx.bin TEXT_LEN #eq

20 XChaCha newChaCha const CHACHA_S
TST_NONCE TST_KEY CHACHA_S setup
PLAINTEXT TEXT_LEN newmemstream CHACHA_S to wrappedstream
TEXT_LEN CHACHA_S readbuf TEXT_LEN #eq drop

PLAINTEXT CIPHERTEXT TEXT_LEN c[]= #true

testend
