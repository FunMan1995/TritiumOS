/* Dusk VM for POSIX platforms

This VM is a partial ARMv4 emulator and has the same HAL operand structure as
fs/xcomp/arm/kernel.fs.

Its "never" condition ($f0000000) makes the rest of the operand into a command
index that is implemented in C.

Its filesystem is a tar bundle of the "fs" directory, which is handled by
fs/tar in the VM.

It's a little endian machine with 32-bit memory cell, 8-bit addressable.
Alignment discipline is required.
*/
#include <stdio.h>
#include <stdlib.h>
#include <unistd.h>
#include <stdarg.h>
#include <string.h>
#include <fcntl.h>
#include <sys/time.h>
#include "mem.h"
#include "opts.h"
#include "vm.h"

#define MAXINT32 0xffffffff
#define MEMSZ 0x2000000 // 32MB
#define BOOTADDR 0x200000 // address at which boot.fs is loaded
#define SYSVARSSZ 0x100
#define SYSVARS (MEMSZ-SYSVARSSZ)
#define COMPILING (SYSVARS+0x04)
#define CURALLOC (SYSVARS+0x08)
#define SYSDICT (SYSVARS+0x0c)
#define FINDSTR (SYSVARS+0x10)
#define _RCNT_ (SYSVARS+0x14)
#define FINDCOMPILER (SYSVARS+0x18)
#define SYSALLOC (SYSVARS+0x1c)
#define INPTR (SYSVARS+0x28)
#define PSTOP (SYSVARS+0x48)
#define PSSZ (SYSVARS+0x4c)
#define RSTOP (SYSVARS+0x50)
#define STACKSZ 0x1000

/* Dead page
   While reading and writing to address 0 is perfectly legal in this VM, it's
   almost certainly a programming error. To catch these errors, we can adjust
   OOBCHK and we're fine. However, when structs are involved, looking for zeroes
   isn't enough because an offset is applied to that zero. Instead, we declare
   a whole zone, or "page" to be "dead" (error if accessed).
*/
#define DEADPAGESZ 0x100

static dword regs[16];
static dword cpsr; // N=b31 Z=b30 C=b29 V=b28
static dword mem[MEMSZ/4];
static char *cmem = (char*)mem;
static FILE *mountedfp=NULL;

#define PSRN 0x80000000
#define PSRZ 0x40000000
#define PSRC 0x20000000
#define PSRV 0x10000000
#define REGW 9
#define REGA 11
#define REGS 8
#define REGSYS 7
#define REGPSP 10
#define REGRSP 13
#define REGLR 14
#define REGPC 15
#define rPC  regs[REGPC]
#define rLR  regs[REGLR]
#define rSP  regs[REGRSP]
#define rA   regs[REGA]
#define rPSP regs[REGPSP]
#define rW   regs[REGW]
#define rS   regs[REGS]
#define rSYS regs[REGSYS]

// Addresses of sysaliases
static dword wordaddr;
static dword stackchkaddr;
static dword wnfaddr;
static dword parseaddr;

static void vmabort() { rPC = MEMSZ + 1; }
static void DBG(); // forward declaration
#define ABORTMSG(...) _printf(__VA_ARGS__); vmabort(); return;
static void memdump() {
	_printf("Dumping memory to memdump.\n");
	FILE *fp = fopen("memdump", "w");
	fwrite(cmem, MEMSZ, 1, fp);
	fclose(fp);
}
static dword ooberr(dword a) {
	_printf("Out of bounds memory access %x\n", a);
	DBG();
	memdump();
	exit(1);
	return 0;
}

