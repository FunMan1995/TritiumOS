needs tests/harness num/crc
testbegin
\test
"thisisatest" c@+ crc32[] $fba2fc21 #eq
"thisisatest" c@+ crc16[] $9228 #eq
testend
