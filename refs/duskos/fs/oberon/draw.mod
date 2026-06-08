MODULE Draw; (*NW 29.6.88 / 12.11.94 / 18.11.2013*)

  IMPORT Fonts, Viewers, Texts, Oberon,
    TextFrames, MenuViewers, Graphics, GraphicFrames;

  CONST Menu = "System.Close  System.Copy  System.Grow  Draw.Delete  Draw.Ticks  Draw.Restore  Draw.Store";

  VAR W: Texts.Writer;

  (*Exported commands:
    Open, Delete,
    SetWidth, ChangeColor, ChangeWidth, ChangeFont,
    Store, Print, Macro, Ticks, Restore*)

  PROCEDURE Open*;
    VAR X, Y: INTEGER;
      beg, end: INTEGER;
      G: Graphics.Graph;
      F: GraphicFrames.Frame;
      V: Viewers.Viewer;
      S: Texts.Scanner;
      text: Texts.Text;
  BEGIN Texts.OpenScanner(S, Oberon.Par.text, Oberon.Par.pos); Texts.Scan(S);
    IF (S.class = Texts.Char) & (S.c = '^') THEN
      TextFrames.GetSelection(text, beg, end);
      IF text # NIL THEN Texts.OpenScanner(S, text, beg); Texts.Scan(S) END
    END ;
    IF S.class = Texts.Name THEN
      NEW(G); Graphics.Open(G, S.s);
      NEW(F); GraphicFrames.Open(F, G);
      Oberon.AllocateUserViewer(Oberon.Par.vwr.X, X, Y);
      V := MenuViewers.New(TextFrames.NewMenu(S.s, Menu), F, TextFrames.menuH, X, Y)
    END
  END Open;

  PROCEDURE Delete*;
    VAR F: GraphicFrames.Frame;
  BEGIN
    IF Oberon.Par.frame = Oberon.Par.vwr.dsc THEN
      F := Oberon.Par.vwr.dsc.next(GraphicFrames.Frame);
      GraphicFrames.Erase(F); Graphics.Delete(F.graph)
    END
  END Delete;

  PROCEDURE GetArg(VAR S: Texts.Scanner);
    VAR T: Texts.Text; beg, end, time: INTEGER;
  BEGIN Texts.OpenScanner(S, Oberon.Par.text, Oberon.Par.pos); Texts.Scan(S);
    IF (S.class = Texts.Char) & (S.c = '^') THEN
      TextFrames.GetSelection(T, beg, end);
      IF T # NIL THEN Texts.OpenScanner(S, T, beg); Texts.Scan(S) END
    END
  END GetArg;

  PROCEDURE SetWidth*;
    VAR S: Texts.Scanner;
  BEGIN GetArg(S);
    IF (S.class = Texts.Int) & (S.n > 0) & (S.n < 7) THEN Graphics.SetWidth(S.n) END
  END SetWidth;

  PROCEDURE ChangeColor*;
    VAR S: Texts.Scanner; CM: Graphics.ColorMsg;
  BEGIN GetArg(S);
    IF S.class = Texts.Int THEN
      CM.col := S.n MOD 16; GraphicFrames.Change(GraphicFrames.Selected(), CM)
    END
  END ChangeColor;

  PROCEDURE ChangeWidth*;
    VAR S: Texts.Scanner; WM: Graphics.WidMsg;
  BEGIN GetArg(S);
    IF S.class = Texts.Int THEN
      WM.w := S.n; GraphicFrames.Change(GraphicFrames.Selected(), WM)
    END
  END ChangeWidth;

  PROCEDURE ChangeFont*;
    VAR S: Texts.Scanner; FM: Graphics.FontMsg;
  BEGIN GetArg(S);
    IF S.class = Texts.Name THEN
      FM.fnt := Fonts.This(S.s);
      IF FM.fnt # NIL THEN GraphicFrames.Change(GraphicFrames.Selected(), FM) END
    END
  END ChangeFont;

  PROCEDURE Redraw(Q: BOOLEAN);
    VAR v: Viewers.Viewer; G: GraphicFrames.Frame;
  BEGIN
    IF Oberon.Par.frame = Oberon.Par.vwr.dsc THEN v := Oberon.Par.vwr
    ELSE v := Oberon.MarkedViewer()
    END ;
    IF (v # NIL) & (v.dsc # NIL) & (v.dsc.next IS GraphicFrames.Frame) THEN
      G := v.dsc.next(GraphicFrames.Frame); G.ticked := Q OR ~G.ticked; GraphicFrames.Restore(G)
    END
  END Redraw;

  PROCEDURE Ticks*;
  BEGIN Redraw(FALSE)
  END Ticks;

  PROCEDURE Restore*;
  BEGIN Redraw(TRUE)
  END Restore;

  PROCEDURE Store*;
    VAR S: Texts.Scanner;
      Menu: TextFrames.Frame; G: GraphicFrames.Frame;
      v: Viewers.Viewer;
  BEGIN
    IF Oberon.Par.frame = Oberon.Par.vwr.dsc THEN
      Menu := Oberon.Par.vwr.dsc(TextFrames.Frame); G := Menu.next(GraphicFrames.Frame);
      Texts.OpenScanner(S, Menu.text, 0); Texts.Scan(S);
      IF S.class = Texts.Name THEN
        Texts.WriteString(W, S.s); Texts.WriteString(W, " storing");
        Texts.WriteLn(W); Texts.Append(Oberon.Log, W.buf);
        GraphicFrames.Store(G, S.s)
      END
    ELSE
      Texts.OpenScanner(S, Oberon.Par.text, Oberon.Par.pos); Texts.Scan(S);
      IF S.class = Texts.Name THEN
        v := Oberon.MarkedViewer();
        IF (v.dsc # NIL) & (v.dsc.next IS GraphicFrames.Frame) THEN
          G := v.dsc.next(GraphicFrames.Frame);
          Texts.WriteString(W, S.s); Texts.WriteString(W, " storing");
          Texts.WriteLn(W); Texts.Append(Oberon.Log, W.buf);
          GraphicFrames.Store(G, S.s)
        END
      END
    END
  END Store;

  PROCEDURE Macro*;
    VAR S: Texts.Scanner;
      T: Texts.Text;
      time, beg, end: INTEGER;
      Lname: ARRAY 32 OF CHAR;
  BEGIN Texts.OpenScanner(S, Oberon.Par.text, Oberon.Par.pos); Texts.Scan(S);
    IF S.class = Texts.Name THEN
      Lname := S.s; Texts.Scan(S);
      IF S.class = Texts.Name THEN GraphicFrames.Macro(Lname, S.s) END ;
    END
  END Macro;

  PROCEDURE SetColor*;
    VAR S: Texts.Scanner;
  BEGIN GetArg(S);
    IF S.class = Texts.Int THEN GraphicFrames.SetFntCol(GraphicFrames.OberonCurFont, S.n) END
  END SetColor;

  PROCEDURE SetFont*;
    VAR S: Texts.Scanner;
  BEGIN GetArg(S);
    IF S.class = Texts.Name THEN GraphicFrames.SetFntCol(Fonts.This(S.s), GraphicFrames.OberonCurCol) END
  END SetFont;

BEGIN Texts.OpenWriter(W); Texts.WriteString(W, "Draw - NW 9.8.2013");
  Texts.WriteLn(W); Texts.Append(Oberon.Log, W.buf)
END Draw.