#ifdef DEADPAGESZ
#define OOBCHK(a) (((a >= DEADPAGESZ) && (a < MEMSZ)) ? a : ooberr(a))
#else
#define OOBCHK(a) (((a >= 0) && (a < MEMSZ)) ? a : ooberr(a))
#endif
static dword gd(dword addr) { return mem[OOBCHK(addr)/4]; }
static byte gb(dword addr) {
	dword d = gd(addr);
	d >>= (addr & 3) * 8;
	return d & 0xff;
}
static void sd(dword addr, dword d) { mem[OOBCHK(addr)/4] = d; }
static void sb(dword addr, byte b) {
	dword d1 = b;
	dword d2 = gd(addr);
	d1 <<= (addr & 3) * 8;
	d2 &= ~(0xff << ((addr & 3) * 8));
	sd(addr, d1 | d2);
}
dword ppop() { dword n = rW; rW = gd(rPSP); rPSP += 4; return n; }
dword ppeek() { return rW; }
void ppush(dword n) { rPSP -= 4; sd(rPSP, rW); rW = n; }
void pset(dword n) { rW = n; }
void* absaddr(dword a) { return &cmem[a]; }
static dword hereptr() { return gd(CURALLOC); }
static dword here() { return gd(hereptr()); }
static void allot(dword n) { sd(hereptr(), here()+n); }
static dword sysdict() { return gd(SYSDICT); }
static dword _findentry(dword dict, byte *name, byte slen) {
	dword a = dict;
	byte buf[4] = {0};
	dword tofind;
	byte n = slen;

	buf[3] = n;
	buf[2] = name[--n];
	if (n) buf[1] = name[--n];
	if (n) buf[0] = name[--n];
	tofind = *(dword*)buf;
	while (a) {
		OOBCHK(a);
		if ((gd(a-4) & 0x3fffffff) == tofind) {
			if (memcmp(name, &cmem[a-1-slen], slen)==0) return a;
		}
		a = gd(a);
	}
	return 0;
}
static dword find(char *name) {
	dword e = _findentry(sysdict(), (byte*)name, strlen(name));
	if (e) e += 4;
	return e;
}
static void makeimm(char *name) {
	dword w = find(name);
	sb(w-5, gb(w-5)|0x80);
}

static void cwrite(byte b) {
	sb(here(), b);
	allot(1);
}
static void dwrite(dword d) {
	sd(here(), d);
	allot(4);
}

static void callword(dword addr); // forward declaration
#define CALLWORD(x) callword(x); if (rPC >= MEMSZ) return

static void ALIGNHERE() {
	allot((4-(here()%4))&3);
}

static void _entry(dword dict, byte *name, byte slen) {
	ALIGNHERE();
	sd(here(), 0);
	allot((4-((slen+1)%4))&3);
	memcpy(&cmem[here()], name, slen);
	allot(slen);
	cwrite(slen);
	dwrite(gd(dict));
	sd(dict, here()-4);
}
static void entry(char *name) {
	_entry(SYSDICT, (byte*)name, strlen(name));
}

static void _stackdump(dword a, dword top) {
	if (top - a <= STACKSZ) {
		while (a < top) { _printf("%08x ", gd(a)); a += 4; } _printf("\n");
	} else {
		_printf("Broken stack!\n");
	}
}
static void DBG() {
	for (int i=0; i<8; i++) _printf("R%02d %08x ", i, regs[i]);
	_printf("\n");
	for (int i=8; i<16; i++) _printf("R%02d %08x ", i, regs[i]);
	_printf("CPSR %08x\n", cpsr);
	_printf("PS ");
	if (rPSP != gd(PSTOP)) _stackdump(rPSP, gd(PSTOP)-4); else _printf("\n");
	_printf("RS "); _stackdump(rSP, gd(RSTOP));
}

/* Code generation */
static void blwr(dword addr) {
	addr -= here();
	dwrite(0xeb000000|(((addr >> 2) - 2) & 0x00ffffff));
}
static void retwr() { dwrite(0xe12fff1e); } // rLR bx) ,)

