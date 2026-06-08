(* Source: https://github.com/Spirit-of-Oberon/wirth-personal
   Filename: people.inf.ethz.ch/wirth/ProjectOberon/Sources/DisplayC.mod.txt
   License: /licenses/oberon.txt *)
MODULE Display;
LOADFORTH oberon/display.pre

CONST black* = 0; white* = 15; (*black = background*)
  BPP = 4; (* color bits per pixel, <=4 *)
  COLORS = VAL(INTEGER, {BPP}); (* nbr of colors, =LSL(1, BPP) *)
  PP8 = 8 DIV BPP; (* nbr of pixels per 8 bit *)
  PP32 = PP8 * 4; (* nbr of pixels per 32 bit *)
  COLMASK = 11111111H; (* BPP = 4, PP32 = 8 *)

VAR Base*, Width*, Height*, Span*, Palette*: INTEGER;
    (*a pattern is an array of bytes; the first is its width (< 32), the second
      its height, the rest the raster*)
    arrow*, star*, hook*, updown*, block*, cross*, grey*: INTEGER;

PROCEDURE Dot*(col, x, y: INTEGER);
VAR a: INTEGER; pix, color: SET;
BEGIN
  a := Base + y * Span + (x DIV PP32)*4;
  x := x MOD PP32 * BPP;
  color := VAL(SET, LSL(col MOD COLORS, x));
  GET(a, pix);
  PUT(a, pix - VAL(SET, LSL(white, x)) + color);
  DUSK.Damage(1, 1, y, x)
END Dot;

PROCEDURE CopyBlock*(sx, sy, w, h, dx, dy: INTEGER);
VAR sa, da, sa0, sa1, d, len: INTEGER;
    u0, u1, u3, v0, v1, v3, n: INTEGER;
    end, step: INTEGER;
    src, dst, spill: SET;
    m0, m1, m2, m3: SET;
BEGIN
  u0 := sx DIV PP32 * 4; v0 := dx DIV PP32 * 4;
  u1 := sx MOD PP32 * BPP; v1 := dx MOD PP32 * BPP;
  u3 := (sx+w) MOD PP32 * BPP; v3 := (dx+w) MOD PP32 * BPP;
  sa := sy * Span + u0 + Base; da := dy * Span + v0 + Base;
  len := (sx+w) DIV PP32 * 4 - u0;
  d := da - sa; n := u1 - v1; (* displacement in words and bits *)
  m0 := {v1 .. 31}; m2 := {v3 .. 31}; m3 := m0 / m2;
  IF n >= 0 THEN m1 := {n .. 31} ELSE m1 := {-n .. 31} END;
  IF d >= 0 THEN (* copy up, scan down *)
    sa0 := sa + (h-1)*Span; end := sa - Span; step := -Span
  ELSE (* copy down, scan up *)
    sa0 := sa; end := sa + h*Span; step := Span
  END;
  WHILE sa0 # end DO
    IF n >= 0 THEN (* shift right *)
      IF v1 + w * BPP < 32 THEN
        GET(sa0, src); src := ROR(src, n);
        GET(sa0+d, dst);
        PUT(sa0+d, (src * m3) + (dst - m3))
      ELSE
        GET(sa0+len, src); src := ROR(src, n);
        GET(sa0+len+d, dst);
        PUT(sa0+len+d, (dst * m2) + (src - m2));
        spill := src - m1;
        FOR sa1 := sa0 + len-4 TO sa0+4  BY -4 DO
          GET(sa1, src); src := ROR(src, n);
          PUT(sa1+d, spill + (src * m1));
          spill := src - m1
        END;
        GET(sa0, src); src := ROR(src, n);
        GET(sa0+d, dst);
        PUT(sa0+d, (src * m0) + (dst - m0))
      END
    ELSE (* shift left *)
      GET(sa0, src); src := ROR(src, n);
      GET(sa0+d, dst);
      IF v1 + w * BPP < 32 THEN
        PUT(sa0+d, (dst - m3) + (src * m3))
      ELSE
        PUT(sa0+d, (dst - m0) + (src * m0));
        spill := src - m1;
        FOR sa1 := sa0 + 4 TO sa0 + len-4 BY 4 DO
          GET(sa1, src); src := ROR(src, n);
          PUT(sa1+d, spill + (src * m1));
          spill := src - m1
        END;
        GET(sa0+len, src); src := ROR(src, n);
        GET(sa0+len+d, dst);
        PUT(sa0+len+d, (src - m2) + (dst * m2))
      END
    END;
    INC(sa0, step)
  END;
  DUSK.Damage(h, w, dy, dx)
