(* This is Virgil's personal helpers module. It's meant as an example.
   You should have your own. *)

MODULE V;
IMPORT Texts;

PROCEDURE Hello*;
VAR R: Texts.Reader; ch: CHAR;
BEGIN
  IF Texts.Sel # NIL THEN
    Texts.OpenReader(R, Texts.Sel, Texts.SelBegin);
    Texts.ReadBackwards(R, ch); DUSK.Emit(ch);
    Texts.ReadBackwards(R, ch); DUSK.Emit(ch);
    Texts.Read(R, ch); DUSK.Emit(ch);
    Texts.ReadBackwards(R, ch); DUSK.Emit(ch);
    Texts.ReadBackwards(R, ch); DUSK.Emit(ch);
  END
END Hello;

END V.