// str) reg rd) rPSP rn) 4 -i) pre) !)
static void pushwr(dword reg) { dwrite(0xe52a0004|reg<<12); }
// ldr) reg rd) rPSP rn) 4 +i) post)
static void rpushwr(dword reg) { dwrite(0xe52d0004|reg<<12); }
static void rpopwr(dword reg) { dwrite(0xe49d0004|reg<<12); }
static void dupwr() { pushwr(REGW); }
static dword immrot(dword n, dword *rest) {
	dword rot = 0;
	while (n && !(n&3)) {
		rot++;
		n >>= 2;
	}
	*rest = (n&0xffffff00) << (rot*2);
	return (((16-rot)&0xf) << 8) | (n & 0xff);
}

static void _addlitwr(dword op, dword dstreg, dword n) {
	dword rest;
	op |= immrot(n, &rest);
	op |= dstreg << 12; // set Rd
	dwrite(op);
	// add) dstreg rn) 0 imm) f)
	if (rest) _addlitwr(0xe2900000|(dstreg<<16), dstreg, rest);
}
static void addlitwr(dword dstreg, dword n) {
	dword op = 0xe2900000;
	if (n&0x80000000) {
		n = 0 - n;
		op ^= 0x00c00000; // add) to sub)
	}
	_addlitwr(op|(dstreg<<16), dstreg, n); // add) dstreg rn) 0 imm) f)
}
static void immwr(dword dstreg, dword n) {
	_addlitwr(0xe3a00000, dstreg, n); // mov) 0 imm)
}
static void litwr(dword n) {
	dupwr();
	immwr(REGW, n);
}
static void sysconst(char *name, dword val) { entry(name); litwr(val); retwr(); }

/* "never" commands */

static void BYE() { rPC = MEMSZ; }
static void BYEFAIL() { vmabort(); }
static void WORD() {
	dword a = gd(INPTR), res;
	byte c;
	while ((c = gb(a++)) <= ' ');
	res = a-2;
	while ((c = gb(a++)) > ' ');
	sb(res, a-res-2);
	sd(INPTR, a);
	ppush(res);
}

// ( name 'dict -- entry-or-0 )
static void FINDENTRY() {
	dword dict = ppop();
	dword s = ppop();
	byte len = gb(s);
	sd(FINDSTR, s);
	ppush(_findentry(dict, (byte*)&cmem[s+1], len));
}
// ( 'dict a u -- )
static void ENTRY() {
	dword u = ppop();
	dword a = ppop();
	dword dict = ppop();
	_entry(dict, (byte*)&cmem[a], u);
	sd(_RCNT_, 0);
}
static void CODE() {
	dword s;
	byte len;

	ppush(SYSDICT);
	CALLWORD(wordaddr);
	s = ppop();
	len = gb(s++);
	ppush(s); ppush(len);
	ENTRY();
}

static void RUN1() {
	CALLWORD(wordaddr);
	ppush(rW);
	CALLWORD(parseaddr);
	if (ppop()) {
		rPSP += 4; // nip
	} else {
		ppush(sysdict());
		FINDENTRY();
		if (!rW) { CALLWORD(wnfaddr); return; }
		sd(FINDCOMPILER, 0);
		if (gb(rW-1) & 0x40) { /* findselector */
			CALLWORD(rW+4);
			rPSP += 4; // nip
		} else rW += 4;
		CALLWORD(ppop());
		CALLWORD(stackchkaddr);
	}
}

static void PARSEHEX() { // ( a u -- ?n f ) *without* the $
	dword len = ppop();
	dword s = ppop();
	dword n = 0;
	byte c = 0;
	if (!len) { ppush(0); return; }
	while (len--) {
		c = (gb(s++) | 0x20) - '0';
		if (c >= 10) {
			c -= ('a'-'0');
			if (c >= 6) { ppush(0); return; }
			c += 10;
		}
		n *= 16;
		n += c;
	}
	ppush(n); ppush(1);
}

static void PARSE() { // ( str -- n? f )
    byte len = gb(rW++);
	if (gb(rW++) == '$') {
		ppush(len-1);
		PARSEHEX();
	} else rW = 0;
}

