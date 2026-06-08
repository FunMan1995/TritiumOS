(* Source: https://github.com/Spirit-of-Oberon/wirth-personal
   Filename: people.inf.ethz.ch/wirth/ProjectOberon/Sources/Hilbert.mod.txt
   License: /licenses/oberon.txt *)
MODULE Hilbert;
IMPORT Display, Viewers, Texts, Oberon, MenuViewers, TextFrames;

CONST Menu = "System.Close  System.Copy  System.Grow";

VAR x, y, d: INTEGER;
    A, B, C, D: PROCEDURE (i: INTEGER);

PROCEDURE E;
BEGIN Display.FillBlock(Display.white, x, y, d, 1); INC(x, d)
END E;

PROCEDURE N;
BEGIN Display.FillBlock(Display.white, x, y, 1, d); INC(y, d)
END N;

PROCEDURE W;
BEGIN DEC(x, d); Display.FillBlock(Display.white, x, y, d, 1)
END W;

PROCEDURE S;
BEGIN DEC(y, d); Display.FillBlock(Display.white, x, y, 1, d)
END S;

PROCEDURE HA(i: INTEGER);
BEGIN
  IF i > 0 THEN D(i-1); W; A(i-1); S; A(i-1); E; B(i-1) END
END HA;

PROCEDURE HB(i: INTEGER);
BEGIN
  IF i > 0 THEN C(i-1); N; B(i-1); E; B(i-1); S; A(i-1) END
END HB;

PROCEDURE HC(i: INTEGER);
BEGIN
  IF i > 0 THEN B(i-1); E; C(i-1); N; C(i-1); W; D(i-1) END
END HC;

PROCEDURE HD(i: INTEGER);
BEGIN
  IF i > 0 THEN A(i-1); S; D(i-1); W; D(i-1); N; C(i-1) END
END HD;

PROCEDURE DrawHilbert(F: Viewers.Frame);
VAR k, n, w, x0, y0: INTEGER;
BEGIN
  k := 0; d := 8;
  IF F.W < F.H THEN w := F.W ELSE w := F.H END ;
  WHILE d*2 < w DO d := d*2; INC(k) END ;
  Display.FillBlock(Display.black, F.X, F.Y, F.W, F.H);
  x0 := F.W DIV 2; y0 := F.H DIV 2; n := 0;
  WHILE n < k DO
    d := d DIV 2; INC(x0, d DIV 2); INC(y0, d DIV 2);
    x := F.X + x0; y := F.Y + y0; INC(n); HA(n)
  END
END DrawHilbert;

PROCEDURE Handler(F: Viewers.Frame; VAR M: Viewers.FrameMsg);
VAR F0: Viewers.Frame;
BEGIN
  CASE M OF
  Oberon.MouseMsg: Oberon.DrawMouseArrow(M.X, M.Y) |
  MenuViewers.ModifyMsg:
    F.Y := M.Y;
    F.H := M.H;
    DrawHilbert(F) |
  Oberon.ControlMsg:
    IF M.id = Oberon.neutralize THEN
      Oberon.RemoveMarks(F.X, F.Y, F.W, F.H)
    END |
  Oberon.CopyMsg:
    NEW(F0); F0^ := F^; M.F := F0
  END
END Handler;

PROCEDURE New(): Viewers.Frame;
VAR F: Viewers.Frame;
BEGIN NEW(F); F.handle := Handler; RETURN F
END New;

PROCEDURE Draw*;
VAR V: Viewers.Viewer; X, Y: INTEGER;
BEGIN
  Oberon.AllocateUserViewer(Oberon.Par.vwr.X, X, Y);
  V := MenuViewers.New(
    TextFrames.NewMenu("Hilbert", Menu), New(), TextFrames.menuH, X, Y)
END Draw;
  
BEGIN A := HA; B := HB; C := HC; D := HD
END Hilbert.
