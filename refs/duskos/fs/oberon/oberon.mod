(* Source: https://github.com/Spirit-of-Oberon/wirth-personal
   Filename: people.inf.ethz.ch/wirth/ProjectOberon/Sources/Oberon.mod.txt
   License: /licenses/oberon.txt *)
MODULE Oberon;
IMPORT Files, Input, Display, Viewers, Texts;

CONST (*message ids*)
  defocus* = 0; neutralize* = 1; mark* = 2;
  off = 0; idle = 1; active = 2;   (*task states*)
  BasicCycle = 20;
  ESC = 1BX; SETSTAR = 09X;

TYPE
  Marker* = RECORD patadr, DX, DY: INTEGER END;
  
  Command* = PROCEDURE;

  Cursor* = RECORD
    marker: Marker; on: BOOLEAN; X, Y: INTEGER
  END;

  MouseMsg* = RECORD(Viewers.FrameMsg)
    keys: SET;
    X, Y: INTEGER
  END;

  KeyMsg* = RECORD(Viewers.FrameMsg)
    ch: CHAR
  END;

  ControlMsg* = RECORD(Viewers.FrameMsg)
    id, X, Y: INTEGER
  END;

  CopyMsg* = RECORD(Viewers.FrameMsg)
    F: Viewers.Frame
  END;

LOADFORTH oberon/oberon.pre

VAR 
  Arrow*, Star*: Marker;
  Mouse, Pointer: Cursor;
  FocusViewer*: Viewers.Viewer;
  Log*: Texts.Text;

  Par*: RECORD
    vwr: Viewers.Viewer;
    frame: Viewers.Frame;
    text: Texts.Text;
    pos: INTEGER
  END;

  BackgroundColor*, TextBackgroundColor*, FrameColor*, CursorColor*,
  ScrollMarkColor*, ChangeMarkColor*, SelectionColor*, MenuBackgroundColor*,
  ScrollBarColor*, UnderlineColor*: INTEGER;
  MixTextColors*: BOOLEAN;

  DW*, DH*: INTEGER;

  prevX, prevY: INTEGER; (* previous mouse position *)

(*cursor handling*)

PROCEDURE FlipMarker(VAR m: Marker; X, Y: INTEGER);
VAR pw, ph: BYTE;
BEGIN
  X := X - m.DX;
  Y := Y - m.DY;
  GET(m.patadr+1, ph); GET(m.patadr, pw);
  IF X > DW - pw THEN X := DW - pw END;
  IF Y < 0 THEN Y := 0 ELSIF Y > DH - ph THEN Y := DH - ph END;
  Display.InvertPattern(CursorColor, m.patadr, X, Y)
END FlipMarker;

PROCEDURE OpenCursor(VAR c: Cursor);
BEGIN c.on := FALSE; c.X := 0; c.Y := 0
END OpenCursor;
 
PROCEDURE FadeCursor(VAR c: Cursor);
BEGIN
  IF c.on THEN FlipMarker(c.marker, c.X, c.Y); c.on := FALSE END
END FadeCursor;