static void STARTCOMP() {
	sd(COMPILING, 1);
	while ((rPC < MEMSZ) && gd(COMPILING)) {
		CALLWORD(wordaddr);
		ppush(rW);
		CALLWORD(parseaddr);
		if (ppop()) {
			litwr(ppop());
			ppop();
		} else {
			ppush(sysdict());
			FINDENTRY();
			if (!rW) { DBG(); CALLWORD(wnfaddr); return; }
			ppush(rW);
			rW += 4; // ( e xt )
			if (gb(rW-5) & 0x40) /* findselector */ {
				sd(FINDCOMPILER, 1);
				CALLWORD(ppop());
			}

			dword xt = ppop();
			sd(FINDCOMPILER, 0);
			if (gb(ppop()-1) & 0x80) /* immediate */ {
				CALLWORD(xt);
				CALLWORD(stackchkaddr);
			} else {
				blwr(xt);
			}
		}
	}
}

static void CREATE() {
	CODE();
	dupwr();
	dwrite(0xe1a0900f); // mov) rW rd) rPC rm)
	dwrite(0xe12fff1e); // rLR bx)
}

static void HERE() { ppush(hereptr()); }

static void TICKS() {
	struct timeval time;
	gettimeofday(&time, NULL);
	int64_t s1 = (int64_t)(time.tv_sec) * 1000000;
	ppush(s1 + time.tv_usec);
}

static void SWAP() { dword n = rW; rW = gd(rPSP); sd(rPSP, n); }
static void DUP() { ppush(rW); }
static void OVER() { ppush(gd(rPSP)); }
static void ROT() { dword n = gd(rPSP); sd(rPSP, rW); rW = gd(rPSP+4); sd(rPSP+4, n); }
static void DROP() { ppop(); }
// IN: r0=dividend rS=divisor OUT: r0=quotient rS=remainder
static void DIVMOD() {
	dword q = regs[0] / rS;
	rS = regs[0] % rS;
	regs[0] = q;
}
static void SDIVMOD() {
	int32_t q = (int32_t)regs[0] / (int32_t)rS;
	rS = (dword)((int32_t)regs[0] % (int32_t)rS);
	regs[0] = (dword)q;
}

static void _z() {	if (rW) cpsr &= ~PSRZ; else cpsr |= PSRZ; }
static void DOADD() { dword n = ppop(); rW += n; _z(); }
static void DOSUB() { dword n = ppop(); rW -= n; _z(); }
static void LSHIFT() { dword n = ppop(); rW <<= n; _z(); }
static void RSHIFT() { dword n = ppop(); rW >>= n; _z(); }
static void BOOL() { rW = !!rW; }
static void NOT() { rW = !rW; }
static void DOOR() { dword n = ppop(); rW |= n; _z(); }
static void DOAND() { dword n = ppop(); rW &= n; _z(); }
static void DOXOR() { dword n = ppop(); rW ^= n; _z(); }
static void DOBIC() { dword n = ppop(); rW &= ~n; _z(); }

static void ADDLITWR() { dword n = ppop(); addlitwr(ppop(), n); }
static void IMMWR() { dword n = ppop(); immwr(ppop(), n); }
static void WRITE() { dwrite(ppop()); }

static void IMMROT() {
	dword rest;
	rW = immrot(rW, &rest);
	ppush(rest); _z();
}

static void EXITWR() { retwr(); }
static void PUSHLRWR() { rpushwr(REGLR); }
static void POPLRWR() { rpopwr(REGLR); }
static void POPEXITWR() { rpopwr(REGPC); }
static void LITN() { litwr(ppop()); }

static void FETCH() { rW = gd(rW); }
static void CFETCH() { rW = gb(rW); }
static void STORE() { dword a = ppop(); sd(a, ppop()); }
static void CSTORE() { dword a = ppop(); sb(a, ppop()); }

