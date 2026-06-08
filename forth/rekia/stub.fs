\ R.E.K.I.A. stub — the real refiner math is in forth/tritium/rekia.fs
\ (loaded in core). Implements pure-math:
\   extract (scope to drena links/subgraph)
\   contract (iterative fixed-point contraction on trit states, bounded for stability)
\   to-forth (emit runnable colon def encoding the refined K)
\   label-group
\ rekiA-refine ( neuron-addr -- ) does the full pipeline using the neuron data blocks.
\ See the rekia.fs for the math + demo.

: rekiA-extract ( k ctx links -- k' ) ." [rekia] extract (see full engine)" cr ;
: rekiA-contract ( k' -- k'' ) ." [rekia] contract/pure-math" cr ;
: rekiA-to-forth ( k'' label -- ) ." [rekia] emit .fs" cr ;
: rekiA-refine ( neuron ctx -- )
  ." [rekia] refine -> forth + label (full math in rekia.fs)" cr ;
: rekiA-label-group ( k -- label$ ) ." [rekia] label" cr ;