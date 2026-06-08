needs arch/core
needsasm
isx86? [if] needs asm/x87
[then]
unit hal/float
arch<< hal/float.fs