static void DOCOL() { CODE(); PUSHLRWR(); STARTCOMP(); }
static void STOPCOMP() { sd(COMPILING, 0); }
static void DOSEMICOL() { STOPCOMP(); POPEXITWR(); }
static void EXECUTE() { rPC = ppop(); }
static void MAIN() { rSP = gd(RSTOP); while(rPC < MEMSZ) RUN1(); }

// from fd.c
void FDWRITE();
void FDREAD();
void FDOPEN();
void FDCLOSE();
void FDSEEK();

// from time.c
void NOW();

#define OPCNT 0x48
static void (*ops[OPCNT])() = {
	BYE, BYEFAIL, NULL, WORD, ALIGNHERE, FINDENTRY, ENTRY, CODE,
	RUN1, PARSEHEX, PARSE, DBG, STARTCOMP, NULL, CREATE, HERE,
	NULL, FDWRITE, FDREAD, FDOPEN, FDCLOSE, FDSEEK, NOW, TICKS,
	SWAP, DUP, OVER, ROT, DROP, DIVMOD, SDIVMOD, NULL,
	DOOR, DOAND, DOXOR, DOBIC, DOADD, DOSUB, LSHIFT, RSHIFT,
	BOOL, NOT, NULL, ADDLITWR, IMMROT, WRITE, NULL, IMMWR,
	EXITWR, PUSHLRWR, POPLRWR, POPEXITWR, NULL, LITN, NULL, NULL,
	FETCH, CFETCH, STORE, CSTORE, NULL, NULL, NULL, NULL,
	DOCOL, STOPCOMP, DOSEMICOL, EXECUTE, MAIN, NULL, NULL, NULL, };

static char* opnames[OPCNT] = {
	"bye", "byefail", NULL, "word", "alignhere", "findentry", "entry[]", "code",
	"run1", "parsehex", "parse", "dbg", "]", NULL, "create", "HERE",
	NULL, "fdwrite", "fdread", "fdopen", "fsclose", "fdseek", "(now)", "(ticks)",
	"swap", "dup", "over", "rot", "drop", "(/mod)", "(s/mod)", NULL,
	"or", "and", "xor", "invand", "+", "-", "lshift", "rshift",
	"bool", "not", NULL, "addlit,", "immrot", ",", NULL, "imm,",
	"exit,", "pushlr,", "poplr,", "popexit,", NULL, "litn", NULL, NULL,
	"@", "c@", "!", "c!", NULL, NULL, NULL, NULL,
	":", "[", ";", "execute", "main", NULL, NULL, NULL, };

/* ARM emulation */
#define Rd(o) ((o>>12)&0xf)
#define Rn(o) ((o>>16)&0xf)
static dword getimm(dword op2) {
	dword n = op2 & 0xff;
	dword s = (op2 >> 7) & 0x1e;
	if (s >= 8) n <<= (32-s);
	else n = (n >> s) | (n << (32 - s));
	return n;
}

static void TODO(dword o) {
	ABORTMSG("Unsupported ARM instruction %x PC %x\n", o, rPC);
}

static void MUL(dword o) {
	unsigned long a = regs[(o>>8)&0xf];
	unsigned long b = regs[o&0xf];
	if (o & 0x00400000) {
		if (a & 0x80000000) a |= 0xffffffff00000000;
		if (b & 0x80000000) b |= 0xffffffff00000000;
	}
	unsigned long n = a * b ;
	if (o & 0x00800000) {
		regs[Rd(o)] = n;
		regs[Rn(o)] = n >> 32;
	} else regs[Rn(o)] = n ;
}
static void BX(dword o) {
	dword d = rPC;
	rPC = regs[o&0xf];
	regs[o&0xf] = d;
}

static void BR(dword o) {
	dword pc = o & 0xffffff;
	if (pc & 0x00800000) pc |= 0xff000000;
	if (o & 0x01000000) rLR = rPC;
	rPC += (pc<<2) + 4;
}

