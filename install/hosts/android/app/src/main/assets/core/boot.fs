\ TritiumOS core bootstrap — Creator: Draco
\ DuskOS/CollapseOS-inspired minimal Forth base for TritiumForth.
\ See docs/FORTH-BASE-REFERENCES.md and refs/duskos/
." TritiumOS core" cr
include trit.fs
include tritium-kernel.fs
include drena.fs   \ Trit intelligence engine (data blocks for neuromorphic)
include rekia.fs   \ R.E.K.I.A. refiner math (pure-math extract/contract -> Forth)
." boot ok" cr