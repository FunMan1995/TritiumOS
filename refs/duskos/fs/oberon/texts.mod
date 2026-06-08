(* Source: https://github.com/Spirit-of-Oberon/wirth-personal
   Filename: people.inf.ethz.ch/wirth/ProjectOberon/Sources/Texts.mod.txt
   License: /licenses/oberon.txt *)
MODULE Texts;
IMPORT Files, Fonts;
LOADFORTH oberon/texts.pre

CONST (*scanner symbol classes*)
  Inval* = 0;   (*invalid symbol*)
  Name* = 1;    (*name s (length len)*)
  String* = 2;  (*literal string s (length len)*)
  Int* = 3;     (*integer i (decimal or hexadecimal)*)
  Char* = 6;    (*special character c*)

  (* TextBlock = TextTag offset run {run} "0" len {AsciiCode}.
     run = fnt [name] col voff len. *)

  TAB = 9X; LF = 0AX;
  TextTag = 0F1X;
  replace* = 0; insert* = 1; delete* = 2; unmark* = 3;  (*op-codes*)

VAR
  TrailerFile: Files.File;
  Sel*: Text;
  SelBegin*, SelEnd*: INTEGER;
  StreamReader: Reader;

(* -------------------- Filing ------------------------*)

PROCEDURE Trailer(): Piece;
VAR Q: Piece;
BEGIN
  NEW(Q);
  Q.f := TrailerFile; Q.off := -1; Q.len := 1;
  Q.fnt := NIL; Q.col := 15; Q.voff := 0;
  RETURN Q
END Trailer;

PROCEDURE Load*(VAR R: Files.Rider; T: Text);
VAR Q, q, p: Piece;
    off: INTEGER;
    N, fno: INTEGER; bt: BYTE;
    f: Files.File;
    FName: ARRAY 32 OF CHAR;
    Dict: ARRAY 32 OF Fonts.Font;
BEGIN
  f := R.file; N := 1; Q := Trailer(); p := Q;
  Files.ReadInt(R, off); Files.ReadByte(R, bt); fno := bt;
  WHILE fno # 0 DO
    IF fno = N THEN
      Files.ReadString(R, FName);
      Dict[N] := Fonts.This(FName); INC(N)
    END;
    NEW(q); q.fnt := Dict[fno];
    Files.ReadByte(R, bt); q.col := bt;
    Files.ReadByte(R, bt); q.voff := ASR(LSL(bt, 24), 24);
    Files.ReadInt(R, q.len);
    Files.ReadByte(R, bt); fno := bt;
    q.f := f; q.off := off; off := off + q.len;
    p.next := q; q.prev := p; p := q
  END;
  p.next := Q; Q.prev := p;
  T.trailer := Q; Files.ReadInt(R, T.len);
END Load;

PROCEDURE OpenInternal(T: Text; name: ARRAY OF CHAR);
VAR f: Files.File; R: Files.Rider; Q, q: Piece;
    tag: CHAR; len: INTEGER;
BEGIN
  f := Files.Old(name);
  IF f # NIL THEN
    T.ascii := FALSE;
    Files.Set(R, f, 0); Files.Read(R, tag); 
    IF tag = TextTag THEN Load(R, T)
    ELSE (*Ascii file*)
      T.ascii := TRUE;
      len := Files.Length(f); Q := Trailer();
      NEW(q);
      q.fnt := Fonts.Mono; q.col := 15; q.voff := 0; q.f := f;
      q.off := 0; q.len := len;
      Q.next := q; q.prev := Q; q.next := Q; Q.prev := q;
      T.trailer := Q; T.len := len
    END
  ELSE (*create new text*)
    Q := Trailer(); Q.next := Q; Q.prev := Q; T.trailer := Q; T.len := 0
  END;
  T.changed := FALSE; T.org := -1; T.pce := T.trailer (*init cache*)
END OpenInternal;

PROCEDURE Store*(VAR W: Files.Rider; T: Text);
VAR p, q: Piece;
    R: Files.Rider;
    off, rlen, pos: INTEGER;
    N, n: INTEGER;
    ch: CHAR;
    Dict: ARRAY 32, 32 OF CHAR;
