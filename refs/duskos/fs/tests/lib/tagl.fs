needs tests/harness lib/tagl
testbegin

\test example usage
$1234 n"CAFE" $cafebabe addtag
$1234 n"DEAD" $deadbeef addtag
$1234 n"CAFE" findtag #true $cafebabe #eq
$1234 n"DEAD" findtag #true $deadbeef #eq
$1238 n"CAFE" findtag not #true

testend
