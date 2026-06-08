(* Source: https://github.com/Spirit-of-Oberon/wirth-personal
   Filename: people.inf.ethz.ch/wirth/ProjectOberon/Sources/Fonts.mod.txt
   License: /licenses/oberon.txt *)
MODULE Fonts;
IMPORT Files;

LOADFORTH oberon/fonts.pre

CONST FontFileId = 0DBH;

TYPE
  RunRec = RECORD beg, end: BYTE END ;
  BoxRec = RECORD dx, x, y, w, h: BYTE END ;
    
VAR Default*, Mono*, root*: Font;
(* This array lives in the stack in the regular Oberon, but we usually run with
   a STACKSZ=$800, it doesn't fit! *)
    box: ARRAY 512 OF BoxRec;

PROCEDURE RdInt16(VAR R: Files.Rider; VAR b0: BYTE);
VAR b1: BYTE;
BEGIN Files.ReadByte(R, b0); Files.ReadByte(R, b1)
END RdInt16;

PROCEDURE ReadInt16(VAR R: Files.Rider; VAR r: INTEGER);
VAR b0, b1: BYTE;
BEGIN Files.ReadByte(R, b0); Files.ReadByte(R, b1); r := b1 * 100H + b0;
END ReadInt16;

PROCEDURE FromFile(name: ARRAY OF CHAR): Font;
VAR F: Font;
    f: Files.File; R: Files.Rider;
    NofRuns, NofBoxes: INTEGER;
    NofBytes: INTEGER;
    height, minX, maxX, minY, maxY: BYTE;
    i, j, k, m, n: INTEGER;
    a, a0: INTEGER;
    b, UsedBoxes: BYTE;
    beg, end: INTEGER;
    run: ARRAY 16 OF RunRec;
    namebuf: ARRAY 32 OF CHAR;
BEGIN
  DUSK.ObStr(
    DUSK.strcat(DUSK.DuskStr(name), DUSK.DuskStr("data/font/")), namebuf);
  f := Files.Old(namebuf);
  IF f # NIL THEN
    Files.Set(R, f, 0); Files.ReadByte(R, b);
    IF b = FontFileId THEN
      Files.ReadByte(R, b); (*abstraction*)
      Files.ReadByte(R, b); (*family*)
      Files.ReadByte(R, b); (*variant*)
      RdInt16(R, height); RdInt16(R, minX); RdInt16(R, maxX);
      RdInt16(R, minY); RdInt16(R, maxY); ReadInt16(R, NofRuns);
      NofBoxes := 0; k := 0; UsedBoxes := 0;
      WHILE k # NofRuns DO
        ReadInt16(R, beg);
        run[k].beg := beg MOD 100H; ReadInt16(R, end);
        run[k].end := end MOD 100H; NofBoxes := NofBoxes + end - beg;
        IF (beg < 128) THEN
          UsedBoxes := UsedBoxes + end - beg; INC(k)
          ELSE DEC(NofRuns) END
      END;
      NofBytes := 5; j := 0;

      WHILE j # NofBoxes DO
        RdInt16(R, box[j].dx); RdInt16(R, box[j].x); RdInt16(R, box[j].y);
        RdInt16(R, box[j].w); RdInt16(R, box[j].h);
        IF j < UsedBoxes THEN
        NofBytes := NofBytes + 5 + (box[j].w + 7) DIV 8 * box[j].h;
        INC(j) ELSE DEC(NofBoxes) END;
      END;
      NEW(F);
      F.raster := `allot@ (NofBytes);
      F.height := height; F.minX := minX; F.maxX := maxX; F.maxY := maxY;
      IF minY >= 80H THEN F.minY := minY - 100H ELSE F.minY := minY END ;
      a0 := F.raster;
      PUT(a0, 0X); PUT(a0+1, 0X); PUT(a0+2, 0X); PUT(a0+3, 0X); PUT(a0+4, 0X);
      (*null pattern for characters not in a run*)
      INC(a0, 3); a := a0+2; j := 0; k := 0; m := 0;
      WHILE k < NofRuns DO
        WHILE (m < run[k].beg) & (m < 128) DO F.T[m] := a0; INC(m) END;
        WHILE (m < run[k].end) & (m < 128) DO
          F.T[m] := a+3;
          PUT(a, box[j].dx); PUT(a+1, box[j].x); PUT(a+2, box[j].y);
          PUT(a+3, box[j].w); PUT(a+4, box[j].h); INC(a, 5);
          n := (box[j].w + 7) DIV 8 * box[j].h;
          WHILE n # 0 DO DEC(n); Files.ReadByte(R, b); PUT(a, b); INC(a) END;
          INC(j); INC(m)
        END;
        INC(k)
      END;
      WHILE m < 128 DO F.T[m] := a0; INC(m) END ;
    ELSE (*bad file id*) F := NIL
    END
  ELSE (*font file not available*) F := NIL END
  RETURN F
END FromFile;

PROCEDURE This*(name: ARRAY OF CHAR): Font;
VAR F: Font;
BEGIN
  F := root;
  WHILE (F # NIL) & (name # F.name) DO F := F.next END;
  IF F = NIL THEN
    F := FromDusk(name);
    IF F = NIL THEN F := FromFile(name) END
    IF F = NIL THEN F := Default ELSE
      F.name := name; F.next := root; root := F
    END
  END;
  RETURN F
END This;

BEGIN
  root := NIL;
  Default := This("ob10.fnt");
  Mono := This("atari8.uf1");
END Fonts.
