\ R.E.K.I.A. stub — real engine: forth/tritium/rekia.fs + tritium.poly/core/rekia.fs
: rekiA-extract ( k ctx links -- k' ) ." [rekia] extract" cr ;
: rekiA-contract ( k' -- k'' ) ." [rekia] contract" cr ;
: rekiA-to-forth ( k'' label -- ) ." [rekia] emit .fs" cr ;
: rekiA-refine ( neuron ctx -- ) ." [rekia] refine" cr ;
: rekiA-label-group ( s0 s1 s2 -- c-addr u ) s" stub-label" ;
: rekia-demo ( -- ) ." [rekia] demo" cr ;
: s3-reserved-demo ( -- ) ." [rekia] s3 reserved demo" cr ;
