MODULE Clipboard;
  IMPORT Texts, Viewers, TextFrames, Oberon;

  LOADFORTH oberon/clipboar.pre

  VAR bufAddr, lenAddr: INTEGER;

  PROCEDURE Copy(T: Texts.Text; beg, end: INTEGER);
    VAR R: Texts.Reader;
      ch: CHAR; i, len: INTEGER;
  BEGIN
    Texts.OpenReader(R, T, beg); len := end - beg;
    EnsureClipboardSize(len, bufAddr);
    FOR i := 0 TO len - 1 DO
      Texts.Read(R, ch);
      PUT(bufAddr + i, ch);
    END;
    PUT(lenAddr, len); HostPaste()
  END Copy;

  PROCEDURE CopySelection*;
    VAR T: Texts.Text;
      beg, end: INTEGER;
  BEGIN
    TextFrames.GetSelection(T, beg, end); Copy(T, beg, end)
  END CopySelection;

  PROCEDURE CopyViewer*;
    VAR V: Viewers.Viewer;
      F: TextFrames.Frame;
  BEGIN
    V := Oberon.MarkedViewer();
    IF (V # NIL) & (V.dsc # NIL) & (V.dsc.next IS TextFrames.Frame) THEN
      F := V.dsc.next(TextFrames.Frame);
      Copy(F.text, 0, F.text.len)
    END
  END CopyViewer;

  PROCEDURE Paste*;
    VAR W: Texts.Writer;
      V: Viewers.Viewer;
      F: TextFrames.Frame;
      len, i: INTEGER;
      ch: CHAR;
  BEGIN
    V := Oberon.FocusViewer; HostCopy(); GetClipboardAddr(bufAddr, lenAddr);
    IF (V # NIL) & (V.dsc # NIL) & (V.dsc.next IS TextFrames.Frame) THEN
      GET(lenAddr, len);
      IF len > 0 THEN
        Texts.OpenWriter(W);
        FOR i := 0 TO len - 1 DO
          GET(bufAddr + i, ch);
          Texts.Write(W, ch)
        END;
        F := V.dsc.next(TextFrames.Frame);
        Texts.Insert(F.text, TextFrames.carloc.pos, W.buf);
        TextFrames.SetCaret(F, TextFrames.carloc.pos + len)
      END
    END
  END Paste;

BEGIN GetClipboardAddr(bufAddr, lenAddr)
END Clipboard.