#define I(o) ((o>>25)&1)
#define P(o) ((o>>24)&1)
#define U(o) ((o>>23)&1)
#define B(o) ((o>>22)&1)
#define W(o) ((o>>21)&1)
#define L(o) ((o>>20)&1)
#define Offset(o) (o&0xfff)
static void LDRSTR(dword o) {
	dword a = regs[Rn(o)];
	dword off;
	if (I(o)) {
		off = regs[o&0xf];
		if (o & 0xff0) { ABORTMSG("TODO LDRSTR w/ register shifted offset\n"); }
	}
	else off = Offset(o);
	if (!U(o)) off = 0-off;
	if (P(o)) a += off;
	if (L(o)) {
		if (B(o)) regs[Rd(o)] = gb(a);
		else regs[Rd(o)] = gd(a);
	} else {
		if (B(o)) sb(a, regs[Rd(o)]);
		else sd(a, regs[Rd(o)]);
	}
	if (P(o) == W(o)) regs[Rn(o)] += off;
}

static void LDRHSTRH(dword o) {
	dword a = regs[Rn(o)];
	dword off = ((o >> 4) & 0xf0) | (o & 0xf);
	// offset register based? in LDRSTRH, the "I" bit is 22, the reg "B" bit.
	if (!B(o)) off = regs[o&0xf];
	if (!U(o)) off = 0-off;
	if (P(o)) a += off;
	if (((o & 0x60) != 0x40) && (a%2)) {
		// check alignment unless it's a signed byte
		ABORTMSG("disaligned LDRH/STRH: %08x o=%08x\n", a, o);
	}
	if (L(o)) {
		if (o & 0x40) { // sign-extend halfword or byte
			if (o & 0x20) { // it's a halfword
				regs[Rd(o)] = gb(a) | (gb(a+1)<<8);
				if (regs[Rd(o)] & 0x8000) regs[Rd(o)] |= 0xffff0000;
			} else { // it's a byte
				regs[Rd(o)] = gb(a);
				if (regs[Rd(o)] & 0x80) regs[Rd(o)] |= 0xffffff00;
			}
		} else regs[Rd(o)] = gb(a) | (gb(a+1)<<8);
	} else {
		if (o & 0x40) {
			ABORTMSG("Signed STRH/STRB are undefined instructions\n");
		}
		sb(a, regs[Rd(o)]);
		sb(a+1, regs[Rd(o)]>>8);
	}
	if (P(o) == W(o)) regs[Rn(o)] += off;
}

#define S(o) ((o>>20)&1)
#define Op2(o) (o&0xfff)
#define Rm(o) (o&0xf)
#define Shift(o) ((o>>4)&0xff)

