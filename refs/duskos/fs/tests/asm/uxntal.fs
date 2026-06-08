needs tests/harness asm/uxntal emul/uxn
testbegin

create binbuf $200 allot0
: reset binbuf $200 4/ 0 fill ;

\test prefix system in runes
binbuf uxntal"@foo &bar $2a &baz |100 [ -foo/baz ]"
binbuf be@ $2a000000 #eq

\test runes with "/" in them correctly set curlabel
reset
binbuf uxntal"@foo/bar $2b &baz |100 [ -foo/baz ]"
binbuf be@ $2b000000 #eq

\test a rune reference starting with "/"
\ ... is the equivalent of prefixing it with curlabel
reset
binbuf uxntal"@foo/bar $2b &baz |100 /baz"
binbuf be@ $60002800 #eq

\test acid.tal
create expected ,"padabs pass\npadrel pass\npadrel pass\ncoment pass\n\
rawasc pass\nrawhex pass\nlithex pass\nopcode pass\nrawrel pass\nlitrel pass\n\
rawzep pass\nlitzep pass\nrawabs pass\nlitabs pass\nlabels pass\nlambda pass\n\
rewind pass\nfinish pass\n"
here const expectedend
expected value ptr
create myhandlers $100 4* allot0
:> [dev@] $18 dup emit ptr c@+ rot <> ?abort"wrong output" to ptr ;
myhandlers $18 4* + !
myhandlers newuxn
rstvec uxntal<< tests/asm/uxnacid.tal
poweron

testend
