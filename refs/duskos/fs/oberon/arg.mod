MODULE Arg;
IMPORT Texts, TextFrames, Oberon;

PROCEDURE GetArg*(VAR S: Texts.Scanner);
VAR T: Texts.Text; beg, end: INTEGER;
BEGIN Texts.OpenScanner(S, Oberon.Par.text, Oberon.Par.pos); Texts.Scan(S);
  IF (S.class = Texts.Char) & (S.c = '^') THEN
    TextFrames.GetSelection(T, beg, end);
    IF T # NIL THEN Texts.OpenScanner(S, T, beg); Texts.Scan(S) END
  END
END GetArg;

END Arg.