static void _setC(dword o, dword carry) {
	if (S(o)) { if (carry) cpsr |= PSRC; else cpsr &= ~PSRC; }
}
// Unlike the real ARM, we don't have an execution pipeline and, when executing
// an instruction, rPC is 4 bytes ahead, not 8. To have "PC arithmetic" work in
// the same way as the real thing, we need to add 4 to every rPC fetch.
static dword _getRm(dword o) {
	if (I(o)) {
		return getimm(Op2(o));
	} else {
		dword n = regs[Rm(o)];
		if (Rm(o) == REGPC) n += 4;
		dword s = Shift(o);
		dword by;
		dword carry = 0;
		if (s&1) by = regs[(s >> 4) & 0xf];
		else by = (s >> 3) & 0x1f;
		if (!by) return n;
		switch ((s>>1)&3) {
			case 0: n <<= (by-1); carry = n >> 31; n <<= 1; break;
			case 1: n >>= (by-1); carry = n&1; n >>= 1; break;
			case 2:
			carry = n&1;
			if (n & 0x80000000) {
				int32_t sn = n;
				n = (dword)(~((~sn)) >> by);
			} else n >>= by;
			break;
			case 3: n = (n >> by) | (n << (32 - by));
		}
		_setC(o, carry);
		return n;
	}
}
static void _setNZ(dword o) {
	if (!S(o)) return;
	if (regs[Rd(o)]) cpsr &= ~PSRZ; else cpsr |= PSRZ;
	cpsr &= ~PSRN;
	cpsr |= regs[Rd(o)] & PSRN;
}
#define BINOP(x) regs[Rd(o)] = regs[Rn(o)] x _getRm(o); _setNZ(o);
static void AND(dword o) { BINOP(&) }
static void EOR(dword o) { BINOP(^) }
static void _sub(dword o, dword a, dword b, dword c) {
	unsigned long n = (unsigned long)a - (unsigned long)b;
	if (c) n -= !((cpsr >> 29) & 1);
	dword carry = n <= MAXINT32;
	regs[Rd(o)] = n;
	_setNZ(o);
	_setC(o, carry);
}
static void SUB(dword o) { _sub(o, regs[Rn(o)], _getRm(o), 0); }
static void SBC(dword o) { _sub(o, regs[Rn(o)], _getRm(o), 1); }
static void RSB(dword o) { _sub(o, _getRm(o), regs[Rn(o)], 0); }
static void RSC(dword o) { _sub(o, _getRm(o), regs[Rn(o)], 1); }
static void _add(dword o, dword a, dword b, dword c) {
	unsigned long n = (unsigned long)a + (unsigned long)b;
	if (c) n += (cpsr >> 29) & 1;
	dword carry = n > MAXINT32;
	regs[Rd(o)] = n;
	_setNZ(o);
	_setC(o, carry);
}
static void ADD(dword o) { _add(o, regs[Rn(o)], _getRm(o), 0); }
static void ADC(dword o) { _add(o, regs[Rn(o)], _getRm(o), 1); }
static void ORR(dword o) { BINOP(|) }
static void BIC(dword o) { BINOP(&~) }
static void CMP(dword o) {
	dword Rn = regs[Rn(o)];
	dword Rm = _getRm(o);
	dword res = Rn - Rm;
	if (res) cpsr &= ~PSRZ; else cpsr |= PSRZ;
	cpsr &= ~PSRN;
	cpsr |= res & PSRN;
	if (Rn < Rm) cpsr &= ~PSRC; else cpsr |= PSRC;
	if ((Rn & PSRN) == (res & PSRN)) cpsr &= ~PSRV; else cpsr |= PSRV;
	//printf("CMP %x %x %x %x\n", Rn, Rm, res, cpsr);
}

static void MOV(dword o) { regs[Rd(o)] = _getRm(o); _setNZ(o); }
static void (*dataops[16])(dword opcode) = {
	AND, EOR, SUB, RSB, ADD, ADC, SBC, RSC,
	TODO, TODO, CMP, TODO, ORR, MOV, BIC, TODO};

static void armop(dword opcode) {
	if ((opcode & 0x0ffffff0) == 0x012fff10) BX(opcode);
	else if ((opcode & 0x0f0000f0) == 0x00000090) MUL(opcode);
	else if ((opcode & 0x0a000000) == 0x0a000000) BR(opcode);
	else if ((opcode & 0x0e000090) == 0x00000090) LDRHSTRH(opcode);
	else if ((opcode & 0x0c000000) == 0x04000000) LDRSTR(opcode);
	else if ((opcode & 0x0c000000) == 0) { // Data processing
		dataops[(opcode>>21)&0xf](opcode);
	} else TODO(opcode);
}

