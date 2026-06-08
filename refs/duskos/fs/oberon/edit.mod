(* Source: https://github.com/Spirit-of-Oberon/wirth-personal
   Filename: people.inf.ethz.ch/wirth/ProjectOberon/Sources/Edit.mod.txt
   License: /licenses/oberon.txt *)
MODULE Edit;
IMPORT Files, Fonts, Texts, Viewers, Oberon, MenuViewers, TextFrames, Arg;

CONST LF = 0AX; maxlen = 32;
  StandardMenu = "System.Close System.Copy System.Grow Edit.Search Edit.Store";

TYPE Line = POINTER TO LineDesc;
  LineDesc = RECORD
    p, n: Line;
    b, e, h: INTEGER;
    c: BOOLEAN
  END;

VAR
  W: Texts.Writer;
  M: INTEGER;
  pat: ARRAY maxlen OF CHAR;
  searchBuf: ARRAY 32 OF CHAR;
  d: ARRAY 256 OF INTEGER;

PROCEDURE Store*;
VAR V: Viewers.Viewer;
    Text: TextFrames.Frame;
    T: Texts.Text;
    S: Texts.Scanner;
    f: Files.File; R: Files.Rider;
    beg, end, len: INTEGER;

BEGIN
  Texts.WriteString(W, "Edit.Store ");
  IF Oberon.Par.frame = Oberon.Par.vwr.dsc THEN
    V := Oberon.Par.vwr; Texts.OpenScanner(S, V.dsc(TextFrames.Frame).text, 0)
  ELSE
    V := Oberon.MarkedViewer();
    Texts.OpenScanner(S, Oberon.Par.text, Oberon.Par.pos)
  END;
  Texts.Scan(S);
  IF (S.class = Texts.Char) & (S.c = '^') THEN
    TextFrames.GetSelection(T, beg, end);
    IF T # NIL THEN Texts.OpenScanner(S, T, beg); Texts.Scan(S) END
  END;
  IF (S.class = Texts.Name)
     & (V.dsc # NIL) & (V.dsc.next IS TextFrames.Frame) THEN
    Text := V.dsc.next(TextFrames.Frame);
    Texts.WriteString(W, S.s); Texts.WriteInt(W, Text.text.len, 8);
    Texts.WriteLn(W); Texts.Append(Oberon.Log, W.buf);
    Texts.Close(Text.text, S.s);
    (* TODO: Open() below shouldn't be needed *)
    Texts.Open(Text.text, S.s)
  END
END Store;

PROCEDURE CopyLooks*;
VAR T: Texts.Text;
    beg, end: INTEGER;
    fnt: Fonts.Font; col, voff: INTEGER;
BEGIN
  TextFrames.GetSelection(T, beg, end);
  IF (T # NIL) & (TextFrames.caretF # NIL) THEN
    Texts.Attributes(
      TextFrames.caretF.text, TextFrames.carloc.pos, fnt, col, voff);
    Texts.ChangeLooks(T, beg, end, {0,1,2}, fnt, col, voff)
  END
END CopyLooks;

PROCEDURE ChangeFont*;
VAR S: Texts.Scanner; T: Texts.Text; beg, end: INTEGER;
BEGIN
  TextFrames.GetSelection(T, beg, end);
  IF T # NIL THEN
    Texts.OpenScanner(S, Oberon.Par.text, Oberon.Par.pos); Texts.Scan(S);
    IF S.class = Texts.Name THEN
      Texts.ChangeLooks(T, beg, end, {0}, Fonts.This(S.s), 0, 0)
    END
  END
END ChangeFont;

PROCEDURE ChangeColor*;
VAR S: Texts.Scanner;
    T: Texts.Text;
    col: INTEGER;
    fnt: Fonts.Font;
    beg, end: INTEGER;
BEGIN
  Texts.OpenScanner(S, Oberon.Par.text, Oberon.Par.pos); Texts.Scan(S);
  IF S.class = Texts.Int THEN
    col := S.n; TextFrames.GetSelection(T, beg, end); fnt := NIL;
    IF T # NIL THEN Texts.ChangeLooks(T, beg, end, {1}, fnt, col, 0) END
  END
END ChangeColor;

PROCEDURE ChangeOffset*;
VAR S: Texts.Scanner;
    T: Texts.Text;
    voff: INTEGER; ch: CHAR; fnt: Fonts.Font;
    beg, end: INTEGER;
BEGIN
  Texts.OpenScanner(S, Oberon.Par.text, Oberon.Par.pos); Texts.Scan(S);
  IF S.class = Texts.Int THEN
    voff := S.n; TextFrames.GetSelection(T, beg, end); fnt := NIL;
    IF T # NIL THEN Texts.ChangeLooks(T, beg, end, {2}, fnt, voff, 0) END
  END
END ChangeOffset;

PROCEDURE ShowPosCountForw(l: Line): INTEGER;
VAR cnt: INTEGER;
BEGIN
  cnt := 0; REPEAT l := l.n; INC(cnt) UNTIL l = NIL
  RETURN cnt
END ShowPosCountForw;

PROCEDURE ShowPosCountBack(l: Line): INTEGER;
VAR cnt: INTEGER; c: BOOLEAN;
BEGIN
  cnt := -1; REPEAT c := l.c; l := l.p; INC(cnt) UNTIL l = NIL;
  IF ~ c THEN DEC(cnt) END
  RETURN cnt
END ShowPosCountBack;

PROCEDURE ShowPosForward(t: Texts.Text; p: INTEGER; line: Line);
VAR R: Texts.Reader; ch: CHAR; h: INTEGER;
BEGIN
  Texts.OpenReader(R, t, p); h := 0;
  REPEAT
    Texts.Read(R, ch);
    IF R.eot THEN ch := LF
    ELSIF (ch # LF) & (h < R.fnt.height) THEN
      h := R.fnt.height
    END
  UNTIL ch = LF;
  line.b := p; line.e := Texts.Pos(R) - 1; line.h := h
END ShowPosForward;

PROCEDURE ShowPosBackward(t: Texts.Text; p: INTEGER; line: Line; VAR fst: Line); (* 0 < p *)
VAR R: Texts.Reader; ch: CHAR; b, n, h: INTEGER; l: Line;
BEGIN
  l := NIL;
  REPEAT IF 64 < p THEN n := p - 64 ELSE n := 0 END;
    Texts.OpenReader(R, t, n); h := 0;
    REPEAT Texts.Read(R, ch);
      IF ch = LF THEN
        IF l = NIL THEN
          NEW(l); l.p := NIL; l.n := NIL; l.b := n; l.c := FALSE; fst := l
        ELSE
          NEW(l.n); l.n.p := l; l := l.n; l.n := NIL; l.b := b; l.c := TRUE
        END;
        l.e := Texts.Pos(R) - 1; b := l.e + 1;
        IF h = 0 THEN l.h := R.fnt.height ELSE l.h := h END
      ELSIF h < R.fnt.height THEN
        h := R.fnt.height
      END
    UNTIL Texts.Pos(R) - 1 = p;
    p := n
  UNTIL (p = 0) OR (l # NIL);
  IF l = NIL THEN line.b := 0; fst := line
  ELSE l.n := line; line.p := l; line.b := b
  END;
  IF h = 0 THEN line.h := R.fnt.height ELSE line.h := h END;
  line.c := TRUE
END ShowPosBackward;

PROCEDURE ShowPosHeight(l: Line): INTEGER;
VAR h: INTEGER;
BEGIN
  IF l.c THEN h := l.h ELSE h := 0 END;
  l := l.n;
  WHILE l # NIL DO INC(h, l.h); l := l.n END
  RETURN h
END ShowPosHeight;

PROCEDURE ShowPos(F: TextFrames.Frame; pos: INTEGER);
VAR R: Texts.Reader; ch: CHAR; p, n, hh: INTEGER; Fst, Mid, Lst: Line;
BEGIN
  IF (pos < F.org) OR (TextFrames.Pos(F, F.X + F.W, F.Y) < pos) THEN
    IF pos <= 0 THEN
      TextFrames.Show(F, 0)
    ELSE
      NEW(Mid); Fst := Mid; Lst := Mid;
      ShowPosForward(F.text, pos, Lst); ShowPosBackward(F.text, pos - 1, Fst, Fst);
      hh := F.H - F.bot - F.top;
      IF Mid.h <= hh THEN
        p := ShowPosCountBack(Mid); n := ShowPosCountForw(Mid);
        WHILE ((0 < Fst.b)
              OR (Lst.e < F.text.len)) & (ShowPosHeight(Fst) < hh)
              OR ((2 * (n - 1) < p) & (Lst.e < F.text.len)) DO
          IF (2 * (n - 1) <= p) OR (Fst.b = 0) THEN
            NEW(Lst.n); Lst.n.p := Lst; Lst := Lst.n;
            Lst.n := NIL; Lst.c := TRUE;
            ShowPosForward(F.text, Lst.p.e + 1, Lst); INC(n)
          ELSE
            ShowPosBackward(F.text, Fst.b - 1, Fst, Fst);
            p := ShowPosCountBack(Mid)
          END
        END;
        p := ShowPosCountBack(Mid); n := ShowPosCountForw(Mid);
        WHILE (F.H - F.bot - F.top < ShowPosHeight(Fst)) & (Fst # Lst) DO
          IF (2 * (n - 1) <= p) OR (Lst = Mid) THEN DEC(p);
            IF Fst.c THEN Fst := Fst.n ELSE Fst := Fst.n.n END; Fst.p := NIL
          ELSE DEC(n); Lst := Lst.p; Lst.n := NIL
          END
        END;
        IF Fst.c THEN p := Fst.b ELSE p := Fst.n.b END
      ELSE IF 256 < pos THEN p := pos - 256 ELSE p := 0 END;
        Texts.OpenReader(R, F.text, p);
        REPEAT Texts.Read(R, ch);
          IF ch = 0DX THEN p := Texts.Pos(R) - 1 END
        UNTIL Texts.Pos(R) = pos
      END;
      TextFrames.Show(F, p)
    END
  END
END ShowPos;

PROCEDURE SearchForward(n: INTEGER; VAR R: Texts.Reader);
VAR m: INTEGER; j: INTEGER;
BEGIN
  m := M - n; j := 0;
  WHILE j # m DO searchBuf[j] := searchBuf[n + j]; INC(j) END;
  WHILE j # M DO Texts.Read(R, searchBuf[j]); INC(j) END
END SearchForward;

(*uses global variables M, pat, d, searchBuf for Boyer-Moore search*)
PROCEDURE Search*;
VAR Text: TextFrames.Frame;
    V: Viewers.Viewer;
    R: Texts.Reader;
    T: Texts.Text;
    pos, beg, end, len: INTEGER; n, i, j: INTEGER;
BEGIN 
  V := Oberon.Par.vwr;
  IF Oberon.Par.frame # V.dsc THEN V := Oberon.FocusViewer END;
  IF (V.dsc # NIL) & (V.dsc.next IS TextFrames.Frame) THEN
    Text := V.dsc.next(TextFrames.Frame);
    TextFrames.GetSelection(T, beg, end);
    IF T # NIL THEN
      Texts.OpenReader(R, T, beg);
      i := 0; pos := beg;
      REPEAT Texts.Read(R, pat[i]); INC(i); INC(pos)
      UNTIL (i = maxlen) OR (pos = end);
      M := i; j := 0;
      WHILE j # 256 DO d[j] := M; INC(j) END;
      j := 0;
      WHILE j # M - 1 DO d[ORD(pat[j])] := M - 1 - j; INC(j) END
    END;
    IF Text = TextFrames.caretF THEN
      pos := TextFrames.carloc.pos ELSE pos := 0 END;
    len := Text.text.len;
    Texts.OpenReader(R, Text.text, pos);
    SearchForward(M, R); INC(pos, M);
    j := M;
    REPEAT DEC(j) UNTIL (j < 0) OR (searchBuf[j] # pat[j]);
    WHILE (j >= 0) & (pos < len) DO
      n := d[ORD(searchBuf[M-1])]; SearchForward(n, R); INC(pos, n); j := M;
      REPEAT DEC(j) UNTIL (j < 0) OR (searchBuf[j] # pat[j])
    END ;
    IF j < 0 THEN
      TextFrames.RemoveSelection(Text); TextFrames.RemoveCaret(Text);
      Oberon.RemoveMarks(Text.X, Text.Y, Text.W, Text.H);
      ShowPos(Text, pos); Oberon.PassFocus(V);
      TextFrames.SetCaret(Text, pos)
    END
  END
END Search;

PROCEDURE Locate*;
VAR Text: TextFrames.Frame;
    T: Texts.Text; S: Texts.Scanner;
    V: Viewers.Viewer;
    name: ARRAY 32 OF CHAR;
    beg, end, pos, i, X, Y: INTEGER;
BEGIN
  V := Oberon.FocusViewer;
  IF (V.dsc # NIL) & (V.dsc.next IS TextFrames.Frame) THEN
    TextFrames.GetSelection(T, beg, end);
    IF T # NIL THEN
      Texts.OpenScanner(S, T, beg);
      REPEAT Texts.Scan(S) UNTIL (S.class >= Texts.Int); (*skip names*)
      IF S.class = Texts.Int THEN
        pos := S.n;
        IF (S.nextCh = '@') THEN
          Texts.Scan(S); Texts.Scan(S);
          IF (S.class = Texts.Name) THEN
            i := 0;
            WHILE S.s[i] # 0X DO name[i] := S.s[i]; INC(i) END;
            name[i] := '.'; name[i+1] := 'M'; name[i+2] := 'o';
            name[i+3] := 'd'; name[i+4] := 0X;
            Texts.OpenScanner(S, V.dsc(TextFrames.Frame).text, 0);
            Texts.Scan(S);
            IF (S.class = Texts.Name) & (S.s # name) THEN
              Oberon.AllocateUserViewer(V.X, X, Y);
              T := TextFrames.Text(); Texts.Open(T, name);
              V := MenuViewers.New(
                TextFrames.NewMenu(name, StandardMenu),
                TextFrames.NewText(T, 0), TextFrames.menuH, X, Y)
            END
          END
        END;
        Text := V.dsc.next(TextFrames.Frame);
        TextFrames.RemoveSelection(Text);
        TextFrames.RemoveCaret(Text);
        Oberon.RemoveMarks(Text.X, Text.Y, Text.W, Text.H);
        ShowPos(Text, pos);
        Oberon.PassFocus(V);
        TextFrames.SetCaret(Text, pos)
      END
    END
  END
END Locate;

PROCEDURE Recall*;
VAR Menu, Main: Viewers.Frame;
    buf: Texts.Buffer;
    V: Viewers.Viewer;
    pos: INTEGER;
    M: TextFrames.Frame;
BEGIN V := Oberon.FocusViewer;
  IF (V # NIL) & (V IS MenuViewers.Viewer) THEN
    Menu := V.dsc; Main := V.dsc.next;
    IF Main IS TextFrames.Frame THEN
      M := Main(TextFrames.Frame);
      IF M = TextFrames.caretF THEN
        TextFrames.Recall(buf);
        pos := TextFrames.carloc.pos + buf.len;
        Texts.Insert(M.text, TextFrames.carloc.pos, buf);
        TextFrames.SetCaret(M, pos)
      END
    ELSIF Menu IS TextFrames.Frame THEN
      M := Menu(TextFrames.Frame);
      IF M = TextFrames.caretF THEN
        TextFrames.Recall(buf);
        pos := TextFrames.carloc.pos + buf.len;
        Texts.Insert(M.text, TextFrames.carloc.pos, buf);
        TextFrames.SetCaret(M, pos)
      END
    END
  END
END Recall;

BEGIN Texts.OpenWriter(W)
END Edit.
