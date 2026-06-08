/* test a few simple C constructs */

// The tokenizer used to crash on a // comment before a #define
## 40 const FORTY
#define MYCONST FORTY+FOUR-2

#if 0
this is not parsed by the C compiler and can even contain a #endif !
onewordhere
#else
#if 0
Those #ifs can be nested and this isn't parsed
#else
// just return a constant
short retconst() {
    return MYCONST;
}
#endif

#if 1
// Let's try a parametrized macro
#define ASSIGN %< = %<
#else
not parsed
#endif
#endif
/* There used to be a bug where this type of comment with "'" char in it would
   cause a tokenization error. */
short variables() {
    short ASSIGN(foo 40 ), ASSIGN(_bar "2");
    _bar = foo + _bar;
    return foo + _bar;
}

// The presence of this array used to make the compiler crash
uint bigarray[24] = {
0x00000000, 0x00000000, 0x00000000, 0x00000000,
0x00000000, 0x00000000, 0x00000000, 0x00000000,
0x00000000, 0x00000000, 0x00000000, 0x00000000,
0x00000000, 0x00000000, 0x00000000, 0x00000000,
0x00000000, 0x00000000, 0x00000000, 0x00000000,
0x00000000, 0x00000000, 0x00000000, 0x00000000
};

// test that the compiler can manage to find a reference to the function being
// currently compiled.
void recursive() { recursive(); }
