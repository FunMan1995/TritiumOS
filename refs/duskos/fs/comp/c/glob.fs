\ Global variables affecting the whole CC
needs lib/str mem/arena comp/sym
unit comp/c/glob

: psrestore ( n -- ) PSdisp over - ?dup if ps+, then to PSdisp ;
: psneutral 0 psrestore ;

\ forward declaration implemented in expr.fs
alias noop parseConstExpr ( -- n ) \ forward declaration

\ Temporary and permanent arena
\ The purpose of arenas in CC is twofold. First, they allow to reclaim temporary
\ memory generated during the compilation of a unit. This is the Temporary
\ arena. It is cleared before a C unit is compiled. In that arena, there is:
\ 1. Local function variable declarations
\ 2. Static function signatures
\ 3. Static global variables
\ 4. Macro definitions
\ 5. Parsed {} array literals until they're copied away
newarena dup bindalloc[ tarena[ dup bindrun1 tarena1 bindrun1@ tarena1@

\ The Permanent arena is never cleared, so it's not used for memory reclamation.
\ It's used for data that might be generated in the middle of function code
\ generation (it's *also* the purpose of the temporary arena). If it's allowed
\ to use "allot", it will corrupt the generated code.
\ Moreover, to be sure that we're not unlucky and hit an arena's end of buffer
\ in the middle of code generation, we call ":reserve" on both arenas before
\ generating code. That arena is used for:
\ 1. Typedefs
\ 2. Structures, unions, enum
\ 3. Non-static function definitions
\ 4. Non-static global variables
\ 5. Literals used at runtime
newarena dup bindalloc[ parena[ dup bindrun1 parena1 bindrun1@ parena1@

0 value curfunc   \ comp/sig of the current function

: ccmemreserve parena1 ensurenext tarena1 ensurenext ;
