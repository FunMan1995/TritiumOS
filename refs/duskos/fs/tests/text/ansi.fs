needs tests/harness tests/gridh io/stream io/grid text/ansi mem/stream
testbegin

\test hello
8 4 newansigrid const g
: CSI ESC g putc '[' g putc ;
"hello" g puts
g expectgrid
hello   |
        |
        |
        |

\test CUB
CSI "Da!" g puts
g expectgrid
hella!  |
        |
        |
        |

\test CUB with arg
CSI "4Ddonistic" g puts
g expectgrid
hedonist|
ic      |
        |
        |

\test CUU
CSI "Axagon" g puts
g expectgrid
hexagont|
ic      |
        |
        |

\test CUD with arg
CSI "9BX" g puts
g expectgrid
hexagont|
ic      |
        |
       X|

\test CUP
CSI "3;4HY" g puts
g expectgrid
hexagont|
ic      |
   Y    |
       X|

\test ED arg=2
CSI "2Jhey" g puts
g expectgrid
hexagont|
ic      |
   Yhey |
        |

\test ED arg=1
CSI "3D" g puts
CSI "1Jr" g puts
g expectgrid
        |
        |
    rey |
        |

\test ED with no arg
CSI "Jclear!" g puts
g expectgrid
        |
        |
     cle|
ar!     |

\test EL arg=1
CSI "1Klnclr" g puts
g expectgrid
        |
        |
     cle|
   lnclr|

\test EL no arg
CSI "3D" g puts
CSI "K" g puts
g expectgrid
        |
        |
     cle|
   ln   |

\test EL arg=2
CSI "D" g puts
CSI "2K" g puts
g expectgrid
        |
        |
     cle|
        |

\test SGR set FG
$42 g to color
CSI "33m" g puts
g color $4b #eq

\test SGR set BG
CSI "47m" g puts
g color $7b #eq

\test SGR arg=1
CSI "1m" g puts
g color INVERTEDCOLOR #eq

\test SGR arg=0
CSI "m" g puts
g color DEFAULTCOLOR #eq

\test ignore CSI ? ...
g clear
"hello" g puts
CSI "?2004h" g puts
g expectgrid
hello   |
        |
        |
        |

\test insertlines
CSI "2Lfoo" g puts
g expectgrid
foo     |
        |
hello   |
        |

\test deletelines
CSI "M" g puts
g expectgrid
        |
hello   |
        |
        |


\test and now, the encode logic
8 4 newgrid const g
64 newmemstreambuf const s
:> s write# ; ' ansirtype realias
g overridegrid
refreshgrid
restoregrid
\ A fresh empty grid should result in a CSI H, then SGR, then 32x SPCs
: esc, ESC c, ;
create expected 41 c, esc, ,"[1;1H" esc, ,"[m                                "
expected s writtenrange s[]= #true
s rewind

\test refreshgrid after single change
2 3 g seekxy
'X' g putc
create expected 10 c, esc, ,"[4;3H" esc, ,"[mX"
g overridegrid
refreshgrid
restoregrid
s writtenrange cspit[]
expected s writtenrange s[]= #true
s rewind

restoregrid
testend