BEGIN
  pos := W.pos; Files.WriteInt(W, 0); (*place holder*)
  N := 1; p := T.trailer.next;
  WHILE p # T.trailer DO
    rlen := p.len; q := p.next;
    WHILE (q # T.trailer) & (q.fnt = p.fnt) & (q.col = p.col) & (q.voff = p.voff) DO
      rlen := rlen + q.len; q := q.next
    END;
    Dict[N] := p.fnt.name;
    n := 1;
    WHILE Dict[n] # p.fnt.name DO INC(n) END;
    Files.WriteByte(W, n);
    IF n = N THEN Files.WriteString(W, p.fnt.name); INC(N) END;
    Files.WriteByte(W, p.col); Files.WriteByte(W, p.voff);
    Files.WriteInt(W, rlen);
    p := q
  END;
  Files.WriteByte(W, 0); Files.WriteInt(W, T.len);
  off := W.pos; p := T.trailer.next;
  WHILE p # T.trailer DO
    rlen := p.len; Files.Set(R, p.f, p.off);
    WHILE rlen > 0 DO Files.Read(R, ch); Files.Write(W, ch); DEC(rlen) END ;
    p := p.next
  END;
  Files.Set(W, W.file, pos); Files.WriteInt(W, off); (*fixup*)
  T.changed := FALSE;
  IF T.notify # NIL THEN T.notify(T, unmark, 0, 0) END
END Store;

(* -------------------- Editing ----------------------- *)

PROCEDURE OpenBuf*(B: Buffer);
BEGIN
  NEW(B.header); (*null piece*)
  B.last := B.header; B.len := 0
END OpenBuf;

PROCEDURE FindPiece(T: Text; pos: INTEGER; VAR org: INTEGER; VAR pce: Piece);
VAR p: Piece; porg: INTEGER;
BEGIN
  p := T.pce; porg := T.org;
  IF pos >= porg THEN
    WHILE pos >= porg + p.len DO INC(porg, p.len); p := p.next END
  ELSE p := p.prev; DEC(porg, p.len);
    WHILE pos < porg DO p := p.prev; DEC(porg, p.len) END
  END ;
  T.pce := p; T.org := porg;  (*update cache*)
  pce := p; org := porg
END FindPiece;

PROCEDURE SplitPiece(p: Piece; off: INTEGER; VAR pr: Piece);
VAR q: Piece;
BEGIN
  IF off > 0 THEN
    NEW(q);
    q.fnt := p.fnt; q.col := p.col; q.voff := p.voff;
    q.len := p.len - off;
    q.f := p.f; q.off := p.off + off;
    p.len := off;
    q.next := p.next; p.next := q;
    q.prev := p; q.next.prev := q;
    pr := q
  ELSE pr := p
  END
END SplitPiece;

PROCEDURE Save*(T: Text; beg, end: INTEGER; B: Buffer);
VAR p, q, qb, qe: Piece; org: INTEGER;
BEGIN
  IF end > T.len THEN end := T.len END;
  FindPiece(T, beg, org, p);
  NEW(qb); qb^ := p^;
  qb.len := qb.len - (beg - org);
  qb.off := qb.off + (beg - org);
  qe := qb;
  WHILE end > org + p.len DO 
    org := org + p.len; p := p.next;
    NEW(q); q^ := p^; qe.next := q; q.prev := qe; qe := q
  END;
  qe.next := NIL; qe.len := qe.len - (org + p.len - end);
  B.last.next := qb; qb.prev := B.last; B.last := qe;
  B.len := B.len + (end - beg)
END Save;

PROCEDURE Copy*(SB, DB: Buffer);
VAR Q, q, p: Piece;
BEGIN
  p := SB.header; Q := DB.last;
  WHILE p # SB.last DO p := p.next;
    NEW(q); q^ := p^; Q.next := q; q.prev := Q; Q := q
  END;
  DB.last := Q; DB.len := DB.len + SB.len
END Copy;

PROCEDURE Insert*(T: Text; pos: INTEGER; B: Buffer);
VAR pl, pr, p, qb, qe: Piece; org, end: INTEGER;
BEGIN
  FindPiece(T, pos, org, p); SplitPiece(p, pos - org, pr);
  IF T.org >= org THEN T.org := org - p.prev.len; T.pce := p.prev END;
  pl := pr.prev; qb := B.header.next;
  IF (qb # NIL) & (qb.f = pl.f) & (qb.off = pl.off + pl.len)
      & (qb.fnt = pl.fnt) & (qb.col = pl.col) & (qb.voff = pl.voff) THEN
    pl.len := pl.len + qb.len; qb := qb.next
  END;
  IF qb # NIL THEN qe := B.last;
    qb.prev := pl; pl.next := qb; qe.next := pr; pr.prev := qe
  END;
  T.len := T.len + B.len; end := pos + B.len;
  B.last := B.header; B.last.next := NIL; B.len := 0;
  T.changed := TRUE;
  IF T.notify # NIL THEN T.notify(T, insert, pos, end) END
END Insert;

PROCEDURE Append*(T: Text; B: Buffer);
BEGIN Insert(T, T.len, B)
END Append;

PROCEDURE Delete*(T: Text; beg, end: INTEGER; B: Buffer);
VAR pb, pe, pbr, per: Piece; orgb, orge: INTEGER;
BEGIN
  IF end > T.len THEN end := T.len END;
  FindPiece(T, beg, orgb, pb); SplitPiece(pb, beg - orgb, pbr);
  FindPiece(T, end, orge, pe);
  SplitPiece(pe, end - orge, per);
  IF T.org >= orgb THEN (*adjust cache*)
    T.org := orgb - pb.prev.len; T.pce := pb.prev
  END;
  B.header.next := pbr; B.last := per.prev;
  B.last.next := NIL; B.len := end - beg;
  per.prev := pbr.prev; pbr.prev.next := per;
  T.len := T.len - B.len;
  T.changed := TRUE;
  IF T.notify # NIL THEN T.notify(T, delete, beg, end) END
END Delete;

PROCEDURE ChangeLooks*(
  T: Text; beg, end: INTEGER; sel: SET; fnt: Fonts.Font; col, voff: INTEGER);
VAR pb, pe, p: Piece; org: INTEGER;
BEGIN
  IF end > T.len THEN end := T.len END;
  FindPiece(T, beg, org, p); SplitPiece(p, beg - org, pb);
  FindPiece(T, end, org, p); SplitPiece(p, end - org, pe);
  p := pb;
  REPEAT
    IF 0 IN sel THEN p.fnt := fnt END;
    IF 1 IN sel THEN p.col := col END;
    IF 2 IN sel THEN p.voff := voff END;
    p := p.next
  UNTIL p = pe;
  T.changed := TRUE;
  IF T.notify # NIL THEN T.notify(T, replace, beg, end) END
END ChangeLooks;

PROCEDURE Attributes*(
  T: Text; pos: INTEGER; VAR fnt: Fonts.Font; VAR col, voff: INTEGER);
VAR p: Piece; org: INTEGER;
BEGIN
  FindPiece(T, pos, org, p);
  IF p.fnt = NIL THEN (* sentinel *) p := p.prev END;
  fnt := p.fnt; col := p.col; voff := p.voff
  IF fnt = NIL THEN (* empty text *)
    fnt := Fonts.Default; col := 15;
    IF T.ascii THEN fnt := Fonts.Mono END
  END
END Attributes;

(* ------------------ Access: Readers ------------------------- *)

PROCEDURE OpenReader*(VAR R: Reader; T: Text; pos: INTEGER);
VAR p: Piece; org: INTEGER;
BEGIN
  FindPiece(T, pos, org, p);
  R.ref := p; R.org := org; R.off := pos - org;
  Files.Set(R.rider, p.f, p.off + R.off); R.eot := FALSE
END OpenReader;

PROCEDURE Read*(VAR R: Reader; VAR ch: CHAR);
BEGIN
  Files.Read(R.rider, ch);
  R.fnt := R.ref.fnt; R.col := R.ref.col; R.voff := R.ref.voff;
  INC(R.off);
  IF R.off = R.ref.len THEN
    IF R.ref.f = TrailerFile THEN R.eot := TRUE END;
    R.ref := R.ref.next; R.org := R.org + R.off; R.off := 0;
    Files.Set(R.rider, R.ref.f, R.ref.off)
  END
END Read;

PROCEDURE ReadBackwards*(VAR R: Reader; VAR ch: CHAR);
BEGIN
  IF R.off = 0 THEN
    R.ref := R.ref.prev; R.off := R.ref.len; R.org := R.org - R.off;
  END;
  IF R.off > 0 THEN
    DEC(R.off);
    Files.Set(R.rider, R.ref.f, R.ref.off+R.off)
    Files.Read(R.rider, ch);
    Files.Set(R.rider, R.ref.f, R.ref.off+R.off)
    R.fnt := R.ref.fnt; R.col := R.ref.col; R.voff := R.ref.voff;
  END;
  R.eot := R.ref.f = TrailerFile;
END ReadBackwards;

PROCEDURE Pos*(VAR R: Reader): INTEGER;
BEGIN RETURN R.org + R.off
END Pos;  

PROCEDURE AsStream*(T: Text; beg, end: INTEGER): OPAQUE;
BEGIN
  OpenReader(StreamReader, T, beg);
  RETURN SetupTextStream(StreamReader, end-beg)
END AsStream;

PROCEDURE SelectionAsStream*(): OPAQUE;
VAR res: OPAQUE;
BEGIN
  res := NIL;
  IF Sel # NIL THEN res := AsStream(Sel, SelBegin, SelEnd) END;
  RETURN res
END SelectionAsStream;

(* Filing, cont. *)

PROCEDURE StoreASCII*(VAR W: Files.Rider; T: Text);
VAR R: Reader; ch: CHAR;
BEGIN
  OpenReader(R, T, 0); Read(R, ch);
  WHILE ~R.eot DO Files.Write(W, ch); Read(R, ch)  END;
  T.changed := FALSE;
  IF T.notify # NIL THEN T.notify(T, unmark, 0, 0) END
END StoreASCII;

PROCEDURE Close*(T: Text; name: ARRAY OF CHAR);
VAR f, mf: Files.File; w: Files.Rider;
BEGIN
  mf := Files.New(name);
  Files.Set(w, mf, 0);
  IF T.ascii THEN StoreASCII(w, T)
  ELSE Files.Write(w, TextTag); Store(w, T) END;
  f := Files.Ensure(name); 
  Files.Copy(mf, f);
  Files.Truncate(f);
  Files.Close(mf);
  Files.Close(f)
END Close;

(* ------------------ Access: Scanners (NW) ------------------------- *)

PROCEDURE OpenScanner* (VAR S: Scanner; T: Text; pos: INTEGER);
BEGIN OpenReader(S, T, pos); S.line := 0; S.nextCh := ' ' 
END OpenScanner;

PROCEDURE Scan*(VAR S: Scanner);
VAR ch, term: CHAR;
    neg, negE, hex: BOOLEAN;
    i, j, h, d, e, n, s: INTEGER;
    k: INTEGER;
BEGIN
  ch := S.nextCh; i := 0;
  WHILE (ch = ' ') OR (ch = TAB) OR (ch = LF) DO
    IF ch = LF THEN INC(S.line) END ;
    Read(S, ch)
  END;
  IF ('A' <= ch) & (ch <= 'Z') OR ('a' <= ch) & (ch <= 'z') THEN (*name*)
    REPEAT S.s[i] := ch; INC(i); Read(S, ch)
    UNTIL ((ch < '0') & (ch # '.') & (ch # '/') OR
          ('9' < ch) & (ch < 'A') OR
          ('Z' < ch) & (ch < 'a') OR
          ('z' < ch)) OR (i = 31);
    S.s[i] := 0X; S.len := i; S.class := Name
  ELSIF ch = 22X THEN (*string*)
    Read(S, ch);
    WHILE (ch # 22X) & (ch >= ' ') & (i # 31) DO
      S.s[i] := ch; INC(i); Read(S, ch) END;
    S.s[i] := 0X; S.len := i+1; Read(S, ch); S.class := String
  ELSE hex := FALSE;
    IF ch = '-' THEN neg := TRUE; Read(S, ch) ELSE neg := FALSE END;
    IF ('0' <= ch) & (ch <= '9') THEN (*number*)
      n := ORD(ch) - 30H; h := n; Read(S, ch);
      WHILE ('0' <= ch) & (ch <= '9') OR ('A' <= ch) & (ch <= 'F') DO
        IF ch <= '9' THEN
          d := ORD(ch) - 30H ELSE d := ORD(ch) - 37H; hex := TRUE END;
        n := 10*n + d; h := 10H*h + d; Read(S, ch)
      END;
      IF ch = 'H' THEN (*hex integer*)
        Read(S, ch); S.n := h; S.class := Int  (*neg?*)
      ELSE (*decimal integer*)
        IF neg THEN S.n := -n ELSE S.n := n END;
        IF hex THEN S.class := Inval ELSE S.class := Int END
      END
    ELSE (*special character*) S.class := Char;
      IF neg THEN S.c := '-' ELSE S.c := ch; Read(S, ch) END
    END
  END;
  S.nextCh := ch
END Scan;

(* --------------- Access: Writers (NW) ------------------ *)

PROCEDURE OpenWriter*(VAR W: Writer);
BEGIN
  NEW(W.buf);
  OpenBuf(W.buf); W.fnt := Fonts.Default; W.col := 15; W.voff := 0;
  Files.Set(W.rider, Files.New(""), 0)
END OpenWriter;

PROCEDURE Write*(VAR W: Writer; ch: CHAR);
VAR p: Piece;
BEGIN
  IF (W.buf.last.fnt # W.fnt) OR (W.buf.last.col # W.col) OR (W.buf.last.voff # W.voff) THEN
    NEW(p); p.f := W.rider.file; p.off := W.rider.pos; p.len := 0;
    p.fnt := W.fnt; p.col := W.col; p.voff:= W.voff;
    p.next := NIL; W.buf.last.next := p;
    p.prev := W.buf.last; W.buf.last := p
  END;
  Files.Write(W.rider, ch);
  INC(W.buf.last.len); INC(W.buf.len)
END Write;

PROCEDURE WriteLn*(VAR W: Writer);
BEGIN Write(W, LF)
END WriteLn;

PROCEDURE WriteString*(VAR W: Writer; s: ARRAY OF CHAR);
VAR i: INTEGER;
BEGIN
  i := 0;
  WHILE s[i] # 0X DO Write(W, s[i]); INC(i) END
END WriteString;

PROCEDURE WriteInt*(VAR W: Writer; x, n: INTEGER);
VAR i: INTEGER; x0: INTEGER;
    a: ARRAY 10 OF CHAR;
BEGIN
  IF ROR(x, 31) = 1 THEN WriteString(W, " -2147483648")
  ELSE i := 0;
    IF x < 0 THEN DEC(n); x0 := -x ELSE x0 := x END;
    REPEAT
      a[i] := CHR(x0 MOD 10 + 30H); x0 := x0 DIV 10; INC(i)
    UNTIL x0 = 0;
    WHILE n > i DO Write(W, ' '); DEC(n) END;
    IF x < 0 THEN Write(W, '-') END;
    REPEAT DEC(i); Write(W, a[i]) UNTIL i = 0
  END
END WriteInt;

PROCEDURE WriteHex*(VAR W: Writer; x: INTEGER);
VAR i: INTEGER; y: INTEGER;
    a: ARRAY 10 OF CHAR;
BEGIN
  i := 0; Write(W, ' ');
  REPEAT y := x MOD 10H;
    IF y < 10 THEN a[i] := CHR(y + 30H) ELSE a[i] := CHR(y + 37H) END;
    x := x DIV 10H; INC(i)
  UNTIL i = 8;
  REPEAT DEC(i); Write(W, a[i]) UNTIL i = 0
END WriteHex;

PROCEDURE Open*(T: Text; name: ARRAY OF CHAR);
VAR pos: INTEGER; ch: CHAR;
    buf: Buffer; R: Reader; W: Writer; 
BEGIN
  OpenInternal(T, name);
  IF T.ascii THEN 
    NEW(buf); OpenBuf(buf); OpenReader(R, T, 0); OpenWriter(W);
    Read(R, ch);
    WHILE ~R.eot DO
      IF ch = 0DX THEN 
        Read(R, ch);
        pos := Pos(R); Delete(T, pos - 2, pos - 1, buf);
        IF ~(ch = 0AX) THEN WriteLn(W); Insert(T, pos - 2, W.buf) END;
        OpenReader(R, T, pos - 1);
      END;
      Read(R, ch)
    END
  END
END Open;

PROCEDURE Print*(T: Text);
BEGIN DUSK.spit(AsStream(T, 0, T.len), DUSK.console()) END Print;

BEGIN TrailerFile := Files.New("")
END Texts.