END CopyBlock;

PROCEDURE ReplPattern*(col, patadr, x, y, w, h: INTEGER);
(* BW pattern width = 32, fixed; pattern starts at patadr+4 *)
(* Color pattern width = 8, fixed; pattern starts at patadr+4 *)
(* NOTE that BW patterns will be converted in place on first call. *)
VAR al, ar, a0, a1: INTEGER;
    pta0, pta1: INTEGER; (*pattern addresses*)
    pw, ph: BYTE;
    left, right, mid, pix, pat, color: SET;
BEGIN
  al := Base + y*Span; GET(patadr+1, ph); GET(patadr, pw);
  IF pw = 32 THEN pw := 8; PUT(patadr, pw);
    FOR a0 := 1 TO ph DO
      GET(patadr+4*a0, pix);
      pat := {};
      FOR ar := 0 TO 3 DO
        IF (pix * {ar} # {}) THEN pat := pat + {ar*4 .. ar*4+3} END;
        IF (pix * {12+ar} # {}) THEN pat := pat + {(ar+4)*4 .. (ar+4)*4+3} END
      END;
      PUT(patadr+4*a0, pat)
    END
  END;
  ASSERT(pw = 8); (* width MUST be 8 *)
  pta0 := patadr+4; pta1 := ph*4 + pta0;
  (* copy "col" to all PP32 pixels in a word *)
  color := VAL(SET, (col MOD COLORS) * COLMASK);
  INC(w, x-1);
  ar := (w DIV PP32) *4 + al; al := (x DIV PP32) *4 + al;
  left := { ((x MOD PP32) * BPP) .. 31};
  right := {0 .. ((w MOD PP32) * BPP + (BPP-1))};
  IF ar = al THEN
    mid := left * right; a1 := al;
    WHILE a1 <= al + (h-1) * Span DO
      GET(pta0, pat); pat := pat * color;
      GET(a1, pix); PUT(a1, (pix - mid) + (pix/pat * mid));
      INC(pta0, 4);
      IF pta0 = pta1 THEN pta0 := patadr+4 END;
      INC(a1, Span)
    END
  ELSIF ar > al THEN
    a0 := al;
    WHILE a0 <= al + (h-1) * Span DO
      GET(pta0, pat); pat := pat * color;
      GET(a0, pix); PUT(a0, (pix - left) + (pix/pat * left));
      FOR a1 := a0+4 TO ar-4 BY 4 DO GET(a1, pix); PUT(a1, pix/pat) END;
      GET(ar, pix); PUT(ar, (pix - right) + (pix/pat * right));
      INC(pta0, 4); INC(ar, Span); INC(a0, Span);
      IF pta0 = pta1 THEN pta0 := patadr+4 END
    END
  END;
  DUSK.Damage(h, w, y, x)
END ReplPattern;

PROCEDURE GetPalette*(col: INTEGER; VAR val: INTEGER): BOOLEAN;
VAR result: BOOLEAN;
BEGIN result := FALSE;
  IF (Palette # 0) & (col < 16) THEN
    GET(Palette + col * 4, val);
    result := TRUE;
  END;
RETURN result
END GetPalette;

PROCEDURE SetPalette*(col, val: INTEGER);
BEGIN
  IF (Palette # 0) & (col < 16) THEN PUT(Palette + col * 4, val) END
END SetPalette;

BEGIN
  GetDisplayParams(Base, Palette, Width, Height);
  Span := Width DIV 2;
  GetSysPatterns(arrow, star, hook, updown, block, cross, grey);
END Display.
