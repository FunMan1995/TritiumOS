\ D.R.E.N.A. stub — real engine is tritium.poly/core/drena.fs (and forth/tritium/drena.fs)
\ Kept for layout parity; prefer the engine file in boot.
: drena-group ( label-addr -- ) ." [drena] group: " type cr ;
: drena-spawn ( variation -- neuron ) ." [drena] spawn" cr ;
: drena-grow ( parent -- child ) ." [drena] grow" cr ;
: drena-rewire ( neuron -- ) ." [drena] rewire S3 (see engine)" cr ;
: drena-step ( -- ) ." [drena] evolution tick" cr ;
: link! ( src dst type w -- ) ." [drena] link!" cr ;
: group-link! ( ga gb -- ) ." [drena] group-link" cr ;
