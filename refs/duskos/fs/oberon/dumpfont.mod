MODULE DumpFont;
  IMPORT Files, Fonts, Texts, Oberon;

  CONST FontFileId = 0DBH;

  PROCEDURE WriteInt16(VAR R: Files.Rider; v: INTEGER);
  BEGIN Files.WriteByte(R, v MOD 100H); Files.WriteByte(R, v DIV 100H);
  END WriteInt16;

  PROCEDURE Dump*();
    VAR S: Texts.Scanner;
      Fnt: Fonts.Font;
      F: Files.File;
      R: Files.Rider;
      b: BYTE;
      i, len, dx, x, y, w, h, adr: INTEGER;
  BEGIN
    Texts.OpenScanner(S, Oberon.Par.text, Oberon.Par.pos); Texts.Scan(S);
    IF S.class = Texts.Name THEN
      Fnt := Fonts.This(S.s); Texts.Scan(S)
    END;
    IF S.class = Texts.Name THEN
      F := Files.Ensure(S.s); Files.Set(R, F, 0);
      Files.WriteByte(R, FontFileId); (* id *)
      Files.WriteByte(R, 0); (* abstraction *)
      Files.WriteByte(R, 0); (* family *)
      Files.WriteByte(R, 0); (* variant *)
      WriteInt16(R, Fnt.height); (* height *)
      WriteInt16(R, Fnt.minX); (* minX *)
      WriteInt16(R, Fnt.maxX); (* maxX *)
      WriteInt16(R, Fnt.minY); (* minY *)
      WriteInt16(R, Fnt.maxY); (* maxY *)
      WriteInt16(R, 1); (* runs *)
      (* write runs *)
      WriteInt16(R, 20H);
      WriteInt16(R, 80H);
      (* write boxes *)
      FOR i := 20H TO 7FH DO
        Fonts.GetPat(Fnt, CHR(i), dx, x, y, w, h, adr);
        WriteInt16(R, dx); WriteInt16(R, x); WriteInt16(R, y);
        WriteInt16(R, w); WriteInt16(R, h)
      END;
      (* write glyphs *)
      FOR i := 20H TO 7FH DO
        Fonts.GetPat(Fnt, CHR(i), dx, x, y, w, h, adr);
        len := (w + 7) DIV 8 * h; adr := adr + 2;
        WHILE len > 0 DO
          GET(adr, b); Files.WriteByte(R, b); DEC(len); INC(adr)
        END
      END;
      Files.Truncate(F);
      Files.Close(F);
    END;
  END Dump;
END DumpFont.
