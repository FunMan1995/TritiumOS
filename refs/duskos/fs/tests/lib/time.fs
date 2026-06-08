needs tests/harness lib/time
testbegin

\test decompose HMS
5 3600 * 22 60 * + 42 + decompose
year 2000 #eq
month 1 #eq
day 1 #eq
hour 5 #eq
minute 22 #eq
second 42 #eq

\test decompose epoch
0 decompose
year 2000 #eq
month 1 #eq
day 1 #eq
hour 0 #eq
minute 0 #eq
second 0 #eq

\test compose epoch
2000 to year
1 to month
1 to day
0 to hour
0 to minute
0 to second
compose 0 #eq

\test decompose 2001/09/11
31 5 * 30 2* + 28 + \ days up to september 2001
10 +  \ from september 1 to september 11
366 + \ days in year 2000
86400 * decompose
year 2001 #eq
month 9 #eq
day 11 #eq
hour 0 #eq
minute 0 #eq
second 0 #eq

\test compose/decompose pairs

:~ ( n -- ) dup compose decompose #eq ;
$cafebabe ~
123456789 ~
987654321 ~
-1 ~
0 ~
42424242 ~

\test formatttime on a contemporary date
12 to second
13 to minute
9 to hour
26 to day
4 to month
2026 to year
"2026/04/26 09:13:12" compose formattime #s[]=

testend
