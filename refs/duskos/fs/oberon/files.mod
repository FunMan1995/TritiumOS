(* Source: https://github.com/Spirit-of-Oberon/wirth-personal
   Filename: people.inf.ethz.ch/wirth/ProjectOberon/Sources/Files.mod.txt
   License: /licenses/oberon.txt *)
MODULE Files;

TYPE
  FileDesc = RECORD
    handle: DUSK.StreamRef;
    name: ARRAY 32 OF CHAR
  END;
  File* = POINTER TO FileDesc;
  Rider* = RECORD
    eof: BOOLEAN;
    pos: INTEGER;
    file: File
  END;

LOADFORTH oberon/files.pre

PROCEDURE Old*(name: ARRAY OF CHAR): File;
VAR f: File;
BEGIN
  f := NIL;
  IF (LEN(name) # 0) & (name[0] # 0X) THEN
    IF Lookup(name) THEN
      NEW(f);
      NEW(f.handle);
      f.name := name;
      f.handle.n := Open();
    END;
  END;
  RETURN f
END Old;

PROCEDURE Ensure*(name: ARRAY OF CHAR): File;
VAR f: File;
    fsid: INTEGER;
BEGIN
  fsid := 0;
  f := NIL;
  IF (LEN(name) # 0) & (name[0] # 0X) THEN
    EnsurePriv(name)
    NEW(f);
    NEW(f.handle);
    f.name := name;
    f.handle.n := Open()
  END;
  RETURN f
END Ensure;

PROCEDURE New*(name: ARRAY OF CHAR): File;
VAR f: File;
    fsid: INTEGER;
BEGIN
  NEW(f); NEW(f.handle);
  f.name := name;
  f.handle.n := NewPoolFile();
  RETURN f
END New;

PROCEDURE Copy*(src, dst: File);
BEGIN
  DUSK.seek(src.handle.n, 0);
  DUSK.seek(dst.handle.n, 0);
  DUSK.spit(src.handle.n, dst.handle.n)
END Copy;

PROCEDURE Delete*(name: ARRAY OF CHAR; VAR res: INTEGER);
BEGIN
  res := 2;
  IF Lookup(name) THEN res := 0; Remove() END;
END Delete;

(* TODO: Rename. fs/core doesn't have this concept yet
   TODO: Date. fs/core doesn't have this concept yet *)

PROCEDURE Close*(f: File);
BEGIN
  IF f # NIL THEN DUSK.close(f.handle.n) END
END Close;

PROCEDURE Truncate*(f: File);
BEGIN
  IF f # NIL THEN DUSK.truncate(f.handle.n) END
END Truncate;

(*---------------------------Read---------------------------*)
PROCEDURE Set*(VAR r: Rider; f: File; pos: INTEGER);
BEGIN r.eof := FALSE; r.pos := pos; r.file := f; END Set;

PROCEDURE ReadBytes*(VAR r: Rider; x: DUSK.AnyPtr; n: INTEGER);
VAR readn: INTEGER;
BEGIN
  DUSK.seek(r.file.handle.n, r.pos);
  readn := DUSK.read(r.file.handle.n, n, x);
  r.eof := readn # n;
  r.pos := r.pos + readn;
END ReadBytes;

PROCEDURE ReadByte*(VAR r: Rider; VAR x: BYTE);
VAR buf: ARRAY 1 OF BYTE;
BEGIN
  buf[0] := 0;
  ReadBytes(r, buf, 1);
  x := buf[0]
END ReadByte;

PROCEDURE Read*(VAR r: Rider; VAR ch: CHAR);
VAR buf: ARRAY 1 OF CHAR;
BEGIN
  buf[0] := 0X;
  ReadBytes(r, buf, 1);
  ch := buf[0]
END Read;

PROCEDURE ReadInt*(VAR r: Rider; VAR x: INTEGER);
VAR buf: ARRAY 1 OF INTEGER;
BEGIN
  buf[0] := 0;
  ReadBytes(r, buf, 4);
  x := buf[0]
END ReadInt;

PROCEDURE ReadSet*(VAR r: Rider; VAR x: SET);
VAR buf: ARRAY 1 OF SET;
BEGIN
  buf[0] := {};
  ReadBytes(r, buf, 4);
  x := buf[0]
END ReadSet;

PROCEDURE ReadString*(VAR r: Rider; x: ARRAY OF CHAR);
VAR i: INTEGER; ch: CHAR;
BEGIN
  i := 0; Read(r, ch);
  WHILE ch # 0X DO
    IF i < LEN(x)-1 THEN x[i] := ch; INC(i) END;
    Read(r, ch)
  END;
  x[i] := 0X
END ReadString;

(* TODO: ReadNum? WriteNum? if useful *)

(*---------------------------Write--------------------------*)
PROCEDURE WriteBytes*(VAR r: Rider; x: DUSK.AnyPtr; n: INTEGER);
BEGIN
  DUSK.seek(r.file.handle.n, r.pos);
  `write# (r.file.handle.n, n, x);
  r.pos := r.pos + n;
END WriteBytes;

PROCEDURE WriteByte*(VAR r: Rider; x: BYTE);
VAR buf: ARRAY 1 OF BYTE;
BEGIN
  buf[0] := x;
  WriteBytes(r, buf, 1);
END WriteByte;

PROCEDURE Write*(VAR r: Rider; ch: CHAR);
VAR buf: ARRAY 1 OF CHAR;
BEGIN
  buf[0] := ch;
  WriteBytes(r, buf, 1);
END Write;

PROCEDURE WriteInt*(VAR r: Rider; x: INTEGER);
VAR buf: ARRAY 1 OF INTEGER;
BEGIN
  buf[0] := x;
  WriteBytes(r, buf, 4);
END WriteInt;

PROCEDURE WriteSet*(VAR r: Rider; x: SET);
VAR buf: ARRAY 1 OF SET;
BEGIN
  buf[0] := x;
  WriteBytes(r, buf, 4);
END WriteSet;

PROCEDURE WriteString*(VAR r: Rider; x: ARRAY OF CHAR);
VAR i: INTEGER;
BEGIN
  i := 0;
  WHILE x[i] # 0X DO INC(i) END;
  WriteBytes(r, x, i+1);
END WriteString;
END Files.
