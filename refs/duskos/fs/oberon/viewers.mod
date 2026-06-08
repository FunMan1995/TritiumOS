(* Source: https://github.com/Spirit-of-Oberon/wirth-personal
   Filename: people.inf.ethz.ch/wirth/ProjectOberon/Sources/Viewers.mod.txt
   License: /licenses/oberon.txt *)
MODULE Viewers;
IMPORT Display;

CONST restore* = 0; modify* = 1; suspend* = 2; (*message ids*)
  inf = 65535;

TYPE
  Frame* = POINTER TO FrameDesc;
  FrameMsg* = RECORD END;
  Handler* = PROCEDURE (F: Frame; VAR M: FrameMsg);
  FrameDesc* = RECORD
    next, dsc: Frame;
    X, Y, W, H: INTEGER;
    handle: Handler
  END;

  Viewer* = POINTER TO ViewerDesc;
  ViewerDesc* = RECORD (FrameDesc) state: INTEGER END;

  (*state > 1: displayed; state = 1: filler; state = 0: closed;
    state < 0: suspended*)

  ViewerMsg* = RECORD (FrameMsg)
    id: INTEGER;
    X, Y, W, H: INTEGER;
    state: INTEGER
  END;

  Track = POINTER TO TrackDesc;
  TrackDesc = RECORD (ViewerDesc) under: Track END;

VAR curW*, minH*, DH: INTEGER;
  FillerTrack: Track;
  FillerViewer, backup: Viewer; (*last closed viewer*)

