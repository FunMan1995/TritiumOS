needs tests/harness fs/sh

: runall
  "tests/kernel.fs" loadpath
  p"tests" enterdir begin gotonext while
    walkdir? if walk>r rundir r>walk then repeat
  ."All tests passed. Total count: " gtestcnt . nl> ;
runall
