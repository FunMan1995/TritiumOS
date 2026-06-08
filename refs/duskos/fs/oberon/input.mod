(* Source: https://github.com/Spirit-of-Oberon/wirth-personal
   Filename: people.inf.ethz.ch/wirth/ProjectOberon/Sources/Input.mod.txt
   License: /licenses/oberon.txt *)
MODULE Input;
IMPORT Display (* for DUSK.drawbuf *);
LOADFORTH oberon/input.pre

VAR lastNKC: INTEGER;

PROCEDURE Peek();
BEGIN
  lastNKC := DUSK.PeekKbd();
END Peek;

PROCEDURE Available*(): BOOLEAN;
VAR ret: BOOLEAN;
BEGIN
  Peek(); ret := lastNKC # 0;
  RETURN ret
END Available;

PROCEDURE TranslateNKC*(nkc: INTEGER): CHAR;
VAR ch: CHAR;
BEGIN
  ch := DUSK.NKCtoCHAR(nkc);
  IF ch = 0DX THEN ch := 0AX END;
  IF (ch = 0X) & (nkc # 0) THEN
    IF (nkc >= 81H) & (nkc <= 86H) THEN
      ch := CHR(26 - (nkc - 81H))
    ELSIF (nkc >= 0A9H) & (nkc <= 0ACH) THEN
      ch := CHR(17 + (nkc - 0A9H))
    ELSIF nkc = 0A4H THEN ch := 7FX
    END
  END;
  RETURN ch
END TranslateNKC;

PROCEDURE Read*(VAR ch: CHAR);
BEGIN
  WHILE lastNKC = 0 DO lastNKC := DUSK.PeekKbd() END;
  ch := TranslateNKC(lastNKC);
  lastNKC := 0
END Read;

PROCEDURE Mouse*(VAR keys: SET; VAR x, y: INTEGER);
BEGIN DUSK.drawbuf; DUSK.Mouse(keys, x, y)
END Mouse;

PROCEDURE SetMouseLimits*(w, h: INTEGER);
BEGIN DUSK.SetMouseLimits(w, h);
END SetMouseLimits;

END Input.