PROCEDURE Open*(V: Viewer; X, Y: INTEGER);
VAR T, u, v: Frame; M: ViewerMsg;
BEGIN
  IF (V.state = 0) & (X < inf) THEN
    Y := MIN(Y, DH);
    T := FillerTrack.next;
    WHILE X >= T.X + T.W DO T := T.next END;
    u := T.dsc; v := u.next;
    WHILE Y > v.Y + v.H DO u := v; v := u.next END;
    Y := MAX(Y, v.Y + minH);
    IF (v.next.Y # 0) & (Y > v.Y + v.H - minH) THEN
      V.X := T.X; V.W := T.W; V.Y := v.Y; V.H := v.H;
      M.id := suspend; M.state := 0;
      v.handle(v, M);
      v(Viewer).state := 0;
      V.next := v.next; u.next := V; V.state := 2
    ELSE V.X := T.X; V.W := T.W; V.Y := v.Y; V.H := Y - v.Y;
      M.id := modify; M.Y := Y; M.H := v.Y + v.H - Y;
      v.handle(v, M); v.Y := M.Y; v.H := M.H;
      V.next := v; u.next := V; V.state := 2
    END
  END
END Open;

PROCEDURE Change*(V: Viewer; Y: INTEGER);
VAR v: Frame; M: ViewerMsg;
BEGIN
  IF V.state > 1 THEN
    Y := MIN(Y, DH);
    v := V.next;
    IF (v.next.Y # 0) & (Y > v.Y + v.H - minH) THEN Y := v.Y + v.H - minH END;
    IF Y >= V.Y + minH THEN
      M.id := modify; M.Y := Y; M.H := v.Y + v.H - Y;
      v.handle(v, M); v.Y := M.Y; v.H := M.H; V.H := Y - V.Y
    END
  END
END Change;

PROCEDURE RestoreTrack(S: Track);
VAR T: Track; t, v: Frame; M: ViewerMsg;
BEGIN t := S.next;
  WHILE t.next # S DO t := t.next END;
  T := S.under;
  WHILE T.next # NIL DO T := T.next END;
  t.next := S.under; T.next := S.next; M.id := restore;
  REPEAT t := t.next; v := t.dsc;
    REPEAT v := v.next; v.handle(v, M); v(Viewer).state := - v(Viewer).state
    UNTIL v = t.dsc
  UNTIL t = T
END RestoreTrack;

PROCEDURE Close*(V: Viewer);
VAR T: Track; U: Frame; M: ViewerMsg;
BEGIN
  IF V.state > 1 THEN
    U := V.next; T := FillerTrack;
    REPEAT T := T.next(Track) UNTIL V.X < T.X + T.W;
    IF (T.under = NIL) OR (U.next # V) THEN
      M.id := suspend; M.state := 0;
      V.handle(V, M); V.state := 0; backup := V;
      M.id := modify; M.Y := V.Y; M.H := V.H + U.H;
      U.handle(U, M); U.Y := M.Y; U.H := M.H;
      WHILE U.next # V DO U := U.next END;
      U.next := V.next
    ELSE (*close track*)
      M.id := suspend; M.state := 0;
      V.handle(V, M); V.state := 0; backup := V;
      U.handle(U, M); U(Viewer).state := 0;
      RestoreTrack(T)
    END
  END
END Close;

PROCEDURE Recall*(VAR V: Viewer);
BEGIN V := backup
END Recall;

PROCEDURE This*(X, Y: INTEGER): Viewer;
VAR T, V: Frame;
BEGIN
  IF (X < inf) & (Y < DH) THEN
    T := FillerTrack;
    REPEAT T := T.next UNTIL X < T.X + T.W;
    V := T.dsc;
    REPEAT V := V.next UNTIL Y < V.Y + V.H
  ELSE V := NIL
  END ;
  RETURN V(Viewer)
END This;

PROCEDURE Locate*(X, H: INTEGER; VAR fil, bot, alt, max: Frame);
VAR T, V: Frame;
BEGIN
  IF X < inf THEN
    T := FillerTrack;
    REPEAT T := T.next UNTIL X < T.X + T.W;
    fil := T.dsc; bot := fil.next;
    IF bot.next # fil THEN
      alt := bot.next; V := alt.next;
      WHILE (V # fil) & (alt.H < H) DO
        IF V.H > alt.H THEN alt := V END;
        V := V.next
      END
    ELSE alt := bot
    END;
    max := T.dsc; V := max.next;
    WHILE V # fil DO
      IF V.H > max.H THEN max := V END;
      V := V.next
    END
  END
END Locate;

PROCEDURE InitTrack*(W, H: INTEGER; Filler: Viewer);
VAR S: Frame; T: Track;
BEGIN
  IF Filler.state = 0 THEN
    Filler.X := curW; Filler.W := W; Filler.Y := 0; Filler.H := H;
    Filler.state := 1; Filler.next := Filler;
    NEW(T); T.X := curW; T.W := W; T.Y := 0; T.H := H;
    T.dsc := Filler; T.under := NIL;
    FillerViewer.X := curW + W; FillerViewer.W := inf - FillerViewer.X;
    FillerTrack.X := FillerViewer.X; FillerTrack.W := FillerViewer.W;
    S := FillerTrack;
    WHILE S.next # FillerTrack DO S := S.next END;
    S.next := T; T.next := FillerTrack; curW := curW + W
  END
END InitTrack;

PROCEDURE OpenTrack*(X, W: INTEGER; Filler: Viewer);
VAR newT: Track; S, T, t, v: Frame; M: ViewerMsg; v0: Viewer;
BEGIN
  IF (X < inf) & (Filler.state = 0) THEN
    S := FillerTrack; T := S.next;
    WHILE X >= T.X + T.W DO S := T; T := S.next END;
    WHILE X + W > T.X + T.W DO T := T.next END;
    M.id := suspend; t := S;
    REPEAT t := t.next; v := t.dsc;
      REPEAT
        v := v.next; M.state := -v(Viewer).state; v.handle(v, M);
        v(Viewer).state := M.state
      UNTIL v = t.dsc
    UNTIL t = T;
    Filler.X := S.next.X; Filler.W := T.X + T.W - S.next.X;
    Filler.Y := 0; Filler.H := DH;
    Filler.state := 1; Filler.next := Filler;
    NEW(newT); newT.X := Filler.X; newT.W := Filler.W;
    newT.Y := 0; newT.H := DH;
    newT.dsc := Filler; newT.under := S.next(Track); S.next := newT;
    newT.next := T.next; T.next := NIL
  END
END OpenTrack;

PROCEDURE CloseTrack*(X: INTEGER);
VAR T: Track; V: Frame; M: ViewerMsg;
BEGIN
  IF X < inf THEN
    T := FillerTrack;
    REPEAT T := T.next(Track) UNTIL X < T.X + T.W;
    IF T.under # NIL THEN
      M.id := suspend; M.state := 0; V := T.dsc;
      REPEAT V := V.next; V.handle(V, M); V(Viewer).state := 0 UNTIL V = T.dsc;
      RestoreTrack(T)
    END
  END
END CloseTrack;

PROCEDURE Broadcast*(VAR M: FrameMsg);
VAR T, V: Frame;
BEGIN T := FillerTrack.next;
  WHILE T # FillerTrack DO
    V := T.dsc; 
    REPEAT V := V.next; V.handle(V, M) UNTIL V = T.dsc;
    T := T.next
  END
END Broadcast;

PROCEDURE AdjustSize(V: Frame; ow, oh, nw, nh: INTEGER);
VAR oldX: INTEGER;
BEGIN oldX := V.X;
  V.X := (((V.X * nw) DIV ow + nw DIV 16) DIV (nw DIV 8)) * (nw DIV 8);
  IF V.W = inf - oldX THEN
    V.W := inf - V.X
  ELSIF V.W # inf THEN
    V.W := (((V.W * nw) DIV ow + nw DIV 16) DIV (nw DIV 8)) * (nw DIV 8)
  END;
  V.Y := (V.Y * nh) DIV oh;
  V.H := (V.H * nh) DIV oh;
END AdjustSize;

PROCEDURE UpdateScreenSize*(ow, oh, nw, nh: INTEGER);
VAR T, V: Frame; M: ViewerMsg;
BEGIN
  M.id := suspend;
  Broadcast(M);
  T := FillerTrack.next;
  WHILE T # FillerTrack DO
    AdjustSize(T, ow, oh, nw, nh);
    V := T.dsc;
    REPEAT V := V.next; AdjustSize(V, ow, oh, nw, nh) UNTIL V = T.dsc;
    T := T.next
  END;
  DH := nh;
  FillerViewer.H := DH;
  FillerTrack.H := DH;
  M.id := restore;
  Broadcast(M);
END UpdateScreenSize;

BEGIN
  backup := NIL; curW := 0; minH := 1; DH := Display.Height;
  NEW(FillerViewer); FillerViewer.X := 0; FillerViewer.W := inf;
  FillerViewer.Y := 0; FillerViewer.H := DH;
  FillerViewer.next := FillerViewer;
  NEW(FillerTrack);
  FillerTrack.X := 0; FillerTrack.W := inf;
  FillerTrack.Y := 0; FillerTrack.H := DH;
  FillerTrack.dsc := FillerViewer; FillerTrack.next := FillerTrack
END Viewers.
