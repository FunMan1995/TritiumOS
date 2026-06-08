needs tests/harness lib/psrs
testbegin

\test dipdup
1 2 dipdup 2 #eq 1 #eq 1 #eq

\test dipswap
1 2 3 dipswap 3 #eq 1 #eq 2 #eq

\test dipnip
1 2 3 dipnip 3 #eq 2 #eq

\test dipover
1 2 3 dipover 3 #eq 1 #eq 2 #eq 1 #eq

\test dig
1 2 3 2 dig 1 #eq 3 #eq 2 #eq 1 #eq

\test roll
1 2 3 4 4 roll 1 #eq 4 #eq 3 #eq 2 #eq
1 2 3 4 2 roll 3 #eq 4 #eq 2 #eq 1 #eq
1 2 3 4 1 roll 4 #eq 3 #eq 2 #eq 1 #eq
1 2 3 4 0 roll 4 #eq 3 #eq 2 #eq 1 #eq

\test roll>
1 2 3 4 4 roll> 3 #eq 2 #eq 1 #eq 4 #eq
1 2 3 4 2 roll> 3 #eq 4 #eq 2 #eq 1 #eq
1 2 3 4 1 roll> 4 #eq 3 #eq 2 #eq 1 #eq
1 2 3 4 0 roll> 4 #eq 3 #eq 2 #eq 1 #eq

\test rswap
:~ 1 >r 2 >r rswap r> 1 #eq r> 2 #eq ; ~

\test nconcat
1 2 3 4 4 5 6 7 3 nconcat 7 #eq 7 #eq 6 #eq 5 #eq 4 #eq 3 #eq 2 #eq 1 #eq

\test nfirst
3 1 2 3 nfirst 2 #eq
0 nfirst 0 #eq

\test nsame
1 2 3 3 2 nsame 6 #eq 3 #eq 2 #eq 1 #eq 3 #eq 2 #eq 1 #eq
0 42 nsame 0 #eq
1 2 3 3 0 nsame 0 #eq

\test ndup
1 2 3 3 ndup 3 #eq 2 #eq 1 #eq 3 #eq 2 #eq 1 #eq

testend
