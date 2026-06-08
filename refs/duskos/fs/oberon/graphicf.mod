MODULE GraphicFrames; (*NW 18.4.88 / 18.11.2013 / 27.8.2018*)
  IMPORT Display, Viewers, Input, Fonts, Texts, Oberon, TextFrames, Graphics, MenuViewers;

  CONST (*update message ids*)
    drawobj = 1; drawobjs = 2; drawobjd = 3;
    drawnorm = 4; drawsel = 5; drawdel = 6;

    markW = 5;

  TYPE
    Frame* = POINTER TO FrameDesc;
    Location* = POINTER TO LocDesc;

    LocDesc* = RECORD
        x, y: INTEGER;
        next: Location
      END ;

    FrameDesc* = RECORD (Viewers.FrameDesc)
        graph: Graphics.Graph;
        Xg, Yg: INTEGER;  (*pos rel to graph origin*)
        X1, Y1: INTEGER;  (*right and upper margins*)
        x, y, col: INTEGER;  (*x = X + Xg, y = Y + Yg*)
        marked, ticked: BOOLEAN;
        mark: LocDesc
      END ;

    DrawMsg* = RECORD (Graphics.Msg)
        f: Frame;
        x, y, col, mode: INTEGER
      END ;

    UpdateMsg = RECORD (Viewers.FrameMsg)
        id: INTEGER;
        graph: Graphics.Graph;
        obj: Graphics.Object
      END ;

    ChangedMsg = RECORD (Viewers.FrameMsg)
        f: Frame;
        graph: Graphics.Graph;
        mode: INTEGER
      END ;

    SelQuery = RECORD (Viewers.FrameMsg)
        f: Frame
      END ;

    FocusQuery = RECORD (Viewers.FrameMsg)
        f: Frame
      END ;

    PosQuery = RECORD (Viewers.FrameMsg)
        f: Frame; x, y: INTEGER
      END ;

    DispMsg = RECORD (Viewers.FrameMsg)
        x1, y1, w: INTEGER;
        pat: INTEGER;
        graph: Graphics.Graph
      END ;

    PPat = POINTER TO DPat;
    DPat = RECORD next: PPat; x: ARRAY 5 OF INTEGER END;

  VAR Crosshair*: Oberon.Marker;
    LastPattern: PPat;
    tack*, dotted*, dotted1*: INTEGER;  (*patterns*)
    newcap: Graphics.Caption;
    TBuf: Texts.Buffer;
    CL: INTEGER;
    W: Texts.Writer;
    OberonCurCol*: INTEGER;
    OberonCurFont*: Fonts.Font;

  (*Exported procedures:
    Restore, Focus, Selected, This, Draw, DrawNorm, Erase,
    DrawObj, EraseObj, Change, Defocus, Deselect, Macro, Open*)


  PROCEDURE SetChangeMark(F: Frame; col: INTEGER); (*set mark in corner of frame*)
  BEGIN
    IF F.H > 16 THEN
      IF col = 0 THEN Display.FillBlock(Display.black, F.X+F.W-12, F.Y+F.H-12, 8, 8)
      ELSE Display.PaintPattern(Display.white, Display.block, F.X+F.W-12, F.Y+F.H-12)
      END
    END
  END SetChangeMark;

  PROCEDURE Restore*(F: Frame);
    VAR x, x0, y: INTEGER; M: DrawMsg;
  BEGIN F.X1 := F.X + F.W; F.Y1 := F.Y + F.H;
    F.x := (F.X + F.Xg) DIV 16 * 16; F.y := (F.Y + F.Yg) DIV 16 * 16; F.marked := FALSE; F.mark.next := NIL;
    Oberon.RemoveMarks(F.X, F.Y, F.W, F.H); Display.FillBlock(F.col, F.X, F.Y, F.W, F.H);
    IF F.ticked THEN
      x0 := (F.X + 15) DIV 16 * 16; y := (F.Y + 15) DIV 16 * 16;
      WHILE y < F.Y1 DO
        x := x0;
        WHILE x < F.X1 DO Display.Dot(Display.white, x, y); INC(x, 16) END ;
        INC(y, 16)
      END
    END ;
    M.f := F; M.x := F.x; M.y := F.y; M.col := 0; M.mode := 0; Graphics.Draw(F.graph, M);
    IF F.graph.changed THEN SetChangeMark(F, 1) END
  END Restore;

  PROCEDURE Focus*(): Frame;
    VAR FQ: FocusQuery;
  BEGIN FQ.f := NIL; Viewers.Broadcast(FQ); RETURN FQ.f
  END Focus;

  PROCEDURE Selected*(): Frame;
    VAR SQ: SelQuery;
  BEGIN SQ.f := NIL; Viewers.Broadcast(SQ); RETURN SQ.f
  END Selected;

  PROCEDURE This*(x, y: INTEGER): Frame;
    VAR PQ: PosQuery;
  BEGIN PQ.f := NIL; PQ.x := x; PQ.y := y; Viewers.Broadcast(PQ); RETURN PQ.f
  END This;

  PROCEDURE Mark(F: Frame; mode: INTEGER);
    VAR CM: ChangedMsg;
  BEGIN CM.f := F; CM.graph := F.graph; CM.mode := mode; Viewers.Broadcast(CM)
  END Mark;

  PROCEDURE Draw*(F: Frame);
    VAR UM: UpdateMsg;
  BEGIN UM.id := drawsel; UM.graph := F.graph; Viewers.Broadcast(UM)
  END Draw;

  PROCEDURE DrawNorm(F: Frame);
    VAR UM: UpdateMsg;
  BEGIN UM.id := drawnorm; UM.graph := F.graph; Viewers.Broadcast(UM)
  END DrawNorm;

  PROCEDURE Erase*(F: Frame);
    VAR UM: UpdateMsg;
  BEGIN UM.id := drawdel; UM.graph := F.graph; Viewers.Broadcast(UM); Mark(F, 1)
  END Erase;

  PROCEDURE DrawObj*(F: Frame; obj: Graphics.Object);
    VAR UM: UpdateMsg;
  BEGIN UM.id := drawobj; UM.graph := F.graph; UM.obj := obj; Viewers.Broadcast(UM)
  END DrawObj;

  PROCEDURE EraseObj*(F: Frame; obj: Graphics.Object);
    VAR UM: UpdateMsg;
  BEGIN UM.id := drawobjd; UM.graph := F.graph; UM.obj := obj; Viewers.Broadcast(UM)
  END EraseObj;

  PROCEDURE Change*(F: Frame; VAR msg: Graphics.Msg);
  BEGIN
    IF F # NIL THEN Erase(F); Graphics.Change(F.graph, msg); Draw(F) END
  END Change;

  PROCEDURE FlipMark(x, y: INTEGER);
  BEGIN
    Display.InvertBlock(Display.white, x-7, y, 15, 1);
    Display.InvertBlock(Display.white, x, y-7, 1, 15)
  END FlipMark;

  PROCEDURE Defocus*(F: Frame);
    VAR m: Location;
  BEGIN newcap := NIL;
    IF F.marked THEN
      FlipMark(F.mark.x, F.mark.y); m := F.mark.next;
      WHILE m # NIL DO FlipMark(m.x, m.y); m := m.next END ;
      F.marked := FALSE; F.mark.next := NIL
    END
  END Defocus;

  PROCEDURE Deselect*(F: Frame);
    VAR UM: UpdateMsg;
  BEGIN
    IF F # NIL THEN
      UM.id := drawnorm; UM.graph := F.graph; Viewers.Broadcast(UM);
      Graphics.Deselect(F.graph)
    END
  END Deselect;

  PROCEDURE Macro*(Lname, Mname: ARRAY OF CHAR);
    VAR x, y: INTEGER;
      F: Frame;
      mac: Graphics.Macro; mh: Graphics.MacHead;
      L: Graphics.Library;
  BEGIN F := Focus();
    IF F # NIL THEN
      x := F.mark.x - F.x; y := F.mark.y - F.y;
      Graphics.GetLib(Lname, FALSE, L);
      IF L # NIL THEN
        mh := Graphics.ThisMac(L, Mname);
        IF mh # NIL THEN
          Deselect(F); Defocus(F);
          NEW(mac); mac.x := x; mac.y := y; mac.w := mh.w; mac.h := mh.h;
          mac.mac := mh; mac.do := Graphics.MacMethod; mac.col := OberonCurCol;
          Graphics.Add(F.graph, mac); DrawObj(F, mac); Mark(F, 1)
        END
      ELSE Texts.WriteString(W, Lname); Texts.WriteString(W, " not available");
        Texts.WriteLn(W); Texts.Append(Oberon.Log, W.buf)
      END
    END
  END Macro;

  PROCEDURE CaptionCopy(F: Frame;
      x1, y1: INTEGER; T: Texts.Text; beg, end: INTEGER): Graphics.Caption;
    VAR ch: CHAR;
      dx, w, x2, y2, w1, h1: INTEGER;
      cap: Graphics.Caption;
      pat: INTEGER;
      R: Texts.Reader;
  BEGIN Texts.Write(W, 0DX);
    NEW(cap); cap.len := end - beg;
    cap.pos := Graphics.T.len + 1; cap.do := Graphics.CapMethod;
    Texts.OpenReader(R, T, beg); Texts.Read(R, ch); W.fnt := R.fnt; W.col := R.col; w := 0;
    cap.x := x1 - F.x; cap.y := y1 - F.y + R.fnt.minY;
    WHILE beg < end DO
      Fonts.GetPat(R.fnt, ch, dx, x2, y2, w1, h1, pat);
      INC(w, dx); INC(beg); Texts.Write(W, ch); Texts.Read(R, ch)
    END ;
    cap.w := w; cap.h := W.fnt.height; cap.col := W.col;
    Texts.Append(Graphics.T, W.buf); Graphics.Add(F.graph, cap);
    Mark(F, 1); RETURN cap
  END CaptionCopy;

  PROCEDURE NewLine(F: Frame; G: Graphics.Graph; x, y, w, h: INTEGER);
    VAR line: Graphics.Line;
  BEGIN NEW(line); line.col := OberonCurCol; line.x := x - F.x; line.y := y - F.y;
    line.w := w; line.h := h; line.do := Graphics.LineMethod;
    Graphics.Add(G, line); Mark(F, 1)
  END NewLine;

  PROCEDURE Edit(F: Frame; x0, y0: INTEGER; k0: SET);
    VAR obj: Graphics.Object;
      x1, y1, w, h, t: INTEGER;
      beg, end: INTEGER;
      k1, k2: SET;
      mark, newmark: Location;
      T: Texts.Text;
      Fd: Frame;
      G: Graphics.Graph;
  BEGIN k1 := k0; G := F.graph;
    REPEAT Input.Mouse(k2, x1, y1); k1 := k1 + k2;
      DEC(x1, (x1-F.x) MOD 4); DEC(y1, (y1-F.y) MOD 4);
      Oberon.DrawMouse(Crosshair, x1, y1)
    UNTIL  k2 = {};
    Oberon.FadeMouse;
    IF k0 = {2} THEN (*left key*)
      w := ABS(x1-x0); h := ABS(y1-y0);
      IF k1 = {2} THEN
        IF (w < 7) & (h < 7) THEN (*set mark*)
          IF (x1 - markW >= F.X) & (x1 + markW < F.X1) &
            (y1 - markW >= F.Y) & (y1 + markW < F.Y1) THEN
            Defocus(F); Oberon.PassFocus(Viewers.This(F.X, F.Y));
            F.mark.x := x1; F.mark.y := y1; F.marked := TRUE; FlipMark(x1, y1)
          END
        ELSE (*draw line*) Deselect(F);
          IF w < h THEN
            IF y1 < y0 THEN y0 := y1 END ;
            NewLine(F, G, x0, y0, Graphics.width, h)
          ELSE
            IF x1 < x0 THEN x0 := x1 END ;
            NewLine(F, G, x0, y0, w, Graphics.width)
          END ;
          Draw(F)
        END
      ELSIF k1 = {2, 1} THEN (*copy text selection to mark*)
        Deselect(F); TextFrames.GetSelection(T, beg, end);
        IF T # NIL THEN
          DrawObj(F, CaptionCopy(F, x1, y1, T, beg, end)); Mark(F, 1)
        END
      ELSIF k1 = {2, 0} THEN
        IF F.marked THEN (*set secondary mark*)
            NEW(newmark); newmark.x := x1; newmark.y := y1; newmark.next := NIL;
          FlipMark(x1, y1); mark := F.mark.next;
          IF mark = NIL THEN F.mark.next := newmark ELSE
            WHILE mark.next # NIL DO mark := mark.next END ;
            mark.next := newmark
          END
        END
      END
    ELSIF k0 = {1} THEN (*middle key*)
      IF k1 = {1} THEN (*move*)
        IF (x0 # x1) OR (y0 # y1) THEN
          Fd := This(x1, y1); Erase(F);
          IF Fd = F THEN Graphics.Move(G, x1-x0, y1-y0)
          ELSIF (Fd # NIL) & (Fd.graph = G) THEN
            Graphics.Move(G, (x1-Fd.x-x0+F.x) DIV 4 * 4, (y1-Fd.y-y0+F.y) DIV 4 * 4)
          END ;
          Draw(F); Mark(F, 1)
        END
      ELSIF k1 = {1, 2} THEN (*copy*)
        Fd := This(x1, y1);
        IF Fd # NIL THEN DrawNorm(F);
          IF Fd = F THEN Graphics.Copy(G, G, x1-x0, y1-y0)
          ELSE Deselect(Fd);
            Graphics.Copy(G, Fd.graph, (x1-Fd.x-x0+F.x) DIV 4 * 4, (y1-Fd.y-y0+F.y) DIV 4 * 4)
          END ;
          Draw(Fd); Mark(F, 1)
        END
      ELSIF k1 = {1, 0} THEN (*shift graph*)
        INC(F.Xg, x1-x0); INC(F.Yg, y1-y0); Restore(F)
      END
    ELSIF k0 = {0} THEN (*right key: select*)
      newcap := NIL;
      IF k1 = {0} THEN Deselect(F) END ;
      IF (ABS(x0-x1) < 7) & (ABS(y0-y1) < 7) THEN
        obj := Graphics.ThisObj(G, x1 - F.x, y1 - F.y);
        IF obj # NIL THEN Graphics.SelectObj(G, obj); DrawObj(F, obj) END
      ELSE
        IF x1 < x0 THEN t := x0; x0 := x1; x1 := t END ;
        IF y1 < y0 THEN t := y0; y0 := y1; y1 := t END ;
        Graphics.SelectArea(G, x0 - F.x, y0 - F.y, x1 - F.x, y1 - F.y); Draw(F)
      END
    END
  END Edit;

  PROCEDURE NewCaption(F: Frame; col: INTEGER; font: Fonts.Font);
  BEGIN Texts.Write(W, 0DX);
    NEW(newcap); newcap.x := F.mark.x - F.x; newcap.y := F.mark.y - F.y + font.minY;
    newcap.w := 0; newcap.h := font.height; newcap.col := col;
    newcap.pos := Graphics.T.len + 1; newcap.len := 0; newcap.do := Graphics.CapMethod;
    Graphics.Add(F.graph, newcap); W.fnt := font; ; Mark(F, 1)
  END NewCaption;

  PROCEDURE InsertChar(F: Frame; ch: CHAR);
    VAR w1, h1: INTEGER; DM: DispMsg;
  BEGIN DM.graph := F.graph;
    Fonts.GetPat(W.fnt, ch, DM.w, DM.x1, DM.y1, w1, h1, DM.pat); DEC(DM.y1, W.fnt.minY);
    IF h1 = 0 THEN DM.pat := 0 END;
    IF newcap.x + newcap.w + DM.w + F.x < F.X1 THEN
      Viewers.Broadcast(DM); INC(newcap.w, DM.w); INC(newcap.len); Texts.Write(W, ch)
    END ;
    Texts.Append(Graphics.T, W.buf)
  END InsertChar;

  PROCEDURE DeleteChar(F: Frame);
    VAR w1, h1: INTEGER; ch: CHAR; pos: INTEGER;
      DM: DispMsg; R: Texts.Reader;
  BEGIN DM.graph := F.graph;
    IF newcap.len > 0 THEN
      pos := Graphics.T.len; Texts.OpenReader(R, Graphics.T, pos-1);  (*backspace*)
      Texts.Read(R, ch);
      IF ch >= ' ' THEN
        Fonts.GetPat(R.fnt, ch, DM.w, DM.x1, DM.y1, w1, h1, DM.pat);
        DEC(newcap.w, DM.w); DEC(newcap.len); DEC(DM.y1, R.fnt.minY);
        IF h1 = 0 THEN DM.pat := 0 END;
        Viewers.Broadcast(DM); Texts.Delete(Graphics.T, pos-1, pos, TBuf)
      END
    END
  END DeleteChar;

  PROCEDURE Handle*(G: Viewers.Frame; VAR M: Viewers.FrameMsg);
    VAR x, y, h: INTEGER;
      DM: DispMsg; dM: DrawMsg;
      G1: Frame; loc: Location; obj: Graphics.Object;
  BEGIN
    CASE G OF Frame:
      CASE M OF
      Oberon.MouseMsg:
        x := M.X - (M.X - G.x) MOD 4; y := M.Y - (M.Y - G.y) MOD 4;
        IF M.keys # {} THEN Edit(G, x, y, M.keys) ELSE Oberon.DrawMouse(Crosshair, x, y) END
      | Oberon.KeyMsg:
        IF M.ch = 7FX THEN (*DEL*)
          Erase(G); Graphics.Delete(G.graph); Mark(G, 1)
        ELSIF (M.ch >= 11X) & (M.ch <= 14X) THEN (*cursor keys*)
          IF G.ticked THEN x := 16 ELSE x := 1 END; y := 0;
          IF M.ch = 11X THEN (*left*) x := -x
          ELSIF M.ch = 12X THEN (*right*) (* no-op *)
          ELSIF M.ch = 13X THEN (*up*) y := x; x := 0
          ELSIF M.ch = 14X THEN (*down*) y := -x; x := 0 END;
          IF G.graph.sel # NIL THEN
            Erase(G); Graphics.Move(G.graph, x, y); Draw(G)
          END;
          IF G.marked THEN
            FlipMark(G.mark.x, G.mark.y); INC(G.mark.x, x); INC(G.mark.y, y); FlipMark(G.mark.x, G.mark.y);
            loc := G.mark.next;
            WHILE loc # NIL DO
              FlipMark(loc.x, loc.y); INC(loc.x, x); INC(loc.y, y); FlipMark(loc.x, loc.y);
              loc := loc.next
            END
          END;
          Mark(G, 1)
        ELSIF (M.ch >= 20X) & (M.ch < 7FX) THEN
          IF newcap # NIL THEN InsertChar(G, M.ch); Mark(G, 1)
          ELSIF G.marked THEN
            Defocus(G); Deselect(G); NewCaption(G, OberonCurCol, OberonCurFont); InsertChar(G, M.ch)
          END
        ELSIF (M.ch = 8X) & (newcap # NIL) THEN DeleteChar(G); Mark(G, 1)
        END
      | UpdateMsg:
          IF M.graph = G.graph THEN
            dM.f := G; dM.x := G.x; dM.y := G.y; dM.col := 0;
            IF M.id = drawobj THEN dM.mode := 0; M.obj.do.draw(M.obj, dM)
            ELSIF M.id = drawobjs THEN dM.mode := 1; M.obj.do.draw(M.obj, dM)
            ELSIF M.id = drawobjd THEN dM.mode := 3; M.obj.do.draw(M.obj, dM)
            ELSIF M.id = drawsel THEN  dM.mode := 0; Graphics.DrawSel(G.graph, dM)
            ELSIF M.id = drawnorm THEN dM.mode := 2; Graphics.DrawSel(G.graph, dM)
            ELSIF M.id = drawdel THEN dM.mode := 3; Graphics.DrawSel(G.graph, dM)
            END
          END
      | ChangedMsg:
          IF M.graph = G.graph THEN SetChangeMark(G, M.mode) END
      | SelQuery:
          IF (G.graph = Graphics.SelGraph) THEN M.f := G(Frame) END
      | FocusQuery: IF G.marked THEN M.f := G END
      | PosQuery: IF (G.X <= M.x) & (M.x < G.X1) & (G.Y <= M.y) & (M.y < G.Y1) THEN M.f := G END
      | DispMsg:
        DM := M;
        x := G.x + newcap.x + newcap.w; y := G.y + newcap.y;
        IF (DM.graph = G.graph) & (x >= G.X) & (x + DM.w < G.X1) & (y >= G.Y) & (y < G.Y1) THEN
          IF DM.pat # 0 THEN
            Display.InvertPattern(OberonCurCol, DM.pat, x + DM.x1, y + DM.y1)
          END;
          Display.InvertBlock(Display.white, x, y, DM.w, newcap.h)
        END
      | Oberon.ControlMsg:
          IF M.id = Oberon.neutralize THEN
            Oberon.RemoveMarks(G.X, G.Y, G.W, G.H); Defocus(G); DrawNorm(G); Graphics.Deselect(G.graph)
          ELSIF M.id = Oberon.defocus THEN Defocus(G)
          END
      | Oberon.CopyMsg: Oberon.RemoveMarks(G.X, G.Y, G.W, G.H); Defocus(G); NEW(G1); G1^ := G^; M.F := G1
      | MenuViewers.ModifyMsg: G.Y := M.Y; G.H := M.H; Restore(G)
      END
    END
  END Handle;

  PROCEDURE Store*(F: Frame; name: ARRAY OF CHAR);
  BEGIN Mark(F, 0); Graphics.WriteFile(F.graph, name)
  END Store;

  (*------------------- Draw Methods -----------------------*)

  PROCEDURE FillBlockClip*(F: Frame; col, x, y, w, h: INTEGER);
  BEGIN
    IF x < F.X THEN DEC(w, F.X-x); x := F.X END ;
    IF x+w >= F.X1 THEN w := F.X1 - x END ;
    IF y < F.Y THEN DEC(h, F.Y-y); y := F.Y END ;
    IF y+h >= F.Y1 THEN h := F.Y1 - y END ;
    Display.FillBlock(col, x, y, w, h)
  END FillBlockClip;

  PROCEDURE InvertBlockClip*(F: Frame; col, x, y, w, h: INTEGER);
  BEGIN
    IF x < F.X THEN DEC(w, F.X-x); x := F.X END ;
    IF x+w >= F.X1 THEN w := F.X1 - x END ;
    IF y < F.Y THEN DEC(h, F.Y-y); y := F.Y END ;
    IF y+h >= F.Y1 THEN h := F.Y1 - y END ;
    Display.InvertBlock(col, x, y, w, h)
  END InvertBlockClip;

  PROCEDURE ReplPatternClip*(F: Frame; col, patadr, x, y, w, h: INTEGER);
  BEGIN
    IF x < F.X THEN DEC(w, F.X-x); x := F.X END ;
    IF x+w >= F.X1 THEN w := F.X1 - x END ;
    IF y < F.Y THEN DEC(h, F.Y-y); y := F.Y END ;
    IF y+h >= F.Y1 THEN h := F.Y1 - y END ;
    Display.ReplPattern(col, patadr, x, y, w, h)
  END ReplPatternClip;

  PROCEDURE DrawLine(obj: Graphics.Object; VAR M: Graphics.Msg);
    (*M.mode = 0: draw according to state,
        = 1: normal -> selected,
        = 2: selected -> normal,
        = 3: erase*)
    VAR x, y, w, h, col: INTEGER; f: Frame;
  BEGIN
    CASE M OF DrawMsg:
      x := obj.x + M.x; y := obj.y + M.y; w := obj.w; h := obj.h; f := M.f;
      IF (x+w > f.X) & (x < f.X1) & (y+h > f.Y) & (y < f.Y1) THEN
        col := obj.col;
        IF (M.mode = 0) & obj.selected OR (M.mode = 1) THEN
          ReplPatternClip(f, col, Display.grey, x, y, w, h)
        ELSIF M.mode IN {0, 2} THEN FillBlockClip(f, col, x, y, w, h)
        ELSIF M.mode = 3 THEN FillBlockClip(f, Display.black, x, y, w, h)  (*erase*)
        END
      END
    END
  END DrawLine;

  PROCEDURE DrawCaption(obj: Graphics.Object; VAR M: Graphics.Msg);
    VAR x, y, dx, x0, x1, y0, y1, w, h, w1, h1, col: INTEGER;
      f: Frame;
      ch: CHAR; pat: INTEGER; fnt: Fonts.Font;
      R: Texts.Reader;
  BEGIN
    CASE M OF DrawMsg:
      x := obj.x + M.x; y := obj.y + M.y; w := obj.w; h := obj.h; f := M.f;
      IF (f.X <= x) & (x <= f.X1) & (f.Y <= y) & (y+h <= f.Y1) THEN
        IF x+w > f.X1 THEN w := f.X1-x END ;
        Texts.OpenReader(R, Graphics.T, obj(Graphics.Caption).pos); Texts.Read(R, ch);
        IF M.mode = 0 THEN
          IF ch >= ' ' THEN
            fnt := R.fnt; x0 := x; y0 := y - fnt.minY; col := obj.col;
            REPEAT Fonts.GetPat(fnt, ch, dx, x1, y1, w1, h1, pat);
              IF x0+x1+w1 <= f.X1 THEN
                IF (h1 # 0) THEN
                  Display.PaintPattern(col, pat, x0+x1, y0+y1)
                END;
                INC(x0, dx); Texts.Read(R, ch)
              ELSE ch := 0X
              END
            UNTIL ch < ' ';
            IF obj.selected THEN InvertBlockClip(f, Display.white, x, y, w, h) END
          END
        ELSIF M.mode IN {1, 2} THEN InvertBlockClip(f, Display.white, x, y, w, h)
        ELSIF M.mode = 3 THEN FillBlockClip(f, Display.black, x, y, w, h)
        END
      END
    END
  END DrawCaption;

  PROCEDURE DrawMacro(obj: Graphics.Object; VAR M: Graphics.Msg);
    VAR x, y, w, h: INTEGER;
      f: Frame; M1: DrawMsg;
  BEGIN
    CASE M OF DrawMsg:
      x := obj.x + M.x; y := obj.y + M.y; w := obj.w; h := obj.h; f := M.f;
      IF (x+w > f.X) & (x < f.X1) & (y+h > f.Y) & (y < f.Y1) THEN
        M1.x := x; M1.y := y;
        IF M.mode = 0 THEN
          M1.f := f; M1.col := obj.col; M1.mode := 0; Graphics.DrawMac(obj(Graphics.Macro).mac, M1);
          IF obj.selected THEN ReplPatternClip(f, Display.white, dotted, x, y, w, h) END
        ELSIF M.mode IN {1, 2} THEN ReplPatternClip(f, Display.white, dotted, x, y, w, h)
        ELSIF M.mode = 3 THEN FillBlockClip(f, Display.black, x, y, w, h)
        END
      END
    END
  END DrawMacro;

  (*---------------------------------------------------------------*)

  PROCEDURE Open*(G: Frame; graph: Graphics.Graph);
  BEGIN G.graph := graph; G.Xg := 0; G.Yg := 0; G.x := G.X; G.y := G.Y;
    G.col := Display.black; G.marked := FALSE;
    G.mark.next := NIL; G.ticked := TRUE; G.handle := Handle
  END Open;

  PROCEDURE SetFntCol*(fnt: Fonts.Font; col: INTEGER);
  BEGIN OberonCurFont := fnt; OberonCurCol := col
  END SetFntCol;

  PROCEDURE SwapCombine(lo, hi: INTEGER): INTEGER;
  BEGIN
  RETURN (lo DIV 100H) + (lo MOD 100H) * 100H + (hi DIV 100H) * 10000H + (hi MOD 100H) * 1000000H
  END SwapCombine;

  PROCEDURE MakeAddr*(x0l, x0h, x1l, x1h, x2l, x2h, x3l, x3h, x4l, x4h: INTEGER) : INTEGER;
  VAR pat: PPat;
  BEGIN NEW(pat); pat.next := LastPattern; LastPattern := pat;
    pat.x[0] := SwapCombine(x0l, x0h); pat.x[1] := SwapCombine(x1l, x1h);
    pat.x[2] := SwapCombine(x2l, x2h); pat.x[3] := SwapCombine(x3l, x3h);
    pat.x[4] := SwapCombine(x4l, x4h)
  RETURN ADR(pat.x[0])
  END MakeAddr;

BEGIN CL := 0; OberonCurCol := 15; OberonCurFont := Fonts.Default;
  Texts.OpenWriter(W);
  NEW(TBuf); Texts.OpenBuf(TBuf);
  tack := MakeAddr(0707H, 4122H, 1408H, 1422H, 4100H,0,0,0,0,0);
  dotted := MakeAddr(2004H,  0000H, 1111H, 1111H, 0000H, 0000H, 0000H, 0000H, 0000H, 0000H);
  dotted1 := MakeAddr(2004H, 0000H, 1111H, 1111H, 0000H, 0000H, 4444H, 4444H, 0000H, 0000H);
  Crosshair.patadr := Display.cross;
  Crosshair.DX := 7; Crosshair.DY := 7;
  Graphics.InstallDrawMethods(DrawLine, DrawCaption, DrawMacro)
END GraphicFrames.
