(* Source: https://github.com/Spirit-of-Oberon/wirth-personal
   Filename: people.inf.ethz.ch/wirth/ProjectOberon/Sources/System.mod.txt
   License: /licenses/oberon.txt *)
MODULE System;
IMPORT Files, Input, Display, Viewers, Fonts, Texts, Oberon, MenuViewers,
  TextFrames, Arg;
LOADFORTH oberon/system.pre

CONST
  StandardMenu = "System.Close System.Copy System.Grow Edit.Search Edit.Store";
  LogMenu = "Edit.Locate Edit.Search System.Copy System.Grow System.Clear";

VAR W: Texts.Writer;

PROCEDURE EndLine;
BEGIN Texts.WriteLn(W); Texts.Append(Oberon.Log, W.buf)
END EndLine;

PROCEDURE LogString*(s: ARRAY OF CHAR);
BEGIN Texts.WriteString(W, s); Texts.Append(Oberon.Log, W.buf); END LogString;

(* ------------- Toolbox for system control ---------------*)

PROCEDURE ShowCommands*;
VAR S: Texts.Scanner;
BEGIN
  Arg.GetArg(S);
  IF S.class = Texts.Name THEN DUSK.ShowCommands(S.s) END
END ShowCommands;

PROCEDURE RunForth*;
BEGIN
  IF Texts.Sel # NIL THEN
    DUSK.interpretstream(Texts.SelectionAsStream());
  END
END RunForth;

PROCEDURE RunForthLine*;
BEGIN
  IF TextFrames.caretF # NIL THEN
    DUSK.interpretstream(Texts.AsStream(
      TextFrames.caretF.text, TextFrames.carloc.org,
      TextFrames.carloc.org + TextFrames.carloc.lin.len));
  END
END RunForthLine;

(* ------------- Toolbox for standard display ---------------*)

PROCEDURE OpenSysViewer(DX: INTEGER; T: Texts.Text; name, menu: ARRAY OF CHAR);
VAR X, Y: INTEGER;
    V: Viewers.Viewer;
BEGIN
  Oberon.AllocateSystemViewer(DX, X, Y);
  V := MenuViewers.New(
    TextFrames.NewMenu(name, menu),
    TextFrames.NewText(T, 0), TextFrames.menuH, X, Y)
END OpenSysViewer;

PROCEDURE OpenTool*;
VAR S: Texts.Scanner;
    T: Texts.Text;
BEGIN
  Arg.GetArg(S);
  IF S.class = Texts.Name THEN
    T := TextFrames.Text(); Texts.Open(T, S.s);
    OpenSysViewer(Oberon.Par.vwr.X, T, S.s, StandardMenu)
  END
END OpenTool;

PROCEDURE Open*;
VAR T: Texts.Text;
    S: Texts.Scanner;
    V: Viewers.Viewer;
    X, Y: INTEGER;
BEGIN
  Arg.GetArg(S);
  IF S.class = Texts.Name THEN
    Oberon.AllocateUserViewer(Oberon.Par.vwr.X, X, Y);
    T := TextFrames.Text(); T.ascii := TRUE; Texts.Open(T, S.s);
    V := MenuViewers.New(
      TextFrames.NewMenu(S.s, StandardMenu),
      TextFrames.NewText(T, 0), TextFrames.menuH, X, Y)
  END
END Open;

PROCEDURE Clear*;  (*clear Log*)
VAR T: Texts.Text; F: Viewers.Frame; buf: Texts.Buffer;
BEGIN
  F := Oberon.Par.frame;
  IF (F # NIL) & (F.next IS TextFrames.Frame) & (F = Oberon.Par.vwr.dsc) THEN
    NEW(buf);
    Texts.OpenBuf(buf);
    T := F.next(TextFrames.Frame).text;
    Texts.Delete(T, 0, T.len, buf)
  END
END Clear;

PROCEDURE Close*;
VAR V: Viewers.Viewer;
BEGIN
  IF Oberon.Par.frame = Oberon.Par.vwr.dsc THEN V := Oberon.Par.vwr
  ELSE V := Oberon.MarkedViewer()
  END;
  Viewers.Close(V)
END Close;

PROCEDURE CloseTrack*;
VAR V: Viewers.Viewer;
BEGIN V := Oberon.MarkedViewer(); Viewers.CloseTrack(V.X)
END CloseTrack;

PROCEDURE Recall*;
VAR V: Viewers.Viewer; M: Viewers.ViewerMsg;
BEGIN Viewers.Recall(V);
  IF (V#NIL) & (V.state = 0) THEN
    Viewers.Open(V, V.X, V.Y + V.H);
     M.id := Viewers.restore;
     V.handle(V, M)
  END
END Recall;

PROCEDURE Copy*;
VAR V, V1: Viewers.Viewer; M: Oberon.CopyMsg; N: Viewers.ViewerMsg;
BEGIN V := Oberon.Par.vwr; V.handle(V, M); V1 := M.F(Viewers.Viewer);
  Viewers.Open(V1, V.X, V.Y + V.H DIV 2);
  N.id := Viewers.restore; V1.handle(V1, N)
END Copy;

PROCEDURE Grow*;
VAR V, V1: Viewers.Viewer; M: Oberon.CopyMsg; N: Viewers.ViewerMsg;
    DW, DH: INTEGER;
BEGIN V := Oberon.Par.vwr;
  DW := Oberon.DW; DH := Oberon.DH;
  IF V.H < DH - Viewers.minH THEN Oberon.OpenTrack(V.X, V.W)
  ELSIF V.W < DW THEN Oberon.OpenTrack(0, DW)
  END;
  IF (V.H < DH - Viewers.minH) OR (V.W < DW) THEN
    V.handle(V, M); V1 := M.F(Viewers.Viewer);
    Viewers.Open(V1, V.X, DH);;
    N.id := Viewers.restore; V1.handle(V1, N)
  END
END Grow;

PROCEDURE OpenViewers;
VAR T: Texts.Text;
BEGIN
  Texts.WriteString(W, "Duskberon"); EndLine;
  OpenSysViewer(0, Oberon.Log, "System Log", LogMenu);
  T := TextFrames.Text(); Texts.Open(T, "oberon/system.ort");
  OpenSysViewer(0, T, "oberon/system.ort", StandardMenu);
END OpenViewers;

PROCEDURE ExtendDisplay*;
VAR DX, DW, DH: INTEGER;
    S: Texts.Scanner;
    T: Texts.Text;
BEGIN
  Arg.GetArg(S);
  IF S.class = Texts.Name THEN
    DX := Viewers.curW; DW := Oberon.DW; DH := Oberon.DH;
    Oberon.OpenDisplay(DW DIV 8 * 5, DW DIV 8 * 3, DH);
    T := TextFrames.Text(); Texts.Open(T, S.s);
    OpenSysViewer(DX, T, S.s, StandardMenu)
  END
END ExtendDisplay;

BEGIN
  Texts.OpenWriter(W);
  Oberon.Log := TextFrames.Text();
  Texts.Open(Oberon.Log, "");
  OpenViewers;
END System.