static void oprun1() { // run next op
	dword opcode = gd(rPC);
	//printf("running %x %x\n", opcode, rPC);
	rPC += 4;
	switch (opcode >> 28) {
		case 0x0: if (cpsr & PSRZ) break; return;
		case 0x1: if (!(cpsr & PSRZ)) break; return;
		case 0x2: if (cpsr & PSRC) break; return;
		case 0x3: if (!(cpsr & PSRC)) break; return;
		case 0x8: if ((cpsr & PSRC) && !(cpsr & PSRZ)) break; return;
		case 0x9: if (!(cpsr & PSRC) || (cpsr & PSRZ)) break; return;
		case 0xa: if ((cpsr & PSRN) == (cpsr & PSRV)) break; return;
		case 0xb: if ((cpsr & PSRN) != (cpsr & PSRV)) break; return;
		case 0xc: if (!(cpsr & PSRZ) && ((cpsr & PSRN) == (cpsr & PSRV))) break; return;
		case 0xd: if ((cpsr & PSRZ) || ((cpsr & PSRN) != (cpsr & PSRV))) break; return;
		case 0xe: break;
		case 0xf:
		opcode &= 0x0fffffff;
		if ((opcode >= OPCNT) || (!ops[opcode])) {
			_printf("Illegal opcode %02x at PC %08x\n", opcode, rPC-4);
			BYEFAIL();
		} else {
			ops[opcode]();
		}
		return;
		default: TODO(opcode);
	}
	armop(opcode);
}

/* Dictionary setup and main() */
static void callword(dword addr) {
	dword oldPC, oldLR, oldSP;
	// We push a special "0" marker to rLR. This way, when PC is 0, we know that
	// it's time to return from our callword().
	oldLR = rLR;
	oldPC = rPC;
	oldSP = rSP;
	rLR = 0;
	rPC = OOBCHK(addr);
	while (rPC && (rPC < MEMSZ)) oprun1();
	if (rPC < MEMSZ) {
		// When unwinding the stack in hackish ways, for example in
		// tests/harness expectabort, it's possible that we remove one of our 0
		// markers from the stack. In this case, we want to unwind the C stack at the
		// same proportions, so we "propagate" the 0. This isn't supposed to happen
		// often, hence the printf().
		if (rSP > oldSP) {
			printf("callstack unwinding from underneath callword()!\n");
			oldPC = 0;
			oldLR = 0;
		}
		rPC = oldPC;
		rLR = oldLR;
	}
}
static void wentry(char *name, byte op) {
	entry(name); dwrite(op|0xf0000000); retwr();
}

static void buildsysdict() {
	memset(&cmem[SYSVARS], 0, SYSVARSSZ);
	sd(CURALLOC, SYSALLOC);
#ifdef DEADPAGESZ
	sd(hereptr(), DEADPAGESZ);
#endif
	sd(RSTOP, rSP);
	sd(PSTOP, rPSP);
	sd(PSSZ, STACKSZ);
	sysconst("ARCH", 0x23);
	sysconst("EMITFD", stderremit ? 2 : 1);
	sysconst("SYSVARS", SYSVARS);
	for (int i=0; i<OPCNT; i++) {
		if (opnames[i]) wentry(opnames[i], i);
	}
	makeimm("[");
	makeimm(";");
	sd(INPTR, BOOTADDR);
	parseaddr = find("parse");
	wordaddr = find("word");
	entry("(wnf)"); wnfaddr = here(); retwr();
	entry("stack?"); stackchkaddr = here(); retwr();
	entry("(div0)"); retwr();
	entry("clearicache"); retwr();
}

void newsyscall(char *name, void *func) {
	for (int i=0; i<OPCNT; i++) {
		if (ops[i] == NULL) {
			ops[i] = func;
			wentry(name, i);
			return;
		}
	}
	_printf("out of OPS slots!\n");
}

void setupvm(const unsigned char *bootstring, int bootlen) {
	memcpy(&cmem[BOOTADDR], bootstring, bootlen);
	strcpy(&cmem[BOOTADDR+bootlen-1], bootcmd);
	rSYS = rSP = SYSVARS;
	rPSP = rSP - STACKSZ;
	buildsysdict();
	rPC = find("main");
}

int runvm() {
	while (rPC < MEMSZ) oprun1();
	if (mountedfp) fclose(mountedfp);
	if (rPC > MEMSZ) {
		DBG();
		memdump();
	}
	return MEMSZ - rPC;
}
