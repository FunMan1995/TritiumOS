needs tests/harness io/stream mem/ll lib/type comp/oberon/gc
testbegin

resetgc

\test no slot? everything goes unused
uint newptr const ptr1
gc
uint newptr ptr1 #eq
gchdlcnt 1 #eq

\test types each have their own pointers
gc
uchar newptr const ptr2
ptr1 ptr2 <> #true
gchdlcnt 2 #eq

\test ... but this doesn't prevent ptr reuse
uint newptr ptr1 #eq
gchdlcnt 2 #eq

\test GC slots inhibit reuse
variable slot1
slot1 addgcslot
ptr1 slot1 !
gc \ ptr1 not freed
uint newptr const ptr3
ptr3 ptr1 <> #true
gchdlcnt 3 #eq

\test but if we make that slot point to 0...
0 slot1 !
gc \ ptr1 freed
uint newptr ptr1 #eq
gchdlcnt 3 #eq

\test if a ptr holds a GCPointer, recurse the marking process
struct Node { }
Node newgcpointer addtype NodePtr
struct Node {
  NodePtr left right ;
}

Node newptr const node1
node1 left 0 #eq
node1 right 0 #eq
Node newptr const node2
node2 node1 to right
gchdlcnt 5 #eq

variable slot2
slot2 addgcslot
node1 slot2 !

gc \ both nodes kept
Node newptr const node3
gchdlcnt 6 #eq
node2 node3 <> #true
markedcnt 3 #eq

0 slot2 !
gc
markedcnt 0 #eq

\test GCPointer holding a straight structure holding a GCPointer
struct NodeHolder { Node holder ; }

NodeHolder newgcpointer const NodeHolderPtr
NodeHolder newptr const holder1
Node newptr const node1
Node newptr const node2
node1 holder1 to left
node2 holder1 to right

holder1 slot2 !
markedcnt 3 #eq
gc \ keep node1 and node2 marked!
markedcnt 3 #eq

0 slot2 !
gc
markedcnt 0 #eq

\test collecting a StreamRef calls its close method
' abort dup newstream const fakestream
variable cnt
:> drop 1 cnt +! ; fakestream to close
StreamRef newptr const streamptr
fakestream streamptr !
gc \ streamptr goes unused
cnt @ 1 #eq
gc \ doesn't call it twice
cnt @ 1 #eq

testend
