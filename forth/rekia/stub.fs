\ R.E.K.I.A. stub — real engine is tritium.poly/core/rekia.fs (and forth/tritium/rekia.fs)
: rekiA-extract ( k ctx links -- k' ) ." [rekia] extract" cr ;
: rekiA-contract ( k' -- k'' ) ." [rekia] contract" cr ;
: rekiA-to-forth ( k'' label -- ) ." [rekia] emit .fs (see engine)" cr ;
: rekiA-refine ( neuron ctx -- ) ." [rekia] refine" cr ;
: rekiA-label-group ( k -- label$ ) ." [rekia] label" cr ;
: rekia-demo ( -- ) ." [rekia] demo (see engine)" cr ;