PROCEDURE DrawCursor(VAR c: Cursor; VAR m: Marker; x, y: INTEGER);
BEGIN
  IF c.on & ((x # c.X) OR (y # c.Y) OR (m # c.marker)) THEN
    FlipMarker(c.marker, c.X, c.Y);
    c.on := FALSE
  END;
  IF ~c.on THEN
    FlipMarker(m, x, y);
    c.marker := m; c.X := x; c.Y := y; c.on := TRUE
  END
END DrawCursor;

PROCEDURE DrawMouse*(VAR m: Marker; x, y: INTEGER);
BEGIN DrawCursor(Mouse, m, x, y)
END DrawMouse;

PROCEDURE DrawMouseArrow*(x, y: INTEGER);
BEGIN DrawCursor(Mouse, Arrow, x, y)
END DrawMouseArrow;

PROCEDURE FadeMouse*;
BEGIN FadeCursor(Mouse)
END FadeMouse;

PROCEDURE DrawPointer*(x, y: INTEGER);
BEGIN DrawCursor(Pointer, Star, x, y)
END DrawPointer;

(*display management*)

PROCEDURE RemoveMarks*(X, Y, W, H: INTEGER);
BEGIN
  IF (Mouse.X > X - 16) & (Mouse.X < X + W + 16)
     & (Mouse.Y > Y - 16) & (Mouse.Y < Y + H + 16) THEN
    FadeCursor(Mouse)
  END;
  IF (Pointer.X > X - 8) & (Pointer.X < X + W + 8)
     & (Pointer.Y > Y - 8) & (Pointer.Y < Y + H + 8) THEN
    FadeCursor(Pointer)
  END
END RemoveMarks;

PROCEDURE HandleFiller(V: Viewers.Frame; VAR M: Viewers.FrameMsg);
BEGIN
  CASE M OF
  MouseMsg: DrawCursor(Mouse, Arrow, M.X, M.Y) |
  ControlMsg: IF M.id = mark THEN DrawCursor(Pointer, Star, M.X, M.Y) END |
  Viewers.ViewerMsg:
    IF (M.id = Viewers.restore) & (V.W > 0) & (V.H > 0) THEN
      RemoveMarks(V.X, V.Y, V.W, V.H);
      Display.FillBlock(BackgroundColor, V.X, V.Y, V.W, V.H)
    ELSIF (M.id = Viewers.modify) & (M.Y < V.Y) THEN
      RemoveMarks(V.X, M.Y, V.W, V.Y - M.Y);
      Display.FillBlock(BackgroundColor, V.X, M.Y, V.W, V.Y - M.Y)
    END
  END
END HandleFiller;

PROCEDURE OpenDisplay*(UW, SW, H: INTEGER);
VAR Filler: Viewers.Viewer;
BEGIN
   Input.SetMouseLimits(Viewers.curW + UW + SW, H);
   Display.FillBlock(BackgroundColor, Viewers.curW, 0, UW + SW, H);
   NEW(Filler); Filler.handle := HandleFiller;
   Viewers.InitTrack(UW, H, Filler); (*init user track*)
   NEW(Filler); Filler.handle := HandleFiller;
   Viewers.InitTrack(SW, H, Filler) (*init system track*)
END OpenDisplay;

PROCEDURE OpenTrack*(X, W: INTEGER);
VAR Filler: Viewers.Viewer;
BEGIN
  NEW(Filler); Filler.handle := HandleFiller;
  Viewers.OpenTrack(X, W, Filler)
END OpenTrack;

PROCEDURE UY(X: INTEGER): INTEGER;
VAR h: INTEGER;
    fil, bot, alt, max: Viewers.Frame;
BEGIN
  Viewers.Locate(X, 0, fil, bot, alt, max);
  IF fil.H >= DH DIV 8 THEN h := DH ELSE h := max.Y + max.H DIV 2 END ;
  RETURN h
END UY;

PROCEDURE AllocateUserViewer*(DX: INTEGER; VAR X, Y: INTEGER);
BEGIN
  IF Pointer.on THEN X := Pointer.X; Y := Pointer.Y
  ELSE X := DX DIV DW * DW; Y := UY(X)
  END
END AllocateUserViewer;

PROCEDURE SY(X: INTEGER): INTEGER;
VAR H0, H1, H2, H3, y: INTEGER;
    fil, bot, alt, max: Viewers.Frame;
BEGIN H3 := DH - DH DIV 3;
  H2 := H3 - H3 DIV 2; H1 := DH DIV 5; H0 := DH DIV 10;
  Viewers.Locate(X, DH, fil, bot, alt, max);
  IF fil.H >= DH DIV 8 THEN y := DH
  ELSIF max.H >= DH - H0 THEN y := max.Y + H3
  ELSIF max.H >= H3 - H0 THEN y := max.Y + H2
  ELSIF max.H >= H2 - H0 THEN y := max.Y + H1
  ELSIF max # bot THEN y := max.Y + max.H DIV 2
  ELSIF bot.H >= H1 THEN y := bot.H DIV 2
  ELSE y := alt.Y + alt.H DIV 2
  END ;
  RETURN y
END SY;

PROCEDURE AllocateSystemViewer*(DX: INTEGER; VAR X, Y: INTEGER);
BEGIN
  IF Pointer.on THEN X := Pointer.X; Y := Pointer.Y
  ELSE X := DX DIV DW * DW + DW DIV 8 * 5; Y := SY(X)
  END
END AllocateSystemViewer;

PROCEDURE MarkedViewer*(): Viewers.Viewer;
BEGIN RETURN Viewers.This(Pointer.X, Pointer.Y)
END MarkedViewer;

PROCEDURE PassFocus*(V: Viewers.Viewer);
VAR M: ControlMsg;
BEGIN M.id := defocus; FocusViewer.handle(FocusViewer, M); FocusViewer := V
END PassFocus;

(*command interpretation*)
PROCEDURE SetPar*(F: Viewers.Frame; T: Texts.Text; pos: INTEGER);
BEGIN
  Par.vwr := Viewers.This(F.X, F.Y); Par.frame := F; Par.text := T;
  Par.pos := pos
END SetPar;

PROCEDURE UpdateScreenSize*();
BEGIN
  Viewers.UpdateScreenSize(DW, DH, Display.Width, Display.Height);
  DW := Display.Width; DH := Display.Height;
  Input.SetMouseLimits(DW, DH);
END UpdateScreenSize;

PROCEDURE TypeChar*(ch: CHAR);
VAR V: Viewers.Viewer; K: KeyMsg; N: ControlMsg;
    X, Y: INTEGER; keys: SET;
BEGIN
    IF ch = ESC THEN
      N.id := neutralize; Viewers.Broadcast(N); FadeCursor(Pointer);
    ELSIF ch = SETSTAR THEN
      Input.Mouse(keys, X, Y);
      N.id := mark; N.X := X; N.Y := Y; V := Viewers.This(X, Y); V.handle(V, N)
    ELSE
      K.ch := ch;
      FocusViewer.handle(FocusViewer, K);
    END
END TypeChar;

PROCEDURE ProcessMouse*(keys: SET; X, Y: INTEGER);
VAR V: Viewers.Viewer; M: MouseMsg; 
BEGIN
  IF keys # {} THEN
    M.X := X; M.Y := Y; M.keys := keys;
    REPEAT
      V := Viewers.This(M.X, M.Y); V.handle(V, M); Input.Mouse(M.keys, M.X, M.Y)
    UNTIL M.keys = {};
  ELSE
    IF (X # prevX) OR (Y # prevY) OR ~Mouse.on THEN
      M.X := X; 
      M.Y := Y; M.keys := keys;
      V := Viewers.This(X, Y);
      V.handle(V, M);
      prevX := X; prevY := Y
    END;
  END
END ProcessMouse;

BEGIN
  BackgroundColor := 12; FrameColor := 0;
  CursorColor := Display.white; ScrollMarkColor := 1; ChangeMarkColor := Display.white;
  SelectionColor := Display.white; MenuBackgroundColor := 13;
  ScrollBarColor := 3; UnderlineColor := Display.white;
  TextBackgroundColor := 14;
  MixTextColors := TRUE;

  Arrow.patadr := Display.arrow; Arrow.DX := 0; Arrow.DY := 14;
  Star.patadr := Display.star; Star.DX := 7; Star.DY := 7;
  OpenCursor(Mouse); OpenCursor(Pointer);

  DW := Display.Width; DH := Display.Height;
  OpenDisplay(DW DIV 16 * 9, DW DIV 16 * 7, DH);
  FocusViewer := Viewers.This(0, 0);
END Oberon.
