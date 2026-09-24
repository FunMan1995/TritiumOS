\ D.R.E.N.A. stub shim — real engine lives in forth/tritium/drena.fs
\ (mirrored: tritium.poly/core/drena.fs + android assets core/drena.fs).
\ Prefer loading the real core. Print-only grow/step are NOT the implementation.
\ Real drena-grow / drena-step never call rewire and never advance RESERVED
\ (RESERVED parent → RESERVED child; step skips RESERVED). See docs/ASSUMPTIONS.md.
\
\ Thin redirects / warnings if this file is included without the real engine:
: drena-grow ( parent -- child )
  drop 0 ." [drena] stub: real drena-grow is in forth/tritium/drena.fs" cr ;
: drena-step ( -- )
  ." [drena] stub: real drena-step is in forth/tritium/drena.fs" cr ;
\ Other names also implemented in tritium/drena.fs — keep minimal placeholders:
: drena-group ( label-addr -- ) count type ." [drena] group (stub — use tritium/drena.fs)" cr ;
: drena-spawn ( variation -- neuron ) drop 0 ." [drena] spawn (stub — use tritium/drena.fs)" cr ;
: drena-rewire ( neuron -- ) drop ." [drena] rewire (stub — use tritium/drena.fs)" cr ;
: link! ( src dst type w -- ) 2drop 2drop ." [drena] link! (stub)" cr ;
: link@ ( idx -- rec ) drop 0 ;
: links-for-neuron ( neuron -- addr count ) drop 0 0 ;
: group-link! ( ga gb -- ) 2drop ." [drena] group-link (stub)" cr ;
: group-label! ( c-addr u gid -- ) drop 2drop ." [drena] group-label! (stub)" cr ;
