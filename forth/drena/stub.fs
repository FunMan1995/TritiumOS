\ D.R.E.N.A. stub — labeled groups + neural linking data
\ The real Trit intelligence engine (data blocks + stability) is in forth/tritium/drena.fs
\ (loaded in core boot). It implements the exact layout requested:
\   first 4 bits (nibble0): 2 trit states (-1/0/1)
\   4th nibble low 2 bits: RANDOM mode etc.
\   followed by node address + connected node addresses (for neuromorphic graph).
\ See trit intelligence engine file + docs for full stable impl.

: drena-group ( label-addr -- ) ." [drena] group: " type cr ;
: drena-spawn ( variation -- neuron ) ." [drena] spawn neuron var=" . cr ;
: drena-grow ( parent -- child ) ." [drena] grow" cr ;
: drena-rewire ( neuron -- ) ." [drena] rewire S3" cr ;
: drena-step ( -- ) ." [drena] evolution tick" cr ;
: link! ( src dst type w -- ) ." [drena] link!" cr ;
: group-link! ( ga gb -- ) ." [drena] group-link" cr ;