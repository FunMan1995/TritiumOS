needs tests/harness tests/gridh io/stream io/grid
testbegin

\test fresh grid
8 4 newgrid const g
g expectgrid
        |
        |
        |
        |

\test default color fills the grid
g expectfg
ffffffff|
ffffffff|
ffffffff|
ffffffff|

g expectbg
00000000|
00000000|
00000000|
00000000|

\test cleared grid has full damage
:~ 32 0 do i g getnextchange #true i #eq SPC #eq DEFAULTCOLOR #eq loop ; ~

\test checking damage clears it
0 g getnextchange not #true

\test write something
1 2 g seekxy
"foobar" g puts
g expectgrid
        |
        |
 foobar |
        |

\test line[]
0 g line[] 0 #eq drop
" foobar" 2 g line[] #s[]=

\test check damage
0 g getnextchange #true 17 #eq 'f' #eq DEFAULTCOLOR #eq
0 g getnextchange #true 18 #eq 'o' #eq DEFAULTCOLOR #eq
0 g getnextchange #true 19 #eq 'o' #eq DEFAULTCOLOR #eq
0 g getnextchange #true 20 #eq 'b' #eq DEFAULTCOLOR #eq
0 g getnextchange #true 21 #eq 'a' #eq DEFAULTCOLOR #eq
0 g getnextchange #true 22 #eq 'r' #eq DEFAULTCOLOR #eq
0 g getnextchange not #true

\test scattered damage
3 1 g seekxy
'X' g putc
5 3 g seekxy
'Y' g putc
g expectgrid
        |
   X    |
 foobar |
     Y  |

0 g getnextchange #true 11 #eq 'X' #eq DEFAULTCOLOR #eq
0 g getnextchange #true 29 #eq 'Y' #eq DEFAULTCOLOR #eq
0 g getnextchange not #true

\test setting color affects further writes
$2a g to color
6 2 g seekxy
'z' g putc
g resetcolor

g expectgrid
        |
   X    |
 foobaz |
     Y  |
0 g getnextchange #true 22 #eq 'z' #eq 42 #eq
0 g getnextchange not #true

g expectfg
ffffffff|
ffffffff|
ffffffaf|
ffffffff|

g expectbg
00000000|
00000000|
00000020|
00000000|

\test BS
1 2 g seekxy
'b' g putc
g expectgrid
        |
   X    |
 boobaz |
     Y  |

BS g putc
g expectgrid
        |
   X    |
  oobaz |
     Y  |

'm' g putc
g expectgrid
        |
   X    |
 moobaz |
     Y  |

\test CR
"\rpl" g puts
g expectgrid
        |
   X    |
ploobaz |
     Y  |

\test LF
"\nt" g puts
g expectgrid
        |
   X    |
ploobaz |
t    Y  |

\test resize
10 4 g resize
g expectgrid
          |
          |
          |
          |

\test TAB
"foo" g puts
9 g putc
"bar" g puts
g expectgrid
foo     ba|
r         |
          |
          |

\test scrolling
1 3 g seekxy
"hello\nhey" g puts
g expectgrid
r         |
          |
 hello    |
hey       |

\test insertlines
1 2 g seekxy
1 g insertlines
"inserted" g puts
g expectgrid
r         |
          |
inserted  |
 hello    |

\test deletelines
1 1 g seekxy
2 g deletelines
g expectgrid
r         |
 hello    |
          |
          |

\test scrolling preserves colors
2 2 g seekxy
BLUE YELLOW joinfgbg g to color
"blue" g puts
DEFAULTCOLOR g to color
g expectgrid
r         |
 hello    |
  blue    |
          |
g expectfg
ffffffffff|
ffffffffff|
ff1111ffff|
ffffffffff|
g expectbg
0000000000|
0000000000|
00eeee0000|
0000000000|

g scrollup
g expectgrid
 hello    |
  blue    |
          |
          |
g expectfg
ffffffffff|
ff1111ffff|
ffffffffff|
ffffffffff|
g expectbg
0000000000|
00eeee0000|
0000000000|
0000000000|

testend
